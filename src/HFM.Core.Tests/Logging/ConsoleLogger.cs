using System;

namespace HFM.Core.Logging
{
    public class ConsoleLogger : LoggerBase
    {
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();

        public ConsoleLogger() : base(LoggerLevel.Debug)
        {

        }

        protected override void Log(LoggerLevel loggerLevel, string message, Exception exception)
        {
            Console.WriteLine("[{0}] {1}", loggerLevel, message);
            if (exception is null)
            {
                return;
            }
            Console.WriteLine("[{0}] {1}: {2} {3}", loggerLevel, exception.GetType().FullName, exception.Message, exception.StackTrace);
        }
    }
}
