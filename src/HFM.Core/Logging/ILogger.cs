﻿
using System;

namespace HFM.Core.Logging
{
    public interface ILogger
    {
        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        void Debug(string message);

        void Debug(string message, Exception exception);

        void Error(string message);

        void Error(string message, Exception exception);

        void Info(string message);

        void Info(string message, Exception exception);

        void Warn(string message);

        void Warn(string message, Exception exception);
    }
}
