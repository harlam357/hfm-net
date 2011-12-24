/*
 * HFM.NET - Markup Generator Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Core.Serializers;

namespace HFM.Core
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
      IEnumerable<string> XmlFilePaths { get; }

      /// <summary>
      /// Contains HTML File Paths from most recent HTML Generation
      /// </summary>
      IEnumerable<string> HtmlFilePaths { get; }

      /// <summary>
      /// Generate Web Files from the given slot data.
      /// </summary>
      /// <param name="slots">Slot Models</param>
      /// <exception cref="ArgumentNullException">Throws if slots is null.</exception>
      /// <exception cref="InvalidOperationException">Throws if a Generate method is called in succession.</exception>
      void Generate(IEnumerable<SlotModel> slots);
   }

   public sealed class MarkupGenerator : IMarkupGenerator
   {
      #region Properties

      private volatile bool _generationInProgress;

      public bool GenerationInProgress
      {
         get { return _generationInProgress; }
      }

      public IEnumerable<string> XmlFilePaths { get; private set; }

      public IEnumerable<string> HtmlFilePaths { get; private set; }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      #endregion

      private readonly IPreferenceSet _prefs;
      
      public MarkupGenerator(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      public void Generate(IEnumerable<SlotModel> slots)
      {
         if (slots == null) throw new ArgumentNullException("slots");
         if (_generationInProgress) throw new InvalidOperationException("Markup Generation already in progress.");

         _generationInProgress = true;

         XmlFilePaths = null;
         HtmlFilePaths = null;

         try
         {
            var copyHtml = _prefs.Get<bool>(Preference.WebGenCopyHtml);
            var copyXml = _prefs.Get<bool>(Preference.WebGenCopyXml);
            //var copyClientData = _prefs.Get<bool>(Preference.WebGenCopyClientData);

            if (copyHtml)
            {
               // GenerateHtml calls GenerateXml - these two
               // calls are mutually exclusive
               GenerateHtml(slots);
            }
            else if (copyXml)
            {
               GenerateXml(slots);
            }

            //// Issue 79
            //if (copyClientData)
            //{
            //   GenerateClientData(slots);
            //}
         }
         finally
         {
            _generationInProgress = false;
         }
      }
      
      #region HTML Generation

      public void GenerateHtml(IEnumerable<SlotModel> slots)
      {
         try
         {
            HtmlFilePaths = DoHtmlGeneration(Path.GetTempPath(), slots);
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
      }
      
      private IEnumerable<string> DoHtmlGeneration(string folderPath, IEnumerable<SlotModel> slots)
      {
         Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
         Debug.Assert(slots != null);

         // Generate XML Files
         var xmlFiles = DoXmlGeneration(folderPath, slots);
            
         var fileList = new List<string>(slots.Count() + 4);
         var cssFileName = _prefs.Get<string>(Preference.CssFile);

         // Load the Overview XML
         var overviewXml = new XmlDocument();
         overviewXml.Load(xmlFiles.First());

         StreamWriter sw;
         // Generate the index page
         string filePath = Path.Combine(folderPath, "index.html");
         using (sw = new StreamWriter(filePath, false))
         {
            sw.Write(Transform(overviewXml, GetXsltFileName(Preference.WebOverview), cssFileName));
         }
         // Success, add it to the list
         fileList.Add(filePath);

         // Generate the mobile index page
         filePath = Path.Combine(folderPath, "mobile.html");
         using (sw = new StreamWriter(filePath, false))
         {
            sw.Write(Transform(overviewXml, GetXsltFileName(Preference.WebMobileOverview), cssFileName));
         }
         // Success, add it to the list
         fileList.Add(filePath);

         // Generate the summary page
         filePath = Path.Combine(folderPath, "summary.html");
         using (sw = new StreamWriter(filePath, false))
         {
            sw.Write(Transform(overviewXml, GetXsltFileName(Preference.WebSummary), cssFileName));
         }
         // Success, add it to the list
         fileList.Add(filePath);

         // Generate the mobile summary page
         filePath = Path.Combine(folderPath, "mobilesummary.html");
         using (sw = new StreamWriter(filePath, false))
         {
            sw.Write(Transform(overviewXml, GetXsltFileName(Preference.WebMobileSummary), cssFileName));
         }
         // Success, add it to the list
         fileList.Add(filePath);

         string instanceXslt = GetXsltFileName(Preference.WebInstance);
         // Generate a page per slot
         foreach (var slot in slots)
         {
            // Load the Instances XML
            var instancesXml = new XmlDocument();
            SlotModel slot1 = slot;
            string xmlFile = xmlFiles.FirstOrDefault(x => Path.GetFileName(x).StartsWith(slot1.Name));
            if (xmlFile != null)
            {
               instancesXml.Load(xmlFile);

               filePath = Path.Combine(folderPath, String.Concat(slot.Name, ".html"));
               using (sw = new StreamWriter(filePath, false))
               {
                  sw.Write(Transform(instancesXml, instanceXslt, cssFileName));
               }
               // Success, add it to the list
               fileList.Add(filePath);
            }
         }

         XmlFilePaths = xmlFiles;

         return fileList.AsReadOnly();
      }

      /// <summary>
      /// Transforms an XML Document using the given XSLT file
      /// </summary>
      /// <param name="xmlDoc">XML Source Document/Node</param>
      /// <param name="xsltFilePath">Path to the XSL Transform to apply</param>
      /// <param name="cssFileName">CSS file name to embed in the transformed XML</param>
      private static string Transform(XmlNode xmlDoc, string xsltFilePath, string cssFileName)
      {
         // Create XmlReaderSettings and XmlReader
         var xsltSettings = new XmlReaderSettings();
         xsltSettings.ProhibitDtd = false;
         XmlReader xmlReader = XmlReader.Create(xsltFilePath, xsltSettings);

         // Create the XslCompiledTransform and Load the XmlReader
         var xslt = new XslCompiledTransform();
         xslt.Load(xmlReader, XsltSettings.TrustedXslt, null);

         // Transform the XML data to an in memory stream
         var ms = new MemoryStream();
         xslt.Transform(xmlDoc, null, ms);

         // Return the transformed XML
         string webPage = Encoding.UTF8.GetString(ms.ToArray());
         webPage = webPage.Replace("$CSSFILE", cssFileName);
         return webPage;
      }

      private string GetXsltFileName(Preference p)
      {
         var xslt = _prefs.Get<string>(p);
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
      
      #endregion
      
      #region XML Generation

      public void GenerateXml(IEnumerable<SlotModel> slots)
      {
         try
         {
            XmlFilePaths = DoXmlGeneration(Path.GetTempPath(), slots);
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
      }

      private IEnumerable<string> DoXmlGeneration(string folderPath, IEnumerable<SlotModel> slots)
      {
         Debug.Assert(!String.IsNullOrEmpty(folderPath));
         Debug.Assert(slots != null);

         var fileList = new List<string>();

         DateTime updateDateTime = DateTime.Now;
         var slotSummary = CreateSlotSummary(slots, updateDateTime);
         var serializer = new XmlFileSerializer<SlotSummary>();
         string filePath = Path.Combine(Path.GetTempPath(), "SlotSummary.xml");
         fileList.Add(filePath);
         serializer.Serialize(filePath, slotSummary);

         var slotDetailSerializer = new XmlFileSerializer<SlotDetail>();
         foreach (var slot in slots)
         {
            var slotDetail = CreateSlotDetail(slot, updateDateTime);
            filePath = Path.Combine(Path.GetTempPath(), String.Concat(slot.Name, ".xml"));
            fileList.Add(filePath);
            slotDetailSerializer.Serialize(filePath, slotDetail);
         }

         return fileList.AsReadOnly();
      }

      private SlotSummary CreateSlotSummary(IEnumerable<SlotModel> slots, DateTime updateDateTime)
      {
         var slotSummary = new SlotSummary();
         slotSummary.HfmVersion = Application.VersionWithRevision;
         slotSummary.NumberFormat = _prefs.PpdFormatString;
         slotSummary.UpdateDateTime = updateDateTime;
         slotSummary.SlotTotals = slots.GetSlotTotals();
         slotSummary.Slots = slots.Select(AutoMapper.Mapper.Map<SlotModel, SlotData>).ToList();
         return slotSummary;
      }

      private SlotDetail CreateSlotDetail(SlotModel slot, DateTime updateDateTime)
      {
         var slotDetail = new SlotDetail();
         slotDetail.HfmVersion = Application.VersionWithRevision;
         slotDetail.NumberFormat = _prefs.PpdFormatString;
         slotDetail.UpdateDateTime = updateDateTime;
         slotDetail.LogFilesAvailable = _prefs.Get<bool>(Preference.WebGenCopyFAHlog);
         slotDetail.LogFileName = slot.Settings.CachedFahLogFileName();
         slotDetail.TotalRunCompletedUnits = slot.TotalRunCompletedUnits;
         slotDetail.TotalCompletedUnits = slot.TotalCompletedUnits;
         slotDetail.TotalRunFailedUnits = slot.TotalRunFailedUnits;
         slotDetail.GridData = AutoMapper.Mapper.Map<SlotModel, GridData>(slot);
         slotDetail.Protein = slot.UnitInfoLogic.CurrentProtein;
         return slotDetail;
      }
      
      #endregion
   }

   [DataContract(Namespace = "")]
   public class SlotSummary
   {
      [DataMember(Order = 1)]
      public string HfmVersion { get; set; }

      [DataMember(Order = 2)]
      public string NumberFormat { get; set; }

      [DataMember(Order = 3)]
      public DateTime UpdateDateTime { get; set; }

      [DataMember(Order = 4)]
      public SlotTotals SlotTotals { get; set; }

      [DataMember(Order = 5)]
      public List<SlotData> Slots { get; set; }
   }

   [DataContract(Namespace = "")]
   public class SlotData
   {
      [DataMember(Order = 1)]
      public string Arguments { get; set; }

      [DataMember(Order = 2)]
      public string ClientVersion { get; set; }

      [DataMember(Order = 3)]
      public string UserId { get; set; }

      [DataMember(Order = 4)]
      public int MachineId { get; set; }

      [DataMember(Order = 5)]
      public int TotalRunCompletedUnits { get; set; }

      [DataMember(Order = 6)]
      public int TotalCompletedUnits { get; set; }

      [DataMember(Order = 7)]
      public int TotalRunFailedUnits { get; set; }

      [DataMember(Order = 8)]
      public GridData GridData { get; set; }

      [DataMember(Order = 9)]
      public UnitInfo UnitInfo { get; set; }
   }

   [DataContract(Namespace = "")]
   public class GridData
   {
      [DataMember(Order = 1)]
      public SlotStatus Status { get; set; }

      [DataMember(Order = 2)]
      public string StatusColor { get; set; }

      [DataMember(Order = 3)]
      public string StatusFontColor { get; set; }

      [DataMember(Order = 4)]
      public int PercentComplete { get; set; }

      [DataMember(Order = 5)]
      public bool UserIdIsDuplicate { get; set; }

      [DataMember(Order = 6)]
      public string Name { get; set; }

      [DataMember(Order = 7)]
      public string SlotType { get; set; }

      [DataMember(Order = 8)]
      public string TPF { get; set; }

      [DataMember(Order = 9)]
      public double PPD { get; set; }

      [DataMember(Order = 10)]
      public double UPD { get; set; }

      [DataMember(Order = 11)]
      public int MHz { get; set; }

      [DataMember(Order = 12)]
      public double PPDMHz { get; set; }

      [DataMember(Order = 13)]
      public string ETA { get; set; }

      [DataMember(Order = 14)]
      public string Core { get; set; }

      [DataMember(Order = 15)]
      public string CoreId { get; set; }

      [DataMember(Order = 16)]
      public bool ProjectIsDuplicate { get; set; }

      [DataMember(Order = 17)]
      public string ProjectRunCloneGen { get; set; }

      [DataMember(Order = 18)]
      public double Credit { get; set; }

      [DataMember(Order = 19)]
      public int Completed { get; set; }

      [DataMember(Order = 20)]
      public int Failed { get; set; }

      [DataMember(Order = 21)]
      public bool UsernameOk { get; set; }

      [DataMember(Order = 22)]
      public string Username { get; set; }

      [DataMember(Order = 23)]
      public string DownloadTime { get; set; }

      [DataMember(Order = 24)]
      public string PreferredDeadline { get; set; }

      [DataMember(Order = 25)]
      public IList<LogLine> CurrentLogLines { get; set; }
   }

   [DataContract(Namespace = "")]
   public class SlotDetail
   {
      [DataMember(Order = 1)]
      public string HfmVersion { get; set; }

      [DataMember(Order = 2)]
      public string NumberFormat { get; set; }

      [DataMember(Order = 3)]
      public DateTime UpdateDateTime { get; set; }

      [DataMember(Order = 4)]
      public bool LogFilesAvailable { get; set; }

      [DataMember(Order = 5)]
      public string LogFileName { get; set; }

      [DataMember(Order = 6)]
      public int TotalRunCompletedUnits { get; set; }

      [DataMember(Order = 7)]
      public int TotalCompletedUnits { get; set; }

      [DataMember(Order = 8)]
      public int TotalRunFailedUnits { get; set; }

      [DataMember(Order = 9)]
      public GridData GridData { get; set; }

      [DataMember(Order = 10)]
      public Protein Protein { get; set; }
   }
}
