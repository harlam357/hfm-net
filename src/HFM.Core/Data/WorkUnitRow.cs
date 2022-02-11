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

    [PetaPoco.TableName("WuHistory")]
    [PetaPoco.PrimaryKey("ID")]
    public class PetaPocoWorkUnitRow : WorkUnitRow
    {
        [PetaPoco.Column("InstanceName")]
        public string Name { get; set; }

        [PetaPoco.Column("InstancePath")]
        public string Path { get; set; }

        public float CoreVersion { get; set; }

        [PetaPoco.Ignore]
        public TimeSpan FrameTime => TimeSpan.FromSeconds(FrameTimeValue);

        [PetaPoco.Column("FrameTime")]
        public int FrameTimeValue { get; set; }

        [PetaPoco.Ignore]
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

        [PetaPoco.Column("Result")]
        public int ResultValue { get; set; }

        [PetaPoco.Column("DownloadDateTime")]
        public DateTime Assigned { get; set; }

        [PetaPoco.Column("CompletionDateTime")]
        public DateTime Finished { get; set; }

        [PetaPoco.Column("Credit")]
        public double BaseCredit { get; set; }

        [PetaPoco.ResultColumn]
        public string SlotType { get; set; }

        [PetaPoco.ResultColumn]
        public int ProductionView { get; set; }

        [PetaPoco.ResultColumn]
        public double PPD { get; set; }

        [PetaPoco.ResultColumn("CalcCredit")]
        public double Credit { get; set; }
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

        public int ProductionView { get; set; }

        public double PPD { get; set; }

        public double Credit { get; set; }
    }
}
