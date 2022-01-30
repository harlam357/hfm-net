
namespace HFM.Core.WorkUnits
{
    /// <summary>
    /// Work unit result types.
    /// </summary>
    public enum WorkUnitResult
    {
        // 0: Unknown result
        Unknown,
        // 1: Finished Unit (Terminating)
        FinishedUnit,
        // 2: Early Unit End (Terminating)
        EarlyUnitEnd,
        // 3: Unstable Machine (Terminating)
        UnstableMachine,
        // 4: Interrupted (Non-Terminating)
        Interrupted,
        // 5: Bad Work Unit (Terminating)
        BadWorkUnit,
        // 6: Core outdated (Non-Terminating)
        CoreOutdated,
        // 7: Client core communications error (Terminating)
        ClientCoreError,
        // 8: GPU memtest error (Non-Terminating) - No unit test coverage
        GpuMemtestError,
        // 9: Unknown Enum (Non-Terminating)
        UnknownEnum,
        // 10: Bad Frame Checksum (Terminating)
        BadFrameChecksum
    }

    public static class WorkUnitResultString
    {
        public const string FinishedUnit = "FINISHED_UNIT";
        public const string EarlyUnitEnd = "EARLY_UNIT_END";
        public const string UnstableMachine = "UNSTABLE_MACHINE";
        public const string Interrupted = "INTERRUPTED";
        public const string BadWorkUnit = "BAD_WORK_UNIT";
        public const string CoreOutdated = "CORE_OUTDATED";
        public const string GpuMemtestError = "GPU_MEMTEST_ERROR";
        public const string UnknownEnum = "UNKNOWN_ENUM";
        public const string BadFrameChecksum = "BAD_FRAME_CHECKSUM";
        public const string Unknown = "UNKNOWN";

        internal static WorkUnitResult ToWorkUnitResult(string result) =>
            result switch
            {
                FinishedUnit => WorkUnitResult.FinishedUnit,
                EarlyUnitEnd => WorkUnitResult.EarlyUnitEnd,
                UnstableMachine => WorkUnitResult.UnstableMachine,
                Interrupted => WorkUnitResult.Interrupted,
                BadWorkUnit => WorkUnitResult.BadWorkUnit,
                CoreOutdated => WorkUnitResult.CoreOutdated,
                GpuMemtestError => WorkUnitResult.GpuMemtestError,
                UnknownEnum => WorkUnitResult.UnknownEnum,
                BadFrameChecksum => WorkUnitResult.BadFrameChecksum,
                _ => WorkUnitResult.Unknown
            };

        internal static string FromWorkUnitResult(WorkUnitResult result) =>
            result switch
            {
                WorkUnitResult.FinishedUnit => FinishedUnit,
                WorkUnitResult.EarlyUnitEnd => EarlyUnitEnd,
                WorkUnitResult.UnstableMachine => UnstableMachine,
                WorkUnitResult.Interrupted => Interrupted,
                WorkUnitResult.BadWorkUnit => BadWorkUnit,
                WorkUnitResult.CoreOutdated => CoreOutdated,
                WorkUnitResult.GpuMemtestError => GpuMemtestError,
                WorkUnitResult.UnknownEnum => UnknownEnum,
                WorkUnitResult.BadFrameChecksum => BadFrameChecksum,
                _ => Unknown
            };
    }

    internal static class WorkUnitResultExtensions
    {
        internal static bool IsTerminating(this WorkUnitResult result)
        {
            switch (result)
            {
                case WorkUnitResult.FinishedUnit:
                case WorkUnitResult.EarlyUnitEnd:
                case WorkUnitResult.UnstableMachine:
                case WorkUnitResult.BadWorkUnit:
                case WorkUnitResult.ClientCoreError:
                case WorkUnitResult.BadFrameChecksum:
                    return true;
                default:
                    return false;
            }
        }
    }
}
