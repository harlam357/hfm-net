using System.Diagnostics;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public class ScopedWorkUnitContextRepositoryProxy : IWorkUnitRepository
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopedWorkUnitContextRepositoryProxy(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<long> UpdateAsync(WorkUnitModel workUnitModel)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.UpdateAsync(workUnitModel).ConfigureAwait(false);
        }
    }

    public async Task<int> DeleteAsync(WorkUnitRow row)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.DeleteAsync(row).ConfigureAwait(false);
        }
    }

    public async Task<IList<WorkUnitRow>> FetchAsync(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.FetchAsync(query, bonusCalculation).ConfigureAwait(false);
        }
    }

    public async Task<Page<WorkUnitRow>> PageAsync(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.PageAsync(page, itemsPerPage, query, bonusCalculation).ConfigureAwait(false);
        }
    }

    public async Task<long> CountCompletedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.CountCompletedAsync(slotIdentifier, clientStartTime).ConfigureAwait(false);
        }
    }

    public async Task<long> CountFailedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(_logger, context);
            return await repository.CountFailedAsync(slotIdentifier, clientStartTime).ConfigureAwait(false);
        }
    }
}

public class WorkUnitContextRepository : IWorkUnitRepository
{
    public ILogger Logger { get; }

    private readonly WorkUnitContext _context;

