
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using HFM.Log;

namespace HFM.Core.WorkUnits
{
    public class WorkUnit : IProjectInfo
    {
        /// <summary>
        /// Returns a shallow copy of this <see cref="WorkUnit"/>.
        /// </summary>
        public WorkUnit Copy()
        {
            var u = new WorkUnit
            {
                UnitRetrievalTime = UnitRetrievalTime,
                FoldingID = FoldingID,
                Team = Team,
                Assigned = Assigned,
                Timeout = Timeout,
                UnitStartTimeStamp = UnitStartTimeStamp,
                Finished = Finished,
                CoreVersion = CoreVersion,
                ProjectID = ProjectID,
                ProjectRun = ProjectRun,
                ProjectClone = ProjectClone,
                ProjectGen = ProjectGen,
                ProteinName = ProteinName,
                ProteinTag = ProteinTag,
                UnitResult = UnitResult,
                FramesObserved = FramesObserved,
                LogLines = LogLines,
                CoreID = CoreID,
                QueueIndex = QueueIndex
            };
            return u;
        }

        #region Properties

        /// <summary>
        /// Local time the logs used to generate this WorkUnit were retrieved
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
        /// Gets or sets the work unit assigned date and time.
        /// </summary>
        public DateTime Assigned { get; set; }

        /// <summary>
        /// Gets or sets the work unit timeout date and time.
        /// </summary>
        public DateTime Timeout { get; set; }

        /// <summary>
        /// Unit Start Time Stamp
        /// </summary>
        public TimeSpan UnitStartTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the work unit finished date and time.
        /// </summary>
        public DateTime Finished { get; set; }

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
        public int QueueIndex { get; set; } = -1;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the WorkUnitFrameData for the frame ID.
        /// </summary>
        public WorkUnitFrameData GetFrameData(int frameId)
        {
            return FrameData != null && FrameData.ContainsKey(frameId) ? FrameData[frameId] : null;
        }

        internal bool EqualsProjectAndDownloadTime(WorkUnit other)
        {
            if (other == null)
            {
                return false;
            }

            // if the Projects are known
            if (this.HasProject() && other.HasProject())
            {
                // equals the Project and Assigned
                if (this.EqualsProject(other) && Assigned.Equals(other.Assigned))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
