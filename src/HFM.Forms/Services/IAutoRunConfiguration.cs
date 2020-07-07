
using System;

namespace HFM.Forms
{
    public interface IAutoRunConfiguration
    {
        /// <summary>
        /// Is auto run enabled?
        /// </summary>
        bool IsEnabled();

        /// <summary>
        /// Sets the HFM.NET auto run value.
        /// </summary>
        /// <param name="filePath">The file path to HFM.exe executable.</param>
        /// <exception cref="InvalidOperationException">Auto run value cannot be set.</exception>
        void SetFilePath(string filePath);
    }

    public class InMemoryAutoRunConfiguration : IAutoRunConfiguration
    {
        public bool IsEnabled()
        {
            return !String.IsNullOrEmpty(_filePath);
        }

        private string _filePath;

        public void SetFilePath(string filePath)
        {
            _filePath = filePath;
        }
    }
}