/*
 * HFM.NET - Framework Enumerations
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace HFM.Framework
{
   #region Instance and Client Types
   public enum InstanceType
   {
      PathInstance,
      FtpInstance,
      HttpInstance
   }

   public enum ClientType
   {
      Unknown,
      Standard,
      SMP,
      GPU
   }
   #endregion
   
   public enum FtpType
   {
      Passive,
      Active
   }
   
   #region Client and Work Unit Status Types
   public enum ClientStatus
   {
      Unknown,
      Offline,
      Stopped,
      EuePause,
      Hung,
      Paused,
      SendingWorkPacket,
      GettingWorkPacket,
      RunningNoFrameTimes,
      RunningAsync,
      Running
   }

   public enum WorkUnitResult
   {
      Unknown,
      FinishedUnit,
      EarlyUnitEnd,
      UnstableMachine,
      Interrupted,
      BadWorkUnit,
      CoreOutdated
   }
   #endregion

   /// <summary>
   /// The Queue Entry Status
   /// </summary>
   public enum QueueEntryStatus
   {
      Unknown = -1,
      Empty,
      Deleted,
      Finished,
      Garbage,
      FoldingNow,
      Queued,
      ReadyForUpload,
      Abandonded,
      FetchingFromServer
   }

   /// <summary>
   /// Log Line Types
   /// </summary>
   public enum LogLineType
   {
      Unknown = -1,
      LogOpen = 0,
      LogHeader,
      ClientVersion,
      ClientSendWorkToServer,
      ClientAutosendStart,
      ClientAutosendComplete,
      ClientSendStart,
      ClientSendConnectFailed,
      ClientSendFailed,
      ClientSendComplete,
      ClientArguments,
      ClientUserNameTeam,
      ClientRequestingUserID,
      ClientReceivedUserID,
      ClientUserID,
      ClientMachineID,
      ClientAttemptGetWorkPacket,
      ClientIndicateMemory,
      ClientDetectCpu,
      WorkUnitProcessing,
      WorkUnitCoreDownload,
      WorkUnitIndex,
      WorkUnitQueueIndex,
      WorkUnitWorking,
      WorkUnitStart,
      WorkUnitCoreVersion,
      WorkUnitRunning,
      WorkUnitProject,
      WorkUnitFrame,
      WorkUnitPaused,
      WorkUnitPausedForBattery,
      WorkUnitResumeFromBattery,
      WorkUnitShuttingDownCore,
      WorkUnitCoreShutdown,
      ClientNumberOfUnitsCompleted,
      ClientCoreCommunicationsErrorShutdown,
      ClientEuePauseState,
      ClientShutdown
   }

   public enum PpdCalculationType
   {
      LastFrame,
      LastThreeFrames,
      AllFrames,
      EffectiveRate
   }

   public enum HistoryProductionView
   {
      BonusDownloadTime,
      BonusFrameTime,
      Standard
   }

   public enum QueryFieldName
   {
      ProjectID = 0,
      ProjectRun,
      ProjectClone,
      ProjectGen,
      InstanceName,
      InstancePath,
      Username,
      Team,
      CoreVersion,
      FramesCompleted,
      FrameTime,
      Result,
      DownloadDateTime,
      CompletionDateTime,
      WorkUnitName,
      KFactor,
      Core,
      Frames,
      Atoms,
      ClientType,
      PPD,
      Credit
   }

   public enum QueryFieldType
   {
      Equal,
      GreaterThan,
      GreaterThanOrEqual,
      LessThan,
      LessThanOrEqual,
   }

   public enum TimeStyleType
   {
      Standard,
      Formatted
   }
   
   public enum CompletedCountDisplayType
   {
      ClientTotal,
      ClientRunTotal
   }
   
   public enum FormShowStyleType
   {
      SystemTray,
      TaskBar,
      Both
   }
   
   public enum Preference
   {
      FormLocation,
      FormSize,
      FormColumns,
      FormSortColumn,
      FormSortOrder,
      FormSplitLocation,
      FormLogWindowHeight,
      FormLogVisible,
      QueueViewerVisible,
      TimeStyle,
      CompletedCountDisplay,
      ShowVersions,
      FormShowStyle,
      BenchmarksFormLocation,
      BenchmarksFormSize,
      GraphColors,
      MessagesFormLocation,
      MessagesFormSize,
      SyncOnLoad,
      SyncOnSchedule,
      SyncTimeMinutes,
      DuplicateUserIdCheck,
      DuplicateProjectCheck,
      AllowRunningAsync,
      ShowXmlStats,
      ShowTeamStats,
      GenerateWeb,
      GenerateInterval,
      WebGenAfterRefresh,
      WebRoot,
      WebGenCopyFAHlog,
      WebGenFtpMode,
      WebGenCopyHtml,
      WebGenCopyXml,
      WebGenLimitLogSize,
      WebGenLimitLogSizeLength,
      CssFile,
      WebOverview,
      WebMobileOverview,
      WebSummary,
      WebMobileSummary,
      WebInstance,
      RunMinimized,
      StartupCheckForUpdate,
      UseDefaultConfigFile,
      DefaultConfigFile,
      OfflineLast,
      ColorLogFile,
      AutoSaveConfig,
      MaintainSelectedClient,
      PpdCalculation,
      DecimalPlaces,
      CalculateBonus,
      EtaDate,
      LogFileViewer,
      FileExplorer,
      MessageLevel,
      EmailReportingEnabled,
      EmailReportingServerSecure,
      EmailReportingToAddress,
      EmailReportingFromAddress,
      EmailReportingServerAddress,
      EmailReportingServerPort,
      EmailReportingServerUsername,
      EmailReportingServerPassword,
      ReportEuePause,
      EocUserId,
      StanfordId,
      TeamId,
      ProjectDownloadUrl,
      UseProxy,
      ProxyServer,
      ProxyPort,
      UseProxyAuth,
      ProxyUser,
      ProxyPass,
      HistoryProductionType,
      ShowTopChecked,
      ShowTopValue,
      HistorySortColumnName,
      HistorySortOrder,
      CacheFolder,
      ApplicationDataFolderPath 
   }
}
