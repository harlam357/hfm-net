/*
 * HFM.NET - XML Generation Helper Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using HFM.Framework;
using HFM.Instrumentation;
using HFM.Helpers;

namespace HFM.Instances
{
   public static class XMLGen
   {
      /// <summary>
      /// Generate HTML Pages.
      /// </summary>
      /// <param name="FolderPath">Folder Path to place Generated Pages.</param>
      /// <param name="Instances">Client Instance Array.</param>
      /// <exception cref="ArgumentException">Throws if FolderPath is Null or Empty or if Instances is a Zero Length Array.</exception>
      public static void DoHtmlGeneration(string FolderPath, ClientInstance[] Instances)
      {
         if (String.IsNullOrEmpty(FolderPath)) throw new ArgumentException("Argument 'FolderPath' cannot be a null or empty string.");
         if (Instances == null) throw new ArgumentNullException("Instances", "Argument 'Instances' cannot be null.");
         if (Instances.Length == 0) throw new ArgumentException("Argument 'Instances' cannot be a zero length array.");
         
         StreamWriter sw = null;

         try
         {
            // Get instance totals   
            InstanceTotals totals = InstanceCollectionHelpers.GetInstanceTotals(Instances);

            // Generate the index page XML
            XmlDocument OverviewXml = CreateOverviewXml(totals);

            // Generate the index page
            sw = new StreamWriter(Path.Combine(FolderPath, "index.html"), false);
            sw.Write(XMLOps.Transform(OverviewXml, GetXsltFileName(Preference.WebOverview)));
            sw.Close();

            // Generate the mobile index page
            sw = new StreamWriter(Path.Combine(FolderPath, "mobile.html"), false);
            sw.Write(XMLOps.Transform(OverviewXml, GetXsltFileName(Preference.WebMobileOverview)));
            sw.Close();

            List<string> duplicateUserID = new List<string>(Instances.Length);
            List<string> duplicateProjects = new List<string>(Instances.Length);
            InstanceCollectionHelpers.FindDuplicates(duplicateUserID, duplicateProjects, Instances);

            // Generate the summart page XML
            XmlDocument SummaryXml = CreateSummaryXml(OverviewXml, Instances, duplicateUserID, duplicateProjects);

            // Generate the summary page
            sw = new StreamWriter(Path.Combine(FolderPath, "summary.html"), false);
            sw.Write(XMLOps.Transform(SummaryXml, GetXsltFileName(Preference.WebSummary)));
            sw.Close();

            // Generate the mobile summary page
            sw = new StreamWriter(Path.Combine(FolderPath, "mobilesummary.html"), false);
            sw.Write(XMLOps.Transform(SummaryXml, GetXsltFileName(Preference.WebMobileSummary)));
            sw.Close();

            string InstanceXslt = GetXsltFileName(Preference.WebInstance);
            // Generate a page per instance
            foreach (ClientInstance instance in Instances)
            {
               sw = new StreamWriter(Path.Combine(FolderPath, String.Concat(instance.InstanceName, ".html")), false);
               sw.Write(XMLOps.Transform(CreateInstanceXml(instance), InstanceXslt));
               sw.Close();
            }
         }
         finally
         {
            if (sw != null)
            {
               sw.Close();
            }
         }
      }
      
      private static string GetXsltFileName(Preference p)
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         string xslt = Prefs.GetPreference<string>(p);

         if (Path.IsPathRooted(xslt))
         {
            if (File.Exists(xslt))
            {
               return xslt;
            }
         }
         else
         {
            string XsltFileName = Path.Combine(Path.Combine(Prefs.ApplicationPath, "XSL"), xslt);
            if (File.Exists(XsltFileName))
            {
               return XsltFileName;
            }
         }
         
         throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
            "XSLT File '{0}' cannot be found.", xslt));
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Server">Server Name</param>
      /// <param name="FtpPath">Path from FTP Server Root</param>
      /// <param name="Username">FTP Server Username</param>
      /// <param name="Password">FTP Server Password</param>
      /// <param name="Instances">Client Instance Collection</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      public static void DoWebFtpUpload(string Server, string FtpPath, string Username, string Password, ICollection<ClientInstance> Instances, FtpType ftpMode)
      {
         // Time FTP Upload Conversation - Issue 52
         DateTime Start = HfmTrace.ExecStart;

         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();

         NetworkOps net = new NetworkOps();

         try
         {
            net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.Combine(Prefs.ApplicationPath, "CSS"), Prefs.GetPreference<string>(Preference.CssFile)), Username, Password, ftpMode);
            net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "index.html"), Username, Password, ftpMode);
            net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "summary.html"), Username, Password, ftpMode);
            net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "mobile.html"), Username, Password, ftpMode);
            net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "mobilesummary.html"), Username, Password, ftpMode);

            foreach (ClientInstance Instance in Instances)
            {
               net.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(),
                                   Path.ChangeExtension(Instance.InstanceName, ".html")),
                                   Username, Password, ftpMode);

               string CachedFAHlogPath = Path.Combine(Prefs.CacheDirectory, Instance.CachedFAHLogName);
               if (Prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog) && File.Exists(CachedFAHlogPath))
               {
                  net.FtpUploadHelper(Server, FtpPath, CachedFAHlogPath, Username, Password, ftpMode);
               }
            }
         }
         finally
         {
            // Time FTP Upload Conversation - Issue 52
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
         }
      }
   
      /// <summary>
      /// Generates XML Data for a Client Instance page.
      /// </summary>
      /// <param name="Instance">Instance to use as data source</param>
      /// <returns>Generated Xml Document</returns>
      private static XmlDocument CreateInstanceXml(ClientInstance Instance)
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
      
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(Prefs.ApplicationPath, "XML"), "Instance.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         //    <UnitInfo>
         //        <FramesComplete>35</FramesComplete>
         //        <PercentComplete>35</PercentComplete>
         //        <TimePerFrame>1h 15m 44s</TimePerFrame>
         //        <EstPPD>82.36</EstPPD>
         //        <EstPPW>576.54</EstPPW>
         //        <EstUPD>0.46</EstUPD>
         //        <EstUPW>3.77</EstUPW>
         //        <CompletedProjects>5</CompletedProjects>
         //        <FailedProjects>0</FailedProjects>
         //        <TotalProjects>243</TotalProjects>
         //        <DownloadTime>10 August 09:37:33</DownloadTime>
         //        <ExpectedCompletionDate>16 August 2006 1:46 am</ExpectedCompletionDate>
         //    </UnitInfo>

         xmlData.SetAttribute("Name", Instance.InstanceName);
         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XMLOps.setXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", Instance.FramesComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", Instance.PercentComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", Instance.TimePerFrame.Hours, Instance.TimePerFrame.Minutes, Instance.TimePerFrame.Seconds));

         string PpdFormatString = Prefs.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstPPD", String.Format("{0:" + PpdFormatString + "}", Instance.PPD));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstPPW", String.Format("{0:" + PpdFormatString + "}", Instance.PPD * 7));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstUPD", String.Format("{0:0.00}", Instance.UPD));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstUPW", String.Format("{0:0.00}", Instance.UPD * 7));
         XMLOps.setXmlNode(xmlData, "UnitInfo/CompletedProjects", Instance.NumberOfCompletedUnitsSinceLastStart.ToString());
         XMLOps.setXmlNode(xmlData, "UnitInfo/FailedProjects", Instance.NumberOfFailedUnitsSinceLastStart.ToString());
         XMLOps.setXmlNode(xmlData, "UnitInfo/TotalProjects", Instance.TotalUnits.ToString());

         if (Instance.CurrentUnitInfo.DownloadTimeUnknown)
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", "Unknown");
         }
         else
         {
            DateTime DownloadTime = Instance.CurrentUnitInfo.DownloadTime;
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", String.Format(CultureInfo.CurrentCulture, "{0} at {1}", DownloadTime.ToLongDateString(), DownloadTime.ToLongTimeString()));
         }
         
         if (Instance.ETA.Equals(TimeSpan.Zero))
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", "Unknown");
         }
         else
         {
            DateTime CompleteTime = DateTime.Now.Add(Instance.ETA);
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", String.Format(CultureInfo.CurrentCulture, "{0} at {1}", CompleteTime.ToLongDateString(), CompleteTime.ToLongTimeString()));
         }

         //    <Protein>
         //        <ProjectNumber>1814</ProjectNumber>
         //        <ServerIP>171.65.103.158 </ServerIP>
         //        <WorkUnit>p1814_Collagen_POG10more_refolding</WorkUnit>
         //        <NumAtoms>1116</NumAtoms>
         //        <PreferredDays>30</PreferredDays>
         //        <MaxDays>44</MaxDays>
         //        <Credit>153</Credit>
         //        <KFactor>0</KFactor>
         //        <Frames>100</Frames>
         //        <Core>Amber</Core>
         //        <Description>Project Description</Description>
         //        <Contact>spark7</Contact>
         //    </Protein>

         IProteinCollection proteinCollection = InstanceProvider.GetInstance<IProteinCollection>();

         IProtein p = proteinCollection.GetNewProtein();
         if (proteinCollection.ContainsKey(Instance.CurrentUnitInfo.ProjectID))
         {
            p = proteinCollection[Instance.CurrentUnitInfo.ProjectID];
         }
         XMLOps.setXmlNode(xmlData, "Protein/ProjectNumber", p.ProjectNumber.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/ServerIP", p.ServerIP);
         XMLOps.setXmlNode(xmlData, "Protein/WorkUnit", p.WorkUnitName);
         XMLOps.setXmlNode(xmlData, "Protein/NumAtoms", p.NumAtoms.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/PreferredDays", p.PreferredDays.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/MaxDays", p.MaxDays.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Credit", p.Credit.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/KFactor", p.KFactor.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Frames", p.Frames.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Core", p.Core);
         XMLOps.setXmlNode(xmlData, "Protein/Description", p.Description);
         XMLOps.setXmlNode(xmlData, "Protein/Contact", p.Contact);
         
         StringBuilder sb = new StringBuilder();
         foreach (ILogLine line in Instance.DataAggregator.CurrentLogLines)
         {
            sb.Append(line.LineRaw);
            sb.Append("<BR>");
            sb.Append(Environment.NewLine);
         }

         XMLOps.setXmlNode(xmlData, "UnitLog/Text", sb.ToString());
         if (Prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
         {
            XMLOps.setXmlNode(xmlData, "UnitLog/FullLogFile", Instance.CachedFAHLogName);
         }
         else
         {
            XMLOps.setXmlNode(xmlData, "UnitLog/FullLogFile", String.Empty);
         }

         //    <LastUpdatedDate>10 August 2006</LastUpdatedDate>
         //    <LastUpdatedTime>9:25:23 pm</LastUpdatedTime>

         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         //    <LastRetrievedDate>10 August 2006</LastRetrievedDate>
         //    <LastRetrievedTime>9:25:23 pm</LastRetrievedTime>

         XMLOps.setXmlNode(xmlData, "LastRetrievedDate", Instance.LastRetrievalTime.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastRetrievedTime", Instance.LastRetrievalTime.ToLongTimeString());

         return xmlDoc;
      }

      /// <summary>
      /// Generates XML Data for the Summary page.
      /// </summary>
      /// <param name="xmlDoc">XML Document to Append Summary Information</param>
      /// <param name="Instances">Array of folding instances</param>
      /// <param name="duplicateUserID">List of Duplicate UserID strings</param>
      /// <param name="duplicateProjects">List of Duplicate ProjectRCG strings</param>
      /// <returns>Generated Xml Document</returns>
      private static XmlDocument CreateSummaryXml(XmlDocument xmlDoc, ClientInstance[] Instances, ICollection<string> duplicateUserID, ICollection<string> duplicateProjects)
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         Array.Sort(Instances, delegate(ClientInstance instance1, ClientInstance instance2)
                               {
                                  // Make the HTML Summary respect the 'List Office Clients Last' option - Issue 78
                                  if (Prefs.GetPreference<bool>(Preference.OfflineLast))
                                  {
                                     if (instance1.Status.Equals(ClientStatus.Offline) &&
                                         instance2.Status.Equals(ClientStatus.Offline))
                                     {
                                        return instance1.InstanceName.CompareTo(instance2.InstanceName);
                                     }
                                     else if (instance1.Status.Equals(ClientStatus.Offline))
                                     {
                                        return 1;
                                     }
                                     else if (instance2.Status.Equals(ClientStatus.Offline))
                                     {
                                        return -1;
                                     }
                                  }

                                  return instance1.InstanceName.CompareTo(instance2.InstanceName);
                               });

         XmlElement xmlRootData = xmlDoc.DocumentElement;
         foreach (ClientInstance Instance in Instances)
         {
            XmlDocument xmlFrag = new XmlDocument();
            xmlFrag.Load(Path.Combine(Path.Combine(Prefs.ApplicationPath, "XML"), "SummaryFrag.xml"));
            XmlElement xmlData = xmlFrag.DocumentElement;

            XMLOps.setXmlNode(xmlData, "Status", Instance.Status.ToString());
            XMLOps.setXmlNode(xmlData, "StatusColor", ClientInstance.GetStatusHtmlColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "StatusFontColor", ClientInstance.GetStatusHtmlFontColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "PercentComplete", Instance.PercentComplete.ToString());
            XMLOps.setXmlNode(xmlData, "Name", Instance.InstanceName);
            XMLOps.setXmlNode(xmlData, "UserIDDuplicate", duplicateUserID.Contains(Instance.UserAndMachineID).ToString());
            XMLOps.setXmlNode(xmlData, "ClientType", Instance.CurrentUnitInfo.TypeOfClient.ToString());
            XMLOps.setXmlNode(xmlData, "TPF", Instance.TimePerFrame.ToString());
            XMLOps.setXmlNode(xmlData, "PPD", String.Format("{0:" + Prefs.PpdFormatString + "}", Instance.PPD));
            XMLOps.setXmlNode(xmlData, "UPD", String.Format("{0:0.00}", Instance.UPD));
            XMLOps.setXmlNode(xmlData, "MHz", Instance.ClientProcessorMegahertz.ToString());
            XMLOps.setXmlNode(xmlData, "PPDMHz", String.Format("{0:0.000}", Instance.PPD / Instance.ClientProcessorMegahertz));
            XMLOps.setXmlNode(xmlData, "ETA", Instance.ETA.ToString());
            XMLOps.setXmlNode(xmlData, "Core", Instance.CurrentUnitInfo.Core);
            XMLOps.setXmlNode(xmlData, "CoreVersion", Instance.CurrentUnitInfo.CoreVersion);
            XMLOps.setXmlNode(xmlData, "ProjectRunCloneGen", Instance.CurrentUnitInfo.ProjectRunCloneGen);
            XMLOps.setXmlNode(xmlData, "ProjectDuplicate", duplicateProjects.Contains(Instance.CurrentUnitInfo.ProjectRunCloneGen).ToString());
            XMLOps.setXmlNode(xmlData, "Credit", String.Format("{0:0}", Instance.Credit));
            XMLOps.setXmlNode(xmlData, "Completed", Instance.NumberOfCompletedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Failed", Instance.NumberOfFailedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Username", String.Format("{0} ({1})", Instance.FoldingID, Instance.Team));
            XMLOps.setXmlNode(xmlData, "UsernameMatch", Instance.IsUsernameOk().ToString()); //Issue 51
            if (Instance.CurrentUnitInfo.DownloadTimeUnknown)
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", String.Format("{0} {1}", Instance.CurrentUnitInfo.DownloadTime.ToShortDateString(), Instance.CurrentUnitInfo.DownloadTime.ToShortTimeString()));
            }
            if (Instance.CurrentUnitInfo.PreferredDeadlineUnknown)
            {
               XMLOps.setXmlNode(xmlData, "Deadline", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "Deadline", String.Format("{0} {1}", Instance.CurrentUnitInfo.PreferredDeadline.ToShortDateString(), Instance.CurrentUnitInfo.PreferredDeadline.ToShortTimeString()));
            }

            XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
            xmlRootData.AppendChild(xImpNode);
         }

         return xmlDoc;
      }

      /// <summary>
      /// Generates the Overview page. xslTransform allows the calling method to select the appropriate transform.
      /// This allows the same overview method to generate the website page and the app page.
      /// </summary>
      /// <param name="Totals">Instance Totals</param>
      /// <returns>Generated Xml Document</returns>
      private static XmlDocument CreateOverviewXml(InstanceTotals Totals)
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
      
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(Prefs.ApplicationPath, "XML"), "Overview.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XMLOps.setXmlNode(xmlData, "TotalHosts", Totals.TotalClients.ToString());
         XMLOps.setXmlNode(xmlData, "GoodHosts", Totals.WorkingClients.ToString());
         XMLOps.setXmlNode(xmlData, "BadHosts", Totals.NonWorkingClients.ToString());

         string PpdFormatString = Prefs.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "EstPPD", String.Format("{0:" + PpdFormatString + "}", Totals.PPD));
         XMLOps.setXmlNode(xmlData, "EstPPW", String.Format("{0:" + PpdFormatString + "}", Totals.PPD * 7));
         XMLOps.setXmlNode(xmlData, "EstUPD", String.Format("{0:" + PpdFormatString + "}", Totals.UPD));
         XMLOps.setXmlNode(xmlData, "EstUPW", String.Format("{0:" + PpdFormatString + "}", Totals.UPD * 7));

         if (Totals.WorkingClients > 0)
         {
            XMLOps.setXmlNode(xmlData, "AvEstPPD", String.Format("{0:" + PpdFormatString + "}", Totals.PPD / Totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstPPW", String.Format("{0:" + PpdFormatString + "}", Totals.PPD * 7 / Totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPD", String.Format("{0:" + PpdFormatString + "}", Totals.UPD / Totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPW", String.Format("{0:" + PpdFormatString + "}", Totals.UPD * 7 / Totals.WorkingClients));
         }
         else
         {
            XMLOps.setXmlNode(xmlData, "AvEstPPD", "N/A");
            XMLOps.setXmlNode(xmlData, "AvEstPPW", "N/A");
            XMLOps.setXmlNode(xmlData, "AvEstUPD", "N/A");
            XMLOps.setXmlNode(xmlData, "AvEstUPW", "N/A");
         }

         XMLOps.setXmlNode(xmlData, "TotalCompleted", Totals.TotalCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "TotalFailed", Totals.TotalFailedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         return xmlDoc;
      }
   }
}
