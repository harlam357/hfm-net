
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

        /// <summary>
        /// Validates a host name or IP address and TCP port.
        /// </summary>
        public static bool ValidateNameAndPort(string hostName, int port, out string message)
        {
            if (!Validate(hostName) && !TcpPort.Validate(port))
            {
                message = "Server and port are required.";
                return false;
            }

            if (!Validate(hostName))
            {
                message = "Server is required when specifying Port.";
                return false;
            }

            if (!TcpPort.Validate(port))
            {
                message = "Port is required when specifying Server.";
                return false;
            }

            message = null;
            return true;
        }
    }
}
