/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using Rhino.Mocks;

using HFM.Core.DataTypes;
using HFM.Core.WorkUnits;

namespace HFM.Core
{
   [TestFixture]
   public class ExtensionsTests
   {
      #region DateTime/TimeSpan

      [Test]
      public void DateTimeIsKnownTest1()
      {
         var dateTime = new DateTime();
         Assert.IsFalse(dateTime.IsKnown());
      }

      [Test]
      public void DateTimeIsKnownTest2()
      {
         var dateTime = DateTime.Now;
         Assert.IsTrue(dateTime.IsKnown());
      }

      [Test]
      public void DateTimeIsUnknownTest1()
      {
         var dateTime = new DateTime();
         Assert.IsTrue(dateTime.IsUnknown());
      }

      [Test]
      public void DateTimeIsUnknownTest2()
      {
         var dateTime = DateTime.Now;
         Assert.IsFalse(dateTime.IsUnknown());
      }

      #endregion

      #region DuplicateFinder

      [Test]
      public void DuplicateTestWithDuplicates()
      {
         var instance1 = new SlotModel { UserId = "1" };
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = CreateUnitInfoLogic(unitInfo1);
         instance1.UnitInfoModel = logic1;

         var instance2 = new SlotModel { UserId = "1" };
         var unitInfo2 = new UnitInfo { ProjectID = 1 };
         var logic2 = CreateUnitInfoLogic(unitInfo2);
         instance2.UnitInfoModel = logic2;
         
         (new[] { instance1, instance2 }).FindDuplicates();
         
         Assert.IsTrue(instance1.UserIdIsDuplicate);
         Assert.IsTrue(instance1.ProjectIsDuplicate);
         Assert.IsTrue(instance2.UserIdIsDuplicate);
         Assert.IsTrue(instance2.ProjectIsDuplicate);
      }

      [Test]
      public void DuplicateTestNoDuplicates()
      {
         var instance1 = new SlotModel { UserId = "1" };
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = CreateUnitInfoLogic(unitInfo1);
         instance1.UnitInfoModel = logic1;

         var instance2 = new SlotModel { UserId = "2" };
         var unitInfo2 = new UnitInfo { ProjectID = 2 };
         var logic2 = CreateUnitInfoLogic(unitInfo2);
         instance2.UnitInfoModel = logic2;

         (new[] { instance1, instance2 }).FindDuplicates();

         Assert.IsFalse(instance1.UserIdIsDuplicate);
         Assert.IsFalse(instance1.ProjectIsDuplicate);
         Assert.IsFalse(instance2.UserIdIsDuplicate);
         Assert.IsFalse(instance2.ProjectIsDuplicate);
      }

      [Test]
      public void DuplicateTestMixed()
      {
         var instance1 = new SlotModel { UserId = "1" };
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = CreateUnitInfoLogic(unitInfo1);
         instance1.UnitInfoModel = logic1;

         var instance2 = new SlotModel { UserId = "1" };
         var unitInfo2 = new UnitInfo { ProjectID = 2 };
         var logic2 = CreateUnitInfoLogic(unitInfo2);
         instance2.UnitInfoModel = logic2;

         var instance3 = new SlotModel { UserId = "2" };
         var unitInfo3 = new UnitInfo { ProjectID = 1 };
         var logic3 = CreateUnitInfoLogic(unitInfo3);
         instance3.UnitInfoModel = logic3;

         (new[] { instance1, instance2, instance3 }).FindDuplicates();

         Assert.IsTrue(instance1.UserIdIsDuplicate);
         Assert.IsTrue(instance1.ProjectIsDuplicate);
         Assert.IsTrue(instance2.UserIdIsDuplicate);
         Assert.IsFalse(instance2.ProjectIsDuplicate);
         Assert.IsFalse(instance3.UserIdIsDuplicate);
         Assert.IsTrue(instance3.ProjectIsDuplicate);
      }

      private static UnitInfoModel CreateUnitInfoLogic(UnitInfo unitInfo)
      {
         return new UnitInfoModel(MockRepository.GenerateStub<IProteinBenchmarkService>())
         {
            UnitInfoData = unitInfo
         };
      }

      #endregion
   }
}
