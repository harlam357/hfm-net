
using System;

namespace HFM.Core.Logging
{
    public enum LoggerLevel
    {
        Off,
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }

    [Serializable]
    public abstract class LoggerBase : ILogger
    {
        protected LoggerBase()
        {

        }

        protected LoggerBase(LoggerLevel loggerLevel)
        {
            Level = loggerLevel;
        }

        public LoggerLevel Level { get; set; }

        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            Log(LoggerLevel.Debug, message, null);
        }

        public void Debug(string message, Exception exception)
        {
            if (!IsDebugEnabled) return;
            Log(LoggerLevel.Debug, message, exception);
        }

        public void Info(string message)
        {
            if (!IsInfoEnabled) return;
            Log(LoggerLevel.Info, message, null);
        }

        public void Info(string message, Exception exception)
        {
            if (!IsInfoEnabled) return;
            Log(LoggerLevel.Info, message, exception);
        }

        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;
            Log(LoggerLevel.Warn, message, null);
        }

        public void Warn(string message, Exception exception)
        {
            if (!IsWarnEnabled) return;
            Log(LoggerLevel.Warn, message, exception);
        }

        public void Error(string message)
        {
            if (!IsErrorEnabled) return;
            Log(LoggerLevel.Error, message, null);
        }

        public void Error(string message, Exception exception)
        {
            if (!IsErrorEnabled) return;
            Log(LoggerLevel.Error, message, exception);
        }

        public void Fatal(string message)
        {
            if (!IsFatalEnabled) return;
            Log(LoggerLevel.Fatal, message, null);
        }

        public void Fatal(string message, Exception exception)
        {
            if (!IsFatalEnabled) return;
            Log(LoggerLevel.Fatal, message, exception);
        }

        public bool IsDebugEnabled => Level >= LoggerLevel.Debug;
        public bool IsInfoEnabled => Level >= LoggerLevel.Info;
        public bool IsWarnEnabled => Level >= LoggerLevel.Warn;
        public bool IsErrorEnabled => Level >= LoggerLevel.Error;
        public bool IsFatalEnabled => Level >= LoggerLevel.Fatal;

        protected abstract void Log(LoggerLevel loggerLevel, string message, Exception exception);
    }
}
