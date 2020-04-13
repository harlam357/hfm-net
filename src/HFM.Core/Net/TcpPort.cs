
using System;

namespace HFM.Core.Net
{
    public static class TcpPort
    {
        public static bool Validate(int port)
        {
            return port > 0 && port < UInt16.MaxValue;
        }
    }
}
