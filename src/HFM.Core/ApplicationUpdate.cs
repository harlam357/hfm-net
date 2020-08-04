using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using HFM.Core.Internal;

namespace HFM.Core
{
    [Serializable]
    public class ApplicationUpdate
    {
        public string Version { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<ApplicationUpdateFile> UpdateFiles { get; set; }

        public bool VersionIsGreaterThan(int versionNumber)
        {
            if (Version is null) return false;

            try
            {
                return Application.ParseVersionNumber(Version) > versionNumber;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

    public enum ApplicationUpdateFileType
    {
        Executable = 0,
        DownloadOnly = 1
    }

    [Serializable]
    public class ApplicationUpdateFile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HttpAddress { get; set; }
        public int Size { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public int UpdateType { get; set; }

        public virtual void Verify(string path)
        {
            Stream stream = null;
            try
            {
                var fileInfo = new FileInfo(path);
                if (Size != fileInfo.Length)
                {
                    throw new IOException(String.Format(CultureInfo.CurrentCulture,
                        "File length is '{0}', expected length is '{1}'.", fileInfo.Length, Size));
                }

                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                if (!String.IsNullOrEmpty(SHA1))
                {
                    using (var hash = new Hash(HashProvider.SHA1))
                    {
                        byte[] hashData = hash.Calculate(stream);
                        if (String.Compare(SHA1, hashData.ToHex(), StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw new IOException("SHA1 file hash is not correct.");
                        }
                    }
                }
                stream.Position = 0;
                if (!String.IsNullOrEmpty(MD5))
                {
                    using (var hash = new Hash(HashProvider.MD5))
                    {
                        byte[] hashData = hash.Calculate(stream);
                        if (String.Compare(MD5, hashData.ToHex(), StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            throw new IOException("MD5 file hash is not correct.");
                        }
                    }
                }
            }
            catch (Exception)
            {
                FileSystem.TryFileDelete(path);
                throw;
            }
            finally
            {
                stream?.Dispose();
            }
        }
    }
}
