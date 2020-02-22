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

using AutoMapper;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Client;
using HFM.Core.Configuration;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRepositoryTests
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

        private WorkUnitRepository _repository;
        private readonly IProteinService _proteinService = CreateProteinService();

        #region Setup and TearDown

        [OneTimeSetUp]
        public void FixtureInit()
        {
            Mapper.Initialize(c => c.AddProfile<AutoMapperProfile>());
        }

        [SetUp]
        public void Init()
        {
            SetupTestDataFileCopies();

            if (File.Exists(TestScratchFile))
            {
                File.Delete(TestScratchFile);
            }

            _repository = new WorkUnitRepository(_proteinService, null);
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
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }

        [OneTimeTearDown]
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
            _repository.Initialize(TestScratchFile);

            Parallel.For(0, 100, i =>
                                 {
                                     Debug.WriteLine("Writing unit {0:00} on thread id: {1:00}", i, Thread.CurrentThread.ManagedThreadId);

                                     var workUnitModel = new WorkUnitModel();
                                     workUnitModel.CurrentProtein = BuildProtein1();
                                     workUnitModel.Data = BuildWorkUnit1(i);

                                     _repository.Insert(workUnitModel);
                                 });

            Assert.AreEqual(100, _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None).Count);
        }

        #region Connected

        [Test]
        public void Connected_Test1()
        {
            _repository.Initialize(TestScratchFile);
            VerifyWuHistoryTableSchema(TestScratchFile);
            Assert.AreEqual(Application.Version, _repository.GetDatabaseVersion());
            Assert.AreEqual(true, _repository.Connected);
        }

        #endregion

        #region Upgrade

        [Test]
        public void Upgrade_v092_Test1()
        {
            Assert.AreEqual(15, GetWuHistoryColumnCount(_testDataFileCopy));
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
            _repository.Initialize(_testDataFileCopy);
            VerifyWuHistoryTableSchema(_testDataFileCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
            Assert.AreEqual(Application.ParseVersion("0.9.2"), Application.ParseVersion(_repository.GetDatabaseVersion()));
        }

        [Test]
        public void Upgrade_v092_AlreadyUpgraded_Test()
        {
            VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
            _repository.Initialize(_testDataFileUpgradedCopy);
            VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
            Assert.IsTrue(Application.ParseVersion("0.9.2") <= Application.ParseVersion(_repository.GetDatabaseVersion()));
        }

        [Test]
        public void Upgrade_v092_Test2()
        {
            Assert.AreEqual(15, GetWuHistoryColumnCount(_testData2FileCopy));
            Assert.AreEqual(285, GetWuHistoryRowCount(_testData2FileCopy));
            _repository.Initialize(_testData2FileCopy);
            VerifyWuHistoryTableSchema(_testData2FileCopy);
            // 32 duplicates deleted
            Assert.AreEqual(253, GetWuHistoryRowCount(_testData2FileCopy));
            Assert.AreEqual(Application.ParseVersion("0.9.2"), Application.ParseVersion(_repository.GetDatabaseVersion()));
        }

        #endregion

        #region Insert

        [Test]
        public void Insert_Test1()
        {
            InsertTestInternal(BuildWorkUnit1(), BuildProtein1(), BuildWorkUnit1VerifyAction());
        }

        [Test]
        public void Insert_Test1_CzechCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            InsertTestInternal(BuildWorkUnit1(), BuildProtein1(), BuildWorkUnit1VerifyAction());
        }

        [Test]
        public void Insert_Test2()
        {
            InsertTestInternal(BuildWorkUnit2(), BuildProtein2(), BuildWorkUnit2VerifyAction());
        }

        [Test]
        public void Insert_Test3()
        {
            InsertTestInternal(BuildWorkUnit3(), BuildProtein3(), BuildWorkUnit3VerifyAction());
        }

        [Test]
        public void Insert_Test4()
        {
            InsertTestInternal(BuildWorkUnit4(), BuildProtein4(), BuildWorkUnit4VerifyAction());
        }

        private void InsertTestInternal(WorkUnit workUnit, Protein protein, Action<IList<WorkUnitRow>> verifyAction)
        {
            _repository.Initialize(TestScratchFile);

            var workUnitModel = new WorkUnitModel();
            workUnitModel.CurrentProtein = protein;
            workUnitModel.Data = workUnit;

            _repository.Insert(workUnitModel);

            var rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            verifyAction(rows);

            // test code to ensure this unit is NOT written again
            _repository.Insert(workUnitModel);
            // verify
            rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            Assert.AreEqual(1, rows.Count);
        }

        private static WorkUnit BuildWorkUnit1()
        {
            return BuildWorkUnit1(1);
        }

        private static WorkUnit BuildWorkUnit1(int run)
        {
            var workUnit = new WorkUnit();

            workUnit.ProjectID = 2669;
            workUnit.ProjectRun = run;
            workUnit.ProjectClone = 2;
            workUnit.ProjectGen = 3;
            workUnit.OwningClientName = "Owner";
            workUnit.OwningClientPath = "Path";
            //workUnit.OwningSlotId = 
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = 2.09f;
            workUnit.UnitResult = WorkUnitResult.FinishedUnit;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.DownloadTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            workUnit.FinishedTime = new DateTime(2010, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.FrameData = frameDataDictionary;
            return workUnit;
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

        private static Action<IList<WorkUnitRow>> BuildWorkUnit1VerifyAction()
        {
            return rows =>
            {
                Assert.AreEqual(1, rows.Count);
                WorkUnitRow row = rows[0];
                Assert.AreEqual(2669, row.ProjectID);
                Assert.AreEqual(1, row.ProjectRun);
                Assert.AreEqual(2, row.ProjectClone);
                Assert.AreEqual(3, row.ProjectGen);
                Assert.AreEqual("Owner", row.Name);
                Assert.AreEqual("Path", row.Path);
                Assert.AreEqual("harlam357", row.Username);
                Assert.AreEqual(32, row.Team);
                Assert.AreEqual(2.09f, row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.FromSeconds(600), row.FrameTime);
                Assert.AreEqual((int)WorkUnitResult.FinishedUnit, row.ResultValue);
                Assert.AreEqual(new DateTime(2010, 1, 1), row.DownloadDateTime);
                Assert.AreEqual(new DateTime(2010, 1, 2), row.CompletionDateTime);
                Assert.AreEqual("TestUnit1", row.WorkUnitName);
                Assert.AreEqual(1.0, row.KFactor);
                Assert.AreEqual("GRO-A3", row.Core);
                Assert.AreEqual(100, row.Frames);
                Assert.AreEqual(1000, row.Atoms);
                Assert.AreEqual(100.0, row.BaseCredit);
                Assert.AreEqual(3.0, row.PreferredDays);
                Assert.AreEqual(5.0, row.MaximumDays);
                Assert.AreEqual(SlotType.CPU.ToString(), row.SlotType);
            };
        }

        private static WorkUnit BuildWorkUnit2()
        {
            var workUnit = new WorkUnit();

            workUnit.ProjectID = 6900;
            workUnit.ProjectRun = 4;
            workUnit.ProjectClone = 5;
            workUnit.ProjectGen = 6;
            workUnit.OwningClientName = "Owner's";
            workUnit.OwningClientPath = "The Path's";
            //workUnit.OwningSlotId = 
            workUnit.FoldingID = "harlam357's";
            workUnit.Team = 100;
            workUnit.CoreVersion = 2.27f;
            workUnit.UnitResult = WorkUnitResult.EarlyUnitEnd;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.DownloadTime = new DateTime(2009, 5, 5);
            workUnit.FinishedTime = new DateTime(2009, 5, 6);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 56, Duration = TimeSpan.FromSeconds(1000) });
            workUnit.FrameData = frameDataDictionary;
            return workUnit;
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

        private static Action<IList<WorkUnitRow>> BuildWorkUnit2VerifyAction()
        {
            return rows =>
            {
                Assert.AreEqual(1, rows.Count);
                WorkUnitRow row = rows[0];
                Assert.AreEqual(6900, row.ProjectID);
                Assert.AreEqual(4, row.ProjectRun);
                Assert.AreEqual(5, row.ProjectClone);
                Assert.AreEqual(6, row.ProjectGen);
                Assert.AreEqual("Owner's", row.Name);
                Assert.AreEqual("The Path's", row.Path);
                Assert.AreEqual("harlam357's", row.Username);
                Assert.AreEqual(100, row.Team);
                Assert.AreEqual(2.27f, row.CoreVersion);
                Assert.AreEqual(56, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.FromSeconds(1000), row.FrameTime);
                Assert.AreEqual((int)WorkUnitResult.EarlyUnitEnd, row.ResultValue);
                Assert.AreEqual(new DateTime(2009, 5, 5), row.DownloadDateTime);
                Assert.AreEqual(new DateTime(2009, 5, 6), row.CompletionDateTime);
                Assert.AreEqual("TestUnit2", row.WorkUnitName);
                Assert.AreEqual(2.0, row.KFactor);
                Assert.AreEqual("GRO-A4", row.Core);
                Assert.AreEqual(200, row.Frames);
                Assert.AreEqual(2000, row.Atoms);
                Assert.AreEqual(200.0, row.BaseCredit);
                Assert.AreEqual(6.0, row.PreferredDays);
                Assert.AreEqual(10.0, row.MaximumDays);
                Assert.AreEqual(SlotType.CPU.ToString(), row.SlotType);
            };
        }

        private static WorkUnit BuildWorkUnit3()
        {
            var workUnit = new WorkUnit();

            workUnit.ProjectID = 2670;
            workUnit.ProjectRun = 2;
            workUnit.ProjectClone = 3;
            workUnit.ProjectGen = 4;
            workUnit.OwningClientName = "Owner";
            workUnit.OwningClientPath = "Path";
            //workUnit.OwningSlotId = 
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = 2.09f;
            workUnit.UnitResult = WorkUnitResult.EarlyUnitEnd;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.DownloadTime = new DateTime(2010, 2, 2);
            workUnit.FinishedTime = new DateTime(2010, 2, 3);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            //workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.FrameData = frameDataDictionary;
            return workUnit;
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

        private static Action<IList<WorkUnitRow>> BuildWorkUnit3VerifyAction()
        {
            return rows =>
            {
                Assert.AreEqual(1, rows.Count);
                WorkUnitRow row = rows[0];
                Assert.AreEqual(2670, row.ProjectID);
                Assert.AreEqual(2, row.ProjectRun);
                Assert.AreEqual(3, row.ProjectClone);
                Assert.AreEqual(4, row.ProjectGen);
                Assert.AreEqual("Owner", row.Name);
                Assert.AreEqual("Path", row.Path);
                Assert.AreEqual("harlam357", row.Username);
                Assert.AreEqual(32, row.Team);
                Assert.AreEqual(2.09f, row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.Zero, row.FrameTime);
                Assert.AreEqual((int)WorkUnitResult.EarlyUnitEnd, row.ResultValue);
                Assert.AreEqual(new DateTime(2010, 2, 2), row.DownloadDateTime);
                Assert.AreEqual(new DateTime(2010, 2, 3), row.CompletionDateTime);
                Assert.AreEqual("TestUnit3", row.WorkUnitName);
                Assert.AreEqual(3.0, row.KFactor);
                Assert.AreEqual("GRO-A5", row.Core);
                Assert.AreEqual(300, row.Frames);
                Assert.AreEqual(3000, row.Atoms);
                Assert.AreEqual(300.0, row.BaseCredit);
                Assert.AreEqual(7.0, row.PreferredDays);
                Assert.AreEqual(12.0, row.MaximumDays);
                Assert.AreEqual(SlotType.CPU.ToString(), row.SlotType);
            };
        }

        private static WorkUnit BuildWorkUnit4()
        {
            var workUnit = new WorkUnit();

            workUnit.ProjectID = 6903;
            workUnit.ProjectRun = 2;
            workUnit.ProjectClone = 3;
            workUnit.ProjectGen = 4;
            workUnit.OwningClientName = "Owner2";
            workUnit.OwningClientPath = "Path2";
            workUnit.OwningSlotId = 2;
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = 2.27f;
            workUnit.UnitResult = WorkUnitResult.FinishedUnit;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.DownloadTime = new DateTime(2012, 1, 2);
            workUnit.FinishedTime = new DateTime(2012, 1, 5);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            //workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.FrameData = frameDataDictionary;
            return workUnit;
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

        private static Action<IList<WorkUnitRow>> BuildWorkUnit4VerifyAction()
        {
            return rows =>
            {
                Assert.AreEqual(1, rows.Count);
                WorkUnitRow row = rows[0];
                Assert.AreEqual(6903, row.ProjectID);
                Assert.AreEqual(2, row.ProjectRun);
                Assert.AreEqual(3, row.ProjectClone);
                Assert.AreEqual(4, row.ProjectGen);
                Assert.AreEqual("Owner2 Slot 02", row.Name);
                Assert.AreEqual("Path2", row.Path);
                Assert.AreEqual("harlam357", row.Username);
                Assert.AreEqual(32, row.Team);
                Assert.AreEqual(2.27f, row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.Zero, row.FrameTime);
                Assert.AreEqual((int)WorkUnitResult.FinishedUnit, row.ResultValue);
                Assert.AreEqual(new DateTime(2012, 1, 2), row.DownloadDateTime);
                Assert.AreEqual(new DateTime(2012, 1, 5), row.CompletionDateTime);
                Assert.AreEqual("TestUnit4", row.WorkUnitName);
                Assert.AreEqual(4.0, row.KFactor);
                Assert.AreEqual("OPENMMGPU", row.Core);
                Assert.AreEqual(400, row.Frames);
                Assert.AreEqual(4000, row.Atoms);
                Assert.AreEqual(400.0, row.BaseCredit);
                Assert.AreEqual(2.0, row.PreferredDays);
                Assert.AreEqual(5.0, row.MaximumDays);
                Assert.AreEqual(SlotType.GPU.ToString(), row.SlotType);
            };
        }

        #endregion

        #region Delete

        [Test]
        public void Delete_Test()
        {
            _repository.Initialize(_testDataFileCopy);
            var entries = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            Assert.AreEqual(44, entries.Count);
            Assert.AreEqual(1, _repository.Delete(entries[14]));
            entries = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            Assert.AreEqual(43, entries.Count);
        }

        [Test]
        public void Delete_NotExist_Test()
        {
            _repository.Initialize(_testDataFileCopy);
            Assert.AreEqual(0, _repository.Delete(new WorkUnitRow { ID = 100 }));
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
            var collection = new List<Protein>();

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
            collection.Add(protein);

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
            collection.Add(protein);

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
            collection.Add(protein);

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
            collection.Add(protein);

            var dataContainer = new ProteinDataContainer();
            dataContainer.Data = collection;
            return new ProteinService(dataContainer, null, null);
        }

        #endregion
    }
}
