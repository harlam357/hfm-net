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
using System.Xml;

using HFM.Preferences;
using HFM.Helpers;
using HFM.Proteins;

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
            xmlDoc.Load(PreferenceSet.Instance.AppPath + "\\XML\\Instance.XML");
            XmlElement xmlData = xmlDoc.DocumentElement;

            //    <UnitInfo>
            //        <DateStarted>10 August 09:37:33</DateStarted>
            //        <FramesComplete>35</FramesComplete>
            //        <PercentComplete>35</PercentComplete>
            //        <TimePerFrame>1h 15m 44s</TimePerFrame>
            //        <ExpectedCompletionDate>16 August 2006 1:46 am</ExpectedCompletionDate>
            //    </UnitInfo>

            xmlData.SetAttribute("Name", Instance.InstanceName);

            XMLOps.setXmlNode(xmlData, "UnitInfo/DateStarted", Instance.UnitInfo.DownloadTime.ToString("d MMMM yyyy hh:mm tt"));
            XMLOps.setXmlNode(xmlData, "UnitInfo/FramesComplete", String.Format("{0}", Instance.UnitInfo.FramesComplete));
            XMLOps.setXmlNode(xmlData, "UnitInfo/PercentComplete", String.Format("{0}", Instance.UnitInfo.PercentComplete));
            XMLOps.setXmlNode(xmlData, "UnitInfo/TimePerFrame", String.Format("{0}h, {1}m, {2}s", Instance.UnitInfo.TimePerFrame.Hours, Instance.UnitInfo.TimePerFrame.Minutes, Instance.UnitInfo.TimePerFrame.Seconds));

            TimeSpan TotalCalcTime = new TimeSpan(Instance.UnitInfo.TimePerFrame.Hours * (Instance.CurrentProtein.Frames - Instance.UnitInfo.FramesComplete),
                                                  Instance.UnitInfo.TimePerFrame.Minutes * (Instance.CurrentProtein.Frames - Instance.UnitInfo.FramesComplete),
                                                  Instance.UnitInfo.TimePerFrame.Seconds * (Instance.CurrentProtein.Frames - Instance.UnitInfo.FramesComplete));
            //TODO: Fix this issue with TimeOfLastFrame when fixing XML web output
            DateTime CompleteTime = DateTime.Today.Add(Instance.UnitInfo.TimeOfLastFrame);
            CompleteTime.Add(TotalCalcTime);
            XMLOps.setXmlNode(xmlData, "UnitInfo/ExpectedCompletionDate", CompleteTime.ToLongDateString() + " at " + CompleteTime.ToLongTimeString());

            //    <Computer>
            //        <EstPPD>82.36</EstPPD>
            //        <EstPPW>576.54</EstPPW>
            //        <EstUPD>0.46</EstUPD>
            //        <EstUPW>3.77</EstUPW>
            //        <TotalProjects>243</TotalProjects>
            //    </Computer>

            XMLOps.setXmlNode(xmlData, "Computer/EstPPD", String.Format("{0:#,###,##0.00}", Instance.UnitInfo.PPD));
            XMLOps.setXmlNode(xmlData, "Computer/EstPPW", String.Format("{0:#,###,##0.00}", Instance.UnitInfo.PPD * 7));
            XMLOps.setXmlNode(xmlData, "Computer/EstUPD", String.Format("{0:#,###,##0.00}", Instance.UnitInfo.UPD));
            XMLOps.setXmlNode(xmlData, "Computer/EstUPW", String.Format("{0:#,###,##0.00}", Instance.UnitInfo.UPD * 7));
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

            XMLOps.setXmlNode(xmlData, "Protein/ProjectNumber", Instance.CurrentProtein.ProjectNumber.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/ServerIP", Instance.CurrentProtein.ServerIP);
            XMLOps.setXmlNode(xmlData, "Protein/WorkUnit", Instance.UnitInfo.ProteinName);
            XMLOps.setXmlNode(xmlData, "Protein/NumAtoms", Instance.CurrentProtein.NumAtoms.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/PreferredDays", Instance.CurrentProtein.PreferredDays.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/MaxDays", Instance.CurrentProtein.MaxDays.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/Credit", Instance.CurrentProtein.Credit.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/Frames", Instance.CurrentProtein.Frames.ToString());
            XMLOps.setXmlNode(xmlData, "Protein/Core", Instance.CurrentProtein.Core);
            XMLOps.setXmlNode(xmlData, "Protein/Description", ProteinData.DescriptionFromURL(Instance.CurrentProtein.Description));
            XMLOps.setXmlNode(xmlData, "Protein/Contact", Instance.CurrentProtein.Contact);

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
            xmlDoc.Load(PreferenceSet.Instance.AppPath + "\\XML\\Summary.XML");
            XmlElement xmlRootData = xmlDoc.DocumentElement;

            ClientInstance[] instances = new ClientInstance[HostInstances.Count];
            HostInstances.InstanceCollection.Values.CopyTo(instances, 0);

            Array.Sort(instances, delegate(ClientInstance instance1, ClientInstance instance2)
                                  {
                                     return instance1.InstanceName.CompareTo(instance2.InstanceName);
                                  });

            foreach (ClientInstance Instance in instances)
            {
                //ClientInstance Instance = kvp.Value;
                XmlDocument xmlFrag = new XmlDocument();
                xmlFrag.Load(PreferenceSet.Instance.AppPath + "\\XML\\SummaryFrag.XML");
                XmlElement xmlData = xmlFrag.DocumentElement;

                XMLOps.setXmlNode(xmlData, "Name", Instance.InstanceName);
                XMLOps.setXmlNode(xmlData, "PercentComplete", Instance.UnitInfo.PercentComplete.ToString());
                XMLOps.setXmlNode(xmlData, "Credit", String.Format("{0:0}", Instance.CurrentProtein.Credit));
                XMLOps.setXmlNode(xmlData, "PPD", String.Format("{0:0.00}", Instance.UnitInfo.PPD));
                XMLOps.setXmlNode(xmlData, "PPW", String.Format("{0:0.00}", 7 * Instance.UnitInfo.PPD));
                XMLOps.setXmlNode(xmlData, "UPD", String.Format("{0:0.00}", Instance.UnitInfo.UPD));
                XMLOps.setXmlNode(xmlData, "UPW", String.Format("{0:0.00}", 7 * Instance.UnitInfo.UPD));
                XMLOps.setXmlNode(xmlData, "Completed", Instance.NumberOfCompletedUnitsSinceLastStart.ToString());
                XMLOps.setXmlNode(xmlData, "Failed", Instance.NumberOfFailedUnitsSinceLastStart.ToString());
                XMLOps.setXmlNode(xmlData, "LastUpdate", Instance.LastRetrievalTime.ToString("d-MMM hh:mm tt"));

                Int32 addSeconds = Convert.ToInt32((Instance.CurrentProtein.Frames - Instance.UnitInfo.FramesComplete) * Instance.UnitInfo.TimePerFrame.TotalSeconds);
                DateTime completeTime = DateTime.Now + new TimeSpan(0, 0, addSeconds);
                XMLOps.setXmlNode(xmlData, "CompleteTime", completeTime.ToLocalTime().ToString("d-MMM h:mm tt"));

                XmlNode xImpNode = xmlDoc.ImportNode(xmlFrag.ChildNodes[0], true);
                xmlRootData.AppendChild(xImpNode);
            }

            //    <LastUpdatedDate>10 August 2006</LastUpdatedDate>
            //    <LastUpdatedTime>9:25:23 pm</LastUpdatedTime>

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
            Int32 badHostCount = 0, goodHostCount = 0, totalCompleted = 0, totalFailed = 0;
            Double TotalPPD = 0, TotalUPD = 0;

            foreach (KeyValuePair<String, ClientInstance> kvp in HostInstances.InstanceCollection)
            {
               TotalPPD += kvp.Value.UnitInfo.PPD;
               TotalUPD += kvp.Value.UnitInfo.UPD;
               //totalCompleted += kvp.Value.TotalUnits;
               totalCompleted += kvp.Value.NumberOfCompletedUnitsSinceLastStart;
               totalFailed += kvp.Value.NumberOfFailedUnitsSinceLastStart;

               switch (kvp.Value.Status)
               {
                  case ClientStatus.Running:
                  case ClientStatus.RunningNoFrameTimes:
                     goodHostCount++;
                     break;
                  //case ClientStatus.Paused:
                  //   newPausedHosts++;
                  //   break;
                  //case ClientStatus.Hung:
                  //   newHungHosts++;
                  //   break;
                  //case ClientStatus.Stopped:
                  //   newStoppedHosts++;
                  //   break;
                  //case ClientStatus.Offline:
                  //case ClientStatus.Unknown:
                  //   newOfflineUnknownHosts++;
                  //   break;
                  default:
                     badHostCount++;
                     break;
               }
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(PreferenceSet.Instance.AppPath + "\\XML\\Overview.XML");
            XmlElement xmlData = xmlDoc.DocumentElement;

            //<Overview>
            //    <TotalHosts>0</TotalHosts>
            //    <GoodHosts>0</GoodHosts>
            //    <BadHosts>0</BadHosts>
            //</Overview>

            XMLOps.setXmlNode(xmlData, "TotalHosts", HostInstances.Count.ToString());
            XMLOps.setXmlNode(xmlData, "GoodHosts", goodHostCount.ToString());
            XMLOps.setXmlNode(xmlData, "BadHosts", badHostCount.ToString());

            //    <EstPPD>0.00</EstPPD>
            //    <EstPPW>0.00</EstPPW>
            //    <EstUPD>0.00</EstUPD>
            //    <EstUPW>0.00</EstUPW>

            XMLOps.setXmlNode(xmlData, "EstPPD", String.Format("{0:0.00}", TotalPPD));
            XMLOps.setXmlNode(xmlData, "EstPPW", String.Format("{0:0.00}", TotalPPD * 7));
            XMLOps.setXmlNode(xmlData, "EstUPD", String.Format("{0:0.00}", TotalUPD));
            XMLOps.setXmlNode(xmlData, "EstUPW", String.Format("{0:0.00}", TotalUPD * 7));

            //    <AvEstPPD>0.00</AvEstPPD>
            //    <AvEstPPW>0.00</AvEstPPW>
            //    <AvEstUPD>0.00</AvEstUPD>
            //    <AvEstUPW>0.00</AvEstUPW>

            if (goodHostCount > 0)
            {
                XMLOps.setXmlNode(xmlData, "AvEstPPD", String.Format("{0:0.00}", TotalPPD / goodHostCount));
                XMLOps.setXmlNode(xmlData, "AvEstPPW", String.Format("{0:0.00}", TotalPPD * 7 / goodHostCount));
                XMLOps.setXmlNode(xmlData, "AvEstUPD", String.Format("{0:0.00}", TotalUPD / goodHostCount));
                XMLOps.setXmlNode(xmlData, "AvEstUPW", String.Format("{0:0.00}", TotalUPD * 7 / goodHostCount));
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

            XMLOps.setXmlNode(xmlData, "TotalCompleted", totalCompleted.ToString());
            XMLOps.setXmlNode(xmlData, "TotalFailed", totalFailed.ToString());
            XMLOps.setXmlNode(xmlData, "LastUpdatedDate", DateTime.Now.ToLongDateString());
            XMLOps.setXmlNode(xmlData, "LastUpdatedTime", DateTime.Now.ToLongTimeString());

            return XMLOps.Transform(xmlDoc, xslTransform);
        }

    }
}
