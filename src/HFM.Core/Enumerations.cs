/*
 * HFM.NET - Core Enumerations
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

using System;

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
      Unknown = 0,
      [Obsolete("Use CPU.")]
      Uniprocessor = 1,
      CPU = 2,
      GPU = 3
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

   public enum TimeFormatting
   {
      None,
      Format1
   }

   public enum UnitTotalsType
   {
      All,
      ClientStart
   }

   public enum MinimizeToOption
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
   public enum FtpMode
   {
      Passive,
      Active
   }

   public enum WebDeploymentType
   {
      Path,
      Ftp
   }

   public enum ProcessingMode
   {
      Parallel,
      Serial
   }

   public enum StatsType
   {
      User,
      Team
   }

   // ReSharper disable InconsistentNaming

   public enum Preference
   {
      FormLocation,
      FormSize,
      FormColumns,
      FormSortColumn,
      FormSortOrder,
      FormSplitterLocation,
      FormLogWindowHeight,
      FormLogWindowVisible,
      QueueWindowVisible,
      TimeFormatting,
      UnitTotals,
      DisplayVersions,
      MinimizeTo,
      BenchmarksFormLocation,
      BenchmarksFormSize,
      GraphColors,
      BenchmarksGraphLayoutType,
      BenchmarksClientsPerGraph,
      MessagesFormLocation,
      MessagesFormSize,
      ClientRetrievalTaskType,
      ClientRetrievalTaskEnabled,
      ClientRetrievalTaskInterval,
      ClientRetrievalTask,
      DuplicateUserIdCheck,
      DuplicateProjectCheck,
      AllowRunningAsync,
      EnableUserStats,
      UserStatsType,
      WebGenerationTask,
      WebGenerationTaskEnabled,
      WebGenerationTaskInterval,
      WebGenerationTaskAfterClientRetrieval,
      WebDeploymentType,
      WebDeploymentRoot,
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
      BonusCalculation,
      FollowLog,
      DisplayEtaAsDate,
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
      HistoryBonusCalculation,
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
