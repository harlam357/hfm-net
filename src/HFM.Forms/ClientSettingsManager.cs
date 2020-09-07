using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.Serializers;

namespace HFM.Forms
{
    public class ClientSettingsManager
    {
        /// <summary>
        /// Current File Name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Current Filter Index
        /// </summary>
        public int FilterIndex { get; private set; }

        /// <summary>
        /// Current Config File Extension or the Default File Extension
        /// </summary>
        public string FileExtension
        {
            get
            {
                if (!String.IsNullOrEmpty(FileName))
                {
                    return Path.GetExtension(FileName);
                }

                return _serializers[FilterIndex - 1].FileExtension;
            }
        }

        public string FileTypeFilters => _serializers.GetFileTypeFilters();

        private readonly List<IFileSerializer<List<ClientSettings>>> _serializers;

        public ILogger Logger { get; }

        public ClientSettingsManager(ILogger logger)
        {
            Logger = logger ?? NullLogger.Instance;
            _serializers = new List<IFileSerializer<List<ClientSettings>>>
            {
                new ClientSettingsFileSerializer(Logger)
            };

            ClearFileName();
        }

        /// <summary>
        /// Clear the Configuration Filename
        /// </summary>
        public void ClearFileName()
        {
            FilterIndex = 1;
            FileName = String.Empty;
        }

        /// <summary>
        /// Reads a collection of client settings from a file.
        /// </summary>
        /// <param name="path">Path to config file.</param>
        /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
        public virtual IEnumerable<ClientSettings> Read(string path, int filterIndex)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (filterIndex < 1 || filterIndex > _serializers.Count) throw new ArgumentOutOfRangeException(nameof(filterIndex));

            var serializer = _serializers[filterIndex - 1];
            List<ClientSettings> settings = serializer.Deserialize(path);

            if (settings.Count != 0)
            {
                // update the serializer index only if something was loaded
                FilterIndex = filterIndex;
                FileName = path;
            }

            return settings;
        }

        /// <summary>
        /// Writes a collection of client settings to a file.
        /// </summary>
        /// <param name="settings">Settings collection.</param>
        /// <param name="path">Path to config file.</param>
        /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
        public virtual void Write(IEnumerable<ClientSettings> settings, string path, int filterIndex)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (filterIndex < 1 || filterIndex > _serializers.Count) throw new ArgumentOutOfRangeException(nameof(filterIndex));

            var serializer = _serializers[filterIndex - 1];
            serializer.Serialize(path, settings.ToList());

            FilterIndex = filterIndex;
            FileName = path;
        }
    }
}
