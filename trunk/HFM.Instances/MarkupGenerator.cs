/*
 * HFM.NET - Markup Generator Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */
 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using HFM.Framework;
using HFM.Helpers;

namespace HFM.Instances
{
   public sealed class MarkupGenerator : IMarkupGenerator
   {
      private volatile bool _generationInProgress;

      /// <summary>
      /// In Progress Flag for Callers
      /// </summary>
      public bool GenerationInProgress
      {
         get { return _generationInProgress; }
      }

      /// <summary>
      /// Contains XML File Paths from most recent XML Generation
      /// </summary>
      public ReadOnlyCollection<string> XmlFilePaths { get; private set; }

      /// <summary>
      /// Contains HTML File Paths from most recent HTML Generation
      /// </summary>
      public ReadOnlyCollection<string> HtmlFilePaths { get; private set; }
      
      private readonly IPreferenceSet _prefs;
      private readonly IProteinCollection _proteinCollection;
      
      private enum XmlFileName
      {
         Overview,
         Instances
      }
      
      public MarkupGenerator(IPreferenceSet prefs, IProteinCollection proteinCollection)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
      }
      
      #region HTML Generation

      /// <summary>
      /// Generate HTML Files from the given Client Instance Collection.
      /// </summary>
      /// <param name="instances">Client Instance Collection.</param>
      /// <exception cref="ArgumentNullException">Throws if instances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      public void GenerateHtml(ICollection<IClientInstance> instances)
      {
         if (instances == null) throw new ArgumentNullException("instances", "Argument 'instances' cannot be null.");
         if (_generationInProgress) throw new InvalidOperationException("Markup Generation already in progress.");

         _generationInProgress = true;

         try
         {
            HtmlFilePaths = DoHtmlGeneration(Path.GetTempPath(), instances);
         }
         finally
         {
            _generationInProgress = false;
         }
      }
      
      public ReadOnlyCollection<string> DoHtmlGeneration(string folderPath, ICollection<IClientInstance> instances)
      {
         Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
         Debug.Assert(instances != null);

         StreamWriter sw = null;

         try
         {
            // Generate XML Files
            XmlFilePaths = DoXmlGeneration(folderPath, instances);

            List<string> fileList = new List<string>(instances.Count + 4);
            string cssFileName = _prefs.GetPreference<string>(Preference.CssFile);
         
            // Load the Overview XML
            XmlDocument overviewXml = new XmlDocument();
            overviewXml.Load(XmlFilePaths[(int)XmlFileName.Overview]);

            // Generate the index page
            string filePath = Path.Combine(folderPath, "index.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XMLOps.Transform(overviewXml, GetXsltFileName(Preference.WebOverview), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the mobile index page
            filePath = Path.Combine(folderPath, "mobile.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XMLOps.Transform(overviewXml, GetXsltFileName(Preference.WebMobileOverview), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the summary page
            filePath = Path.Combine(folderPath, "summary.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XMLOps.Transform(overviewXml, GetXsltFileName(Preference.WebSummary), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the mobile summary page
            filePath = Path.Combine(folderPath, "mobilesummary.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XMLOps.Transform(overviewXml, GetXsltFileName(Preference.WebMobileSummary), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            string instanceXslt = GetXsltFileName(Preference.WebInstance);
            // Load the Instances XML
            XmlDocument instancesXml = new XmlDocument();
            instancesXml.Load(XmlFilePaths[(int)XmlFileName.Instances]);
            // Generate a page per instance
            foreach (IClientInstance instance in instances)
            {
               filePath = Path.Combine(folderPath, String.Concat(instance.InstanceName, ".html"));
               sw = new StreamWriter(filePath, false);
               sw.Write(XMLOps.Transform(FindInstanceNode(instancesXml, instance.InstanceName), instanceXslt, cssFileName));
               sw.Close();
               // Success, add it to the list
               fileList.Add(filePath);
            }

            return fileList.AsReadOnly();
         }
         finally
         {
            if (sw != null)
            {
               sw.Close();
            }
         }
      }

      private string GetXsltFileName(Preference p)
      {
         string xslt = _prefs.GetPreference<string>(p);
         Debug.Assert(String.IsNullOrEmpty(xslt) == false);

         if (Path.IsPathRooted(xslt))
         {
            if (File.Exists(xslt))
            {
               return xslt;
            }
         }
         else
         {
            string xsltFileName = Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XsltFolderName), xslt);
            if (File.Exists(xsltFileName))
            {
               return xsltFileName;
            }
         }

         throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
            "XSLT File '{0}' cannot be found.", xslt));
      }
      
