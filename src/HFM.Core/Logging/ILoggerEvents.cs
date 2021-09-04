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

#pragma warning disable 0067
        public event EventHandler<LoggedEventArgs> Logged;
#pragma warning restore 0067
    }
}
