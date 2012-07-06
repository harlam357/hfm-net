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
   public class UnitInfo : IProjectInfo, IOwnedByClient, IEquatable<UnitInfo>
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
         UnitFrames = new Dictionary<int, UnitFrame>();
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
      private Dictionary<int, UnitFrame> UnitFrames { get; set; }

      /// <summary>
      /// Gets the total number of frames added to this unit.
      /// </summary>
      internal int FrameCount
      {
         get { return UnitFrames.Count; }
      }

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
      /// Sets the UnitFrame based on its frame ID.
      /// </summary>
      /// <exception cref="ArgumentNullException">frame is null.</exception>
      public void SetUnitFrame(UnitFrame frame)
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
            frame.FrameDuration = frame.TimeOfFrame.GetDelta(UnitFrames[frame.FrameID - 1].TimeOfFrame);
         }
      }

      /// <summary>
      /// Gets the UnitFrame for the frame ID.
      /// </summary>
      public UnitFrame GetUnitFrame(int frameId)
      {
         return UnitFrames.ContainsKey(frameId) ? UnitFrames[frameId] : null;
      }
      
      #endregion

      #region IEquatable<T> Implementation

      /// <summary>
      /// Indicates whether the current object is equal to another object of the same type.
      /// </summary>
      /// <returns>
      /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
      /// </returns>
      /// <param name="other">An object to compare with this object.</param>
      public bool Equals(UnitInfo other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Equals(other.OwningClientName, OwningClientName) && 
                Equals(other.OwningClientPath, OwningClientPath) && 
                other.OwningSlotId == OwningSlotId && 
                other.UnitRetrievalTime.Equals(UnitRetrievalTime) && 
                Equals(other.FoldingID, FoldingID) && 
                other.Team == Team && 
                Equals(other.SlotType, SlotType) && 
                other.DownloadTime.Equals(DownloadTime) && 
                other.DueTime.Equals(DueTime) && 
                other.UnitStartTimeStamp.Equals(UnitStartTimeStamp) && 
                other.FinishedTime.Equals(FinishedTime) && 
                other.CoreVersion.Equals(CoreVersion) && 
                other.ProjectID == ProjectID && 
                other.ProjectRun == ProjectRun && 
                other.ProjectClone == ProjectClone && 
                other.ProjectGen == ProjectGen && 
                Equals(other.ProteinName, ProteinName) && 
                Equals(other.ProteinTag, ProteinTag) && 
                Equals(other.UnitResult, UnitResult) && 
                other.RawFramesComplete == RawFramesComplete && 
                other.RawFramesTotal == RawFramesTotal && 
                other.FramesObserved == FramesObserved && 
                other.UnitFrames.SequenceEqual(UnitFrames) && 
                Equals(other.CoreID, CoreID) && 
                other.QueueIndex == QueueIndex;
      }

      /// <summary>
      /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
      /// </returns>
      /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj)) return false;
         if (ReferenceEquals(this, obj)) return true;
         if (obj.GetType() != typeof(UnitInfo)) return false;
         return Equals((UnitInfo)obj);
      }

      /// <summary>
      /// Serves as a hash function for a particular type. 
      /// </summary>
      /// <returns>
      /// A hash code for the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override int GetHashCode()
      {
         unchecked
         {
            int result = (OwningClientName != null ? OwningClientName.GetHashCode() : 0);
            result = (result * 397) ^ (OwningClientPath != null ? OwningClientPath.GetHashCode() : 0);
            result = (result * 397) ^ OwningSlotId;
            result = (result * 397) ^ UnitRetrievalTime.GetHashCode();
            result = (result * 397) ^ (FoldingID != null ? FoldingID.GetHashCode() : 0);
            result = (result * 397) ^ Team;
            result = (result * 397) ^ SlotType.GetHashCode();
            result = (result * 397) ^ DownloadTime.GetHashCode();
            result = (result * 397) ^ DueTime.GetHashCode();
            result = (result * 397) ^ UnitStartTimeStamp.GetHashCode();
            result = (result * 397) ^ FinishedTime.GetHashCode();
            result = (result * 397) ^ CoreVersion.GetHashCode();
            result = (result * 397) ^ ProjectID;
            result = (result * 397) ^ ProjectRun;
            result = (result * 397) ^ ProjectClone;
            result = (result * 397) ^ ProjectGen;
            result = (result * 397) ^ (ProteinName != null ? ProteinName.GetHashCode() : 0);
            result = (result * 397) ^ (ProteinTag != null ? ProteinTag.GetHashCode() : 0);
            result = (result * 397) ^ UnitResult.GetHashCode();
            result = (result * 397) ^ RawFramesComplete;
            result = (result * 397) ^ RawFramesTotal;
            result = (result * 397) ^ FramesObserved;
            result = (result * 397) ^ (UnitFrames != null ? UnitFrames.GetHashCode() : 0);
            result = (result * 397) ^ (CoreID != null ? CoreID.GetHashCode() : 0);
            result = (result * 397) ^ QueueIndex;
            return result;
         }
      }

      public static bool operator ==(UnitInfo left, UnitInfo right)
      {
         return Equals(left, right);
      }

      public static bool operator !=(UnitInfo left, UnitInfo right)
      {
         return !Equals(left, right);
      }

      #endregion
   }
}
