using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

using HFM.Core.WorkUnits;
using HFM.Preferences;

using ZedGraph;

namespace HFM.Forms.Models
{
    public class ProjectComparisonZedGraphBenchmarksReport : ZedGraphBenchmarksReport
    {
        public const string KeyName = "Project Comparison";

        public IPreferenceSet Preferences { get; }

        public ProjectComparisonZedGraphBenchmarksReport(IPreferenceSet preferences, IProteinService proteinService, IProteinBenchmarkService benchmarkService)
            : base(KeyName, proteinService, benchmarkService)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
        }

        public override void Generate(IBenchmarksReportSource source)
        {
            var slotIdentifier = source.SlotIdentifier;
            var projects = source.Projects;
            if (slotIdentifier is null || projects.Count == 0)
            {
                Result = null;
                return;
            }

            var benchmarks = BenchmarkService.GetBenchmarks(slotIdentifier.Value, projects)
                .OrderBy(x => x.SlotIdentifier.Name)
                .ThenBy(x => x.Threads)
                .ToList();

            double ordinal = 1.0;
            var projectToXAxisOrdinal = new Dictionary<int, double>();
            foreach (int projectID in benchmarks.Select(x => x.ProjectID).OrderBy(x => x).Distinct())
            {
                projectToXAxisOrdinal.Add(projectID, ordinal++);
            }

            var zg = new ZedGraphControl();
            try
            {
                GraphPane pane = zg.GraphPane;

                bool calculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation) != BonusCalculation.None;

                int i = 0;
                var ppd = new List<double>();
                var graphColors = source.Colors;
                foreach (var group in benchmarks.GroupBy(x => (x.SlotIdentifier, x.Processor, x.Threads)))
                {
                    string label = null;
                    var points = new PointPairList();
                    foreach (var benchmark in group.OrderBy(x => x.ProjectID))
                    {
                        var protein = ProteinService.Get(benchmark.ProjectID);
                        if (protein is null)
                        {
                            continue;
                        }

                        if (label is null)
                        {
                            label = GetSlotNameAndProcessor(benchmark, protein);
                        }

                        double y = GetPPD(protein, benchmark.AverageFrameTime, calculateBonus);
                        ppd.Add(y);
                        points.Add(projectToXAxisOrdinal[benchmark.ProjectID], y);
                    }

                    if (points.Count > 0)
                    {
                        int colorIndex = i++ % graphColors.Count;
                        Color color = graphColors[colorIndex];
                        var lineItem = pane.AddCurve(label, points, color, SymbolType.Circle);
                        lineItem.Symbol.Fill = new Fill(color);
                        lineItem.IsOverrideOrdinal = true;
                    }
                }

                var averagePPD = ppd.Average();
                var averagePoints = new PointPairList();
                foreach (var x in projectToXAxisOrdinal.Values)
                {
                    averagePoints.Add(x, averagePPD);
                }
                var averageLineItem = pane.AddCurve("Average PPD", averagePoints, Color.Black, SymbolType.Circle);
                averageLineItem.Symbol.Fill = new Fill(Color.Black);
                averageLineItem.IsOverrideOrdinal = true;

                // Set the Titles
                pane.Title.Text = "HFM.NET - Slot Benchmarks";
                pane.XAxis.Title.Text = "Project Number";
                pane.YAxis.Title.Text = "PPD";

                // Set the XAxis labels
                pane.XAxis.Scale.TextLabels = projectToXAxisOrdinal.Keys.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
                // Set the XAxis to Text type
                pane.XAxis.Type = AxisType.Text;

                ConfigureYAxis(pane.YAxis, ppd.Max());

                // Fill Pane background
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

        private static void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {
            // Don't show YAxis.Scale as 10^3         
            yAxis.Scale.MagAuto = false;
            // Set the YAxis Steps
            SetYAxisScale(yAxis, yMaximum);
        }
    }
}
