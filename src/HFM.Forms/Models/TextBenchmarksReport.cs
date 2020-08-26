using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms.Models
{
    public class TextBenchmarksReport : BenchmarksReport
    {
        public const string KeyName = "Text";

        public IPreferenceSet Preferences { get; }
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkService BenchmarkService { get; }
        public ClientConfiguration ClientConfiguration { get; }

        public TextBenchmarksReport(IPreferenceSet preferences, IProteinService proteinService,
            IProteinBenchmarkService benchmarkService, ClientConfiguration clientConfiguration) : base(KeyName)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
            ProteinService = proteinService ?? NullProteinService.Instance;
            BenchmarkService = benchmarkService ?? NullProteinBenchmarkService.Instance;
            ClientConfiguration = clientConfiguration;
        }

        public override void Generate(IBenchmarksReportSource source)
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

            foreach (var projectID in projects)
            {
                var protein = ProteinService.Get(projectID);
                if (protein is null)
                {
                    benchmarkText.Add(String.Format(CultureInfo.InvariantCulture, " Project ID: {0} Not Found", projectID));
                    benchmarkText.AddRange(Enumerable.Repeat(String.Empty, 2));
                    continue;
                }

                var benchmarks = BenchmarkService.GetBenchmarks(slotIdentifier.Value, protein.ProjectNumber)
                    .OrderBy(x => x.SlotIdentifier.Name)
                    .ThenBy(x => x.Threads);

                benchmarkText
                    .AddRange(EnumerateProjectInformation(protein)
                        .Concat(Enumerable.Repeat(String.Empty, 2)));

                foreach (var b in benchmarks)
                {
                    benchmarkText
                        .AddRange(EnumerateBenchmarkInformation(protein, b, numberFormat, calculateBonus)
                            .Concat(EnumerateSlotInformation(FindRunningSlot(b), numberFormat, bonusCalculation))
                            .Concat(Enumerable.Repeat(String.Empty, 2)));
                }
            }

            Result = benchmarkText;
        }

        private static IEnumerable<string> EnumerateBenchmarkInformation(Protein protein, ProteinBenchmark benchmark, string numberFormat, bool calculateBonus)
        {
            yield return $" Name: {benchmark.SlotIdentifier.Name}";
            yield return $" Path: {benchmark.SlotIdentifier.ClientIdentifier.ToServerPortString()}";
            if (benchmark.BenchmarkIdentifier.HasProcessor)
            {
                var slotType = SlotTypeConvert.FromCoreName(protein.Core);
                yield return $" Proc: {benchmark.BenchmarkIdentifier.ToProcessorAndThreadsString(slotType)}";
            }
            yield return $" Number of Frames Observed: {benchmark.FrameTimes.Count}";

            yield return String.Empty;

            yield return String.Format(CultureInfo.InvariantCulture, " Min. Time / Frame : {0} - {1} PPD",
                benchmark.MinimumFrameTime, GetPPD(protein, benchmark.MinimumFrameTime, calculateBonus).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " Avg. Time / Frame : {0} - {1} PPD",
                benchmark.AverageFrameTime, GetPPD(protein, benchmark.AverageFrameTime, calculateBonus).ToString(numberFormat));
        }

        private static IEnumerable<string> EnumerateSlotInformation(SlotModel slot, string numberFormat, BonusCalculation bonusCalculation)
        {
            if (slot is null)
            {
                yield break;
            }

            var workUnit = slot.WorkUnitModel;
            var status = slot.Status;

            yield return String.Format(CultureInfo.InvariantCulture, " Cur. Time / Frame : {0} - {1} PPD",
                workUnit.GetFrameTime(PPDCalculation.LastFrame), workUnit.GetPPD(status, PPDCalculation.LastFrame, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " R3F. Time / Frame : {0} - {1} PPD",
                workUnit.GetFrameTime(PPDCalculation.LastThreeFrames), workUnit.GetPPD(status, PPDCalculation.LastThreeFrames, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " All  Time / Frame : {0} - {1} PPD",
                workUnit.GetFrameTime(PPDCalculation.AllFrames), workUnit.GetPPD(status, PPDCalculation.AllFrames, bonusCalculation).ToString(numberFormat));
            yield return String.Format(CultureInfo.InvariantCulture, " Eff. Time / Frame : {0} - {1} PPD",
                workUnit.GetFrameTime(PPDCalculation.EffectiveRate), workUnit.GetPPD(status, PPDCalculation.EffectiveRate, bonusCalculation).ToString(numberFormat));
        }

        private SlotModel FindRunningSlot(ProteinBenchmark benchmark)
        {
            var slot = ClientConfiguration?.Slots.FirstOrDefault(x =>
                x.SlotIdentifier.Equals(benchmark.SlotIdentifier) &&
                x.WorkUnitModel.BenchmarkIdentifier.Equals(benchmark.BenchmarkIdentifier));

            return slot != null && slot.Status.IsRunning() ? slot : null;
        }
    }
}
