
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace HFM.Core
{
   public static class FileSystem
   {
      public static FileStream TryFileOpen(string path, FileMode mode, FileAccess access, FileShare share)
      {
         return TryFileOpenInternal(path, mode, access, share);
      }

      public static FileStream TryFileOpen(string path, FileMode mode, FileAccess access, FileShare share, int sleep, int timeout)
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
         throw new TimeoutException(String.Format(CultureInfo.CurrentCulture, "File open timed out after {0}ms.", timeout));
      }
   }
}
