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
      private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";

      private readonly IProteinDictionary _proteinDictionary = CreateProteinDictionary();

      [Test]
      public void WriteUnitInfoTest1()
      {
         const string testFile = "UnitInfoTest1.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = 1;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;
         unitInfo.DownloadTime = new DateTime(2010, 1, 1);
         unitInfo.FinishedTime = new DateTime(2010, 1, 2);
         unitInfo.FramesObserved = 2;
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });

         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
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
         
         // test code to ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);
         // verify
         rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);

         File.Delete(testFile);
      }

      [Test]
      public void WriteUnitInfoTest1CzechCulture()
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");

         const string testFile = "UnitInfoTest1.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = 1;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.FinishedUnit;
         unitInfo.DownloadTime = new DateTime(2010, 1, 1);
         unitInfo.FinishedTime = new DateTime(2010, 1, 2);
         unitInfo.FramesObserved = 2;
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });

         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
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

         // test code to ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);
         // verify
         rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);

         File.Delete(testFile);
      }

      [Test]
      public void WriteUnitInfoTest2()
      {
         const string testFile = "UnitInfoTest2.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 6900;
         unitInfo.ProjectRun = 4;
         unitInfo.ProjectClone = 5;
         unitInfo.ProjectGen = 6;
         unitInfo.OwningClientName = "Owner's";
         unitInfo.OwningClientPath = "The Path's";
         unitInfo.FoldingID = "harlam357's";
         unitInfo.Team = 100;
         unitInfo.CoreVersion = 2.27f;
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2009, 5, 5);
         unitInfo.FinishedTime = DateTime.MinValue;
         unitInfo.FramesObserved = 2;
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 55, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 56, TimeOfFrame = TimeSpan.FromSeconds(1000) });

         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
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

         // test code to ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);
         // verify
         rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);

         File.Delete(testFile);
      }

      [Test]
      public void WriteUnitInfoTest3()
      {
         const string testFile = "UnitInfoTest3.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var unitInfo = new UnitInfo();
         unitInfo.ProjectID = 2670;
         unitInfo.ProjectRun = 2;
         unitInfo.ProjectClone = 3;
         unitInfo.ProjectGen = 4;
         unitInfo.OwningClientName = "Owner";
         unitInfo.OwningClientPath = "Path";
         unitInfo.FoldingID = "harlam357";
         unitInfo.Team = 32;
         unitInfo.CoreVersion = 2.09f;
         unitInfo.UnitResult = WorkUnitResult.EarlyUnitEnd;
         unitInfo.DownloadTime = new DateTime(2010, 2, 2);
         unitInfo.FinishedTime = DateTime.MinValue;
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });

         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
         HistoryEntry entry = rows[0];
         Assert.AreEqual("Owner", entry.Name);
         Assert.AreEqual("Path", entry.Path);
         Assert.AreEqual(2670, entry.ProjectID);
         Assert.AreEqual(2, entry.ProjectRun);
         Assert.AreEqual(3, entry.ProjectClone);
         Assert.AreEqual(4, entry.ProjectGen);
         Assert.AreEqual(WorkUnitResult.EarlyUnitEnd, entry.Result.ToWorkUnitResult());
         Assert.AreEqual(new DateTime(2010, 2, 2), entry.DownloadDateTime);
         Assert.AreEqual(DateTime.MinValue, entry.CompletionDateTime);

         File.Delete(testFile);
      }

      [Test]
      public void WriteUnitInfoTest4()
      {
         const string testFile = "UnitInfoTest4.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

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
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 99, TimeOfFrame = TimeSpan.Zero });
         unitInfo.SetCurrentFrame(new UnitFrame { FrameID = 100, TimeOfFrame = TimeSpan.FromMinutes(10) });

         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Count);
         HistoryEntry entry = rows[0];
         Assert.AreEqual("Owner2 Slot 02", entry.Name);
         Assert.AreEqual("Path2", entry.Path);
         Assert.AreEqual(6903, entry.ProjectID);
         Assert.AreEqual(2, entry.ProjectRun);
         Assert.AreEqual(3, entry.ProjectClone);
         Assert.AreEqual(4, entry.ProjectGen);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, entry.Result.ToWorkUnitResult());
         Assert.AreEqual(new DateTime(2012, 1, 2), entry.DownloadDateTime);
         Assert.AreEqual(new DateTime(2012, 1, 5), entry.CompletionDateTime);

         File.Delete(testFile);
      }
      
      [Test]
      public void DeleteAllUnitInfoDataTest()
      {
         const string testFile = "UnitInfoTest4.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         Assert.AreEqual(true, database.TableExists());
         database.DeleteAllUnitInfoData();
         Assert.AreEqual(false, database.TableExists());

         File.Delete(testFile);
      }
      
      [Test]
      public void DeleteUnitInfoTest()
      {
         string testFile = Path.ChangeExtension(TestDataFile, ".dbcopy");
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         //var t = new Thread(CopyTestFile);
         //t.Start();
         //t.Join(3000);

         CopyTestFile();
         // sometimes the file is not finished
         // copying before we attempt to open
         // the copied file.
         Thread.Sleep(100);

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         Assert.AreEqual(44, database.QueryUnitData(new QueryParameters()).Count);
         Assert.AreEqual(1, database.DeleteUnitInfo(15));
         Assert.AreEqual(43, database.QueryUnitData(new QueryParameters()).Count);

         File.Delete(testFile);
      }
      
      private static void CopyTestFile()
      {
         string testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");
         File.Copy(TestDataFile, testDataFileCopy, true);
      }

      [Test]
      public void DeleteUnitInfoTableNotExistTest()
      {
         const string testFile = "NewDatabase.db3";
         if (File.Exists(testFile))
         {
            File.Delete(testFile);
         }

         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = testFile;
         Assert.AreEqual(0, database.DeleteUnitInfo(100));

         File.Delete(testFile);
      }

      [Test]
      public void DeleteUnitInfoUnitNotExistTest()
      {
         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = TestDataFile;
         Assert.AreEqual(0, database.DeleteUnitInfo(100));
      }

      //[Test]
      //public void ImportCompletedUnitsTest()
      //{
      //   if (File.Exists(TestFile))
      //   {
      //      File.Delete(TestFile);
      //   }
         
      //   _mocks.ReplayAll();

      //   var database = new UnitInfoDatabase(_proteinDictionary) { DatabaseFilePath = TestFile };
      //   var completedUnitsReader = new CompletedUnitsFileReader { CompletedUnitsFilePath = "..\\..\\TestFiles\\CompletedUnits.csv" };
      //   completedUnitsReader.Process();
      //   database.ImportCompletedUnits(completedUnitsReader.Result.Entries);

      //   var rows = database.QueryUnitData(new QueryParameters());
      //   Assert.AreEqual(44, rows.Count);
         
      //   _mocks.VerifyAll();
      //}
      
      [Test]
      public void QueryUnitDataTest()
      {
         var database = CreateUnitInfoDatabase();
         database.DatabaseFilePath = TestDataFile;
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
      }

      private static UnitInfoLogic CreateUnitInfoLogic(Protein protein, UnitInfo unitInfo)
      {
         return new UnitInfoLogic(MockRepository.GenerateStub<IProteinBenchmarkCollection>())
                {
                   CurrentProtein = protein,
                   UnitInfoData = unitInfo
                };
      }

      private UnitInfoDatabase CreateUnitInfoDatabase()
      {
         return new UnitInfoDatabase(null, _proteinDictionary);
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
         protein.KFactor = 0;
         protein.Frames = 100;
         protein.NumberOfAtoms = 7000;
         protein.PreferredDays = 2;
         protein.MaximumDays = 3;
         proteins.Add(protein.ProjectNumber, protein);

         return proteins;
      }
   }
}
