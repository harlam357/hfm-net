/*
 * HFM.NET - Core Logger
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.IO;

using Castle.Core.Logging;

namespace HFM.Core.Logging
{
   [CoverageExclude]
   public class Logger : LevelFilteredLogger
   {
      private readonly IPreferenceSet _prefs;

      public Logger(IPreferenceSet prefs)
         : base("Default")
      {
         _prefs = prefs;

         Initialize();
         Level = (LoggerLevel)_prefs.Get<int>(Preference.MessageLevel);
      }

      public override ILogger CreateChildLogger(string loggerName)
      {
         throw new NotImplementedException();
      }

      protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
      {
         string formattedMessage = FormatMessage(loggerLevel, message);
         if (loggerLevel <= Level)
         {
            OnTextMessage(new TextMessageEventArgs(formattedMessage));

            if (exception == null)
            {
               Trace.WriteLine(formattedMessage);
            }
            else
            {
               Trace.WriteLine(exception.ToString());
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
            case LoggerLevel.Fatal:
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
         return String.Format("[{0}-{1}] {2} {3}", dateTime.ToShortDateString(), dateTime.ToLongTimeString(), messageIdentifier, message);
      }

      #region Text Message Event

      public event EventHandler<TextMessageEventArgs> TextMessage;

      private void OnTextMessage(TextMessageEventArgs e)
      {
         if (TextMessage != null)
         {
            TextMessage(this, e);
         }
      }

      #endregion

      void Initialize()
      {
         // Ensure the HFM User Application Data Folder Exists
         var path = _prefs.ApplicationDataFolderPath;
         if (Directory.Exists(path) == false)
         {
            Directory.CreateDirectory(path);
         }

         string logFilePath = Path.Combine(path, Constants.HfmLogFileName);
         string prevLogFilePath = Path.Combine(path, Constants.HfmPrevLogFileName);

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

         _prefs.MessageLevelChanged += delegate
         {
            var newLevel = (LoggerLevel)_prefs.Get<int>(Preference.MessageLevel);
            if (newLevel != Level)
            {
               Level = newLevel;
               Info("Debug Message Level Changed: {0}", newLevel);
            }
         };
      }
   }

   [CoverageExclude]
   public class TextMessageEventArgs : EventArgs
   {
      private readonly string _message;

      public string Message
      {
         get { return _message; }
      }

      public TextMessageEventArgs(string message)
      {
         _message = message;
      }
   }
}
