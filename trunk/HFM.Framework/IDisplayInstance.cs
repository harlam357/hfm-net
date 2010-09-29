/*
 * HFM.NET - Display Instance Interface
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */
 
using System;
using System.Collections.Generic;

namespace HFM.Framework
{
   [CLSCompliant(false)]
   public interface IDisplayInstance
   {
      /// <summary>
      /// Specifies the name of External Instance if this DisplayInstance belongs to an external data source
      /// </summary>
      string ExternalInstanceName { get; }
   
      IUnitInfoLogic CurrentUnitInfo { get; set; }
   
      IClientInstanceSettings Settings { get; }
      
      bool UserIdUnknown { get; }
      
      string UserAndMachineId { get; }
   
      #region Grid Properties
   
      /// <summary>
      /// 
      /// </summary>
      ClientStatus Status { get; set; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      float Progress { get; }

      /// <summary>
      /// 
      /// </summary>
      String Name { get; }

      /// <summary>
      /// 
      /// </summary>
      string ClientType { get; }

      /// <summary>
      /// 
      /// </summary>
      TimeSpan TPF { get; }

      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      double PPD { get; }

      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      double UPD { get; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      Int32 MHz { get; }

      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      double PPD_MHz { get; }

      /// <summary>
      /// ETA for this instance
      /// </summary>
      TimeSpan ETA { get; }

      /// <summary>
      /// 
      /// </summary>
      string Core { get; }

      /// <summary>
      /// 
      /// </summary>
      string CoreID { get; }

      /// <summary>
      /// 
      /// </summary>
      string ProjectRunCloneGen { get; }

      /// <summary>
      /// 
      /// </summary>
      double Credit { get; }

      /// <summary>
      /// 
      /// </summary>
      int Complete { get; }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      int TotalRunFailedUnits { get; }

      /// <summary>
      /// 
      /// </summary>
      string Username { get; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; }

      /// <summary>
      /// 
      /// </summary>
      DateTime PreferredDeadline { get; }
      
      #endregion

      #region Complex Interfaces

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      IProtein CurrentProtein { get; }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      IList<LogLine> CurrentLogLines { get; }

      /// <summary>
      /// Return LogLine List for Specified Queue Index
      /// </summary>
      /// <param name="queueIndex">Index in Queue</param>
      /// <exception cref="ArgumentOutOfRangeException">If queueIndex is outside the bounds of the Log Lines Array</exception>
      IList<LogLine> GetLogLinesForQueueIndex(int queueIndex);

      /// <summary>
      /// Queue Base Interface
      /// </summary>
      IQueueBase Queue { get; }
      
      #endregion

      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      DateTime LastRetrievalTime { get; set; }

      /// <summary>
      /// Flag denoting if Progress, Production, and Time based values are OK to Display
      /// </summary>
      bool ProductionValuesOk { get; }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      InstanceType InstanceHostType { get; }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      string Path { get; }

      /// <summary>
      /// Client Path and Arguments (If Arguments Exist)
      /// </summary>
      string ClientPathAndArguments { get; }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      bool ClientIsOnVirtualMachine { get; }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      int FramesComplete { get; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      int PercentComplete { get; }
      
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType TypeOfClient { get; }

      /// <summary>
      /// Client Version
      /// </summary>
      string ClientVersion { get; }
      
      string CoreName { get; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      string CoreVersion { get; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      int ProjectID { get; }

      /// <summary>
      /// User ID is a Duplicate of another Client's User ID
      /// </summary>
      bool UserIdIsDuplicate { get; set; }

      /// <summary>
      /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
      /// </summary>
      bool ProjectIsDuplicate { get; set; }

      bool UsernameOk { get; }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      int TotalRunCompletedUnits { get; }

      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      int TotalClientCompletedUnits { get; }

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      string CachedFahLogName { get; }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      DateTime EtaDate { get; }

      bool Owns(IOwnedByClientInstance value);
   }
}