      private static XmlNode FindInstanceNode(XmlNode instancesXml, string instanceName)
      {
         foreach (XmlNode xmlNode in instancesXml.ChildNodes[1])
         {
            if (xmlNode.Attributes["Name"].Value.Equals(instanceName))
            {
               return xmlNode;
            }  
         }

         return null;
      }
      
      #endregion
      
      #region XML Generation

      /// <summary>
      /// Generate XML Files from the given Client Instance Collection.
      /// </summary>
      /// <param name="instances">Client Instance Collection.</param>
      /// <exception cref="ArgumentNullException">Throws if instances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      public void GenerateXml(ICollection<IClientInstance> instances)
      {
         if (instances == null) throw new ArgumentNullException("instances", "Argument 'instances' cannot be null.");
         if (_generationInProgress) throw new InvalidOperationException("Markup Generation already in progress.");

         _generationInProgress = true;

         try
         {
            XmlFilePaths = DoXmlGeneration(Path.GetTempPath(), instances);
         }
         finally
         {
            _generationInProgress = false;
         }
      }

      public ReadOnlyCollection<string> DoXmlGeneration(string folderPath, ICollection<IClientInstance> instances)
      {
         Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
         Debug.Assert(instances != null);
      
         string[] fileList = new string[2];
      
         // Get instance totals
         InstanceTotals totals = InstanceCollectionHelpers.GetInstanceTotals(instances);

         // Generate the Overview XML
         XmlDocument summaryXml = CreateSummaryXml(CreateOverviewXml(totals), instances);
         string filePath = Path.Combine(folderPath, "Overview.xml");
         summaryXml.Save(filePath);
         // Success, add it to the list
         fileList[(int)XmlFileName.Overview] = filePath;

         XmlDocument instancesXml = new XmlDocument();
         // Write down the XML declaration
         XmlDeclaration xmlDeclaration = instancesXml.CreateXmlDeclaration("1.0", "utf-8", null);
         // Create the root element
         instancesXml.InsertBefore(xmlDeclaration, instancesXml.DocumentElement);
         XmlElement xmlRoot = instancesXml.CreateElement("Instances");
         instancesXml.AppendChild(xmlRoot);
         
         // Generate an XML Node per Client Instance
         foreach (IClientInstance instance in instances)
         {
            XmlDocument instanceXml = CreateInstanceXml(instance);
            XmlNode instanceNode = instancesXml.ImportNode(instanceXml.ChildNodes[1], true);
            xmlRoot.AppendChild(instanceNode);
         }
         filePath = Path.Combine(folderPath, "Instances.xml");
         instancesXml.Save(filePath);
         // Success, add it to the list
         fileList[(int)XmlFileName.Instances] = filePath;

         return Array.AsReadOnly(fileList);
      }

