/*
 * HFM.NET - Protein Service Tests
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

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
         prefs.Stub(x => x.ApplicationDataFolderPath).Return(Environment.CurrentDirectory);
         // Act
         var service = new ProteinService(prefs, null);
         // Assert
         Assert.AreEqual(System.IO.Path.Combine(Environment.CurrentDirectory, Constants.ProjectInfoFileName), service.FileName);
      }

      [Test]
      public void ProteinService_Get_Test1()
      {
         // Arrange
         var service = new ProteinService();
         var protein = CreateValidProtein(2483);
         service.Add(protein);
         // Act & Assert
         Protein p = service.Get(2483);
         Assert.IsNotNull(p);
         p = service.Get(2482);
         Assert.IsNull(p);
      }

      [Test]
      public void ProteinService_Get_Test2()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download()).Repeat.Once();
         downloader.Stub(x => x.FilePath).Return("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json");
         
         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         var protein = CreateValidProtein(2483);
         service.Add(protein);
         // Act
         Protein p = service.Get(2483, true);
         Assert.IsNotNull(p);
         p = service.Get(2482, true);
         Assert.IsNull(p);
         // Call twice to internally exercise the projects not found list
         p = service.Get(2482, true);
         Assert.IsNull(p);
         // Assert
         downloader.VerifyAllExpectations();
      }

      [Test]
      public void ProteinService_Get_Test3()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Act
         service.Get(2482, true);
         // Assert
         Assert.IsTrue(service.ProjectsNotFound.ContainsKey(2482));
      }

      [Test]
      public void ProteinService_Get_Test4()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download()).Repeat.Once();
         downloader.Stub(x => x.FilePath).Return("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json");

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         // Set project not found to excercise removal code
         service.ProjectsNotFound.Add(6940, DateTime.MinValue);
         // Act
         Protein p = service.Get(6940, true);
         Assert.IsNotNull(p);
         // Assert
         downloader.VerifyAllExpectations();
         Assert.IsFalse(service.ProjectsNotFound.ContainsKey(6940));
      }

      [Test]
      public void ProteinService_GetProjects_Test1()
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
      public void ProteinService_ResetRefreshParameters_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.ResetDownloadParameters()).Repeat.Once();
         var service = new ProteinService(null, downloader);
         service.ProjectsNotFound.Add(2968, DateTime.MinValue);
         // Act
         service.ResetRefreshParameters();
         // Assert
         Assert.IsFalse(service.ProjectsNotFound.ContainsKey(2968));
         downloader.VerifyAllExpectations();
      }

      [Test]
      public void ProteinService_Refresh_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.Download()).Repeat.Once();
         downloader.Stub(x => x.FilePath).Return("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json");

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         Assert.AreEqual(0, service.GetProjects().Count());
         // Act
         service.Refresh();
         // Assert
         Assert.AreNotEqual(0, service.GetProjects().Count());
      }

      [Test]
      public void ProteinService_RefreshAsync_Test()
      {
         // Arrange
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         var taskSource = new TaskCompletionSource<object>();
         taskSource.SetResult(null);
         downloader.Expect(x => x.DownloadAsync(null)).Return(taskSource.Task).Repeat.Once();
         downloader.Stub(x => x.FilePath).Return("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\summary.json");

         var service = new ProteinService(null, downloader) { Logger = new Logging.DebugLogger() };
         Assert.AreEqual(0, service.GetProjects().Count());
         // Act
         service.RefreshAsync(null).Wait();
         // Assert
         Assert.AreNotEqual(0, service.GetProjects().Count());
      }

      private static Protein CreateValidProtein(int projectNumber)
      {
         return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
      }
   }
}
