/*
 * HFM.NET - User Stats Data Container Tests
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

using NUnit.Framework;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class UserStatsDataContainerTests
   {
      [Test]
      public void TimeForNextUpdate()
      {
         Assert.IsTrue(UserStatsDataContainer.TimeForNextUpdate(DateTime.MinValue, DateTime.UtcNow, DateTime.Now.IsDaylightSavingTime()));
         Assert.IsTrue(UserStatsDataContainer.TimeForNextUpdate(DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddHours(4), false));
         Assert.IsFalse(UserStatsDataContainer.TimeForNextUpdate(DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddHours(2), false));
      }

      [Test]
      public void GetNextUpdateTime()
      {
         DateTime NowUtc = DateTime.UtcNow.Date;
      
         #region Standard Time
         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(3)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(0)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(3)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(1)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(3)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(2)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(6)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(3)), false));
                         
         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(6)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(4)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(6)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(5)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(9)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(6)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(9)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(7)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(9)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(8)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(12)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(9)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(12)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(10)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(12)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(11)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(15)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(12)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(15)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(13)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(15)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(14)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(18)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(15)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(18)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(16)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(18)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(17)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(21)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(18)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(21)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(19)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(21)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(20)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromDays(1)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(21)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromDays(1)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(22)), false));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromDays(1)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(23)), false));
         #endregion

         #region Daylight Savings Time
         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(2)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(0)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(2)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(1)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(5)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(2)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(5)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(3)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(5)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(4)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(8)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(5)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(8)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(6)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(8)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(7)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(11)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(8)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(11)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(9)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(11)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(10)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(14)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(11)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(14)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(12)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(14)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(13)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(17)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(14)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(17)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(15)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(17)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(16)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(20)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(17)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(20)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(18)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(20)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(19)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(23)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(20)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(23)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(21)), true));

         Assert.AreEqual(NowUtc.Add(TimeSpan.FromHours(23)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(22)), true));

         Assert.AreEqual(NowUtc.Add(new TimeSpan(1, 2, 0, 0)),
                         UserStatsDataContainer.GetNextUpdateTime(NowUtc.Add(TimeSpan.FromHours(23)), true));
         #endregion
      }
   }
}
