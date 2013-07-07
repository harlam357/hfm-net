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
using System.Threading.Tasks;

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

      #region Setup and TearDown

      [TestFixtureSetUp]
      public void FixtureInit()
      {
         Core.Configuration.ObjectMapper.CreateMaps();
      }

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

      #endregion

      [Test]
      public void MultiThreadTest()
      {
         _database.DatabaseFilePath = TestScratchFile;
         var benchmarkCollection = MockRepository.GenerateStub<IProteinBenchmarkCollection>();
         
         Parallel.For(0, 100, i =>
                              {
                                 Debug.WriteLine("Writing unit {0:00} on thread id: {1:00}", i, Thread.CurrentThread.ManagedThreadId);
 
                                 var unitInfoLogic = new UnitInfoLogic(benchmarkCollection);
                                 unitInfoLogic.CurrentProtein = BuildProtein1();
                                 unitInfoLogic.UnitInfoData = BuildUnitInfo1(i);

                                 _database.Insert(unitInfoLogic);
                              });

         Assert.AreEqual(100, _database.Fetch(QueryParameters.SelectAll).Count);
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

      #region Upgrade

      [Test]
      public void Upgrade_v092_Test1()
      {
         Assert.AreEqual(15, GetWuHistoryColumnCount(_testDataFileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         _database.DatabaseFilePath = _testDataFileCopy;
         _database.Upgrade();
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testDataFileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         Assert.AreEqual(Application.VersionWithRevision, _database.GetDatabaseVersion());
      }

      [Test]
      public void Upgrade_v092_AlreadyUpgraded_Test()
      {
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testData_1FileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         _database.DatabaseFilePath = _testData_1FileCopy;
         _database.Upgrade();
         Assert.AreEqual(24, GetWuHistoryColumnCount(_testData_1FileCopy));
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         Assert.IsTrue(Application.ParseVersion("0.9.2.0") <= Application.ParseVersion(_database.GetDatabaseVersion()));
      }

      [Test]
      public void Upgrade_v092_Test2()
      {
         Assert.AreEqual(15, GetWuHistoryColumnCount(_testData2FileCopy));
         Assert.AreEqual(285, GetWuHistoryRowCount(_testData2FileCopy));
         _database.DatabaseFilePath = _testData2FileCopy;
         _database.Upgrade();
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
         InsertTestInternal(BuildUnitInfo1(), BuildProtein1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void InsertTest1CzechCulture()
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
         InsertTestInternal(BuildUnitInfo1(), BuildProtein1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void InsertTest2()
      {
         InsertTestInternal(BuildUnitInfo2(), BuildProtein2(), BuildUnitInfo2VerifyAction());
      }

      [Test]
      public void InsertTest3()
      {
         InsertTestInternal(BuildUnitInfo3(), BuildProtein3(), BuildUnitInfo3VerifyAction());
      }

      [Test]
      public void InsertTest4()
      {
         InsertTestInternal(BuildUnitInfo4(), BuildProtein4(), BuildUnitInfo4VerifyAction());
      }

      private void InsertTestInternal(UnitInfo unitInfo, Protein protein, Action<IList<HistoryEntry>> verifyAction)
      {
         _database.DatabaseFilePath = TestScratchFile;

         var unitInfoLogic = new UnitInfoLogic(MockRepository.GenerateStub<IProteinBenchmarkCollection>());
         unitInfoLogic.CurrentProtein = protein;
         unitInfoLogic.UnitInfoData = unitInfo;

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
         return BuildUnitInfo1(1);
      }

      private static UnitInfo BuildUnitInfo1(int run)
      {
         var unitInfo = new UnitInfo();

         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = run;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         //unitInfo.OwningSlotId = 
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;

         // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
         // connection string option to Utc will force SQLite to handle all DateTime 
         // values as Utc regardless of the DateTimeKind specified in the value.
         unitInfo.DownloadTime = new DateTime(2010, 1, 1, 0 ,0 ,0, DateTimeKind.Utc);
         unitInfo.FinishedTime = new DateTime(2010, 1, 2, 0, 0, 0, DateTimeKind.Utc);

         // these values effect the value reported when UnitInfoLogic.GetRawTime() is called
         unitInfo.FramesObserved = 2;
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });

         return unitInfo;
      }

      private static Protein BuildProtein1()
      {
         return new Protein
                {
                   WorkUnitName = "TestUnit1",
                   KFactor = 1.0,
                   Core = "GRO-A3",
                   Frames = 100,
                   NumberOfAtoms = 1000,
                   Credit = 100.0,
                   PreferredDays = 3.0,
                   MaximumDays = 5.0
                };
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
            Assert.AreEqual("TestUnit1", entry.WorkUnitName);
            Assert.AreEqual(1.0, entry.KFactor);
            Assert.AreEqual("GRO-A3", entry.Core);
            Assert.AreEqual(100, entry.Frames);
            Assert.AreEqual(1000, entry.Atoms);
            Assert.AreEqual(100.0, entry.BaseCredit);
            Assert.AreEqual(3.0, entry.PreferredDays);
            Assert.AreEqual(5.0, entry.MaximumDays);
            Assert.AreEqual(SlotType.CPU.ToString(), entry.SlotType);
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

         // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
         // connection string option to Utc will force SQLite to handle all DateTime 
         // values as Utc regardless of the DateTimeKind specified in the value.
         unitInfo.DownloadTime = new DateTime(2009, 5, 5);
         unitInfo.FinishedTime = DateTime.MinValue;

         // these values effect the value reported when UnitInfoLogic.GetRawTime() is called
         unitInfo.FramesObserved = 2;
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 55, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 56, TimeOfFrame = TimeSpan.FromSeconds(1000) });
         return unitInfo;
      }

      private static Protein BuildProtein2()
      {
         return new Protein
         {
            WorkUnitName = "TestUnit2",
            KFactor = 2.0,
            Core = "GRO-A4",
            Frames = 200,
            NumberOfAtoms = 2000,
            Credit = 200.0,
            PreferredDays = 6.0,
            MaximumDays = 10.0
         };
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
            Assert.AreEqual("TestUnit2", entry.WorkUnitName);
            Assert.AreEqual(2.0, entry.KFactor);
            Assert.AreEqual("GRO-A4", entry.Core);
            Assert.AreEqual(200, entry.Frames);
            Assert.AreEqual(2000, entry.Atoms);
            Assert.AreEqual(200.0, entry.BaseCredit);
            Assert.AreEqual(6.0, entry.PreferredDays);
            Assert.AreEqual(10.0, entry.MaximumDays);
            Assert.AreEqual(SlotType.CPU.ToString(), entry.SlotType);
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

         // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
         // connection string option to Utc will force SQLite to handle all DateTime 
         // values as Utc regardless of the DateTimeKind specified in the value.
         unitInfo.DownloadTime = new DateTime(2010, 2, 2);
         unitInfo.FinishedTime = DateTime.MinValue;

         // these values effect the value reported when UnitInfoLogic.GetRawTime() is called
         //unitInfo.FramesObserved = 
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });
         return unitInfo;
      }

      private static Protein BuildProtein3()
      {
         return new Protein
         {
            WorkUnitName = "TestUnit3",
            KFactor = 3.0,
            Core = "GRO-A5",
            Frames = 300,
            NumberOfAtoms = 3000,
            Credit = 300.0,
            PreferredDays = 7.0,
            MaximumDays = 12.0
         };
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
            Assert.AreEqual("TestUnit3", entry.WorkUnitName);
            Assert.AreEqual(3.0, entry.KFactor);
            Assert.AreEqual("GRO-A5", entry.Core);
            Assert.AreEqual(300, entry.Frames);
            Assert.AreEqual(3000, entry.Atoms);
            Assert.AreEqual(300.0, entry.BaseCredit);
            Assert.AreEqual(7.0, entry.PreferredDays);
            Assert.AreEqual(12.0, entry.MaximumDays);
            Assert.AreEqual(SlotType.CPU.ToString(), entry.SlotType);
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

         // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
         // connection string option to Utc will force SQLite to handle all DateTime 
         // values as Utc regardless of the DateTimeKind specified in the value.
         unitInfo.DownloadTime = new DateTime(2012, 1, 2);
         unitInfo.FinishedTime = new DateTime(2012, 1, 5);

         // these values effect the value reported when UnitInfoLogic.GetRawTime() is called
         //unitInfo.FramesObserved = 
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetUnitFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });
         return unitInfo;
      }

      private static Protein BuildProtein4()
      {
         return new Protein
         {
            WorkUnitName = "TestUnit4",
            KFactor = 4.0,
            Core = "OPENMMGPU",
            Frames = 400,
            NumberOfAtoms = 4000,
            Credit = 400.0,
            PreferredDays = 2.0,
            MaximumDays = 5.0
         };
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
            Assert.AreEqual("TestUnit4", entry.WorkUnitName);
            Assert.AreEqual(4.0, entry.KFactor);
            Assert.AreEqual("OPENMMGPU", entry.Core);
            Assert.AreEqual(400, entry.Frames);
            Assert.AreEqual(4000, entry.Atoms);
            Assert.AreEqual(400.0, entry.BaseCredit);
            Assert.AreEqual(2.0, entry.PreferredDays);
            Assert.AreEqual(5.0, entry.MaximumDays);
            Assert.AreEqual(SlotType.GPU.ToString(), entry.SlotType);
         };
      }

      #endregion

      #region Delete

      [Test]
      public void DeleteTest()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         _database.Upgrade();
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
         _database.Upgrade();
         Assert.AreEqual(0, _database.Delete(new HistoryEntry { ID = 100 }));
      }

      #endregion

      #region Fetch

      [Test]
      public void Fetch_All_Test()
      {
         // Select All
         FetchTestData(44, BuildParameters());
      }

      [Test]
      public void Fetch_Equal_ProjectID_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Equal,
            Value = 6600
         }));
      }

      [Test]
      public void Fetch_Equal_ProjectRun_Test()
      {
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Equal,
            Value = 7
         }));
      }

      [Test]
      public void Fetch_Equal_ProjectClone_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Equal,
            Value = 18
         }));
      }

      [Test]
      public void Fetch_Equal_ProjectGen_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Equal,
            Value = 18
         }));
      }

      [Test]
      public void Fetch_Equal_Name_Test()
      {
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Equal,
            Value = "nVidia GPU - GTX275"
         }));
      }

      [Test]
      public void Fetch_Equal_Path_Test()
      {
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Equal,
            Value = @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\"
         }));
      }

      [Test]
      public void Fetch_Equal_Username_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Equal,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_Equal_Team_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Equal,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_Equal_CoreVersion_Test()
      {
         FetchTestData(11, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Equal,
            Value = 2.09
         }));
      }

      [Test]
      public void Fetch_Equal_FramesCompleted_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_Equal_FrameTime_Test()
      {
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Equal,
            Value = 41  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_Equal_Result_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Equal,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_Equal_DownloadDateTime_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_Equal_CompletionDateTime_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Equal,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_Equal_WorkUnitName_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Equal,
            Value = "WorkUnitName"
         }));
      }

      [Test]
      public void Fetch_Equal_KFactor_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Equal,
            Value = 2.3
         }));
      }

      [Test]
      public void Fetch_Equal_Core_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Equal,
            Value = "GROGPU2"
         }));
      }

      [Test]
      public void Fetch_Equal_Frames_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Equal,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_Equal_Atoms_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Equal,
            Value = 7000
         }));
      }

      [Test]
      public void Fetch_Equal_SlotType_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Equal,
            Value = "GPU"
         }));
      }

      //[Test]
      //public void Fetch_Equal_PPD_Test()
      //{
      //   FetchTestData(6, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.Equal,
      //      Value = 9482.92683
      //   }));
      //}

      [Test]
      public void Fetch_Equal_Credit_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Equal,
            Value = 450
         }));
      }

      [Test]
      public void Fetch_GreaterThan_ProjectID_Test()
      {
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThan,
            Value = 10502
         }));
      }

      [Test]
      public void Fetch_GreaterThan_ProjectRun_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThan,
            Value = 79
         }));
      }

      [Test]
      public void Fetch_GreaterThan_ProjectClone_Test()
      {
         FetchTestData(7, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThan,
            Value = 761
         }));
      }

      [Test]
      public void Fetch_GreaterThan_ProjectGen_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThan,
            Value = 279
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Name_Test()
      {
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThan,
            Value = "nVidia GPU - GTX275"
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Path_Test()
      {
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThan,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Username_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThan,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Team_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThan,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_GreaterThan_CoreVersion_Test()
      {
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThan,
            Value = 2.09
         }));
      }

      [Test]
      public void Fetch_GreaterThan_FramesCompleted_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThan,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_GreaterThan_FrameTime_Test()
      {
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThan,
            Value = 41  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Result_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThan,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_GreaterThan_DownloadDateTime_Test()
      {
         FetchTestData(7, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_GreaterThan_CompletionDateTime_Test()
      {
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThan,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_GreaterThan_WorkUnitName_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThan,
            Value = "WorkUnitName"
         }));
      }

      [Test]
      public void Fetch_GreaterThan_KFactor_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThan,
            Value = 2.3
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Core_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThan,
            Value = "GRO-A3"
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Frames_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThan,
            Value = 99
         }));
      }

      [Test]
      public void Fetch_GreaterThan_Atoms_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThan,
            Value = 0
         }));
      }

      [Test]
      public void Fetch_GreaterThan_SlotType_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThan,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_GreaterThan_PPD_Test()
      //{
      //   FetchTestData(6, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.GreaterThan,
      //      Value = 9482.92683
      //   }));
      //}

      [Test]
      public void Fetch_GreaterThan_Credit_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThan,
            Value = 450
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectID_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 10502
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectRun_Test()
      {
         FetchTestData(5, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 79
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectClone_Test()
      {
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 761
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectGen_Test()
      {
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 279
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Name_Test()
      {
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "nVidia GPU - GTX275"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Path_Test()
      {
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Username_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Team_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_CoreVersion_Test()
      {
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.09
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_FramesCompleted_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_FrameTime_Test()
      {
         FetchTestData(35, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 41  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Result_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_DownloadDateTime_Test()
      {
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_CompletionDateTime_Test()
      {
         FetchTestData(24, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_WorkUnitName_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "WorkUnitName"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_KFactor_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.3
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Core_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "GRO-A3"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Frames_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 99
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Atoms_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 0
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_SlotType_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_GreaterThanOrEqual_PPD_Test()
      //{
      //   FetchTestData(12, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.GreaterThanOrEqual,
      //      Value = 9482.92683
      //   }));
      //}

      [Test]
      public void Fetch_GreaterThanOrEqual_Credit_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 450
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectID_Test()
      {
         FetchTestData(31, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThan,
            Value = 10502
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectRun_Test()
      {
         FetchTestData(39, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThan,
            Value = 79
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectClone_Test()
      {
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThan,
            Value = 761
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectGen_Test()
      {
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThan,
            Value = 279
         }));
      }

      [Test]
      public void Fetch_LessThan_Name_Test()
      {
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThan,
            Value = "nVidia GPU - GTX275"
         }));
      }

      [Test]
      public void Fetch_LessThan_Path_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThan,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
      }

      [Test]
      public void Fetch_LessThan_Username_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThan,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_LessThan_Team_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThan,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_LessThan_CoreVersion_Test()
      {
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThan,
            Value = 2.09
         }));
      }

      [Test]
      public void Fetch_LessThan_FramesCompleted_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_LessThan_FrameTime_Test()
      {
         FetchTestData(9, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThan,
            Value = 41  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_LessThan_Result_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThan,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_LessThan_DownloadDateTime_Test()
      {
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_LessThan_CompletionDateTime_Test()
      {
         FetchTestData(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_LessThan_WorkUnitName_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThan,
            Value = "WorkUnitName"
         }));
      }

      [Test]
      public void Fetch_LessThan_KFactor_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThan,
            Value = 2.3
         }));
      }

      [Test]
      public void Fetch_LessThan_Core_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThan,
            Value = "GRO-A3"
         }));
      }

      [Test]
      public void Fetch_LessThan_Frames_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThan,
            Value = 99
         }));
      }

      [Test]
      public void Fetch_LessThan_Atoms_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThan,
            Value = 0
         }));
      }

      [Test]
      public void Fetch_LessThan_SlotType_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThan,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_LessThan_PPD_Test()
      //{
      //   FetchTestData(32, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.LessThan,
      //      Value = 9482.92683
      //   }));
      //}

      [Test]
      public void Fetch_LessThan_Credit_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThan,
            Value = 450
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_ProjectID_Test()
      {
         FetchTestData(32, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 10502
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_ProjectRun_Test()
      {
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 79
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_ProjectClone_Test()
      {
         FetchTestData(37, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 761
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_ProjectGen_Test()
      {
         FetchTestData(41, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 279
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Name_Test()
      {
         FetchTestData(23, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "nVidia GPU - GTX275"
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Path_Test()
      {
         FetchTestData(12, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThanOrEqual,
            Value = @"\\Mainworkstation\Folding@home-gpu\"
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Username_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Team_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_CoreVersion_Test()
      {
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 2.09
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_FramesCompleted_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_FrameTime_Test()
      {
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 41  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Result_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_DownloadDateTime_Test()
      {
         FetchTestData(37, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_CompletionDateTime_Test()
      {
         FetchTestData(21, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThanOrEqual,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_WorkUnitName_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "WorkUnitName"
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_KFactor_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 2.3
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Core_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "GRO-A3"
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Frames_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 99
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_Atoms_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 0
         }));
      }

      [Test]
      public void Fetch_LessThanOrEqual_SlotType_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_LessThanOrEqual_PPD_Test()
      //{
      //   FetchTestData(38, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.LessThanOrEqual,
      //      Value = 9482.92683
      //   }));
      //}

      [Test]
      public void Fetch_LessThanOrEqual_Credit_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 450
         }));
      }

      [Test]
      public void Fetch_Like_ProjectID_Test()
      {
         FetchTestData(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.Like,
            Value = "10%"
         }));
      }

      [Test]
      public void Fetch_Like_ProjectRun_Test()
      {
         FetchTestData(8, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_Like_ProjectClone_Test()
      {
         FetchTestData(5, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.Like,
            Value = "9%"
         }));
      }

      [Test]
      public void Fetch_Like_ProjectGen_Test()
      {
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.Like,
            Value = "2%"
         }));
      }

      [Test]
      public void Fetch_Like_Name_Test()
      {
         FetchTestData(40, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.Like,
            Value = "nVidia GPU%"
         }));
      }

      [Test]
      public void Fetch_Like_Path_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.Like,
            Value = @"\\%\%"
         }));
      }

      [Test]
      public void Fetch_Like_Username_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.Like,
            Value = "h%"
         }));
      }

      [Test]
      public void Fetch_Like_Team_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.Like,
            Value = "%2"
         }));
      }

      [Test]
      public void Fetch_Like_CoreVersion_Test()
      {
         FetchTestData(15, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.Like,
            Value = "2%"
         }));
      }

      [Test]
      public void Fetch_Like_FramesCompleted_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_Like_FrameTime_Test()
      {
         FetchTestData(14, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.Like,
            Value = "4%"  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_Like_Result_Test()
      {
         FetchTestData(44, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.Like,
            Value = "1%"   // not a String value
         }));
      }

      [Test]
      public void Fetch_Like_DownloadDateTime_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_Like_CompletionDateTime_Test()
      {
         FetchTestData(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.Like,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_Like_WorkUnitName_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.Like,
            Value = "Work%Name%"
         }));
      }

      [Test]
      public void Fetch_Like_KFactor_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
      }

      [Test]
      public void Fetch_Like_Core_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.Like,
            Value = "GRO%"
         }));
      }

      [Test]
      public void Fetch_Like_Frames_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Like,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_Like_Atoms_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
      }

      [Test]
      public void Fetch_Like_SlotType_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.Like,
            Value = "%U"
         }));
      }

      //[Test]
      //public void Fetch_Like_PPD_Test()
      //{
      //   FetchTestData(9, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.Like,
      //      Value = "9%"
      //   }));
      //}

      [Test]
      public void Fetch_Like_Credit_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Credit,
            Type = QueryFieldType.Like,
            Value = "6%"
         }));
      }

      [Test]
      public void Fetch_NotLike_ProjectID_Test()
      {
         FetchTestData(30, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.NotLike,
            Value = "10%"
         }));
      }

      [Test]
      public void Fetch_NotLike_ProjectRun_Test()
      {
         FetchTestData(36, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_NotLike_ProjectClone_Test()
      {
         FetchTestData(39, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.NotLike,
            Value = "9%"
         }));
      }

      [Test]
      public void Fetch_NotLike_ProjectGen_Test()
      {
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.NotLike,
            Value = "2%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Name_Test()
      {
         FetchTestData(4, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.NotLike,
            Value = "nVidia GPU%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Path_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.NotLike,
            Value = @"\\%\%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Username_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.NotLike,
            Value = "h%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Team_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.NotLike,
            Value = "%2"
         }));
      }

      [Test]
      public void Fetch_NotLike_CoreVersion_Test()
      {
         FetchTestData(29, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.NotLike,
            Value = "2%"
         }));
      }

      [Test]
      public void Fetch_NotLike_FramesCompleted_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_NotLike_FrameTime_Test()
      {
         FetchTestData(30, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.NotLike,
            Value = "4%"  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_NotLike_Result_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.NotLike,
            Value = "1%"   // not a String value
         }));
      }

      [Test]
      public void Fetch_NotLike_DownloadDateTime_Test()
      {
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2010, 8, 22, 0, 42, 0)
         }));
      }

      [Test]
      public void Fetch_NotLike_CompletionDateTime_Test()
      {
         FetchTestData(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.NotLike,
            Value = new DateTime(2010, 8, 21, 20, 57, 0)
         }));
      }

      [Test]
      public void Fetch_NotLike_WorkUnitName_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.NotLike,
            Value = "Work%Name%"
         }));
      }

      [Test]
      public void Fetch_NotLike_KFactor_Test()
      {
         FetchTestData(3, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Core_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.NotLike,
            Value = "GRO%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Frames_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.NotLike,
            Value = "1%"
         }));
      }

      [Test]
      public void Fetch_NotLike_Atoms_Test()
      {
         FetchTestData(16, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.NotLike,
            Value = "0%"
         }));
      }

      [Test]
      public void Fetch_NotLike_SlotType_Test()
      {
         FetchTestData(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.NotLike,
            Value = "%U"
         }));
      }

      //[Test]
      //public void Fetch_NotLike_PPD_Test()
      //{
      //   FetchTestData(35, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.NotLike,
      //      Value = "9%"
      //   }));
      //}

      [Test]
      public void Fetch_NotLike_Credit_Test()
      {
         FetchTestData(13, BuildParameters(new QueryField
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
      public void Fetch_All_Test2()
      {
         // Select All
         FetchTestData2(253, BuildParameters());
      }

      [Test, TestCaseSource("FetchEqualCases2")]
      public void Fetch_Equal_Test2(int expected, QueryParameters parameters)
      {
         FetchTestData2(expected, parameters);
      }

      private static object[] FetchEqualCases2 = 
      {
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.ProjectID,          Type = QueryFieldType.Equal,  Value = 8011 }) },
         new object[] { 72,   BuildParameters(new QueryField { Name = QueryFieldName.ProjectRun,         Type = QueryFieldType.Equal,  Value = 0 }) },
         new object[] { 6,    BuildParameters(new QueryField { Name = QueryFieldName.ProjectClone,       Type = QueryFieldType.Equal,  Value = 63 }) },
         new object[] { 2,    BuildParameters(new QueryField { Name = QueryFieldName.ProjectGen,         Type = QueryFieldType.Equal,  Value = 188 }) },
         new object[] { 12,   BuildParameters(new QueryField { Name = QueryFieldName.Name,               Type = QueryFieldType.Equal,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 30,   BuildParameters(new QueryField { Name = QueryFieldName.Path,               Type = QueryFieldType.Equal,  Value = "192.168.0.172-36330" }) },
         new object[] { 252,  BuildParameters(new QueryField { Name = QueryFieldName.Username,           Type = QueryFieldType.Equal,  Value = "harlam357" }) },
         new object[] { 253,  BuildParameters(new QueryField { Name = QueryFieldName.Team,               Type = QueryFieldType.Equal,  Value = 32 }) },
         new object[] { 63,   BuildParameters(new QueryField { Name = QueryFieldName.CoreVersion,        Type = QueryFieldType.Equal,  Value = 2.27 }) },
         new object[] { 252,  BuildParameters(new QueryField { Name = QueryFieldName.FramesCompleted,    Type = QueryFieldType.Equal,  Value = 100 }) },
         new object[] { 14,   BuildParameters(new QueryField { Name = QueryFieldName.FrameTime,          Type = QueryFieldType.Equal,  Value = 100 })},  // not a TimeSpan value
         new object[] { 252,  BuildParameters(new QueryField { Name = QueryFieldName.Result,             Type = QueryFieldType.Equal,  Value = 1 }) },   // not a String value
         new object[] { 1,    BuildParameters(new QueryField { Name = QueryFieldName.DownloadDateTime,   Type = QueryFieldType.Equal,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 1,    BuildParameters(new QueryField { Name = QueryFieldName.CompletionDateTime, Type = QueryFieldType.Equal,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.WorkUnitName,       Type = QueryFieldType.Equal,  Value = "WorkUnitName3" }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.KFactor,            Type = QueryFieldType.Equal,  Value = 0.75 }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.Core,               Type = QueryFieldType.Equal,  Value = "GRO-A5" }) },
         new object[] { 20,   BuildParameters(new QueryField { Name = QueryFieldName.Frames,             Type = QueryFieldType.Equal,  Value = 100 }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.Atoms,              Type = QueryFieldType.Equal,  Value = 11000 }) },
         new object[] { 20,   BuildParameters(new QueryField { Name = QueryFieldName.SlotType,           Type = QueryFieldType.Equal,  Value = "CPU" }) },
         //new object[] { 1,    BuildParameters(new QueryField { Name = QueryFieldName.PPD,                Type = QueryFieldType.Equal,  Value = 486876.03173 }) },
         //new object[] { 2,    BuildParameters(new QueryField { Name = QueryFieldName.Credit,             Type = QueryFieldType.Equal,  Value = 869.4797 }) }
      };

      [Test, TestCaseSource("FetchGreaterThanCases2")]
      public void Fetch_GreaterThan_Test2(int expected, QueryParameters parameters)
      {
         FetchTestData2(expected, parameters);
      }

      private static object[] FetchGreaterThanCases2 = 
      {
         new object[] { 75,   BuildParameters(new QueryField { Name = QueryFieldName.ProjectID,          Type = QueryFieldType.GreaterThan,  Value = 7137 }) },
         new object[] { 47,   BuildParameters(new QueryField { Name = QueryFieldName.ProjectRun,         Type = QueryFieldType.GreaterThan,  Value = 18 }) },
         new object[] { 99,   BuildParameters(new QueryField { Name = QueryFieldName.ProjectClone,       Type = QueryFieldType.GreaterThan,  Value = 63 }) },
         new object[] { 146,  BuildParameters(new QueryField { Name = QueryFieldName.ProjectGen,         Type = QueryFieldType.GreaterThan,  Value = 188 }) },
         new object[] { 86,   BuildParameters(new QueryField { Name = QueryFieldName.Name,               Type = QueryFieldType.GreaterThan,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 197,  BuildParameters(new QueryField { Name = QueryFieldName.Path,               Type = QueryFieldType.GreaterThan,  Value = @"\\192.168.0.133\FAH\" }) },
         new object[] { 0,    BuildParameters(new QueryField { Name = QueryFieldName.Username,           Type = QueryFieldType.GreaterThan,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new QueryField { Name = QueryFieldName.Team,               Type = QueryFieldType.GreaterThan,  Value = 32 }) },
         new object[] { 166,  BuildParameters(new QueryField { Name = QueryFieldName.CoreVersion,        Type = QueryFieldType.GreaterThan,  Value = 2.15 }) },
         new object[] { 0,    BuildParameters(new QueryField { Name = QueryFieldName.FramesCompleted,    Type = QueryFieldType.GreaterThan,  Value = 100 }) },
         new object[] { 150,  BuildParameters(new QueryField { Name = QueryFieldName.FrameTime,          Type = QueryFieldType.GreaterThan,  Value = 100 })},  // not a TimeSpan value
         new object[] { 1,    BuildParameters(new QueryField { Name = QueryFieldName.Result,             Type = QueryFieldType.GreaterThan,  Value = 1 }) },   // not a String value
         new object[] { 42,   BuildParameters(new QueryField { Name = QueryFieldName.DownloadDateTime,   Type = QueryFieldType.GreaterThan,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 16,   BuildParameters(new QueryField { Name = QueryFieldName.CompletionDateTime, Type = QueryFieldType.GreaterThan,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.WorkUnitName,       Type = QueryFieldType.GreaterThan,  Value = "WorkUnitName3" }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.KFactor,            Type = QueryFieldType.GreaterThan,  Value = 0.75 }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.Core,               Type = QueryFieldType.GreaterThan,  Value = "GRO-A4" }) },
         new object[] { 0,    BuildParameters(new QueryField { Name = QueryFieldName.Frames,             Type = QueryFieldType.GreaterThan,  Value = 100 }) },
         new object[] { 10,   BuildParameters(new QueryField { Name = QueryFieldName.Atoms,              Type = QueryFieldType.GreaterThan,  Value = 9000 }) },
         new object[] { 0,    BuildParameters(new QueryField { Name = QueryFieldName.SlotType,           Type = QueryFieldType.GreaterThan,  Value = "CPU" }) },
         //new object[] { 5,    BuildParameters(new QueryField { Name = QueryFieldName.PPD,                Type = QueryFieldType.GreaterThan,  Value = 486876.03173 }) },
         //new object[] { 14,   BuildParameters(new QueryField { Name = QueryFieldName.Credit,             Type = QueryFieldType.GreaterThan,  Value = 869.4797 }) }
      };

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectID_Test2()
      {
         FetchTestData2(78, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 7137
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectRun_Test2()
      {
         FetchTestData2(51, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 18
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectClone_Test2()
      {
         FetchTestData2(105, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 63
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_ProjectGen_Test2()
      {
         FetchTestData2(148, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 188
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Name_Test2()
      {
         FetchTestData2(98, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "Windows - Test Workstation Slot 00"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Path_Test2()
      {
         FetchTestData2(205, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = @"\\192.168.0.133\FAH\"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Username_Test2()
      {
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Team_Test2()
      {
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_CoreVersion_Test2()
      {
         FetchTestData2(226, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 2.15
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_FramesCompleted_Test2()
      {
         FetchTestData2(252, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_FrameTime_Test2()
      {
         FetchTestData2(164, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Result_Test2()
      {
         FetchTestData2(253, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_DownloadDateTime_Test2()
      {
         FetchTestData2(43, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_CompletionDateTime_Test2()
      {
         FetchTestData2(17, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_WorkUnitName_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "WorkUnitName3"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_KFactor_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 0.75
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Core_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "GRO-A4"
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Frames_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_Atoms_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = 9000
         }));
      }

      [Test]
      public void Fetch_GreaterThanOrEqual_SlotType_Test2()
      {
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.GreaterThanOrEqual,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_GreaterThanOrEqual_PPD_Test2()
      //{
      //   FetchTestData2(6, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.GreaterThanOrEqual,
      //      Value = 486876.03173
      //   }));
      //}

      //[Test]
      //public void Fetch_GreaterThanOrEqual_Credit_Test2()
      //{
      //   FetchTestData2(16, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.Credit,
      //      Type = QueryFieldType.GreaterThanOrEqual,
      //      Value = 869.4797
      //   }));
      //}

      [Test]
      public void Fetch_LessThan_ProjectID_Test2()
      {
         FetchTestData2(175, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectID,
            Type = QueryFieldType.LessThan,
            Value = 7137
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectRun_Test2()
      {
         FetchTestData2(202, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectRun,
            Type = QueryFieldType.LessThan,
            Value = 18
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectClone_Test2()
      {
         FetchTestData2(148, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectClone,
            Type = QueryFieldType.LessThan,
            Value = 63
         }));
      }

      [Test]
      public void Fetch_LessThan_ProjectGen_Test2()
      {
         FetchTestData2(105, BuildParameters(new QueryField
         {
            Name = QueryFieldName.ProjectGen,
            Type = QueryFieldType.LessThan,
            Value = 188
         }));
      }

      [Test]
      public void Fetch_LessThan_Name_Test2()
      {
         FetchTestData2(155, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Name,
            Type = QueryFieldType.LessThan,
            Value = "Windows - Test Workstation Slot 00"
         }));
      }

      [Test]
      public void Fetch_LessThan_Path_Test2()
      {
         FetchTestData2(48, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Path,
            Type = QueryFieldType.LessThan,
            Value = @"\\192.168.0.133\FAH\"
         }));
      }

      [Test]
      public void Fetch_LessThan_Username_Test2()
      {
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Username,
            Type = QueryFieldType.LessThan,
            Value = "harlam357"
         }));
      }

      [Test]
      public void Fetch_LessThan_Team_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Team,
            Type = QueryFieldType.LessThan,
            Value = 32
         }));
      }

      [Test]
      public void Fetch_LessThan_CoreVersion_Test2()
      {
         FetchTestData2(27, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CoreVersion,
            Type = QueryFieldType.LessThan,
            Value = 2.15
         }));
      }

      [Test]
      public void Fetch_LessThan_FramesCompleted_Test2()
      {
         FetchTestData2(1, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FramesCompleted,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_LessThan_FrameTime_Test2()
      {
         FetchTestData2(89, BuildParameters(new QueryField
         {
            Name = QueryFieldName.FrameTime,
            Type = QueryFieldType.LessThan,
            Value = 100  // not a TimeSpan value
         }));
      }

      [Test]
      public void Fetch_LessThan_Result_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Result,
            Type = QueryFieldType.LessThan,
            Value = 1   // not a String value
         }));
      }

      [Test]
      public void Fetch_LessThan_DownloadDateTime_Test2()
      {
         FetchTestData2(210, BuildParameters(new QueryField
         {
            Name = QueryFieldName.DownloadDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2012, 7, 5, 0, 25, 7)
         }));
      }

      [Test]
      public void Fetch_LessThan_CompletionDateTime_Test2()
      {
         FetchTestData2(236, BuildParameters(new QueryField
         {
            Name = QueryFieldName.CompletionDateTime,
            Type = QueryFieldType.LessThan,
            Value = new DateTime(2012, 11, 19, 6, 56, 47)
         }));
      }

      [Test]
      public void Fetch_LessThan_WorkUnitName_Test2()
      {
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThan,
            Value = "WorkUnitName4"
         }));
      }

      [Test]
      public void Fetch_LessThan_KFactor_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThan,
            Value = 0.75
         }));
      }

      [Test]
      public void Fetch_LessThan_Core_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThan,
            Value = "GRO-A4"
         }));
      }

      [Test]
      public void Fetch_LessThan_Frames_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThan,
            Value = 100
         }));
      }

      [Test]
      public void Fetch_LessThan_Atoms_Test2()
      {
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThan,
            Value = 11000
         }));
      }

      [Test]
      public void Fetch_LessThan_SlotType_Test2()
      {
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThan,
            Value = "CPU"
         }));
      }

      //[Test]
      //public void Fetch_LessThan_PPD_Test2()
      //{
      //   FetchTestData2(247, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.PPD,
      //      Type = QueryFieldType.LessThan,
      //      Value = 486876.03173
      //   }));
      //}

      //[Test]
      //public void Fetch_LessThan_Credit_Test2()
      //{
      //   FetchTestData2(10, BuildParameters(new QueryField
      //   {
      //      Name = QueryFieldName.Credit,
      //      Type = QueryFieldType.LessThan,
      //      Value = 869.4797
      //   }));
      //}

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
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.WorkUnitName,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "WorkUnitName4"
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.KFactor,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 0.75
         }));
         FetchTestData2(10, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Core,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "GRO-A4"
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 100
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Atoms,
            Type = QueryFieldType.LessThanOrEqual,
            Value = 11000
         }));
         FetchTestData2(20, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.LessThanOrEqual,
            Value = "CPU"
         }));
         //FetchTestData2(248, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.PPD,
         //   Type = QueryFieldType.LessThanOrEqual,
         //   Value = 486876.03173
         //}));
         //FetchTestData2(239, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.Credit,
         //   Type = QueryFieldType.LessThanOrEqual,
         //   Value = 869.4797
         //}));
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
         FetchTestData2(10, BuildParameters(new QueryField
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
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.Frames,
            Type = QueryFieldType.Like,
            Value = "0%"
         }));
         FetchTestData2(0, BuildParameters(new QueryField
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
         //FetchTestData2(10, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.PPD,
         //   Type = QueryFieldType.Like,
         //   Value = "1%"
         //}));
         //FetchTestData2(3, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.Credit,
         //   Type = QueryFieldType.Like,
         //   Value = "9%"
         //}));
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
         FetchTestData2(0, BuildParameters(new QueryField
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
         FetchTestData2(0, BuildParameters(new QueryField
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
         FetchTestData2(0, BuildParameters(new QueryField
         {
            Name = QueryFieldName.SlotType,
            Type = QueryFieldType.NotLike,
            Value = "%U"
         }));
         //FetchTestData2(243, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.PPD,
         //   Type = QueryFieldType.NotLike,
         //   Value = "1%"
         //}));
         //FetchTestData2(250, BuildParameters(new QueryField
         //{
         //   Name = QueryFieldName.Credit,
         //   Type = QueryFieldType.NotLike,
         //   Value = "9%"
         //}));
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
         _database.Upgrade();
         FetchInternal(count, parameters, HistoryProductionView.BonusDownloadTime);
      }

      private void FetchTestData2(int count, QueryParameters parameters)
      {
         _database.DatabaseFilePath = _testData2FileCopy;
         _database.Upgrade();
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
   }
}
