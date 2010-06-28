/*
 * HFM.NET - Protein Interface
 * Copyright (C) 2006 David Rawling
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

namespace HFM.Framework
{
   public interface IProtein
   {
      /// <summary>
      /// Project Number
      /// </summary>
      int ProjectNumber { get; }

      /// <summary>
      /// Server IP Address
      /// </summary>
      String ServerIP { get; }

      /// <summary>
      /// Work Unit Name
      /// </summary>
      String WorkUnitName { get; }

      /// <summary>
      /// Number of Atoms
      /// </summary>
      int NumAtoms { get; }

      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      double PreferredDays { get; }

      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      double MaxDays { get; }

      /// <summary>
      /// Work Unit Credit
      /// </summary>
      double Credit { get; }

      /// <summary>
      /// Number of Frames
      /// </summary>
      int Frames { get; }

      /// <summary>
      /// Core Identification String
      /// </summary>
      String Core { get; }

      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      String Description { get; }

      /// <summary>
      /// Project Research Contact
      /// </summary>
      String Contact { get; }

      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      double KFactor { get; }

      /// <summary>
      /// Flag Denoting if Project Number is Unknown
      /// </summary>
      bool IsUnknown { get; }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      double GetPPD(TimeSpan frameTime);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="instanceName">Calling Instance Name</param>
      double GetPPD(TimeSpan frameTime, string instanceName);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      /// <param name="instanceName">Calling Instance Name</param>
      double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit, string instanceName);

      /// <summary>
      /// Get Units Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      double GetUPD(TimeSpan frameTime);

      /// <summary>
      /// Get the Credit of the Unit (including bonus)
      /// </summary>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      double GetBonusCredit(TimeSpan estTimeOfUnit);
   }
}
