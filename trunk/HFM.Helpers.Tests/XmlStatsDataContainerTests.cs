/*
 * HFM.NET - XML Stats Data Container Tests
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

using NUnit.Framework;

using HFM.Framework;

namespace HFM.Helpers.Tests
{
   [TestFixture]
   public class XmlStatsDataContainerTests
   {
      XmlStatsData data;

      [SetUp]
      public void Init()
      {
         data = LoadTestData();
         ValidateTestData(data);
      }

      [Test]
      public void ProtoBufSerializationTest()
      {
         XmlStatsDataContainer.Serialize(data, "XmlStatsProtoBufTest.dat");

         XmlStatsData data2 = XmlStatsDataContainer.Deserialize("XmlStatsProtoBufTest.dat");
         ValidateTestData(data2);
      }

      [Test]
      public void ProtoBufDeserializeFileNotFoundTest()
      {
         XmlStatsData testData = XmlStatsDataContainer.Deserialize("FileNotFound.dat");
         Assert.IsNull(testData);
      }

      private static XmlStatsData LoadTestData()
      {
         XmlStatsData testData = new XmlStatsData();
         testData.UserTwentyFourHourAvgerage = 36123;
         testData.UserPointsToday = 5675;
         testData.UserPointsWeek = 256176;
         testData.UserPointsTotal = 11222333;
         testData.UserWorkUnitsTotal = 50987;
         
         return testData;
      }

      private static void ValidateTestData(IXmlStatsData data)
      {
         Assert.AreEqual(36123, data.UserTwentyFourHourAvgerage);
         Assert.AreEqual(5675, data.UserPointsToday);
         Assert.AreEqual(256176, data.UserPointsWeek);
         Assert.AreEqual(11222333, data.UserPointsTotal);
         Assert.AreEqual(50987, data.UserWorkUnitsTotal);
      }
   }
}
