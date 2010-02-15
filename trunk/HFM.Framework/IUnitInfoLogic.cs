/*
 * HFM.NET - Unit Info Logic Interface
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
using System.Diagnostics.CodeAnalysis;

namespace HFM.Framework
{
   public interface IUnitInfoLogic : IProjectInfo
   {
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      IUnitInfo UnitInfoData { get; }
   
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      string OwningInstanceName { get; set; }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      string OwningInstancePath { get; set; }

      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      DateTime UnitRetrievalTime { get; set; }

      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      string FoldingID { get; }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      Int32 Team { get; }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      ClientType TypeOfClient { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; }

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
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      TimeSpan UnitStartTimeStamp { get; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; }

      /// <summary>
      /// Core Version Number
      /// </summary>
      string CoreVersion { get; }

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
      String ProteinName { get; }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      string ProteinTag { get; }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      WorkUnitResult UnitResult { get; }

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      IProtein CurrentProtein { get; }

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
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      double GetBonusCredit();

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
   }
}