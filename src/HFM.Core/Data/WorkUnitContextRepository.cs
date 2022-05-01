using System.Diagnostics;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public class Page<T>
{
    public long CurrentPage { get; set; }

    public long TotalPages { get; set; }

    public long TotalItems { get; set; }

    public long ItemsPerPage { get; set; }

    public IList<T> Items { get; set; }
}

public interface IWorkUnitRepository
{
    long Update(WorkUnitModel workUnitModel);

    int Delete(WorkUnitEntityRow row);

    IList<WorkUnitEntityRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation);

    Page<WorkUnitEntityRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation);

    long CountCompleted(string clientName, DateTime? clientStartTime);

    long CountFailed(string clientName, DateTime? clientStartTime);
}

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

    public long Update(WorkUnitModel workUnitModel)
    {
        if (!ValidateWorkUnit(workUnitModel.WorkUnit))
        {
            return -1;
        }

        using var context = CreateWorkUnitContext();

        var workUnit = GetExistingWorkUnit(context, workUnitModel.WorkUnit);
        var client = GetOrInsertClientEntity(context, workUnitModel);
        var protein = GetOrInsertProteinEntity(context, workUnitModel);
        var platform = GetOrInsertPlatformEntity(context, workUnitModel);
        workUnit = UpsertWorkUnitEntity(context, workUnitModel, workUnit, client.ID, protein.ID, platform?.ID);
        UpsertWorkUnitFrameEntities(context, workUnitModel, workUnit);

        return workUnit.ID;
    }

    private static bool ValidateWorkUnit(WorkUnit workUnit) =>
        workUnit.HasProject() &&
        !workUnit.Assigned.IsMinValue();

    private static WorkUnitEntity GetExistingWorkUnit(WorkUnitContext context, WorkUnit workUnit) =>
        context.WorkUnits
            .Include(x => x.Protein)
            .Include(x => x.Frames)
            .FirstOrDefault(x => x.Protein.ProjectID == workUnit.ProjectID &&
                                 x.ProjectRun == workUnit.ProjectRun &&
                                 x.ProjectClone == workUnit.ProjectClone &&
                                 x.ProjectGen == workUnit.ProjectGen &&
                                 x.Assigned == workUnit.Assigned.Normalize());

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
            protein = new ProteinEntity
            {
                ProjectID = workUnitModel.CurrentProtein.ProjectNumber,
                Credit = workUnitModel.CurrentProtein.Credit,
                KFactor = workUnitModel.CurrentProtein.KFactor,
                Frames = workUnitModel.CurrentProtein.Frames,
                Core = workUnitModel.CurrentProtein.Core,
                Atoms = workUnitModel.CurrentProtein.NumberOfAtoms,
                TimeoutDays = workUnitModel.CurrentProtein.PreferredDays,
                ExpirationDays = workUnitModel.CurrentProtein.MaximumDays
            };

            context.Proteins.Add(protein);
            context.SaveChanges();
        }

        return protein;
    }

    private static PlatformEntity GetOrInsertPlatformEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        var p = CreatePlatformEntity(workUnitModel);
        if (!ValidatePlatformEntity(p))
        {
            return null;
        }

        var platform = context.Platforms
            .FirstOrDefault(x => x.ClientVersion == p.ClientVersion &&
                                 x.OperatingSystem == p.OperatingSystem &&
                                 x.Implementation == p.Implementation &&
                                 x.Processor == p.Processor &&
                                 x.Threads == p.Threads &&
                                 x.DriverVersion == p.DriverVersion &&
                                 x.ComputeVersion == p.ComputeVersion &&
                                 x.CUDAVersion == p.CUDAVersion);

        if (platform is null)
        {
            platform = p;
            context.Platforms.Add(platform);
            context.SaveChanges();
        }

        return platform;
    }

    private static PlatformEntity CreatePlatformEntity(WorkUnitModel workUnitModel)
    {
        var clientPlatform = workUnitModel.SlotModel.Client.Platform;
        var clientVersion = clientPlatform?.ClientVersion;
        var operatingSystem = clientPlatform?.OperatingSystem;

        var workUnitPlatform = workUnitModel.WorkUnit.Platform;
        var implementation = workUnitPlatform?.Implementation;
        var processor = workUnitModel.BenchmarkIdentifier.Processor;
        int? threads = workUnitModel.BenchmarkIdentifier.HasThreads
            ? workUnitModel.BenchmarkIdentifier.Threads
            : null;
        var driverVersion = workUnitPlatform?.DriverVersion;
        var computeVersion = workUnitPlatform?.ComputeVersion;
        var cudaVersion = workUnitPlatform?.CUDAVersion;

        return new PlatformEntity
        {
            ClientVersion = clientVersion,
            OperatingSystem = operatingSystem,
            Implementation = implementation,
            Processor = processor,
            Threads = threads,
            DriverVersion = driverVersion,
            ComputeVersion = computeVersion,
            CUDAVersion = cudaVersion
        };
    }

    private static bool ValidatePlatformEntity(PlatformEntity platform) =>
        !String.IsNullOrEmpty(platform.ClientVersion) &&
        !String.IsNullOrEmpty(platform.OperatingSystem) &&
        !String.IsNullOrEmpty(platform.Implementation) &&
        !String.IsNullOrEmpty(platform.Processor);

    private static WorkUnitEntity UpsertWorkUnitEntity(WorkUnitContext context, WorkUnitModel workUnitModel,
        WorkUnitEntity workUnit, long clientID, long proteinID, long? platformID)
    {
        bool insert = workUnit is null;

        workUnit ??= new WorkUnitEntity();
        workUnit.DonorName = workUnitModel.WorkUnit.FoldingID;
        workUnit.DonorTeam = workUnitModel.WorkUnit.Team;
        workUnit.CoreVersion = workUnitModel.WorkUnit.CoreVersion?.ToString();
        workUnit.Result = workUnitModel.WorkUnit.UnitResult == WorkUnitResult.Unknown
            ? null
            : WorkUnitResultString.FromWorkUnitResult(workUnitModel.WorkUnit.UnitResult);
        workUnit.Assigned = workUnitModel.WorkUnit.Assigned.Normalize();
        workUnit.Finished = workUnitModel.WorkUnit.Finished.IsMinValue()
            ? null
            : workUnitModel.WorkUnit.Finished.Normalize();
        workUnit.ProjectRun = workUnitModel.WorkUnit.ProjectRun;
        workUnit.ProjectClone = workUnitModel.WorkUnit.ProjectClone;
        workUnit.ProjectGen = workUnitModel.WorkUnit.ProjectGen;
        workUnit.FramesCompleted = workUnitModel.FramesComplete;
        workUnit.FrameTimeInSeconds = workUnitModel.GetRawTime(PPDCalculation.AllFrames);
        workUnit.ProteinID = proteinID;
        workUnit.ClientID = clientID;
        workUnit.PlatformID = platformID;
        workUnit.ClientSlot = workUnitModel.SlotModel.SlotID == SlotIdentifier.NoSlotID
            ? null
            : workUnitModel.SlotModel.SlotID;

        if (insert)
        {
            context.WorkUnits.Add(workUnit);
        }
        context.SaveChanges();

        return workUnit;
    }

    private static void UpsertWorkUnitFrameEntities(WorkUnitContext context, WorkUnitModel workUnitModel, WorkUnitEntity workUnit)
    {
        if (workUnitModel.WorkUnit.Frames is null)
        {
            return;
        }

        foreach (var f in workUnitModel.WorkUnit.Frames.Values)
        {
            var frame = workUnit.Frames?.FirstOrDefault(x => x.FrameID == f.ID);
            bool insert = frame is null;
            frame ??= new WorkUnitFrameEntity();
            frame.WorkUnitID = workUnit.ID;
            frame.FrameID = f.ID;
            frame.RawFramesComplete = f.RawFramesComplete;
            frame.RawFramesTotal = f.RawFramesTotal;
            frame.TimeStamp = f.TimeStamp;
            frame.Duration = f.Duration;
            if (insert)
            {
                context.WorkUnitFrames.Add(frame);
            }
        }

        context.SaveChanges();
    }

    public int Delete(WorkUnitEntityRow row)
    {
        using var context = CreateWorkUnitContext();
        var workUnit = context.WorkUnits.Find(row.ID);
        if (workUnit is null)
        {
            return 0;
        }
        context.WorkUnits.Remove(workUnit);
        return context.SaveChanges();
    }

    public IList<WorkUnitEntityRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation)
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

    private IList<WorkUnitEntityRow> FetchInternal(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        using var context = CreateWorkUnitContext();
        IQueryable<WorkUnitEntity> q = WorkUnitQuery(context, bonusCalculation);

        foreach (var p in query.Parameters)
        {
            q = q.Where(p);
        }

        return _mapper.Map<IList<WorkUnitEntityRow>>(q.ToList());
    }

    public Page<WorkUnitEntityRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
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

    private Page<WorkUnitEntityRow> PageInternal(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
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

        var items = _mapper.Map<IList<WorkUnitEntityRow>>(q.ToList());
        return new Page<WorkUnitEntityRow>
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
            .AsNoTracking()
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
                // ReSharper disable SpecifyACultureInStringConversionExplicitly
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
                // ReSharper restore SpecifyACultureInStringConversionExplicitly
            });

    public long CountCompleted(string clientName, DateTime? clientStartTime)
    {
        var slotIdentifier = SlotIdentifier.FromName(clientName, String.Empty, Guid.Empty);

        using var context = CreateWorkUnitContext();
        var query = QueryWorkUnitsByClientName(context, slotIdentifier.ClientIdentifier.Name);
        query = WhereResultIsFinishedUnit(query);
        query = WhereClientSlot(query, slotIdentifier.SlotID);
        query = WhereFinishedAfterClientStart(query, clientStartTime);
        return query.Count();
    }

    public long CountFailed(string clientName, DateTime? clientStartTime)
    {
        var slotIdentifier = SlotIdentifier.FromName(clientName, String.Empty, Guid.Empty);

        using var context = CreateWorkUnitContext();
        var query = QueryWorkUnitsByClientName(context, slotIdentifier.ClientIdentifier.Name);
        query = WhereResultIsNotFinishedUnit(query);
        query = WhereClientSlot(query, slotIdentifier.SlotID);
        query = WhereFinishedAfterClientStart(query, clientStartTime);
        return query.Count();
    }

    private static IQueryable<WorkUnitEntity> QueryWorkUnitsByClientName(WorkUnitContext context, string clientName) =>
        context.WorkUnits
            .AsNoTracking()
            .Include(x => x.Client)
            .Where(x => x.Client.Name == clientName);

    private static IQueryable<WorkUnitEntity> WhereResultIsFinishedUnit(IQueryable<WorkUnitEntity> query) =>
        query.Where(x => x.Result == WorkUnitResultString.FinishedUnit);

    private static IQueryable<WorkUnitEntity> WhereResultIsNotFinishedUnit(IQueryable<WorkUnitEntity> query) =>
        query.Where(x => x.Result != null && x.Result != WorkUnitResultString.FinishedUnit);

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
