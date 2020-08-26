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
            var colors = source.Colors;

            if (slotIdentifier is null || projects.Count == 0)
            {
                Result = null;
                return;
            }

            var benchmarks = BenchmarkService.GetBenchmarks(slotIdentifier.Value, projects)
                .OrderBy(x => x.SlotIdentifier.Name)
                .ThenBy(x => x.Threads)
                .ToList();

            if (benchmarks.Count == 0)
            {
                Result = null;
                return;
            }

            var projectToXAxisOrdinal = BuildProjectToXAxisOrdinal(benchmarks);

            var zg = CreateZedGraphControl();
            try
            {
                GraphPane pane = zg.GraphPane;

                bool calculateBonus = Preferences.Get<BonusCalculation>(Preference.BonusCalculation) != BonusCalculation.None;

                int i = 0;
                var ppd = new List<double>();
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
                        Color color = GetNextColor(i++, colors);
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

                ConfigureXAxis(pane.XAxis, projectToXAxisOrdinal);
                ConfigureYAxis(pane.YAxis, ppd.Max());

                FillGraphPane(pane);
            }
            finally
            {
                zg.AxisChange();
            }

            Result = zg;
        }

        private static Dictionary<int, double> BuildProjectToXAxisOrdinal(IEnumerable<ProteinBenchmark> benchmarks)
        {
            double ordinal = 1.0;
            var projectToXAxisOrdinal = new Dictionary<int, double>();
            foreach (int projectID in benchmarks.Select(x => x.ProjectID).OrderBy(x => x).Distinct())
            {
                projectToXAxisOrdinal.Add(projectID, ordinal++);
            }
            return projectToXAxisOrdinal;
        }

        private static void ConfigureXAxis(XAxis xAxis, Dictionary<int, double> projectToXAxisOrdinal)
        {
            xAxis.Title.Text = "Project Number";
            xAxis.Scale.TextLabels = projectToXAxisOrdinal.Keys.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
            xAxis.Type = AxisType.Text;
        }

        private static void ConfigureYAxis(YAxis yAxis, double yMaximum)
        {
            yAxis.Title.Text = "PPD";

            // Don't show YAxis.Scale as 10^3         
            yAxis.Scale.MagAuto = false;
            SetYAxisScale(yAxis, yMaximum);
        }
    }
}
