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
using System.Linq;
using System.Text;
using System.Xml;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public interface IMarkupGenerator
   {
      /// <summary>
      /// In Progress Flag for Callers
      /// </summary>
      bool GenerationInProgress { get; }

      /// <summary>
      /// Contains XML File Paths from most recent XML Generation
      /// </summary>
      ReadOnlyCollection<string> XmlFilePaths { get; }

      /// <summary>
      /// Contains HTML File Paths from most recent HTML Generation
      /// </summary>
      ReadOnlyCollection<string> HtmlFilePaths { get; }

      /// <summary>
      /// Contains File Path to the most recent Client Data File
      /// </summary>
      string ClientDataFilePath { get; }

      /// <summary>
      /// Generate Web Files from the given Display and Client Instances.
      /// </summary>
      /// <param name="displayInstances">Display Instances</param>
      /// <param name="clientInstances">Client Instances</param>
      /// <exception cref="ArgumentNullException">Throws if displayInstances or clientInstances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      void Generate(IEnumerable<IDisplayInstance> displayInstances, IEnumerable<IClientInstance> clientInstances);
   }

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

      /// <summary>
      /// Contains File Path to the most recent Client Data File
      /// </summary>
      public string ClientDataFilePath { get; private set; }
      
      private readonly IPreferenceSet _prefs;
      
      private enum XmlFileName
      {
         Overview,
         Instances
      }
      
      public MarkupGenerator(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }
      
      /// <summary>
      /// Generate Web Files from the given Display and Client Instances.
      /// </summary>
      /// <param name="displayInstances">Display Instances</param>
      /// <param name="clientInstances">Client Instances</param>
      /// <exception cref="ArgumentNullException">Throws if displayInstances or clientInstances is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      public void Generate(IEnumerable<IDisplayInstance> displayInstances, IEnumerable<IClientInstance> clientInstances)
      {
         if (displayInstances == null) throw new ArgumentNullException("displayInstances");
         if (clientInstances == null) throw new ArgumentNullException("clientInstances");
         if (_generationInProgress) throw new InvalidOperationException("Markup Generation already in progress.");

         _generationInProgress = true;

         XmlFilePaths = null;
         HtmlFilePaths = null;
         ClientDataFilePath = null;

         try
         {
            var copyHtml = _prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
            var copyXml = _prefs.GetPreference<bool>(Preference.WebGenCopyXml);
            var copyClientData = _prefs.GetPreference<bool>(Preference.WebGenCopyClientData);

            if (copyHtml)
            {
               // GenerateHtml calls GenerateXml - these two
               // calls are mutually exclusive
               GenerateHtml(displayInstances);
            }
            else if (copyXml)
            {
               GenerateXml(displayInstances);
            }
            // Issue 79
            if (copyClientData)
            {
               GenerateClientData(clientInstances);
            }
         }
         finally
         {
            _generationInProgress = false;
         }
      }
      
      #region HTML Generation

      /// <summary>
      /// Generate HTML Files from the given Display Instances
      /// </summary>
      /// <param name="instances">Display Instances</param>
      public void GenerateHtml(IEnumerable<IDisplayInstance> instances)
      {
         try
         {
            HtmlFilePaths = DoHtmlGeneration(Path.GetTempPath(), instances);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      
      private ReadOnlyCollection<string> DoHtmlGeneration(string folderPath, IEnumerable<IDisplayInstance> instances)
      {
         Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
         Debug.Assert(instances != null);

         StreamWriter sw = null;

         try
         {
            // Generate XML Files
            XmlFilePaths = DoXmlGeneration(folderPath, instances);

            var fileList = new List<string>(instances.Count() + 4);
            var cssFileName = _prefs.GetPreference<string>(Preference.CssFile);
         
            // Load the Overview XML
            var overviewXml = new XmlDocument();
            overviewXml.Load(XmlFilePaths[(int)XmlFileName.Overview]);

            // Generate the index page
            string filePath = Path.Combine(folderPath, "index.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XmlOps.Transform(overviewXml, GetXsltFileName(Preference.WebOverview), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the mobile index page
            filePath = Path.Combine(folderPath, "mobile.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XmlOps.Transform(overviewXml, GetXsltFileName(Preference.WebMobileOverview), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the summary page
            filePath = Path.Combine(folderPath, "summary.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XmlOps.Transform(overviewXml, GetXsltFileName(Preference.WebSummary), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            // Generate the mobile summary page
            filePath = Path.Combine(folderPath, "mobilesummary.html");
            sw = new StreamWriter(filePath, false);
            sw.Write(XmlOps.Transform(overviewXml, GetXsltFileName(Preference.WebMobileSummary), cssFileName));
            sw.Close();
            // Success, add it to the list
            fileList.Add(filePath);

            string instanceXslt = GetXsltFileName(Preference.WebInstance);
            // Load the Instances XML
            var instancesXml = new XmlDocument();
            instancesXml.Load(XmlFilePaths[(int)XmlFileName.Instances]);
            // Generate a page per instance
            foreach (IDisplayInstance instance in instances)
            {
               filePath = Path.Combine(folderPath, String.Concat(instance.Name, ".html"));
               sw = new StreamWriter(filePath, false);
               sw.Write(XmlOps.Transform(FindInstanceNode(instancesXml, instance.Name), instanceXslt, cssFileName));
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
         var xslt = _prefs.GetPreference<string>(p);
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
      /// Generate XML Files from the given Display Instances
      /// </summary>
      /// <param name="instances">Display Instances</param>
      public void GenerateXml(IEnumerable<IDisplayInstance> instances)
      {
         try
         {
            XmlFilePaths = DoXmlGeneration(Path.GetTempPath(), instances);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      private ReadOnlyCollection<string> DoXmlGeneration(string folderPath, IEnumerable<IDisplayInstance> instances)
      {
         Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
         Debug.Assert(instances != null);
      
         var fileList = new string[2];
      
         // Get instance totals
         InstanceTotals totals = InstanceCollectionHelpers.GetInstanceTotals(instances);

         // Generate the Overview XML
         XmlDocument summaryXml = CreateSummaryXml(CreateOverviewXml(totals), instances);
         string filePath = Path.Combine(folderPath, "Overview.xml");
         summaryXml.Save(filePath);
         // Success, add it to the list
         fileList[(int)XmlFileName.Overview] = filePath;

         var instancesXml = new XmlDocument();
         // Write down the XML declaration
         XmlDeclaration xmlDeclaration = instancesXml.CreateXmlDeclaration("1.0", "utf-8", null);
         // Create the root element
         instancesXml.InsertBefore(xmlDeclaration, instancesXml.DocumentElement);
         XmlElement xmlRoot = instancesXml.CreateElement("Instances");
         instancesXml.AppendChild(xmlRoot);
         
         // Generate an XML Node per Client Instance
         foreach (IDisplayInstance instance in instances)
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

      private XmlDocument CreateInstanceXml(IDisplayInstance instance)
      {
         var xmlDoc = new XmlDocument();
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

         xmlData.SetAttribute("Name", instance.Name);
         XmlOps.SetXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XmlOps.SetXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", instance.FramesComplete));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", instance.PercentComplete));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", instance.TPF.Hours, instance.TPF.Minutes, instance.TPF.Seconds));

         string ppdFormatString = _prefs.PpdFormatString;
         XmlOps.SetXmlNode(xmlData, "UnitInfo/EstPPD", String.Format("{0:" + ppdFormatString + "}", instance.PPD));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/EstPPW", String.Format("{0:" + ppdFormatString + "}", instance.PPD * 7));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/EstUPD", String.Format("{0:0.00}", instance.UPD));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/EstUPW", String.Format("{0:0.00}", instance.UPD * 7));
         XmlOps.SetXmlNode(xmlData, "UnitInfo/Completed", instance.TotalRunCompletedUnits.ToString());
         XmlOps.SetXmlNode(xmlData, "UnitInfo/Failed", instance.TotalRunFailedUnits.ToString());
         XmlOps.SetXmlNode(xmlData, "UnitInfo/TotalCompleted", instance.TotalClientCompletedUnits.ToString());

         if (instance.DownloadTime.Equals(DateTime.MinValue))
         {
            XmlOps.SetXmlNode(xmlData, "UnitInfo/DownloadTime", "Unknown");
         }
         else
         {
            DateTime downloadTime = instance.DownloadTime;
            // TODO: When localizing use ToString("format", CultureInfo.CurrentCulture) instead.
            XmlOps.SetXmlNode(xmlData, "UnitInfo/DownloadTime", String.Format(CultureInfo.InvariantCulture, "{0} at {1}", 
               downloadTime.ToString("D", CultureInfo.InvariantCulture), downloadTime.ToString("h:mm:ss tt", CultureInfo.InvariantCulture)));
         }

         if (instance.ETA.Equals(TimeSpan.Zero))
         {
            XmlOps.SetXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", "Unknown");
         }
         else
         {
            DateTime completeTime = DateTime.Now.Add(instance.ETA);
            // TODO: When localizing use ToString("format", CultureInfo.CurrentCulture) instead.
            XmlOps.SetXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", String.Format(CultureInfo.InvariantCulture, "{0} at {1}",
               completeTime.ToString("D", CultureInfo.InvariantCulture), completeTime.ToString("h:mm:ss tt", CultureInfo.InvariantCulture)));
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

         IProtein p = instance.CurrentProtein;
         XmlOps.SetXmlNode(xmlData, "Protein/ProjectNumber", p.ProjectNumber.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/ServerIP", p.ServerIP);
         XmlOps.SetXmlNode(xmlData, "Protein/WorkUnit", p.WorkUnitName);
         XmlOps.SetXmlNode(xmlData, "Protein/NumAtoms", p.NumAtoms.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/PreferredDays", p.PreferredDays.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/MaxDays", p.MaxDays.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/Credit", p.Credit.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/KFactor", p.KFactor.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/Frames", p.Frames.ToString());
         XmlOps.SetXmlNode(xmlData, "Protein/Core", p.Core);
         XmlOps.SetXmlNode(xmlData, "Protein/Description", p.Description);
         XmlOps.SetXmlNode(xmlData, "Protein/Contact", p.Contact);

         var sb = new StringBuilder();
         // Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
         if (instance.CurrentLogLines != null)
         {
            foreach (ILogLine line in instance.CurrentLogLines)
            {
               sb.Append(line.LineRaw);
               sb.Append("<BR>");
               sb.Append(Environment.NewLine);
            }
         }

         XmlOps.SetXmlNode(xmlData, "UnitLog/Text", sb.ToString());
         // Issue 79 - External Instances don't have full FAHlog.txt files available
         if (instance.ExternalInstanceName == null &&
             _prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
         {
            XmlOps.SetXmlNode(xmlData, "UnitLog/FullLogFile", instance.CachedFahLogName);
         }
         else
         {
            XmlOps.SetXmlNode(xmlData, "UnitLog/FullLogFile", String.Empty);
         }

         //    <LastUpdatedDate>10 August 2006</LastUpdatedDate>
         //    <LastUpdatedTime>9:25:23 pm</LastUpdatedTime>

         XmlOps.SetXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         // TODO: When localizing use ToString("h:mm:ss tt zzz", CultureInfo.CurrentCulture) instead.
         XmlOps.SetXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToString("h:mm:ss tt zzz", CultureInfo.InvariantCulture));

         //    <LastRetrievedDate>10 August 2006</LastRetrievedDate>
         //    <LastRetrievedTime>9:25:23 pm</LastRetrievedTime>

         XmlOps.SetXmlNode(xmlData, "LastRetrievedDate", instance.LastRetrievalTime.ToLongDateString());
         // TODO: When localizing use ToString("h:mm:ss tt zzz", CultureInfo.CurrentCulture) instead.
         XmlOps.SetXmlNode(xmlData, "LastRetrievedTime", instance.LastRetrievalTime.ToString("h:mm:ss tt zzz", CultureInfo.InvariantCulture));

         return xmlDoc;
      }

      private XmlDocument CreateSummaryXml(XmlDocument xmlDoc, IEnumerable<IDisplayInstance> instanceCollection)
      {
         var etaDate = _prefs.GetPreference<bool>(Preference.EtaDate);
         var completedCountDisplayType = _prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay);
         var showVersions = _prefs.GetPreference<bool>(Preference.ShowVersions);
         
         XmlElement xmlRootData = xmlDoc.DocumentElement;
         foreach (IDisplayInstance instance in instanceCollection)
         {
            var xmlFrag = new XmlDocument();
            xmlFrag.Load(Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XmlFolderName), "SummaryFrag.xml"));
            XmlElement xmlData = xmlFrag.DocumentElement;

            XmlOps.SetXmlNode(xmlData, "Status", instance.Status.ToString());
            XmlOps.SetXmlNode(xmlData, "StatusColor", PlatformOps.GetStatusHtmlColor(instance.Status));
            XmlOps.SetXmlNode(xmlData, "StatusFontColor", PlatformOps.GetStatusHtmlFontColor(instance.Status));
            XmlOps.SetXmlNode(xmlData, "PercentComplete", instance.PercentComplete.ToString());
            XmlOps.SetXmlNode(xmlData, "Name", instance.Name);
            XmlOps.SetXmlNode(xmlData, "UserIDDuplicate", instance.UserIdIsDuplicate.ToString());
            XmlOps.SetXmlNode(xmlData, "ClientType", instance.TypeOfClient.ToString());
            XmlOps.SetXmlNode(xmlData, "ClientVersion", instance.ClientVersion); // Issue 193
            XmlOps.SetXmlNode(xmlData, "TPF", instance.TPF.ToString());
            XmlOps.SetXmlNode(xmlData, "PPD", String.Format("{0:" + _prefs.PpdFormatString + "}", instance.PPD));
            XmlOps.SetXmlNode(xmlData, "UPD", String.Format("{0:0.00}", instance.UPD));
            XmlOps.SetXmlNode(xmlData, "MHz", instance.MHz.ToString());
            XmlOps.SetXmlNode(xmlData, "PPDMHz", instance.PPD_MHz.ToString());
            XmlOps.SetXmlNode(xmlData, "ETA", instance.ETA.ToString());
            if (instance.EtaDate.Equals(DateTime.MinValue))
            {
               XmlOps.SetXmlNode(xmlData, "ETADate", "Unknown");
            }
            else
            {
               XmlOps.SetXmlNode(xmlData, "ETADate", String.Format("{0} {1}", instance.EtaDate.ToShortDateString(), instance.EtaDate.ToShortTimeString()));
            }             
            XmlOps.SetXmlNode(xmlData, "ShowETADate", etaDate.ToString());
            XmlOps.SetXmlNode(xmlData, "Core", instance.CoreName);
            XmlOps.SetXmlNode(xmlData, "CoreVersion", instance.CoreVersion);
            XmlOps.SetXmlNode(xmlData, "CoreID", instance.CoreID); // Issue 193
            XmlOps.SetXmlNode(xmlData, "ProjectRunCloneGen", instance.ProjectRunCloneGen);
            XmlOps.SetXmlNode(xmlData, "ProjectDuplicate", instance.ProjectIsDuplicate.ToString());
            XmlOps.SetXmlNode(xmlData, "Credit", String.Format("{0:0}", instance.Credit));
            XmlOps.SetXmlNode(xmlData, "Completed", instance.TotalRunCompletedUnits.ToString());
            XmlOps.SetXmlNode(xmlData, "Failed", instance.TotalRunFailedUnits.ToString());
            XmlOps.SetXmlNode(xmlData, "TotalCompleted", instance.TotalClientCompletedUnits.ToString());
            XmlOps.SetXmlNode(xmlData, "CompletedCountDisplay", completedCountDisplayType.ToString());
            XmlOps.SetXmlNode(xmlData, "Username", instance.Username);
            XmlOps.SetXmlNode(xmlData, "UsernameMatch", instance.UsernameOk.ToString()); // Issue 51
            XmlOps.SetXmlNode(xmlData, "ShowVersions", showVersions.ToString()); // Issue 193
            if (instance.DownloadTime.Equals(DateTime.MinValue))
            {
               XmlOps.SetXmlNode(xmlData, "DownloadTime", "Unknown");
            }
            else
            {
               XmlOps.SetXmlNode(xmlData, "DownloadTime", String.Format("{0} {1}", instance.DownloadTime.ToShortDateString(), instance.DownloadTime.ToShortTimeString()));
            }
            if (instance.PreferredDeadline.Equals(DateTime.MinValue))
            {
               XmlOps.SetXmlNode(xmlData, "Deadline", "Unknown");
            }
            else
            {
               XmlOps.SetXmlNode(xmlData, "Deadline", String.Format("{0} {1}", instance.PreferredDeadline.ToShortDateString(), instance.PreferredDeadline.ToShortTimeString()));
            }

            XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
            xmlRootData.AppendChild(xImpNode);
         }

         return xmlDoc;
      }

      private XmlDocument CreateOverviewXml(InstanceTotals totals)
      {
         var xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.XmlFolderName), "Overview.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         XmlOps.SetXmlNode(xmlData, "HFMVersion", PlatformOps.ShortFormattedApplicationVersionWithRevision);
         XmlOps.SetXmlNode(xmlData, "TotalHosts", totals.TotalClients.ToString());
         XmlOps.SetXmlNode(xmlData, "GoodHosts", totals.WorkingClients.ToString());
         XmlOps.SetXmlNode(xmlData, "BadHosts", totals.NonWorkingClients.ToString());

         string ppdFormatString = _prefs.PpdFormatString;
         XmlOps.SetXmlNode(xmlData, "EstPPD", String.Format("{0:" + ppdFormatString + "}", totals.PPD));
         XmlOps.SetXmlNode(xmlData, "EstPPW", String.Format("{0:" + ppdFormatString + "}", totals.PPD * 7));
         XmlOps.SetXmlNode(xmlData, "EstUPD", String.Format("{0:" + ppdFormatString + "}", totals.UPD));
         XmlOps.SetXmlNode(xmlData, "EstUPW", String.Format("{0:" + ppdFormatString + "}", totals.UPD * 7));

         if (totals.WorkingClients > 0)
         {
            XmlOps.SetXmlNode(xmlData, "AvEstPPD", String.Format("{0:" + ppdFormatString + "}", totals.PPD / totals.WorkingClients));
            XmlOps.SetXmlNode(xmlData, "AvEstPPW", String.Format("{0:" + ppdFormatString + "}", totals.PPD * 7 / totals.WorkingClients));
            XmlOps.SetXmlNode(xmlData, "AvEstUPD", String.Format("{0:" + ppdFormatString + "}", totals.UPD / totals.WorkingClients));
            XmlOps.SetXmlNode(xmlData, "AvEstUPW", String.Format("{0:" + ppdFormatString + "}", totals.UPD * 7 / totals.WorkingClients));
         }
         else
         {
            XmlOps.SetXmlNode(xmlData, "AvEstPPD", "0");
            XmlOps.SetXmlNode(xmlData, "AvEstPPW", "0");
            XmlOps.SetXmlNode(xmlData, "AvEstUPD", "0");
            XmlOps.SetXmlNode(xmlData, "AvEstUPW", "0");
         }

         XmlOps.SetXmlNode(xmlData, "Completed", totals.TotalRunCompletedUnits.ToString());
         XmlOps.SetXmlNode(xmlData, "Failed", totals.TotalRunFailedUnits.ToString());
         XmlOps.SetXmlNode(xmlData, "TotalCompleted", totals.TotalClientCompletedUnits.ToString());
         XmlOps.SetXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         // TODO: When localizing use ToString("h:mm:ss tt zzz", CultureInfo.CurrentCulture) instead.
         XmlOps.SetXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToString("h:mm:ss tt zzz", CultureInfo.InvariantCulture));

         return xmlDoc;
      }
      
      #endregion
      
      #region Client Data Generation

      /// <summary>
      /// Generate Client Data File from the given Client Instances
      /// </summary>
      /// <param name="instances">Client Instances</param>
      public void GenerateClientData(IEnumerable<IClientInstance> instances)
      {
         try
         {
            ClientDataFilePath = DoClientDataGeneration(Path.GetTempPath(), instances);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      
      private static string DoClientDataGeneration(string folderPath, IEnumerable<IClientInstance> instances)
      {
         var list = (from instance in instances
                     where instance.Settings.ExternalInstance == false
                     select (DisplayInstance)instance.DisplayInstance).ToList();

         var filePath = Path.Combine(folderPath, Default.ExternalDataFileName);
         using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            ProtoBuf.Serializer.Serialize(fileStream, list);
         }
         return filePath;
      }
      
      #endregion
   }
}
