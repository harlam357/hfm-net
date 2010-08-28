
using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;

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
      public void WriteUnitInfoTest()
      {
         if (File.Exists(TestFile))
         {
            File.Delete(TestFile);
         }
      
         var unitInfoLogic = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic.ProjectID).Return(2669);
         SetupResult.For(unitInfoLogic.ProjectRun).Return(1);
         SetupResult.For(unitInfoLogic.ProjectClone).Return(2);
         SetupResult.For(unitInfoLogic.ProjectGen).Return(3);
         SetupResult.For(unitInfoLogic.OwningInstanceName).Return("Owner");
         SetupResult.For(unitInfoLogic.OwningInstancePath).Return("Path");
         SetupResult.For(unitInfoLogic.FoldingID).Return("harlam357");
         SetupResult.For(unitInfoLogic.Team).Return(32);
         SetupResult.For(unitInfoLogic.CoreVersion).Return("2.09");
         SetupResult.For(unitInfoLogic.FramesComplete).Return(100);
         SetupResult.For(unitInfoLogic.RawTimePerAllSections).Return(600);
         SetupResult.For(unitInfoLogic.UnitResult).Return(WorkUnitResult.FinishedUnit);
         SetupResult.For(unitInfoLogic.RawDownloadTime).Return(new DateTime(2010, 1, 1));
         SetupResult.For(unitInfoLogic.RawFinishedTime).Return(new DateTime(2010, 1, 2));

         _mocks.ReplayAll();

         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestFile };
         database.WriteUnitInfo(unitInfoLogic);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(1, rows.Length);
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
         
         // test code the ensure this unit is NOT written again
         database.WriteUnitInfo(unitInfoLogic);

         _mocks.VerifyAll();
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
         database.ImportCompletedUnits(database.ReadCompletedUnits("..\\..\\TestFiles\\CompletedUnits.csv").Entries);

         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(44, rows.Length);
         
         _mocks.VerifyAll();
      }
      
      [Test]
      public void QueryUnitDataTest()
      {
         _mocks.ReplayAll();
      
         var database = new UnitInfoDatabase(_proteinCollection) { DatabaseFilePath = TestDataFile };
         var rows = database.QueryUnitData(new QueryParameters());
         Assert.AreEqual(44, rows.Length);

         var parameters = new QueryParameters { ProjectID = { Enabled1 = true, Type1 = QueryFieldType.Equal, Value1 = 6600 } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(13, rows.Length);

         parameters.DownloadDateTime.Enabled1 = true;
         parameters.DownloadDateTime.Type1 = QueryFieldType.GreaterThanOrEqual;
         parameters.DownloadDateTime.Value1 = new DateTime(2010, 8, 20);

         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Length);

         parameters = new QueryParameters { DownloadDateTime = { Enabled1 = true, Type1 = QueryFieldType.GreaterThan, Value1 = new DateTime(2010, 8, 8),
                                                                 Enabled2 = true, Type2 = QueryFieldType.LessThan, Value2 = new DateTime(2010, 8, 22) } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(33, rows.Length);

         parameters = new QueryParameters { WorkUnitName = { Enabled1 = true, Type1 = QueryFieldType.Equal, Value1 = "WorkUnitName" } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(13, rows.Length);

         parameters = new QueryParameters { WorkUnitName = { Enabled1 = true, Type1 = QueryFieldType.Equal, Value1 = "WorkUnitName2" } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Length);

         parameters = new QueryParameters { Atoms = { Enabled1 = true, Type1 = QueryFieldType.GreaterThan, Value1 = 5000,
                                                      Enabled2 = true, Type2 = QueryFieldType.LessThanOrEqual, Value2 = 7000 } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(3, rows.Length);

         parameters = new QueryParameters { Core = { Enabled1 = true, Type1 = QueryFieldType.Equal, Value1 = "GROGPU2" } };
         rows = database.QueryUnitData(parameters);
         Assert.AreEqual(16, rows.Length);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void ReadCompletedUnitsTest()
      {
         var database = new UnitInfoDatabase(_proteinCollection);
         var result = database.ReadCompletedUnits("..\\..\\TestFiles\\CompletedUnits.csv");
         Assert.AreEqual(0, result.Duplicates);
         Assert.AreEqual(44, result.Entries.Count);
         Assert.AreEqual(0, result.ErrorLines.Count);
      }

      [Test]
      public void ReadCompletedUnitsLargeTest()
      {
         var database = new UnitInfoDatabase(_proteinCollection);
         var result = database.ReadCompletedUnits("..\\..\\TestFiles\\CompletedUnitsLarge.csv");
         Assert.AreEqual(330, result.Duplicates);
         Assert.AreEqual(30413, result.Entries.Count);
         Assert.AreEqual(153, result.ErrorLines.Count);
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
