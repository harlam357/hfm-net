/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Core;

using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core
{
   [TestFixture]
   public class ProteinServiceTests
   {
      [Test]
      public void ProteinService_FileName_Test()
      {
         // Arrange
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         prefs.Stub(x => x.Get<string>(Preference.ApplicationDataFolderPath)).Return(Environment.CurrentDirectory);
         // Act
         var service = new ProteinService(prefs, null);
         // Assert
         Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, Constants.ProjectInfoFileName), service.FileName);
      }

      [Test]
      public void ProteinService_GetAvailableProtein_Test()
      {
         // Arrange
         var service = new ProteinService();
         var protein = CreateValidProtein(2483);
         service.Add(protein);
         // Act
         var p = service.Get(2483);
         // Assert
         Assert.AreSame(protein, p);
      }

      [Test]
      public void ProteinService_GetUnavailableProtein_Test()
      {
         // Arrange
         var service = new ProteinService();
         // Act
         var p = service.Get(2482);
         // Assert
         Assert.IsNull(p);
      }

      [Test]
      public void ProteinService_GetUnavailableProtein_AddsTheProteinIdToProjectsNotFound_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateStub<IProjectSummaryDownloader>();
         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Act
         service.Get(2482, true);
         // Assert
         Assert.IsTrue(service.ProjectsNotFound.ContainsKey(2482));
      }

      [Test]
      public void ProteinService_GetWithRefreshCalledMultipleTimesOnlyTriggersOneRefresh_ByLastRefreshTime_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            })).Repeat.Once();
         
         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Act
         var p = service.Get(2482, true);
         Assert.IsNull(p);
         // Call twice to internally exercise the last refresh time
         p = service.Get(2482, true);
         Assert.IsNull(p);
         // Assert
         downloader.VerifyAllExpectations();
      }

      [Test]
      public void ProteinService_GetWithRefreshCalledMultipleTimesOnlyTriggersOneRefresh_ByProjectsNotFound_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            })).Repeat.Once();

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Act
         var p = service.Get(2482, true);
         Assert.IsNull(p);
         // Call twice to internally exercise the projects not found list
         service.LastRefreshTime = null;
         p = service.Get(2482, true);
         Assert.IsNull(p);
         // Assert
         downloader.VerifyAllExpectations();
      }

      [Test]
      public void ProteinService_GetWithRefreshAllowsRefreshWhenLastRefreshTimeElapsed_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            })).Repeat.Once();

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // set to over the elapsed time (1 hour)
         service.LastRefreshTime = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(61));
         // Act
         var p = service.Get(6940, true);
         Assert.IsNotNull(p);
         // Assert
         downloader.VerifyAllExpectations();
      }

      [Test]
      public void ProteinService_GetWithRefreshRemovesFromProjectsNotFound_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateStub<IProjectSummaryDownloader>();
         downloader.Stub(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            }));

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Set project not found to excercise removal code
         service.ProjectsNotFound.Add(6940, DateTime.MinValue);
         // Act
         Protein p = service.Get(6940, true);
         // Assert
         Assert.IsNotNull(p);
         Assert.IsFalse(service.ProjectsNotFound.ContainsKey(6940));
      }

      [Test]
      public void ProteinService_GetProjects_Test()
      {
         // Arrange
         var service = new ProteinService();
         var projects = Enumerable.Range(1, 5).ToList();
         foreach (int projectNumber in projects)
         {
            service.Add(CreateValidProtein(projectNumber));
         }
         // Act
         var serviceProjects = service.GetProjects();
         // Assert
         Assert.IsTrue(projects.SequenceEqual(serviceProjects));
      }

      [Test]
      public void ProteinService_RefreshRemovesFromProjectsNotFound_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateStub<IProjectSummaryDownloader>();
         downloader.Stub(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            }));

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         service.ProjectsNotFound.Add(6940, DateTime.MinValue);
         // Act
         service.Refresh(null);
         // Assert
         Assert.IsFalse(service.ProjectsNotFound.ContainsKey(6940));
      }

      [Test]
      public void ProteinService_RefreshUpdatesRefreshParameters_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateStub<IProjectSummaryDownloader>();
         downloader.Stub(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            }));

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         service.ProjectsNotFound.Add(2968, DateTime.MinValue);
         service.LastRefreshTime = DateTime.MinValue;
         // Act
         service.Refresh(null);
         // Assert
         Assert.AreNotEqual(DateTime.MinValue, service.ProjectsNotFound[2968]);
         Assert.AreNotEqual(DateTime.MinValue, service.LastRefreshTime);
      }

      [Test]
      public void ProteinService_RefreshLoadsData_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateStub<IProjectSummaryDownloader>();
         downloader.Stub(x => x.Download(null, null)).IgnoreArguments()
            .Callback(new Func<Stream, IProgress<ProgressInfo>, bool>((stream, progress) =>
            {
               File.OpenRead("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json").CopyTo(stream);
               return true;
            }));

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         Assert.AreEqual(0, service.GetProjects().Count);
         // Act
         service.Refresh(null);
         // Assert
         Assert.AreNotEqual(0, service.GetProjects().Count);
      }

      private static Protein CreateValidProtein(int projectNumber)
      {
         return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
      }
   }
}
