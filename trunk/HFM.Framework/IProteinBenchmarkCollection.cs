/*
 * HFM.NET - Benchmark Collection Interface
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

namespace HFM.Framework
{
   public interface IProteinBenchmarkCollection
   {
      /// <summary>
      /// Update Benchmark Data from given UnitInfo and frame indexes
      /// </summary>
      /// <param name="unit">The UnitInfo containing the UnitFrame data</param>
      /// <param name="startingFrame">Starting Frame Index</param>
      /// <param name="endingFrame">Ending Frame Index</param>
      void UpdateBenchmarkData(IUnitInfo unit, int startingFrame, int endingFrame);

      /// <summary>
      /// Get the Benchmark Average frame time based on the Owner and Project info from the given UnitInfo
      /// </summary>
      /// <param name="unit">The UnitInfo containing the Owner and Project data</param>
      TimeSpan GetBenchmarkAverageFrameTime(IUnitInfo unit);

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      void Delete(IBenchmarkClient Client);

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      void Delete(IBenchmarkClient Client, int ProjectID);

      /// <summary>
      /// Determine if the given BenchmarkClient exists in the Benchmarks data
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      bool ContainsClient(IBenchmarkClient Client);

      /// <summary>
      /// Refresh the Minimum Frame Time for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      void RefreshMinimumFrameTime(IBenchmarkClient Client, int ProjectID);

      /// <summary>
      /// Get List of BenchmarkClients
      /// </summary>
      IBenchmarkClient[] GetBenchmarkClients();

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      int[] GetBenchmarkProjects();

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      int[] GetBenchmarkProjects(IBenchmarkClient Client);

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Name
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewName">New Benchmark Owner Name</param>
      void UpdateInstanceName(IBenchmarkClient Client, string NewName);

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Path
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewPath">New Benchmark Owner Path</param>
      void UpdateInstancePath(IBenchmarkClient Client, string NewPath);
   }
}