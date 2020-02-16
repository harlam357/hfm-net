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
using System.IO;
using System.Threading;

using NUnit.Framework;

using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRepositoryReadOnlyTests
    {
        private const string TestDataFilesFolder = "..\\..\\TestFiles";

        private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";
        private readonly string _testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");

        private const string TestData2File = "..\\..\\TestFiles\\TestData2.db3";
        private readonly string _testData2FileCopy = Path.ChangeExtension(TestData2File, ".dbcopy");

        private WorkUnitRepository _database;
        private readonly IProteinService _proteinService = WorkUnitRepositoryTests.CreateProteinService();

        #region Setup and TearDown

        [OneTimeSetUp]
        public void FixtureInit()
        {
            SetupTestDataFileCopies();
            _database = new WorkUnitRepository(null, _proteinService, null);
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
        }

        [OneTimeTearDown]
        public void FixtureDestroy()
        {
            foreach (var file in Directory.EnumerateFiles(TestDataFilesFolder, "*.dbcopy"))
            {
                File.Delete(file);
            }
        }

        #endregion

        #region Fetch

        [Test]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_All_Test()
        {
            // Select All
            FetchTestData(44, WorkUnitHistoryQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_Equal_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchEqualCases =
        {
            new object[] { 13,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.Equal, 6600) },
            new object[] { 4,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.Equal, 7) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.Equal, 18) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.Equal, 18) },
            new object[] { 11,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Equal, "nVidia GPU - GTX275") },
            new object[] { 11,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.Equal, @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.Equal, "harlam357") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.Equal, 32) },
            new object[] { 11,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.Equal, 2.09) },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.Equal, 100) },
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.Equal, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.Equal, 1) },   // not a String value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.Equal, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.Equal, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 13,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.Equal, "WorkUnitName") },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.Equal, 2.3) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.Equal, "GROGPU2") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.Equal, 100) },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.Equal, 7000) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.Equal, "GPU") },
            new object[] { 6,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.Equal, 9482.92683) },
            new object[] { 13,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.Equal, 450) }
        };

        [Test]
        [TestCaseSource("FetchNotEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_NotEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchNotEqualCases =
        {
            new object[] { 31,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.NotEqual, 6600) },
            new object[] { 40,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.NotEqual, 7) },
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.NotEqual, 18) },
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.NotEqual, 18) },
            new object[] { 33,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.NotEqual, "nVidia GPU - GTX275") },
            new object[] { 33,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.NotEqual, @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.NotEqual, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.NotEqual, 32) },
            new object[] { 33,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.NotEqual, 2.09) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.NotEqual, 100) },
            new object[] { 32,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.NotEqual, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.NotEqual, 1) },   // not a String value
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.NotEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.NotEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 31,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.NotEqual, "WorkUnitName") },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.NotEqual, 2.3) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.NotEqual, "GROGPU2") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.NotEqual, 100) },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.NotEqual, 7000) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.NotEqual, "GPU") },
            new object[] { 38,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.NotEqual, 9482.92683) },
            new object[] { 31,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.NotEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_GreaterThan_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchGreaterThanCases =
        {
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.GreaterThan, 10502) },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.GreaterThan, 79) },
            new object[] { 7,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.GreaterThan, 761) },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.GreaterThan, 279) },
            new object[] { 21,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.GreaterThan, "nVidia GPU - GTX275") },
            new object[] { 32,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.GreaterThan, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.GreaterThan, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.GreaterThan, 32) },
            new object[] { 4,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.GreaterThan, 2.09) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.GreaterThan, 100) },
            new object[] { 23,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.GreaterThan, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.GreaterThan, 1) },   // not a String value
            new object[] { 7,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThan, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 23,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.GreaterThan, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.GreaterThan, "WorkUnitName") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.GreaterThan, 2.3) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.GreaterThan, "GRO-A3") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.GreaterThan, 99) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.GreaterThan, 0) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.GreaterThan, "CPU") },
            new object[] { 6,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.GreaterThan, 9482.92683) },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.GreaterThan, 450) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_GreaterThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchGreaterThanOrEqualCases =
        {
            new object[] { 13,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 10502) },
            new object[] { 5,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 79) },
            new object[] { 8,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 761) },
            new object[] { 4,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 279) },
            new object[] { 32,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "nVidia GPU - GTX275") },
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "harlam357") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 32) },
            new object[] { 15,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 2.09) },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 35,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 1) },   // not a String value
            new object[] { 8,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 24,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "WorkUnitName") },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 2.3) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "GRO-A3") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 99) },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 0) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "CPU") },
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 9482.92683) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchLessThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_LessThan_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLessThanCases =
        {
            new object[] { 31,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.LessThan, 10502) },
            new object[] { 39,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.LessThan, 79) },
            new object[] { 36,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.LessThan, 761) },
            new object[] { 40,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.LessThan, 279) },
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.LessThan, "nVidia GPU - GTX275") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.LessThan, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.LessThan, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.LessThan, 32) },
            new object[] { 29,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.LessThan, 2.09) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.LessThan, 100) },
            new object[] { 9,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.LessThan, 41)},   // not a TimeSpan value
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.LessThan, 1) },   // not a String value
            new object[] { 36,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThan, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.LessThan, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.LessThan, "WorkUnitName") },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.LessThan, 2.3) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.LessThan, "GRO-A3") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.LessThan, 99) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.LessThan, 0) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.LessThan, "CPU") },
            new object[] { 32,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.LessThan, 9482.92683) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.LessThan, 450) }
        };

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_LessThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLessThanOrEqualCases =
        {
            new object[] { 32,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.LessThanOrEqual, 10502) },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.LessThanOrEqual, 79) },
            new object[] { 37,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.LessThanOrEqual, 761) },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.LessThanOrEqual, 279) },
            new object[] { 23,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.LessThanOrEqual, "nVidia GPU - GTX275") },
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.LessThanOrEqual, @"\\Mainworkstation\Folding@home-gpu\") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.LessThanOrEqual, "harlam357") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.LessThanOrEqual, 32) },
            new object[] { 40,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.LessThanOrEqual, 2.09) },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.LessThanOrEqual, 100) },
            new object[] { 21,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, 41)},   // not a TimeSpan value
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.LessThanOrEqual, 1) },   // not a String value
            new object[] { 37,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 21,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.LessThanOrEqual, "WorkUnitName") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.LessThanOrEqual, 2.3) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.LessThanOrEqual, "GRO-A3") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.LessThanOrEqual, 99) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.LessThanOrEqual, 0) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.LessThanOrEqual, "CPU") },
            new object[] { 38,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.LessThanOrEqual, 9482.92683) },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.LessThanOrEqual, 450) }
        };

        [Test]
        [TestCaseSource("FetchLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_Like_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchLikeCases =
        {
            new object[] { 14,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.Like, "10%") },
            new object[] { 8,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 5,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.Like, "9%") },
            new object[] { 15,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.Like, "2%") },
            new object[] { 40,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Like, "nVidia GPU%") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.Like,  @"\\%\%") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.Like, "h%") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.Like, "%2") },
            new object[] { 15,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.Like, "2%") },
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 14,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.Like, "4%")},     // not a TimeSpan value
            new object[] { 44,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.Like, "1%") },    // not a String value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.Like, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.Like, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.Like, "Work%Name%") },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.Like, "0%") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.Like, "GRO%") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.Like, "0%") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.Like, "%U") },
            new object[] { 9,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.Like, "9%") },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.Like, "6%") }
        };

        [Test]
        [TestCaseSource("FetchNotLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_NotLike_Test(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData(expected, query);
        }

        private static object[] FetchNotLikeCases =
        {
            new object[] { 30,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.NotLike, "10%") },
            new object[] { 36,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 39,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.NotLike, "9%") },
            new object[] { 29,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.NotLike, "2%") },
            new object[] { 4,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.NotLike, "nVidia GPU%") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.NotLike,  @"\\%\%") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.NotLike, "h%") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.NotLike, "%2") },
            new object[] { 29,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.NotLike, "2%") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 30,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.NotLike, "4%")},     // not a TimeSpan value
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.NotLike, "1%") },    // not a String value
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.NotLike, new DateTime(2010, 8, 22, 0, 42, 0)) },
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.NotLike, new DateTime(2010, 8, 21, 20, 57, 0)) },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.NotLike, "Work%Name%") },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.NotLike, "0%") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.NotLike, "GRO%") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.NotLike, "0%") },
            new object[] { 28,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.NotLike, "%U") },
            new object[] { 35,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.NotLike, "9%") },
            new object[] { 41,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.NotLike, "6%") }
        };

        [Test]
        public void Fetch_Complex_Test1()
        {
            FetchTestData(33, new WorkUnitHistoryQuery()
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThan,
                    new DateTime(2010, 8, 8))
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThan,
                    new DateTime(2010, 8, 22)));
        }

        [Test]
        public void Fetch_Complex_Test2()
        {
            FetchTestData(3, new WorkUnitHistoryQuery()
                .AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.GreaterThan,
                    5000)
                .AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.LessThanOrEqual,
                    7000));
        }

        [Test]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_All_Test2()
        {
            // Select All
            FetchTestData2(253, WorkUnitHistoryQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_Equal_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchEqualCases2 =
        {
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.Equal, 8011) },
            new object[] { 72,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.Equal, 0) },
            new object[] { 6,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.Equal, 63) },
            new object[] { 2,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.Equal, 188) },
            new object[] { 12,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Equal, "Windows - Test Workstation Slot 00") },
            new object[] { 30,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.Equal, "192.168.0.172-36330") },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.Equal, "harlam357") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.Equal, 32) },
            new object[] { 63,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.Equal, 2.27) },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.Equal, 100) },
            new object[] { 14,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.Equal, 100)},  // not a TimeSpan value
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.Equal, 1) },   // not a String value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.Equal, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.Equal, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.Equal, "WorkUnitName3") },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.Equal, 0.75) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.Equal, "GRO-A5") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.Equal, 100) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.Equal, 11000) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.Equal, "CPU") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.Equal, 486876.03173) },
            new object[] { 2,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.Equal, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchNotEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_NotEqual_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchNotEqualCases2 =
        {
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.NotEqual, 8011) },
            new object[] { 181,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.NotEqual, 0) },
            new object[] { 247,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.NotEqual, 63) },
            new object[] { 251,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.NotEqual, 188) },
            new object[] { 241,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.NotEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 223,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.NotEqual, "192.168.0.172-36330") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.NotEqual, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.NotEqual, 32) },
            new object[] { 190,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.NotEqual, 2.27) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.NotEqual, 100) },
            new object[] { 239,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.NotEqual, 100)},  // not a TimeSpan value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.NotEqual, 1) },   // not a String value
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.NotEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.NotEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.NotEqual, "WorkUnitName3") },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.NotEqual, 0.75) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.NotEqual, "GRO-A5") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.NotEqual, 100) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.NotEqual, 11000) },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.NotEqual, "CPU") },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.NotEqual, 486876.03173) },
            new object[] { 251,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.NotEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_GreaterThan_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchGreaterThanCases2 =
        {
            new object[] { 75,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.GreaterThan, 7137) },
            new object[] { 47,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.GreaterThan, 18) },
            new object[] { 99,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.GreaterThan, 63) },
            new object[] { 146,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.GreaterThan, 188) },
            new object[] { 86,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.GreaterThan, "Windows - Test Workstation Slot 00") },
            new object[] { 197,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.GreaterThan, @"\\192.168.0.133\FAH\") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.GreaterThan, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.GreaterThan, 32) },
            new object[] { 166,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.GreaterThan, 2.15) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.GreaterThan, 100) },
            new object[] { 150,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.GreaterThan, 100)},  // not a TimeSpan value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.GreaterThan, 1) },   // not a String value
            new object[] { 42,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThan, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.GreaterThan, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.GreaterThan, "WorkUnitName3") },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.GreaterThan, 0.75) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.GreaterThan, "GRO-A4") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.GreaterThan, 100) },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.GreaterThan, 9000) },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.GreaterThan, "CPU") },
            new object[] { 5,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.GreaterThan, 486876.03173) },
            new object[] { 14,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.GreaterThan, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_GreaterThanOrEqual_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchGreaterThanOrEqualCases2 =
        {
            new object[] { 78,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 7137) },
            new object[] { 51,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 18) },
            new object[] { 105,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 63) },
            new object[] { 148,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 188) },
            new object[] { 98,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 205,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, @"\\192.168.0.133\FAH\") },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "harlam357") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 32) },
            new object[] { 226,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 2.15) },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 164,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 100)},  // not a TimeSpan value
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 1) },   // not a String value
            new object[] { 43,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 17,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "WorkUnitName3") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 0.75) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "GRO-A4") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 100) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 9000) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, "CPU") },
            new object[] { 6,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 486876.03173) },
            new object[] { 16,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.GreaterThanOrEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLessThanCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_LessThan_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLessThanCases2 =
        {
            new object[] { 175,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.LessThan, 7137) },
            new object[] { 202,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.LessThan, 18) },
            new object[] { 148,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.LessThan, 63) },
            new object[] { 105,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.LessThan, 188) },
            new object[] { 155,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.LessThan, "Windows - Test Workstation Slot 00") },
            new object[] { 48,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.LessThan, @"\\192.168.0.133\FAH\") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.LessThan, "harlam357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.LessThan, 32) },
            new object[] { 27,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.LessThan, 2.15) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.LessThan, 100) },
            new object[] { 89,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.LessThan, 100)},  // not a TimeSpan value
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.LessThan, 1) },   // not a String value
            new object[] { 210,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThan, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 236,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.LessThan, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.LessThan, "WorkUnitName4") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.LessThan, 0.75) },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.LessThan, "GRO-A4") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.LessThan, 100) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.LessThan, 11000) },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.LessThan, "CPU") },
            new object[] { 247,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.LessThan, 486876.03173) },
            new object[] { 237,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.LessThan, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_LessThanOrEqual_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLessThanOrEqualCases2 =
        {
            new object[] { 178,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.LessThanOrEqual, 7137) },
            new object[] { 206,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.LessThanOrEqual, 18) },
            new object[] { 154,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.LessThanOrEqual, 63) },
            new object[] { 107,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.LessThanOrEqual, 188) },
            new object[] { 167,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.LessThanOrEqual, "Windows - Test Workstation Slot 00") },
            new object[] { 56,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.LessThanOrEqual, @"\\192.168.0.133\FAH\") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.LessThanOrEqual, "harlam357") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.LessThanOrEqual, 32) },
            new object[] { 87,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.LessThanOrEqual, 2.15) },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.LessThanOrEqual, 100) },
            new object[] { 103,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, 100)},  // not a TimeSpan value
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.LessThanOrEqual, 1) },   // not a String value
            new object[] { 211,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 237,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.LessThanOrEqual, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.LessThanOrEqual, "WorkUnitName4") },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.LessThanOrEqual, 0.75) },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.LessThanOrEqual, "GRO-A4") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.LessThanOrEqual, 100) },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.LessThanOrEqual, 11000) },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.LessThanOrEqual, "CPU") },
            new object[] { 248,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.LessThanOrEqual, 486876.03173) },
            new object[] { 239,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.LessThanOrEqual, 869.4797) }
        };

        [Test]
        [TestCaseSource("FetchLikeCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_Like_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchLikeCases2 =
        {
            new object[] { 33,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.Like, "8%") },
            new object[] { 70,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 14,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.Like, "9%") },
            new object[] { 29,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.Like, "2%") },
            new object[] { 76,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Like, "Ubuntu VM SMP%") },
            new object[] { 160,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.Like, "%192%") },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.Like, "%357") },
            new object[] { 253,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.Like, "3%") },
            new object[] { 27,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 27,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.Like, "3_")},     // not a TimeSpan value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.Like, "3%") },    // not a String value
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.Like, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.Like, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.Like, "Work%Name%") },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.Like, "0%") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.Like, "GRO%") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.Like, "0%") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.Like, "0%") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.Like, "%U") },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.Like, "1%") },
            new object[] { 3,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.Like, "9%") }
        };

        [Test]
        [TestCaseSource("FetchNotLikeCases2")]
        [Category("HFM.Core.WorkUnitRepository.Fetch")]
        public void Fetch_NotLike_Test2(int expected, WorkUnitHistoryQuery query)
        {
            FetchTestData2(expected, query);
        }

        private static object[] FetchNotLikeCases2 =
        {
            new object[] { 220,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectID, WorkUnitHistoryQueryOperator.NotLike, "8%") },
            new object[] { 183,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectRun, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 239,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectClone, WorkUnitHistoryQueryOperator.NotLike, "9%") },
            new object[] { 224,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.ProjectGen, WorkUnitHistoryQueryOperator.NotLike, "2%") },
            new object[] { 177,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.NotLike, "Ubuntu VM SMP%") },
            new object[] { 93,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Path, WorkUnitHistoryQueryOperator.NotLike, "%192%") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Username, WorkUnitHistoryQueryOperator.NotLike, "%357") },
            new object[] { 0,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Team, WorkUnitHistoryQueryOperator.NotLike, "3%") },
            new object[] { 226,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CoreVersion, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 1,    new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FramesCompleted, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 226,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.FrameTime, WorkUnitHistoryQueryOperator.NotLike, "3_")},     // not a TimeSpan value
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Result, WorkUnitHistoryQueryOperator.NotLike, "3%") },    // not a String value
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.NotLike, new DateTime(2012, 7, 5, 0, 25, 7)) },
            new object[] { 252,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.CompletionDateTime, WorkUnitHistoryQueryOperator.NotLike, new DateTime(2012, 11, 19, 6, 56, 47)) },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.WorkUnitName, WorkUnitHistoryQueryOperator.NotLike, "Work%Name%") },
            new object[] { 10,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.KFactor, WorkUnitHistoryQueryOperator.NotLike, "0%") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Core, WorkUnitHistoryQueryOperator.NotLike, "GRO%") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Frames, WorkUnitHistoryQueryOperator.NotLike, "0%") },
            new object[] { 20,   new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Atoms, WorkUnitHistoryQueryOperator.NotLike, "0%") },
            new object[] { 233,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.SlotType, WorkUnitHistoryQueryOperator.NotLike, "%U") },
            new object[] { 243,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.PPD, WorkUnitHistoryQueryOperator.NotLike, "1%") },
            new object[] { 250,  new WorkUnitHistoryQuery().AddParameter(WorkUnitHistoryRowColumn.Credit, WorkUnitHistoryQueryOperator.NotLike, "9%") }
        };

        [Test]
        public void Fetch_Complex_Test3()
        {
            FetchTestData2(52, new WorkUnitHistoryQuery()
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThan,
                    new DateTime(2012, 5, 29))
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.LessThan,
                    new DateTime(2012, 11, 1)));
        }

        [Test]
        public void Fetch_Complex_Test4()
        {
            FetchTestData2(77, new WorkUnitHistoryQuery()
                .AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.GreaterThanOrEqual,
                    "Ubuntu VM SMP - Media Server")
                .AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.LessThanOrEqual,
                    "n"));
        }

        private void FetchTestData(int count, WorkUnitHistoryQuery query)
        {
            _database.Initialize(_testDataFileCopy);
            FetchInternal(count, query, BonusCalculationType.DownloadTime);
        }

        private void FetchTestData2(int count, WorkUnitHistoryQuery query)
        {
            _database.Initialize(_testData2FileCopy);
            FetchInternal(count, query, BonusCalculationType.FrameTime);
        }

        private void FetchInternal(int count, WorkUnitHistoryQuery query, BonusCalculationType bonusCalculation)
        {
            var entries = _database.Fetch(query, bonusCalculation);
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
        public void Page_All_Test1()
        {
            // Select All
            PageTestData(44, WorkUnitHistoryQuery.SelectAll);
        }

        [Test]
        [TestCaseSource("FetchEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_Equal_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchNotEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_NotEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchGreaterThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_GreaterThan_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchGreaterThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_GreaterThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLessThanCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_LessThan_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLessThanOrEqualCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_LessThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_Like_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        [Test]
        [TestCaseSource("FetchNotLikeCases")]
        [Category("HFM.Core.WorkUnitRepository.Page")]
        public void Page_NotLike_Test(int expected, WorkUnitHistoryQuery query)
        {
            PageTestData(expected, query);
        }

        private void PageTestData(long totalItems, WorkUnitHistoryQuery query)
        {
            const long itemsPerPage = 10;

            _database.Initialize(_testDataFileCopy);
            var page = _database.Page(1, itemsPerPage, query, BonusCalculationType.DownloadTime);
            int expectedPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
            Assert.AreEqual(totalItems, page.TotalItems);
            Assert.AreEqual(expectedPages, page.TotalPages);
        }

        #endregion

        #region Count

        [Test]
        public void CountCompleted_Test1()
        {
            _database.Initialize(_testDataFileCopy);
            long count = _database.CountCompleted("nVidia GPU - GTX285 - 1", null);
            Assert.AreEqual(11, count);
        }

        [Test]
        public void CountCompleted_Test2()
        {
            _database.Initialize(_testDataFileCopy);
            long count = _database.CountCompleted("nVidia GPU - GTX285 - 1", new DateTime(2010, 8, 21));
            Assert.AreEqual(6, count);
        }

        [Test]
        public void CountFailed_Test1()
        {
            _database.Initialize(_testData2FileCopy);
            long count = _database.CountFailed("nVidia GPU - GTX470", null);
            Assert.AreEqual(1, count);
        }

        [Test]
        public void CountFailed_Test2()
        {
            _database.Initialize(_testData2FileCopy);
            long count = _database.CountFailed("nVidia GPU - GTX470", new DateTime(2012, 2, 1));
            Assert.AreEqual(0, count);
        }

        #endregion
    }
}
