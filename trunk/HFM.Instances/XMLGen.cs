/*
 * HFM.NET - XML Generation Helper Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Xml;

using HFM.Instrumentation;
using HFM.Preferences;
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
            sw.Write(XMLOps.Transform(OverviewXml, "WebOverview.xslt"));
            sw.Close();

            // Generate the mobile index page
            sw = new StreamWriter(Path.Combine(FolderPath, "mobile.html"), false);
            sw.Write(XMLOps.Transform(OverviewXml, "WebMobileOverview.xslt"));
            sw.Close();

            // Generate the summart page XML
            XmlDocument SummaryXml = CreateSummaryXml(Instances);

            // Generate the summary page
            sw = new StreamWriter(Path.Combine(FolderPath, "summary.html"), false);
            sw.Write(XMLOps.Transform(SummaryXml, "WebSummary.xslt"));
            sw.Close();

            // Generate the mobile summary page
            sw = new StreamWriter(Path.Combine(FolderPath, "mobilesummary.html"), false);
            sw.Write(XMLOps.Transform(SummaryXml, "WebMobileSummary.xslt"));
            sw.Close();

            // Generate a page per instance
            foreach (ClientInstance instance in Instances)
            {
               sw = new StreamWriter(Path.Combine(FolderPath, Path.ChangeExtension(instance.InstanceName, ".html")), false);
               sw.Write(XMLOps.Transform(CreateInstanceXml(instance), "WebInstance.xslt"));
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

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Server">Server Name</param>
      /// <param name="FtpPath">Path from FTP Server Root</param>
      /// <param name="Username">FTP Server Username</param>
      /// <param name="Password">FTP Server Password</param>
      /// <param name="Instances">Client Instance Collection</param>
      public static void DoWebFtpUpload(string Server, string FtpPath, string Username, string Password, ICollection<ClientInstance> Instances)
      {
         DoWebFtpUpload(Server, FtpPath, Username, Password, Instances, true);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="Server">Server Name</param>
      /// <param name="FtpPath">Path from FTP Server Root</param>
      /// <param name="Username">FTP Server Username</param>
      /// <param name="Password">FTP Server Password</param>
      /// <param name="Instances">Client Instance Collection</param>
      /// <param name="PassiveMode">Passive FTP Mode</param>
      public static void DoWebFtpUpload(string Server, string FtpPath, string Username, string Password, ICollection<ClientInstance> Instances, bool PassiveMode)
      {
         // Time FTP Upload Conversation - Issue 52
         DateTime Start = HfmTrace.ExecStart;

         try
         {
            NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.Combine(PreferenceSet.AppPath, "CSS"), PreferenceSet.Instance.CSSFileName), Username, Password, PassiveMode);
            NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "index.html"), Username, Password, PassiveMode);
            NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "summary.html"), Username, Password, PassiveMode);
            NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "mobile.html"), Username, Password, PassiveMode);
            NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(), "mobilesummary.html"), Username, Password, PassiveMode);

            foreach (ClientInstance instance in Instances)
            {
                  NetworkOps.FtpUploadHelper(Server, FtpPath, Path.Combine(Path.GetTempPath(),
                                             Path.ChangeExtension(instance.InstanceName, ".html")),
                                             Username, Password, PassiveMode);
            }
         }
         finally
         {
            // Time FTP Upload Conversation - Issue 52
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
         }
      }
   
      /// <summary>
      /// Generates and transforms data for an Instance
      /// </summary>
      /// <param name="Instance">Instance to use as data source</param>
      /// <returns>Generated Xml Document</returns>
      private static XmlDocument CreateInstanceXml(ClientInstance Instance)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Instance.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         //    <UnitInfo>
         //        <DownloadTime>10 August 09:37:33</DownloadTime>
         //        <FramesComplete>35</FramesComplete>
         //        <PercentComplete>35</PercentComplete>
         //        <TimePerFrame>1h 15m 44s</TimePerFrame>
         //        <ExpectedCompletionDate>16 August 2006 1:46 am</ExpectedCompletionDate>
         //    </UnitInfo>

         xmlData.SetAttribute("Name", Instance.InstanceName);

         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);

         if (Instance.CurrentUnitInfo.DownloadTime.Equals(DateTime.MinValue))
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", "Unknown");
         }
         else
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", Instance.CurrentUnitInfo.DownloadTime.ToString("d MMMM yyyy hh:mm tt"));
         }
         XMLOps.setXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", Instance.FramesComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", Instance.PercentComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", Instance.TimePerFrame.Hours, Instance.TimePerFrame.Minutes, Instance.TimePerFrame.Seconds));

         if (Instance.ETA.Equals(TimeSpan.Zero))
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", "Unknown");
         }
         else
         {
            DateTime CompleteTime = DateTime.Now.Add(Instance.ETA);
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", CompleteTime.ToLongDateString() + " at " + CompleteTime.ToLongTimeString());
         }

         //    <Computer>
         //        <EstPPD>82.36</EstPPD>
         //        <EstPPW>576.54</EstPPW>
         //        <EstUPD>0.46</EstUPD>
         //        <EstUPW>3.77</EstUPW>
         //        <TotalProjects>243</TotalProjects>
         //    </Computer>

         string PpdFormatString = PreferenceSet.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "Computer/EstPPD", String.Format("{0:" + PpdFormatString + "}", Instance.PPD));
         XMLOps.setXmlNode(xmlData, "Computer/EstPPW", String.Format("{0:" + PpdFormatString + "}", Instance.PPD * 7));
         XMLOps.setXmlNode(xmlData, "Computer/EstUPD", String.Format("{0:" + PpdFormatString + "}", Instance.UPD));
         XMLOps.setXmlNode(xmlData, "Computer/EstUPW", String.Format("{0:" + PpdFormatString + "}", Instance.UPD * 7));
         XMLOps.setXmlNode(xmlData, "Computer/TotalProjects", Instance.TotalUnits.ToString());

         //    <Protein>
         //        <ProjectNumber>1814</ProjectNumber>
         //        <ServerIP>171.65.103.158 </ServerIP>
         //        <WorkUnit>p1814_Collagen_POG10more_refolding</WorkUnit>
         //        <NumAtoms>1116</NumAtoms>
         //        <PreferredDays>30</PreferredDays>
         //        <MaxDays>44</MaxDays>
         //        <Credit>153</Credit>
         //        <Frames>100</Frames>
         //        <Core>Amber</Core>
         //        <Description>This is the BLAH</Description>
         //        <Contact>spark7</Contact>
         //    </Protein>

         XMLOps.setXmlNode(xmlData, "Protein/ProjectNumber", Instance.CurrentUnitInfo.CurrentProtein.ProjectNumber.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/ServerIP", Instance.CurrentUnitInfo.CurrentProtein.ServerIP);
         XMLOps.setXmlNode(xmlData, "Protein/WorkUnit", Instance.CurrentUnitInfo.ProteinName);
         XMLOps.setXmlNode(xmlData, "Protein/NumAtoms", Instance.CurrentUnitInfo.CurrentProtein.NumAtoms.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/PreferredDays", Instance.CurrentUnitInfo.CurrentProtein.PreferredDays.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/MaxDays", Instance.CurrentUnitInfo.CurrentProtein.MaxDays.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Credit", Instance.CurrentUnitInfo.CurrentProtein.Credit.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Frames", Instance.CurrentUnitInfo.CurrentProtein.Frames.ToString());
         XMLOps.setXmlNode(xmlData, "Protein/Core", Instance.CurrentUnitInfo.CurrentProtein.Core);
         XMLOps.setXmlNode(xmlData, "Protein/Description", NetworkOps.GetProteinDescription(Instance.CurrentUnitInfo.CurrentProtein.Description));
         XMLOps.setXmlNode(xmlData, "Protein/Contact", Instance.CurrentUnitInfo.CurrentProtein.Contact);

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
      /// Generates the Summary page. xslTransform allows the calling method to select the appropriate transform.
      /// This allows the same summary method to generate the website page and the app page.
      /// </summary>
      /// <param name="Instances">Array of folding instances</param>
      /// <returns>Generated Xml Document</returns>
      private static XmlDocument CreateSummaryXml(ClientInstance[] Instances)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Summary.xml"));
         XmlElement xmlRootData = xmlDoc.DocumentElement;
         
         XMLOps.setXmlNode(xmlRootData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);

         List<string> duplicateUserID = new List<string>(Instances.Length);
         List<string> duplicateProjects = new List<string>(Instances.Length);
         
         InstanceCollectionHelpers.FindDuplicates(duplicateUserID, duplicateProjects, Instances);

         Array.Sort(Instances, delegate(ClientInstance instance1, ClientInstance instance2)
                               {
                                  // Make the HTML Summary respect the 'List Office Clients Last' option - Issue 78
                                  if (PreferenceSet.Instance.OfflineLast)
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

         foreach (ClientInstance Instance in Instances)
         {
            XmlDocument xmlFrag = new XmlDocument();
            xmlFrag.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "SummaryFrag.xml"));
            XmlElement xmlData = xmlFrag.DocumentElement;

            XMLOps.setXmlNode(xmlData, "Status", Instance.Status.ToString());
            XMLOps.setXmlNode(xmlData, "StatusColor", ClientInstance.GetStatusHtmlColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "StatusFontColor", ClientInstance.GetStatusHtmlFontColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "PercentComplete", Instance.PercentComplete.ToString());
            XMLOps.setXmlNode(xmlData, "Name", Instance.InstanceName);
            XMLOps.setXmlNode(xmlData, "UserIDDuplicate", duplicateUserID.Contains(Instance.UserAndMachineID).ToString());
            XMLOps.setXmlNode(xmlData, "ClientType", Instance.CurrentUnitInfo.TypeOfClient.ToString());
            XMLOps.setXmlNode(xmlData, "TPF", Instance.TimePerFrame.ToString());
            XMLOps.setXmlNode(xmlData, "PPD", String.Format("{0:" + PreferenceSet.PpdFormatString + "}", Instance.PPD));
            XMLOps.setXmlNode(xmlData, "UPD", String.Format("{0:0.00}", Instance.UPD));
            XMLOps.setXmlNode(xmlData, "MHz", Instance.ClientProcessorMegahertz.ToString());
            XMLOps.setXmlNode(xmlData, "PPDMHz", String.Format("{0:0.000}", Instance.PPD / Instance.ClientProcessorMegahertz));
            XMLOps.setXmlNode(xmlData, "ETA", Instance.ETA.ToString());
            XMLOps.setXmlNode(xmlData, "Core", Instance.CurrentUnitInfo.CurrentProtein.Core);
            XMLOps.setXmlNode(xmlData, "CoreVersion", Instance.CurrentUnitInfo.CoreVersion);
            XMLOps.setXmlNode(xmlData, "ProjectRunCloneGen", Instance.CurrentUnitInfo.ProjectRunCloneGen);
            XMLOps.setXmlNode(xmlData, "ProjectDuplicate", duplicateProjects.Contains(Instance.CurrentUnitInfo.ProjectRunCloneGen).ToString());
            XMLOps.setXmlNode(xmlData, "Credit", String.Format("{0:0}", Instance.CurrentUnitInfo.CurrentProtein.Credit));
            XMLOps.setXmlNode(xmlData, "Completed", Instance.NumberOfCompletedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Failed", Instance.NumberOfFailedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Username", String.Format("{0} ({1})", Instance.FoldingID, Instance.Team));
            XMLOps.setXmlNode(xmlData, "UsernameMatch", Instance.IsUsernameOk().ToString()); //Issue 51
            if (Instance.CurrentUnitInfo.DownloadTime.Equals(DateTime.MinValue))
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", String.Format("{0} {1}", Instance.CurrentUnitInfo.DownloadTime.ToShortDateString(), Instance.CurrentUnitInfo.DownloadTime.ToShortTimeString()));
            }
            if (Instance.CurrentUnitInfo.Deadline.Equals(DateTime.MinValue))
            {
               XMLOps.setXmlNode(xmlData, "Deadline", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "Deadline", String.Format("{0} {1}", Instance.CurrentUnitInfo.Deadline.ToShortDateString(), Instance.CurrentUnitInfo.Deadline.ToShortTimeString()));
            }

            XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
            xmlRootData.AppendChild(xImpNode);
         }

         XmlNode xLastUpdDate = xmlDoc.CreateNode(XmlNodeType.Element, "LastUpdatedDate", null);
         xLastUpdDate.InnerText = DateTime.Now.ToLongDateString();

         XmlNode xLastUpdTime = xmlDoc.CreateNode(XmlNodeType.Element, "LastUpdatedTime", null);
         xLastUpdTime.InnerText = DateTime.Now.ToLongTimeString();

         xmlRootData.AppendChild(xLastUpdDate);
         xmlRootData.AppendChild(xLastUpdTime);

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
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Overview.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);

         //<Overview>
         //    <TotalHosts>0</TotalHosts>
         //    <GoodHosts>0</GoodHosts>
         //    <BadHosts>0</BadHosts>
         //</Overview>

         XMLOps.setXmlNode(xmlData, "TotalHosts", Totals.TotalClients.ToString());
         XMLOps.setXmlNode(xmlData, "GoodHosts", Totals.WorkingClients.ToString());
         XMLOps.setXmlNode(xmlData, "BadHosts", Totals.NonWorkingClients.ToString());

         //    <EstPPD>0.00</EstPPD>
         //    <EstPPW>0.00</EstPPW>
         //    <EstUPD>0.00</EstUPD>
         //    <EstUPW>0.00</EstUPW>

         string PpdFormatString = PreferenceSet.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "EstPPD", String.Format("{0:" + PpdFormatString + "}", Totals.PPD));
         XMLOps.setXmlNode(xmlData, "EstPPW", String.Format("{0:" + PpdFormatString + "}", Totals.PPD * 7));
         XMLOps.setXmlNode(xmlData, "EstUPD", String.Format("{0:" + PpdFormatString + "}", Totals.UPD));
         XMLOps.setXmlNode(xmlData, "EstUPW", String.Format("{0:" + PpdFormatString + "}", Totals.UPD * 7));

         //    <AvEstPPD>0.00</AvEstPPD>
         //    <AvEstPPW>0.00</AvEstPPW>
         //    <AvEstUPD>0.00</AvEstUPD>
         //    <AvEstUPW>0.00</AvEstUPW>

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

         //    <TotalCompleted>0</TotalCompleted>
         //    <TotalFailed>0</TotalFailed>
         //    <LastUpdatedDate>Now</LastUpdatedDate>
         //    <LastUpdatedTime>Now</LastUpdatedTime>

         XMLOps.setXmlNode(xmlData, "TotalCompleted", Totals.TotalCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "TotalFailed", Totals.TotalFailedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         return xmlDoc;
      }
   }
}
