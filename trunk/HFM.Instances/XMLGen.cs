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
using System.Diagnostics;
using System.IO;
using System.Xml;

using HFM.Preferences;
using HFM.Helpers;

namespace HFM.Instances
{
   public static class XMLGen
   {
      /// <summary>
      /// Generates and transforms data for an Instance
      /// </summary>
      /// <param name="xslTransform">Filename of XSL transform to apply</param>
      /// <param name="Instance">Instance to use as data source</param>
      /// <returns>Transformed data</returns>
      public static String InstanceXml(String xslTransform, ClientInstance Instance)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Instance.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         //    <UnitInfo>
         //        <DateStarted>10 August 09:37:33</DateStarted>
         //        <FramesComplete>35</FramesComplete>
         //        <PercentComplete>35</PercentComplete>
         //        <TimePerFrame>1h 15m 44s</TimePerFrame>
         //        <ExpectedCompletionDate>16 August 2006 1:46 am</ExpectedCompletionDate>
         //    </UnitInfo>

         xmlData.SetAttribute("Name", Instance.InstanceName);
         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         string Version = String.Format("v{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                            fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
         XMLOps.setXmlNode(xmlData, "HFMVersion", Version);

         XMLOps.setXmlNode(xmlData, "UnitInfo/DateStarted", Instance.CurrentUnitInfo.DownloadTime.ToString("d MMMM yyyy hh:mm tt"));
         XMLOps.setXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", Instance.CurrentUnitInfo.FramesComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", Instance.CurrentUnitInfo.PercentComplete));
         XMLOps.setXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", Instance.CurrentUnitInfo.TimePerFrame.Hours, Instance.CurrentUnitInfo.TimePerFrame.Minutes, Instance.CurrentUnitInfo.TimePerFrame.Seconds));

         TimeSpan TotalCalcTime = new TimeSpan(Instance.CurrentUnitInfo.TimePerFrame.Hours * (Instance.CurrentUnitInfo.CurrentProtein.Frames - Instance.CurrentUnitInfo.FramesComplete),
                                               Instance.CurrentUnitInfo.TimePerFrame.Minutes * (Instance.CurrentUnitInfo.CurrentProtein.Frames - Instance.CurrentUnitInfo.FramesComplete),
                                               Instance.CurrentUnitInfo.TimePerFrame.Seconds * (Instance.CurrentUnitInfo.CurrentProtein.Frames - Instance.CurrentUnitInfo.FramesComplete));
         //TODO: Fix this issue with TimeOfLastFrame when fixing XML web output
         DateTime CompleteTime = DateTime.Today.Add(Instance.CurrentUnitInfo.TimeOfLastFrame);
         CompleteTime.Add(TotalCalcTime);
         XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", CompleteTime.ToLongDateString() + " at " + CompleteTime.ToLongTimeString());

         //    <Computer>
         //        <EstPPD>82.36</EstPPD>
         //        <EstPPW>576.54</EstPPW>
         //        <EstUPD>0.46</EstUPD>
         //        <EstUPW>3.77</EstUPW>
         //        <TotalProjects>243</TotalProjects>
         //    </Computer>

         XMLOps.setXmlNode(xmlData, "Computer/EstPPD", String.Format("{0:#,###,##0.00}", Instance.CurrentUnitInfo.PPD));
         XMLOps.setXmlNode(xmlData, "Computer/EstPPW", String.Format("{0:#,###,##0.00}", Instance.CurrentUnitInfo.PPD * 7));
         XMLOps.setXmlNode(xmlData, "Computer/EstUPD", String.Format("{0:#,###,##0.00}", Instance.CurrentUnitInfo.UPD));
         XMLOps.setXmlNode(xmlData, "Computer/EstUPW", String.Format("{0:#,###,##0.00}", Instance.CurrentUnitInfo.UPD * 7));
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
         XMLOps.setXmlNode(xmlData, "Protein/Description", ProteinData.DescriptionFromURL(Instance.CurrentUnitInfo.CurrentProtein.Description));
         XMLOps.setXmlNode(xmlData, "Protein/Contact", Instance.CurrentUnitInfo.CurrentProtein.Contact);

