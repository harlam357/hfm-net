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
using System.Linq;
using System.Runtime.Serialization;

using HFM.Core.Client;

namespace HFM.Core.WorkUnits
{
    [Serializable]
    [DataContract]
    public sealed class ProteinBenchmark
    {
        private const int DefaultMaxFrames = 300;

        private static readonly object FrameTimesListLock = new object();

        public SlotIdentifier SlotIdentifier => new SlotIdentifier(ClientIdentifier.FromPath(SourceName, SourcePath, SourceGuid), SourceSlotID);

        /// <summary>
        /// Gets or sets the source client name.
        /// </summary>
        [DataMember(Order = 1)]
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the source client path (physical path or host name and port).
        /// </summary>
        [DataMember(Order = 2)]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the source client unique identifier.
        /// </summary>
        [DataMember(Order = 7)]
        public Guid SourceGuid { get; set; }

        /// <summary>
        /// Gets or sets the source client slot ID number.
        /// </summary>
        [DataMember(Order = 6, IsRequired = true)]
        public int SourceSlotID { get; set; }

        [DataMember(Order = 3)]
        public int ProjectID { get; set; }

        [DataMember(Order = 4)]
        public TimeSpan MinimumFrameTime { get; set; }

        public TimeSpan AverageFrameTime
        {
            get
            {
                if (FrameTimes.Count <= 0) return TimeSpan.Zero;

                TimeSpan totalTime = TimeSpan.Zero;
                lock (FrameTimesListLock)
                {
                    totalTime = FrameTimes.Aggregate(totalTime, (current, frameTime) => current.Add(frameTime.Duration));
                }
                // ReSharper disable once PossibleLossOfFraction
                return TimeSpan.FromSeconds(Convert.ToInt32(totalTime.TotalSeconds) / FrameTimes.Count);
            }
        }

        [DataMember(Order = 5)]
        public List<ProteinBenchmarkFrameTime> FrameTimes { get; set; }

        public ProteinBenchmark()
        {
            SourceSlotID = SlotIdentifier.NoSlotID;
            MinimumFrameTime = TimeSpan.Zero;
            FrameTimes = new List<ProteinBenchmarkFrameTime>(DefaultMaxFrames);
        }

        public void UpdateFromSlotIdentifier(SlotIdentifier slotIdentifier)
        {
            SourceName = slotIdentifier.Client.Name;
            SourcePath = slotIdentifier.Client.ToPath();
            SourceGuid = slotIdentifier.Client.Guid;
            SourceSlotID = slotIdentifier.SlotID;
        }

        public static ProteinBenchmark FromSlotIdentifier(SlotIdentifier slotIdentifier)
        {
            return new ProteinBenchmark
            {
                SourceName = slotIdentifier.Client.Name,
                SourcePath = slotIdentifier.Client.ToPath(),
                SourceGuid = slotIdentifier.Client.Guid,
                SourceSlotID = slotIdentifier.SlotID
            };
        }

        public void AddFrameTime(TimeSpan frameTime)
        {
            if (frameTime <= TimeSpan.Zero) return;

            if (frameTime < MinimumFrameTime || MinimumFrameTime.Equals(TimeSpan.Zero))
            {
                MinimumFrameTime = frameTime;
            }

            lock (FrameTimesListLock)
            {
                // Dequeue once we have the Maximum number of frame times
                if (FrameTimes.Count == DefaultMaxFrames)
                {
                    FrameTimes.RemoveAt(DefaultMaxFrames - 1);
                }
                FrameTimes.Insert(0, new ProteinBenchmarkFrameTime { Duration = frameTime });
            }
        }

        /// <summary>
        /// Updates the <see cref="MinimumFrameTime"/> property value based on the current <see cref="FrameTimes"/> collection.
        /// </summary>
        public void UpdateMinimumFrameTime()
        {
            TimeSpan minimumFrameTime = TimeSpan.Zero;
            lock (FrameTimesListLock)
            {
                foreach (ProteinBenchmarkFrameTime frameTime in FrameTimes)
                {
                    if (frameTime.Duration < minimumFrameTime || minimumFrameTime.Equals(TimeSpan.Zero))
                    {
                        minimumFrameTime = frameTime.Duration;
                    }
                }
            }

            if (minimumFrameTime.Equals(TimeSpan.Zero) == false)
            {
                MinimumFrameTime = minimumFrameTime;
            }
        }
    }
}