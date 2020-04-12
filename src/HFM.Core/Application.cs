
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HFM.Core
{
    public static class Application
    {
        public const string Name = "HFM.NET";
        
        public const string CssFolderName = "CSS";
        public const string XsltFolderName = "XSL";
        
        public const string ProjectSiteUrl = "https://github.com/harlam357/hfm-net";
        public const string SupportForumUrl = "https://groups.google.com/group/hfm-net/";

        /// <summary>
        /// Gets a string in the format Name vMajor.Minor.Build.
        /// </summary>
        public static string NameAndVersion => $"{Name} v{Version}";

        /// <summary>
        /// Gets a string in the format Name vMajor.Minor.Build.Revision
        /// </summary>
        public static string NameAndFullVersion => $"{Name} v{FullVersion}";

        private static string _Version;
        /// <summary>
        /// Gets a string in the format Major.Minor.Build.
        /// </summary>
        public static string Version => _Version ?? (_Version = CreateVersionString("{0}.{1}.{2}"));

        private static string _FullVersion;
        /// <summary>
        /// Gets a string in the format Major.Minor.Build.Revision.
        /// </summary>
        public static string FullVersion => _FullVersion ?? (_FullVersion = CreateVersionString("{0}.{1}.{2}.{3}"));

        private static string CreateVersionString(string format)
        {
            Debug.Assert(format != null);

            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return String.Format(CultureInfo.InvariantCulture, format, fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
        }

        public static int VersionNumber
        {
            get
            {
                // Example: 0.3.1.50 == 30010050 / 1.3.4.75 == 1030040075
                var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return GetVersionNumberFromArray(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
            }
        }

        /// <summary>
        /// Parses a version number from a 'x.x.x.x' or 'x.x.x' formatted string.
        /// </summary>
        /// <exception cref="ArgumentNullException">version is null.</exception>
        /// <exception cref="FormatException">Throws when given version cannot be parsed.</exception>
        public static int ParseVersionNumber(string version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));

            var versionNumbers = ParseVersionNumberArray(version);
            return GetVersionNumberFromArray(versionNumbers);
        }

        private static int GetVersionNumberFromArray(params int[] versionNumbers)
        {
            Debug.Assert(versionNumbers != null);

            int value = 0;

            if (versionNumbers.Length > 0)
            {
                value += versionNumbers[0] * 1_000_000_000;
            }
            if (versionNumbers.Length > 1)
            {
                value += versionNumbers[1] * 10_000_000;
            }
            if (versionNumbers.Length > 2)
            {
                value += versionNumbers[2] * 10_000;
            }
            if (versionNumbers.Length > 3)
            {
                value += versionNumbers[3];
            }

            return value;
        }

        private static int[] ParseVersionNumberArray(string version)
        {
            Debug.Assert(version != null);

            var regex = new Regex("^(?<Major>(\\d+))\\.(?<Minor>(\\d+))\\.(?<Build>(\\d+))\\.(?<Revision>(\\d+))$", RegexOptions.ExplicitCapture);
            var match = regex.Match(version);
            if (match.Success)
            {
                return new[]
                {
                    Int32.Parse(match.Groups["Major"].Value, CultureInfo.InvariantCulture),
                    Int32.Parse(match.Groups["Minor"].Value, CultureInfo.InvariantCulture),
                    Int32.Parse(match.Groups["Build"].Value, CultureInfo.InvariantCulture),
                    Int32.Parse(match.Groups["Revision"].Value, CultureInfo.InvariantCulture)
                };
            }

            regex = new Regex("^(?<Major>(\\d+))\\.(?<Minor>(\\d+))\\.(?<Build>(\\d+))$", RegexOptions.ExplicitCapture);
            match = regex.Match(version);
            if (match.Success)
            {
                return new[]
                {
                    Int32.Parse(match.Groups["Major"].Value, CultureInfo.InvariantCulture),
                    Int32.Parse(match.Groups["Minor"].Value, CultureInfo.InvariantCulture),
                    Int32.Parse(match.Groups["Build"].Value, CultureInfo.InvariantCulture)
                };
            }

            throw new FormatException(String.Format(CultureInfo.CurrentCulture,
               "Given version '{0}' is not in the correct format.", version));
        }

        public static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;

        private static string GetMonoDisplayName()
        {
            if (!IsRunningOnMono) return String.Empty;

            var monoRuntimeType = typeof(object).Assembly.GetType("Mono.Runtime");
            const BindingFlags bindings = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.ExactBinding;
            return (string)monoRuntimeType.InvokeMember("GetDisplayName", bindings, null, null, null);
        }

        /// <summary>
        /// Returns the Mono version number found in the runtime display name.
        /// </summary>
        public static Version GetMonoVersion()
        {
            return GetMonoDisplayName()
                .Split(' ')
                .Select(ParseVersionOrDefault)
                .FirstOrDefault(v => v != null);
        }

        private static Version ParseVersionOrDefault(string value)
        {
            return System.Version.TryParse(value, out var result) ? result : null;
        }
    }
}
