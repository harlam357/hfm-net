/*
 * HFM.NET - XML Stats Data Interface
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

namespace HFM.Framework
{
   public interface IXmlStatsData : IEquatable<IXmlStatsData>
   {
      /// <summary>
      /// Stats Last Updated
      /// </summary>
      DateTime LastUpdated { get; }

      /// <summary>
      /// User 24 Hour Points Average
      /// </summary>
      long UserTwentyFourHourAvgerage { get; }

      /// <summary>
      /// User Points Today
      /// </summary>
      long UserPointsToday { get; }

      /// <summary>
      /// User Points Week
      /// </summary>
      long UserPointsWeek { get; }

      /// <summary>
      /// User Points Total
      /// </summary>
      long UserPointsTotal { get; }

      /// <summary>
      /// User Work Units Total
      /// </summary>
      long UserWorkUnitsTotal { get; }

      /// <summary>
      /// User Points Update
      /// </summary>
      long UserPointsUpdate { get; }

      /// <summary>
      /// User Team Rank
      /// </summary>
      int UserTeamRank { get; }

      /// <summary>
      /// User Overall Rank
      /// </summary>
      int UserOverallRank { get; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      int UserChangeRankTwentyFourHours { get; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      int UserChangeRankSevenDays { get; }

      /// <summary>
      /// Team 24 Hour Points Average
      /// </summary>
      long TeamTwentyFourHourAvgerage { get; }

      /// <summary>
      /// Team Points Today
      /// </summary>
      long TeamPointsToday { get; }

      /// <summary>
      /// Team Points Week
      /// </summary>
      long TeamPointsWeek { get; }

      /// <summary>
      /// Team Points Total
      /// </summary>
      long TeamPointsTotal { get; }

      /// <summary>
      /// Team Work Units Total
      /// </summary>
      long TeamWorkUnitsTotal { get; }

      /// <summary>
      /// Team Points Update
      /// </summary>
      long TeamPointsUpdate { get; }

      /// <summary>
      /// Team Rank
      /// </summary>
      int TeamRank { get; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      int TeamChangeRankTwentyFourHours { get; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      int TeamChangeRankSevenDays { get; }
   }
}