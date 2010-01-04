/*
 * HFM.NET - Unit Info Interface
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HFM.Framework
{
   public interface IUnitInfo
   {
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      string OwningInstanceName { get; }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      string OwningInstancePath { get; }

      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      DateTime UnitRetrievalTime { get; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      string FoldingID { get; set; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      Int32 Team { get; set; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType TypeOfClient { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; set; }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      bool DownloadTimeUnknown { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime PreferredDeadline { get; }

      /// <summary>
      /// Flag specifying if Preferred Deadline is Unknown
      /// </summary>
      bool PreferredDeadlineUnknown { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime FinalDeadline { get; }

      /// <summary>
      /// Flag specifying if Final Deadline is Unknown
      /// </summary>
      bool FinalDeadlineUnknown { get; }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      DateTime DueTime { get; set; }

      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      bool DueTimeUnknown { get; }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      TimeSpan UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; set; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      string CoreVersion { get; set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      Int32 ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      Int32 ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      Int32 ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      Int32 ProjectGen { get; set; }

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      bool ProjectIsUnknown { get; }

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      string ProjectRunCloneGen { get; }

      /// <summary>
      /// Name of the unit
      /// </summary>
      String ProteinName { get; set; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      string ProteinTag { get; set; }

      /// <summary>
      /// Flag specifying if Protein Tag value is Unknown
      /// </summary>
      bool ProteinTagUnknown { get; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      WorkUnitResult UnitResult { get; set; }

      /// <summary>
      /// Raw number of steps complete
      /// </summary>
      Int32 RawFramesComplete { get; set; }

      /// <summary>
      /// Raw total number of steps
      /// </summary>
      Int32 RawFramesTotal { get; set; }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      Int32 FramesComplete { get; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      Int32 PercentComplete { get; }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      TimeSpan TimePerFrame { get; }

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      Double UPD { get; }

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      Double PPD { get; }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      TimeSpan ETA { get; }

      /// <summary>
      /// Esimated Finishing Time for this unit
      /// </summary>
      TimeSpan EFT { get; }

      string WorkUnitName { get; }

      double NumAtoms { get; }

      double Credit { get; }

      /// <summary>
      /// Get the Credit of the Unit (including bonus)
      /// </summary>
      double GetCredit();

      double Frames { get; }

      string Core { get; }

      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      Int32 FramesObserved { get; }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      IUnitFrame CurrentFrame { get; }

      /// <summary>
      /// Timestamp from the last completed frame
      /// </summary>
      TimeSpan TimeOfLastFrame { get; }

      /// <summary>
      /// Last Frame ID based on UnitFrame Data
      /// </summary>
      Int32 LastUnitFrameID { get; }

      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
      IUnitFrame[] UnitFrames { get; }

      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      void SetCurrentFrame(ILogLine logLine, DateTimeStyles style);

      /// <summary>
      /// Clear the Observed Count, Current Frame Pointer, and the UnitFrames Array
      /// </summary>
      void ClearUnitFrameData();

      /// <summary>
      /// Clear the Observed Count and Current Frame Pointer
      /// </summary>
      void ClearCurrentFrame();

      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      Int32 RawTimePerUnitDownload { get; }

      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      TimeSpan TimePerUnitDownload { get; }

      /// <summary>
      /// PPD based on average frame time since unit download
      /// </summary>
      double PPDPerUnitDownload { get; }

      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      Int32 RawTimePerAllSections { get; }

      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      TimeSpan TimePerAllSections { get; }

      /// <summary>
      /// PPD based on average frame time over all sections
      /// </summary>
      double PPDPerAllSections { get; }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      Int32 RawTimePerThreeSections { get; }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      TimeSpan TimePerThreeSections { get; }

      /// <summary>
      /// PPD based on average frame time over the last three sections
      /// </summary>
      double PPDPerThreeSections { get; }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      Int32 RawTimePerLastSection { get; }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      TimeSpan TimePerLastSection { get; }

      /// <summary>
      /// PPD based on frame time of the last section
      /// </summary>
      double PPDPerLastSection { get; }

      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      Int32 RawTimePerSection { get; }

      /// <summary>
      /// Attempts to set the Protein based on the given Project data.
      /// </summary>
      /// <param name="match">Regex Match containing Project values</param>
      void DoProjectIDMatch(Match match);

      /// <summary>
      /// Attempts to set the Protein based on the given Project data.
      /// </summary>
      /// <param name="ProjectRCG">List of Project (R/C/G) values</param>
      void DoProjectIDMatch(IList<int> ProjectRCG);
   }
}