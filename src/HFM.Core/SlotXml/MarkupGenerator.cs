/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

using Castle.Core.Logging;

using HFM.Core.Client;
using HFM.Core.Serializers;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public class MarkupGenerator
    {
        #region Properties

        public IEnumerable<string> XmlFilePaths { get; private set; }

        public IEnumerable<string> HtmlFilePaths { get; private set; }

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        #endregion

        private readonly IPreferenceSet _prefs;

        public MarkupGenerator(IPreferenceSet prefs)
        {
            _prefs = prefs;
        }

        public void Generate(ICollection<SlotModel> slots)
        {
            if (slots == null) throw new ArgumentNullException("slots");

            XmlFilePaths = null;
            HtmlFilePaths = null;

            var copyHtml = _prefs.Get<bool>(Preference.WebGenCopyHtml);
            var copyXml = _prefs.Get<bool>(Preference.WebGenCopyXml);

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
        }

        #region HTML Generation

        internal void GenerateHtml(ICollection<SlotModel> slots)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "hfm-" + Environment.UserName);
                Directory.CreateDirectory(tempPath);
                HtmlFilePaths = GenerateHtmlInternal(tempPath, slots);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private IEnumerable<string> GenerateHtmlInternal(string folderPath, ICollection<SlotModel> slots)
        {
            Debug.Assert(String.IsNullOrEmpty(folderPath) == false);
            Debug.Assert(slots != null);

            // Generate XML Files
            var xmlFiles = GenerateXmlInternal(folderPath, slots);

            var fileList = new List<string>(slots.Count + 4);
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

            string slotXslt = GetXsltFileName(Preference.WebSlot);
            // Generate a page per slot
            foreach (var slot in slots)
            {
                // Load the Instances XML
                var slotXml = new XmlDocument();
                SlotModel slot1 = slot;
                string xmlFile = xmlFiles.FirstOrDefault(x => Path.GetFileName(x).StartsWith(slot1.Name));
                if (xmlFile != null)
                {
                    slotXml.Load(xmlFile);

                    filePath = Path.Combine(folderPath, String.Concat(slot.Name, ".html"));
                    using (sw = new StreamWriter(filePath, false))
                    {
                        sw.Write(Transform(slotXml, slotXslt, cssFileName));
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
            // Create the XslCompiledTransform and Load the XmlReader
            var xslt = new XslCompiledTransform();
            using (var xmlReader = new XmlTextReader(xsltFilePath))
            {
                xslt.Load(xmlReader, null, new XmlUrlResolver());
            }

            // Transform the XML data to an in memory stream
            using (var ms = new MemoryStream())
            {
                xslt.Transform(xmlDoc, null, ms);

                // Return the transformed XML
                string webPage = Encoding.UTF8.GetString(ms.ToArray());
                webPage = webPage.Replace("$CSSFILE", cssFileName);
                return webPage;
            }
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
                string xsltFileName = Path.Combine(_prefs.Get<string>(Preference.ApplicationPath), Constants.XsltFolderName, xslt);
                if (File.Exists(xsltFileName))
                {
                    return xsltFileName;
                }
            }

            throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
               "XSLT File '{0}' cannot be found.", xslt));
        }

        internal static Color GetHtmlFontColor(SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.RunningNoFrameTimes:
                case SlotStatus.Paused:
                case SlotStatus.Finishing:
                case SlotStatus.Offline:
                    return Color.Black;
                default:
                    return Color.White;
            }
        }

        #endregion

        #region XML Generation

        internal void GenerateXml(ICollection<SlotModel> slots)
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "hfm-" + Environment.UserName);
                Directory.CreateDirectory(tempPath);
                XmlFilePaths = GenerateXmlInternal(tempPath, slots);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        private ICollection<string> GenerateXmlInternal(string folderPath, ICollection<SlotModel> slots)
        {
            Debug.Assert(!String.IsNullOrEmpty(folderPath));
            Debug.Assert(slots != null);

            var fileList = new List<string>();

            DateTime updateDateTime = DateTime.Now;
            var slotSummary = CreateSlotSummary(slots, updateDateTime);
            var serializer = new DataContractFileSerializer<SlotSummary>();
            string filePath = Path.Combine(folderPath, "SlotSummary.xml");
            fileList.Add(filePath);
            serializer.Serialize(filePath, slotSummary);

            var slotDetailSerializer = new DataContractFileSerializer<SlotDetail>();
            foreach (var slot in slots)
            {
                var slotDetail = CreateSlotDetail(slot, updateDateTime);
                filePath = Path.Combine(folderPath, String.Concat(slot.Name, ".xml"));
                fileList.Add(filePath);
                slotDetailSerializer.Serialize(filePath, slotDetail);
            }

            return fileList.AsReadOnly();
        }

        private SlotSummary CreateSlotSummary(ICollection<SlotModel> slots, DateTime updateDateTime)
        {
            var slotSummary = new SlotSummary();
            slotSummary.HfmVersion = Application.VersionWithRevision;
            slotSummary.NumberFormat = _prefs.GetPpdFormatString();
            slotSummary.UpdateDateTime = updateDateTime;
            slotSummary.SlotTotals = SlotTotals.Create(slots);
            slotSummary.Slots = SortSlots(slots).Select(AutoMapper.Mapper.Map<SlotModel, SlotData>).ToList();
            return slotSummary;
        }

        private ICollection<SlotModel> SortSlots(ICollection<SlotModel> slots)
        {
            string sortColumn = _prefs.Get<string>(Preference.FormSortColumn);
            if (String.IsNullOrWhiteSpace(sortColumn))
            {
                return slots;
            }
            var property = TypeDescriptor.GetProperties(typeof(SlotModel)).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == sortColumn);
            if (property == null)
            {
                return slots;
            }
            var direction = _prefs.Get<ListSortDirection>(Preference.FormSortOrder);
            var sortComparer = new SlotModelSortComparer { OfflineClientsLast = _prefs.Get<bool>(Preference.OfflineLast) };
            sortComparer.SetSortProperties(property, direction);
            return slots.OrderBy(x => x, sortComparer).ToList();
        }

        private SlotDetail CreateSlotDetail(SlotModel slot, DateTime updateDateTime)
        {
            var slotDetail = new SlotDetail();
            slotDetail.HfmVersion = Application.VersionWithRevision;
            slotDetail.NumberFormat = _prefs.GetPpdFormatString();
            slotDetail.UpdateDateTime = updateDateTime;
            slotDetail.LogFileAvailable = _prefs.Get<bool>(Preference.WebGenCopyFAHlog);
            slotDetail.LogFileName = slot.Settings.ClientLogFileName;
            slotDetail.TotalRunCompletedUnits = slot.TotalRunCompletedUnits;
            slotDetail.TotalCompletedUnits = slot.TotalCompletedUnits;
            slotDetail.TotalRunFailedUnits = slot.TotalRunFailedUnits;
            slotDetail.TotalFailedUnits = slot.TotalFailedUnits;
            slotDetail.SlotData = AutoMapper.Mapper.Map<SlotModel, SlotData>(slot);
            return slotDetail;
        }

        #endregion
    }
}
