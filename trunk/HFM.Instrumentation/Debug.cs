/*
 * HFM.NET - Instrumentation Class
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

namespace HFM.Instrumentation
{
   public static class Debug
   {
      private static readonly object _lockTraceWrite = typeof(Trace);

      /// <summary>
      /// The function name of the caller function (1 stack level up)
      /// </summary>
      /// <returns>Class.Function name </returns>
      public static String FunctionName
      {
         get
         {
            StackFrame sf = new StackFrame(1, true);

            return sf.GetMethod().DeclaringType + "." + sf.GetMethod().Name;
         }
      }

      /// <summary>
      /// The function name of the parent function (2 stack levels up)
      /// </summary>
      /// <returns>Class.Function name </returns>
      public static String ParentFunctionName
      {
         get
         {
            StackFrame sf = new StackFrame(2, true);

            return sf.GetMethod().DeclaringType + "." + sf.GetMethod().Name;
         }
      }

      /// <summary>
      /// The function name of the grandparent function (3 stack levels up)
      /// </summary>
      /// <returns>Class.Function name </returns>
      public static String GParentFunctionName
      {
         get
         {
            StackFrame sf = new StackFrame(3, true);

            return sf.GetMethod().DeclaringType + "." + sf.GetMethod().Name;
         }
      }

      /// <summary>
      /// The filename in which the function can be found
      /// </summary>
      /// <returns></returns>
      public static String FileName
      {
         get
         {
            StackFrame sf = new StackFrame(1, true);

            return System.IO.Path.GetFileName(sf.GetFileName());
         }
      }

      /// <summary>
      /// Returns the execution time of the function
      /// </summary>
      /// <param name="Start">The start time as previously returned by ExecStart</param>
      /// <returns>String formatted as "#,##0 ms"</returns>
      public static String GetExecTime(DateTime Start)
      {
         TimeSpan t = DateTime.Now.Subtract(Start);

         return String.Format("{0:#,##0} ms", t.TotalMilliseconds);
      }

      /// <summary>
      /// Simple wrapper around DateTime to return current time (usable for Start Time of a function or operation)
      /// </summary>
      public static DateTime ExecStart
      {
         get
         {
            return DateTime.Now;
         }
      }

      public static void WriteToHfmConsole(string message)
      {
         lock (_lockTraceWrite)
         {
            Trace.WriteLine(FormatTraceString(TraceLevel.Off, message));
         }
      }

      public static void WriteToHfmConsole(TraceLevel level, string message)
      {
         lock (_lockTraceWrite)
         {
            if (((int) level) <= ((int) TraceLevelSwitch.GetTraceLevelSwitch().Level))
            {
               Trace.WriteLine(FormatTraceString(level, message));
            }
         }
      }

      public static void WriteToHfmConsole(TraceLevel level, string[] messages)
      {
         lock (_lockTraceWrite)
         {
            if (((int) level) <= ((int) TraceLevelSwitch.GetTraceLevelSwitch().Level))
            {
               foreach (string message in messages)
               {
                  Trace.WriteLine(FormatTraceString(level, message));
               }
            }
         }
      }
      
      public static string FormatTraceString(TraceLevel level, string message)
      {
         string messageIdentifier = String.Empty;
         
         switch (level)
         {
            case TraceLevel.Off:
               messageIdentifier = " ";
               break;
            case TraceLevel.Error:
               messageIdentifier = "X";
               break;
            case TraceLevel.Warning:
               messageIdentifier = "!";
               break;
            case TraceLevel.Info:
               messageIdentifier = "-";
               break;
            case TraceLevel.Verbose:
               messageIdentifier = "+";
               break;
         }

         return String.Format("{0} {1}", messageIdentifier, message);
      }
   }
}
