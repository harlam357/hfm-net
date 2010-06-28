/*
 * HFM.NET - Client Instance Interface
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

namespace HFM.Framework
{
   [CLSCompliant(false)]
   public interface IClientInstance
   {
      /// <summary>
      /// Data Aggregator Interface
      /// </summary>
      IDataAggregator DataAggregator { get; }

      /// <summary>
      /// Client Instance Settings
      /// </summary>
      IClientInstanceSettings Settings { get; }

      /// <summary>
      /// Status of this client
      /// </summary>
      ClientStatus Status { get; }

      /// <summary>
      /// Flag denoting if Progress, Production, and Time based values are OK to Display
      /// </summary>
      bool ProductionValuesOk { get; }

      /// <summary>
      /// Client Version
      /// </summary>
      string ClientVersion { get; }

      /// <summary>
      /// Client Startup Arguments
      /// </summary>
      string Arguments { get; }

      /// <summary>
      /// User ID associated with this client
      /// </summary>
      string UserId { get; }

      /// <summary>
      /// User ID is a Duplicate of another Client's User ID
      /// </summary>
      bool UserIdIsDuplicate { get; }

      /// <summary>
      /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
      /// </summary>
      bool ProjectIsDuplicate { get; }

      /// <summary>
      /// True if User ID is Unknown
      /// </summary>
      bool UserIdUnknown { get; }

      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      int MachineId { get; }

      /// <summary>
      /// Combined User ID and Machine ID String
      /// </summary>
      string UserAndMachineId { get; }

      /// <summary>
      /// The Folding ID (Username) attached to this client
      /// </summary>
      string FoldingID { get; }

      /// <summary>
      /// The Team number attached to this client
      /// </summary>
      int Team { get; }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      int TotalRunCompletedUnits { get; }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      int TotalRunFailedUnits { get; }

      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      int TotalClientCompletedUnits { get; }

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      IUnitInfoLogic CurrentUnitInfo { get; }

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      string CachedFAHLogName { get; }

      /// <summary>
      /// Cached UnitInfo Filename for this instance
      /// </summary>
      string CachedUnitInfoName { get; }

      /// <summary>
      /// Cached Queue Filename for this instance
      /// </summary>
      string CachedQueueName { get; }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      int FramesComplete { get; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      int PercentComplete { get; }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      TimeSpan TimePerFrame { get; }

      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      double UPD { get; }

      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      double PPD { get; }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      TimeSpan ETA { get; }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      DateTime EtaDate { get; }

      double Credit { get; }

      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      DateTime LastRetrievalTime { get; }

      bool IsUsernameOk();
      
      bool Owns(IOwnedByClientInstance value);
   }
}