         //    <LastUpdatedDate>10 August 2006</LastUpdatedDate>
         //    <LastUpdatedTime>9:25:23 pm</LastUpdatedTime>

         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         //    <LastRetrievedDate>10 August 2006</LastRetrievedDate>
         //    <LastRetrievedTime>9:25:23 pm</LastRetrievedTime>

         XMLOps.setXmlNode(xmlData, "LastRetrievedDate", Instance.LastRetrievalTime.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastRetrievedTime", Instance.LastRetrievalTime.ToLongTimeString());

         return XMLOps.Transform(xmlDoc, xslTransform);
      }

      /// <summary>
      /// Generates the Summary page. xslTransform allows the calling method to select the appropriate transform.
      /// This allows the same summary method to generate the website page and the app page.
      /// </summary>
      /// <param name="xslTransform">Filename to use for Xsl transform</param>
      /// <param name="HostInstances">Collection of folding instances</param>
      /// <returns>HTML page as string</returns>
      public static String SummaryXml(String xslTransform, FoldingInstanceCollection HostInstances)
      {
         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Summary.xml"));
         XmlElement xmlRootData = xmlDoc.DocumentElement;
         
         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         string Version = String.Format("v{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                            fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
         XMLOps.setXmlNode(xmlRootData, "HFMVersion", Version);

         ClientInstance[] instances = new ClientInstance[HostInstances.Count];
         HostInstances.InstanceCollection.Values.CopyTo(instances, 0);

         Array.Sort(instances, delegate(ClientInstance instance1, ClientInstance instance2)
                               {
                                  return instance1.InstanceName.CompareTo(instance2.InstanceName);
                               });

         foreach (ClientInstance Instance in instances)
         {
            XmlDocument xmlFrag = new XmlDocument();
            xmlFrag.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "SummaryFrag.xml"));
            XmlElement xmlData = xmlFrag.DocumentElement;

            XMLOps.setXmlNode(xmlData, "Status", Instance.Status.ToString());
            XMLOps.setXmlNode(xmlData, "StatusColor", ClientInstance.GetStatusHtmlColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "StatusFontColor", ClientInstance.GetStatusHtmlFontColor(Instance.Status));
            XMLOps.setXmlNode(xmlData, "PercentComplete", Instance.CurrentUnitInfo.PercentComplete.ToString());
            XMLOps.setXmlNode(xmlData, "Name", Instance.InstanceName);
            XMLOps.setXmlNode(xmlData, "ClientType", Instance.CurrentUnitInfo.TypeOfClient.ToString());
            XMLOps.setXmlNode(xmlData, "TPF", Instance.CurrentUnitInfo.TimePerFrame.ToString());
            XMLOps.setXmlNode(xmlData, "PPD", String.Format("{0:0.00}", Instance.CurrentUnitInfo.PPD));
            XMLOps.setXmlNode(xmlData, "UPD", String.Format("{0:0.00}", Instance.CurrentUnitInfo.UPD));
            XMLOps.setXmlNode(xmlData, "MHz", Instance.ClientProcessorMegahertz.ToString());
            XMLOps.setXmlNode(xmlData, "PPDMHz", String.Format("{0:0.000}", Instance.CurrentUnitInfo.PPD / Instance.ClientProcessorMegahertz));
            XMLOps.setXmlNode(xmlData, "ETA", Instance.CurrentUnitInfo.ETA.ToString());
            XMLOps.setXmlNode(xmlData, "Core", Instance.CurrentUnitInfo.CurrentProtein.Core);
            XMLOps.setXmlNode(xmlData, "CoreVersion", Instance.CurrentUnitInfo.CoreVersion);
            XMLOps.setXmlNode(xmlData, "ProjectRunCloneGen", Instance.CurrentUnitInfo.ProjectRunCloneGen);
            XMLOps.setXmlNode(xmlData, "Credit", String.Format("{0:0}", Instance.CurrentUnitInfo.CurrentProtein.Credit));
            XMLOps.setXmlNode(xmlData, "Completed", Instance.NumberOfCompletedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Failed", Instance.NumberOfFailedUnitsSinceLastStart.ToString());
            XMLOps.setXmlNode(xmlData, "Username", String.Format("{0} ({1})", Instance.CurrentUnitInfo.FoldingID, Instance.CurrentUnitInfo.Team));
            XMLOps.setXmlNode(xmlData, "DownloadTime", String.Format("{0} {1}", Instance.CurrentUnitInfo.DownloadTime.ToShortDateString(), Instance.CurrentUnitInfo.DownloadTime.ToShortTimeString()));
            XMLOps.setXmlNode(xmlData, "Deadline", String.Format("{0} {1}", Instance.CurrentUnitInfo.Deadline.ToShortDateString(), Instance.CurrentUnitInfo.Deadline.ToShortTimeString()));

            XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
            xmlRootData.AppendChild(xImpNode);
         }

         XmlNode xLastUpdDate = xmlDoc.CreateNode(XmlNodeType.Element, "LastUpdatedDate", null);
         xLastUpdDate.InnerText = DateTime.Now.ToLongDateString();

         XmlNode xLastUpdTime = xmlDoc.CreateNode(XmlNodeType.Element, "LastUpdatedTime", null);
         xLastUpdTime.InnerText = DateTime.Now.ToLongTimeString();

         xmlRootData.AppendChild(xLastUpdDate);
         xmlRootData.AppendChild(xLastUpdTime);

         return XMLOps.Transform(xmlDoc, xslTransform);
      }

