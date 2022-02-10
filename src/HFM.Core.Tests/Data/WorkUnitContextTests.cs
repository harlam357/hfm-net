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
            string connectionString = $"Data Source={artifacts.GetRandomFilePath()}";
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
        }
    }
}
