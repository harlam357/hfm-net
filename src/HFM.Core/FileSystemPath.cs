
using System;
using System.Text.RegularExpressions;

namespace HFM.Core
{
    public static class FileSystemPath
    {
        private const string WindowsPattern = @"(?:\b[a-z]:|\\\\[a-z0-9.$_-]+\\[a-z0-9.`~!@#$%^&()_-]+)\\(?:[^\\/:*?""<>|\r\n]+\\)*";
        private const string UnixPattern = @"^(?:(/|~)[a-z0-9\-\s._~%!$&'()*+,;=:@/]*)+$";

        /// <summary>
        /// Validates a windows, UNC, or unix style path.
        /// </summary>
        public static bool Validate(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return false;

            return Regex.IsMatch(value, WindowsPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase) ||
                   Regex.IsMatch(value, UnixPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Validates a unix style path.
        /// </summary>
        public static bool ValidateUnix(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return false;
            if (value == "/") return true;

            return Regex.IsMatch(value, UnixPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
    }
}
