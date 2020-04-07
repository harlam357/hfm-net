
using System;
using System.Text.RegularExpressions;

namespace HFM.Core.Net
{
    public static class HostName
    {
        private const string HostNamePattern = @"^[a-z0-9\-._%]+$";
        
        /// <summary>
        /// Validates a host name or IP address.
        /// </summary>
        public static bool Validate(string hostName)
        {
            if (String.IsNullOrWhiteSpace(hostName)) return false;

            var validServer = new Regex(HostNamePattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return validServer.IsMatch(hostName);
        }
    }
}
