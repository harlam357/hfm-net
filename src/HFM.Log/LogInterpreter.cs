/*
 * HFM.NET - Log Interpreter Class
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

using System.Collections.Generic;
using System.Linq;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public sealed class LogInterpreter : LogInterpreterBase
   {
      #region Constructor

      public LogInterpreter(IList<LogLine> logLines, IList<ClientRun> clientRuns)
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
                  int end = ClientRunList[i].UnitIndexes[j].EndIndex != -1 ? ClientRunList[i].UnitIndexes[j].EndIndex : LogLineList.Count - 1;

                  var logLines = LogLineList.WhereLineIndex(start, end);
                  var logLinesIndexOnly = logLines.Filter(LogFilterType.IndexOnly);

                  var info = logLinesIndexOnly.FirstProjectInfoOrDefault();
                  if (info != null && info.EqualsProject(projectInfo))
                  {
                     return logLines.ToList().AsReadOnly();
                  }
                  
                  continue;
               }
            }
         }

         return null;
      }

      #endregion
   }
}
