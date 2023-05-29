using System.Globalization;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public class TextBenchmarksReport : BenchmarksReport
    {
        public const string KeyName = "Text";

        public IPreferences Preferences { get; }
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkRepository Benchmarks { get; }
        public ClientConfiguration ClientConfiguration { get; }

        public TextBenchmarksReport(IPreferences preferences, IProteinService proteinService,
            IProteinBenchmarkRepository benchmarks, ClientConfiguration clientConfiguration) : base(KeyName)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            ProteinService = proteinService ?? NullProteinService.Instance;
            Benchmarks = benchmarks ?? NullProteinBenchmarkRepository.Instance;
            ClientConfiguration = clientConfiguration;
        }

        public override async Task Generate(IBenchmarksReportSource source)
        {
            var benchmarkText = new List<string>();

            var slotIdentifier = source.SlotIdentifier;
            var projects = source.Projects;
            if (slotIdentifier is null || projects.Count == 0)
            {
                Result = benchmarkText;
                return;
            }

            string numberFormat = NumberFormat.Get(Preferences.Get<int>(Preference.DecimalPlaces));
            var bonusCalculation = Preferences.Get<BonusCalculation>(Preference.BonusCalculation);
            bool calculateBonus = bonusCalculation != BonusCalculation.None;
            var benchmarks = (await Benchmarks.GetBenchmarksAsync(slotIdentifier.Value, projects).ConfigureAwait(true))
                .OrderBy(x => x.SlotIdentifier.Name)
                .ThenBy(x => x.BenchmarkIdentifier.Threads)
                .GroupBy(x => x.BenchmarkIdentifier.ProjectID)
                .OrderBy(x => x.Key);

            foreach (var group in benchmarks)
            {
                int projectID = group.Key;
                var protein = ProteinService.Get(projectID);
                if (protein is null)
                {
                    benchmarkText.Add(String.Format(CultureInfo.InvariantCulture, " Project ID: {0} Not Found", projectID));
                    benchmarkText.AddRange(Enumerable.Repeat(String.Empty, 2));
                    continue;
                }

                benchmarkText
                    .AddRange(EnumerateProjectInformation(protein)
                        .Concat(Enumerable.Repeat(String.Empty, 2)));

                foreach (var b in group)
                {
                    benchmarkText
                        .AddRange(EnumerateBenchmarkInformation(protein, b, numberFormat, calculateBonus)
                            .Concat(EnumerateClientInformation(FindRunningClient(b), numberFormat, bonusCalculation))
                            .Concat(Enumerable.Repeat(String.Empty, 2)));
                }
            }

            Result = benchmarkText;
        }

        private static IEnumerable<string> EnumerateBenchmarkInformation(Protein protein, ProteinBenchmark benchmark, string numberFormat, bool calculateBonus)
        {
            yield return $" Name: {benchmark.SlotIdentifier.Name}";
            yield return $" Path: {benchmark.SlotIdentifier.ClientIdentifier.ToConnectionString()}";
            if (benchmark.BenchmarkIdentifier.HasProcessor)
            {
                var slotType = ConvertToSlotType.FromCoreName(protein.Core);
                yield return $" Proc: {benchmark.BenchmarkIdentifier.ToProcessorAndThreadsString(slotType)}";
            }
            yield return $" Number of Frames Observed: {benchmark.FrameTimes.Count}";

            yield return String.Empty;

            yield return String.Format(CultureInfo.InvariantCulture, " Min. Time / Frame : {0} - {1} PPD",
                benchmark.MinimumFrameTime, GetPPD(protein, benchmark.MinimumFrameTime, calculateBonus).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " Avg. Time / Frame : {0} - {1} PPD",
                benchmark.AverageFrameTime, GetPPD(protein, benchmark.AverageFrameTime, calculateBonus).ToString(numberFormat));
        }

        private static IEnumerable<string> EnumerateClientInformation(IClientData clientData, string numberFormat, BonusCalculation bonusCalculation)
        {
            if (clientData is null)
            {
                yield break;
            }

            var provider = clientData.ProductionProvider;
            var status = clientData.Status;

            yield return String.Format(CultureInfo.InvariantCulture, " Cur. Time / Frame : {0} - {1} PPD",
                provider.GetFrameTime(PPDCalculation.LastFrame), provider.GetPPD(status, PPDCalculation.LastFrame, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " R3F. Time / Frame : {0} - {1} PPD",
                provider.GetFrameTime(PPDCalculation.LastThreeFrames), provider.GetPPD(status, PPDCalculation.LastThreeFrames, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " All  Time / Frame : {0} - {1} PPD",
                provider.GetFrameTime(PPDCalculation.AllFrames), provider.GetPPD(status, PPDCalculation.AllFrames, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " Eff. Time / Frame : {0} - {1} PPD",
                provider.GetFrameTime(PPDCalculation.EffectiveRate), provider.GetPPD(status, PPDCalculation.EffectiveRate, bonusCalculation).ToString(numberFormat));
        }

        private IClientData FindRunningClient(ProteinBenchmark benchmark)
        {
            if (ClientConfiguration is null) return null;

            var slot = ClientConfiguration.GetClientDataCollection().FirstOrDefault(x =>
                x.SlotIdentifier.Equals(benchmark.SlotIdentifier) &&
                x.BenchmarkIdentifier.Equals(benchmark.BenchmarkIdentifier));

            return slot != null && slot.Status.IsRunning() ? slot : null;
        }
    }
}
