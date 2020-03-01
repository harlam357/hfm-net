
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.Serializers;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public struct XmlBuilderResult
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
        public IPreferenceSet Preferences { get; }

        public XmlBuilder(IPreferenceSet preferences)
        {
            Preferences = preferences;
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
            var slotSummary = new SlotSummary();
            slotSummary.HfmVersion = Application.VersionWithRevision;
            slotSummary.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces));
            slotSummary.UpdateDateTime = updateDateTime;
            slotSummary.SlotTotals = SlotTotals.Create(slots);
            slotSummary.Slots = SortSlots(slots).Select(AutoMapper.Mapper.Map<SlotModel, SlotData>).ToList();

            var serializer = new DataContractFileSerializer<SlotSummary>();
            string filePath = Path.Combine(path, SlotSummaryXml);
            serializer.Serialize(filePath, slotSummary);
            return filePath;
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

        private SlotDetail CreateSlotDetail(SlotModel slot, DateTime updateDateTime)
        {
            var slotDetail = new SlotDetail();
            slotDetail.HfmVersion = Application.VersionWithRevision;
            slotDetail.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces));
            slotDetail.UpdateDateTime = updateDateTime;
            slotDetail.LogFileAvailable = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            slotDetail.LogFileName = slot.Settings.ClientLogFileName;
            slotDetail.TotalRunCompletedUnits = slot.TotalRunCompletedUnits;
            slotDetail.TotalCompletedUnits = slot.TotalCompletedUnits;
            slotDetail.TotalRunFailedUnits = slot.TotalRunFailedUnits;
            slotDetail.TotalFailedUnits = slot.TotalFailedUnits;
            slotDetail.SlotData = AutoMapper.Mapper.Map<SlotModel, SlotData>(slot);
            return slotDetail;
        }
    }
}
