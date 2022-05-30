using System.Text;

using HFM.Preferences;

namespace HFM.Core.Client
{
    public class FahClientLogFileWriter
    {
        public IFahClient Client { get; }

        public FahClientLogFileWriter(IFahClient client)
        {
            Client = client;
        }

        public async Task WriteAsync(StringBuilder logText, FileMode mode)
        {
            const int sleep = 100;
            const int timeout = 60 * 1000;

            var cacheDirectory = Client.Preferences.Get<string>(Preference.CacheDirectory);
            string fahLogPath = Path.Combine(cacheDirectory, Client.Settings.ClientLogFileName);

            try
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                using (var stream = Core.Internal.FileSystem.TryFileOpen(fahLogPath, mode, FileAccess.Write, FileShare.Read, sleep, timeout))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var chunk in logText.GetChunks())
                    {
                        await writer.WriteAsync(chunk).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Client.Logger.Warn($"Failed to write to {fahLogPath}", ex);
            }
        }
    }
}
