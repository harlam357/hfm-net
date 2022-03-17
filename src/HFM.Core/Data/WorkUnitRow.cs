using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    /// <summary>
    /// Represents the columns returned by a work unit history database query.
    /// </summary>
    public enum WorkUnitRowColumn
    {
        ID = -1,
        ProjectID = 0,
        ProjectRun,
        ProjectClone,
        ProjectGen,
        Name,
        Path,
        Username,
        Team,
        CoreVersion,
        FramesCompleted,
        FrameTime,
        Result,
        Assigned,
        Finished,
        WorkUnitName,
        KFactor,
        Core,
        Frames,
        Atoms,
        SlotType,
        PPD,
        Credit,
        BaseCredit
    }

    public abstract class WorkUnitRow : IProjectInfo
    {
        public long ID { get; set; }

        public int ProjectID { get; set; }

        public int ProjectRun { get; set; }

        public int ProjectClone { get; set; }

        public int ProjectGen { get; set; }

        public string Username { get; set; }

        public int Team { get; set; }

        public int FramesCompleted { get; set; }

        public string WorkUnitName { get; set; }

        public double KFactor { get; set; }

        public string Core { get; set; }

        public int Frames { get; set; }

        public int Atoms { get; set; }

        public double PreferredDays { get; set; }

        public double MaximumDays { get; set; }
    }

    public class PetaPocoWorkUnitRow : WorkUnitRow
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public float CoreVersion { get; set; }

        public TimeSpan FrameTime => TimeSpan.FromSeconds(FrameTimeValue);

        public int FrameTimeValue { get; set; }

        public string Result => ToWorkUnitResultString(ResultValue);

        private static string ToWorkUnitResultString(int result)
        {
            switch ((WorkUnitResult)result)
            {
                case WorkUnitResult.FinishedUnit:
                    return WorkUnitResultString.FinishedUnit;
                case WorkUnitResult.EarlyUnitEnd:
                    return WorkUnitResultString.EarlyUnitEnd;
                case WorkUnitResult.UnstableMachine:
                    return WorkUnitResultString.UnstableMachine;
                case WorkUnitResult.Interrupted:
                    return WorkUnitResultString.Interrupted;
                case WorkUnitResult.BadWorkUnit:
                    return WorkUnitResultString.BadWorkUnit;
                case WorkUnitResult.CoreOutdated:
                    return WorkUnitResultString.CoreOutdated;
                case WorkUnitResult.GpuMemtestError:
                    return WorkUnitResultString.GpuMemtestError;
                case WorkUnitResult.UnknownEnum:
                    return WorkUnitResultString.UnknownEnum;
                default:
                    return String.Empty;
            }
        }

        public int ResultValue { get; set; }

        public DateTime Assigned { get; set; }

        public DateTime Finished { get; set; }

        public double BaseCredit { get; set; }
    }

    public class WorkUnitEntityRow : WorkUnitRow
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string CoreVersion { get; set; }

        public TimeSpan FrameTime => TimeSpan.FromSeconds(FrameTimeInSeconds);

        public int FrameTimeInSeconds { get; set; }

        public string Result { get; set; }

        public DateTime Assigned { get; set; }

        public DateTime Finished { get; set; }

        public double BaseCredit { get; set; }

        public string SlotType { get; set; }

        public double PPD { get; set; }

        public double Credit { get; set; }
    }
}
