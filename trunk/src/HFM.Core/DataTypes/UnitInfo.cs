/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class UnitInfo : IProjectInfo, IOwnedByClientSlot, IEquatable<UnitInfo>
   {
      public UnitInfo()
      {
         UnitRetrievalTime = DateTime.MinValue;
         FoldingID = Default.FoldingID;
         Team = Default.Team;
         SlotType = SlotType.Unknown;
         DownloadTime = DateTime.MinValue;
         DueTime = DateTime.MinValue;
         UnitStartTimeStamp = TimeSpan.Zero;
         FinishedTime = DateTime.MinValue;
         CoreVersion = String.Empty;
         ProteinName = String.Empty;
         ProteinTag = String.Empty;
         UnitResult = WorkUnitResult.Unknown;
         UnitFrames = new Dictionary<int, UnitFrame>();
         CoreID = Default.CoreID;
      }
      
      #region Owner Data Properties

      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      [DataMember(Order = 1)]
      public string OwningSlotName { get; set; }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      [DataMember(Order = 2)]
      public string OwningSlotPath { get; set; }

      #endregion

      #region Retrieval Time Property

      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      [DataMember(Order = 3)]
      public DateTime UnitRetrievalTime { get; set; }

      #endregion

      #region Folding ID and Team Properties

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

      #endregion

      #region Unit Level Members

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
      public string CoreVersion { get; set; }

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

      #endregion

      #region Frames/Percent Completed Unit Level Members

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

      #endregion

      #region Frame (UnitFrame) Data Variables

      /// <summary>
      /// Number of Frames Observed since Last Unit Start or Resume from Pause
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
            if (UnitFrames.Count == 0) return null;

            int max = UnitFrames.Keys.Max();
            if (max >= 0)
            {
               Debug.Assert(UnitFrames[max].FrameID == max);
               return UnitFrames[max];
            }

            return null;
         }
      }

      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [DataMember(Order = 23)]
      public Dictionary<int, UnitFrame> UnitFrames { get; private set; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      [DataMember(Order = 24)]
      public string CoreID { get; set; }

      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      /// <exception cref="ArgumentNullException">Throws if 'frame' is null.</exception>
      public void SetCurrentFrame(UnitFrame frame)
      {
         if (frame == null) throw new ArgumentNullException("frame");

         if (UnitFrames.ContainsKey(frame.FrameID)) return;
         
         // Set Raw Frame Values                                       
         RawFramesComplete = frame.RawFramesComplete;
         RawFramesTotal = frame.RawFramesTotal;
         
         UnitFrames.Add(frame.FrameID, frame);

         frame.FrameDuration = TimeSpan.Zero;
         if (frame.FrameID > 0 &&
             UnitFrames.ContainsKey(frame.FrameID - 1) &&
             FramesObserved > 1)
         {
            frame.FrameDuration = GetDelta(frame.TimeOfFrame, UnitFrames[frame.FrameID - 1].TimeOfFrame);
         }
      }

      /// <summary>
      /// Get Time Delta between given frames
      /// </summary>
      /// <param name="timeLastFrame">Time of last frame</param>
      /// <param name="timeCompareFrame">Time of a previous frame to compare</param>
      private static TimeSpan GetDelta(TimeSpan timeLastFrame, TimeSpan timeCompareFrame)
      {
         TimeSpan tDelta;

         // check for rollover back to 00:00:00 timeLastFrame will be less than previous timeCompareFrame reading
         if (timeLastFrame < timeCompareFrame)
         {
            // get time before rollover
            tDelta = TimeSpan.FromDays(1).Subtract(timeCompareFrame);
            // add time from latest reading
            tDelta = tDelta.Add(timeLastFrame);
         }
         else
         {
            tDelta = timeLastFrame.Subtract(timeCompareFrame);
         }

         return tDelta;
      }
      
      /// <summary>
      /// Get the UnitFrame Interface for this frameID
      /// </summary>
      public UnitFrame GetUnitFrame(int frameID)
      {
         if (UnitFrames.ContainsKey(frameID))
         {
            return UnitFrames[frameID];
         }
         
         return null;
      }
      
      #endregion

      public UnitInfo DeepClone()
      {
         return ProtoBuf.Serializer.DeepClone(this);
      }

      #region IEquatable<UnitInfo> Members

      public bool Equals(UnitInfo other)
      {
         if (other == null)
         {
            return false;
         }

         // if the Projects are known
         if (!this.ProjectIsUnknown() && !other.ProjectIsUnknown())
         {
            // ReSharper disable RedundantThisQualifier
            // equals the Project and Download Time
            if (this.EqualsProject(other) &&
                this.DownloadTime.Equals(other.DownloadTime))
            {
               return true;
            }
            // ReSharper restore RedundantThisQualifier
         }

         return false;
      }

      #endregion
   }
}
