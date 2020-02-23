
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
            if (Validate.UsernamePasswordPair(username, password))
            {
                if (username.Contains("\\"))
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
    }
}