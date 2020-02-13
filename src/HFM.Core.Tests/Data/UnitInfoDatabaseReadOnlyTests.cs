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
   public class UnitInfoDatabaseReadOnlyTests
   {
      private const string TestDataFilesFolder = "..\\..\\TestFiles";

      private const string TestDataFile = "..\\..\\TestFiles\\TestData.db3";
      private readonly string _testDataFileCopy = Path.ChangeExtension(TestDataFile, ".dbcopy");

      private const string TestData2File = "..\\..\\TestFiles\\TestData2.db3";
      private readonly string _testData2FileCopy = Path.ChangeExtension(TestData2File, ".dbcopy");

      private UnitInfoDatabase _database;
      private readonly IProteinService _proteinService = UnitInfoDatabaseTests.CreateProteinService();

      #region Setup and TearDown

      [OneTimeSetUp]
      public void FixtureInit()
      {
         SetupTestDataFileCopies();
         _database = new UnitInfoDatabase(null, _proteinService, null);
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
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_All_Test()
      {
         // Select All
         FetchTestData(44, BuildParameters());
      }

      [Test]
      [TestCaseSource("FetchEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_Equal_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchEqualCases =
      {
         new object[] { 13,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.Equal,  Value = 6600 }) },
         new object[] { 4,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.Equal,  Value = 7 }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.Equal,  Value = 18 }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.Equal,  Value = 18 }) },
         new object[] { 11,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 11,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.Equal,  Value = "harlam357" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = 32 }) },
         new object[] { 11,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.Equal,  Value = 2.09 }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.Equal,  Value = 100 }) },
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.Equal,  Value = 41 })},   // not a TimeSpan value
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 1 }) },   // not a String value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.Equal,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.Equal,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 13,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.Equal,  Value = "WorkUnitName" }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.Equal,  Value = 2.3 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = "GROGPU2" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 100 }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.Equal,  Value = 7000 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.Equal,  Value = "GPU" }) },
         new object[] { 6,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.Equal,  Value = 9482.92683 }) },
         new object[] { 13,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchNotEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_NotEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchNotEqualCases =
      {
         new object[] { 31,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 6600 }) },
         new object[] { 40,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 7 }) },
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 18 }) },
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 18 }) },
         new object[] { 33,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 33,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = @"\\win7i7\Users\harlarw\AppData\Roaming\Folding@home-gpu\" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 32 }) },
         new object[] { 33,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 2.09 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 100 }) },
         new object[] { 32,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 41 })},   // not a TimeSpan value
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 1 }) },   // not a String value
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 31,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "WorkUnitName" }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 2.3 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "GROGPU2" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 100 }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 7000 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "GPU" }) },
         new object[] { 38,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 9482.92683 }) },
         new object[] { 31,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchGreaterThanCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_GreaterThan_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchGreaterThanCases =
      {
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 10502 }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 79 }) },
         new object[] { 7,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 761 }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 279 }) },
         new object[] { 21,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 32,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = @"\\Mainworkstation\Folding@home-gpu\" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 32 }) },
         new object[] { 4,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 2.09 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 100 }) },
         new object[] { 23,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 41 })},   // not a TimeSpan value
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 1 }) },   // not a String value
         new object[] { 7,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 23,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "WorkUnitName" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 2.3 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "GRO-A3" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 99 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 0 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "CPU" }) },
         new object[] { 6,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 9482.92683 }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchGreaterThanOrEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_GreaterThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchGreaterThanOrEqualCases =
      {
         new object[] { 13,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 10502 }) },
         new object[] { 5,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 79 }) },
         new object[] { 8,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 761 }) },
         new object[] { 4,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 279 }) },
         new object[] { 32,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = @"\\Mainworkstation\Folding@home-gpu\" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "harlam357" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 32 }) },
         new object[] { 15,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 2.09 }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 100 }) },
         new object[] { 35,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 41 })},   // not a TimeSpan value
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 1 }) },   // not a String value
         new object[] { 8,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 24,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "WorkUnitName" }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 2.3 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "GRO-A3" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 99 }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 0 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "CPU" }) },
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 9482.92683 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchLessThanCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_LessThan_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchLessThanCases =
      {
         new object[] { 31,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 10502 }) },
         new object[] { 39,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 79 }) },
         new object[] { 36,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 761 }) },
         new object[] { 40,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 279 }) },
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = @"\\Mainworkstation\Folding@home-gpu\" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 32 }) },
         new object[] { 29,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 2.09 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 100 }) },
         new object[] { 9,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 41 })},   // not a TimeSpan value
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 1 }) },   // not a String value
         new object[] { 36,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.LessThan,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.LessThan,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "WorkUnitName" }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 2.3 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "GRO-A3" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 99 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 0 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "CPU" }) },
         new object[] { 32,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 9482.92683 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchLessThanOrEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_LessThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchLessThanOrEqualCases =
      {
         new object[] { 32,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 10502 }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 79 }) },
         new object[] { 37,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 761 }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 279 }) },
         new object[] { 23,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "nVidia GPU - GTX275" }) },
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = @"\\Mainworkstation\Folding@home-gpu\" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "harlam357" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 32 }) },
         new object[] { 40,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 2.09 }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 100 }) },
         new object[] { 21,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 41 })},   // not a TimeSpan value
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 1 }) },   // not a String value
         new object[] { 37,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 21,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "WorkUnitName" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 2.3 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "GRO-A3" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 99 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 0 }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "CPU" }) },
         new object[] { 38,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 9482.92683 }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 450 }) }
      };

      [Test]
      [TestCaseSource("FetchLikeCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_Like_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchLikeCases =
      {
         new object[] { 14,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.Like,  Value = "10%" }) },
         new object[] { 8,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 5,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.Like,  Value = "9%" }) },
         new object[] { 15,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.Like,  Value = "2%" }) },
         new object[] { 40,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "nVidia GPU%" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.Like,  Value =  @"\\%\%" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.Like,  Value = "h%" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "%2" }) },
         new object[] { 15,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.Like,  Value = "2%" }) },
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 14,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.Like,  Value = "4%" })},     // not a TimeSpan value
         new object[] { 44,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },    // not a String value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.Like,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.Like,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.Like,  Value = "Work%Name%" }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.Like,  Value = "0%" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "GRO%" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.Like,  Value = "0%" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.Like,  Value = "%U" }) },
         new object[] { 9,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.Like,  Value = "9%" }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "6%" }) }
      };

      [Test]
      [TestCaseSource("FetchNotLikeCases")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_NotLike_Test(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData(expected, query);
      }

      private static object[] FetchNotLikeCases =
      {
         new object[] { 30,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "10%" }) },
         new object[] { 36,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 39,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "9%" }) },
         new object[] { 29,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "2%" }) },
         new object[] { 4,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "nVidia GPU%" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value =  @"\\%\%" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "h%" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "%2" }) },
         new object[] { 29,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "2%" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 30,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "4%" })},     // not a TimeSpan value
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },    // not a String value
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.NotLike,  Value = new DateTime(2010, 8, 22, 0, 42, 0) }) },
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.NotLike,  Value = new DateTime(2010, 8, 21, 20, 57, 0) }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "Work%Name%" }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "0%" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "GRO%" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "0%" }) },
         new object[] { 28,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "%U" }) },
         new object[] { 35,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "9%" }) },
         new object[] { 41,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "6%" }) }
      };

      [Test]
      public void Fetch_Complex_Test1()
      {
         FetchTestData(33, BuildParameters(new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.DownloadDateTime,
            Type = WorkUnitHistoryQueryOperator.GreaterThan,
            Value = new DateTime(2010, 8, 8)
         },
         new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.DownloadDateTime,
            Type = WorkUnitHistoryQueryOperator.LessThan,
            Value = new DateTime(2010, 8, 22)
         }));
      }

      [Test]
      public void Fetch_Complex_Test2()
      {
         FetchTestData(3, BuildParameters(new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.Atoms,
            Type = WorkUnitHistoryQueryOperator.GreaterThan,
            Value = 5000
         },
         new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.Atoms,
            Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,
            Value = 7000
         }));
      }

      [Test]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_All_Test2()
      {
         // Select All
         FetchTestData2(253, BuildParameters());
      }

      [Test]
      [TestCaseSource("FetchEqualCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_Equal_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchEqualCases2 =
      {
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.Equal,  Value = 8011 }) },
         new object[] { 72,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.Equal,  Value = 0 }) },
         new object[] { 6,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.Equal,  Value = 63 }) },
         new object[] { 2,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.Equal,  Value = 188 }) },
         new object[] { 12,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 30,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = "192.168.0.172-36330" }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.Equal,  Value = "harlam357" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = 32 }) },
         new object[] { 63,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.Equal,  Value = 2.27 }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.Equal,  Value = 100 }) },
         new object[] { 14,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.Equal,  Value = 100 })},  // not a TimeSpan value
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 1 }) },   // not a String value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.Equal,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.Equal,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.Equal,  Value = "WorkUnitName3" }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.Equal,  Value = 0.75 }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.Equal,  Value = "GRO-A5" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 100 }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.Equal,  Value = 11000 }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.Equal,  Value = "CPU" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.Equal,  Value = 486876.03173 }) },
         new object[] { 2,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.Equal,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchNotEqualCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_NotEqual_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchNotEqualCases2 =
      {
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 8011 }) },
         new object[] { 181,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 0 }) },
         new object[] { 247,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 63 }) },
         new object[] { 251,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 188 }) },
         new object[] { 241,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 223,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "192.168.0.172-36330" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 32 }) },
         new object[] { 190,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 2.27 }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 100 }) },
         new object[] { 239,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 100 })},  // not a TimeSpan value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 1 }) },   // not a String value
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "WorkUnitName3" }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 0.75 }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "GRO-A5" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 100 }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 11000 }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = "CPU" }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 486876.03173 }) },
         new object[] { 251,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.NotEqual,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchGreaterThanCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_GreaterThan_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchGreaterThanCases2 =
      {
         new object[] { 75,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 7137 }) },
         new object[] { 47,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 18 }) },
         new object[] { 99,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 63 }) },
         new object[] { 146,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 188 }) },
         new object[] { 86,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 197,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = @"\\192.168.0.133\FAH\" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 32 }) },
         new object[] { 166,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 2.15 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 100 }) },
         new object[] { 150,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 100 })},  // not a TimeSpan value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 1 }) },   // not a String value
         new object[] { 42,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "WorkUnitName3" }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 0.75 }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "GRO-A4" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 100 }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 9000 }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = "CPU" }) },
         new object[] { 5,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 486876.03173 }) },
         new object[] { 14,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.GreaterThan,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchGreaterThanOrEqualCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_GreaterThanOrEqual_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchGreaterThanOrEqualCases2 =
      {
         new object[] { 78,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 7137 }) },
         new object[] { 51,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 18 }) },
         new object[] { 105,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 63 }) },
         new object[] { 148,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 188 }) },
         new object[] { 98,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 205,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = @"\\192.168.0.133\FAH\" }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "harlam357" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 32 }) },
         new object[] { 226,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 2.15 }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 100 }) },
         new object[] { 164,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 100 })},  // not a TimeSpan value
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 1 }) },   // not a String value
         new object[] { 43,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 17,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "WorkUnitName3" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 0.75 }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "GRO-A4" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 100 }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 9000 }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = "CPU" }) },
         new object[] { 6,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 486876.03173 }) },
         new object[] { 16,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchLessThanCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_LessThan_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchLessThanCases2 =
      {
         new object[] { 175,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 7137 }) },
         new object[] { 202,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 18 }) },
         new object[] { 148,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 63 }) },
         new object[] { 105,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 188 }) },
         new object[] { 155,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 48,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = @"\\192.168.0.133\FAH\" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "harlam357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 32 }) },
         new object[] { 27,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 2.15 }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 100 }) },
         new object[] { 89,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 100 })},  // not a TimeSpan value
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 1 }) },   // not a String value
         new object[] { 210,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.LessThan,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 236,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.LessThan,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "WorkUnitName4" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 0.75 }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "GRO-A4" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 100 }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 11000 }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.LessThan,  Value = "CPU" }) },
         new object[] { 247,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 486876.03173 }) },
         new object[] { 237,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.LessThan,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchLessThanOrEqualCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_LessThanOrEqual_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchLessThanOrEqualCases2 =
      {
         new object[] { 178,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 7137 }) },
         new object[] { 206,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 18 }) },
         new object[] { 154,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 63 }) },
         new object[] { 107,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 188 }) },
         new object[] { 167,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "Windows - Test Workstation Slot 00" }) },
         new object[] { 56,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = @"\\192.168.0.133\FAH\" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "harlam357" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 32 }) },
         new object[] { 87,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 2.15 }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 100 }) },
         new object[] { 103,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 100 })},  // not a TimeSpan value
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 1 }) },   // not a String value
         new object[] { 211,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 237,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "WorkUnitName4" }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 0.75 }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "GRO-A4" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 100 }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 11000 }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = "CPU" }) },
         new object[] { 248,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 486876.03173 }) },
         new object[] { 239,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,  Value = 869.4797 }) }
      };

      [Test]
      [TestCaseSource("FetchLikeCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_Like_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchLikeCases2 =
      {
         new object[] { 33,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.Like,  Value = "8%" }) },
         new object[] { 70,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 14,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.Like,  Value = "9%" }) },
         new object[] { 29,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.Like,  Value = "2%" }) },
         new object[] { 76,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "Ubuntu VM SMP%" }) },
         new object[] { 160,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "%192%" }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.Like,  Value = "%357" }) },
         new object[] { 253,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "3%" }) },
         new object[] { 27,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 27,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.Like,  Value = "3_" })},     // not a TimeSpan value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "3%" }) },    // not a String value
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.Like,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.Like,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.Like,  Value = "Work%Name%" }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.Like,  Value = "0%" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.Like,  Value = "GRO%" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "0%" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.Like,  Value = "0%" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.Like,  Value = "%U" }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.Like,  Value = "1%" }) },
         new object[] { 3,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.Like,  Value = "9%" }) }
      };

      [Test]
      [TestCaseSource("FetchNotLikeCases2")]
      [Category("HFM.Core.UnitInfoDatabase.Fetch")]
      public void Fetch_NotLike_Test2(int expected, WorkUnitHistoryQuery query)
      {
         FetchTestData2(expected, query);
      }

      private static object[] FetchNotLikeCases2 =
      {
         new object[] { 220,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectID,          Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "8%" }) },
         new object[] { 183,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectRun,         Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 239,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectClone,       Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "9%" }) },
         new object[] { 224,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.ProjectGen,         Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "2%" }) },
         new object[] { 177,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Name,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "Ubuntu VM SMP%" }) },
         new object[] { 93,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Path,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "%192%" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Username,           Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "%357" }) },
         new object[] { 0,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Team,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "3%" }) },
         new object[] { 226,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CoreVersion,        Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 1,    BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FramesCompleted,    Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 226,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.FrameTime,          Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "3_" })},     // not a TimeSpan value
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Result,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "3%" }) },    // not a String value
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.DownloadDateTime,   Type = WorkUnitHistoryQueryOperator.NotLike,  Value = new DateTime(2012, 7, 5, 0, 25, 7) }) },
         new object[] { 252,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.CompletionDateTime, Type = WorkUnitHistoryQueryOperator.NotLike,  Value = new DateTime(2012, 11, 19, 6, 56, 47) }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.WorkUnitName,       Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "Work%Name%" }) },
         new object[] { 10,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.KFactor,            Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "0%" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Core,               Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "GRO%" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Frames,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "0%" }) },
         new object[] { 20,   BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Atoms,              Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "0%" }) },
         new object[] { 233,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.SlotType,           Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "%U" }) },
         new object[] { 243,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.PPD,                Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "1%" }) },
         new object[] { 250,  BuildParameters(new WorkUnitHistoryQueryParameter { Column = WorkUnitHistoryRowColumn.Credit,             Type = WorkUnitHistoryQueryOperator.NotLike,  Value = "9%" }) }
      };

      [Test]
      public void Fetch_Complex_Test3()
      {
         FetchTestData2(52, BuildParameters(new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.DownloadDateTime,
            Type = WorkUnitHistoryQueryOperator.GreaterThan,
            Value = new DateTime(2012, 5, 29)
         },
         new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.DownloadDateTime,
            Type = WorkUnitHistoryQueryOperator.LessThan,
            Value = new DateTime(2012, 11, 1)
         }));
      }

      [Test]
      public void Fetch_Complex_Test4()
      {
         FetchTestData2(77, BuildParameters(new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.Name,
            Type = WorkUnitHistoryQueryOperator.GreaterThanOrEqual,
            Value = "Ubuntu VM SMP - Media Server"
         },
         new WorkUnitHistoryQueryParameter
         {
            Column = WorkUnitHistoryRowColumn.Name,
            Type = WorkUnitHistoryQueryOperator.LessThanOrEqual,
            Value = "n"
         }));
      }

      private static WorkUnitHistoryQuery BuildParameters(params WorkUnitHistoryQueryParameter[] fields)
      {
         var query = new WorkUnitHistoryQuery();
         query.Parameters.AddRange(fields);
         return query;
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
//         //Debug.WriteLine(query.Parameters[0].Column);
//         foreach (var entry in entries)
//         {
//            Debug.WriteLine(entry.ID);
//         }
//#endif
         Assert.AreEqual(count, entries.Count);
      }

      #endregion

      #region Page

      [Test]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_All_Test1()
      {
         // Select All
         PageTestData(44, BuildParameters());
      }

      [Test]
      [TestCaseSource("FetchEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_Equal_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchNotEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_NotEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchGreaterThanCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_GreaterThan_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchGreaterThanOrEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_GreaterThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchLessThanCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_LessThan_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchLessThanOrEqualCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_LessThanOrEqual_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchLikeCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
      public void Page_Like_Test(int expected, WorkUnitHistoryQuery query)
      {
         PageTestData(expected, query);
      }

      [Test]
      [TestCaseSource("FetchNotLikeCases")]
      [Category("HFM.Core.UnitInfoDatabase.Page")]
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
