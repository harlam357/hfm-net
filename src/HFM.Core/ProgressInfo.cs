
using System;

namespace HFM.Core
{
    /// <summary>
    /// Provides progress information data.
    /// </summary>
    public class ProgressInfo
    {
        /// <summary>
        /// Gets the task progress percentage.
        /// </summary>
        /// <returns>A percentage value indicating the task progress.</returns>
        public int ProgressPercentage { get; }

        /// <summary>
        /// Gets a message value indicating the task progress.
        /// </summary>
        /// <returns>A System.String message value indicating the task progress.</returns>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the ProgressInfo class with progress percentage and message values.
        /// </summary>
        /// <param name="progressPercentage">The progress value.</param>
        /// <param name="message">The text message value.</param>
        public ProgressInfo(int progressPercentage, string message)
        {
            ProgressPercentage = progressPercentage;
            Message = message ?? String.Empty;
        }
    }
}
