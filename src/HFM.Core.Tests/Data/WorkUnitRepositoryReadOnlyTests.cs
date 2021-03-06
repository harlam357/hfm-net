﻿using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRepositoryReadOnlyTests
    {
        private const string TestDataFile = "TestFiles\\TestData.db3";
        private string _testDataFileCopy;

        private const string TestData2File = "TestFiles\\TestData2.db3";
        private string _testData2FileCopy;

        private ArtifactFolder _artifacts;
        private WorkUnitRepository _repository;
        private readonly IProteinService _proteinService = WorkUnitRepositoryTests.CreateProteinService();

        #region Setup and TearDown

        [OneTimeSetUp]
        public void FixtureInit()
        {
            SetupTestDataFileCopies();

            _repository = new WorkUnitRepository(null, _proteinService);
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
        }

        [OneTimeTearDown]
        public void FixtureDestroy()
        {
            _artifacts?.Dispose();
            _repository?.Dispose();
        }

        #endregion

        #region Fetch

        [Test]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_All_Test()
        {
            // Select All
            FetchTestData(44, WorkUnitQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_Equal_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchEqualCases =
        {
            new object[] { 13,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Equal, 6600) },
            new object[] { 4,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Equal, 7) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.Equal, 18) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.Equal, 18) },
            new object[] { 11,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, "nVidia GPU - GTX275") },
            new object[] { 11,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.Equal, @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.Equal, "harlam357") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.Equal, 32) },
            new object[] { 11,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.Equal, 2.09) },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.Equal, 100) },
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.Equal, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.Equal, 1) },   // not a String value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.Equal, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.Equal, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 13,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.Equal, "WorkUnitName") },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.Equal, 2.3) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.Equal, "GROGPU2") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.Equal, 100) },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.Equal, 7000) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.Equal, "GPU") },
            new object[] { 6,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.Equal, 9482.92683) },
            new object[] { 13,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.Equal, 450) }
        };

        [Test]
        [TestCaseSource("FetchNotEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_NotEqual_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchNotEqualCases =
        {
            new object[] { 31,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.NotEqual, 6600) },
            new object[] { 40,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.NotEqual, 7) },
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.NotEqual, 18) },
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.NotEqual, 18) },
            new object[] { 33,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.NotEqual, "nVidia GPU - GTX275") },
            new object[] { 33,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.NotEqual, @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.NotEqual, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.NotEqual, 32) },
            new object[] { 33,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.NotEqual, 2.09) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.NotEqual, 100) },
            new object[] { 32,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.NotEqual, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.NotEqual, 1) },   // not a String value
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.NotEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.NotEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 31,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.NotEqual, "WorkUnitName") },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.NotEqual, 2.3) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.NotEqual, "GROGPU2") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.NotEqual, 100) },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.NotEqual, 7000) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.NotEqual, "GPU") },
            new object[] { 38,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.NotEqual, 9482.92683) },
            new object[] { 31,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.NotEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_GreaterThan_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchGreaterThanCases =
        {
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.GreaterThan, 10502) },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.GreaterThan, 79) },
            new object[] { 7,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.GreaterThan, 761) },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.GreaterThan, 279) },
            new object[] { 21,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.GreaterThan, "nVidia GPU - GTX275") },
            new object[] { 32,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.GreaterThan, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.GreaterThan, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.GreaterThan, 32) },
            new object[] { 4,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.GreaterThan, 2.09) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.GreaterThan, 100) },
            new object[] { 23,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.GreaterThan, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.GreaterThan, 1) },   // not a String value
            new object[] { 7,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThan, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 23,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.GreaterThan, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.GreaterThan, "WorkUnitName") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.GreaterThan, 2.3) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.GreaterThan, "GRO-A3") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.GreaterThan, 99) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.GreaterThan, 0) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.GreaterThan, "CPU") },
            new object[] { 6,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.GreaterThan, 9482.92683) },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.GreaterThan, 450) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_GreaterThanOrEqual_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchGreaterThanOrEqualCases =
        {
            new object[] { 13,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.GreaterThanOrEqual, 10502) },
            new object[] { 5,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.GreaterThanOrEqual, 79) },
            new object[] { 8,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.GreaterThanOrEqual, 761) },
            new object[] { 4,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.GreaterThanOrEqual, 279) },
            new object[] { 32,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.GreaterThanOrEqual, "nVidia GPU - GTX275") },
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.GreaterThanOrEqual, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.GreaterThanOrEqual, "harlam357") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.GreaterThanOrEqual, 32) },
            new object[] { 15,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.GreaterThanOrEqual, 2.09) },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 35,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.GreaterThanOrEqual, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.GreaterThanOrEqual, 1) },   // not a String value
            new object[] { 8,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThanOrEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 24,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.GreaterThanOrEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.GreaterThanOrEqual, "WorkUnitName") },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.GreaterThanOrEqual, 2.3) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.GreaterThanOrEqual, "GRO-A3") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.GreaterThanOrEqual, 99) },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.GreaterThanOrEqual, 0) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.GreaterThanOrEqual, "CPU") },
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.GreaterThanOrEqual, 9482.92683) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.GreaterThanOrEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchLessThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_LessThan_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLessThanCases =
        {
            new object[] { 31,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.LessThan, 10502) },
            new object[] { 39,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.LessThan, 79) },
            new object[] { 36,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.LessThan, 761) },
            new object[] { 40,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.LessThan, 279) },
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.LessThan, "nVidia GPU - GTX275") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.LessThan, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.LessThan, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.LessThan, 32) },
            new object[] { 29,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.LessThan, 2.09) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.LessThan, 100) },
            new object[] { 9,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.LessThan, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.LessThan, 1) },   // not a String value
            new object[] { 36,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThan, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.LessThan, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.LessThan, "WorkUnitName") },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.LessThan, 2.3) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.LessThan, "GRO-A3") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.LessThan, 99) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.LessThan, 0) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.LessThan, "CPU") },
            new object[] { 32,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.LessThan, 9482.92683) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.LessThan, 450) }
        };

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_LessThanOrEqual_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLessThanOrEqualCases =
        {
            new object[] { 32,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.LessThanOrEqual, 10502) },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.LessThanOrEqual, 79) },
            new object[] { 37,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.LessThanOrEqual, 761) },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.LessThanOrEqual, 279) },
            new object[] { 23,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.LessThanOrEqual, "nVidia GPU - GTX275") },
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.LessThanOrEqual, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.LessThanOrEqual, "harlam357") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.LessThanOrEqual, 32) },
            new object[] { 40,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.LessThanOrEqual, 2.09) },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.LessThanOrEqual, 100) },
            new object[] { 21,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.LessThanOrEqual, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.LessThanOrEqual, 1) },   // not a String value
            new object[] { 37,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThanOrEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 21,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.LessThanOrEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.LessThanOrEqual, "WorkUnitName") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.LessThanOrEqual, 2.3) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.LessThanOrEqual, "GRO-A3") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.LessThanOrEqual, 99) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.LessThanOrEqual, 0) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.LessThanOrEqual, "CPU") },
            new object[] { 38,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.LessThanOrEqual, 9482.92683) },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.LessThanOrEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_Like_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLikeCases =
        {
            new object[] { 14,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Like, "10%") },
            new object[] { 8,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 5,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.Like, "9%") },
            new object[] { 15,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.Like, "2%") },
            new object[] { 40,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Like, "nVidia GPU%") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.Like,  @"\\%\%") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.Like, "h%") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.Like, "%2") },
            new object[] { 15,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.Like, "2%") },
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 14,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.Like, "4%")},     // not a TimeSpan value
            new object[] { 44,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.Like, "1%") },    // not a String value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.Like, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.Like, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.Like, "Work%Name%") },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.Like, "0%") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.Like, "GRO%") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.Like, "0%") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.Like, "%U") },
            new object[] { 9,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.Like, "9%") },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.Like, "6%") }
        };

        [Test]
        [TestCaseSource("FetchNotLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_NotLike_Test(int expected, WorkUnitQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchNotLikeCases =
        {
            new object[] { 30,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.NotLike, "10%") },
            new object[] { 36,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 39,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.NotLike, "9%") },
            new object[] { 29,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.NotLike, "2%") },
            new object[] { 4,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.NotLike, "nVidia GPU%") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.NotLike,  @"\\%\%") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.NotLike, "h%") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.NotLike, "%2") },
            new object[] { 29,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.NotLike, "2%") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 30,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.NotLike, "4%")},     // not a TimeSpan value
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.NotLike, "1%") },    // not a String value
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.NotLike, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.NotLike, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.NotLike, "Work%Name%") },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.NotLike, "0%") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.NotLike, "GRO%") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.NotLike, "0%") },
            new object[] { 28,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.NotLike, "%U") },
            new object[] { 35,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.NotLike, "9%") },
            new object[] { 41,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.NotLike, "6%") }
        };

        [Test]
        public void WorkUnitRepository_Fetch_Complex_Test1()
        {
            FetchTestData(33, new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThan,
                    new DateTime(2010, 8, 8))
                .AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThan,
                    new DateTime(2010, 8, 22)));
        }

        [Test]
        public void WorkUnitRepository_Fetch_Complex_Test2()
        {
            FetchTestData(3, new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.GreaterThan,
                    5000)
                .AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.LessThanOrEqual,
                    7000));
        }

        [Test]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_All_Test2()
        {
            // Select All
            FetchTestData2(253, WorkUnitQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_Equal_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchEqualCases2 =
        {
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Equal, 8011) },
            new object[] { 72,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Equal, 0) },
            new object[] { 6,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.Equal, 63) },
            new object[] { 2,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.Equal, 188) },
            new object[] { 12,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, "Windows - Test Workstation Slot 00") },
            new object[] { 30,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.Equal, "192.168.0.172-36330") },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.Equal, "harlam357") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.Equal, 32) },
            new object[] { 63,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.Equal, 2.27) },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.Equal, 100) },
            new object[] { 14,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.Equal, 100)},  // not a TimeSpan value
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.Equal, 1) },   // not a String value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.Equal, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.Equal, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.Equal, "WorkUnitName3") },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.Equal, 0.75) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.Equal, "GRO-A5") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.Equal, 100) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.Equal, 11000) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.Equal, "CPU") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.Equal, 486876.03173) },
            new object[] { 2,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.Equal, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchNotEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_NotEqual_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchNotEqualCases2 =
        {
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.NotEqual, 8011) },
            new object[] { 181,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.NotEqual, 0) },
            new object[] { 247,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.NotEqual, 63) },
            new object[] { 251,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.NotEqual, 188) },
            new object[] { 241,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.NotEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 223,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.NotEqual, "192.168.0.172-36330") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.NotEqual, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.NotEqual, 32) },
            new object[] { 190,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.NotEqual, 2.27) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.NotEqual, 100) },
            new object[] { 239,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.NotEqual, 100)},  // not a TimeSpan value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.NotEqual, 1) },   // not a String value
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.NotEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.NotEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.NotEqual, "WorkUnitName3") },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.NotEqual, 0.75) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.NotEqual, "GRO-A5") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.NotEqual, 100) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.NotEqual, 11000) },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.NotEqual, "CPU") },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.NotEqual, 486876.03173) },
            new object[] { 251,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.NotEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_GreaterThan_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchGreaterThanCases2 =
        {
            new object[] { 75,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.GreaterThan, 7137) },
            new object[] { 47,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.GreaterThan, 18) },
            new object[] { 99,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.GreaterThan, 63) },
            new object[] { 146,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.GreaterThan, 188) },
            new object[] { 86,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.GreaterThan, "Windows - Test Workstation Slot 00") },
            new object[] { 197,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.GreaterThan, @"\\192.168.0.133\FAH\") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.GreaterThan, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.GreaterThan, 32) },
            new object[] { 166,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.GreaterThan, 2.15) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.GreaterThan, 100) },
            new object[] { 150,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.GreaterThan, 100)},  // not a TimeSpan value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.GreaterThan, 1) },   // not a String value
            new object[] { 42,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThan, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.GreaterThan, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.GreaterThan, "WorkUnitName3") },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.GreaterThan, 0.75) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.GreaterThan, "GRO-A4") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.GreaterThan, 100) },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.GreaterThan, 9000) },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.GreaterThan, "CPU") },
            new object[] { 5,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.GreaterThan, 486876.03173) },
            new object[] { 14,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.GreaterThan, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_GreaterThanOrEqual_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchGreaterThanOrEqualCases2 =
        {
            new object[] { 78,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.GreaterThanOrEqual, 7137) },
            new object[] { 51,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.GreaterThanOrEqual, 18) },
            new object[] { 105,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.GreaterThanOrEqual, 63) },
            new object[] { 148,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.GreaterThanOrEqual, 188) },
            new object[] { 98,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.GreaterThanOrEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 205,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.GreaterThanOrEqual, @"\\192.168.0.133\FAH\") },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.GreaterThanOrEqual, "harlam357") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.GreaterThanOrEqual, 32) },
            new object[] { 226,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.GreaterThanOrEqual, 2.15) },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 164,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.GreaterThanOrEqual, 100)},  // not a TimeSpan value
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.GreaterThanOrEqual, 1) },   // not a String value
            new object[] { 43,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThanOrEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 17,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.GreaterThanOrEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.GreaterThanOrEqual, "WorkUnitName3") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.GreaterThanOrEqual, 0.75) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.GreaterThanOrEqual, "GRO-A4") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.GreaterThanOrEqual, 9000) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.GreaterThanOrEqual, "CPU") },
            new object[] { 6,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.GreaterThanOrEqual, 486876.03173) },
            new object[] { 16,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.GreaterThanOrEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLessThanCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_LessThan_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLessThanCases2 =
        {
            new object[] { 175,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.LessThan, 7137) },
            new object[] { 202,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.LessThan, 18) },
            new object[] { 148,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.LessThan, 63) },
            new object[] { 105,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.LessThan, 188) },
            new object[] { 155,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.LessThan, "Windows - Test Workstation Slot 00") },
            new object[] { 48,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.LessThan, @"\\192.168.0.133\FAH\") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.LessThan, "harlam357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.LessThan, 32) },
            new object[] { 27,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.LessThan, 2.15) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.LessThan, 100) },
            new object[] { 89,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.LessThan, 100)},  // not a TimeSpan value
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.LessThan, 1) },   // not a String value
            new object[] { 210,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThan, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 236,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.LessThan, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.LessThan, "WorkUnitName4") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.LessThan, 0.75) },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.LessThan, "GRO-A4") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.LessThan, 100) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.LessThan, 11000) },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.LessThan, "CPU") },
            new object[] { 247,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.LessThan, 486876.03173) },
            new object[] { 237,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.LessThan, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_LessThanOrEqual_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLessThanOrEqualCases2 =
        {
            new object[] { 178,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.LessThanOrEqual, 7137) },
            new object[] { 206,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.LessThanOrEqual, 18) },
            new object[] { 154,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.LessThanOrEqual, 63) },
            new object[] { 107,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.LessThanOrEqual, 188) },
            new object[] { 167,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.LessThanOrEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 56,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.LessThanOrEqual, @"\\192.168.0.133\FAH\") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.LessThanOrEqual, "harlam357") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.LessThanOrEqual, 32) },
            new object[] { 87,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.LessThanOrEqual, 2.15) },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.LessThanOrEqual, 100) },
            new object[] { 103,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.LessThanOrEqual, 100)},  // not a TimeSpan value
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.LessThanOrEqual, 1) },   // not a String value
            new object[] { 211,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThanOrEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 237,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.LessThanOrEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.LessThanOrEqual, "WorkUnitName4") },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.LessThanOrEqual, 0.75) },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.LessThanOrEqual, "GRO-A4") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.LessThanOrEqual, 100) },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.LessThanOrEqual, 11000) },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.LessThanOrEqual, "CPU") },
            new object[] { 248,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.LessThanOrEqual, 486876.03173) },
            new object[] { 239,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.LessThanOrEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLikeCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_Like_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLikeCases2 =
        {
            new object[] { 33,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Like, "8%") },
            new object[] { 70,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 14,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.Like, "9%") },
            new object[] { 29,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.Like, "2%") },
            new object[] { 76,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Like, "Ubuntu VM SMP%") },
            new object[] { 160,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.Like, "%192%") },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.Like, "%357") },
            new object[] { 253,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.Like, "3%") },
            new object[] { 27,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 27,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.Like, "3_")},     // not a TimeSpan value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.Like, "3%") },    // not a String value
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.Like, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.Like, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.Like, "Work%Name%") },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.Like, "0%") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.Like, "GRO%") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.Like, "0%") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.Like, "0%") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.Like, "%U") },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.Like, "1%") },
            new object[] { 3,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.Like, "9%") }
        };

        [Test]
        [TestCaseSource("FetchNotLikeCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void WorkUnitRepository_Fetch_NotLike_Test2(int expected, WorkUnitQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchNotLikeCases2 =
        {
            new object[] { 220,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.NotLike, "8%") },
            new object[] { 183,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 239,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.NotLike, "9%") },
            new object[] { 224,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.NotLike, "2%") },
            new object[] { 177,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.NotLike, "Ubuntu VM SMP%") },
            new object[] { 93,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Path, WorkUnitQueryOperator.NotLike, "%192%") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Username, WorkUnitQueryOperator.NotLike, "%357") },
            new object[] { 0,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Team, WorkUnitQueryOperator.NotLike, "3%") },
            new object[] { 226,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.CoreVersion, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 1,    new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FramesCompleted, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 226,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.FrameTime, WorkUnitQueryOperator.NotLike, "3_")},     // not a TimeSpan value
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Result, WorkUnitQueryOperator.NotLike, "3%") },    // not a String value
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.NotLike, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 252,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Finished, WorkUnitQueryOperator.NotLike, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.WorkUnitName, WorkUnitQueryOperator.NotLike, "Work%Name%") },
            new object[] { 10,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.KFactor, WorkUnitQueryOperator.NotLike, "0%") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Core, WorkUnitQueryOperator.NotLike, "GRO%") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Frames, WorkUnitQueryOperator.NotLike, "0%") },
            new object[] { 20,   new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Atoms, WorkUnitQueryOperator.NotLike, "0%") },
            new object[] { 233,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.SlotType, WorkUnitQueryOperator.NotLike, "%U") },
            new object[] { 243,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.PPD, WorkUnitQueryOperator.NotLike, "1%") },
            new object[] { 250,  new WorkUnitQuery().AddParameter(WorkUnitRowColumn.Credit, WorkUnitQueryOperator.NotLike, "9%") }
        };

        [Test]
        public void WorkUnitRepository_Fetch_Complex_Test3()
        {
            FetchTestData2(52, new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.GreaterThan,
                    new DateTime(2012, 5, 29))
                .AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.LessThan,
                    new DateTime(2012, 11, 1)));
        }

        [Test]
        public void WorkUnitRepository_Fetch_Complex_Test4()
        {
            FetchTestData2(77, new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.GreaterThanOrEqual,
                    "Ubuntu VM SMP - Media Server")
                .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.LessThanOrEqual,
                    "n"));
        }

        private void FetchTestData(int count, WorkUnitQuery query)
        {
            Initialize(_testDataFileCopy);
            FetchInternal(count, query, BonusCalculation.DownloadTime);
        }

        private void FetchTestData2(int count, WorkUnitQuery query)
        {
            Initialize(_testData2FileCopy);
            FetchInternal(count, query, BonusCalculation.FrameTime);
        }

        private void FetchInternal(int count, WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            var entries = _repository.Fetch(query, bonusCalculation);
//#if DEBUG
//          //Debug.WriteLine(query.Parameters[0].Column);
//          foreach (var entry in entries)
//          {
//              Debug.WriteLine(entry.ID);
//          }
//#endif
            Assert.AreEqual(count, entries.Count);
        }

        #endregion

        #region Page

        [Test]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_All_Test1()
        {
            // Select All
            PageTestData(44, WorkUnitQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_Equal_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchNotEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_NotEqual_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchGreaterThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_GreaterThan_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_GreaterThanOrEqual_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLessThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_LessThan_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_LessThanOrEqual_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_Like_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchNotLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void WorkUnitRepository_Page_NotLike_Test(int expected, WorkUnitQuery query)
        {
            PageTestData(expected, query);
        }

        private void PageTestData(long totalItems, WorkUnitQuery query)
        {
            const long itemsPerPage = 10;

            Initialize(_testDataFileCopy);
            var page = _repository.Page(1, itemsPerPage, query, BonusCalculation.DownloadTime);
            int expectedPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
            Assert.AreEqual(totalItems, page.TotalItems);
            Assert.AreEqual(expectedPages, page.TotalPages);
        }

        #endregion

        #region Count

        [Test]
        public void WorkUnitRepository_CountCompleted_Test1()
        {
            Initialize(_testDataFileCopy);
            long count = _repository.CountCompleted("nVidia GPU - GTX285 - 1", null);
            Assert.AreEqual(11, count);
        }

        [Test]
        public void WorkUnitRepository_CountCompleted_Test2()
        {
            Initialize(_testDataFileCopy);
            long count = _repository.CountCompleted("nVidia GPU - GTX285 - 1", new DateTime(2010, 8, 21));
            Assert.AreEqual(6, count);
        }

        [Test]
        public void WorkUnitRepository_CountFailed_Test1()
        {
            Initialize(_testData2FileCopy);
            long count = _repository.CountFailed("nVidia GPU - GTX470", null);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void WorkUnitRepository_CountFailed_Test2()
        {
            Initialize(_testData2FileCopy);
            long count = _repository.CountFailed("nVidia GPU - GTX470", new DateTime(2012, 2, 1));
            Assert.AreEqual(0, count);
        }

        #endregion

        private void Initialize(string path)
        {
            _repository.Initialize(path);
            _repository.Upgrade();
        }
    }
}
