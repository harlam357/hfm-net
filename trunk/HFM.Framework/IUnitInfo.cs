/*
 * HFM.NET - Unit Info Interface
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

using System;

namespace HFM.Framework
{
   public interface IUnitInfo : IOwnedByClientInstance
   {
      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      DateTime UnitRetrievalTime { get; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      Int32 Team { get; set; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType TypeOfClient { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; set; }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      bool DownloadTimeUnknown { get; }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      DateTime DueTime { get; set; }
      
      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      bool DueTimeUnknown { get; }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; set; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      string CoreVersion { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      Int32 ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      Int32 ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      Int32 ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      Int32 ProjectGen { get; set; }

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      bool ProjectIsUnknown { get; }

      /// <summary>
      /// Name of the unit
      /// </summary>
      String ProteinName { get; set; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      string ProteinTag { get; set; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      WorkUnitResult UnitResult { get; set; }

      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      Int32 RawFramesComplete { get; set; }

      /// <summary>
      /// Raw total number of steps
      /// </summary>
      Int32 RawFramesTotal { get; set; }
      
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      Int32 FramesObserved { get; set; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      IUnitFrame CurrentFrame { get; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      string CoreId { get; set; }

      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      void SetCurrentFrame(ILogLine logLine);

      /// <summary>
      /// Get the UnitFrame Interface for this frameID
      /// </summary>
      IUnitFrame GetUnitFrame(int frameID);
   }
}
