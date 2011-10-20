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

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   internal sealed class LogLineParserLegacy : LogLineParserBase
   {
      #region Regex (Static)

      // ReSharper disable InconsistentNaming

      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex rUserTeam =
         new Regex("\\[(?<Timestamp>.{8})\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rReceivedUserID =
         new Regex("\\[(?<Timestamp>.{8})\\].*- Received User ID = (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rUserID =
         new Regex("\\[(?<Timestamp>.{8})\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rMachineID =
         new Regex("\\[(?<Timestamp>.{8})\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rUnitIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on Unit 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rQueueIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on queue slot 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /*** ProtoMol Only */
      /// <summary>
      /// Regular Expression to match ProtoMol Core Version string.
      /// </summary>
      private static readonly Regex rProtoMolCoreVersion =
         new Regex("\\[(?<Timestamp>.{8})\\]   Version: (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      /*******************/

      /// <summary>
      /// Regular Expression to match Completed Work Units string.
      /// </summary>
      private static readonly Regex rCompletedWUs =
         new Regex("\\[(?<Timestamp>.{8})\\] \\+ Number of Units Completed: (?<Completed>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      // ReSharper restore InconsistentNaming

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
               Match mUserTeam;
               if ((mUserTeam = rUserTeam.Match(logLine.LineRaw)).Success)
               {
                  var list = new ArrayList(2);
                  list.Add(mUserTeam.Result("${Username}"));
                  list.Add(Int32.Parse(mUserTeam.Result("${TeamNumber}")));
                  return list;
               }
               return new LogLineError(String.Format("Failed to parse User Name and Team values from '{0}'", logLine.LineRaw));
            case LogLineType.ClientReceivedUserID:
               Match receivedUserId;
               if ((receivedUserId = rReceivedUserID.Match(logLine.LineRaw)).Success)
               {
                  return receivedUserId.Result("${UserID}");
               }
               return new LogLineError(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientUserID:
               Match userId;
               if ((userId = rUserID.Match(logLine.LineRaw)).Success)
               {
                  return userId.Result("${UserID}");
               }
               return new LogLineError(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientMachineID:
               Match machineId;
               if ((machineId = rMachineID.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(machineId.Result("${MachineID}"));
               }
               return new LogLineError(String.Format("Failed to parse Machine ID value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitIndex:
               Match mUnitIndex;
               if ((mUnitIndex = rUnitIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mUnitIndex.Result("${QueueIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitQueueIndex:
               Match mQueueIndex;
               if ((mQueueIndex = rQueueIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mQueueIndex.Result("${QueueIndex}"));
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreVersion:
               /*** ProtoMol Only */
               Match mCoreVer;
               if ((mCoreVer = rProtoMolCoreVersion.Match(logLine.LineRaw)).Success)
               {
                  return mCoreVer.Result("${CoreVer}");
               }
               /*******************/
               return new LogLineError(String.Format("Failed to parse Core Version value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientNumberOfUnitsCompleted:
               Match mCompletedWUs;
               if ((mCompletedWUs = rCompletedWUs.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mCompletedWUs.Result("${Completed}"));
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
