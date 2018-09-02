
namespace HFM.Log
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
      UnknownEnum
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
   }
}