/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.Client;
using HFM.Core.Data;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProteinBenchmarkServiceTests
    {
        private static readonly string _TestFile = "..\\..\\TestFiles\\TestBenchmarkCache.dat";

        [TearDown]
        public void ProteinBenchmarkServiceTests_TearDown()
        {
            File.Delete(_TestFile);
        }

        [Test]
        public void ProteinBenchmarkService_GetSlotIdentifiers_Test()
        {
            // Arrange
            var benchmarks = new ProteinBenchmarkService(CreateTestDataContainer());
            // Act
            var identifiers = benchmarks.GetSlotIdentifiers();
            // Assert
            Assert.AreEqual(12, identifiers.Count);
        }

        [Test]
        public void ProteinBenchmarkService_Update_CreatesNewBenchmark()
        {
            // Arrange
            var benchmarks = new ProteinBenchmarkService(CreateTestDataContainer());
            var slotIdentifier = new SlotIdentifier("New Client", 0, "New Path");
            var projectID = Int32.MaxValue;
            var frameTimes = new[]
            {
                TimeSpan.FromMinutes(1.0),
                TimeSpan.FromMinutes(1.2),
                TimeSpan.FromMinutes(1.1)
            };
            // Act
            benchmarks.Update(slotIdentifier, projectID, frameTimes);
            // Assert
            var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(3, benchmark.FrameTimes.Count);
            var proteinFrameTimes = benchmark.FrameTimes;
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), proteinFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), proteinFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), proteinFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_UpdatesExistingBenchmark()
        {
            // Arrange
            var benchmarks = new ProteinBenchmarkService(CreateTestDataContainer());
            var slotIdentifier = new SlotIdentifier("Windows - Main Workstation", 0, "192.168.1.194-36330");
            var projectID = 9039;
            var frameTimes = new[]
            {
                TimeSpan.FromMinutes(1.0),
                TimeSpan.FromMinutes(1.2),
                TimeSpan.FromMinutes(1.1)
            };
            // Act
            benchmarks.Update(slotIdentifier, projectID, frameTimes);
            // Assert
            var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
            Assert.IsNotNull(benchmark);
            var proteinFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), proteinFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), proteinFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), proteinFrameTimes[2].Duration);
        }

        private static ProteinBenchmarkDataContainer CreateTestDataContainer()
        {
            var source = Path.Combine("..\\..\\TestFiles", ProteinBenchmarkDataContainer.DefaultFileName);
            File.Copy(source, _TestFile, true);

            var dataContainer = new ProteinBenchmarkDataContainer
            {
                FilePath = _TestFile
            };
            dataContainer.Read();
            return dataContainer;
        }
    }
}
