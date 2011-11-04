/*
 * HFM.NET - Core Enumerations
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
   public enum PpdCalculationType
   {
      LastFrame,
      LastThreeFrames,
      AllFrames,
      EffectiveRate
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
      WebRoot,
      WebGenCopyFAHlog,
      WebGenFtpMode,
      WebGenCopyHtml,
      WebGenCopyXml,
      WebGenCopyClientData,
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
}
