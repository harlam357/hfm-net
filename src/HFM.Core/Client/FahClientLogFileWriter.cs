using System.Text;

namespace HFM.Core.Client;

public interface IFahClientLogFileWriter
{
    Task WriteAsync(string path, FileMode mode, StringBuilder logText);
}

public class FahClientLogFileWriter : IFahClientLogFileWriter
{
    public async Task WriteAsync(string path, FileMode mode, StringBuilder logText)
    {
        const int sleep = 100;
        const int timeout = 60 * 1000;

        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var stream = Core.Internal.FileSystem.TryFileOpen(path, mode, FileAccess.Write, FileShare.Read, sleep, timeout))
        using (var writer = new StreamWriter(stream))
        {
            foreach (var chunk in logText.GetChunks())
            {
                await writer.WriteAsync(chunk).ConfigureAwait(false);
            }
        }
    }
}

public class NullFahClientLogFileWriter : IFahClientLogFileWriter
{
    public static NullFahClientLogFileWriter Instance { get; } = new NullFahClientLogFileWriter();

    public Task WriteAsync(string path, FileMode mode, StringBuilder logText) => Task.CompletedTask;
}
