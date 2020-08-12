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
