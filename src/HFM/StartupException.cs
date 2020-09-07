
using System;

namespace HFM
{
    [Serializable]
    public class StartupException : Exception
    {
        public StartupException()
        {
        }

        public StartupException(string message) : base(message)
        {
        }

        public StartupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StartupException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
