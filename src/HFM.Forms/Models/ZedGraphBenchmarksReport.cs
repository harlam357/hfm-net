using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Preferences;
using HFM.Proteins;

using ZedGraph;

namespace HFM.Forms.Models
{
    public abstract class ZedGraphBenchmarksReport : BenchmarksReport
    {
        public IProteinService ProteinService { get; }
        public IProteinBenchmarkService BenchmarkService { get; }

        protected ZedGraphBenchmarksReport(string key, IProteinService proteinService, IProteinBenchmarkService benchmarkService) : base(key)
        {
            ProteinService = proteinService ?? NullProteinService.Instance;
            BenchmarkService = benchmarkService ?? NullProteinBenchmarkService.Instance;
        }

        protected static ZedGraphControl CreateZedGraphControl()
        {
            var zg = new ZedGraphControl();
            zg.GraphPane.Title.Text = "HFM.NET - Slot Benchmarks";
            return zg;
        }

        protected static Color GetNextColor(int index, IReadOnlyList<Color> colors)
        {
            int colorIndex = index % colors.Count;
            return colors[colorIndex];
        }

        protected static string GetSlotNameAndProcessor(ProteinBenchmark benchmark, Protein protein)
        {
            if (protein != null)
            {
                var slotType = ConvertToSlotType.FromCoreName(protein.Core);
                var processorAndThreads = benchmark.BenchmarkIdentifier.ToProcessorAndThreadsString(slotType);
                if (!String.IsNullOrWhiteSpace(processorAndThreads))
                {
                    return String.Join(" / ", benchmark.SlotIdentifier.Name, processorAndThreads);
                }
            }
            return benchmark.SlotIdentifier.Name;
        }

        /// <summary>
        /// Setup yAxis scale steps based on maximum y value.
        /// </summary>
        protected static void SetYAxisScale(YAxis yAxis, double yMaximum)
        {
            double roundTo = GetRoundTo(yMaximum);
            double majorStep = RoundUpToNext(yMaximum, roundTo) / 10.0;

            yAxis.Scale.MajorStep = majorStep;
            yAxis.Scale.MinorStep = majorStep / 2;
        }

        // for a value like    26,273.1, this function will return 1,000.0
        // for a value like 1,119,789.5, this function will return 100,000.0
        private static double GetRoundTo(double value)
        {
            double roundTo = 10.0;
            while (value / roundTo > 100.0)
            {
                roundTo *= 10.0;
            }
            return roundTo;
        }

        private static double RoundUpToNext(double value, double roundTo)
        {
            // drop all digits less significant than roundTo
            value = (int)(value / roundTo) * roundTo;
            // apply the roundTo increment
            return value + roundTo;
        }

        protected static void FillGraphPane(GraphPane pane)
        {
            pane.Fill = new Fill(Color.FromArgb(250, 250, 255));
        }
    }

    public abstract class ZedGraphBarGraphBenchmarksReport : ZedGraphBenchmarksReport
    {
        protected ZedGraphBarGraphBenchmarksReport(string key, IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(key, proteinService, benchmarkService)
        {

        }

        public sealed override void Generate(IBenchmarksReportSource source)
        {
            var slotIdentifier = source.SlotIdentifier;
            var projectID = GetProjectID(source);
            var protein = GetProtein(projectID);
            var colors = source.Colors;

            if (slotIdentifier is null || projectID is null || protein is null)
            {
                Result = null;
                return;
            }

            var benchmarks = SortBenchmarks(BenchmarkService.GetBenchmarks(slotIdentifier.Value, protein.ProjectNumber));

            var zg = CreateZedGraphControl();
            try
            {
                GraphPane pane = zg.GraphPane;

                // Create the bars for each benchmark
                int i = 0;
                double yMaximum = 0.0;
                foreach (ProteinBenchmark benchmark in benchmarks)
                {
                    var yPoints = GetYPoints(protein, benchmark);
                    foreach (var y in yPoints)
                    {
                        yMaximum = Math.Max(yMaximum, y);
                    }

                    string label = GetSlotNameAndProcessor(benchmark, protein);
                    var color = GetNextColor(i++, colors);
                    AddBar(pane, label, yPoints, color);
                }

                ConfigureXAxis(pane.XAxis, protein);
                ConfigureYAxis(pane.YAxis, yMaximum);

                FillChart(pane.Chart);
                FillGraphPane(pane);
            }
            finally
            {
                zg.AxisChange();
            }

            Result = zg;
        }

        private static int? GetProjectID(IBenchmarksReportSource source)
        {
            return source.Projects.Count > 0 ? (int?)source.Projects.First() : null;
        }

        private Protein GetProtein(int? projectID)
        {
            if (!projectID.HasValue) return null;
            return ProteinService.Get(projectID.Value);
        }

        protected virtual IEnumerable<ProteinBenchmark> SortBenchmarks(IEnumerable<ProteinBenchmark> benchmarks)
        {
            return benchmarks.OrderBy(x => x.MinimumFrameTime + x.AverageFrameTime);
        }

        protected abstract double[] GetYPoints(Protein protein, ProteinBenchmark benchmark);

        protected virtual void ConfigureXAxis(XAxis xAxis, Protein protein)
        {
            xAxis.Title.Text = String.Join("   ", EnumerateProjectInformation(protein));

            xAxis.MajorTic.IsBetweenLabels = true;
            xAxis.Scale.TextLabels = new[] { "Min. Frame Time", "Avg. Frame Time" };
            xAxis.Type = AxisType.Text;
        }

        protected virtual void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {
            yAxis.Title.Text = Key;
        }

        private static void AddBar(GraphPane pane, string label, double[] yPoints, Color color)
        {
            var barItem = pane.AddBar(label, null, yPoints, color);
            barItem.Bar.Fill = new Fill(color, Color.White, color);
        }

        private static void FillChart(Chart chart)
        {
            chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
        }
    }

    public class FrameTimeZedGraphBenchmarksReport : ZedGraphBarGraphBenchmarksReport
    {
        public const string KeyName = "Frame Time (Seconds)";

        public FrameTimeZedGraphBenchmarksReport(IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, proteinService, benchmarkService)
        {

        }

        protected override double[] GetYPoints(Protein protein, ProteinBenchmark benchmark)
        {
            return new[]
            {
                benchmark.MinimumFrameTime.TotalSeconds,
                benchmark.AverageFrameTime.TotalSeconds
            };
        }
    }

    public class ProductionZedGraphBenchmarksReport : ZedGraphBarGraphBenchmarksReport
    {
        public const string KeyName = "PPD";

        public IPreferenceSet Preferences { get; }

        public ProductionZedGraphBenchmarksReport(IPreferenceSet preferences, IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, proteinService, benchmarkService)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
        }

        protected override double[] GetYPoints(Protein protein, ProteinBenchmark benchmark)
        {
            int decimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
            bool calculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation) != BonusCalculation.None;

            double minimumFrameTimePPD = GetPPD(protein, benchmark.MinimumFrameTime, calculateBonus);
            double averageFrameTimePPD = GetPPD(protein, benchmark.AverageFrameTime, calculateBonus);

            return new[]
            {
                Math.Round(minimumFrameTimePPD, decimalPlaces),
                Math.Round(averageFrameTimePPD, decimalPlaces)
            };
        }

        protected override void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {
            base.ConfigureYAxis(yAxis, yMaximum);

            // Don't show YAxis.Scale as 10^3         
            yAxis.Scale.MagAuto = false;
            SetYAxisScale(yAxis, yMaximum);
        }
    }
}
