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
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract]
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
      [DataMember(Order = 1)]
      public DateTime LastUpdated { get; private set; }

      /// <summary>
      /// User 24 Hour Points Average
      /// </summary>
      [DataMember(Order = 2)]
      public long UserTwentyFourHourAvgerage { get; set; }

      /// <summary>
      /// User Points Today
      /// </summary>
      [DataMember(Order = 3)]
      public long UserPointsToday { get; set; }

      /// <summary>
      /// User Points Week
      /// </summary>
      [DataMember(Order = 4)]
      public long UserPointsWeek { get; set; }

      /// <summary>
      /// User Points Total
      /// </summary>
      [DataMember(Order = 5)]
      public long UserPointsTotal { get; set; }

      /// <summary>
      /// User Work Units Total
      /// </summary>
      [DataMember(Order = 6)]
      public long UserWorkUnitsTotal { get; set; }

      /// <summary>
      /// User Points Update
      /// </summary>
      [DataMember(Order = 7)]
      public long UserPointsUpdate { get; set; }

      /// <summary>
      /// User Team Rank
      /// </summary>
      [DataMember(Order = 8)]
      public int UserTeamRank { get; set; }

      /// <summary>
      /// User Overall Rank
      /// </summary>
      [DataMember(Order = 9)]
      public int UserOverallRank { get; set; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      [DataMember(Order = 10)]
      public int UserChangeRankTwentyFourHours { get; set; }

      /// <summary>
      /// User Change Rank Twenty Four Hours
      /// </summary>
      [DataMember(Order = 11)]
      public int UserChangeRankSevenDays { get; set; }

      /// <summary>
      /// Team 24 Hour Points Average
      /// </summary>
      [DataMember(Order = 12)]
      public long TeamTwentyFourHourAvgerage { get; set; }

      /// <summary>
      /// Team Points Today
      /// </summary>
      [DataMember(Order = 13)]
      public long TeamPointsToday { get; set; }

      /// <summary>
      /// Team Points Week
      /// </summary>
      [DataMember(Order = 14)]
      public long TeamPointsWeek { get; set; }

      /// <summary>
      /// Team Points Total
      /// </summary>
      [DataMember(Order = 15)]
      public long TeamPointsTotal { get; set; }

      /// <summary>
      /// Team Work Units Total
      /// </summary>
      [DataMember(Order = 16)]
      public long TeamWorkUnitsTotal { get; set; }

      /// <summary>
      /// Team Points Update
      /// </summary>
      [DataMember(Order = 17)]
      public long TeamPointsUpdate { get; set; }

      /// <summary>
      /// Team Rank
      /// </summary>
      [DataMember(Order = 18)]
      public int TeamRank { get; set; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      [DataMember(Order = 19)]
      public int TeamChangeRankTwentyFourHours { get; set; }

      /// <summary>
      /// Team Change Rank Twenty Four Hours
      /// </summary>
      [DataMember(Order = 20)]
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
