/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
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
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class UnitInfo : IProjectInfo, IOwnedByClient
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

      #region Properties

      #region IOwnedByClient Implementation

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
      [DataMember(Order = 1)]
      public string OwningClientName { get; set; }

      /// <summary>
      /// Path of the folding slot that own this object.
      /// </summary>
      [DataMember(Order = 2)]
      public string OwningClientPath { get; set; }

      /// <summary>
      /// Identification number of the folding slot on the folding client that owns this object.
      /// </summary>
      [DataMember(Order = 26, IsRequired = true)]
      public int OwningSlotId { get; set; }

      #endregion

      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      [DataMember(Order = 3)]
      public DateTime UnitRetrievalTime { get; set; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      [DataMember(Order = 4)]
      public string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      [DataMember(Order = 5)]
      public int Team { get; set; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      [DataMember(Order = 6)]
      public SlotType SlotType { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      [DataMember(Order = 7)]
      public DateTime DownloadTime { get; set; }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      [DataMember(Order = 8)]
      public DateTime DueTime { get; set; }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See LegacyClient.HandleReturnedStatus</remarks>
      [DataMember(Order = 9)]
      public TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      [DataMember(Order = 10)]
      public DateTime FinishedTime { get; set; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      [DataMember(Order = 11)]
      public float CoreVersion { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      [DataMember(Order = 12)]
      public int ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      [DataMember(Order = 13)]
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      [DataMember(Order = 14)]
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      [DataMember(Order = 15)]
      public int ProjectGen { get; set; }

      /// <summary>
      /// Name of the unit
      /// </summary>
      [DataMember(Order = 16)]
      public string ProteinName { get; set; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      [DataMember(Order = 17)]
      public string ProteinTag { get; set; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      [DataMember(Order = 18)]
      public WorkUnitResult UnitResult { get; set; }

      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      [DataMember(Order = 19)]
      public int RawFramesComplete { get; set; }

      /// <summary>
      /// Raw total number of steps
      /// </summary>
      [DataMember(Order = 20)]
      public int RawFramesTotal { get; set; }

      /// <summary>
      /// Gets or sets the number of frames observed since the unit was last started.
      /// </summary>
      [DataMember(Order = 21)]
      public int FramesObserved { get; set; }

      // Open DataMember - was of type UnitFrame
      //[DataMember(Order = 22)]

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public UnitFrame CurrentFrame
      {
         get
         {
            if (UnitFrames == null || UnitFrames.Count == 0)
            {
               return null;
            }

            int max = UnitFrames.Keys.Max();
            if (max >= 0)
            {
               Debug.Assert(UnitFrames[max].FrameID == max);
               return UnitFrames[max];
            }

            return null;
         }
      }

      private IList<LogLine> _logLines;

      [DataMember(Order = 23)]
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
            UnitFrames = BuildUnitFrameDictionary(_logLines);
            if (UnitFrames.Count > 0)
            {
               var lastUnitFrame = UnitFrames[UnitFrames.Keys.Max()];
               RawFramesComplete = lastUnitFrame.RawFramesComplete;
               RawFramesTotal = lastUnitFrame.RawFramesTotal;
            }
         }
      }

      private static Dictionary<int, UnitFrame> BuildUnitFrameDictionary(IEnumerable<LogLine> logLines)
      {
         var unitFrames = logLines.Where(x => x.LineType == LogLineType.WorkUnitFrame).Select(x =>
         {
            var unitFrame = x.LineData as UnitFrame;
            if (unitFrame != null && unitFrame.FrameID >= 0)
            {
               return unitFrame;
            }
            return null;
         }).Where(x => x != null).ToDictionary(x => x.FrameID, true);

         foreach (var frame in unitFrames.Values)
         {
            if (unitFrames.ContainsKey(frame.FrameID - 1))
            {
               frame.FrameDuration = frame.TimeOfFrame.GetDelta(unitFrames[frame.FrameID - 1].TimeOfFrame);
            }
         }

         return unitFrames;
      }

      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      internal Dictionary<int, UnitFrame> UnitFrames { get; set; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      [DataMember(Order = 24)]
      public string CoreID { get; set; }

      /// <summary>
      /// Unit Queue Index
      /// </summary>
      [DataMember(Order = 25)]
      public int QueueIndex { get; set; }

      #endregion

      #region Methods

      /// <summary>
      /// Gets the UnitFrame for the frame ID.
      /// </summary>
      public UnitFrame GetUnitFrame(int frameId)
      {
         return UnitFrames != null && UnitFrames.ContainsKey(frameId) ? UnitFrames[frameId] : null;
      }

      #endregion
   }
}
