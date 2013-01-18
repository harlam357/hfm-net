/*
 * HFM.NET - Core Enumerations
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

namespace HFM.Core
{
   /// <summary>
   /// Client Types
   /// </summary>
   public enum ClientType
   {
      FahClient,
      Legacy,
      External //?
   }

   /// <summary>
   /// Legacy Client Sub Types
   /// </summary>
   public enum LegacyClientSubType
   {
      None,
      Path,
      Ftp,
      Http
   }

   // ReSharper disable InconsistentNaming

   /// <summary>
   /// Folding Slot Type
   /// </summary>
   public enum SlotType
   {
      Unknown,
      Uniprocessor,
      SMP,
      GPU
   }

   // ReSharper restore InconsistentNaming

   /// <summary>
   /// Queue Entry Status Types
   /// </summary>
   public enum QueueEntryStatus
   {
      Unknown,
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

   public enum PpdCalculationType
   {
      LastFrame,
      LastThreeFrames,
      AllFrames,
      EffectiveRate
   }

   /// <summary>
   /// Bonus Calculation Types
   /// </summary>
   public enum BonusCalculationType
   {
      DownloadTime,
      FrameTime,
      None
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

   public enum GraphLayoutType
   {
      Single,
      ClientsPerGraph
   }

   /// <summary>
   /// Ftp Transfer Types
   /// </summary>
   public enum FtpType
   {
      Passive,
      Active
   }

   /// <summary>
   /// Work Unit History Production View Type
   /// </summary>
   public enum HistoryProductionView
   {
      BonusDownloadTime,
      BonusFrameTime,
      Standard
   }

   /// <summary>
   /// Work Unit History Query Field Names
   /// </summary>
   public enum QueryFieldName
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
      DownloadDateTime,
      CompletionDateTime,
      WorkUnitName,
      KFactor,
      Core,
      Frames,
      Atoms,
      SlotType,
      PPD,
      Credit
   }

   /// <summary>
   /// Work Unit History Query Field Types
   /// </summary>
   public enum QueryFieldType
   {
      Equal,
      GreaterThan,
      GreaterThanOrEqual,
      LessThan,
      LessThanOrEqual,
      Like
   }

   public enum WebGenType
   {
      Path,
      Ftp
   }

   // ReSharper disable InconsistentNaming
   
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
      BenchmarksGraphLayoutType,
      BenchmarksClientsPerGraph,
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
      WebGenType,
      WebRoot,
      WebGenServer,
      WebGenPort,
      WebGenUsername,
      WebGenPassword,
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
      WebSlot,
      RunMinimized,
      StartupCheckForUpdate,
      UseDefaultConfigFile,
      DefaultConfigFile,
      OfflineLast,
      ColorLogFile,
      AutoSaveConfig,
      PpdCalculation,
      DecimalPlaces,
      CalculateBonus,
      FollowLogFile,
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
      ReportHung,
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
      ShowFirstChecked,
      ShowLastChecked,
      ShowEntriesValue,
      HistorySortColumnName,
      HistorySortOrder,
      HistoryFormLocation,
      HistoryFormSize,
      HistoryFormColumns,
      CacheFolder
   }

   // ReSharper restore InconsistentNaming
}
