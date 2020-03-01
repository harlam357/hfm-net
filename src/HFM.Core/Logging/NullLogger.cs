
using System;

namespace HFM.Core.Logging
{
    public class NullLogger : LoggerBase
    {
        public static NullLogger Instance { get; } = new NullLogger();

        private NullLogger()
        {

        }

        protected override void Log(LoggerLevel loggerLevel, string message, Exception exception)
        {
            
        }
    }
}
