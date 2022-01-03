using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;

namespace HFM.Core.Data
{
    public class MigrateToWorkUnitContext
    {
        private readonly ILogger _logger;
        private readonly WorkUnitContext _context;
        private readonly IWorkUnitRepository _repository;

        public MigrateToWorkUnitContext(ILogger logger, WorkUnitContext context, IWorkUnitRepository repository)
        {
            _logger = logger ?? NullLogger.Instance;
            _context = context;
            _repository = repository;
        }

        public async Task ExecuteAsync(IProgress<ProgressInfo> progress)
        {
            await _context.Database.EnsureCreatedAsync().ConfigureAwait(false);

            progress.Report(new ProgressInfo(0, "Fetching existing work unit data..."));
            var rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);

            int count = 0;
            double total = rows.Count * 2;

            var proteins = new HashSet<ProteinEntity>();
            var clients = new HashSet<ClientEntity>();
            foreach (var r in rows)
            {
                int progressPercentage = Convert.ToInt32(++count / total * 100);
                progress.Report(new ProgressInfo(progressPercentage, $"Processing record {count} of {total}"));

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

                var slotIdentifier = SlotIdentifier.FromName(r.Name, r.Path, Guid.Empty);

                var c = new ClientEntity
                {
                    Name = slotIdentifier.ClientIdentifier.Name,
                    ConnectionString = r.Path
                };
                clients.Add(c);
            }

            _context.Proteins.AddRange(proteins);
            _context.Clients.AddRange(clients);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            var workUnits = new HashSet<WorkUnitEntity>();
            foreach (var r in rows)
            {
                int progressPercentage = Convert.ToInt32(++count / total * 100);
                progress.Report(new ProgressInfo(progressPercentage, $"Processing record {count} of {total}"));

                var slotIdentifier = SlotIdentifier.FromName(r.Name, r.Path, Guid.Empty);

                var p = await _context.Proteins.FirstAsync(x =>
                    x.ProjectID == r.ProjectID &&
                    Math.Abs(x.Credit - r.BaseCredit) < 0.001 &&
                    Math.Abs(x.KFactor - r.KFactor) < 0.001 &&
                    x.Frames == r.Frames &&
                    x.Core == r.Core &&
                    x.Atoms == r.Atoms &&
                    Math.Abs(x.TimeoutDays - r.PreferredDays) < 0.001 &&
                    Math.Abs(x.ExpirationDays - r.MaximumDays) < 0.001)
                    .ConfigureAwait(false);

                var c = await _context.Clients.FirstAsync(x =>
                    x.Name == slotIdentifier.ClientIdentifier.Name &&
                    x.ConnectionString == r.Path)
                    .ConfigureAwait(false);

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
                    ClientSlot = slotIdentifier.SlotID >= 0 ? slotIdentifier.SlotID : null
                };
                workUnits.Add(w);
            }

            _context.WorkUnits.AddRange(workUnits);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
