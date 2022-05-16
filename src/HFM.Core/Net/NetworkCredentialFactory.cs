using System.Net;

namespace HFM.Core.Net
{
    public static class NetworkCredentialFactory
    {
        /// <summary>
        /// Creates and returns a new NetworkCredential object.
        /// </summary>
        /// <param name="username">The username literal or in domain\username format.</param>
        /// <param name="password">The password literal.</param>
        public static NetworkCredential Create(string username, string password)
        {
            if (ValidateRequired(username, password, out _))
            {
                if (username.Contains('\\'))
                {
                    string[] domainAndUsername = username.Split('\\');
                    string domain = domainAndUsername[0];
                    username = domainAndUsername[1];
                    return new NetworkCredential(username, password, domain);
                }

                return new NetworkCredential(username, password);
            }

            return null;
        }

        public static bool ValidateRequired(string username, string password, out string message)
        {
            if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
            {
                message = "Username and password are required.";
                return false;
            }

            return ValidateInternal(username, password, out message);
        }

        public static bool ValidateOrEmpty(string username, string password, out string message)
        {
            if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
            {
                message = null;
                return true;
            }

            return ValidateInternal(username, password, out message);
        }

        private static bool ValidateInternal(string username, string password, out string message)
        {
            if (String.IsNullOrEmpty(username))
            {
                message = "Username is required when specifying Password.";
                return false;
            }

            if (String.IsNullOrEmpty(password))
            {
                message = "Password is required when specifying Username.";
                return false;
            }

            message = null;
            return true;
        }
    }
}
