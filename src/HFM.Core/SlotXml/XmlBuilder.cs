using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Serializers;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public readonly struct XmlBuilderResult
    {
        public XmlBuilderResult(string slotSummaryFile, ICollection<string> slotDetailFiles)
        {
            SlotSummaryFile = slotSummaryFile;
            SlotDetailFiles = slotDetailFiles;
        }

        public string SlotSummaryFile { get; }

        public ICollection<string> SlotDetailFiles { get; }
    }

    public class XmlBuilder
    {
        public IPreferences Preferences { get; }

        private readonly IMapper _mapper;

        public XmlBuilder(IPreferences preferences)
        {
            Preferences = preferences;
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<XmlBuilderProfile>()).CreateMapper();
        }

        public XmlBuilderResult Build(ICollection<SlotModel> slots, string path)
        {
            if (slots == null) throw new ArgumentNullException(nameof(slots));

            var updateDateTime = DateTime.Now;
            var slotSummaryFile = CreateSlotSummaryFile(slots, path, updateDateTime);
            var slotDetailFiles = EnumerateSlotDetailFiles(slots, path, updateDateTime).ToList();
            return new XmlBuilderResult(slotSummaryFile, slotDetailFiles);
        }

        private const string SlotSummaryXml = "SlotSummary.xml";

        private string CreateSlotSummaryFile(ICollection<SlotModel> slots, string path, DateTime updateDateTime)
        {
            var slotSummary = CreateSlotSummary(slots, updateDateTime);

            var serializer = new DataContractFileSerializer<SlotSummary>();
            string filePath = Path.Combine(path, SlotSummaryXml);
            serializer.Serialize(filePath, slotSummary);
            return filePath;
        }

        internal SlotSummary CreateSlotSummary(ICollection<SlotModel> slots, DateTime updateDateTime)
        {
            var slotSummary = new SlotSummary();
            slotSummary.HfmVersion = Application.Version;
            slotSummary.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotSummary.UpdateDateTime = updateDateTime;
            slotSummary.SlotTotals = SlotTotals.Create(slots);
            slotSummary.Slots = SortSlots(slots).Select(CreateSlotData).ToList();
            return slotSummary;
        }

        private IEnumerable<SlotModel> SortSlots(IEnumerable<SlotModel> slots)
        {
            string sortColumn = Preferences.Get<string>(Preference.FormSortColumn);
            if (String.IsNullOrWhiteSpace(sortColumn))
            {
                return slots;
            }

            var property = TypeDescriptor.GetProperties(typeof(SlotModel)).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == sortColumn);
            if (property == null)
            {
                return slots;
            }

            var direction = Preferences.Get<ListSortDirection>(Preference.FormSortOrder);
            var sortComparer = new SlotModelSortComparer { OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast) };
            sortComparer.SetSortProperties(property, direction);
            return slots.OrderBy(x => x, sortComparer);
        }

        private IEnumerable<string> EnumerateSlotDetailFiles(ICollection<SlotModel> slots, string path, DateTime updateDateTime)
        {
            var serializer = new DataContractFileSerializer<SlotDetail>();
            foreach (var slot in slots)
            {
                var slotDetail = CreateSlotDetail(slot, updateDateTime);
                string filePath = Path.Combine(path, String.Concat(slot.Name, ".xml"));
                serializer.Serialize(filePath, slotDetail);
                yield return filePath;
            }
        }

        internal SlotDetail CreateSlotDetail(SlotModel slot, DateTime updateDateTime)
        {
            var slotDetail = new SlotDetail();
            slotDetail.HfmVersion = Application.Version;
            slotDetail.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotDetail.UpdateDateTime = updateDateTime;
            slotDetail.LogFileAvailable = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            slotDetail.LogFileName = slot.Client.Settings.ClientLogFileName;
            if (slot is ICompletedFailedUnitsSource unitsSource)
            {
                slotDetail.TotalRunCompletedUnits = unitsSource.TotalRunCompletedUnits;
                slotDetail.TotalCompletedUnits = unitsSource.TotalCompletedUnits;
                slotDetail.TotalRunFailedUnits = unitsSource.TotalRunFailedUnits;
                slotDetail.TotalFailedUnits = unitsSource.TotalFailedUnits;
            }
            slotDetail.SlotData = CreateSlotData(slot);
            return slotDetail;
        }

        internal SlotData CreateSlotData(SlotModel slot)
        {
            // to prevent large xml files, limit the number of log lines
            var maxLogLines = 500;

            var slotData = new SlotData();
            slotData.Status = slot.Status.ToUserString();
            slotData.StatusColor = ColorTranslator.ToHtml(slot.Status.GetStatusColor());
            slotData.StatusFontColor = ColorTranslator.ToHtml(HtmlBuilder.GetHtmlFontColor(slot.Status));
            slotData.PercentComplete = slot.PercentComplete;
            slotData.Name = slot.Name;
            slotData.SlotType = slot.SlotTypeString;
            slotData.Processor = slot.Processor;
            slotData.ClientVersion = slot.Client.ClientVersion;
            slotData.TPF = slot.TPF.ToString();
            slotData.PPD = slot.PPD;
            slotData.UPD = slot.UPD;
            slotData.ETA = ToETAString(slot);
            slotData.Core = slot.Core ?? String.Empty;
            slotData.CoreId = slot.CoreID ?? String.Empty;
            slotData.ProjectIsDuplicate = slot.ProjectIsDuplicate;
            slotData.ProjectRunCloneGen = slot.ProjectRunCloneGen;
            slotData.Credit = slot.Credit;
            slotData.Completed = slot.Completed;
            slotData.Failed = slot.Failed;
            if (slot is ICompletedFailedUnitsSource unitsSource)
            {
                slotData.TotalRunCompletedUnits = unitsSource.TotalRunCompletedUnits;
                slotData.TotalCompletedUnits = unitsSource.TotalCompletedUnits;
                slotData.TotalRunFailedUnits = unitsSource.TotalRunFailedUnits;
                slotData.TotalFailedUnits = unitsSource.TotalFailedUnits;
            }
            slotData.UsernameOk = slot.UsernameOk;
            slotData.Username = slot.Username;
            slotData.DownloadTime = slot.Assigned.ToShortStringOrEmpty();
            slotData.PreferredDeadline = slot.PreferredDeadline.ToShortStringOrEmpty();
            slotData.CurrentLogLines = slot.CurrentLogLines.Skip(slot.CurrentLogLines.Count - maxLogLines).Select(x => _mapper.Map<Log.LogLine, LogLine>(x)).ToList();
            slotData.Protein = _mapper.Map<Proteins.Protein, Protein>(slot.WorkUnitModel.CurrentProtein);
            return slotData;
        }

        private static string ToETAString(SlotModel slotModel)
        {
            var showETADate = slotModel.Client.Preferences.Get<bool>(Preference.DisplayEtaAsDate);
            return showETADate ? slotModel.ETADate.ToShortStringOrEmpty() : slotModel.ETA.ToString();
        }

        private const string XsltNumberFormat = "###,###,##0";
    }
}
