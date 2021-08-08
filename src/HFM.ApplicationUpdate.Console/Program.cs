using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using HFM.Core;

namespace HFM.ApplicationUpdate.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var update = new Core.ApplicationUpdate
            {
                Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                UpdateDate = DateTime.UtcNow
            };
            update.UpdateFiles = EnumerateUpdateFiles(update.Version, args).ToList();

            using var stream = File.Create("ApplicationUpdate.xml");
            var serializer = new ApplicationUpdateSerializer();
            serializer.Serialize(stream, update);
        }

        private static IEnumerable<ApplicationUpdateFile> EnumerateUpdateFiles(string version, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string path = args.ElementAtOrDefault(i);
                string description = args.ElementAtOrDefault(++i);

                if (path is null || description is null)
                {
                    yield break;
                }

                var fileInfo = new FileInfo(path);

                var updateFile = new ApplicationUpdateFile
                {
                    Name = fileInfo.Name,
                    Description = description,
                    HttpAddress = BuildGitHubHttpAddress(version, fileInfo.Name),
                    Size = (int)fileInfo.Length,
                    UpdateType = UpdateTypeFromExtension(fileInfo.Extension)
                };

                using (var stream = fileInfo.OpenRead())
                {
                    updateFile.MD5 = ApplicationUpdateFile.CalculateHash(stream, HashProvider.MD5);
                }
                using (var stream = fileInfo.OpenRead())
                {
                    updateFile.SHA1 = ApplicationUpdateFile.CalculateHash(stream, HashProvider.SHA1);
                }

                yield return updateFile;
            }
        }

        private static string BuildGitHubHttpAddress(string version, string fileName) =>
            $"https://github.com/harlam357/hfm-net/releases/download/v{ToMajorMinorPatchVersion(version)}/{fileName}";

        private static Version ToMajorMinorPatchVersion(string version)
        {
            var fullVersion = new Version(version);
            return new Version(fullVersion.Major, fullVersion.Minor, fullVersion.Build);
        }

        private static int UpdateTypeFromExtension(string extension) =>
            extension switch
            {
                ".msi" => 0,
                ".zip" => 1,
                _ => throw new ArgumentException("Extension is unknown.", nameof(extension))
            };
    }
}