    public WorkUnitContextRepository(ILogger logger, WorkUnitContext context)
    {
        Logger = logger ?? NullLogger.Instance;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<WorkUnitRowProfile>()).CreateMapper();
    }

    private readonly IMapper _mapper;

    public async Task<long> UpdateAsync(WorkUnitModel workUnitModel)
    {
        if (!ValidateWorkUnit(workUnitModel.WorkUnit))
        {
            return -1;
        }

        var workUnit = await GetExistingWorkUnit(_context, workUnitModel.WorkUnit).ConfigureAwait(false);
        var client = await GetOrInsertClientEntity(_context, workUnitModel).ConfigureAwait(false);
        var protein = await GetOrInsertProteinEntity(_context, workUnitModel).ConfigureAwait(false);
        var platform = await GetOrInsertPlatformEntity(_context, workUnitModel).ConfigureAwait(false);
        workUnit = await UpsertWorkUnitEntity(_context, workUnitModel, workUnit, client.ID, protein.ID, platform?.ID).ConfigureAwait(false);
        await UpsertWorkUnitFrameEntities(_context, workUnitModel, workUnit).ConfigureAwait(false);

        return workUnit.ID;
    }

    private static bool ValidateWorkUnit(WorkUnit workUnit) =>
        workUnit.HasProject() &&
        !workUnit.Assigned.IsMinValue();

    private static async Task<WorkUnitEntity> GetExistingWorkUnit(WorkUnitContext context, WorkUnit workUnit) =>
        await context.WorkUnits
            .Include(x => x.Protein)
            .Include(x => x.Frames)
            .FirstOrDefaultAsync(x =>
                x.Protein.ProjectID == workUnit.ProjectID &&
                x.ProjectRun == workUnit.ProjectRun &&
                x.ProjectClone == workUnit.ProjectClone &&
                x.ProjectGen == workUnit.ProjectGen &&
                x.Assigned == workUnit.Assigned.Normalize()).ConfigureAwait(false);

    private static async Task<ClientEntity> GetOrInsertClientEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        ClientEntity client = null;

        var identifier = workUnitModel.SlotModel.SlotIdentifier.ClientIdentifier;
        string connectionString = identifier.ToConnectionString();
        string guid = identifier.HasGuid ? identifier.Guid.ToString() : null;

        if (guid is not null)
        {
            client = await context.Clients.OrderByDescending(x => x.ID).FirstOrDefaultAsync(x => x.Guid == guid).ConfigureAwait(false);
            if (client is not null && (client.Name != identifier.Name || client.ConnectionString != connectionString))
            {
                client = new ClientEntity
                {
                    Name = identifier.Name,
                    ConnectionString = connectionString,
                    Guid = guid
                };
                context.Clients.Add(client);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        if (client is null)
        {
            client = await context.Clients.FirstOrDefaultAsync(x => x.Name == identifier.Name && x.ConnectionString == connectionString).ConfigureAwait(false);
            if (client is not null && client.Guid is null && guid is not null)
            {
                client.Guid = guid;
                await context.SaveChangesAsync().ConfigureAwait(false);
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
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        return client;
    }

    private static async Task<ProteinEntity> GetOrInsertProteinEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        var p = workUnitModel.CurrentProtein;
        var protein = await context.Proteins
            .FirstOrDefaultAsync(x =>
                x.ProjectID == p.ProjectNumber &&
                Math.Abs(x.Credit - p.Credit) < 0.001 &&
                Math.Abs(x.KFactor - p.KFactor) < 0.001 &&
                x.Frames == p.Frames &&
                x.Core == p.Core &&
                x.Atoms == p.NumberOfAtoms &&
                Math.Abs(x.TimeoutDays - p.PreferredDays) < 0.001 &&
                Math.Abs(x.ExpirationDays - p.MaximumDays) < 0.001).ConfigureAwait(false);

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
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        return protein;
    }

    private static async Task<PlatformEntity> GetOrInsertPlatformEntity(WorkUnitContext context, WorkUnitModel workUnitModel)
    {
        var p = CreatePlatformEntity(workUnitModel);
        if (!ValidatePlatformEntity(p))
        {
            return null;
        }

        var platform = await context.Platforms
            .FirstOrDefaultAsync(x =>
                x.ClientVersion == p.ClientVersion &&
                x.OperatingSystem == p.OperatingSystem &&
                x.Implementation == p.Implementation &&
                x.Processor == p.Processor &&
                x.Threads == p.Threads &&
                x.DriverVersion == p.DriverVersion &&
                x.ComputeVersion == p.ComputeVersion &&
                x.CUDAVersion == p.CUDAVersion).ConfigureAwait(false);

        if (platform is null)
        {
            platform = p;
            context.Platforms.Add(platform);
            await context.SaveChangesAsync().ConfigureAwait(false);
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

    private static async Task<WorkUnitEntity> UpsertWorkUnitEntity(WorkUnitContext context, WorkUnitModel workUnitModel,
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
        workUnit.HexID = workUnitModel.WorkUnit.UnitID;
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
        await context.SaveChangesAsync().ConfigureAwait(false);

        return workUnit;
    }

    private static async Task UpsertWorkUnitFrameEntities(WorkUnitContext context, WorkUnitModel workUnitModel, WorkUnitEntity workUnit)
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

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<int> DeleteAsync(WorkUnitRow row)
    {
        var workUnit = await _context.WorkUnits.FindAsync(row.ID).ConfigureAwait(false);
        if (workUnit is null)
        {
            return 0;
        }
        _context.WorkUnits.Remove(workUnit);
        return await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<IList<WorkUnitRow>> FetchAsync(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await FetchInternal(query, bonusCalculation).ConfigureAwait(false);
        }
        finally
        {
            Logger.Debug($"Database Fetch ({query}) completed in {sw.GetExecTime()}");
        }
    }

    private async Task<IList<WorkUnitRow>> FetchInternal(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        IQueryable<WorkUnitEntity> q = WorkUnitQuery(_context, bonusCalculation);

        foreach (var p in query.Parameters)
        {
            q = q.Where(p);
        }

        var workUnits = await q.ToListAsync().ConfigureAwait(false);
        return _mapper.Map<IList<WorkUnitRow>>(workUnits);
    }

    public async Task<Page<WorkUnitRow>> PageAsync(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await PageInternal(page, itemsPerPage, query, bonusCalculation).ConfigureAwait(false);
        }
        finally
        {
            Logger.Debug($"Database Page Fetch ({query}) completed in {sw.GetExecTime()}");
        }
    }

    private async Task<Page<WorkUnitRow>> PageInternal(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        IQueryable<WorkUnitEntity> q = WorkUnitQuery(_context, bonusCalculation);

        foreach (var p in query.Parameters)
        {
            q = q.Where(p);
        }

        long count = await q.LongCountAsync().ConfigureAwait(false);

        q = q
            .Skip((int)((page - 1) * itemsPerPage))
            .Take((int)itemsPerPage);

        long totalPages = count / itemsPerPage;
        if (count % itemsPerPage != 0)
        {
            totalPages++;
        }

        var workUnits = await q.ToListAsync().ConfigureAwait(false);
        var items = _mapper.Map<IList<WorkUnitRow>>(workUnits);
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
            .AsNoTracking()
            .Include(x => x.Client)
            .Include(x => x.Protein)
            .Include(x => x.Platform)
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
                PlatformID = x.PlatformID,
                ClientSlot = x.ClientSlot,
                Protein = x.Protein,
                Client = x.Client,
                Platform = x.Platform,
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

    public async Task<long> CountCompletedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime)
    {
        var query = QueryWorkUnitsByClient(_context, slotIdentifier.ClientIdentifier);
        query = WhereResultIsFinishedUnit(query);
        query = WhereClientSlot(query, slotIdentifier.SlotID);
        query = WhereFinishedAfterClientStart(query, clientStartTime);
        return await query.LongCountAsync().ConfigureAwait(false);
    }

    public async Task<long> CountFailedAsync(SlotIdentifier slotIdentifier, DateTime? clientStartTime)
    {
        var query = QueryWorkUnitsByClient(_context, slotIdentifier.ClientIdentifier);
        query = WhereResultIsNotFinishedUnit(query);
        query = WhereClientSlot(query, slotIdentifier.SlotID);
        query = WhereFinishedAfterClientStart(query, clientStartTime);
        return await query.LongCountAsync().ConfigureAwait(false);
    }

    private static IQueryable<WorkUnitEntity> QueryWorkUnitsByClient(WorkUnitContext context, ClientIdentifier client) =>
        context.WorkUnits
            .AsNoTracking()
            .Include(x => x.Client)
            .Where(x => client.HasGuid ? x.Client.Guid == client.Guid.ToString() : x.Client.Name == client.Name);

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
}
