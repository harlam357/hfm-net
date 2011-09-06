/*
 * HFM.NET - Log Line Parser Class
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
using System.Text.RegularExpressions;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   internal sealed class LogLineParser : LogLineParserBase
   {
      #region Regex (Static)

      private static readonly Regex WorkUnitWorkingRegex =
         new Regex("(?<Timestamp>.{8}):Starting Unit (?<UnitIndex>\\d{2})", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex WorkUnitCleanUpRegex =
         new Regex("(?<Timestamp>.{8}):Cleaning up Unit (?<UnitIndex>\\d{2})", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      #endregion

      #region Methods
      
      internal override object GetLineData(ILogLine logLine)
      {
         var data = base.GetLineData(logLine);
         if (data != null)
         {
            return data;
         }

         switch (logLine.LineType)
         {
            case LogLineType.WorkUnitWorking:
               Match workUnitWorkingMatch;
               if ((workUnitWorkingMatch = WorkUnitWorkingRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitWorkingMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitRunning:
               Match workUnitRunningMatch;
               if ((workUnitRunningMatch = Extensions.WorkUnitRunningRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitRunningMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCleaningUp:
               Match workUnitCleanUpMatch;
               if ((workUnitCleanUpMatch = WorkUnitCleanUpRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitCleanUpMatch.Result("${UnitIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
         }

         return null;
      }

      #endregion
   }
}
