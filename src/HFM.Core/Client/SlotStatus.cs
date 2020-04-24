
using System;
using System.Drawing;

namespace HFM.Core.Client
{
    /// <summary>
    /// Represents the status of a Folding@Home client slot.
    /// </summary>
    public enum SlotStatus
    {
        // Matches HFM.Client.DataTypes.FahClientSlotStatus

        /// <summary>
        /// The status of the slot is unknown.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The slot is paused.
        /// </summary>
        Paused = 1,
        /// <summary>
        /// The slot is running.
        /// </summary>
        Running = 2,
        /// <summary>
        /// The slot is finishing.
        /// </summary>
        Finishing = 3,
        /// <summary>
        /// The slot is ready for work.
        /// </summary>
        Ready = 4,
        /// <summary>
        /// The slot is stopping.
        /// </summary>
        Stopping = 5,
        /// <summary>
        /// The slot work has failed.
        /// </summary>
        Failed = 6,
        /// <summary>
        /// The slot is running but does not have enough frame data to calculate a frame time.
        /// </summary>
        RunningNoFrameTimes = 7,
        /// <summary>
        /// The slot is offline.
        /// </summary>
        Offline = 8
    }

    public static class SlotStatusExtensions
    {
        /// <summary>
        /// Gets Status Color Object
        /// </summary>
        public static Color GetStatusColor(this SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.Running:
                    return Color.Green;
                case SlotStatus.RunningNoFrameTimes:
                    return Color.Yellow;
                case SlotStatus.Finishing:
                    return Color.Khaki;
                case SlotStatus.Ready:
                    return Color.DarkCyan;
                case SlotStatus.Stopping:
                case SlotStatus.Failed:
                    return Color.DarkRed;
                case SlotStatus.Paused:
                    return Color.Orange;
                case SlotStatus.Offline:
                    return Color.Gray;
                default:
                    return Color.Gray;
            }
        }

        public static bool IsOnline(this SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.Paused:
                case SlotStatus.Running:
                case SlotStatus.Finishing:
                case SlotStatus.Ready:
                case SlotStatus.Stopping:
                case SlotStatus.Failed:
                case SlotStatus.RunningNoFrameTimes:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsRunning(this SlotStatus status)
        {
            switch (status)
            {
                case SlotStatus.Running:
                case SlotStatus.Finishing:
                case SlotStatus.RunningNoFrameTimes:
                    return true;
                default:
                    return false;
            }
        }
    }
}