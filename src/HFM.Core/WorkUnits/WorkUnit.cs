using System.Diagnostics;

using HFM.Log;

namespace HFM.Core.WorkUnits;

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
            UnitResult = UnitResult,
            FramesObserved = FramesObserved,
            LogLines = LogLines,
            CoreID = CoreID,
            ID = ID
        };

    /// <summary>
    /// Local time the logs used to generate this WorkUnit were retrieved
    /// </summary>
    public DateTime UnitRetrievalTime { get; set; }

    /// <summary>
    /// The Folding ID (Username) attached to this work unit
    /// </summary>
    public string FoldingID { get; set; }

    /// <summary>
    /// The Team number attached to this work unit
    /// </summary>
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
    /// Unit Start Time Stamp
    /// </summary>
    public TimeSpan UnitStartTimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the work unit finished date and time.
    /// </summary>
    public DateTime Finished { get; set; }

    /// <summary>
    /// Core Version Number
    /// </summary>
    public Version CoreVersion { get; set; }

    /// <summary>
    /// Project ID Number
    /// </summary>
    public int ProjectID { get; set; }

    /// <summary>
    /// Project ID (Run)
    /// </summary>
    public int ProjectRun { get; set; }

    /// <summary>
    /// Project ID (Clone)
    /// </summary>
    public int ProjectClone { get; set; }

    /// <summary>
    /// Project ID (Gen)
    /// </summary>
    public int ProjectGen { get; set; }

    /// <summary>
    /// The Result of this Work Unit
    /// </summary>
    public WorkUnitResult UnitResult { get; set; }

    /// <summary>
    /// Gets or sets the number of frames observed since the unit was last started.
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
    /// Core ID (Hex) Value
    /// </summary>
    public string CoreID { get; set; }

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
