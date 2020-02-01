/*
 * HFM.NET
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using HFM.Core.WorkUnits;
using HFM.Log;

namespace HFM.Core.DataTypes
{
   public class UnitInfo : IProjectInfo
   {
      #region Constructor

      public UnitInfo()
      {
         UnitRetrievalTime = DateTime.MinValue;
         FoldingID = Constants.DefaultFoldingID;
         Team = Constants.DefaultTeam;
         SlotType = SlotType.Unknown;
         DownloadTime = DateTime.MinValue;
         DueTime = DateTime.MinValue;
         UnitStartTimeStamp = TimeSpan.Zero;
         FinishedTime = DateTime.MinValue;
         CoreVersion = 0;
         ProteinName = String.Empty;
         ProteinTag = String.Empty;
         UnitResult = WorkUnitResult.Unknown;
         CoreID = Constants.DefaultCoreID;
         QueueIndex = -1;
         OwningSlotId = -1;
      }

      #endregion

      // TODO: Rename to Copy()
      public UnitInfo DeepClone()
      {
         var u = new UnitInfo
         {
            OwningClientName = OwningClientName,
            OwningClientPath = OwningClientPath,
            UnitRetrievalTime = UnitRetrievalTime,
            FoldingID = FoldingID,
            Team = Team,
            SlotType = SlotType,
            DownloadTime = DownloadTime,
            DueTime = DueTime,
            UnitStartTimeStamp = UnitStartTimeStamp,
            FinishedTime = FinishedTime,
            CoreVersion = CoreVersion,
            ProjectID = ProjectID,
            ProjectRun = ProjectRun,
            ProjectClone = ProjectClone,
            ProjectGen = ProjectGen,
            ProteinName = ProteinName,
            ProteinTag = ProteinTag,
            UnitResult = UnitResult,
            FramesObserved = FramesObserved,
            // TODO: LogLines is NOT a clone
            LogLines = LogLines,
            CoreID = CoreID,
            QueueIndex = QueueIndex,
            OwningSlotId = OwningSlotId
         };
         return u;
      }

      #region Properties

      /// <summary>
      /// Fully qualified name of the folding slot that owns this object (includes "Slot" designation).
      /// </summary>
      public string OwningSlotName
      {
         get { return OwningClientName.AppendSlotId(OwningSlotId); }
      }

      /// <summary>
      /// Name of the folding client that owns this object (name given during client setup).
      /// </summary>
      public string OwningClientName { get; set; }

      /// <summary>
      /// Path of the folding slot that own this object.
      /// </summary>
      public string OwningClientPath { get; set; }

      /// <summary>
      /// Identification number of the folding slot on the folding client that owns this object.
      /// </summary>
      public int OwningSlotId { get; set; }

      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      public DateTime UnitRetrievalTime { get; set; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      public string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      public int Team { get; set; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public SlotType SlotType { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime { get; set; }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime { get; set; }

      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      public TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      public DateTime FinishedTime { get; set; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      public float CoreVersion { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public int ProjectGen { get; set; }

      /// <summary>
      /// Name of the unit
      /// </summary>
      public string ProteinName { get; set; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      public string ProteinTag { get; set; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      public WorkUnitResult UnitResult { get; set; }

      /// <summary>
      /// Gets or sets the number of frames observed since the unit was last started.
      /// </summary>
      public int FramesObserved { get; set; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public WorkUnitFrameData CurrentFrame
      {
         get
         {
            if (FrameData == null || FrameData.Count == 0)
            {
               return null;
            }

            int max = FrameData.Keys.Max();
            if (max >= 0)
            {
               Debug.Assert(FrameData[max].ID == max);
               return FrameData[max];
            }

            return null;
         }
      }

      private IList<LogLine> _logLines;

      public IList<LogLine> LogLines
      {
         get { return _logLines; }
         set
         {
            if (value == null)
            {
               return;
            }
            _logLines = value;
         }
      }

      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      public IDictionary<int, WorkUnitFrameData> FrameData { get; set; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      public string CoreID { get; set; }

      /// <summary>
      /// Unit Queue Index
      /// </summary>
      public int QueueIndex { get; set; }

      #endregion

      #region Methods

      /// <summary>
      /// Gets the WorkUnitFrameData for the frame ID.
      /// </summary>
      public WorkUnitFrameData GetFrameData(int frameId)
      {
         return FrameData != null && FrameData.ContainsKey(frameId) ? FrameData[frameId] : null;
      }

      #endregion
   }
}
