/*
 * HFM.NET - Duplicate Finder Test Class
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
         var unitInfo1 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo1.ProjectIsUnknown).Return(false);
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         SetupResult.For(logic1.ProjectRunCloneGen).Return("1");
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "1";
         var unitInfo2 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo2.ProjectIsUnknown).Return(false);
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         SetupResult.For(logic2.ProjectRunCloneGen).Return("1");
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
         var unitInfo1 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo1.ProjectIsUnknown).Return(false);
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         SetupResult.For(logic1.ProjectRunCloneGen).Return("1");
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "2";
         var unitInfo2 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo2.ProjectIsUnknown).Return(false);
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         SetupResult.For(logic2.ProjectRunCloneGen).Return("2");
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
         var unitInfo1 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo1.ProjectIsUnknown).Return(false);
         var logic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic1.UnitInfoData).Return(unitInfo1);
         SetupResult.For(logic1.ProjectRunCloneGen).Return("1");
         instance1.CurrentUnitInfo = logic1;

         var instance2 = new DisplayInstance();
         instance2.UserId = "1";
         var unitInfo2 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo2.ProjectIsUnknown).Return(false);
         var logic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic2.UnitInfoData).Return(unitInfo2);
         SetupResult.For(logic2.ProjectRunCloneGen).Return("2");
         instance2.CurrentUnitInfo = logic2;

         var instance3 = new DisplayInstance();
         instance3.UserId = "2";
         var unitInfo3 = _mocks.DynamicMock<IUnitInfo>();
         SetupResult.For(unitInfo3.ProjectIsUnknown).Return(false);
         var logic3 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(logic3.UnitInfoData).Return(unitInfo3);
         SetupResult.For(logic3.ProjectRunCloneGen).Return("1");
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
