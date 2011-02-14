/*
 * HFM.NET - Work Unit History Database Tests
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
using System.Collections.Generic;
using System.IO;
using System.Threading;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class UnitInfoDatabaseTests
   {
      private const string TestFile = "Test.db3";
      private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";

      private MockRepository _mocks;
      private IProteinCollection _proteinCollection;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _proteinCollection = SetupMockProteinCollection(_mocks);
      }
   
      [Test]
      public void WriteUnitInfoTest1()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }
      
         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = 1;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
         unitInfo.OwningInstanceName = "Owner";
         unitInfo.OwningInstancePath = "Path";
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = "2.09";
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;
         unitInfo.DownloadTime = new DateTime(2010, 1, 1);
         unitInfo.FinishedTime = new DateTime(2010, 1, 2);
         
         var unitInfoLogic = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic.UnitInfoData).Return(unitInfo);
         SetupResult.For(unitInfoLogic.FramesComplete).Return(100);
         SetupResult.For(unitInfoLogic.RawTimePerAllSections).Return(600);

         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
         HistoryEntry entry = rows[0];
         Assert.AreEqual(2669, entry.ProjectID);
         Assert.AreEqual(1, entry.ProjectRun);
         Assert.AreEqual(2, entry.ProjectClone);
         Assert.AreEqual(3, entry.ProjectGen);
         Assert.AreEqual("Owner", entry.InstanceName);
         Assert.AreEqual("Path", entry.InstancePath);
         Assert.AreEqual("harlam357", entry.Username);
         Assert.AreEqual(32, entry.Team);
         Assert.AreEqual(2.09f, entry.CoreVersion);
         Assert.AreEqual(100, entry.FramesCompleted);
         Assert.AreEqual(TimeSpan.FromSeconds(600), entry.FrameTime);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, entry.Result);
         Assert.AreEqual(new DateTime(2010, 1, 1), entry.DownloadDateTime);
         Assert.AreEqual(new DateTime(2010, 1, 2), entry.CompletionDateTime);
         
         // test code to ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);
         // verify
         rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);

         _mocks.VerifyAll();
      }

      [Test]
      public void WriteUnitInfoTest2()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 6900;
         unitInfo.ProjectRun = 4;
         unitInfo.ProjectClone = 5;
         unitInfo.ProjectGen = 6;
         unitInfo.OwningInstanceName = "Owner's";
         unitInfo.OwningInstancePath = "The Path's";
         unitInfo.FoldingID = "harlam357's";
         unitInfo.Team = 100;
         unitInfo.CoreVersion = "2.27";
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2009, 5, 5);
         unitInfo.FinishedTime = DateTime.MinValue;

         var unitInfoLogic = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic.UnitInfoData).Return(unitInfo);
         SetupResult.For(unitInfoLogic.FramesComplete).Return(56);
         SetupResult.For(unitInfoLogic.RawTimePerAllSections).Return(1000);

         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
         HistoryEntry entry = rows[0];
         Assert.AreEqual(6900, entry.ProjectID);
         Assert.AreEqual(4, entry.ProjectRun);
         Assert.AreEqual(5, entry.ProjectClone);
         Assert.AreEqual(6, entry.ProjectGen);
         Assert.AreEqual("Owner's", entry.InstanceName);
         Assert.AreEqual("The Path's", entry.InstancePath);
         Assert.AreEqual("harlam357's", entry.Username);
         Assert.AreEqual(100, entry.Team);
         Assert.AreEqual(2.27f, entry.CoreVersion);
         Assert.AreEqual(56, entry.FramesCompleted);
         Assert.AreEqual(TimeSpan.FromSeconds(1000), entry.FrameTime);
         Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, entry.Result);
         Assert.AreEqual(new DateTime(2009, 5, 5), entry.DownloadDateTime);
         Assert.AreEqual(DateTime.MinValue, entry.CompletionDateTime);

         // test code to ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);
         // verify
         rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);

         _mocks.VerifyAll();
      }

      [Test]
      public void WriteUnitInfoEarlyUnitEndTest()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2670;
         unitInfo.ProjectRun = 2;
         unitInfo.ProjectClone = 3;
         unitInfo.ProjectGen = 4;
         unitInfo.OwningInstanceName = "Owner";
         unitInfo.OwningInstancePath = "Path";
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = "2.09";
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2010, 2, 2);
         unitInfo.FinishedTime = DateTime.MinValue;

         var unitInfoLogic = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic.UnitInfoData).Return(unitInfo);
         SetupResult.For(unitInfoLogic.FramesComplete).Return(100);
         SetupResult.For(unitInfoLogic.RawTimePerAllSections).Return(600);

         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
         HistoryEntry entry = rows[0];
         Assert.AreEqual(2670, entry.ProjectID);
         Assert.AreEqual(2, entry.ProjectRun);
         Assert.AreEqual(3, entry.ProjectClone);
         Assert.AreEqual(4, entry.ProjectGen);
         Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, entry.Result);
         Assert.AreEqual(new DateTime(2010, 2, 2), entry.DownloadDateTime);
         Assert.AreEqual(DateTime.MinValue, entry.CompletionDateTime);

         _mocks.VerifyAll();
      }
      
      [Test]
      public void DeleteAllUnitInfoDataTest()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         database.CreateTable(UnitInfoDatabase.WuHistoryTableName);
         Assert.AreEqual(true, database.TableExists(UnitInfoDatabase.WuHistoryTableName));
         database.DeleteAllUnitInfoData();
         Assert.AreEqual(false, database.TableExists(UnitInfoDatabase.WuHistoryTableName));
      }
      
      [Test]
      public void DeleteUnitInfoTest()
      {
         var t = new Thread(CopyTestFile);
         t.Start();
         t.Join(3000);
      
         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = Path.ChangeExtension(TestDataFile, ".dbcopy") };
         Assert.AreEqual(44, database.QueryUnitData(new QueryParameters()).Count);
         Assert.AreEqual(1, database.DeleteUnitInfo(15));
         Assert.AreEqual(43, database.QueryUnitData(new QueryParameters()).Count);

         _mocks.VerifyAll();
      }
      
      private static void CopyTestFile()
      {
         string testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");

         if (File.Exists(testDataFileCopy))
         {
            File.Delete(testDataFileCopy);
            File.Copy(TestDataFile, testDataFileCopy);
         }
      }

      [Test]
      public void DeleteUnitInfoTableNotExistTest()
      {
         const string newFile = "NewFile.db3";
         
         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = newFile };
         Assert.AreEqual(0, database.DeleteUnitInfo(100));
         
         if (File.Exists(newFile))
         {
            File.Delete(newFile);
         }
      }

      [Test]
      public void DeleteUnitInfoUnitNotExistTest()
      {
         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestDataFile };
         Assert.AreEqual(0, database.DeleteUnitInfo(100));
      }

      [Test]
      public void ImportCompletedUnitsTest()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }
         
         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         var completedUnitsReader = new CompletedUnitsFileReader { CompletedUnitsFilePath = "..\\..\\TestFiles\\CompletedUnits.csv" };
         completedUnitsReader.Process();
         database.ImportCompletedUnits(completedUnitsReader.Result.Entries);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(44, rows.Count);
         
         _mocks.VerifyAll();
      }
      
      [Test]
      public void QueryUnitDataTest()
      {
         _mocks.ReplayAll();
      
         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestDataFile };
         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(44, rows.Count);

         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectID, Type = QueryFieldType.Equal, Value = 6600 });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(13, rows.Count);

         parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.GreaterThanOrEqual, Value = new DateTime(2010, 8, 20) });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Count);

         parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.GreaterThan, Value = new DateTime(2010, 8, 8) });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.LessThan, Value = new DateTime(2010, 8, 22) });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(33, rows.Count);

         parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.WorkUnitName, Type = QueryFieldType.Equal, Value = "WorkUnitName" });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(13, rows.Count);

         parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.WorkUnitName, Type = QueryFieldType.Equal, Value = "WorkUnitName2" });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Count);

         parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.Atoms, Type = QueryFieldType.GreaterThan, Value = 5000});
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.Atoms, Type = QueryFieldType.LessThanOrEqual, Value = 7000 });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Count);

         parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.Core, Type = QueryFieldType.Equal, Value = "GROGPU2" });
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(16, rows.Count);
         
         _mocks.VerifyAll();
      }

      private static IProteinCollection SetupMockProteinCollection(MockRepository mocks)
      {
         var proteinCollection = mocks.DynamicMock<IProteinCollection>();
         var proteins = new List<IProtein>();
         
         var protein = mocks.DynamicMock<IProtein>();
         SetupResult.For(protein.ProjectNumber).Return(6600);
         SetupResult.For(protein.WorkUnitName).Return("WorkUnitName");
         SetupResult.For(protein.Core).Return("GROGPU2");
         SetupResult.For(protein.Credit).Return(450);
         SetupResult.For(protein.KFactor).Return(0);
         SetupResult.For(protein.Frames).Return(100);
         SetupResult.For(protein.NumAtoms).Return(5000);

         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(100).Repeat.Any();
         Expect.Call(protein.GetPPD(TimeSpan.Zero, TimeSpan.Zero)).IgnoreArguments().Return(200).Repeat.Any();
         Expect.Call(protein.GetBonusCredit(TimeSpan.Zero)).IgnoreArguments().Return(900).Repeat.Any();
         proteins.Add(protein);

         protein = mocks.DynamicMock<IProtein>();
         SetupResult.For(protein.ProjectNumber).Return(5797);
         SetupResult.For(protein.WorkUnitName).Return("WorkUnitName2");
         SetupResult.For(protein.Core).Return("GROGPU2");
         SetupResult.For(protein.Credit).Return(675);
         SetupResult.For(protein.KFactor).Return(0);
         SetupResult.For(protein.Frames).Return(100);
         SetupResult.For(protein.NumAtoms).Return(7000);

         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(300).Repeat.Any();
         Expect.Call(protein.GetPPD(TimeSpan.Zero, TimeSpan.Zero)).IgnoreArguments().Return(400).Repeat.Any();
         Expect.Call(protein.GetBonusCredit(TimeSpan.Zero)).IgnoreArguments().Return(1350).Repeat.Any();
         proteins.Add(protein);

         SetupResult.For(proteinCollection.Proteins).Return(proteins);
         return proteinCollection;
      }
   }
}
