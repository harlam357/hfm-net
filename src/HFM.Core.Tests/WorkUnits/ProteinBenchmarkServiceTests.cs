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
using HFM.Core.DataTypes;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProteinBenchmarkServiceTests
    {
        [Test]
        public void ProteinBenchmarkService_GetSlotIdentifiers_Test()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarks = new ProteinBenchmarkService(container);
                // Act
                var identifiers = benchmarks.GetSlotIdentifiers();
                // Assert
                Assert.AreEqual(12, identifiers.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_Update_CreatesNewBenchmark()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                int containerCount = container.Data.Count;
                var benchmarks = new ProteinBenchmarkService(container);
                var clientGuid = Guid.NewGuid();
                var slotIdentifier = new SlotIdentifier(new ClientIdentifier("New Client", "10.9.8.1", ClientSettings.DefaultPort, clientGuid), 0);
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
                Assert.AreEqual(containerCount + 1, container.Data.Count);
                var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
                Assert.IsNotNull(benchmark);
                Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
                Assert.AreEqual(clientGuid, benchmark.SlotIdentifier.Client.Guid);

                Assert.AreEqual(3, benchmark.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmark.FrameTimes[0].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmark.FrameTimes[1].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmark.FrameTimes[2].Duration);
            }
        }

        [Test]
        public void ProteinBenchmarkService_Update_UpdatesExistingBenchmark_ByMatchingClientIdentifierWithGuid()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                int containerCount = container.Data.Count;
                var benchmarks = new ProteinBenchmarkService(container);
                var clientGuid = Guid.NewGuid();
                var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Windows - Main Workstation", "192.168.1.194", 36330, clientGuid), 0);
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
                Assert.AreEqual(containerCount, container.Data.Count);
                var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
                Assert.IsNotNull(benchmark);
                Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
                Assert.AreEqual(clientGuid, benchmark.SlotIdentifier.Client.Guid);
                
                var proteinFrameTimes = benchmark.FrameTimes.Take(3).ToList();
                Assert.AreEqual(TimeSpan.FromMinutes(1.1), proteinFrameTimes[0].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.2), proteinFrameTimes[1].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.0), proteinFrameTimes[2].Duration);
            }
        }

        [Test]
        public void ProteinBenchmarkService_Update_UpdatesExistingBenchmark_ByMatchingClientIdentifierWithoutGuid()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                int containerCount = container.Data.Count;
                var benchmarks = new ProteinBenchmarkService(container);
                var slotIdentifier = new SlotIdentifier(ClientIdentifier.FromPath("Windows - Main Workstation", "192.168.1.194-36330"), 0);
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
                Assert.AreEqual(containerCount, container.Data.Count);
                var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
                Assert.IsNotNull(benchmark);
                Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
                Assert.AreEqual(Guid.Empty, benchmark.SlotIdentifier.Client.Guid);

                var proteinFrameTimes = benchmark.FrameTimes.Take(3).ToList();
                Assert.AreEqual(TimeSpan.FromMinutes(1.1), proteinFrameTimes[0].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.2), proteinFrameTimes[1].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.0), proteinFrameTimes[2].Duration);
            }
        }

        [Test]
        public void ProteinBenchmarkService_Update_UpdatesExistingBenchmarkSourceProperties_ByMatchingClientIdentifierWithGuid()
        {
            // Arrange
            var container = new ProteinBenchmarkDataContainer();
            var clientName = "Name";
            var clientServer = "Server";
            var clientPort = 36331;
            var clientGuid = Guid.NewGuid();
            var slotID = 1;
            var projectID = Int32.MaxValue;
            container.Data.Add(new ProteinBenchmark { SourceGuid = clientGuid, SourceSlotID = slotID, ProjectID = projectID });
            var benchmarks = new ProteinBenchmarkService(container);
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier(clientName, clientServer, clientPort, clientGuid), slotID);
            // Act
            benchmarks.Update(slotIdentifier, projectID, Enumerable.Empty<TimeSpan>());
            // Assert
            Assert.AreEqual(1, container.Data.Count);
            var benchmark = benchmarks.GetBenchmark(slotIdentifier, projectID);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(clientName, benchmark.SourceName);
            Assert.AreEqual($"{clientServer}:{clientPort}", benchmark.SourcePath);
            Assert.AreEqual(clientGuid, benchmark.SourceGuid);
            Assert.AreEqual(slotID, benchmark.SourceSlotID);
        }

        private static ProteinBenchmarkDataContainer CreateTestDataContainer(string path)
        {
            var source = Path.Combine("..\\..\\TestFiles", ProteinBenchmarkDataContainer.DefaultFileName);
            File.Copy(source, path, true);

            var dataContainer = new ProteinBenchmarkDataContainer { FilePath = path };
            dataContainer.Read();
            return dataContainer;
        }
    }
}