      /// <summary>
      /// Generates the Overview page. xslTransform allows the calling method to select the appropriate transform.
      /// This allows the same overview method to generate the website page and the app page.
      /// </summary>
      /// <param name="xslTransform">Filename to use for Xsl transform</param>
      /// <param name="HostInstances">Collection of folding instances</param>
      /// <returns>HTML page as string</returns>
      public static String OverviewXml(String xslTransform, FoldingInstanceCollection HostInstances)
      {
         InstanceTotals totals = HostInstances.GetInstanceTotals();

         XmlDocument xmlDoc = new XmlDocument();
         xmlDoc.Load(Path.Combine(Path.Combine(PreferenceSet.AppPath, "XML"), "Overview.xml"));
         XmlElement xmlData = xmlDoc.DocumentElement;

         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         string Version = String.Format(" v{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                             fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
         XMLOps.setXmlNode(xmlData, "HFMVersion", Version);

         //<Overview>
         //    <TotalHosts>0</TotalHosts>
         //    <GoodHosts>0</GoodHosts>
         //    <BadHosts>0</BadHosts>
         //</Overview>

         XMLOps.setXmlNode(xmlData, "TotalHosts", HostInstances.Count.ToString());
         XMLOps.setXmlNode(xmlData, "GoodHosts", totals.WorkingClients.ToString());
         XMLOps.setXmlNode(xmlData, "BadHosts", totals.NonWorkingClients.ToString());

         //    <EstPPD>0.00</EstPPD>
         //    <EstPPW>0.00</EstPPW>
         //    <EstUPD>0.00</EstUPD>
         //    <EstUPW>0.00</EstUPW>

         XMLOps.setXmlNode(xmlData, "EstPPD", String.Format("{0:0.00}", totals.PPD));
         XMLOps.setXmlNode(xmlData, "EstPPW", String.Format("{0:0.00}", totals.PPD * 7));
         XMLOps.setXmlNode(xmlData, "EstUPD", String.Format("{0:0.00}", totals.UPD));
         XMLOps.setXmlNode(xmlData, "EstUPW", String.Format("{0:0.00}", totals.UPD * 7));

         //    <AvEstPPD>0.00</AvEstPPD>
         //    <AvEstPPW>0.00</AvEstPPW>
         //    <AvEstUPD>0.00</AvEstUPD>
         //    <AvEstUPW>0.00</AvEstUPW>

         if (totals.WorkingClients > 0)
         {
            XMLOps.setXmlNode(xmlData, "AvEstPPD", String.Format("{0:0.00}", totals.PPD / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstPPW", String.Format("{0:0.00}", totals.PPD * 7 / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPD", String.Format("{0:0.00}", totals.UPD / totals.WorkingClients));
            XMLOps.setXmlNode(xmlData, "AvEstUPW", String.Format("{0:0.00}", totals.UPD * 7 / totals.WorkingClients));
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

         XMLOps.setXmlNode(xmlData, "TotalCompleted", totals.TotalCompletedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "TotalFailed", totals.TotalFailedUnits.ToString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
         XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

         return XMLOps.Transform(xmlDoc, xslTransform);
      }
   }
}
