using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.SlotXml
{
    public class WebArtifactBuilder
    {
        public ILogger Logger { get; }
        public IPreferences Preferences { get; }
        public string Path { get; }

        public WebArtifactBuilder(ILogger logger, IPreferences preferences)
            : this(logger, preferences, System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()))
        {

        }

        public WebArtifactBuilder(ILogger logger, IPreferences preferences, string path)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences;
            Path = path;
        }

        public string Build(ICollection<SlotModel> slots)
        {
            if (slots == null) throw new ArgumentNullException(nameof(slots));

            Directory.CreateDirectory(Path);

            var copyHtml = Preferences.Get<bool>(Preference.WebGenCopyHtml);
            var copyLogs = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);

            var xmlBuilder = new XmlBuilder(Preferences);
            var result = xmlBuilder.Build(slots, Path);

            if (copyHtml)
            {
                var htmlBuilder = new HtmlBuilder(Preferences);
                htmlBuilder.Build(result, Path);
            }

            if (copyLogs)
            {
                CopyLogs(slots);
            }

            return Path;
        }

        private void CopyLogs(IEnumerable<SlotModel> slots)
        {
            var logCache = Preferences.Get<string>(Preference.CacheDirectory);
            var logPaths = slots.Select(x => System.IO.Path.Combine(logCache, x.Settings.ClientLogFileName)).Distinct();
            int maximumLength = Preferences.Get<bool>(Preference.WebGenLimitLogSize)
                ? Preferences.Get<int>(Preference.WebGenLimitLogSizeLength) * 1024
                : -1;

            foreach (var path in logPaths.Where(File.Exists))
            {
                using (var readStream = Internal.FileSystem.TryFileOpen(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (readStream == null)
                    {
                        Logger.Warn($"Could not open {path} for web generation.");
                        continue;
                    }

                    if (maximumLength >= 0 && readStream.Length > maximumLength)
                    {
                        readStream.Position = readStream.Length - maximumLength;
                    }

                    using (var writeStream = new FileStream(System.IO.Path.Combine(Path, System.IO.Path.GetFileName(path)), FileMode.Create))
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }
    }
}
