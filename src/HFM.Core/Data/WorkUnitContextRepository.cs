using System.Data.Entity;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

using Microsoft.Extensions.DependencyInjection;

using PetaPoco;

namespace HFM.Core.Data
{
    public class ScopedWorkUnitContextRepository : WorkUnitContextRepository
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScopedWorkUnitContextRepository(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override WorkUnitContext CreateWorkUnitContext()
        {
            var scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        }
    }

    public abstract class WorkUnitContextRepository : IWorkUnitRepository
    {
        public bool Connected => true;

        public bool Insert(WorkUnitModel workUnitModel)
        {
            if (!ValidateWorkUnit(workUnitModel.WorkUnit))
            {
                return false;
            }

            // ensure the given work unit is not written more than once
            if (WorkUnitExists(workUnitModel.WorkUnit))
            {
                return false;
            }

            using var context = CreateWorkUnitContext();

            var client = GetOrInsertClientEntity(context, workUnitModel);
            var protein = GetOrInsertProteinEntity(context, workUnitModel);
            var workUnit = InsertWorkUnitEntity(context, workUnitModel, client.ID, protein.ID);

            return true;
        }

        private static bool ValidateWorkUnit(WorkUnit workUnit) =>
            workUnit.HasProject() &&
            !workUnit.Assigned.IsMinValue() &&
            !workUnit.Finished.IsMinValue();

        private bool WorkUnitExists(WorkUnit workUnit)
        {
            using var context = CreateWorkUnitContext();

            return context.WorkUnits
                .Include(x => x.Protein)
                .Any(x => x.Protein.ProjectID == workUnit.ProjectID &&
                          x.ProjectRun == workUnit.ProjectRun &&
                          x.ProjectClone == workUnit.ProjectClone &&
                          x.ProjectGen == workUnit.ProjectGen &&
                          x.Assigned == workUnit.Assigned);
        }

        private static ClientEntity GetOrInsertClientEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
        {
            ClientEntity client = null;

            var identifier = workUnitModel.SlotModel.SlotIdentifier.ClientIdentifier;
            if (identifier.HasGuid)
            {
                client = context.Clients
                    .FirstOrDefault(x => x.Guid == identifier.Guid.ToString());
            }

            if (client is null)
            {
                client = context.Clients
                    .FirstOrDefault(x => x.Name == identifier.Name && x.ConnectionString == identifier.ToServerPortString());
            }

            if (client is null)
            {
                client = new ClientEntity
                {
                    Name = identifier.Name,
                    ConnectionString = identifier.ToServerPortString(),
                    Guid = identifier.Guid.ToString()
                };
                context.Clients.Add(client);
                context.SaveChanges();
            }
            else if (client.Guid is null)
            {
                client.Guid = identifier.Guid.ToString();
                context.SaveChanges();
            }

            return client;
        }

        private static ProteinEntity GetOrInsertProteinEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
        {
            var protein = new ProteinEntity();
            protein.ProjectID = workUnitModel.CurrentProtein.ProjectNumber;
            protein.Credit = workUnitModel.CurrentProtein.Credit;
            protein.KFactor = workUnitModel.CurrentProtein.KFactor;
            protein.Frames = workUnitModel.CurrentProtein.Frames;
            protein.Core = workUnitModel.CurrentProtein.Core;
            protein.Atoms = workUnitModel.CurrentProtein.NumberOfAtoms;
            protein.TimeoutDays = workUnitModel.CurrentProtein.PreferredDays;
            protein.ExpirationDays = workUnitModel.CurrentProtein.MaximumDays;

            context.Proteins.Add(protein);
            context.SaveChanges();

            return protein;
        }

        private static WorkUnitEntity InsertWorkUnitEntity(WorkUnitContext context, WorkUnitModel workUnitModel, long clientID, long proteinID)
        {
            var workUnit = new WorkUnitEntity();
            workUnit.DonorName = workUnitModel.WorkUnit.FoldingID;
            workUnit.DonorTeam = workUnitModel.WorkUnit.Team;
            workUnit.CoreVersion = workUnitModel.WorkUnit.CoreVersion?.ToString();
            workUnit.Result = WorkUnitResultString.FromWorkUnitResult(workUnitModel.WorkUnit.UnitResult);
            workUnit.Assigned = workUnitModel.WorkUnit.Assigned;
            workUnit.Finished = workUnitModel.WorkUnit.Finished;
            workUnit.ProjectRun = workUnitModel.WorkUnit.ProjectRun;
            workUnit.ProjectClone = workUnitModel.WorkUnit.ProjectClone;
            workUnit.ProjectGen = workUnitModel.WorkUnit.ProjectGen;
            workUnit.FramesCompleted = workUnitModel.FramesComplete;
            workUnit.FrameTimeInSeconds = workUnitModel.GetRawTime(PPDCalculation.AllFrames);
            workUnit.ProteinID = proteinID;
            workUnit.ClientID = clientID;

            context.WorkUnits.Add(workUnit);
            context.SaveChanges();

            return workUnit;
        }

        public int Delete(WorkUnitRow row) => throw new NotImplementedException();

        public IList<WorkUnitRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation) => throw new NotImplementedException();

        public Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation) => throw new NotImplementedException();

        public long CountCompleted(string clientName, DateTime? clientStartTime)
        {
            using var context = CreateWorkUnitContext();

            var slotIdentifier = SlotIdentifier.FromName(clientName, String.Empty, Guid.Empty);

            var query = context.WorkUnits
                .Include(x => x.Client)
                .Where(x => x.Client.Name == slotIdentifier.ClientIdentifier.Name && x.Result == WorkUnitResultString.FinishedUnit);

            query = WhereClientSlot(query, slotIdentifier.SlotID);
            query = WhereFinishedAfterClientStart(query, clientStartTime);
            return query.Count();
        }

        public long CountFailed(string clientName, DateTime? clientStartTime)
        {
            using var context = CreateWorkUnitContext();

            var slotIdentifier = SlotIdentifier.FromName(clientName, String.Empty, Guid.Empty);

            var query = context.WorkUnits
                .Include(x => x.Client)
                .Where(x => x.Client.Name == slotIdentifier.ClientIdentifier.Name && x.Result != WorkUnitResultString.FinishedUnit);

            query = WhereClientSlot(query, slotIdentifier.SlotID);
            query = WhereFinishedAfterClientStart(query, clientStartTime);
            return query.Count();
        }

        private static IQueryable<WorkUnitEntity> WhereClientSlot(IQueryable<WorkUnitEntity> query, int slotID)
        {
            if (slotID != SlotIdentifier.NoSlotID)
            {
                query = query.Where(x => x.ClientSlot == slotID);
            }
            return query;
        }

        private static IQueryable<WorkUnitEntity> WhereFinishedAfterClientStart(IQueryable<WorkUnitEntity> query, DateTime? clientStartTime)
        {
            if (clientStartTime.HasValue)
            {
                query = query.Where(x => x.Finished > clientStartTime.Value);
            }
            return query;
        }

        protected abstract WorkUnitContext CreateWorkUnitContext();
    }
}
