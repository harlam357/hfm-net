/*
 * HFM.NET - UnitInfo Container Class Tests
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class UnitInfoContainerTests
   {
      [Test]
      public void ReadTest1()
      {
         var container = new UnitInfoContainer
                         {
                            FileName = Path.Combine("..\\..\\TestFiles", Constants.UnitInfoCacheFileName),
                            Serializer = new ProtoBufFileSerializer<List<UnitInfo>>()
                         };

         container.Read();
         Assert.AreEqual(6, container.Count);
      }

      [Test]
      public void ReadTest2()
      {
         var container = new UnitInfoContainer
         {
            FileName = Path.Combine("..\\..\\TestFiles", Constants.UnitInfoCacheFileName),
            Serializer = new ProtoBufFileSerializer<List<UnitInfo>>()
         };

         container.Read();
         Assert.AreEqual(0, container.Count);
      }

      [Test]
      public void WriteTest1()
      {
         var container = new UnitInfoContainer
                         {
                            FileName = "TestUnitInfoBinary.dat",
                            Serializer = new ProtoBufFileSerializer<List<UnitInfo>>()
                         };

         var list = CreateTestList();
         foreach (UnitInfo item in list)
         {
            container.Add(item);
         }
         Assert.AreEqual(list.Count, container.Count);
         container.Write();
         container.Clear();
         Assert.AreEqual(0, container.Count);
         container.Read();
         Assert.AreEqual(list.Count, container.Count);
         ValidateTestList(container.ToList());
      }

      private static IList<UnitInfo> CreateTestList()
      {
         var list = new List<UnitInfo>();
         for (int i = 0; i < 10; i++)
         {
            var unitInfo = new UnitInfo();
            unitInfo.OwningInstanceName = "TestOwner";
            unitInfo.OwningInstancePath = "TestPath";
            unitInfo.UnitRetrievalTime = new DateTime((2000 + i), 1, 1, 0, 0, 0);
            unitInfo.FoldingID = "TestID";
            unitInfo.Team = 32;
            unitInfo.SlotType = SlotType.Uniprocessor;
            unitInfo.DownloadTime = new DateTime(2000, 2, 2, 0, 0, 0);
            unitInfo.DueTime = new DateTime(2000, 3, 3, 0, 0, 0);
            unitInfo.UnitStartTimeStamp = TimeSpan.FromHours(i + 1);
            unitInfo.FinishedTime = new DateTime(2000, 4, 4, 0, 0, 0);
            unitInfo.CoreVersion = "2.10";
            unitInfo.ProjectID = 2669;
            unitInfo.ProjectRun = 1;
            unitInfo.ProjectClone = 2;
            unitInfo.ProjectGen = 3;
            unitInfo.ProteinName = "Protein";
            unitInfo.ProteinTag  = "ProteinTag";
            unitInfo.UnitResult = WorkUnitResult.CoreOutdated;
            unitInfo.RawFramesComplete = 2500;
            unitInfo.RawFramesTotal = 250000;

            for (int j = 0; j < 4; j++)
            {
               var unitFrame = new UnitFrame { FrameID = j, TimeOfFrame = TimeSpan.FromMinutes(j + 1) };
               unitInfo.UnitFrames.Add(j, unitFrame);
            }
            unitInfo.FramesObserved = 4;

            list.Add(unitInfo);
         }

         return list.AsReadOnly();
      }

      private static void ValidateTestList(IList<UnitInfo> list)
      {
         for (int i = 0; i < 10; i++)
         {
            UnitInfo unitInfo = list[i];
            
            Assert.AreEqual("TestOwner", unitInfo.OwningInstanceName);
            Assert.AreEqual("TestPath", unitInfo.OwningInstancePath);
            Assert.AreEqual(new DateTime((2000 + i), 1, 1, 0, 0, 0), unitInfo.UnitRetrievalTime);
            Assert.AreEqual("TestID", unitInfo.FoldingID);
            Assert.AreEqual(32, unitInfo.Team);
            Assert.AreEqual(SlotType.Uniprocessor, unitInfo.SlotType);
            Assert.AreEqual(new DateTime(2000, 2, 2, 0, 0, 0), unitInfo.DownloadTime);
            Assert.AreEqual(new DateTime(2000, 3, 3, 0, 0, 0), unitInfo.DueTime);
            Assert.AreEqual(TimeSpan.FromHours(i + 1), unitInfo.UnitStartTimeStamp);
            Assert.AreEqual(new DateTime(2000, 4, 4, 0, 0, 0), unitInfo.FinishedTime);
            Assert.AreEqual("2.10", unitInfo.CoreVersion);
            Assert.AreEqual(2669, unitInfo.ProjectID);
            Assert.AreEqual(1, unitInfo.ProjectRun);
            Assert.AreEqual(2, unitInfo.ProjectClone);
            Assert.AreEqual(3, unitInfo.ProjectGen);
            Assert.AreEqual("Protein", unitInfo.ProteinName);
            Assert.AreEqual("ProteinTag", unitInfo.ProteinTag);
            Assert.AreEqual(WorkUnitResult.CoreOutdated, unitInfo.UnitResult);
            Assert.AreEqual(2500, unitInfo.RawFramesComplete);
            Assert.AreEqual(250000, unitInfo.RawFramesTotal);
            
            UnitFrame unitFrame = null;
            for (int j = 0; j < 4; j++)
            {
               unitFrame = unitInfo.UnitFrames[j];
               Assert.AreEqual(j, unitFrame.FrameID);
               Assert.AreEqual(TimeSpan.FromMinutes(j + 1), unitFrame.TimeOfFrame);
            }
            Assert.AreEqual(4, unitInfo.FramesObserved);
            Assert.AreEqual(unitFrame, unitInfo.CurrentFrame);
         }
      }
   }
}
