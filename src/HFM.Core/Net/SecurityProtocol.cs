
using System.Net;

namespace HFM.Core.Net
{
    public static class SecurityProtocol
    {
        public static void Setup()
        {
            // default is Tls | Ssl3, add Tls11 and Tls12 to fix issues accessing EOC stats XML
            // https://stackoverflow.com/questions/2859790/the-request-was-aborted-could-not-create-ssl-tls-secure-channel
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }
    }
}
