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

            var benchmarks = SortBenchmarks(BenchmarkService.GetBenchmarks(slotIdentifier.Value, protein.ProjectNumber));

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
                double yMaximum = 0.0;
                var graphColors = source.Colors;
                foreach (ProteinBenchmark benchmark in benchmarks)
                {
                    var yPoints = GetYPoints(protein, benchmark);
                    foreach (var y in yPoints)
                    {
                        yMaximum = Math.Max(yMaximum, y);
                    }

                    string processorAndThreads = GetProcessorAndThreads(benchmark, protein);
                    CreateBar(i, pane, benchmark.SlotIdentifier.Name + processorAndThreads, yPoints, graphColors);
                    i++;
                }

                // Set the Titles
                pane.Title.Text = "HFM.NET - Slot Benchmarks";
                pane.XAxis.Title.Text = String.Join("   ", EnumerateProjectInformation(protein));
                pane.YAxis.Title.Text = Key;

                // Draw the X tics between the labels instead of at the labels
                pane.XAxis.MajorTic.IsBetweenLabels = true;
                // Set the XAxis labels
                pane.XAxis.Scale.TextLabels = new[] { "Min. Frame Time", "Avg. Frame Time" };
                // Set the XAxis to Text type
                pane.XAxis.Type = AxisType.Text;

                ConfigureYAxis(pane.YAxis, yMaximum);

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

        protected virtual IEnumerable<ProteinBenchmark> SortBenchmarks(IEnumerable<ProteinBenchmark> benchmarks)
        {
            return benchmarks;
        }

        protected abstract double[] GetYPoints(Protein protein, ProteinBenchmark benchmark);

        protected virtual void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {

        }

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

        private static void CreateBar(int index, GraphPane pane, string name, double[] yPoints, IReadOnlyList<Color> graphColors)
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

        public FrameTimeZedGraphBenchmarksReport(IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, proteinService, benchmarkService)
        {

        }

        protected override IEnumerable<ProteinBenchmark> SortBenchmarks(IEnumerable<ProteinBenchmark> benchmarks)
        {
            return benchmarks.OrderBy(x => x.MinimumFrameTime + x.AverageFrameTime);
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

    public class ProductionZedGraphBenchmarksReport : ZedGraphBenchmarksReport
    {
        public const string KeyName = "PPD";

        public IPreferenceSet Preferences { get; }

        private int DecimalPlaces { get; }
        private bool CalculateBonus { get; }

        public ProductionZedGraphBenchmarksReport(IPreferenceSet preferences, IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, proteinService, benchmarkService)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
            DecimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
            CalculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation) != BonusCalculation.None;
        }

        protected override IEnumerable<ProteinBenchmark> SortBenchmarks(IEnumerable<ProteinBenchmark> benchmarks)
        {
            return benchmarks.OrderBy(x => x.MinimumFrameTime + x.AverageFrameTime);
        }

        protected override double[] GetYPoints(Protein protein, ProteinBenchmark benchmark)
        {
            double minimumFrameTimePPD = GetPPD(benchmark.MinimumFrameTime, protein, CalculateBonus);
            double averageFrameTimePPD = GetPPD(benchmark.AverageFrameTime, protein, CalculateBonus);

            return new[]
            {
                Math.Round(minimumFrameTimePPD, DecimalPlaces),
                Math.Round(averageFrameTimePPD, DecimalPlaces)
            };
        }

        private static double GetPPD(TimeSpan frameTime, Protein protein, bool calculateBonus)
        {
            if (calculateBonus)
            {
                var unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
                return protein.GetBonusPPD(frameTime, unitTime);
            }
            return protein.GetPPD(frameTime);
        }

        protected override void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {
            // Don't show YAxis.Scale as 10^3         
            yAxis.Scale.MagAuto = false;
            // Set the YAxis Steps
            SetYAxisScale(yAxis, yMaximum);
        }

        private static void SetYAxisScale(YAxis yAxis, double maxValue)
        {
            double roundTo = GetRoundTo(maxValue);
            double majorStep = RoundUpToNext(maxValue, roundTo) / 10.0;

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
    }
}
