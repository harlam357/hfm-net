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

            var client = GetOrCreateClientEntity(context, workUnitModel);
            var protein = GetOrCreateProteinEntity(context, workUnitModel);
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

        private static ClientEntity GetOrCreateClientEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
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

        private static ProteinEntity GetOrCreateProteinEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
        {
            var protein = new ProteinEntity();
            protein.ProjectID = workUnitModel.CurrentProtein.ProjectNumber;

            context.Proteins.Add(protein);
            context.SaveChanges();

            return protein;
        }

        private static WorkUnitEntity InsertWorkUnitEntity(WorkUnitContext context, WorkUnitModel workUnitModel, long clientID, long proteinID)
        {
            var workUnit = new WorkUnitEntity();
            workUnit.Result = WorkUnitResultString.FromWorkUnitResult(workUnitModel.WorkUnit.UnitResult);
            workUnit.Assigned = workUnitModel.WorkUnit.Assigned;
            workUnit.Finished = workUnitModel.WorkUnit.Finished;
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

            if (slotIdentifier.SlotID != SlotIdentifier.NoSlotID)
            {
                query = query
                    .Where(x => x.ClientSlot == slotIdentifier.SlotID);
            }

            return query.Count();
        }

        public long CountFailed(string clientName, DateTime? clientStartTime)
        {
            using var context = CreateWorkUnitContext();

            var slotIdentifier = SlotIdentifier.FromName(clientName, String.Empty, Guid.Empty);

            var query = context.WorkUnits
                .Include(x => x.Client)
                .Where(x => x.Client.Name == slotIdentifier.ClientIdentifier.Name && x.Result != WorkUnitResultString.FinishedUnit);

            if (slotIdentifier.SlotID != SlotIdentifier.NoSlotID)
            {
                query = query
                    .Where(x => x.ClientSlot == slotIdentifier.SlotID);
            }

            return query.Count();
        }

        protected abstract WorkUnitContext CreateWorkUnitContext();
    }
}
