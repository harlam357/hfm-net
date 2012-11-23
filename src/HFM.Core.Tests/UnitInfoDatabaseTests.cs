/*
 * HFM.NET - Work Unit History Database Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class UnitInfoDatabaseTests
   {
      private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";
      private readonly string _testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");
      private const string TestScratchFile = "UnitInfoTest.db3";

      private UnitInfoDatabase _database;
      private readonly IProteinDictionary _proteinDictionary = CreateProteinDictionary();

      [SetUp]
      public void Init()
      {
         if (File.Exists(TestScratchFile))
         {
            File.Delete(TestScratchFile);
         }

         _database = new UnitInfoDatabase(null, _proteinDictionary);
      }

      [TearDown]
      public void Destroy()
      {
         if (File.Exists(_testDataFileCopy))
         {
            File.Delete(_testDataFileCopy);
         }
         if (File.Exists(TestScratchFile))
         {
            File.Delete(TestScratchFile);
         }
      }

      [Test]
      public void TableExistsAndDropTableTest()
      {
         SetupTestDataFileCopy();

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
         Assert.AreEqual(true, _database.Connected);
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
         SetupTestDataFileCopy();

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
         _database.DatabaseFilePath = TestDataFile;
         Assert.AreEqual(0, _database.Delete(new HistoryEntry { ID = 100 }));
      }

      #endregion

      #region Fetch

      [Test]
      public void FetchTest1()
      {
         FetchTestInternal(44, new QueryParameters());
      }

      [Test]
      public void FetchTest2()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               { 
                                  Name = QueryFieldName.ProjectID, 
                                  Type = QueryFieldType.Equal, 
                                  Value = 6600 
                               });
         FetchTestInternal(13, parameters);
      }

      [Test]
      public void FetchTest3()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.DownloadDateTime,
                                  Type = QueryFieldType.GreaterThanOrEqual,
                                  Value = new DateTime(2010, 8, 20)
                               });
         FetchTestInternal(25, parameters);
      }

      [Test]
      public void FetchTest4()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.DownloadDateTime,
                                  Type = QueryFieldType.GreaterThan,
                                  Value = new DateTime(2010, 8, 8)
                               });
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.DownloadDateTime,
                                  Type = QueryFieldType.LessThan,
                                  Value = new DateTime(2010, 8, 22)
                               });
         FetchTestInternal(33, parameters);
      }

      [Test]
      public void FetchTest5()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.WorkUnitName,
                                  Type = QueryFieldType.Equal,
                                  Value = "WorkUnitName"
                               });
         FetchTestInternal(13, parameters);
      }

      [Test]
      public void FetchTest6()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.WorkUnitName,
                                  Type = QueryFieldType.Equal,
                                  Value = "WorkUnitName2"
                               });
         FetchTestInternal(3, parameters);
      }

      [Test]
      public void FetchTest7()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.Atoms,
                                  Type = QueryFieldType.GreaterThan,
                                  Value = 5000
                               });
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.Atoms,
                                  Type = QueryFieldType.LessThanOrEqual,
                                  Value = 7000
                               });
         FetchTestInternal(3, parameters);
      }

      [Test]
      public void FetchTest8()
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField
                               {
                                  Name = QueryFieldName.Core, 
                                  Type = QueryFieldType.Equal, 
                                  Value = "GROGPU2"
                               });
         FetchTestInternal(16, parameters);
      }

      public void FetchTestInternal(int count, QueryParameters parameters)
      {
         _database.DatabaseFilePath = TestDataFile;
         var entries = _database.Fetch(parameters);
         foreach (var entry in entries)
         {
            Debug.WriteLine(entry.ID);
         }
         Assert.AreEqual(count, entries.Count);
      }

      #endregion

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
         protein.KFactor = 0;
         protein.Frames = 100;
         protein.NumberOfAtoms = 7000;
         protein.PreferredDays = 2;
         protein.MaximumDays = 3;
         proteins.Add(protein.ProjectNumber, protein);

         return proteins;
      }

      private void SetupTestDataFileCopy()
      {
         if (File.Exists(_testDataFileCopy))
         {
            File.Delete(_testDataFileCopy);
         }

         File.Copy(TestDataFile, _testDataFileCopy, true);
         // sometimes the file is not finished
         // copying before we attempt to open
         // the copied file.
         Thread.Sleep(100);
      }
   }
}
