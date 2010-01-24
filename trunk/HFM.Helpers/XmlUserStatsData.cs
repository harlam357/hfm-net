/*
 * HFM.NET - XML Stats Data
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

using ProtoBuf;

using HFM.Framework;

namespace HFM.Helpers
{
   [ProtoContract]
   public class XmlStatsData : IXmlStatsData
   {
      #region Members
      private DateTime _LastUpdated = DateTime.MinValue;
      /// <summary>
      /// Stats Last Updated
      /// </summary>
      [ProtoMember(1)]
      public DateTime LastUpdated
      {
         get { return _LastUpdated; }
         set { _LastUpdated = value; }
      }

      private long _TwentyFourHourAvgerage;
      /// <summary>
      /// User 24 Hour Points Average
      /// </summary>
      [ProtoMember(2)]
      public long TwentyFourHourAvgerage
      {
         get { return _TwentyFourHourAvgerage; }
         set
         {
            if (_TwentyFourHourAvgerage != value)
            {
               _TwentyFourHourAvgerage = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }

      private long _PointsToday;
      /// <summary>
      /// User Points Today
      /// </summary>
      [ProtoMember(3)]
      public long PointsToday
      {
         get { return _PointsToday; }
         set
         {
            if (_PointsToday != value)
            {
               _PointsToday = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }

      private long _PointsWeek;
      /// <summary>
      /// User Points Week
      /// </summary>
      [ProtoMember(4)]
      public long PointsWeek
      {
         get { return _PointsWeek; }
         set
         {
            if (_PointsWeek != value)
            {
               _PointsWeek = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }

      private long _PointsTotal;
      /// <summary>
      /// User Points Total
      /// </summary>
      [ProtoMember(5)]
      public long PointsTotal
      {
         get { return _PointsTotal; }
         set
         {
            if (_PointsTotal != value)
            {
               _PointsTotal = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }

      private long _WorkUnitsTotal;
      /// <summary>
      /// User Work Units Total
      /// </summary>
      [ProtoMember(6)]
      public long WorkUnitsTotal
      {
         get { return _WorkUnitsTotal; }
         set
         {
            if (_WorkUnitsTotal != value)
            {
               _WorkUnitsTotal = value;
               LastUpdated = DateTime.UtcNow;
            }
         }
      }
      #endregion
   }
}
