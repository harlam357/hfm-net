using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;
using HFM.Proteins;

using ZedGraph;

namespace HFM.Forms.Models
{
    public interface IBenchmarksReportSource
    {
        SlotIdentifier? SlotIdentifier { get; }

        int? ProjectID { get; }
    }

    public abstract class BenchmarksReport
    {
        public string Key { get; }

        public object Result { get; protected set; }

        protected BenchmarksReport(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public abstract void Generate(IBenchmarksReportSource source);

        protected static IEnumerable<string> EnumerateProjectInformation(Protein protein)
        {
            yield return String.Format(CultureInfo.InvariantCulture, " Project ID: {0}", protein.ProjectNumber);
            yield return String.Format(CultureInfo.InvariantCulture, " Core: {0}", protein.Core);
            yield return String.Format(CultureInfo.InvariantCulture, " Credit: {0}", protein.Credit);
            yield return String.Format(CultureInfo.InvariantCulture, " Frames: {0}", protein.Frames);
        }
    }

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

        public int DecimalPlaces => Preferences.Get<int>(Preference.DecimalPlaces);

        public BonusCalculation BonusCalculation => Preferences.Get<BonusCalculation>(Preference.BonusCalculation);

        public override void Generate(IBenchmarksReportSource source)
        {
            var benchmarkText = new List<string>();

            var slotIdentifier = source.SlotIdentifier;
            var projectID = source.ProjectID;
            if (slotIdentifier is null || projectID is null)
            {
                Result = benchmarkText;
                return;
            }

            var protein = ProteinService.Get(projectID.Value);
            if (protein is null)
            {
                benchmarkText.Add(String.Format(CultureInfo.InvariantCulture, " Project ID: {0} Not Found", projectID));
                Result = benchmarkText;
                return;
            }

            var benchmarks = BenchmarkService.GetBenchmarks(slotIdentifier.Value, protein.ProjectNumber)
                .OrderBy(x => x.SlotIdentifier.Name)
                .ThenBy(x => x.Threads)
                .ToList();

            string numberFormat = NumberFormat.Get(DecimalPlaces);
            var bonusCalculation = BonusCalculation;
            bool calculateBonus = bonusCalculation != BonusCalculation.None;

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

        private static double GetPPD(Protein protein, TimeSpan frameTime, bool calculateBonus)
        {
            if (calculateBonus)
            {
                var unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
                return protein.GetBonusPPD(frameTime, unitTime);
            }
            return protein.GetPPD(frameTime);
        }

        private SlotModel FindRunningSlot(ProteinBenchmark benchmark)
        {
            var slot = ClientConfiguration?.Slots.FirstOrDefault(x =>
                x.SlotIdentifier.Equals(benchmark.SlotIdentifier) &&
                x.WorkUnitModel.BenchmarkIdentifier.Equals(benchmark.BenchmarkIdentifier));

            return slot != null && slot.Status.IsRunning() ? slot : null;
        }
    }

    public abstract class ZedGraphBenchmarksReport : BenchmarksReport
    {
        public IPreferenceSet Preferences { get; }
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkService BenchmarkService { get; }

        protected ZedGraphBenchmarksReport(string key, IPreferenceSet preferences,
            IProteinService proteinService, IProteinBenchmarkService benchmarkService) : base(key)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
            ProteinService = proteinService ?? NullProteinService.Instance;
            BenchmarkService = benchmarkService ?? NullProteinBenchmarkService.Instance;
        }

        public sealed override void Generate(IBenchmarksReportSource source)
        {
            var slotIdentifier = source.SlotIdentifier;
            var projectID = source.ProjectID;
            if (slotIdentifier is null || projectID is null)
            {
                Result = null;
                return;
            }

            var protein = ProteinService.Get(projectID.Value);
            if (protein is null)
            {
                Result = null;
                return;
            }

            var benchmarks = BenchmarkService.GetBenchmarks(slotIdentifier.Value, protein.ProjectNumber)
                .OrderBy(x => x.SlotIdentifier.Name)
                .ThenBy(x => x.Threads)
                .ToList();

            var zg = new ZedGraphControl();
            try
            {
                // get a reference to the GraphPane
                GraphPane pane = zg.GraphPane;

                // Clear the bars
                pane.CurveList.Clear();
                // Clear the bar labels
                pane.GraphObjList.Clear();
                // Clear the XAxis Project Information
                pane.XAxis.Title.Text = String.Empty;

                // Create the bars for each benchmark
                int i = 0;
                var graphColors = Preferences.Get<List<Color>>(Preference.GraphColors);
                foreach (ProteinBenchmark benchmark in benchmarks)
                {
                    var yPoints = GetYPoints(benchmark);

                    string processorAndThreads = GetProcessorAndThreads(benchmark, protein);
                    CreateBar(i, pane, benchmark.SlotIdentifier.Name + processorAndThreads, yPoints, graphColors);
                    i++;
                }

                // Set the Titles
                pane.Title.Text = "HFM.NET - Slot Benchmarks";
                pane.XAxis.Title.Text = String.Join("   ", EnumerateProjectInformation(protein));
                pane.YAxis.Title.Text = Key; // "Frame Time (Seconds)";

                // Draw the X tics between the labels instead of at the labels
                pane.XAxis.MajorTic.IsBetweenLabels = true;
                // Set the XAxis labels
                pane.XAxis.Scale.TextLabels = new[] { "Min. Frame Time", "Avg. Frame Time" };
                // Set the XAxis to Text type
                pane.XAxis.Type = AxisType.Text;

                // Fill the Axis and Pane backgrounds
                pane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
                pane.Fill = new Fill(Color.FromArgb(250, 250, 255));
            }
            finally
            {
                // Tell ZedGraph to reconfigure the
                // axes since the data has changed
                zg.AxisChange();
                // Refresh the control
                zg.Refresh();
            }

            Result = zg;
        }

        protected abstract double[] GetYPoints(ProteinBenchmark benchmark);

        private static string GetProcessorAndThreads(ProteinBenchmark benchmark, Protein protein)
        {
            if (protein == null) return String.Empty;

            var slotType = SlotTypeConvert.FromCoreName(protein.Core);
            var processorAndThreads = benchmark.BenchmarkIdentifier.ToProcessorAndThreadsString(slotType);
            if (!String.IsNullOrWhiteSpace(processorAndThreads))
            {
                processorAndThreads = " / " + processorAndThreads;
            }
            return processorAndThreads;
        }

        private static void CreateBar(int index, GraphPane pane, string name, double[] yPoints, IList<Color> graphColors)
        {
            int colorIndex = index % graphColors.Count;
            Color barColor = graphColors[colorIndex];

            // Generate a bar with the name in the legend
            BarItem myBar = pane.AddBar(name, null, yPoints, barColor);
            myBar.Bar.Fill = new Fill(barColor, Color.White, barColor);
        }
    }

    public class FrameTimeZedGraphBenchmarksReport : ZedGraphBenchmarksReport
    {
        public const string KeyName = "Frame Time (Seconds)";

        public FrameTimeZedGraphBenchmarksReport(IPreferenceSet preferences, IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, preferences, proteinService, benchmarkService)
        {

        }

        protected override double[] GetYPoints(ProteinBenchmark benchmark)
        {
            return new[]
            {
                benchmark.MinimumFrameTime.TotalSeconds,
                benchmark.AverageFrameTime.TotalSeconds
            };
        }
    }
}
