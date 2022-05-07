using HFM.Core.WorkUnits;

namespace HFM.Core.Data;

/// <summary>
/// Represents the columns returned by a work unit history database query.
/// </summary>
public enum WorkUnitRowColumn
{
    ID = -1,
    ProjectID = 0,
    ProjectRun = 1,
    ProjectClone = 2,
    ProjectGen = 3,
    SlotName = 4,
    ConnectionString = 5,
    DonorName = 6,
    DonorTeam = 7,
    CoreVersion = 8,
    FramesCompleted = 9,
    FrameTime = 10,
    Result = 11,
    Assigned = 12,
    Finished = 13,
    //WorkUnitName = 14,
    KFactor = 15,
    Core = 16,
    Frames = 17,
    Atoms = 18,
    SlotType = 19,
    PPD = 20,
    Credit = 21,
    BaseCredit = 22,
    ClientVersion = 23,
    OperatingSystem = 24,
    PlatformImplementation = 25,
    PlatformProcessor = 26,
    PlatformThreads = 27,
    DriverVersion = 28,
    ComputeVersion = 29,
    CUDAVersion = 30
}

public class WorkUnitRow : IProjectInfo
{
    public long ID { get; set; }
    public int ProjectID { get; set; }
    public int ProjectRun { get; set; }
    public int ProjectClone { get; set; }
    public int ProjectGen { get; set; }
    public string SlotName { get; set; }
    public string ConnectionString { get; set; }
    public string DonorName { get; set; }
    public int DonorTeam { get; set; }
    public string CoreVersion { get; set; }
    public int FramesCompleted { get; set; }
    public TimeSpan FrameTime => TimeSpan.FromSeconds(FrameTimeInSeconds);
    public int FrameTimeInSeconds { get; set; }
    public string Result { get; set; }
    public DateTime Assigned { get; set; }
    public DateTime Finished { get; set; }
    //public string WorkUnitName { get; set; }
    public double KFactor { get; set; }
    public string Core { get; set; }
    public int Frames { get; set; }
    public int Atoms { get; set; }
    public string SlotType { get; set; }
    public double PPD { get; set; }
    public double Credit { get; set; }
    public double BaseCredit { get; set; }
    public string ClientVersion { get; set; }
    public string OperatingSystem { get; set; }
    public string PlatformImplementation { get; set; }
    public string PlatformProcessor { get; set; }
    public int? PlatformThreads { get; set; }
    public string DriverVersion { get; set; }
    public string ComputeVersion { get; set; }
    public string CUDAVersion { get; set; }
}
