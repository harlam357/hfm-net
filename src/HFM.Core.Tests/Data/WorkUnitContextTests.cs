using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitContextTests
    {
        [Test]
        public void WorkUnitContext_CanAddWorkUnitEntity()
        {
            using var artifacts = new ArtifactFolder();
            string connectionString = $"Data Source={Path.Combine(artifacts.Path, "WorkUnits.db")}";
            using var context = new WorkUnitContext(connectionString);
            context.Database.EnsureCreated();

            context.Proteins.Add(new ProteinEntity());
            context.SaveChanges();

            context.Clients.Add(new ClientEntity());
            context.SaveChanges();

            context.WorkUnits.Add(new WorkUnitEntity
            {
                ProteinID = context.Proteins.First().ID,
                ClientID = context.Clients.First().ID
            });
            context.SaveChanges();

            context.WorkUnitFrames.Add(new WorkUnitFrameEntity
            {
                WorkUnitID = context.WorkUnits.First().ID,
                FrameID = 0
            });
            context.WorkUnitFrames.Add(new WorkUnitFrameEntity
            {
                WorkUnitID = context.WorkUnits.First().ID,
                FrameID = 1
            });
            context.SaveChanges();

            var workUnit = context.WorkUnits
                .Include(x => x.Client)
                .Include(x => x.Protein)
                .Include(x => x.Frames)
                .First();
            Assert.IsNotNull(workUnit);
            Assert.IsNotNull(workUnit.Client);
            Assert.IsNotNull(workUnit.Protein);
            Assert.AreEqual(2, workUnit.Frames.Count);

            context.Database.EnsureDeleted();
        }

        [Test]
        public async Task WorkUnitContext_CanMigrateWuHistory()
        {
            using var artifacts = new ArtifactFolder();
            string connectionString = $"Data Source={Path.Combine(artifacts.Path, "Migrated.db")}";
            using var context = new WorkUnitContext(connectionString);
            await context.Database.EnsureCreatedAsync();

            var repository = new WorkUnitRepository(null, null);
            repository.Initialize(@"C:\Path to\WuHistory.db3");
            var rows = repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);

            var proteins = new HashSet<ProteinEntity>();
            var clients = new HashSet<ClientEntity>();
            foreach (var r in rows)
            {
                var p = new ProteinEntity
                {
                    ProjectID = r.ProjectID,
                    Atoms = r.Atoms,
                    Core = r.Core,
                    Credit = r.BaseCredit,
                    Frames = r.Frames,
                    KFactor = r.KFactor,
                    TimeoutDays = r.PreferredDays,
                    ExpirationDays = r.MaximumDays
                };
                proteins.Add(p);

                var c = new ClientEntity
                {
                    Name = r.Name,
                    ConnectionString = r.Path
                };
                clients.Add(c);
            }

            context.Proteins.AddRange(proteins);
            Console.WriteLine($"Added {context.Proteins.Local.Count} Proteins");

            context.Clients.AddRange(clients);
            Console.WriteLine($"Added {context.Clients.Local.Count} Clients");

            await context.SaveChangesAsync();

            var workUnits = new HashSet<WorkUnitEntity>();
            foreach (var r in rows)
            {
                var p = await context.Proteins.FirstAsync(x =>
                    x.ProjectID == r.ProjectID &&
                    Math.Abs(x.Credit - r.BaseCredit) < 0.001 &&
                    Math.Abs(x.KFactor - r.KFactor) < 0.001 &&
                    x.Frames == r.Frames &&
                    x.Core == r.Core &&
                    x.Atoms == r.Atoms &&
                    Math.Abs(x.TimeoutDays - r.PreferredDays) < 0.001 &&
                    Math.Abs(x.ExpirationDays - r.MaximumDays) < 0.001);

                var c = await context.Clients.FirstAsync(x =>
                    x.Name == r.Name &&
                    x.ConnectionString == r.Path);

                var w = new WorkUnitEntity
                {
                    ProteinID = p.ID,
                    ClientID = c.ID,
                    DonorName = r.Username,
                    DonorTeam = r.Team,
                    CoreVersion = r.CoreVersion.ToString(CultureInfo.InvariantCulture),
                    Result = r.Result,
                    Assigned = r.Assigned,
                    Finished = r.Finished,
                    ProjectRun = r.ProjectRun,
                    ProjectClone = r.ProjectClone,
                    ProjectGen = r.ProjectGen,
                    HexID = null,
                    FramesCompleted = r.FramesCompleted,
                    FrameTimeInSeconds = r.FrameTimeValue,
                    ClientSlot = null
                };
                workUnits.Add(w);
            }

            context.WorkUnits.AddRange(workUnits);
            Console.WriteLine($"Added {context.WorkUnits.Local.Count} WorkUnits");

            await context.SaveChangesAsync();
        }
    }
}
