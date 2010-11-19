/*
 * HFM.NET - Benchmark Data Interface
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IInstanceProteinBenchmark : IOwnedByClientInstance
   {
      /// <summary>
      /// Project ID
      /// </summary>
      Int32 ProjectID { get; }

      /// <summary>
      /// Minimum Frame Time
      /// </summary>
      TimeSpan MinimumFrameTime { get; }

      /// <summary>
      /// PPD based on Minimum Frame Time
      /// </summary>
      double MinimumFrameTimePPD { get; }

      /// <summary>
      /// Average Frame Time
      /// </summary>
      TimeSpan AverageFrameTime { get; }

      /// <summary>
      /// PPD based on Average Frame Time
      /// </summary>
      double AverageFrameTimePPD { get; }

      /// <summary>
      /// Benchmark Client Descriptor
      /// </summary>
      BenchmarkClient Client { get; }

      /// <summary>
      /// Benchmark Protein
      /// </summary>
      IProtein Protein { get; }

      /// <summary>
      /// Set Next Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      bool SetFrameTime(TimeSpan frameTime);

      /// <summary>
      /// Refresh the Minimum Frame Time for this Benchmark based on current List of Frame Times
      /// </summary>
      void RefreshBenchmarkMinimumFrameTime();

      /// <summary>
      /// Return Multi-Line String (Array)
      /// </summary>
      /// <param name="UnitInfo">Client Instance UnitInfo (null for unavailable)</param>
      /// <param name="PpdFormatString">PPD Format String</param>
      /// <param name="ProductionValuesOk">Client Instance Production Values Flag</param>
      string[] ToMultiLineString(IUnitInfoLogic UnitInfo, string PpdFormatString, bool ProductionValuesOk);
   }
}
