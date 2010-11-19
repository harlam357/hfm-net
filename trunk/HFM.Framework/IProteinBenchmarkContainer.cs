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
using System.Collections.Generic;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IProteinBenchmarkContainer
   {
      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="currentUnitInfo">Current UnitInfo</param>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="nextUnitIndex">Index of Current UnitInfo</param>
      void UpdateBenchmarkData(IUnitInfoLogic currentUnitInfo, IUnitInfoLogic[] parsedUnits, int nextUnitIndex);
   
      /// <summary>
      /// Get the Benchmark Average frame time based on the Owner and Project info from the given UnitInfo
      /// </summary>
      /// <param name="unit">The UnitInfo containing the Owner and Project data</param>
      TimeSpan GetBenchmarkAverageFrameTime(IUnitInfoLogic unit);

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      void Delete(BenchmarkClient client);

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectID">Project Number</param>
      void Delete(BenchmarkClient client, int projectID);

      /// <summary>
      /// Determine if the given BenchmarkClient exists in the Benchmarks data
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      bool ContainsClient(BenchmarkClient client);

      /// <summary>
      /// Refresh the Minimum Frame Time for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectID">Project Number</param>
      void RefreshMinimumFrameTime(BenchmarkClient client, int projectID);

      /// <summary>
      /// Get List of BenchmarkClients
      /// </summary>
      BenchmarkClient[] GetBenchmarkClients();

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      int[] GetBenchmarkProjects();

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      int[] GetBenchmarkProjects(BenchmarkClient client);
      
      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      List<IInstanceProteinBenchmark> GetBenchmarks(BenchmarkClient client);
      
      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectID">Project Number</param>
      List<IInstanceProteinBenchmark> GetBenchmarks(BenchmarkClient client, int projectID);

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Name
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="newName">New Benchmark Owner Name</param>
      void UpdateInstanceName(BenchmarkClient client, string newName);

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Path
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="newPath">New Benchmark Owner Path</param>
      void UpdateInstancePath(BenchmarkClient client, string newPath);
      
      /// <summary>
      /// Read Binary File
      /// </summary>
      void Read();
      
      ///// <summary>
      ///// Read Binary File
      ///// </summary>
      //void Read(bool merge);
      
      /// <summary>
      /// Write Binary File
      /// </summary>
      void Write();

      /// <summary>
      /// Read Xml File
      /// </summary>
      void ReadXml(string filePath);
      
      ///// <summary>
      ///// Read Xml File
      ///// </summary>
      //void ReadXml(string filePath, bool merge);
      
      /// <summary>
      /// Write Xml File
      /// </summary>
      void WriteXml(string filePath);
   }
}
