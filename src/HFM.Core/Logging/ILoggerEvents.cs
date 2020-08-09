using System;

namespace HFM.Core.Logging
{
    public interface ILoggerEvents
    {
        event EventHandler<LoggedEventArgs> Logged;
    }
}
