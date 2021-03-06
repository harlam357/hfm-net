﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace HFM.Core.Logging
{
    [ExcludeFromCodeCoverage]
    public class Logger : LoggerBase, ILoggerEvents
    {
        public const string NameFormat = "({0}) {1}";

        public Logger(string path)
        {
            try
            {
                Initialize(path);
            }
            catch (IOException ex)
            {
                string message = String.Format(CultureInfo.CurrentCulture,
                    "Logging failed to initialize.  Please check to be sure that the {0} or {1} file is not open or otherwise in use.",
                    LogFileName, PreviousLogFileName);
                throw new InvalidOperationException(message, ex);
            }
            // default level
            Level = LoggerLevel.Info;
        }

        private static readonly object _LogLock = new object();

        protected override void Log(LoggerLevel loggerLevel, string message, Exception exception)
        {
            if (loggerLevel <= Level)
            {
                lock (_LogLock)
                {
                    var lines = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => FormatMessage(loggerLevel, x)).ToList();
                    OnLogged(new LoggedEventArgs(lines));
                    foreach (var line in lines)
                    {
                        Trace.WriteLine(line);
                    }

                    if (exception != null)
                    {
                        Trace.WriteLine(FormatMessage(loggerLevel, exception.ToString()));
                    }
                }
            }
        }

        private static string FormatMessage(LoggerLevel loggerLevel, string message)
        {
            string messageIdentifier = String.Empty;

            switch (loggerLevel)
            {
                case LoggerLevel.Off:
                    messageIdentifier = " ";
                    break;
                case LoggerLevel.Error:
                    messageIdentifier = "X";
                    break;
                case LoggerLevel.Warn:
                    messageIdentifier = "!";
                    break;
                case LoggerLevel.Info:
                    messageIdentifier = "-";
                    break;
                case LoggerLevel.Debug:
                    messageIdentifier = "+";
                    break;
            }

            DateTime dateTime = DateTime.Now;
            return $"[{dateTime.ToShortDateString()}-{dateTime.ToLongTimeString()}] {messageIdentifier} {message}";
        }

        public event EventHandler<LoggedEventArgs> Logged;

        private void OnLogged(LoggedEventArgs e)
        {
            Logged?.Invoke(this, e);
        }

        public const string LogFileName = "HFM.log";
        public const string PreviousLogFileName = "HFM-prev.log";

        private static void Initialize(string path)
        {
            // ensure the path exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string logFilePath = Path.Combine(path, LogFileName);
            string prevLogFilePath = Path.Combine(path, PreviousLogFileName);

            var fi = new FileInfo(logFilePath);
            if (fi.Exists && fi.Length > 512000)
            {
                var fi2 = new FileInfo(prevLogFilePath);
                if (fi2.Exists)
                {
                    fi2.Delete();
                }
                fi.MoveTo(prevLogFilePath);
            }

            Trace.Listeners.Add(new TextWriterTraceListener(logFilePath));
            Trace.AutoFlush = true;
        }
    }

    [ExcludeFromCodeCoverage]
    public class LoggedEventArgs : EventArgs
    {
        public ICollection<string> Messages { get; }

        public LoggedEventArgs(ICollection<string> messages)
        {
            Messages = messages;
        }
    }
}
