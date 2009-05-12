/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

namespace HFM.Proteins
{
   #region Enum
   public enum ClientType
   {
      Unknown,
      Standard,
      SMP,
      GPU
   } 
   #endregion

   /// <summary>
   /// Contains the state of a protein in progress
   /// </summary>
   public class UnitInfo
   {
      #region Public Properties and Related Private Members
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      private ClientType _TypeOfClient;
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public ClientType TypeOfClient
      {
         get { return _TypeOfClient; }
         set { _TypeOfClient = value; }
      }
      
      /// <summary>
      /// Username associated with this work unit
      /// </summary>
      private string _Username;
      /// <summary>
      /// Username associated with this work unit
      /// </summary>
      public string Username
      {
         get { return _Username; }
         set { _Username = value; }
      }

      /// <summary>
      /// Team Number associated with this work unit
      /// </summary>
      private int _Team;
      /// <summary>
      /// Team Number associated with this work unit
      /// </summary>
      public int Team
      {
         get { return _Team; }
         set { _Team = value; }
      }

      /// <summary>
      /// Core Version Number
      /// </summary>
      private string _CoreVersion = String.Empty;
      /// <summary>
      /// Core Version Number
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
         set { _CoreVersion = value; }
      }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      private DateTime _DownloadTime;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
         set { _DownloadTime = value; }
      }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      private DateTime _DueTime;
      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime
      {
         get { return _DueTime; }
         set { _DueTime = value; }
      }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      private int _FramesComplete;
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public int FramesComplete
      {
         get { return _FramesComplete; }
         set { _FramesComplete = value; }
      }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      private int _PercentComplete;
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public int PercentComplete
      {
         get { return _PercentComplete; }
         set
         {
            // Add check for valid values instead of just accepting whatever is given - Issue 2
            if (value < 0 || value > 100)
            {
               _PercentComplete = 0;
            }
            else
            {
               _PercentComplete = value;
            }
         }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      private Int32 _ProjectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      private Int32 _ProjectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get { return _ProjectRun; }
         set { _ProjectRun = value; }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      private Int32 _ProjectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get { return _ProjectClone; }
         set { _ProjectClone = value; }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      private Int32 _ProjectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get { return _ProjectGen; }
         set { _ProjectGen = value; }
      }
      
      /// <summary>
      /// Name of the unit
      /// </summary>
      private String _ProteinName = String.Empty;
      /// <summary>
      /// Name of the unit
      /// </summary>
      public String ProteinName
      {
         get { return _ProteinName; }
         set { _ProteinName = value; }
      }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      private string _ProteinTag = String.Empty;
      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      public string ProteinTag
      {
         get { return _ProteinTag; }
         set { _ProteinTag = value; }
      }

      /// <summary>
      /// Raw number of frames complete (this is not always a percent value)
      /// </summary>
      private Int32 _RawComplete;
      /// <summary>
      /// Raw number of frames complete (this is not always a percent value)
      /// </summary>
      public Int32 RawFramesComplete
      {
         get { return _RawComplete; }
         set
         {
            _RawComplete = value;
         }
      }

      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      private Int32 _RawTotal;
      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      public Int32 RawFramesTotal
      {
         get { return _RawTotal; }
         set
         {
            _RawTotal = value;
         }
      }

      /// <summary>
      /// Timestamp from log file for the last completed frame
      /// </summary>
      private TimeSpan _TimeOfLastFrame;
      /// <summary>
      /// Timestamp from log file for the last completed frame
      /// </summary>
      public TimeSpan TimeOfLastFrame
      {
         get { return _TimeOfLastFrame; }
         set { _TimeOfLastFrame = value; }
      }

      #region Time Based Values
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      private TimeSpan _TimePerFrame;
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TimePerFrame
      {
         get { return _TimePerFrame; }
         set { _TimePerFrame = value; }
      }

      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      private Double _UPD;
      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public Double UPD
      {
         get
         {
            return _UPD;
         }
         set { _UPD = value; }
      }

      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      private Double _PPD;
      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public Double PPD
      {
         get
         {
            return _PPD;
         }
         set { _PPD = value; }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      private TimeSpan _ETA;
      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public TimeSpan ETA
      {
         get { return _ETA; }
         set { _ETA = value; }
      }

      /// <summary>
      /// Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      public Int32 RawTimePerSection
      {
         get
         {
            switch (Preferences.PreferenceSet.Instance.PpdCalculation)
            {
               case Preferences.ePpdCalculation.LastFrame:
                  return _RawTimePerLastSection;
               case Preferences.ePpdCalculation.LastThreeFrames:
                  return _RawTimePerThreeSections;
            }

            return 0;
         }
      }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      private Int32 _RawTimePerThreeSections = 0;
      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      public Int32 RawTimePerThreeSections
      {
         get { return _RawTimePerThreeSections; }
         set
         {
            _RawTimePerThreeSections = value;
         }
      }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      private Int32 _RawTimePerLastSection = 0;
      /// <summary>
      /// Frame time of the last section
      /// </summary>
      public Int32 RawTimePerLastSection
      {
         get { return _RawTimePerLastSection; }
         set
         {
            _RawTimePerLastSection = value;
         }
      }
      #endregion
      
      #endregion
   }
}
