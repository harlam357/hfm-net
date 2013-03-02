/*
 * HFM.NET - Work Unit History Database Tests
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class UnitInfoDatabaseTests
   {
      private const string TestDataFilesFolder = "..\\..\\TestFiles";

      private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";
      private readonly string _testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");
      
      private const string TestData2File = "..\\..\\TestFiles\\TestData2.db3";
      private readonly string _testData2FileCopy = Path.ChangeExtension(TestData2File, ".dbcopy");

      // this file is the same as TestDataFile but has already had UpgradeWuHistory1() run on it
      private const string TestData_1File = "..\\..\\TestFiles\\TestData_1.db3";
      private readonly string _testData_1FileCopy = Path.ChangeExtension(TestData_1File, ".dbcopy");
      
      private const string TestScratchFile = "UnitInfoTest.db3";

      private UnitInfoDatabase _database;
      private readonly IProteinDictionary _proteinDictionary = CreateProteinDictionary();

      [SetUp]
      public void Init()
      {
         SetupTestDataFileCopies();

         if (File.Exists(TestScratchFile))
         {
            File.Delete(TestScratchFile);
         }

         _database = new UnitInfoDatabase(null, _proteinDictionary);
      }

      [TearDown]
      public void Destroy()
      {
         if (_database != null)
         {
            _database.Dispose();
         }
      }

      [TestFixtureTearDown]
      public void FixtureDestroy()
      {
         foreach (var file in Directory.EnumerateFiles(TestDataFilesFolder, "*.dbcopy"))
         {
            File.Delete(file);
         }
         if (File.Exists(TestScratchFile))
         {
            File.Delete(TestScratchFile);
         }
      }

      [Test]
      public void TableExistsAndDropTableTest()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         Assert.AreEqual(true, _database.TableExists(SqlTable.WuHistory));
         _database.DropTable(SqlTable.WuHistory);
         Assert.AreEqual(false, _database.TableExists(SqlTable.WuHistory));
      }

      #region Connected

      [Test]
      public void ConnectedTest1()
      {
         _database.DatabaseFilePath = TestScratchFile;
         Assert.AreEqual(24, GetWuHistoryColumnCount(TestScratchFile));
         Assert.AreEqual(Application.VersionWithRevision, _database.GetDatabaseVersion());
         Assert.AreEqual(true, _database.Connected);
      }

      #endregion

      #region PerformUpgrade

      [Test]
      public void PerformUpgrade_v092_Test1()
      {
         Assert.AreEqual(15, GetWuHistoryColumnCount(_testDataFileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         _database.DatabaseFilePath = _testDataFileCopy;
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testDataFileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         Assert.AreEqual(Application.VersionWithRevision, _database.GetDatabaseVersion());
      }

      [Test]
      public void PerformUpgrade_v092_AlreadyUpgraded_Test()
      {
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testData_1FileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         _database.DatabaseFilePath = _testData_1FileCopy;
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testData_1FileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         Assert.IsTrue(Application.ParseVersion("0.9.2.0") <= Application.ParseVersion(_database.GetDatabaseVersion()));
      }

      [Test]
      public void PerformUpgrade_v092_Test2()
      {
         Assert.AreEqual(15, GetWuHistoryColumnCount(_testData2FileCopy));
         Assert.AreEqual(285, GetWuHistoryRowCount(_testData2FileCopy));
         _database.DatabaseFilePath = _testData2FileCopy;
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testData2FileCopy));
         // 32 duplicates deleted
         Assert.AreEqual(253, GetWuHistoryRowCount(_testData2FileCopy));
         Assert.AreEqual(Application.VersionWithRevision, _database.GetDatabaseVersion());
      }

      #endregion

      #region Insert

      [Test]
      public void InsertTest1()
      {
         InsertTestInternal(BuildUnitInfo1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void InsertTest1CzechCulture()
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
         InsertTestInternal(BuildUnitInfo1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void InsertTest2()
      {
         InsertTestInternal(BuildUnitInfo2(), BuildUnitInfo2VerifyAction());
      }

      [Test]
      public void InsertTest3()
      {
         InsertTestInternal(BuildUnitInfo3(), BuildUnitInfo3VerifyAction());
      }

      [Test]
      public void InsertTest4()
      {
         InsertTestInternal(BuildUnitInfo4(), BuildUnitInfo4VerifyAction());
      }

      private void InsertTestInternal(UnitInfo unitInfo, Action<IList<HistoryEntry>> verifyAction)
      {
         _database.DatabaseFilePath = TestScratchFile;
         Core.Configuration.ObjectMapper.CreateMaps();

         var unitInfoLogic = new UnitInfoLogic(MockRepository.GenerateStub<IProteinBenchmarkCollection>())
                             {
                                CurrentProtein = new Protein(),
                                UnitInfoData = unitInfo
                             };
         _database.Insert(unitInfoLogic);

         var rows = _database.Fetch(new QueryParameters());
         verifyAction(rows);

         // test code to ensure this unit is NOT written again
         _database.Insert(unitInfoLogic);
         // verify
         rows = _database.Fetch(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
      }

      private static UnitInfo BuildUnitInfo1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = 1;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         //unitInfo.OwningSlotId = 
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;
         unitInfo.DownloadTime = new DateTime(2010, 1, 1); //, 0 ,0 ,0, DateTimeKind.Utc);
         unitInfo.FinishedTime = new DateTime(2010, 1, 2); //, 0, 0, 0, DateTimeKind.Utc);
         unitInfo.FramesObserved = 2;
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });
         return unitInfo;
      }

      private static Action<IList<HistoryEntry>> BuildUnitInfo1VerifyAction()
      {
         return rows =>
         {
            Assert.AreEqual(1, rows.Count);
            HistoryEntry entry = rows[0];
            Assert.AreEqual(2669, entry.ProjectID);
            Assert.AreEqual(1, entry.ProjectRun);
            Assert.AreEqual(2, entry.ProjectClone);
            Assert.AreEqual(3, entry.ProjectGen);
            Assert.AreEqual("Owner", entry.Name);
            Assert.AreEqual("Path", entry.Path);
            Assert.AreEqual("harlam357", entry.Username);
            Assert.AreEqual(32, entry.Team);
            Assert.AreEqual(2.09f, entry.CoreVersion);
            Assert.AreEqual(100, entry.FramesCompleted);
            Assert.AreEqual(TimeSpan.FromSeconds(600), entry.FrameTime);
            Assert.AreEqual(WorkUnitResult.FinishedUnit, entry.Result.ToWorkUnitResult());
            Assert.AreEqual(new DateTime(2010, 1, 1), entry.DownloadDateTime);
            Assert.AreEqual(new DateTime(2010, 1, 2), entry.CompletionDateTime);
         };
      }

      private static UnitInfo BuildUnitInfo2()
      {
         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 6900;
         unitInfo.ProjectRun = 4;
         unitInfo.ProjectClone = 5;
         unitInfo.ProjectGen = 6;
         unitInfo.OwningClientName = "Owner's";
         unitInfo.OwningClientPath = "The Path's";
         //unitInfo.OwningSlotId = 
         unitInfo.FoldingID = "harlam357's";
         unitInfo.Team = 100;
         unitInfo.CoreVersion = 2.27f;
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2009, 5, 5);
         unitInfo.FinishedTime = DateTime.MinValue;
         unitInfo.FramesObserved = 2;
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 55, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 56, TimeOfFrame = TimeSpan.FromSeconds(1000) });
         return unitInfo;
      }

      private static Action<IList<HistoryEntry>> BuildUnitInfo2VerifyAction()
      {
         return rows =>
         {
            Assert.AreEqual(1, rows.Count);
            HistoryEntry entry = rows[0];
            Assert.AreEqual(6900, entry.ProjectID);
            Assert.AreEqual(4, entry.ProjectRun);
            Assert.AreEqual(5, entry.ProjectClone);
            Assert.AreEqual(6, entry.ProjectGen);
            Assert.AreEqual("Owner's", entry.Name);
            Assert.AreEqual("The Path's", entry.Path);
            Assert.AreEqual("harlam357's", entry.Username);
            Assert.AreEqual(100, entry.Team);
            Assert.AreEqual(2.27f, entry.CoreVersion);
            Assert.AreEqual(56, entry.FramesCompleted);
            Assert.AreEqual(TimeSpan.FromSeconds(1000), entry.FrameTime);
            Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, entry.Result.ToWorkUnitResult());
            Assert.AreEqual(new DateTime(2009, 5, 5), entry.DownloadDateTime);
            Assert.AreEqual(DateTime.MinValue, entry.CompletionDateTime);
         };
      }

      private static UnitInfo BuildUnitInfo3()
      {
         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2670;
         unitInfo.ProjectRun = 2;
         unitInfo.ProjectClone = 3;
         unitInfo.ProjectGen = 4;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         //unitInfo.OwningSlotId = 
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2010, 2, 2);
         unitInfo.FinishedTime = DateTime.MinValue;
         //unitInfo.FramesObserved = 
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });
         return unitInfo;
      }

      private static Action<IList<HistoryEntry>> BuildUnitInfo3VerifyAction()
      {
         return rows =>
         {
            Assert.AreEqual(1, rows.Count);
            HistoryEntry entry = rows[0];
            Assert.AreEqual(2670, entry.ProjectID);
            Assert.AreEqual(2, entry.ProjectRun);
            Assert.AreEqual(3, entry.ProjectClone);
            Assert.AreEqual(4, entry.ProjectGen);
            Assert.AreEqual("Owner", entry.Name);
            Assert.AreEqual("Path", entry.Path);
            Assert.AreEqual("harlam357", entry.Username);
            Assert.AreEqual(32, entry.Team);
            Assert.AreEqual(2.09f, entry.CoreVersion);
            Assert.AreEqual(100, entry.FramesCompleted);
            Assert.AreEqual(TimeSpan.Zero, entry.FrameTime);
            Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, entry.Result.ToWorkUnitResult());
            Assert.AreEqual(new DateTime(2010, 2, 2), entry.DownloadDateTime);
            Assert.AreEqual(DateTime.MinValue, entry.CompletionDateTime);
         };
      }

      private static UnitInfo BuildUnitInfo4()
      {
         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 6903;
         unitInfo.ProjectRun = 2;
         unitInfo.ProjectClone = 3;
         unitInfo.ProjectGen = 4;
         unitInfo.OwningClientName = "Owner2";
         unitInfo.OwningClientPath = "Path2";
         unitInfo.OwningSlotId = 2;
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.27f;
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;
         unitInfo.DownloadTime = new DateTime(2012, 1, 2);
         unitInfo.FinishedTime = new DateTime(2012, 1, 5);
         //unitInfo.FramesObserved = 
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });
         return unitInfo;
      }

      private static Action<IList<HistoryEntry>> BuildUnitInfo4VerifyAction()
      {
         return rows =>
         {
            Assert.AreEqual(1, rows.Count);
            HistoryEntry entry = rows[0];
            Assert.AreEqual(6903, entry.ProjectID);
            Assert.AreEqual(2, entry.ProjectRun);
            Assert.AreEqual(3, entry.ProjectClone);
            Assert.AreEqual(4, entry.ProjectGen);
            Assert.AreEqual("Owner2 Slot 02", entry.Name);
            Assert.AreEqual("Path2", entry.Path);
            Assert.AreEqual("harlam357", entry.Username);
            Assert.AreEqual(32, entry.Team);
            Assert.AreEqual(2.27f, entry.CoreVersion);
            Assert.AreEqual(100, entry.FramesCompleted);
            Assert.AreEqual(TimeSpan.Zero, entry.FrameTime);
            Assert.AreEqual(WorkUnitResult.FinishedUnit, entry.Result.ToWorkUnitResult());
            Assert.AreEqual(new DateTime(2012, 1, 2), entry.DownloadDateTime);
            Assert.AreEqual(new DateTime(2012, 1, 5), entry.CompletionDateTime);
         };
      }

      #endregion

      #region Delete

      [Test]
      public void DeleteTest()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         var entries = _database.Fetch(new QueryParameters());
         Assert.AreEqual(44, entries.Count);
         Assert.AreEqual(1, _database.Delete(entries[14]));
         entries = _database.Fetch(new QueryParameters());
         Assert.AreEqual(43, entries.Count);
      }
      
      [Test]
      public void DeleteNotExistTest()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         Assert.AreEqual(0, _database.Delete(new HistoryEntry { ID = 100 }));
      }

      #endregion

      #region Fetch

      [Test]
      public void FetchAllTest()
      {
         // Select All
         FetchTestData(44, BuildParameters());
      }

      [Test]
      public void FetchEqualTest()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Equal,
            Value = 6600
         }));
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Equal,
            Value = 7
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Equal,
            Value = 18
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Equal,
            Value = 18
         }));
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Equal,
            Value = "nVidia GPU - GTX275"
         }));
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Equal,
            Value = @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Equal,
            Value = "harlam357"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Equal,
            Value = 32
         }));
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Equal,
            Value = 2.09
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Equal,
            Value = 41  // not a TimeSpan value
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Equal,
            Value = 1   // not a String value
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Equal,
            Value = "WorkUnitName"
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Equal,
            Value = 2.3
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Equal,
            Value = "GROGPU2"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Equal,
            Value = 7000
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Equal,
            Value = "GPU"
         }));
         FetchTestData(6, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.Equal,
            Value = 9482.92683
         }));
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Equal,
            Value = 450
         }));
      }

      [Test]
      public void FetchGreaterThanTest()
      {
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThan,
            Value = 10502
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThan,
            Value = 79
         }));
         FetchTestData(7, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThan,
            Value = 761
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThan,
            Value = 279
         }));
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThan,
            Value = "nVidia GPU - GTX275"
         }));
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThan,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThan,
            Value = "harlam357"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThan,
            Value = 32
         }));
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThan,
            Value = 2.09
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThan,
            Value = 100
         }));
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThan,
            Value = 41  // not a TimeSpan value
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThan,
            Value = 1   // not a String value
         }));
         FetchTestData(7, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThan,
            Value = "WorkUnitName"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThan,
            Value = 2.3
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThan,
            Value = "GRO-A3"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThan,
            Value = 99
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThan,
            Value = 0
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThan,
            Value = "CPU"
         }));
         FetchTestData(6, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.GreaterThan,
            Value = 9482.92683
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThan,
            Value = 450
         }));
      }

      [Test]
      public void FetchGreaterThanOrEqualTest()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 10502
         }));
         FetchTestData(5, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 79
         }));
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 761
         }));
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 279
         }));
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "nVidia GPU - GTX275"
         }));
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "harlam357"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 32
         }));
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.09
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
         FetchTestData(35, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 41  // not a TimeSpan value
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 1   // not a String value
         }));
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(24, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "WorkUnitName"
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.3
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "GRO-A3"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 99
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 0
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "CPU"
         }));
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 9482.92683
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 450
         }));
      }

      [Test]
      public void FetchLessThanTest()
      {
         FetchTestData(31, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThan,
            Value = 10502
         }));
         FetchTestData(39, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThan,
            Value = 79
         }));
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThan,
            Value = 761
         }));
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThan,
            Value = 279
         }));
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThan,
            Value = "nVidia GPU - GTX275"
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThan,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThan,
            Value = "harlam357"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThan,
            Value = 32
         }));
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThan,
            Value = 2.09
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
         FetchTestData(9, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThan,
            Value = 41  // not a TimeSpan value
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThan,
            Value = 1   // not a String value
         }));
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThan,
            Value = "WorkUnitName"
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThan,
            Value = 2.3
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThan,
            Value = "GRO-A3"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThan,
            Value = 99
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThan,
            Value = 0
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThan,
            Value = "CPU"
         }));
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.LessThan,
            Value = 9482.92683
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThan,
            Value = 450
         }));
      }

      [Test]
      public void FetchLessThanOrEqualTest()
      {
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 10502
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 79
         }));
         FetchTestData(37, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 761
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 279
         }));
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "nVidia GPU - GTX275"
         }));
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThanOrEqual,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "harlam357"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 32
         }));
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 2.09
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100
         }));
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 41  // not a TimeSpan value
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 1   // not a String value
         }));
         FetchTestData(37, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "WorkUnitName"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 2.3
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "GRO-A3"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 99
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 0
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "CPU"
         }));
         FetchTestData(38, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 9482.92683
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 450
         }));
      }

      [Test]
      public void FetchLikeTest()
      {
         FetchTestData(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Like,
            Value = "10%"
         }));
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData(5, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Like,
            Value = "9%"
         }));
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Like,
            Value = "2%"
         }));
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Like,
            Value = "nVidia GPU%"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Like,
            Value = @"\\%\%"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Like,
            Value = "h%"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Like,
            Value = "%2"
         }));
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Like,
            Value = "2%"
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Like,
            Value = "4%"  // not a TimeSpan value
         }));
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Like,
            Value = "1%"   // not a String value
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Like,
            Value = "Work%Name%"
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Like,
            Value = "GRO%"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Like,
            Value = "%U"
         }));
         FetchTestData(9, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.Like,
            Value = "9%"
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Like,
            Value = "6%"
         }));
      }

      [Test]
      public void FetchNotLikeTest()
      {
         FetchTestData(30, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.NotLike,
            Value = "10%"
         }));
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData(39, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.NotLike,
            Value = "9%"
         }));
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.NotLike,
            Value = "2%"
         }));
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.NotLike,
            Value = "nVidia GPU%"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.NotLike,
            Value = @"\\%\%"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.NotLike,
            Value = "h%"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.NotLike,
            Value = "%2"
         }));
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.NotLike,
            Value = "2%"
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData(30, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.NotLike,
            Value = "4%"  // not a TimeSpan value
         }));
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.NotLike,
            Value = "1%"   // not a String value
         }));
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.NotLike,
            Value = "Work%Name%"
         }));
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.NotLike,
            Value = "GRO%"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
         FetchTestData(28, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.NotLike,
            Value = "%U"
         }));
         FetchTestData(35, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.NotLike,
            Value = "9%"
         }));
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.NotLike,
            Value = "6%"
         }));
      }

      [Test]
      public void FetchComplexTest_1()
      {
         FetchTestData(33, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2010, 8, 8)
         },
         new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2010, 8, 22)
         }));
      }

      [Test]
      public void FetchComplexTest_2()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThan,
            Value = 5000
         },
         new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 7000
         }));
      }

      [Test]
      public void FetchAllTest2()
      {
         // Select All
         FetchTestData2(253, BuildParameters());
      }

      [Test]
      public void FetchEqualTest2()
      {
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Equal,
            Value = 8011
         }));
         FetchTestData2(72, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Equal,
            Value = 0
         }));
         FetchTestData2(6, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Equal,
            Value = 63
         }));
         FetchTestData2(2, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Equal,
            Value = 188
         }));
         FetchTestData2(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Equal,
            Value = "Windows - Test Workstation Slot 00"
         }));
         FetchTestData2(30, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Equal,
            Value = "192.168.0.172-36330"
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Equal,
            Value = "harlam357"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Equal,
            Value = 32
         }));
         FetchTestData2(63, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Equal,
            Value = 2.27
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
         FetchTestData2(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Equal,
            Value = 100  // not a TimeSpan value
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Equal,
            Value = 1   // not a String value
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Equal,
            Value = "WorkUnitName3"
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Equal,
            Value = 0.75
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Equal,
            Value = "GRO-A5"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Equal,
            Value = 11000
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Equal,
            Value = "CPU"
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.Equal,
            Value = 486876.03173
         }));
         FetchTestData2(2, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Equal,
            Value = 869.4797
         }));
      }

      [Test]
      public void FetchGreaterThanTest2()
      {
         FetchTestData2(75, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThan,
            Value = 7137
         }));
         FetchTestData2(47, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThan,
            Value = 18
         }));
         FetchTestData2(99, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThan,
            Value = 63
         }));
         FetchTestData2(146, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThan,
            Value = 188
         }));
         FetchTestData2(86, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThan,
            Value = "Windows - Test Workstation Slot 00"
         }));
         FetchTestData2(197, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThan,
            Value = @"\\192.168.0.133\FAH\"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThan,
            Value = "harlam357"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThan,
            Value = 32
         }));
         FetchTestData2(166, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThan,
            Value = 2.15
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThan,
            Value = 100
         }));
         FetchTestData2(150, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThan,
            Value = 100  // not a TimeSpan value
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThan,
            Value = 1   // not a String value
         }));
         FetchTestData2(42, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThan,
            Value = "WorkUnitName3"
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThan,
            Value = 0.75
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThan,
            Value = "GRO-A4"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThan,
            Value = 100
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThan,
            Value = 9000
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThan,
            Value = "CPU"
         }));
         FetchTestData2(5, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.GreaterThan,
            Value = 486876.03173
         }));
         FetchTestData2(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThan,
            Value = 869.4797
         }));
      }

      [Test]
      public void FetchGreaterThanOrEqualTest2()
      {
         FetchTestData2(78, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 7137
         }));
         FetchTestData2(51, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 18
         }));
         FetchTestData2(105, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 63
         }));
         FetchTestData2(148, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 188
         }));
         FetchTestData2(98, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "Windows - Test Workstation Slot 00"
         }));
         FetchTestData2(205, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = @"\\192.168.0.133\FAH\"
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "harlam357"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 32
         }));
         FetchTestData2(226, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.15
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
         FetchTestData2(164, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100  // not a TimeSpan value
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 1   // not a String value
         }));
         FetchTestData2(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(17, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "WorkUnitName3"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 0.75
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "GRO-A4"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 9000
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "CPU"
         }));
         FetchTestData2(6, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 486876.03173
         }));
         FetchTestData2(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 869.4797
         }));
      }

      [Test]
      public void FetchLessThanTest2()
      {
         FetchTestData2(175, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThan,
            Value = 7137
         }));
         FetchTestData2(202, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThan,
            Value = 18
         }));
         FetchTestData2(148, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThan,
            Value = 63
         }));
         FetchTestData2(105, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThan,
            Value = 188
         }));
         FetchTestData2(155, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThan,
            Value = "Windows - Test Workstation Slot 00"
         }));
         FetchTestData2(48, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThan,
            Value = @"\\192.168.0.133\FAH\"
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThan,
            Value = "harlam357"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThan,
            Value = 32
         }));
         FetchTestData2(27, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThan,
            Value = 2.15
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
         FetchTestData2(89, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThan,
            Value = 100  // not a TimeSpan value
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThan,
            Value = 1   // not a String value
         }));
         FetchTestData2(210, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(236, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThan,
            Value = "WorkUnitName4"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThan,
            Value = 0.75
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThan,
            Value = "GRO-A4"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThan,
            Value = 11000
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThan,
            Value = "CPU"
         }));
         FetchTestData2(247, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.LessThan,
            Value = 486876.03173
         }));
         FetchTestData2(237, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThan,
            Value = 869.4797
         }));
      }

      [Test]
      public void FetchLessThanOrEqualTest2()
      {
         FetchTestData2(178, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 7137
         }));
         FetchTestData2(206, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 18
         }));
         FetchTestData2(154, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 63
         }));
         FetchTestData2(107, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 188
         }));
         FetchTestData2(167, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "Windows - Test Workstation Slot 00"
         }));
         FetchTestData2(56, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThanOrEqual,
            Value = @"\\192.168.0.133\FAH\"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "harlam357"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 32
         }));
         FetchTestData2(87, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 2.15
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100
         }));
         FetchTestData2(103, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100  // not a TimeSpan value
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 1   // not a String value
         }));
         FetchTestData2(211, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(237, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "WorkUnitName4"
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 0.75
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "GRO-A4"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 11000
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "CPU"
         }));
         FetchTestData2(248, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 486876.03173
         }));
         FetchTestData2(239, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 869.4797
         }));
      }

      [Test]
      public void FetchLikeTest2()
      {
         FetchTestData2(33, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Like,
            Value = "8%"
         }));
         FetchTestData2(70, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData2(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Like,
            Value = "9%"
         }));
         FetchTestData2(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Like,
            Value = "2%"
         }));
         FetchTestData2(76, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Like,
            Value = "Ubuntu VM SMP%"
         }));
         FetchTestData2(160, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Like,
            Value = "%192%"
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Like,
            Value = "%357"
         }));
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Like,
            Value = "3%"
         }));
         FetchTestData2(27, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData2(27, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Like,
            Value = "3_"  // not a TimeSpan value
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Like,
            Value = "3%"   // not a String value
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Like,
            Value = "Work%Name%"
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Like,
            Value = "GRO%"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Like,
            Value = "%U"
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
         FetchTestData2(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Like,
            Value = "9%"
         }));
      }

      [Test]
      public void FetchNotLikeTest2()
      {
         FetchTestData2(220, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.NotLike,
            Value = "8%"
         }));
         FetchTestData2(183, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData2(239, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.NotLike,
            Value = "9%"
         }));
         FetchTestData2(224, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.NotLike,
            Value = "2%"
         }));
         FetchTestData2(177, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.NotLike,
            Value = "Ubuntu VM SMP%"
         }));
         FetchTestData2(93, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.NotLike,
            Value = "%192%"
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.NotLike,
            Value = "%357"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.NotLike,
            Value = "3%"
         }));
         FetchTestData2(226, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData2(226, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.NotLike,
            Value = "3_"  // not a TimeSpan value
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.NotLike,
            Value = "3%"   // not a String value
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.NotLike,
            Value = "Work%Name%"
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.NotLike,
            Value = "GRO%"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
         FetchTestData2(233, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.NotLike,
            Value = "%U"
         }));
         FetchTestData2(243, BuildParameters(new QueryField
         {
            Name = QueryFieldName.PPD,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
         FetchTestData2(250, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.NotLike,
            Value = "9%"
         }));
      }

      [Test]
      public void FetchComplexTest2_1()
      {
         FetchTestData2(52, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2012, 5, 29)
         },
         new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2012, 11, 1)
         }));
      }

      [Test]
      public void FetchComplexTest2_2()
      {
         FetchTestData2(77, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "Ubuntu VM SMP - Media Server"
         },
         new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "n"
         }));
      }

      private static QueryParameters BuildParameters(params QueryField[] fields)
      {
         var parameters = new QueryParameters();
         parameters.Fields.AddRange(fields);
         return parameters;
      }

      private void FetchTestData(int count, QueryParameters parameters)
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         FetchInternal(count, parameters, HistoryProductionView.BonusDownloadTime);
      }

      private void FetchTestData2(int count, QueryParameters parameters)
      {
         _database.DatabaseFilePath = _testData2FileCopy;
         FetchInternal(count, parameters, HistoryProductionView.BonusFrameTime);
      }

      private void FetchInternal(int count, QueryParameters parameters, HistoryProductionView productionView)
      {
         var entries = _database.Fetch(parameters, productionView);
#if DEBUG
         foreach (var entry in entries)
         {
            Debug.WriteLine(entry.ID);
         }
#endif
         Assert.AreEqual(count, entries.Count);
      }

      #endregion

      private static int GetWuHistoryColumnCount(string dataSource)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
         {
            con.Open();
            using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", con))
            using (var table = new DataTable())
            {
               adapter.Fill(table);
               foreach (DataRow row in table.Rows)
               {
                  Debug.WriteLine(row[1].ToString());
               }
               return table.Rows.Count;
            }
         }
      }

      private static int GetWuHistoryRowCount(string dataSource)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
         {
            con.Open();
            using (var cmd = con.CreateCommand())
            {
               cmd.CommandText = "SELECT COUNT(*) FROM WuHistory";
               return Convert.ToInt32(cmd.ExecuteScalar());
            }
         }
      }

      private static IProteinDictionary CreateProteinDictionary()
      {
         var proteins = new ProteinDictionary();

         var protein = new Protein();
         protein.ProjectNumber = 6600;
         protein.WorkUnitName = "WorkUnitName";
         protein.Core = "GROGPU2";
         protein.Credit = 450;
         protein.KFactor = 0;
         protein.Frames = 100;
         protein.NumberOfAtoms = 5000;
         protein.PreferredDays = 2;
         protein.MaximumDays = 3;
         proteins.Add(protein.ProjectNumber, protein);

         protein = new Protein();
         protein.ProjectNumber = 5797;
         protein.WorkUnitName = "WorkUnitName2";
         protein.Core = "GROGPU2";
         protein.Credit = 675;
         protein.KFactor = 2.3;
         protein.Frames = 100;
         protein.NumberOfAtoms = 7000;
         protein.PreferredDays = 2;
         protein.MaximumDays = 3;
         proteins.Add(protein.ProjectNumber, protein);

         protein = new Protein();
         protein.ProjectNumber = 8011;
         protein.WorkUnitName = "WorkUnitName3";
         protein.Core = "GRO-A4";
         protein.Credit = 106.6;
         protein.KFactor = 0.75;
         protein.Frames = 100;
         protein.NumberOfAtoms = 9000;
         protein.PreferredDays = 2.13;
         protein.MaximumDays = 4.62;
         proteins.Add(protein.ProjectNumber, protein);

         protein = new Protein();
         protein.ProjectNumber = 6903;
         protein.WorkUnitName = "WorkUnitName4";
         protein.Core = "GRO-A5";
         protein.Credit = 22706;
         protein.KFactor = 38.05;
         protein.Frames = 100;
         protein.NumberOfAtoms = 11000;
         protein.PreferredDays = 5;
         protein.MaximumDays = 12;
         proteins.Add(protein.ProjectNumber, protein);

         return proteins;
      }

      private void SetupTestDataFileCopies()
      {
         // sometimes the file is not finished
         // copying before we attempt to open
         // the copied file.  Halt the thread
         // for a bit to ensure the copy has
         // completed.

         File.Copy(TestDataFile, _testDataFileCopy, true);
         Thread.Sleep(100);

         File.Copy(TestData2File, _testData2FileCopy, true);
         Thread.Sleep(100);

         File.Copy(TestData_1File, _testData_1FileCopy, true);
         Thread.Sleep(100);
      }
   }
}
