/*
 * HFM.NET - Legacy Log Line Parser Class
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
using System.Collections;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   internal sealed class LogLineParserLegacy : LogLineParserBase
   {
      #region Regex (Static)

      // ReSharper disable InconsistentNaming

      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex UserTeamRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex ReceivedUserIDRegex =
         new Regex("\\[(?<Timestamp>.{8})\\].*- Received User ID = (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex UserIDRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex MachineIDRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex UnitIndexRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on Unit 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex QueueIndexRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on queue slot 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /*** ProtoMol Only */
      /// <summary>
      /// Regular Expression to match ProtoMol Core Version string.
      /// </summary>
      private static readonly Regex ProtoMolCoreVersionRegex =
         new Regex("\\[(?<Timestamp>.{8})\\]   Version: (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      /*******************/

      /// <summary>
      /// Regular Expression to match Completed Work Units string.
      /// </summary>
      private static readonly Regex CompletedWorkUnitsRegex =
         new Regex("\\[(?<Timestamp>.{8})\\] \\+ Number of Units Completed: (?<Completed>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
      /// <summary>
      /// Regular Expression to match Calling Core string.  Only matches the up to the first three digits of "-np X".
      /// For this legacy detection we're only interested in knowing if this -np number exists and is greater than 1.
      /// </summary>
      private static readonly Regex WorkUnitCallingCore =
         new Regex("\\[(?<Timestamp>.{8})\\].*-np (?<Threads>\\d{1,3}).*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      // ReSharper restore InconsistentNaming

      #endregion

      #region Methods
      
      internal override object GetLineData(LogLine logLine)
      {
         var data = base.GetLineData(logLine);
         if (data != null)
         {
            return data;
         }

         switch (logLine.LineType)
         {
            case LogLineType.ClientVersion:
               int versionIndex = logLine.LineRaw.IndexOf("Version", StringComparison.Ordinal) + 8;
               if (versionIndex < logLine.LineRaw.Length)
               {
                  return logLine.LineRaw.Substring(versionIndex).Trim();
               }
               return new LogLineError(String.Format("Failed to parse Client Version value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientArguments:
               int argumentIndex = logLine.LineRaw.IndexOf("Arguments:", StringComparison.Ordinal) + 11;
               if (argumentIndex < logLine.LineRaw.Length)
               {
                  return logLine.LineRaw.Substring(argumentIndex).Trim();
               }
               return new LogLineError(String.Format("Failed to parse Arguments value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientUserNameTeam:
               Match userTeamMatch;
               if ((userTeamMatch = UserTeamRegex.Match(logLine.LineRaw)).Success)
               {
                  var list = new ArrayList(2);
                  list.Add(userTeamMatch.Result("${Username}"));
                  list.Add(Int32.Parse(userTeamMatch.Result("${TeamNumber}")));
                  return list;
               }
               return new LogLineError(String.Format("Failed to parse User Name and Team values from '{0}'", logLine.LineRaw));
            case LogLineType.ClientReceivedUserID:
               Match receivedUserIdMatch;
               if ((receivedUserIdMatch = ReceivedUserIDRegex.Match(logLine.LineRaw)).Success)
               {
                  return receivedUserIdMatch.Result("${UserID}");
               }
               return new LogLineError(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientUserID:
               Match userIdMatch;
               if ((userIdMatch = UserIDRegex.Match(logLine.LineRaw)).Success)
               {
                  return userIdMatch.Result("${UserID}");
               }
               return new LogLineError(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientMachineID:
               Match machineIdMatch;
               if ((machineIdMatch = MachineIDRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(machineIdMatch.Result("${MachineID}"));
               }
               return new LogLineError(String.Format("Failed to parse Machine ID value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitIndex:
               Match unitIndexMatch;
               if ((unitIndexMatch = UnitIndexRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(unitIndexMatch.Result("${QueueIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitQueueIndex:
               Match queueIndexMatch;
               if ((queueIndexMatch = QueueIndexRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(queueIndexMatch.Result("${QueueIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCallingCore:
               Match workUnitCallingCoreMatch;
               if ((workUnitCallingCoreMatch = WorkUnitCallingCore.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(workUnitCallingCoreMatch.Result("${Threads}"));
               }
               //return new LogLineError(String.Format("Failed to parse Work Unit Threads from '{0}'", logLine.LineRaw));
               // not an error - not all "Calling" lines will have a "-np X" in the line
               return 0; 
            case LogLineType.WorkUnitCoreVersion:
               /*** ProtoMol Only */
               Match coreVersionMatch;
               if ((coreVersionMatch = ProtoMolCoreVersionRegex.Match(logLine.LineRaw)).Success)
               {
                  return coreVersionMatch.Result("${CoreVer}");
               }
               /*******************/
               return new LogLineError(String.Format("Failed to parse Core Version value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientNumberOfUnitsCompleted:
               Match completedWorkUnitsMatch;
               if ((completedWorkUnitsMatch = CompletedWorkUnitsRegex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(completedWorkUnitsMatch.Result("${Completed}"));
               }
               return new LogLineError(String.Format("Failed to parse Units Completed value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientCoreCommunicationsError:
               return WorkUnitResult.ClientCoreError;
         }

         return null;
      }
      
      #endregion
   }
}
