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
using System.Diagnostics;

using Debug=HFM.Instrumentation.Debug;

namespace HFM.Proteins
{
    public enum eClientStatus
    {
       Unknown,
       Offline,
       Stopped,
       Hung,
       Paused,
       RunningNoFrameTimes,
       Running   
    }

    public enum eClientType
    {
       Unknown,
       Standard,
       SMP,
       GPU
    }
    
    /// <summary>
    /// Contains the state of a protein in progress
    /// </summary>
    public class UnitInfo
    {
        /// <summary>
        /// 
        /// </summary>
        private eClientStatus _Status;
        /// <summary>
        /// 
        /// </summary>
        public eClientStatus Status
        {
           get { return _Status; }
           set { _Status = value; }
        }   

        /// <summary>
        /// Private member holding the name of the unit
        /// </summary>
        private String _ProteinName = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        public String ProteinName
        {
            get { return _ProteinName; }
            set { _ProteinName = value; }
        }

        /// <summary>
        /// Private member holding the download time of the unit
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
        /// Private member holding the percentage progress of the unit
        /// </summary>
        private int _PercentComplete;
        /// <summary>
        /// Current progress (percentage) of the unit
        /// </summary>
        public int PercentComplete
        {
            get { return _PercentComplete; }
            set { _PercentComplete = value; }
        }

        /// <summary>
        /// Private member holding the frame progress of the unit
        /// </summary>
        private int _FramesComplete;
        /// <summary>
        /// 
        /// </summary>
        public int FramesComplete
        {
            get { return _FramesComplete; }
            set { _FramesComplete = value; }
        }

        /// <summary>
        /// Private member holding the time per frame of the unit
        /// </summary>
        private TimeSpan _TimePerFrame;
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan TimePerFrame
        {
            get { return _TimePerFrame; }
            set { _TimePerFrame = value; }
        }

        /// <summary>
        /// Private member holding the time that the last frame completed
        /// </summary>
        private TimeSpan _TimeOfLastFrame;
        /// <summary>
        /// Time that the last frame completed
        /// </summary>
        public TimeSpan TimeOfLastFrame
        {
            get { return _TimeOfLastFrame; }
            set { _TimeOfLastFrame = value; }
        }

        /// <summary>
        /// Private variable holding the PPD rating for this instance
        /// </summary>
        private Double _PPD;
        /// <summary>
        /// PPD rating for this instance
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
        /// Private variable holding the UPD rating for this instance
        /// </summary>
        private Double _UPD;
        /// <summary>
        /// UPD rating for this instance
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
        /// 
        /// </summary>
        private Int32 _RawComplete;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private Int32 _RawTotal;
        /// <summary>
        /// 
        /// </summary>
        public Int32 RawFramesTotal
        {
            get { return _RawTotal; }
            set
            {
                _RawTotal = value;
            }
        }

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
        /// 
        /// </summary>
        private Int32 _RawTimePerThreeSections = 0;
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private Int32 _RawTimePerLastSection = 0;
        /// <summary>
        /// 
        /// </summary>
        public Int32 RawTimePerLastSection
        {
           get { return _RawTimePerLastSection; }
           set
           {
              _RawTimePerLastSection = value;
           }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _ProteinTag = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string ProteinTag
        {
            get { return _ProteinTag; }
            set { _ProteinTag = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime _DueTime;
        /// <summary>
        /// 
        /// </summary>
        public DateTime DueTime
        {
            get { return _DueTime; }
            set { _DueTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private Int32 _ProjectID;
        /// <summary>
        /// 
        /// </summary>
        public Int32 ProjectID
        {
            get { return _ProjectID; }
            set { _ProjectID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private Int32 _ProjectRun;
        /// <summary>
        /// 
        /// </summary>
        public Int32 ProjectRun
        {
            get { return _ProjectRun; }
            set { _ProjectRun = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private Int32 _ProjectClone;
        /// <summary>
        /// 
        /// </summary>
        public Int32 ProjectClone
        {
            get { return _ProjectClone; }
            set { _ProjectClone = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private Int32 _ProjectGen;
        /// <summary>
        /// 
        /// </summary>
        public Int32 ProjectGen
        {
            get { return _ProjectGen; }
            set { _ProjectGen = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private eClientType _ClientType;
        /// <summary>
        /// 
        /// </summary>
        public eClientType ClientType
        {
           get { return _ClientType; }
           set { _ClientType = value; }
        }

        /// <summary>
        /// Private variable holding the ETA
        /// </summary>
        private TimeSpan _ETA;
        /// <summary>
        /// ETA for this instance
        /// </summary>
        public TimeSpan ETA
        {
           get { return _ETA; }
           set { _ETA = value; }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //private eCoreType _CoreType;
        ///// <summary>
        ///// 
        ///// </summary>
        //public eCoreType CoreType
        //{
        //   get { return _CoreType; }
        //   set { _CoreType = value; }
        //}
        
        /// <summary>
        /// 
        /// </summary>
        private string _CoreVersion = String.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string CoreVersion
        {
           get { return _CoreVersion; }
           set { _CoreVersion = value; }
        }

        public void SetTimeBasedValues(string InstanceName)
        {
            //DateTime Start = Debug.ExecStart;

            if ((_RawTotal != 0) && (_RawComplete != 0) && (RawTimePerSection != 0))
            {
                try
                {
                    Int32 FramesTotal = ProteinCollection.Instance[ProjectID].Frames;
                    Int32 RawScaleFactor = RawFramesTotal / FramesTotal;

                    FramesComplete = RawFramesComplete / RawScaleFactor;
                    PercentComplete = FramesComplete * 100 / FramesTotal;
                    TimePerFrame = new TimeSpan(0, 0, Convert.ToInt32(RawTimePerSection));

                    UPD = 86400 / (TimePerFrame.TotalSeconds * FramesTotal);
                    PPD = Math.Round(UPD * ProteinCollection.Instance[ProjectID].Credit, 5);
                    ETA = new TimeSpan((100 - PercentComplete) * TimePerFrame.Ticks);
                }
                catch (Exception ex)
                {
                    Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
                }
            }
            
            //Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, InstanceName, Debug.GetExecTime(Start)));
            return;
        }
    }
}
