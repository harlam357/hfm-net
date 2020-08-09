using System;

namespace HFM.Core.Logging
{
    public interface ILoggerEvents
    {
        event EventHandler<LoggedEventArgs> Logged;
    }

    public class NullLoggerEvents : ILoggerEvents
    {
        public static NullLoggerEvents Instance { get; } = new NullLoggerEvents();

        public event EventHandler<LoggedEventArgs> Logged;
    }
}
