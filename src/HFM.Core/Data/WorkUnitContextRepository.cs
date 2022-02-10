using System.Diagnostics;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public class ScopedWorkUnitContextRepository : WorkUnitContextRepository
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopedWorkUnitContextRepository(ILogger logger, IServiceScopeFactory serviceScopeFactory) : base(logger)
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
    public ILogger Logger { get; }

    protected WorkUnitContextRepository() : this(null)
    {

    }

    protected WorkUnitContextRepository(ILogger logger)
    {
        Logger = logger ?? NullLogger.Instance;
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<WorkUnitRowProfile>()).CreateMapper();
    }

    private readonly IMapper _mapper;

    public bool Connected => true;

    public long Insert(WorkUnitModel workUnitModel)
    {
        if (!ValidateWorkUnit(workUnitModel.WorkUnit))
        {
            return -1;
        }

        if (WorkUnitExists(workUnitModel.WorkUnit))
        {
            return -1;
        }

        using var context = CreateWorkUnitContext();

        var client = GetOrInsertClientEntity(context, workUnitModel);
        var protein = GetOrInsertProteinEntity(context, workUnitModel);
        var workUnit = InsertWorkUnitEntity(context, workUnitModel, client.ID, protein.ID);
        InsertWorkUnitFrameEntities(context, workUnitModel, workUnit.ID);

        return workUnit.ID;
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
                      x.Assigned == workUnit.Assigned.Normalize());
    }

    private static ClientEntity GetOrInsertClientEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        ClientEntity client = null;

        var identifier = workUnitModel.SlotModel.SlotIdentifier.ClientIdentifier;
        string connectionString = identifier.ToConnectionString();
        string guid = identifier.HasGuid ? identifier.Guid.ToString() : null;

        if (guid is not null)
        {
            client = context.Clients.OrderByDescending(x => x.ID).FirstOrDefault(x => x.Guid == guid);
            if (client is not null && (client.Name != identifier.Name || client.ConnectionString != connectionString))
            {
                client = new ClientEntity
                {
                    Name = identifier.Name,
                    ConnectionString = connectionString,
                    Guid = guid
                };
                context.Clients.Add(client);
                context.SaveChanges();
            }
        }

        if (client is null)
        {
            client = context.Clients.FirstOrDefault(x => x.Name == identifier.Name && x.ConnectionString == connectionString);
            if (client is not null && client.Guid is null && guid is not null)
            {
                client.Guid = guid;
                context.SaveChanges();
            }
        }

        if (client is null)
        {
            client = new ClientEntity
            {
                Name = identifier.Name,
                ConnectionString = connectionString,
                Guid = guid
            };
            context.Clients.Add(client);
            context.SaveChanges();
        }

        return client;
    }

    private static ProteinEntity GetOrInsertProteinEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        var p = workUnitModel.CurrentProtein;
        var protein = context.Proteins
            .FirstOrDefault(x => x.ProjectID == p.ProjectNumber &&
                                 Math.Abs(x.Credit - p.Credit) < 0.001 &&
                                 Math.Abs(x.KFactor - p.KFactor) < 0.001 &&
                                 x.Frames == p.Frames &&
                                 x.Core == p.Core &&
                                 x.Atoms == p.NumberOfAtoms &&
                                 Math.Abs(x.TimeoutDays - p.PreferredDays) < 0.001 &&
                                 Math.Abs(x.ExpirationDays - p.MaximumDays) < 0.001);

        if (protein is null)
        {
            protein = new ProteinEntity();
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
        }

        return protein;
    }

    private static WorkUnitEntity InsertWorkUnitEntity(WorkUnitContext context, WorkUnitModel workUnitModel, long clientID, long proteinID)
    {
        var workUnit = new WorkUnitEntity();
        workUnit.DonorName = workUnitModel.WorkUnit.FoldingID;
        workUnit.DonorTeam = workUnitModel.WorkUnit.Team;
        workUnit.CoreVersion = workUnitModel.WorkUnit.CoreVersion?.ToString();
        workUnit.Result = WorkUnitResultString.FromWorkUnitResult(workUnitModel.WorkUnit.UnitResult);
        workUnit.Assigned = workUnitModel.WorkUnit.Assigned.Normalize();
        workUnit.Finished = workUnitModel.WorkUnit.Finished.Normalize();
        workUnit.ProjectRun = workUnitModel.WorkUnit.ProjectRun;
        workUnit.ProjectClone = workUnitModel.WorkUnit.ProjectClone;
        workUnit.ProjectGen = workUnitModel.WorkUnit.ProjectGen;
        workUnit.FramesCompleted = workUnitModel.FramesComplete;
        workUnit.FrameTimeInSeconds = workUnitModel.GetRawTime(PPDCalculation.AllFrames);
        workUnit.ProteinID = proteinID;
        workUnit.ClientID = clientID;
        workUnit.ClientSlot = workUnitModel.SlotModel.SlotID;

        context.WorkUnits.Add(workUnit);
        context.SaveChanges();

        return workUnit;
    }

    private static void InsertWorkUnitFrameEntities(WorkUnitContext context, WorkUnitModel workUnitModel, long workUnitID)
    {
        if (workUnitModel.WorkUnit.Frames is null)
        {
            return;
        }

        foreach (var f in workUnitModel.WorkUnit.Frames.Values)
        {
            var frame = new WorkUnitFrameEntity();
            frame.WorkUnitID = workUnitID;
            frame.FrameID = f.ID;
            frame.RawFramesComplete = f.RawFramesComplete;
            frame.RawFramesTotal = f.RawFramesTotal;
            frame.TimeStamp = f.TimeStamp;
            frame.Duration = f.Duration;
            context.WorkUnitFrames.Add(frame);
        }

        context.SaveChanges();
    }

    public int Delete(WorkUnitRow row)
    {
        using var context = CreateWorkUnitContext();
        context.WorkUnits.Remove(new WorkUnitEntity { ID = row.ID });
        return context.SaveChanges();
    }

    public IList<WorkUnitRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return FetchInternal(query, bonusCalculation);
        }
        finally
        {
            Logger.Debug($"Database Fetch ({query}) completed in {sw.GetExecTime()}");
        }
    }

    private IList<WorkUnitRow> FetchInternal(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        using var context = CreateWorkUnitContext();
        IQueryable<WorkUnitEntity> q = WorkUnitQuery(context, bonusCalculation);

        foreach (var p in query.Parameters)
        {
            q = q.Where(p);
        }

        return _mapper.Map<IList<WorkUnitRow>>(q.ToList());
    }

    public Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return PageInternal(page, itemsPerPage, query, bonusCalculation);
        }
        finally
        {
            Logger.Debug($"Database Page Fetch ({query}) completed in {sw.GetExecTime()}");
        }
    }

    private Page<WorkUnitRow> PageInternal(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        using var context = CreateWorkUnitContext();
        IQueryable<WorkUnitEntity> q = WorkUnitQuery(context, bonusCalculation);

        foreach (var p in query.Parameters)
        {
            q = q.Where(p);
        }

        long count = q.LongCount();

        q = q
            .Skip((int)((page - 1) * itemsPerPage))
            .Take((int)itemsPerPage);

        long totalPages = count / itemsPerPage;
        if (count % itemsPerPage != 0)
        {
            totalPages++;
        }

        var items = _mapper.Map<IList<WorkUnitRow>>(q.ToList());
        return new Page<WorkUnitRow>
        {
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = count,
            ItemsPerPage = itemsPerPage,
            Items = items
        };
    }

    private static IQueryable<WorkUnitEntity> WorkUnitQuery(WorkUnitContext context, BonusCalculation bonusCalculation) =>
        context.WorkUnits
            .Include(x => x.Client)
            .Include(x => x.Protein)
            .Include(x => x.Frames)
            .Select(x => new WorkUnitEntity
            {
                ID = x.ID,
                DonorName = x.DonorName,
                DonorTeam = x.DonorTeam,
                CoreVersion = x.CoreVersion,
                Result = x.Result,
                Assigned = x.Assigned,
                Finished = x.Finished,
                ProjectRun = x.ProjectRun,
                ProjectClone = x.ProjectClone,
                ProjectGen = x.ProjectGen,
                HexID = x.HexID,
                FramesCompleted = x.FramesCompleted,
                FrameTimeInSeconds = x.FrameTimeInSeconds,
                ProteinID = x.ProteinID,
                ClientID = x.ClientID,
                ClientSlot = x.ClientSlot,
                Protein = x.Protein,
                Client = x.Client,
                Frames = x.Frames,
                SlotName = WorkUnitContext.ToSlotName(x.Client.Name, x.ClientSlot),
                SlotType = WorkUnitContext.ToSlotType(x.Protein.Core),
                PPD = WorkUnitContext.CalculatePPD(
                    x.FrameTimeInSeconds.GetValueOrDefault(),
                    x.Protein.Frames,
                    x.Protein.Credit,
                    x.Protein.KFactor,
                    x.Protein.TimeoutDays,
                    x.Protein.ExpirationDays,
                    x.Assigned.ToString(),
                    x.Finished.ToString(),
                    (int)bonusCalculation),
                Credit = WorkUnitContext.CalculateCredit(
                    x.FrameTimeInSeconds.GetValueOrDefault(),
                    x.Protein.Frames,
                    x.Protein.Credit,
                    x.Protein.KFactor,
                    x.Protein.TimeoutDays,
                    x.Protein.ExpirationDays,
                    x.Assigned.ToString(),
                    x.Finished.ToString(),
                    (int)bonusCalculation)
            });

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
        int? clientSlot = slotID == SlotIdentifier.NoSlotID ? null : slotID;
        return query.Where(x => x.ClientSlot == clientSlot);
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
