using System.Diagnostics;
using System.Globalization;

using NUnit.Framework;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitContextRepositoryLegacyTests
    {
        private const string TestDataFile = "TestFiles\\TestData.db";
        private string _testDataFileCopy;

        private const string TestData2File = "TestFiles\\TestData2.db";
        private string _testData2FileCopy;

        private string _testScratchFile;

        private ArtifactFolder _artifacts;
        private IWorkUnitRepository _repository;

        #region Setup and TearDown

        [SetUp]
        public void Init()
        {
            SetupTestDataFileCopies();
        }

        private void SetupTestDataFileCopies()
        {
            _artifacts = new ArtifactFolder();

            // sometimes the file is not finished
            // copying before we attempt to open
            // the copied file.  Halt the thread
            // for a bit to ensure the copy has
            // completed.

            _testDataFileCopy = _artifacts.GetRandomFilePath();
            File.Copy(TestDataFile, _testDataFileCopy, true);
            Thread.Sleep(100);

            _testData2FileCopy = _artifacts.GetRandomFilePath();
            File.Copy(TestData2File, _testData2FileCopy, true);
            Thread.Sleep(100);

            _testScratchFile = _artifacts.GetRandomFilePath();
        }

        [TearDown]
        public void Destroy()
        {
            _artifacts?.Dispose();
            (_repository as IDisposable)?.Dispose();
        }

        #endregion

        [Test]
        public void WorkUnitContextRepository_MultiThread_Test()
        {
            Initialize(_testScratchFile);

            Parallel.For(0, 100, i =>
            {
                Debug.WriteLine("Writing unit {0:00} on thread id: {1:00}", i, Thread.CurrentThread.ManagedThreadId);

                var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
                var slotModel = new SlotModel(new NullClient { Settings = settings });
                var workUnitModel = new WorkUnitModel(slotModel, BuildWorkUnit1(i));
                workUnitModel.CurrentProtein = BuildProtein1();

                _repository.Update(workUnitModel);
            });

            Assert.AreEqual(100, _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None).Count);
        }

        #region Insert

        [Test]
        public void WorkUnitContextRepository_Update_Test1()
        {
            var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
            UpdateTestInternal(settings, SlotIdentifier.NoSlotID, BuildWorkUnit1(), BuildProtein1(), BuildWorkUnit1VerifyAction());
        }

        [Test]
        public void WorkUnitContextRepository_Update_Test1_CzechCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
            UpdateTestInternal(settings, SlotIdentifier.NoSlotID, BuildWorkUnit1(), BuildProtein1(), BuildWorkUnit1VerifyAction());
        }

        [Test]
        public void WorkUnitContextRepository_Update_Test2()
        {
            var settings = new ClientSettings { Name = "Owner's", Server = "The Path's", Port = ClientSettings.NoPort };
            UpdateTestInternal(settings, SlotIdentifier.NoSlotID, BuildWorkUnit2(), BuildProtein2(), BuildWorkUnit2VerifyAction());
        }

        [Test]
        public void WorkUnitContextRepository_Update_Test3()
        {
            var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
            UpdateTestInternal(settings, SlotIdentifier.NoSlotID, BuildWorkUnit3(), BuildProtein3(), BuildWorkUnit3VerifyAction());
        }

        [Test]
        public void WorkUnitContextRepository_Update_Test4()
        {
            var settings = new ClientSettings { Name = "Owner2", Server = "Path2", Port = ClientSettings.NoPort };
            UpdateTestInternal(settings, 2, BuildWorkUnit4(), BuildProtein4(), BuildWorkUnit4VerifyAction());
        }

        private void UpdateTestInternal(ClientSettings settings, int slotID, WorkUnit workUnit, Protein protein, Action<IList<WorkUnitRow>> verifyAction)
        {
            Initialize(_testScratchFile);

            var slotModel = new SlotModel(new NullClient { Settings = settings }) { SlotID = slotID };
            var workUnitModel = new WorkUnitModel(slotModel, workUnit);
            workUnitModel.CurrentProtein = protein;

            _repository.Update(workUnitModel);

            var rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None).ToList();
            verifyAction(rows);

            // test code to ensure this unit is NOT written again
            _repository.Update(workUnitModel);
            // verify
            rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None).ToList();
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
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = new Version(0, 2, 9);
            workUnit.UnitResult = WorkUnitResult.FinishedUnit;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.Assigned = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            workUnit.Finished = new DateTime(2010, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.Frames = frameDataDictionary;
            return workUnit;
        }

        private static Protein BuildProtein1()
        {
            return new Protein
            {
                ProjectNumber = 2669,
                WorkUnitName = "",
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
                Assert.AreEqual("0.2.9", row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.FromSeconds(600), row.FrameTime);
                Assert.AreEqual(WorkUnitResultString.FinishedUnit, row.Result);
                Assert.AreEqual(new DateTime(2010, 1, 1), row.Assigned);
                Assert.AreEqual(new DateTime(2010, 1, 2), row.Finished);
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
            workUnit.FoldingID = "harlam357's";
            workUnit.Team = 100;
            workUnit.CoreVersion = new Version(2, 27);
            workUnit.UnitResult = WorkUnitResult.EarlyUnitEnd;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.Assigned = new DateTime(2009, 5, 5);
            workUnit.Finished = new DateTime(2009, 5, 6);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 56, Duration = TimeSpan.FromSeconds(1000) });
            workUnit.Frames = frameDataDictionary;
            return workUnit;
        }

        private static Protein BuildProtein2()
        {
            return new Protein
            {
                ProjectNumber = 6900,
                WorkUnitName = "",
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
                Assert.AreEqual("2.27", row.CoreVersion);
                Assert.AreEqual(56, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.FromSeconds(1000), row.FrameTime);
                Assert.AreEqual(WorkUnitResultString.EarlyUnitEnd, row.Result);
                Assert.AreEqual(new DateTime(2009, 5, 5), row.Assigned);
                Assert.AreEqual(new DateTime(2009, 5, 6), row.Finished);
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
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = new Version(0, 2, 9);
            workUnit.UnitResult = WorkUnitResult.EarlyUnitEnd;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.Assigned = new DateTime(2010, 2, 2);
            workUnit.Finished = new DateTime(2010, 2, 3);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            //workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.Frames = frameDataDictionary;
            return workUnit;
        }

        private static Protein BuildProtein3()
        {
            return new Protein
            {
                ProjectNumber = 2670,
                WorkUnitName = "",
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
                Assert.AreEqual("0.2.9", row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.Zero, row.FrameTime);
                Assert.AreEqual(WorkUnitResultString.EarlyUnitEnd, row.Result);
                Assert.AreEqual(new DateTime(2010, 2, 2), row.Assigned);
                Assert.AreEqual(new DateTime(2010, 2, 3), row.Finished);
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
            workUnit.FoldingID = "harlam357";
            workUnit.Team = 32;
            workUnit.CoreVersion = new Version(2, 27);
            workUnit.UnitResult = WorkUnitResult.FinishedUnit;

            // These values can be either Utc or Unspecified. Setting SQLite's DateTimeKind
            // connection string option to Utc will force SQLite to handle all DateTime 
            // values as Utc regardless of the DateTimeKind specified in the value.
            workUnit.Assigned = new DateTime(2012, 1, 2);
            workUnit.Finished = new DateTime(2012, 1, 5);

            // these values effect the value reported when WorkUnitModel.GetRawTime() is called
            //workUnit.FramesObserved = 1;
            var frameDataDictionary = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 100, Duration = TimeSpan.FromMinutes(10) });
            workUnit.Frames = frameDataDictionary;
            return workUnit;
        }

        private static Protein BuildProtein4()
        {
            return new Protein
            {
                ProjectNumber = 6903,
                WorkUnitName = "",
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
                Assert.AreEqual("2.27", row.CoreVersion);
                Assert.AreEqual(100, row.FramesCompleted);
                Assert.AreEqual(TimeSpan.Zero, row.FrameTime);
                Assert.AreEqual(WorkUnitResultString.FinishedUnit, row.Result);
                Assert.AreEqual(new DateTime(2012, 1, 2), row.Assigned);
                Assert.AreEqual(new DateTime(2012, 1, 5), row.Finished);
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
        public void WorkUnitContextRepository_Delete_Test()
        {
            // Arrange
            Initialize(_testDataFileCopy);
            var entries = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            // Assert (pre-condition)
            Assert.AreEqual(44, entries.Count);
            // Act
            Assert.AreEqual(1, _repository.Delete(entries[14]));
            // Assert
            entries = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.None);
            Assert.AreEqual(43, entries.Count);
        }

        [Test]
        public void WorkUnitContextRepository_Delete_NotExist_Test()
        {
            Initialize(_testDataFileCopy);
            Assert.AreEqual(0, _repository.Delete(new WorkUnitRow { ID = 100 }));
        }

        #endregion

        private void Initialize(string path)
        {
            string connectionString = $"Data Source={path}";
            _repository = new TestableWorkUnitContextRepository(connectionString);
        }
    }
}
