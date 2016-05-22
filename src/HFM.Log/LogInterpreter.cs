/*
 * HFM.NET - Log Interpreter Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public sealed class LogInterpreter : LogInterpreterBase
   {
      #region Constructor

      public LogInterpreter(ICollection<LogLine> logLines, IList<ClientRun> clientRuns)
         : base(logLines, clientRuns)
      {

      }

      #endregion

      #region Methods

      /// <summary>
      /// Get a list of Log Lines that correspond to the given Queue Index and ProjectInfo.
      /// </summary>
      /// <param name="queueIndex">The Queue Index</param>
      /// <param name="projectInfo">Project (R/C/G) to Match</param>
      public IList<LogLine> GetLogLinesForQueueIndex(int queueIndex, IProjectInfo projectInfo)
      {
         // walk backwards through the ClientRunList and then backward
         // through the UnitIndexes list.  Find the first (really last
         // because we're itterating in reverse) UnitIndex that matches
         // the given queueIndex.
         for (int i = ClientRunList.Count - 1; i >= 0; i--)
         {
            for (int j = ClientRunList[i].UnitIndexes.Count - 1; j >= 0; j--)
            {
               // if a match is found
               if (ClientRunList[i].UnitIndexes[j].QueueIndex == queueIndex)
               {
                  int start = ClientRunList[i].UnitIndexes[j].StartIndex;
                  int end = ClientRunList[i].UnitIndexes[j].EndIndex;

                  var logLines = LogLineList.WhereLineIndex(start, end);
                  var logLinesIndexOnly = logLines.Filter(LogFilterType.UnitIndex, queueIndex);

                  var info = FirstProjectInfoOrDefault(logLinesIndexOnly);
                  if (info != null && info.EqualsProject(projectInfo))
                  {
                     return logLinesIndexOnly.ToList().AsReadOnly();
                  }

                  continue;
               }
            }
         }

         return null;
      }

      /// <summary>
      /// Return the first project info found in the log lines or null.
      /// </summary>
      private static IProjectInfo FirstProjectInfoOrDefault(IEnumerable<LogLine> logLines)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         var projectLine = logLines.FirstOrDefault(x => x.LineType.Equals(LogLineType.WorkUnitProject));
         if (projectLine == null)
         {
            return null;
         }

         var match = (Match)projectLine.LineData;
         return new ProjectInfo
         {
            ProjectID = Int32.Parse(match.Result("${ProjectNumber}")),
            ProjectRun = Int32.Parse(match.Result("${Run}")),
            ProjectClone = Int32.Parse(match.Result("${Clone}")),
            ProjectGen = Int32.Parse(match.Result("${Gen}"))
         };
      }

      #endregion
   }
}
