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

using HFM.Framework;

namespace HFM.Instances
{
   [Serializable]
   [ProtoContract]
   public class UnitInfo : IUnitInfo
   {
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
      private string _foldingID = Constants.FoldingIDDefault;
      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      [ProtoMember(4)]
      public string FoldingID
      {
         get { return _foldingID; }
         set { _foldingID = value; }
      }

      private Int32 _team = Constants.TeamDefault;
      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      [ProtoMember(5)]
      public Int32 Team
      {
         get { return _team; }
         set { _team = value; }
      }
      #endregion

      #region Unit Level Members
      private ClientType _typeOfClient = ClientType.Unknown;
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      [ProtoMember(6)]
      public ClientType TypeOfClient
      {
         get { return _typeOfClient; }
         set { _typeOfClient = value; }
      }

      private DateTime _downloadTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      [ProtoMember(7)]
      public DateTime DownloadTime
      {
         get { return _downloadTime; }
         set { _downloadTime = value; }
      }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      public bool DownloadTimeUnknown
      {
         get { return DownloadTime.Equals(DateTime.MinValue); }
      }

      private DateTime _dueTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      [ProtoMember(8)]
      public DateTime DueTime
      {
         get { return _dueTime; }
         set { _dueTime = value; }
      }

      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      public bool DueTimeUnknown
      {
         get { return DueTime.Equals(DateTime.MinValue); }
      }

      private TimeSpan _unitStartTime = TimeSpan.Zero;
      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      [ProtoMember(9)]
      public TimeSpan UnitStartTimeStamp
      {
         get { return _unitStartTime; }
         set { _unitStartTime = value; }
      }

      private DateTime _finishedTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      [ProtoMember(10)]
      public DateTime FinishedTime
      {
         get { return _finishedTime; }
         set { _finishedTime = value; }
      }

      private string _coreVersion = String.Empty;
      /// <summary>
      /// Core Version Number
      /// </summary>
      [ProtoMember(11)]
      public string CoreVersion
      {
         get { return _coreVersion; }
         set { _coreVersion = value; }
      }

      private Int32 _projectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      [ProtoMember(12)]
      public Int32 ProjectID
      {
         get { return _projectID; }
         set { _projectID = value; } 
      }

      private Int32 _projectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      [ProtoMember(13)]
      public Int32 ProjectRun
      {
         get { return _projectRun; }
         set { _projectRun = value; }
      }

      private Int32 _projectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      [ProtoMember(14)]
      public Int32 ProjectClone
      {
         get { return _projectClone; }
         set { _projectClone = value; }
      }

      private Int32 _projectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      [ProtoMember(15)]
      public Int32 ProjectGen
      {
         get { return _projectGen; }
         set { _projectGen = value; }
      }

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

      private String _proteinName = String.Empty;
      /// <summary>
      /// Name of the unit
      /// </summary>
      [ProtoMember(16)]
      public String ProteinName
      {
         get { return _proteinName; }
         set { _proteinName = value; }
      }

      private string _proteinTag = String.Empty;
      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      [ProtoMember(17)]
      public string ProteinTag
      {
         get { return _proteinTag; }
         set { _proteinTag = value; }
      }

      private WorkUnitResult _unitResult = WorkUnitResult.Unknown;
      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      [ProtoMember(18)]
      public WorkUnitResult UnitResult
      {
         get { return _unitResult; }
         set { _unitResult = value; }
      }
      #endregion

      #region Frames/Percent Completed Unit Level Members
      private Int32 _rawFramesComplete;
      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      [ProtoMember(19)]
      public Int32 RawFramesComplete
      {
         get { return _rawFramesComplete; }
         set
         {
            _rawFramesComplete = value;
         }
      }

      private Int32 _rawFramesTotal;
      /// <summary>
      /// Raw total number of steps
      /// </summary>
      [ProtoMember(20)]
      public Int32 RawFramesTotal
      {
         get { return _rawFramesTotal; }
         set
         {
            _rawFramesTotal = value;
         }
      }
      #endregion

      #region Frame (UnitFrame) Data Variables
      private Int32 _framesObserved;
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      [ProtoMember(21)]
      public Int32 FramesObserved
      {
         get { return _framesObserved; }
         set { _framesObserved = value; }
      }

      private UnitFrame _currentFrame;
      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      [ProtoMember(22)]
      public UnitFrame CurrentFrameConcrete
      {
         get { return _currentFrame; }
         set { _currentFrame = value; }
      }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public IUnitFrame CurrentFrame
      {
         get { return _currentFrame; }
      }

      private Dictionary<int, UnitFrame> _unitFrames = new Dictionary<int, UnitFrame>();
      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [ProtoMember(23)]
      public Dictionary<int, UnitFrame> UnitFrames
      {
         get { return _unitFrames; }
         set { _unitFrames = value; }
      }

      private string _coreId = "Unknown";
      /// <summary>
      /// Core ID (Hex) Value
      /// </summary>
      [ProtoMember(24)]
      public string CoreId
      {
         get { return _coreId; }
         set { _coreId = value; }
      }

      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      /// <exception cref="ArgumentException">Throws if 'logLine' does not have LogLineType 'WorkUnitFrame'.</exception>
      public void SetCurrentFrame(ILogLine logLine)
      {
         if (logLine.LineType.Equals(LogLineType.WorkUnitFrame) == false)
         {
            throw new ArgumentException("Argument 'logLine' must have LogLineType 'WorkUnitFrame'.");
         }

         // Check for FrameData
         var frame = logLine.LineData as IFrameData;
         if (frame == null)
         {
            // If not found, clear the LineType and get out
            logLine.LineType = LogLineType.Unknown;
            return;
         }

         // Parse TimeStamp
         DateTime timeStamp = DateTime.ParseExact(frame.TimeStampString, "HH:mm:ss",
                                                  DateTimeFormatInfo.InvariantInfo,
                                                  PlatformOps.GetDateTimeStyle());

         // Set Raw Frame Values                                       
         RawFramesComplete = frame.RawFramesComplete;
         RawFramesTotal = frame.RawFramesTotal;
         // Create new UnitFrame
         var unitFrame = new UnitFrame(frame.FrameID, timeStamp.TimeOfDay);

         if (UnitFrames.ContainsKey(frame.FrameID) == false)
         {
            CurrentFrameConcrete = unitFrame;
            UnitFrames.Add(unitFrame.FrameID, unitFrame);

            CurrentFrameConcrete.FrameDuration = TimeSpan.Zero;
            if (CurrentFrameConcrete.FrameID > 0 &&
                UnitFrames.ContainsKey(CurrentFrameConcrete.FrameID - 1) &&
                FramesObserved > 1)
            {
               CurrentFrameConcrete.FrameDuration = GetDelta(CurrentFrameConcrete.TimeOfFrame,
                                                             UnitFrames[CurrentFrameConcrete.FrameID - 1].TimeOfFrame);
            }
         }
         else
         {
            // FrameID already exists, clear the LineType
            logLine.LineType = LogLineType.Unknown;
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
      public IUnitFrame GetUnitFrame(int frameID)
      {
         if (_unitFrames.ContainsKey(frameID))
         {
            return _unitFrames[frameID];
         }
         
         return null;
      }
      #endregion
   }
}
