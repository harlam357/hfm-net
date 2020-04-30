
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

using ZedGraph;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Proteins;

namespace HFM.Forms
{
    public class ZedGraphManager
    {
        /// <summary>
        /// Build The PPD GraphPane
        /// </summary>
        /// <param name="zg">ZedGraph Control</param>
        /// <param name="projectInfoLines">Project Info Array</param>
        /// <param name="benchmarks">Benchmarks Collection to Plot</param>
        /// <param name="graphColors">Graph Colors List</param>
        /// <param name="decimalPlaces">PPD Decimal Places</param>
        /// <param name="protein"></param>
        /// <param name="calculateBonus"></param>
        [CLSCompliant(false)]
        public void CreatePpdGraph(ZedGraphControl zg, IList<string> projectInfoLines,
                                   IEnumerable<ProteinBenchmark> benchmarks,
                                   IList<Color> graphColors, int decimalPlaces,
                                   Protein protein, bool calculateBonus)
        {
            Debug.Assert(zg != null);

            try
            {
                // get a reference to the GraphPane
                GraphPane myPane = zg.GraphPane;

                // Clear the bars
                myPane.CurveList.Clear();
                // Clear the bar labels
                myPane.GraphObjList.Clear();
                // Clear the XAxis Project Information
                myPane.XAxis.Title.Text = String.Empty;

                // If no Project Information, get out
                if (projectInfoLines.Count == 0)
                {
                    return;
                }

                // Create the bars for each benchmark
                int i = 0;
                double maxPPD = 0.0;
                foreach (ProteinBenchmark benchmark in benchmarks)
                {
                    double minimumFrameTimePPD = 0;
                    double averageFrameTimePPD = 0;

                    if (protein != null)
                    {
                        minimumFrameTimePPD = GetPPD(benchmark.MinimumFrameTime, protein, calculateBonus);
                        averageFrameTimePPD = GetPPD(benchmark.AverageFrameTime, protein, calculateBonus);
                    }

                    maxPPD = Math.Max(maxPPD, minimumFrameTimePPD);
                    maxPPD = Math.Max(maxPPD, averageFrameTimePPD);

                    var yPoints = new double[2];
                    yPoints[0] = Math.Round(minimumFrameTimePPD, decimalPlaces);
                    yPoints[1] = Math.Round(averageFrameTimePPD, decimalPlaces);

                    string processorAndThreads = GetProcessorAndThreads(benchmark, protein);
                    CreateBar(i, myPane, benchmark.SlotIdentifier.Name + processorAndThreads, yPoints, graphColors);
                    i++;
                }

                // Create the bar labels
                string numberFormat = NumberFormat.Get(decimalPlaces);
                BarItem.CreateBarLabels(myPane, true, numberFormat, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

                // Set the Titles
                myPane.Title.Text = "HFM.NET - Client Benchmarks";
                var sb = new StringBuilder();
                for (i = 0; i < projectInfoLines.Count - 2; i++)
                {
                    sb.Append(projectInfoLines[i]);
                    sb.Append("   ");
                }
                sb.Append(projectInfoLines[i]);
                myPane.XAxis.Title.Text = sb.ToString();
                myPane.YAxis.Title.Text = "PPD";

                // Draw the X tics between the labels instead of at the labels
                myPane.XAxis.MajorTic.IsBetweenLabels = true;
                // Set the XAxis labels
                myPane.XAxis.Scale.TextLabels = new[] { "Min. Frame Time", "Avg. Frame Time" };
                // Set the XAxis to Text type
                myPane.XAxis.Type = AxisType.Text;

                // Don't show YAxis.Scale as 10^3         
                myPane.YAxis.Scale.MagAuto = false;
                // Set the YAxis Steps
                SetYAxisScale(myPane, maxPPD);

                // Fill the Axis and Pane backgrounds
                myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
                myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

                SetTextObjFontAngleHorizontal(myPane);

            }
            finally
            {
                // Tell ZedGraph to refigure the
                // axes since the data have changed
                zg.AxisChange();
                // Refresh the control
                zg.Refresh();
            }
        }

        private static void SetYAxisScale(GraphPane myPane, double maxValue)
        {
            double roundTo = GetRoundTo(maxValue);
            double majorStep = RoundUpToNext(maxValue, roundTo) / 10.0;

            myPane.YAxis.Scale.MajorStep = majorStep;
            myPane.YAxis.Scale.MinorStep = majorStep / 2;
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

        private static double GetPPD(TimeSpan frameTime, Protein protein, bool calculateUnitTimeByFrameTime)
        {
            if (calculateUnitTimeByFrameTime)
            {
                var unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
                return protein.GetBonusPPD(frameTime, unitTime);
            }
            return protein.GetPPD(frameTime);
        }

        /// <summary>
        /// Build The Frame Time GraphPane
        /// </summary>
        /// <param name="zg">ZedGraph Control</param>
        /// <param name="projectInfoLines">Project Info Array</param>
        /// <param name="benchmarks">Benchmarks Collection to Plot</param>
        /// <param name="graphColors">Graph Colors List</param>
        /// <param name="protein"></param>
        [CLSCompliant(false)]
        public void CreateFrameTimeGraph(ZedGraphControl zg, IList<string> projectInfoLines,
                                         IEnumerable<ProteinBenchmark> benchmarks,
                                         IList<Color> graphColors, Protein protein)
        {
            Debug.Assert(zg != null);

            try
            {
                // get a reference to the GraphPane
                GraphPane myPane = zg.GraphPane;

                // Clear the bars
                myPane.CurveList.Clear();
                // Clear the bar labels
                myPane.GraphObjList.Clear();
                // Clear the XAxis Project Information
                myPane.XAxis.Title.Text = String.Empty;

                // If no Project Information, get out
                if (projectInfoLines.Count == 0)
                {
                    return;
                }

                // Create the bars for each benchmark
                int i = 0;
                foreach (ProteinBenchmark benchmark in benchmarks)
                {
                    var yPoints = new double[2];
                    yPoints[0] = benchmark.MinimumFrameTime.TotalSeconds;
                    yPoints[1] = benchmark.AverageFrameTime.TotalSeconds;

                    string processorAndThreads = GetProcessorAndThreads(benchmark, protein);
                    CreateBar(i, myPane, benchmark.SlotIdentifier.Name + processorAndThreads, yPoints, graphColors);
                    i++;
                }

                // Create the bar labels
                BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

                // Set the Titles
                myPane.Title.Text = "HFM.NET - Client Benchmarks";
                var sb = new StringBuilder();
                for (i = 0; i < projectInfoLines.Count - 2; i++)
                {
                    sb.Append(projectInfoLines[i]);
                    sb.Append("   ");
                }
                sb.Append(projectInfoLines[i]);
                myPane.XAxis.Title.Text = sb.ToString();
                myPane.YAxis.Title.Text = "Frame Time (Seconds)";

                // Draw the X tics between the labels instead of at the labels
                myPane.XAxis.MajorTic.IsBetweenLabels = true;
                // Set the XAxis labels
                myPane.XAxis.Scale.TextLabels = new[] { "Min. Frame Time", "Avg. Frame Time" };
                // Set the XAxis to Text type
                myPane.XAxis.Type = AxisType.Text;

                // Fill the Axis and Pane backgrounds
                myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
                myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

                SetTextObjFontAngleHorizontal(myPane);
            }
            finally
            {
                // Tell ZedGraph to refigure the
                // axes since the data have changed
                zg.AxisChange();
                // Refresh the control
                zg.Refresh();
            }
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

        private static void SetTextObjFontAngleHorizontal(GraphPane myPane)
        {
            foreach (var textObj in myPane.GraphObjList.OfType<TextObj>())
            {
                textObj.FontSpec.Angle = 0f;
            }
        }

        private static void CreateBar(int index, GraphPane myPane, string instanceName, double[] y, IList<Color> graphColors)
        {
            int colorIndex = index % graphColors.Count;
            Color barColor = graphColors[colorIndex];

            // Generate a bar with the Instance Name in the legend
            BarItem myBar = myPane.AddBar(instanceName, null, y, barColor);
            myBar.Bar.Fill = new Fill(barColor, Color.White, barColor);
        }
    }
}
