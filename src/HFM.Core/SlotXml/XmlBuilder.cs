using System.ComponentModel;
using System.Drawing;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Serializers;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public readonly record struct XmlBuilderResult(string SlotSummaryFile, ICollection<string> SlotDetailFiles);

    public class XmlBuilder
    {
        public IPreferences Preferences { get; }

        private readonly IMapper _mapper;

        public XmlBuilder(IPreferences preferences)
        {
            Preferences = preferences;
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<XmlBuilderProfile>()).CreateMapper();
        }

        public XmlBuilderResult Build(ICollection<IClientData> collection, string path)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var updateDateTime = DateTime.Now;
            var slotSummaryFile = CreateSlotSummaryFile(collection, path, updateDateTime);
            var slotDetailFiles = EnumerateSlotDetailFiles(collection, path, updateDateTime).ToList();
            return new XmlBuilderResult(slotSummaryFile, slotDetailFiles);
        }

        private const string SlotSummaryXml = "SlotSummary.xml";

        private string CreateSlotSummaryFile(ICollection<IClientData> collection, string path, DateTime updateDateTime)
        {
            var slotSummary = CreateSlotSummary(collection, updateDateTime);

            var serializer = new DataContractFileSerializer<SlotSummary>();
            string filePath = Path.Combine(path, SlotSummaryXml);
            serializer.Serialize(filePath, slotSummary);
            return filePath;
        }

        internal SlotSummary CreateSlotSummary(ICollection<IClientData> collection, DateTime updateDateTime)
        {
            var slotSummary = new SlotSummary();
            slotSummary.HfmVersion = Application.Version;
            slotSummary.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotSummary.UpdateDateTime = updateDateTime;
            slotSummary.SlotTotals = SlotTotals.Create(collection);
            slotSummary.Slots = SortSlots(collection).Select(CreateSlotData).ToList();
            return slotSummary;
        }

        private IEnumerable<IClientData> SortSlots(IEnumerable<IClientData> collection)
        {
            string sortColumn = Preferences.Get<string>(Preference.FormSortColumn);
            if (String.IsNullOrWhiteSpace(sortColumn))
            {
                return collection;
            }

            var property = TypeDescriptor.GetProperties(typeof(IClientData)).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == sortColumn);
            if (property == null)
            {
                return collection;
            }

            var direction = Preferences.Get<ListSortDirection>(Preference.FormSortOrder);
            var sortComparer = new ClientDataSortComparer { OfflineClientsLast = Preferences.Get<bool>(Preference.OfflineLast) };
            sortComparer.SetSortProperties(property, direction);
            return collection.OrderBy(x => x, sortComparer);
        }

        private IEnumerable<string> EnumerateSlotDetailFiles(ICollection<IClientData> collection, string path, DateTime updateDateTime)
        {
            var serializer = new DataContractFileSerializer<SlotDetail>();
            foreach (var slot in collection)
            {
                var slotDetail = CreateSlotDetail(slot, updateDateTime);
                string filePath = Path.Combine(path, String.Concat(slot.Name, ".xml"));
                serializer.Serialize(filePath, slotDetail);
                yield return filePath;
            }
        }

        internal SlotDetail CreateSlotDetail(IClientData clientData, DateTime updateDateTime)
        {
            var slotDetail = new SlotDetail();
            slotDetail.HfmVersion = Application.Version;
            slotDetail.NumberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces), XsltNumberFormat);
            slotDetail.UpdateDateTime = updateDateTime;
            slotDetail.LogFileAvailable = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            slotDetail.LogFileName = clientData.ClientLogFileName;
            if (clientData is ICompletedFailedUnitsSource unitsSource)
            {
                slotDetail.TotalRunCompletedUnits = unitsSource.TotalRunCompletedUnits;
                slotDetail.TotalCompletedUnits = unitsSource.TotalCompletedUnits;
                slotDetail.TotalRunFailedUnits = unitsSource.TotalRunFailedUnits;
                slotDetail.TotalFailedUnits = unitsSource.TotalFailedUnits;
            }
            slotDetail.SlotData = CreateSlotData(clientData);
            return slotDetail;
        }

        internal SlotData CreateSlotData(IClientData clientData)
        {
            // to prevent large xml files, limit the number of log lines
            var maxLogLines = 500;

            var slotData = new SlotData();
            slotData.Status = clientData.Status.ToUserString();
            slotData.StatusColor = ColorTranslator.ToHtml(clientData.Status.GetStatusColor());
            slotData.StatusFontColor = ColorTranslator.ToHtml(HtmlBuilder.GetHtmlFontColor(clientData.Status));
            slotData.PercentComplete = clientData.PercentComplete;
            slotData.Name = clientData.Name;
            slotData.SlotType = clientData.SlotTypeString;
            slotData.Processor = clientData.Processor;
            slotData.ClientVersion = clientData.Platform?.ClientVersion;
            slotData.TPF = clientData.TPF.ToString();
            slotData.PPD = clientData.PPD;
            slotData.UPD = clientData.UPD;
            slotData.ETA = ToETAString(clientData);
            slotData.Core = clientData.Core ?? String.Empty;
            slotData.CoreId = clientData.CoreID ?? String.Empty;
            slotData.ProjectIsDuplicate = clientData.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key);
            slotData.ProjectRunCloneGen = clientData.ProjectRunCloneGen;
            slotData.Credit = clientData.Credit;
            slotData.Completed = clientData.Completed;
            slotData.Failed = clientData.Failed;
            if (clientData is ICompletedFailedUnitsSource unitsSource)
            {
                slotData.TotalRunCompletedUnits = unitsSource.TotalRunCompletedUnits;
                slotData.TotalCompletedUnits = unitsSource.TotalCompletedUnits;
                slotData.TotalRunFailedUnits = unitsSource.TotalRunFailedUnits;
                slotData.TotalFailedUnits = unitsSource.TotalFailedUnits;
            }
            slotData.UsernameOk = clientData.Errors.GetValue<bool>(ClientUsernameValidationRule.Key);
            slotData.Username = clientData.Username;
            slotData.DownloadTime = clientData.Assigned.ToShortStringOrEmpty();
            slotData.PreferredDeadline = clientData.PreferredDeadline.ToShortStringOrEmpty();
            var logLines = clientData.CurrentLogLines;
            slotData.CurrentLogLines = logLines.Skip(logLines.Count - maxLogLines).Select(x => _mapper.Map<Log.LogLine, LogLine>(x)).ToList();
            slotData.Protein = _mapper.Map<Proteins.Protein, Protein>(clientData.CurrentProtein);
            return slotData;
        }

        private string ToETAString(IClientData clientData)
        {
            var showETADate = Preferences.Get<bool>(Preference.DisplayEtaAsDate);
            return showETADate ? clientData.ETADate.ToShortStringOrEmpty() : clientData.ETA.ToString();
        }

        private const string XsltNumberFormat = "###,###,##0";
    }
}
