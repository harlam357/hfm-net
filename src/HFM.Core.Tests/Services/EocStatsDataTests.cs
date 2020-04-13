
using System;

using NUnit.Framework;

namespace HFM.Core.Services
{
    [TestFixture]
    public class EocStatsDataTests
    {
        [Test]
        public void EocStatsData_GetNextUpdateTime_WhenDuringStandardTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            // this value has to be in past, a future date will generate a result that equals DateTime.MinValue.
            DateTime lastUpdated = utcNow.Date.Subtract(TimeSpan.FromDays(1));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(0)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(1)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(2)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(3)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(4)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(5)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(6)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(7)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(8)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(9)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(10)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(11)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(12)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(13)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(14)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(15)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(16)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(17)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(18)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(19)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(20)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(21)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(22)), utcNow, false));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(23)), utcNow, false));
        }

        [Test]
        public void EocStatsData_GetNextUpdateTime_WhenDuringDaylightSavingsTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            // this value has to be in past, a future date will generate a result that equals DateTime.MinValue.
            DateTime lastUpdated = utcNow.Date.Subtract(TimeSpan.FromDays(1));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(2)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(0)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(2)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(1)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(2)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(3)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(4)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(5)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(6)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(7)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(8)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(9)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(10)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(11)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(12)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(13)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(14)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(15)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(16)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(17)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(18)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(19)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(20)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(21)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(22)), utcNow, true));

            Assert.AreEqual(lastUpdated.Add(new TimeSpan(1, 2, 0, 0)),
                EocStatsData.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(23)), utcNow, true));
        }

        [Test]
        public void EocStatsData_GetNextUpdateTime_ReturnsGivenUtcNowValueWhenLastUpdatedValueIsMinValue()
        {
            DateTime utcNow = DateTime.UtcNow;

            Assert.AreEqual(utcNow,
                EocStatsData.GetNextUpdateTime(DateTime.MinValue, utcNow, false));
        }

        [Test]
        public void EocStatsData_GetNextUpdateTime_ReturnsGivenUtcNowValueWhenLastUpdatedValueIsInTheFuture()
        {
            DateTime utcNow = DateTime.UtcNow;

            Assert.AreEqual(utcNow,
                EocStatsData.GetNextUpdateTime(DateTime.MaxValue, utcNow, false));
        }
    }
}