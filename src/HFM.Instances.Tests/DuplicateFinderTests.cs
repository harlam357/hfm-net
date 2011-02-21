/*
 * HFM.NET - Duplicate Finder Test Class
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class DuplicateFinderTests
   {
      private MockRepository _mocks;
      
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
      }
   
      [Test]
      public void DuplicateTestWithDuplicates()
      {
         var instance1 = new DisplayInstance();
         instance1.UserId = "1";
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "1";
         var unitInfo2 = new UnitInfo { ProjectID = 1 };
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         instance2.CurrentUnitInfo = logic2;
         
         _mocks.ReplayAll();
         
         DuplicateFinder.FindDuplicates(new[] { instance1, instance2 });
         
         Assert.IsTrue(instance1.UserIdIsDuplicate);
         Assert.IsTrue(instance1.ProjectIsDuplicate);
         Assert.IsTrue(instance2.UserIdIsDuplicate);
         Assert.IsTrue(instance2.ProjectIsDuplicate);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void DuplicateTestNoDuplicates()
      {
         var instance1 = new DisplayInstance();
         instance1.UserId = "1";
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "2";
         var unitInfo2 = new UnitInfo { ProjectID = 2 };
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         instance2.CurrentUnitInfo = logic2;

         _mocks.ReplayAll();

         DuplicateFinder.FindDuplicates(new[] { instance1, instance2 });

         Assert.IsFalse(instance1.UserIdIsDuplicate);
         Assert.IsFalse(instance1.ProjectIsDuplicate);
         Assert.IsFalse(instance2.UserIdIsDuplicate);
         Assert.IsFalse(instance2.ProjectIsDuplicate);

         _mocks.VerifyAll();
      }

      [Test]
      public void DuplicateTestMixed()
      {
         var instance1 = new DisplayInstance();
         instance1.UserId = "1";
         var unitInfo1 = new UnitInfo { ProjectID = 1 };
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "1";
         var unitInfo2 = new UnitInfo { ProjectID = 2 };
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         instance2.CurrentUnitInfo = logic2;

         var instance3 = new DisplayInstance();
         instance3.UserId = "2";
         var unitInfo3 = new UnitInfo { ProjectID = 1 };
         var logic3 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic3.UnitInfoData).Return(unitInfo3);
         instance3.CurrentUnitInfo = logic3;

         _mocks.ReplayAll();

         DuplicateFinder.FindDuplicates(new[] { instance1, instance2, instance3 });

         Assert.IsTrue(instance1.UserIdIsDuplicate);
         Assert.IsTrue(instance1.ProjectIsDuplicate);
         Assert.IsTrue(instance2.UserIdIsDuplicate);
         Assert.IsFalse(instance2.ProjectIsDuplicate);
         Assert.IsFalse(instance3.UserIdIsDuplicate);
         Assert.IsTrue(instance3.ProjectIsDuplicate);

         _mocks.VerifyAll();
      }
   }
}
