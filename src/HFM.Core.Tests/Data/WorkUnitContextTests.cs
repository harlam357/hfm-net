using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitContextTests
    {
        [TestFixture]
        public class CanAddWorkUnitEntity : WorkUnitContextTests
        {
            private ArtifactFolder _artifacts;
            private WorkUnitContext _context;

            [SetUp]
            public void BeforeEach()
            {
                _artifacts = new ArtifactFolder();
                string connectionString = $"Data Source={_artifacts.GetRandomFilePath()}";
                _context = new WorkUnitContext(connectionString);
                _context.Database.EnsureCreated();

                _context.Proteins.Add(new ProteinEntity());
                _context.SaveChanges();

                _context.Clients.Add(new ClientEntity());
                _context.SaveChanges();
            }

            [TearDown]
            public void AfterEach()
            {
                _artifacts?.Dispose();
                _context?.Dispose();
            }

            [Test]
            public void WithFrames()
            {
                _context.WorkUnits.Add(new WorkUnitEntity
                {
                    ProteinID = _context.Proteins.First().ID,
                    ClientID = _context.Clients.First().ID
                });
                _context.SaveChanges();

                _context.WorkUnitFrames.Add(new WorkUnitFrameEntity
                {
                    WorkUnitID = _context.WorkUnits.First().ID,
                    FrameID = 0
                });
                _context.WorkUnitFrames.Add(new WorkUnitFrameEntity
                {
                    WorkUnitID = _context.WorkUnits.First().ID,
                    FrameID = 1
                });
                _context.SaveChanges();

                var workUnit = _context.WorkUnits
                    .Include(x => x.Client)
                    .Include(x => x.Protein)
                    .Include(x => x.Frames)
                    .First();
                Assert.IsNotNull(workUnit);
                Assert.IsNotNull(workUnit.Client);
                Assert.IsNotNull(workUnit.Protein);
                Assert.AreEqual(2, workUnit.Frames.Count);
            }

            [Test]
            public void WithPlatform()
            {
                _context.Platforms.Add(new PlatformEntity
                {
                    ClientVersion = "7",
                    OperatingSystem = "Windows",
                    Implementation = "CPU",
                    Processor = "Ryzen"
                });
                _context.SaveChanges();

                _context.WorkUnits.Add(new WorkUnitEntity
                {
                    ProteinID = _context.Proteins.First().ID,
                    ClientID = _context.Clients.First().ID,
                    PlatformID = _context.Platforms.First().ID
                });
                _context.SaveChanges();

                var workUnit = _context.WorkUnits
                    .Include(x => x.Client)
                    .Include(x => x.Protein)
                    .Include(x => x.Platform)
                    .First();
                Assert.IsNotNull(workUnit);
                Assert.IsNotNull(workUnit.Client);
                Assert.IsNotNull(workUnit.Protein);
                Assert.IsNotNull(workUnit.Platform);
            }
        }
    }
}
