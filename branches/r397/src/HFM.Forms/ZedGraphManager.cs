/*
 * HFM.NET - ZedGraph Drawing Manager Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ZedGraph;

using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public class ZedGraphManager
   {
      /// <summary>
      /// Build The PPD GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="projectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="graphColors">Graph Colors List</param>
      /// <param name="decimalPlaces">PPD Decimal Places</param>
      /// <param name="protein"></param>
      /// <param name="calculateBonus"></param>
      [CLSCompliant(false)]
      public void CreatePpdGraph(ZedGraphControl zg, IList<string> projectInfo, 
                                 IEnumerable<ProteinBenchmark> benchmarks,
                                 IList<Color> graphColors, int decimalPlaces,
                                 IProtein protein, bool calculateBonus)
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
            if (projectInfo.Count == 0)
            {
               return;
            }

            // Scale YAxis In Thousands?
            bool inThousands = false;

            // Create the bars for each benchmark
            int i = 0;
            foreach (ProteinBenchmark benchmark in benchmarks)
            {
               double minimumFrameTimePPD = 0;
               double averageFrameTimePPD = 0;
               if (protein != null)
               {
                  minimumFrameTimePPD = protein.GetPPD(benchmark.MinimumFrameTime, calculateBonus);
                  averageFrameTimePPD = protein.GetPPD(benchmark.AverageFrameTime, calculateBonus);
               }

               if (minimumFrameTimePPD >= 1000 || averageFrameTimePPD >= 1000)
               {
                  inThousands = true;
               }

               var yPoints = new double[2];
               yPoints[0] = Math.Round(minimumFrameTimePPD, decimalPlaces);
               yPoints[1] = Math.Round(averageFrameTimePPD, decimalPlaces);

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, graphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            var sb = new StringBuilder();
            for (i = 0; i < projectInfo.Count - 2; i++)
            {
               sb.Append(projectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(projectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "PPD";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            var labels = new[] { "Min. Frame Time", "Avg. Frame Time" };
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Don't show YAxis.Scale as 10^3         
            myPane.YAxis.Scale.MagAuto = false;
            // Set the YAxis Steps
            if (inThousands)
            {
               myPane.YAxis.Scale.MajorStep = 1000;
               myPane.YAxis.Scale.MinorStep = 500;
            }
            else
            {
               myPane.YAxis.Scale.MajorStep = 100;
               myPane.YAxis.Scale.MinorStep = 10;
            }

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));
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

      /// <summary>
      /// Build The Frame Time GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="projectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="graphColors">Graph Colors List</param>
      [CLSCompliant(false)]
      public void CreateFrameTimeGraph(ZedGraphControl zg, IList<string> projectInfo, 
                                       IEnumerable<ProteinBenchmark> benchmarks,
                                       IList<Color> graphColors)
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
            if (projectInfo.Count == 0)
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

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, graphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            var sb = new StringBuilder();
            for (i = 0; i < projectInfo.Count - 2; i++)
            {
               sb.Append(projectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(projectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "Frame Time (Seconds)";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            var labels = new[] { "Min. Frame Time", "Avg. Frame Time" };
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));
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
