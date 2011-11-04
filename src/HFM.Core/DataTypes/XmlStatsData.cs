/*
 * HFM.NET - XML Stats Data
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

using ProtoBuf;

namespace HFM.Core.DataTypes
{
   [ProtoContract]
   public class XmlStatsData : IEquatable<XmlStatsData>
   {
      public XmlStatsData()
      {
         
      }
      
      public XmlStatsData(DateTime lastUpdated)
      {
         LastUpdated = lastUpdated;
      }
   
      #region Properties
      
      /// <summary>
      /// Stats Last Updated
      /// </summary>
      [ProtoMember(1)]
      public DateTime LastUpdated { get; private set; }

      /// <summary>
      /// User 24 Hour Points Average
      /// </summary>
      [ProtoMember(2)]
      public long UserTwentyFourHourAvgerage { get; set; }

      /// <summary>
      /// User Points Today
      /// </summary>
      [ProtoMember(3)]
      public long UserPointsToday { get; set; }

      /// <summary>
      /// User Points Week
      /// </summary>
      [ProtoMember(4)]
      public long UserPointsWeek { get; set; }

      /// <summary>
      /// User Points Total
      /// </summary>
      [ProtoMember(5)]
      public long UserPointsTotal { get; set; }

      /// <summary>
      /// User Work Units Total
      /// </summary>
      [ProtoMember(6)]
      public long UserWorkUnitsTotal { get; set; }

      /// <summary>
      /// User Points Update
      /// </summary>
      [ProtoMember(7)]
      public long UserPointsUpdate { get; set; }

      /// <summary>
      /// User Team Rank
      /// </summary>
      [ProtoMember(8)]
      public int UserTeamRank { get; set; }

      /// <summary>
      /// User Overall Rank
      /// </summary>
      [ProtoMember(9)]
      public int UserOverallRank { get; set; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      [ProtoMember(10)]
      public int UserChangeRankTwentyFourHours { get; set; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      [ProtoMember(11)]
      public int UserChangeRankSevenDays { get; set; }

      /// <summary>
      /// Team 24 Hour Points Average
      /// </summary>
      [ProtoMember(12)]
      public long TeamTwentyFourHourAvgerage { get; set; }

      /// <summary>
      /// Team Points Today
      /// </summary>
      [ProtoMember(13)]
      public long TeamPointsToday { get; set; }

      /// <summary>
      /// Team Points Week
      /// </summary>
      [ProtoMember(14)]
      public long TeamPointsWeek { get; set; }

      /// <summary>
      /// Team Points Total
      /// </summary>
      [ProtoMember(15)]
      public long TeamPointsTotal { get; set; }

      /// <summary>
      /// Team Work Units Total
      /// </summary>
      [ProtoMember(16)]
      public long TeamWorkUnitsTotal { get; set; }

      /// <summary>
      /// Team Points Update
      /// </summary>
      [ProtoMember(17)]
      public long TeamPointsUpdate { get; set; }

      /// <summary>
      /// Team Rank
      /// </summary>
      [ProtoMember(18)]
      public int TeamRank { get; set; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      [ProtoMember(19)]
      public int TeamChangeRankTwentyFourHours { get; set; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      [ProtoMember(20)]
      public int TeamChangeRankSevenDays { get; set; }
      
      #endregion

      #region IEquatable<XmlStatsData> Members

      public bool Equals(XmlStatsData other)
      {
         if (other == null) return false;

         return (UserTwentyFourHourAvgerage == other.TeamTwentyFourHourAvgerage &&
                 UserPointsToday == other.UserPointsToday &&
                 UserPointsWeek == other.UserPointsWeek &&
                 UserPointsTotal == other.UserPointsWeek &&
                 UserWorkUnitsTotal == other.UserWorkUnitsTotal &&
                 UserPointsUpdate == other.UserPointsUpdate &&
                 UserTeamRank == other.UserTeamRank &&
                 UserOverallRank == other.UserOverallRank &&
                 UserChangeRankTwentyFourHours == other.UserChangeRankTwentyFourHours &&
                 UserChangeRankSevenDays == other.UserChangeRankSevenDays &&
                 TeamTwentyFourHourAvgerage == other.TeamTwentyFourHourAvgerage &&
                 TeamPointsToday == other.TeamPointsToday &&
                 TeamPointsWeek == other.TeamPointsWeek &&
                 TeamPointsTotal == other.TeamPointsTotal &&
                 TeamWorkUnitsTotal == other.TeamWorkUnitsTotal &&
                 TeamPointsUpdate == other.TeamPointsUpdate &&
                 TeamRank == other.TeamRank &&
                 TeamChangeRankTwentyFourHours == other.TeamChangeRankTwentyFourHours &&
                 TeamChangeRankSevenDays == other.TeamChangeRankSevenDays);
      }

      #endregion
   }
}
