using NUnit.Framework;

using HFM.Core.Client;
using HFM.Core.Data;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProteinBenchmarkServiceTests
    {
        [Test]
        public void ProteinBenchmarkService_GetSlotIdentifiers_ReturnsDistinctValuesFromAllBenchmarks()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var identifiers = benchmarkService.GetSlotIdentifiers();
                // Assert
                Assert.AreEqual(11, identifiers.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_GetSlotIdentifiers_ReturnsDistinctValuesWithGuidFromBenchmarksWithAndWithoutSourceGuid()
        {
            // Arrange
            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark { SourceName = "Foo", SourcePath = "192.168.1.255:36330", SourceGuid = Guid.Empty, SourceSlotID = 0 });
            container.Data.Add(new ProteinBenchmark { SourceName = "Foo", SourcePath = "192.168.1.255:36330", SourceGuid = Guid.NewGuid(), SourceSlotID = 0 });
            var benchmarkService = new ProteinBenchmarkService(container);
            // Act
            var identifiers = benchmarkService.GetSlotIdentifiers();
            // Assert
            Assert.AreEqual(1, identifiers.Count);
            Assert.IsTrue(identifiers.First().ClientIdentifier.HasGuid);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarkProjects_ForAllSlots()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var projects = benchmarkService.GetBenchmarkProjects(SlotIdentifier.AllSlots);
                // Assert
                Assert.AreEqual(355, projects.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarkProjects_ForSpecificSlot()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var projects = benchmarkService.GetBenchmarkProjects(benchmarkService.GetSlotIdentifiers().First());
                // Assert
                Assert.AreEqual(45, projects.Count);
            }
        }

        private const string Processor = "Ryzen";
        private const int Threads = 16;

        [Test]
        public void ProteinBenchmarkService_Update_CreatesNewBenchmark()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                int containerCount = container.Data.Count;
                var benchmarkService = new ProteinBenchmarkService(container);
                var clientGuid = Guid.NewGuid();
                var slotIdentifier = new SlotIdentifier(new ClientIdentifier("New Client", "10.9.8.1", ClientSettings.DefaultPort, clientGuid), 0);
                var benchmarkIdentifier = new ProteinBenchmarkIdentifier(Int32.MaxValue, Processor, Threads);
                var frameTimes = new[]
                {
                    TimeSpan.FromMinutes(1.0),
                    TimeSpan.FromMinutes(1.2),
                    TimeSpan.FromMinutes(1.1)
                };
                // Act
                benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);
                // Assert
                Assert.AreEqual(containerCount + 1, container.Data.Count);
                var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);
                Assert.IsNotNull(benchmark);
                Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
                Assert.AreEqual(clientGuid, benchmark.SlotIdentifier.ClientIdentifier.Guid);
                Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

                Assert.AreEqual(3, benchmark.FrameTimes.Count);
                Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmark.FrameTimes[0].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmark.FrameTimes[1].Duration);
                Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmark.FrameTimes[2].Duration);
            }
        }

        [Test]
        public void ProteinBenchmarkService_Update_AddsFrameTimes()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark { SourceGuid = guid, SourceSlotID = slotID, ProjectID = projectID });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, guid), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(3, benchmark.FrameTimes.Count);
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmark.FrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmark.FrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmark.FrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithGuidMatchesSlotIdentifierByGuid()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = guid,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Bar", "192.168.1.200", 46330, guid), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithGuidMatchesSlotIdentifierByNameAndPath()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = guid,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.Empty), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithoutGuidMatchesSlotIdentifierByNameAndPath()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = Guid.Empty,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, guid), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithoutProcessorMatchesBenchmarkIdentifierByProjectID()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, 0);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithoutProcessorAndThreadsMatchesBenchmarkIdentifierByProjectID()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, Threads);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(1, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_BenchmarkWithProcessorAndThreadsMatchesBenchmarkIdentifierByProjectIDAndProcessorAndThreads()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID, Processor = Processor, Threads = 12 }
                .UpdateFromSlotIdentifier(slotIdentifier));
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID, Processor = Processor, Threads = Threads }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, Threads);
            var frameTimes = new[] { 1.0, 1.2, 1.1 }.Select(TimeSpan.FromMinutes);

            // Act
            var benchmark = benchmarkService.Update(slotIdentifier, benchmarkIdentifier, frameTimes);

            // Assert
            Assert.AreEqual(2, container.Data.Count);
            Assert.IsNotNull(benchmark);
            Assert.AreSame(container.Data[1], benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);

            var benchmarkFrameTimes = benchmark.FrameTimes.Take(3).ToList();
            Assert.AreEqual(TimeSpan.FromMinutes(1.1), benchmarkFrameTimes[0].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.2), benchmarkFrameTimes[1].Duration);
            Assert.AreEqual(TimeSpan.FromMinutes(1.0), benchmarkFrameTimes[2].Duration);
        }

        [Test]
        public void ProteinBenchmarkService_Update_ThrowsWhenSlotIdentifierIsAllSlots()
        {
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            Assert.Throws<ArgumentException>(() => benchmarkService.Update(SlotIdentifier.AllSlots, new ProteinBenchmarkIdentifier(), new List<TimeSpan>()));
        }

        [Test]
        public void ProteinBenchmarkService_Update_ThrowsWhenFrameTimesIsNull()
        {
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            Assert.Throws<ArgumentNullException>(() => benchmarkService.Update(new SlotIdentifier(), new ProteinBenchmarkIdentifier(), null));
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithGuidMatchesSlotIdentifierByGuid()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = guid,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Bar", "192.168.1.200", 46330, guid), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithGuidMatchesSlotIdentifierByNameAndPath()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = guid,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.Empty), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier.ClientIdentifier.Name, benchmark.SourceName);
            Assert.AreEqual(slotIdentifier.ClientIdentifier.ToConnectionString(), benchmark.SourcePath);
            Assert.AreNotEqual(slotIdentifier.ClientIdentifier.Guid, benchmark.SourceGuid);
            Assert.AreEqual(slotIdentifier.SlotID, benchmark.SourceSlotID);
            Assert.AreEqual(guid, benchmark.SourceGuid);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithoutGuidMatchesSlotIdentifierByNameAndPath()
        {
            var guid = Guid.NewGuid();
            const int slotID = 0;
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            container.Data.Add(new ProteinBenchmark
            {
                SourceName = "Foo",
                SourcePath = "192.168.1.194:36330",
                SourceGuid = Guid.Empty,
                SourceSlotID = slotID,
                ProjectID = projectID,
                FrameTimes = new[] { 1.4, 1.6, 1.5 }.Select(ProteinBenchmarkFrameTime.FromMinutes).ToList()
            });
            var benchmarkService = new ProteinBenchmarkService(container);

            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, guid), slotID);
            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier.ClientIdentifier.Name, benchmark.SourceName);
            Assert.AreEqual(slotIdentifier.ClientIdentifier.ToConnectionString(), benchmark.SourcePath);
            Assert.AreNotEqual(slotIdentifier.ClientIdentifier.Guid, benchmark.SourceGuid);
            Assert.AreEqual(slotIdentifier.SlotID, benchmark.SourceSlotID);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithoutProcessorMatchesBenchmarkIdentifierByProjectID()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, 0);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier.ProjectID, benchmark.ProjectID);
            Assert.AreNotEqual(benchmarkIdentifier.Processor, benchmark.Processor);
            Assert.AreEqual(benchmarkIdentifier.Threads, benchmark.Threads);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithoutProcessorAndThreadsMatchesBenchmarkIdentifierByProjectID()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, Threads);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier.ProjectID, benchmark.ProjectID);
            Assert.AreNotEqual(benchmarkIdentifier.Processor, benchmark.Processor);
            Assert.AreNotEqual(benchmarkIdentifier.Threads, benchmark.Threads);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmark_BenchmarkWithProcessorAndThreadsMatchesBenchmarkIdentifierByProjectIDAndProcessorAndThreads()
        {
            const int projectID = 9039;

            var container = new ProteinBenchmarkDataContainer();
            var slotIdentifier = new SlotIdentifier(new ClientIdentifier("Foo", "192.168.1.194", ClientSettings.DefaultPort, Guid.NewGuid()), 0);
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID, Processor = Processor, Threads = 12 }
                .UpdateFromSlotIdentifier(slotIdentifier));
            container.Data.Add(new ProteinBenchmark { ProjectID = projectID, Processor = Processor, Threads = Threads }
                .UpdateFromSlotIdentifier(slotIdentifier));
            var benchmarkService = new ProteinBenchmarkService(container);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(projectID, Processor, Threads);

            // Act
            var benchmark = benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier);

            // Assert
            Assert.IsNotNull(benchmark);
            Assert.AreSame(container.Data[1], benchmark);
            Assert.AreEqual(slotIdentifier, benchmark.SlotIdentifier);
            Assert.AreEqual(benchmarkIdentifier, benchmark.BenchmarkIdentifier);
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarks_ForSpecificSlotAndProject()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var benchmarks = benchmarkService.GetBenchmarks(benchmarkService.GetSlotIdentifiers().First(), 9039);
                // Assert
                Assert.AreEqual(1, benchmarks.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarks_ForAllSlotsAndSpecificProject()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var benchmarks = benchmarkService.GetBenchmarks(SlotIdentifier.AllSlots, 9039);
                // Assert
                Assert.AreEqual(4, benchmarks.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarks_ForSpecificSlotAndProjects()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var benchmarks = benchmarkService.GetBenchmarks(benchmarkService.GetSlotIdentifiers().First(), new[] { 9039, 9032 });
                // Assert
                Assert.AreEqual(2, benchmarks.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_GetBenchmarks_ForAllSlotsAndSpecificProjects()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                var benchmarks = benchmarkService.GetBenchmarks(SlotIdentifier.AllSlots, new[] { 9039, 9032 });
                // Assert
                Assert.AreEqual(8, benchmarks.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_RemoveAll_ThrowsWhenSlotIdentifierIsAllSlots()
        {
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            Assert.Throws<ArgumentException>(() => benchmarkService.RemoveAll(SlotIdentifier.AllSlots));
        }

        [Test]
        public void ProteinBenchmarkService_RemoveAll_ForSpecificSlot()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                benchmarkService.RemoveAll(benchmarkService.GetSlotIdentifiers().First());
                // Assert
                Assert.AreEqual(647, container.Data.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_RemoveAll_ForAllSlotsAndSpecificProject()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath());
                var benchmarkService = new ProteinBenchmarkService(container);
                // Act
                benchmarkService.RemoveAll(SlotIdentifier.AllSlots, 9039);
                // Assert
                Assert.AreEqual(688, container.Data.Count);
            }
        }

        [Test]
        public void ProteinBenchmarkService_ValidateProteinBenchmarkEqualityComparerMatchesWithoutConsideringClientIdentifierGuid()
        {
            using (var artifacts = new ArtifactFolder())
            {
                var container = CreateTestDataContainer(artifacts.GetRandomFilePath(), "BenchmarkCache_0_9_13.dat");

                var benchmarksWithGuid = container.Data.Where(x => x.SlotIdentifier.ClientIdentifier.HasGuid).ToList();
                var benchmarksWithGuidIdentifiers = benchmarksWithGuid.Select(x => x.SlotIdentifier).Distinct().ToList();
                foreach (var identifier in benchmarksWithGuidIdentifiers)
                {
                    Console.WriteLine(identifier);
                }

                var identifierMatchesWhenGuidIsNotConsidered = new Func<ProteinBenchmark, bool>(b =>
                    benchmarksWithGuidIdentifiers.Any(x => SlotIdentifier.ProteinBenchmarkEqualityComparer.Equals(b.SlotIdentifier, x)));

                var allBenchmarks = container.Data.Where(b => identifierMatchesWhenGuidIsNotConsidered(b)).ToList();
                var benchmarksWithoutGuid = allBenchmarks.Where(b => !b.SlotIdentifier.ClientIdentifier.HasGuid).ToList();

                Console.WriteLine($"Benchmarks with Guid: {benchmarksWithGuid.Count}");
                Console.WriteLine($"Benchmarks without Guid: {benchmarksWithoutGuid.Count}");
                Console.WriteLine($"All Benchmarks: {allBenchmarks.Count}");

                Assert.AreEqual(benchmarksWithGuid.Count + benchmarksWithoutGuid.Count, allBenchmarks.Count);
            }
        }

        private static ProteinBenchmarkDataContainer CreateTestDataContainer(string path, string fileName = ProteinBenchmarkDataContainer.DefaultFileName)
        {
            var source = Path.GetFullPath(Path.Combine("TestFiles", fileName));
            File.Copy(source, path, true);

            var dataContainer = new ProteinBenchmarkDataContainer { FilePath = path };
            dataContainer.Read();
#if DEBUG
            Console.WriteLine($"Loaded {dataContainer.Data.Count} benchmarks");

            Console.WriteLine("Group By Slot");
            foreach (var g in dataContainer.Data.GroupBy(x => x.SlotIdentifier))
            {
                Console.WriteLine(g.Key);
                foreach (var b in g)
                {
                    Console.WriteLine("\t" + b.BenchmarkIdentifier);
                }
            }

            Console.WriteLine("Group By Benchmark");
            foreach (var g in dataContainer.Data.GroupBy(x => x.BenchmarkIdentifier))
            {
                Console.WriteLine(g.Key);
                foreach (var b in g)
                {
                    Console.WriteLine("\t" + b.SlotIdentifier);
                }
            }
#endif
            return dataContainer;
        }
    }
}
