﻿using System.Diagnostics;
using System.Reflection;

using HFM.Core;
using HFM.Core.ApplicationUpdates;

namespace HFM.ApplicationUpdate.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileVersion = Version.Parse(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            fileVersion = new Version(fileVersion.Major, fileVersion.Minor, fileVersion.Build);
            var updateDate = DateTime.UtcNow;

            System.Console.WriteLine("Version: {0}", fileVersion);
            System.Console.WriteLine("UpdateDate: {0}", updateDate);

            var update = new Core.ApplicationUpdates.ApplicationUpdate
            {
                Version = fileVersion.ToString(),
                UpdateDate = updateDate
            };
            update.UpdateFiles = EnumerateUpdateFiles(update.Version, args).ToList();

            string path = Path.Combine(Environment.CurrentDirectory, "ApplicationUpdate.xml");
            System.Console.WriteLine("Writing {0}", path);

            using var stream = File.Create(path);
            var serializer = new ApplicationUpdateSerializer();
            serializer.Serialize(stream, update);
        }

        private static IEnumerable<ApplicationUpdateFile> EnumerateUpdateFiles(string version, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string path = args.ElementAtOrDefault(i);
                string description = args.ElementAtOrDefault(++i);

                System.Console.WriteLine("Adding {0} {1}", path, description);

                if (path is null || description is null)
                {
                    yield break;
                }

                var fileInfo = new FileInfo(path);

                string cleanName = CleanFileName(fileInfo.Name);
                var updateFile = new ApplicationUpdateFile
                {
                    Name = cleanName,
                    Description = description,
                    HttpAddress = BuildGitHubHttpAddress(version, cleanName),
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

        private static string CleanFileName(string fileName)
        {
            return fileName
                .Replace(' ', '.')
                .Replace("(", "")
                .Replace(")", "");
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
