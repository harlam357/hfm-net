/*
 * HFM.NET - Unit Info Logic Interface
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

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IUnitInfoLogic
   {
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      UnitInfo UnitInfoData { get; }
   
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime PreferredDeadline { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime FinalDeadline { get; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; }

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      IProtein CurrentProtein { get; }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      int FramesComplete { get; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      int PercentComplete { get; }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      TimeSpan GetFrameTime(PpdCalculationType calculationType);

      /// <summary>
      /// Work unit credit
      /// </summary>
      double GetCredit(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      double GetUPD(PpdCalculationType calculationType);

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      double GetPPD(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      TimeSpan GetEta(PpdCalculationType calculationType);

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      DateTime GetEtaDate(PpdCalculationType calculationType);

      /// <summary>
      /// Specifies if All Frames have been Completed
      /// </summary>
      bool AllFramesCompleted { get; }

      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      int GetRawTime(PpdCalculationType calculationType);

      void ShowPPDTrace(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);
   }
}
