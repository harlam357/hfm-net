/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
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
using System.Collections.Generic;
using System.Globalization;

using ProtoBuf;

namespace HFM.Framework.DataTypes
{
   public interface IUnitInfo : IProjectInfo, IOwnedByClientInstance
   {
      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      DateTime UnitRetrievalTime { get; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      string FoldingID { get; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      int Team { get; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType TypeOfClient { get; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      bool DownloadTimeUnknown { get; }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      DateTime DueTime { get; }

      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      bool DueTimeUnknown { get; }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      TimeSpan UnitStartTimeStamp { get; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      string CoreVersion { get; }

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      bool ProjectIsUnknown { get; }

      /// <summary>
      /// Name of the unit
      /// </summary>
      String ProteinName { get; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      string ProteinTag { get; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      WorkUnitResult UnitResult { get; }

      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      int RawFramesComplete { get; }

      /// <summary>
      /// Raw total number of steps
      /// </summary>
      int RawFramesTotal { get; }

      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      int FramesObserved { get; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      IUnitFrame CurrentFrame { get; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      string CoreID { get; }

      /// <summary>
      /// Get the UnitFrame Interface for this frameID
      /// </summary>
      IUnitFrame GetUnitFrame(int frameID);
   }

   [ProtoContract]
   public class UnitInfo : IUnitInfo
   {
      public UnitInfo()
      {
         FoldingID = Constants.FoldingIDDefault;
         Team = Constants.TeamDefault;

         TypeOfClient = ClientType.Unknown;
         DownloadTime = DateTime.MinValue;
         DueTime = DateTime.MinValue;
         UnitStartTimeStamp = TimeSpan.Zero;
         FinishedTime = DateTime.MinValue;
         CoreVersion = String.Empty;
         ProteinName = String.Empty;
         ProteinTag = String.Empty;
         UnitResult = WorkUnitResult.Unknown;
         UnitFrames = new Dictionary<int, UnitFrame>();
         CoreID = Constants.CoreIDDefault;
      }
      
      #region Owner Data Properties
      private string _owningInstanceName;
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      [ProtoMember(1)]
      public string OwningInstanceName
      {
         get { return _owningInstanceName; }
         set { _owningInstanceName = value; }
      }

      private string _owningInstancePath;
      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      [ProtoMember(2)]
      public string OwningInstancePath
      {
         get { return _owningInstancePath; }
         set { _owningInstancePath = value; }
      }
      #endregion

      #region Retrieval Time Property
      private DateTime _unitRetrievalTime;
      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      [ProtoMember(3)]
      public DateTime UnitRetrievalTime
      {
         get { return _unitRetrievalTime; }
         set { _unitRetrievalTime = value; }
      } 
      #endregion

      #region Folding ID and Team Properties

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      [ProtoMember(4)]
      public string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      [ProtoMember(5)]
      public int Team { get; set; }

      #endregion

      #region Unit Level Members

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      [ProtoMember(6)]
      public ClientType TypeOfClient { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      [ProtoMember(7)]
      public DateTime DownloadTime { get; set; }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      public bool DownloadTimeUnknown
      {
         get { return DownloadTime.Equals(DateTime.MinValue); }
      }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      [ProtoMember(8)]
      public DateTime DueTime { get; set; }

      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      public bool DueTimeUnknown
      {
         get { return DueTime.Equals(DateTime.MinValue); }
      }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      [ProtoMember(9)]
      public TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      [ProtoMember(10)]
      public DateTime FinishedTime { get; set; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      [ProtoMember(11)]
      public string CoreVersion { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      [ProtoMember(12)]
      public int ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      [ProtoMember(13)]
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      [ProtoMember(14)]
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      [ProtoMember(15)]
      public int ProjectGen { get; set; }

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      public bool ProjectIsUnknown
      {
         get
         {
            return ProjectID == 0 &&
                   ProjectRun == 0 &&
                   ProjectClone == 0 &&
                   ProjectGen == 0;
         }
      }

      /// <summary>
      /// Name of the unit
      /// </summary>
      [ProtoMember(16)]
      public string ProteinName { get; set; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      [ProtoMember(17)]
      public string ProteinTag { get; set; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      [ProtoMember(18)]
      public WorkUnitResult UnitResult { get; set; }

      #endregion

      #region Frames/Percent Completed Unit Level Members

      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      [ProtoMember(19)]
      public int RawFramesComplete { get; set; }

      /// <summary>
      /// Raw total number of steps
      /// </summary>
      [ProtoMember(20)]
      public int RawFramesTotal { get; set; }

      #endregion

      #region Frame (UnitFrame) Data Variables

      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      [ProtoMember(21)]
      public int FramesObserved { get; set; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      [ProtoMember(22)]
      public UnitFrame CurrentFrameConcrete { get; set; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public IUnitFrame CurrentFrame
      {
         get { return CurrentFrameConcrete; }
      }

      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [ProtoMember(23)]
      public Dictionary<int, UnitFrame> UnitFrames { get; private set; }

      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      [ProtoMember(24)]
      public string CoreID { get; set; }

      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      /// <exception cref="ArgumentNullException">Throws if 'frame' is null.</exception>
      public void SetCurrentFrame(UnitFrame frame)
      {
         if (frame == null) throw new ArgumentNullException("frame");

         // Parse TimeStamp
         frame.TimeOfFrame = DateTime.ParseExact(frame.TimeStampString, "HH:mm:ss",
                                                 DateTimeFormatInfo.InvariantInfo,
                                                 PlatformOps.GetDateTimeStyle()).TimeOfDay;

         // Set Raw Frame Values                                       
         RawFramesComplete = frame.RawFramesComplete;
         RawFramesTotal = frame.RawFramesTotal;

         if (UnitFrames.ContainsKey(frame.FrameID) == false)
         {
            CurrentFrameConcrete = frame;
            UnitFrames.Add(frame.FrameID, frame);

            CurrentFrameConcrete.FrameDuration = TimeSpan.Zero;
            if (CurrentFrameConcrete.FrameID > 0 &&
                UnitFrames.ContainsKey(CurrentFrameConcrete.FrameID - 1) &&
                FramesObserved > 1)
            {
               CurrentFrameConcrete.FrameDuration = GetDelta(CurrentFrameConcrete.TimeOfFrame,
                                                             UnitFrames[CurrentFrameConcrete.FrameID - 1].TimeOfFrame);
            }
         }
         //else
         //{
         //   // FrameID already exists, clear the LineType
         //   logLine.LineType = LogLineType.Unknown;
         //}
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
      public IUnitFrame GetUnitFrame(int frameID)
      {
         if (UnitFrames.ContainsKey(frameID))
         {
            return UnitFrames[frameID];
         }
         
         return null;
      }
      
      #endregion
   }
}
