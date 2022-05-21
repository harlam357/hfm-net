using System.Diagnostics;

using HFM.Log;

namespace HFM.Core.WorkUnits;

public static class WorkUnitPlatformImplementation
{
    public const string CPU = nameof(CPU);
    public const string CUDA = nameof(CUDA);
    public const string OpenCL = nameof(OpenCL);
}

public record WorkUnitPlatform(string Implementation)
{
    public string DriverVersion { get; init; }

    public string ComputeVersion { get; init; }

    public string CUDAVersion { get; init; }
}

public class WorkUnit : IQueueItem, IProjectInfo
{
    /// <summary>
    /// Returns a shallow copy of this <see cref="WorkUnit"/>.
    /// </summary>
    public WorkUnit Copy() =>
        new()
        {
            UnitRetrievalTime = UnitRetrievalTime,
            FoldingID = FoldingID,
            Team = Team,
            Assigned = Assigned,
            Timeout = Timeout,
            UnitStartTimeStamp = UnitStartTimeStamp,
            Finished = Finished,
            CoreVersion = CoreVersion,
            ProjectID = ProjectID,
            ProjectRun = ProjectRun,
            ProjectClone = ProjectClone,
            ProjectGen = ProjectGen,
            Platform = Platform,
            UnitResult = UnitResult,
            FramesObserved = FramesObserved,
            LogLines = LogLines,
            CoreID = CoreID,
            UnitID = UnitID,
            ID = ID
        };

    /// <summary>
    /// Gets or sets the local time this work unit record was generated.
    /// </summary>
    public DateTime UnitRetrievalTime { get; set; }

    // TODO: Rename to DonorName
    public string FoldingID { get; set; }

    // TODO: Rename to DonorTeam
    public int Team { get; set; }

    /// <summary>
    /// Gets or sets the work unit assigned date and time.
    /// </summary>
    public DateTime Assigned { get; set; }

    /// <summary>
    /// Gets or sets the work unit timeout date and time.
    /// </summary>
    public DateTime Timeout { get; set; }

    /// <summary>
    /// Gets or sets the time stamp of the start of the work unit.
    /// </summary>
    public TimeSpan UnitStartTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the work unit finished date and time.
    /// </summary>
    public DateTime Finished { get; set; }

    public Version CoreVersion { get; set; }

    public int ProjectID { get; set; }

    public int ProjectRun { get; set; }

    public int ProjectClone { get; set; }

    public int ProjectGen { get; set; }

    public WorkUnitPlatform Platform { get; set; }

    public WorkUnitResult UnitResult { get; set; }

    /// <summary>
    /// Gets or sets the number of frames observed (completed) since last unit start or resume from pause.
    /// </summary>
    public int FramesObserved { get; set; }

    /// <summary>
    /// Gets the last observed frame of this work unit.
    /// </summary>
    public LogLineFrameData CurrentFrame
    {
        get
        {
            if (Frames == null || Frames.Count == 0)
            {
                return null;
            }

            int max = Frames.Keys.Max();
            if (max >= 0)
            {
                Debug.Assert(Frames[max].ID == max);
                return Frames[max];
            }

            return null;
        }
    }

    private ICollection<LogLine> _logLines;

    public ICollection<LogLine> LogLines
    {
        get => _logLines;
        set
        {
            if (value == null)
            {
                return;
            }
            _logLines = value;
        }
    }

    /// <summary>
    /// Gets or sets the dictionary of frame data parsed from log lines.
    /// </summary>
    public IDictionary<int, LogLineFrameData> Frames { get; set; }

    /// <summary>
    /// Gets or sets the core hex identifier.
    /// </summary>
    public string CoreID { get; set; }

    /// <summary>
    /// Gets or sets the work unit hex identifier.
    /// </summary>
    public string UnitID { get; set; }

    public int ID { get; set; } = WorkUnitCollection.NoID;

    /// <summary>
    /// Gets the frame by frame ID.
    /// </summary>
    public LogLineFrameData GetFrame(int frameId) =>
        Frames != null && Frames.ContainsKey(frameId) ? Frames[frameId] : null;

    internal bool EqualsProjectAndDownloadTime(WorkUnit other) =>
        other != null && this.HasProject() && other.HasProject() && this.EqualsProject(other) && Assigned.Equals(other.Assigned);
}

public class WorkUnitCollection : QueueItemCollection<WorkUnit>
{
    public WorkUnitCollection()
    {

    }

    public WorkUnitCollection(IEnumerable<WorkUnit> workUnits)
    {
        foreach (var workUnit in workUnits)
        {
            Add(workUnit);
        }
    }

    public bool HasWorkUnit(WorkUnit workUnit) => this.Any(x => x.EqualsProjectAndDownloadTime(workUnit));
}
