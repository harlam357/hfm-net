
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace HFM.Core.Internal
{
    internal static class FileSystem
    {
        internal static FileStream TryFileOpen(string path, FileMode mode, FileAccess access, FileShare share)
        {
            return TryFileOpenInternal(path, mode, access, share);
        }

        internal static FileStream TryFileOpen(string path, FileMode mode, FileAccess access, FileShare share, int sleep, int timeout)
        {
            return ExecuteFuncWithTimeout(() => TryFileOpenInternal(path, mode, access, share), sleep, timeout);
        }

        private static FileStream TryFileOpenInternal(string path, FileMode mode, FileAccess access, FileShare share, int buffer = 4096, bool isAsync = false)
        {
            try
            {
                return new FileStream(path, mode, access, share, buffer, isAsync);
            }
            catch (PathTooLongException)
            {
                throw;
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static FileStream ExecuteFuncWithTimeout(Func<FileStream> func, int sleep, int timeout)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeout)
            {
                var result = func();
                if (result != null)
                {
                    return result;
                }
                Thread.Sleep(sleep);
            }
            throw new TimeoutException($"File open timed out after {timeout}ms.");
        }

        internal static bool PathsEqual(string path1, string path2)
        {
            IEnumerable<string> path1Variations = EnumeratePathVariations(path1);
            IEnumerable<string> path2Variations = EnumeratePathVariations(path2);

            return path1Variations.Any(p1 => path2Variations.Any(p2 => String.Equals(p1, p2, StringComparison)));
        }

        private static IEnumerable<string> EnumeratePathVariations(string path)
        {
            if (path.EndsWith("\\", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                yield return path;
            }
            else
            {
                yield return String.Concat(path, "\\");
                yield return String.Concat(path, "/");
            }
        }

        private static StringComparison StringComparison =>
            Application.IsRunningOnMono
                ? StringComparison.Ordinal
                : StringComparison.OrdinalIgnoreCase;
    }
}
