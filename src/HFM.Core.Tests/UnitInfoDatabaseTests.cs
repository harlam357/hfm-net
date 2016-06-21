/*
 * HFM.NET - Work Unit History Database Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
      private const string TestDataFileUpgraded = "..\\..\\TestFiles\\TestData_1.db3";
      private readonly string _testDataFileUpgradedCopy = Path.ChangeExtension(TestDataFileUpgraded, ".dbcopy");
      
      private const string TestScratchFile = "UnitInfoTest.db3";

      private UnitInfoDatabase _database;
      private readonly IProteinService _proteinService = CreateProteinService();

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

         _database = new UnitInfoDatabase(null, _proteinService);
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

         File.Copy(TestDataFileUpgraded, _testDataFileUpgradedCopy, true);
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
      public void MultiThread_Test()
      {
         _database.DatabaseFilePath = TestScratchFile;
         var benchmarkCollection = MockRepository.GenerateStub<IProteinBenchmarkCollection>();
         
         Parallel.For(0, 100, i =>
                              {
                                 Debug.WriteLine("Writing unit {0:00} on thread id: {1:00}", i, Thread.CurrentThread.ManagedThreadId);
 
                                 var unitInfoLogic = new UnitInfoModel(benchmarkCollection);
                                 unitInfoLogic.CurrentProtein = BuildProtein1();
                                 unitInfoLogic.UnitInfoData = BuildUnitInfo1(i);

                                 _database.Insert(unitInfoLogic);
                              });

         Assert.AreEqual(100, _database.Fetch(QueryParameters.SelectAll).Count);
      }

      [Test]
      public void TableExists_DropTable_Test()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         Assert.AreEqual(true, _database.TableExists(SqlTable.WuHistory));
         _database.DropTable(SqlTable.WuHistory);
         Assert.AreEqual(false, _database.TableExists(SqlTable.WuHistory));
      }

      #region Connected

      [Test]
      public void Connected_Test1()
      {
         _database.DatabaseFilePath = TestScratchFile;
         VerifyWuHistoryTableSchema(TestScratchFile);
         Assert.AreEqual(Application.Version, _database.GetDatabaseVersion());
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
         VerifyWuHistoryTableSchema(_testDataFileCopy);
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
         Assert.AreEqual(Application.ParseVersion("0.9.2"), Application.ParseVersion(_database.GetDatabaseVersion()));
      }

      [Test]
      public void Upgrade_v092_AlreadyUpgraded_Test()
      {
         VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
         _database.DatabaseFilePath = _testDataFileUpgradedCopy;
         _database.Upgrade();
         VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
         Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
         Assert.IsTrue(Application.ParseVersion("0.9.2") <= Application.ParseVersion(_database.GetDatabaseVersion()));
      }

      [Test]
      public void Upgrade_v092_Test2()
      {
         Assert.AreEqual(15, GetWuHistoryColumnCount(_testData2FileCopy));
         Assert.AreEqual(285, GetWuHistoryRowCount(_testData2FileCopy));
         _database.DatabaseFilePath = _testData2FileCopy;
         _database.Upgrade();
         VerifyWuHistoryTableSchema(_testData2FileCopy);
         // 32 duplicates deleted
         Assert.AreEqual(253, GetWuHistoryRowCount(_testData2FileCopy));
         Assert.AreEqual(Application.ParseVersion("0.9.2"), Application.ParseVersion(_database.GetDatabaseVersion()));
      }

      #endregion

      #region Insert

      [Test]
      public void Insert_Test1()
      {
         InsertTestInternal(BuildUnitInfo1(), BuildProtein1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void Insert_Test1_CzechCulture()
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
         InsertTestInternal(BuildUnitInfo1(), BuildProtein1(), BuildUnitInfo1VerifyAction());
      }

      [Test]
      public void Insert_Test2()
      {
         InsertTestInternal(BuildUnitInfo2(), BuildProtein2(), BuildUnitInfo2VerifyAction());
      }

      [Test]
      public void Insert_Test3()
      {
         InsertTestInternal(BuildUnitInfo3(), BuildProtein3(), BuildUnitInfo3VerifyAction());
      }

      [Test]
      public void Insert_Test4()
      {
         InsertTestInternal(BuildUnitInfo4(), BuildProtein4(), BuildUnitInfo4VerifyAction());
      }

      private void InsertTestInternal(UnitInfo unitInfo, Protein protein, Action<IList<HistoryEntry>> verifyAction)
      {
         _database.DatabaseFilePath = TestScratchFile;

         var unitInfoLogic = new UnitInfoModel(MockRepository.GenerateStub<IProteinBenchmarkCollection>());
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
      public void Delete_Test()
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
      public void Delete_NotExist_Test()
      {
         _database.DatabaseFilePath = _testDataFileCopy;
         _database.Upgrade();
         Assert.AreEqual(0, _database.Delete(new HistoryEntry { ID = 100 }));
      }

      #endregion

      #region Static Helpers

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

      private static void VerifyWuHistoryTableSchema(string dataSource)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
         {
            con.Open();
            using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", con))
            using (var table = new DataTable())
            {
               adapter.Fill(table);
               Assert.AreEqual(23, table.Rows.Count);

               for (int i = 0; i < table.Rows.Count; i++)
               {
                  var row = table.Rows[i];
                  // notnull check
                  Assert.AreEqual(1, row[3]);
                  // dflt_value check
                  if (i < 15)
                  {
                     Assert.IsTrue(row[4].Equals(DBNull.Value));
                  }
                  else
                  {
                     Assert.IsFalse(row[4].Equals(DBNull.Value));
                  }
                  // pk check
                  Assert.AreEqual(i == 0 ? 1 : 0, row[5]);
               }

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

      public static IProteinService CreateProteinService()
      {
         var service = new ProteinService();

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
         service.Add(protein);

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
         service.Add(protein);

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
         service.Add(protein);

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
         service.Add(protein);

         return service;
      }

      #endregion
   }
}
