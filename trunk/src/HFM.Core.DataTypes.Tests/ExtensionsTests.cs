/*
 * HFM.NET - Data Type Extension Methods Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;

using NUnit.Framework;

namespace HFM.Core.DataTypes.Tests
{
   [TestFixture]
   public class ExtensionsTests
   {
      #region DateTime/TimeSpan

      [Test]
      public void DateTimeIsKnownTest1()
      {
         var dateTime = new DateTime();
         Assert.IsFalse(dateTime.IsKnown());
      }

      [Test]
      public void DateTimeIsKnownTest2()
      {
         var dateTime = DateTime.Now;
         Assert.IsTrue(dateTime.IsKnown());
      }

      [Test]
      public void DateTimeIsUnknownTest1()
      {
         var dateTime = new DateTime();
         Assert.IsTrue(dateTime.IsUnknown());
      }

      [Test]
      public void DateTimeIsUnknownTest2()
      {
         var dateTime = DateTime.Now;
         Assert.IsFalse(dateTime.IsUnknown());
      }

      [Test]
      public void TimeSpanIsZeroTest1()
      {
         var timeSpan = TimeSpan.FromMinutes(1);
         Assert.IsFalse(timeSpan.IsZero());
      }

      [Test]
      public void TimeSpanIsZeroTest2()
      {
         var timeSpan = TimeSpan.Zero;
         Assert.IsTrue(timeSpan.IsZero());
      }

      #endregion

      #region IProjectInfo

      [Test]
      public void ProjectIsUnknownTest1()
      {
         var projectInfo = new ProjectInfo { ProjectID = 1 };
         Assert.IsFalse(projectInfo.ProjectIsUnknown());
      }

      [Test]
      public void ProjectIsUnknownTest2()
      {
         var projectInfo = new ProjectInfo();
         Assert.IsTrue(projectInfo.ProjectIsUnknown());
      }

      [Test]
      public void ProjectIsUnknownTest3()
      {
         IProjectInfo projectInfo = null;
         Assert.IsTrue(projectInfo.ProjectIsUnknown());
      }

      [Test]
      public void ProjectRunCloneGenTest1()
      {
         var projectInfo = new ProjectInfo();
         Assert.AreEqual("P0 (R0, C0, G0)", projectInfo.ProjectRunCloneGen());
      }

      [Test]
      public void ProjectRunCloneGenTest2()
      {
         var projectInfo = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
         Assert.AreEqual("P1 (R2, C3, G4)", projectInfo.ProjectRunCloneGen());
      }

      [Test]
      public void ProjectRunCloneGenTest3()
      {
         IProjectInfo projectInfo = null;
         Assert.AreEqual(String.Empty, projectInfo.ProjectRunCloneGen());
      }

      [Test]
      public void EqualsProjectTest1()
      {
         var projectInfo1 = new ProjectInfo();
         var projectInfo2 = new ProjectInfo();
         Assert.IsTrue(projectInfo1.EqualsProject(projectInfo2));
      }

      [Test]
      public void EqualsProjectTest2()
      {
         var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
         var projectInfo2 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
         Assert.IsTrue(projectInfo1.EqualsProject(projectInfo2));
      }

      [Test]
      public void EqualsProjectTest3()
      {
         var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
         var projectInfo2 = new ProjectInfo { ProjectID = 5, ProjectRun = 6, ProjectClone = 7, ProjectGen = 8 };
         Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
      }

      [Test]
      public void EqualsProjectTest4()
      {
         IProjectInfo projectInfo1 = null;
         var projectInfo2 = new ProjectInfo { ProjectID = 5, ProjectRun = 6, ProjectClone = 7, ProjectGen = 8 };
         Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
      }

      [Test]
      public void EqualsProjectTest5()
      {
         var projectInfo1 = new ProjectInfo { ProjectID = 1, ProjectRun = 2, ProjectClone = 3, ProjectGen = 4 };
         IProjectInfo projectInfo2 = null;
         Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
      }

      [Test]
      public void EqualsProjectTest6()
      {
         IProjectInfo projectInfo1 = null;
         IProjectInfo projectInfo2 = null;
         Assert.IsFalse(projectInfo1.EqualsProject(projectInfo2));
      }

      #endregion

      #region IProtein

      [Test]
      public void ProteinIsUnknown1()
      {
         var protein = new Protein { ProjectNumber = 1 };
         Assert.IsFalse(protein.IsUnknown());
      }

      [Test]
      public void ProteinIsUnknown2()
      {
         var protein = new Protein();
         Assert.IsTrue(protein.IsUnknown());
      }

      [Test]
      public void ProteinIsUnknown3()
      {
         Protein protein = null;
         Assert.IsTrue(protein.IsUnknown());
      }

      [Test]
      public void ProteinIsValid1()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3, MaximumDays = 5, Credit = 500, Frames = 100, KFactor = 26.4 };
         Assert.IsTrue(protein.IsValid());
      }

      [Test]
      public void ProteinIsValid2()
      {
         var protein = new Protein();
         Assert.IsFalse(protein.IsValid());
      }

      [Test]
      public void ProteinIsValid3()
      {
         Protein protein = null;
         Assert.IsFalse(protein.IsValid());
      }

      #endregion
   }
}