      private XmlDocument CreateInstanceXml(IClientInstance instance)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XmlFolderName), "Instance.xml"));
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

         xmlData.SetAttribute("Name", instance.InstanceName);
         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XMLOps.setXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", instance.FramesComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", instance.PercentComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", instance.TimePerFrame.Hours, instance.TimePerFrame.Minutes, instance.TimePerFrame.Seconds));

         string ppdFormatString = _prefs.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstPPD", String.Format("{0:" + ppdFormatString + "}", instance.PPD));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstPPW", String.Format("{0:" + ppdFormatString + "}", instance.PPD * 7));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstUPD", String.Format("{0:0.00}", instance.UPD));
         XMLOps.setXmlNode(xmlData, "UnitInfo/EstUPW", String.Format("{0:0.00}", instance.UPD * 7));
         XMLOps.setXmlNode(xmlData, "UnitInfo/Completed", instance.TotalRunCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "UnitInfo/Failed", instance.TotalRunFailedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "UnitInfo/TotalCompleted", instance.TotalClientCompletedUnits.ToString());

         if (instance.CurrentUnitInfo.DownloadTimeUnknown)
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", "Unknown");
         }
         else
         {
            DateTime downloadTime = instance.CurrentUnitInfo.DownloadTime;
            XMLOps.setXmlNode(xmlData, "UnitInfo/DownloadTime", String.Format(CultureInfo.CurrentCulture, "{0} at {1}", downloadTime.ToLongDateString(), downloadTime.ToLongTimeString()));
         }

         if (instance.ETA.Equals(TimeSpan.Zero))
         {
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", "Unknown");
         }
         else
         {
            DateTime completeTime = DateTime.Now.Add(instance.ETA);
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", String.Format(CultureInfo.CurrentCulture, "{0} at {1}", completeTime.ToLongDateString(), completeTime.ToLongTimeString()));
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

         IProtein p = _proteinCollection.CreateProtein();
         if (_proteinCollection.ContainsKey(instance.CurrentUnitInfo.ProjectID))
         {
            p = _proteinCollection[instance.CurrentUnitInfo.ProjectID];
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
         // Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
         if (instance.DataAggregator.CurrentLogLines != null)
         {
            foreach (ILogLine line in instance.DataAggregator.CurrentLogLines)
            {
               sb.Append(line.LineRaw);
               sb.Append("<BR>");
               sb.Append(Environment.NewLine);
            }
         }

         XMLOps.setXmlNode(xmlData, "UnitLog/Text", sb.ToString());
         if (_prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
         {
            XMLOps.setXmlNode(xmlData, "UnitLog/FullLogFile", instance.CachedFAHLogName);
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

         XMLOps.setXmlNode(xmlData, "LastRetrievedDate", instance.LastRetrievalTime.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastRetrievedTime", instance.LastRetrievalTime.ToLongTimeString());

         return xmlDoc;
      }

      private XmlDocument CreateSummaryXml(XmlDocument xmlDoc, IEnumerable<IClientInstance> instanceCollection)
      {
         bool duplicateUserIdCheck = _prefs.GetPreference<bool>(Preference.DuplicateUserIdCheck);
         bool duplicateProjectCheck = _prefs.GetPreference<bool>(Preference.DuplicateProjectCheck);
         CompletedCountDisplayType completedCountDisplayType =
            _prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay);
         
         XmlElement xmlRootData = xmlDoc.DocumentElement;
         foreach (IClientInstance instance in instanceCollection)
         {
            XmlDocument xmlFrag = new XmlDocument();
            xmlFrag.Load(Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XmlFolderName), "SummaryFrag.xml"));
            XmlElement xmlData = xmlFrag.DocumentElement;

            XMLOps.setXmlNode(xmlData, "Status", instance.Status.ToString());
            XMLOps.setXmlNode(xmlData, "StatusColor", ClientInstance.GetStatusHtmlColor(instance.Status));
            XMLOps.setXmlNode(xmlData, "StatusFontColor", ClientInstance.GetStatusHtmlFontColor(instance.Status));
            XMLOps.setXmlNode(xmlData, "PercentComplete", instance.PercentComplete.ToString());
            XMLOps.setXmlNode(xmlData, "Name", instance.InstanceName);
            XMLOps.setXmlNode(xmlData, "UserIDDuplicate", (duplicateUserIdCheck && instance.UserIdIsDuplicate).ToString());
            XMLOps.setXmlNode(xmlData, "ClientType", instance.CurrentUnitInfo.TypeOfClient.ToString());
            XMLOps.setXmlNode(xmlData, "TPF", instance.TimePerFrame.ToString());
            XMLOps.setXmlNode(xmlData, "PPD", String.Format("{0:" + _prefs.PpdFormatString + "}", instance.PPD));
            XMLOps.setXmlNode(xmlData, "UPD", String.Format("{0:0.00}", instance.UPD));
            XMLOps.setXmlNode(xmlData, "MHz", instance.ClientProcessorMegahertz.ToString());
            XMLOps.setXmlNode(xmlData, "PPDMHz", String.Format("{0:0.000}", instance.PPD / instance.ClientProcessorMegahertz));
            XMLOps.setXmlNode(xmlData, "ETA", instance.ETA.ToString());
            XMLOps.setXmlNode(xmlData, "Core", instance.CurrentUnitInfo.Core);
            XMLOps.setXmlNode(xmlData, "CoreVersion", instance.CurrentUnitInfo.CoreVersion);
            XMLOps.setXmlNode(xmlData, "ProjectRunCloneGen", instance.CurrentUnitInfo.ProjectRunCloneGen);
            XMLOps.setXmlNode(xmlData, "ProjectDuplicate", (duplicateProjectCheck && instance.CurrentUnitInfo.ProjectIsDuplicate).ToString());
            XMLOps.setXmlNode(xmlData, "Credit", String.Format("{0:0}", instance.Credit));
            XMLOps.setXmlNode(xmlData, "Completed", instance.TotalRunCompletedUnits.ToString());
            XMLOps.setXmlNode(xmlData, "Failed", instance.TotalRunFailedUnits.ToString());
            XMLOps.setXmlNode(xmlData, "TotalCompleted", instance.TotalClientCompletedUnits.ToString());
            XMLOps.setXmlNode(xmlData, "CompletedCountDisplay", completedCountDisplayType.ToString());
            XMLOps.setXmlNode(xmlData, "Username", String.Format("{0} ({1})", instance.FoldingID, instance.Team));
            XMLOps.setXmlNode(xmlData, "UsernameMatch", instance.IsUsernameOk().ToString()); //Issue 51
            if (instance.CurrentUnitInfo.DownloadTimeUnknown)
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "DownloadTime", String.Format("{0} {1}", instance.CurrentUnitInfo.DownloadTime.ToShortDateString(), instance.CurrentUnitInfo.DownloadTime.ToShortTimeString()));
            }
            if (instance.CurrentUnitInfo.PreferredDeadlineUnknown)
            {
               XMLOps.setXmlNode(xmlData, "Deadline", "Unknown");
            }
            else
            {
               XMLOps.setXmlNode(xmlData, "Deadline", String.Format("{0} {1}", instance.CurrentUnitInfo.PreferredDeadline.ToShortDateString(), instance.CurrentUnitInfo.PreferredDeadline.ToShortTimeString()));
            }

            XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
            xmlRootData.AppendChild(xImpNode);
         }

         return xmlDoc;
      }

      private XmlDocument CreateOverviewXml(InstanceTotals totals)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XmlFolderName), "Overview.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         XMLOps.setXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XMLOps.setXmlNode(xmlData, "TotalHosts", totals.TotalClients.ToString());
         XMLOps.setXmlNode(xmlData, "GoodHosts", totals.WorkingClients.ToString());
         XMLOps.setXmlNode(xmlData, "BadHosts", totals.NonWorkingClients.ToString());

         string ppdFormatString = _prefs.PpdFormatString;
         XMLOps.setXmlNode(xmlData, "EstPPD", String.Format("{0:" + ppdFormatString + "}", totals.PPD));
         XMLOps.setXmlNode(xmlData, "EstPPW", String.Format("{0:" + ppdFormatString + "}", totals.PPD * 7));
         XMLOps.setXmlNode(xmlData, "EstUPD", String.Format("{0:" + ppdFormatString + "}", totals.UPD));
         XMLOps.setXmlNode(xmlData, "EstUPW", String.Format("{0:" + ppdFormatString + "}", totals.UPD * 7));

         if (totals.WorkingClients > 0)
         {
            XMLOps.setXmlNode(xmlData, "AvEstPPD", String.Format("{0:" + ppdFormatString + "}", totals.PPD / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstPPW", String.Format("{0:" + ppdFormatString + "}", totals.PPD * 7 / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPD", String.Format("{0:" + ppdFormatString + "}", totals.UPD / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPW", String.Format("{0:" + ppdFormatString + "}", totals.UPD * 7 / totals.WorkingClients));
         }
         else
         {
            XMLOps.setXmlNode(xmlData, "AvEstPPD", "0");
            XMLOps.setXmlNode(xmlData, "AvEstPPW", "0");
            XMLOps.setXmlNode(xmlData, "AvEstUPD", "0");
            XMLOps.setXmlNode(xmlData, "AvEstUPW", "0");
         }

         XMLOps.setXmlNode(xmlData, "Completed", totals.TotalRunCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "Failed", totals.TotalRunFailedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "TotalCompleted", totals.TotalClientCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         return xmlDoc;
      }
      
      #endregion
   }
}
