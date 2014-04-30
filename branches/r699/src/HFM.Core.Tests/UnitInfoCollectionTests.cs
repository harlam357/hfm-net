/*
 * HFM.NET - UnitInfo Collection Class Tests
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;
using HFM.Core.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class UnitInfoCollectionTests
   {
      [Test]
      public void ReadBinaryTest()
      {
         var collection = new UnitInfoCollection
         {
            FileName = Path.Combine("..\\..\\TestFiles", Constants.UnitInfoCacheFileName),
         };

         collection.Read();
         Assert.AreEqual(20, collection.Count);
      }

      [Test]
      public void WriteAndReadBinaryTest()
      {
         var collection = new UnitInfoCollection
         {
            FileName = "TestUnitInfo.dat",
         };

         collection.Data = CreateTestList();
         collection.Write();
         collection.Data = null;
         collection.Read();
         Assert.IsTrue(CreateTestList().SequenceEqual(collection.Data));
      }

      [Test]
      public void WriteAndReadXmlTest()
      {
         var data1 = CreateTestList();
         var serializer = new XmlFileSerializer<List<UnitInfo>>();
         serializer.Serialize("TestUnitInfo.xml", data1);

         var data2 = serializer.Deserialize("TestUnitInfo.xml");
         Assert.IsTrue(data1.SequenceEqual(data2));
      }

      private static List<UnitInfo> CreateTestList()
      {
         var list = new List<UnitInfo>();
         for (int i = 0; i < 10; i++)
         {
            var unitInfo = new UnitInfo();
            unitInfo.OwningClientName = "TestOwner";
            unitInfo.OwningClientPath = "TestPath";
            unitInfo.UnitRetrievalTime = new DateTime((2000 + i), 1, 1, 0, 0, 0);
            unitInfo.FoldingID = "TestID";
            unitInfo.Team = 32;
            unitInfo.SlotType = SlotType.CPU;
            unitInfo.DownloadTime = new DateTime(2000, 2, 2, 0, 0, 0);
            unitInfo.DueTime = new DateTime(2000, 3, 3, 0, 0, 0);
            unitInfo.UnitStartTimeStamp = TimeSpan.FromHours(i + 1);
            unitInfo.FinishedTime = new DateTime(2000, 4, 4, 0, 0, 0);
            unitInfo.CoreVersion = 2.10f;
            unitInfo.ProjectID = 2669;
            unitInfo.ProjectRun = 1;
            unitInfo.ProjectClone = 2;
            unitInfo.ProjectGen = 3;
            unitInfo.ProteinName = "Protein";
            unitInfo.ProteinTag  = "ProteinTag";
            unitInfo.UnitResult = WorkUnitResult.CoreOutdated;
            // values set by SetUnitFrame() below
            //unitInfo.RawFramesComplete = 7500;
            //unitInfo.RawFramesTotal = 250000;

            unitInfo.FramesObserved = 4;
            for (int j = 0; j < unitInfo.FramesObserved; j++)
            {
               var unitFrame = new UnitFrame { RawFramesComplete = 2500 * j, RawFramesTotal = 250000, FrameID = j, TimeOfFrame = TimeSpan.FromMinutes((j + 1) * (j + 1.5)) };
               unitInfo.SetUnitFrame(unitFrame);
            }

            list.Add(unitInfo);
         }

         for (int i = 10; i < 20; i++)
         {
            var unitInfo = new UnitInfo();
            unitInfo.OwningClientName = "TestOwner2";
            unitInfo.OwningClientPath = "TestPath2";
            unitInfo.OwningSlotId = i - 10;
            unitInfo.UnitRetrievalTime = new DateTime((2000 + i), 1, 1, 0, 0, 0);
            unitInfo.FoldingID = "TestID";
            unitInfo.Team = 32;
            unitInfo.SlotType = SlotType.CPU;
            unitInfo.DownloadTime = new DateTime(2000, 2, 2, 0, 0, 0);
            unitInfo.DueTime = new DateTime(2000, 3, 3, 0, 0, 0);
            unitInfo.UnitStartTimeStamp = TimeSpan.FromHours(i + 1);
            unitInfo.FinishedTime = new DateTime(2000, 4, 4, 0, 0, 0);
            unitInfo.CoreVersion = 2.27f;
            unitInfo.ProjectID = 6903;
            unitInfo.ProjectRun = 1;
            unitInfo.ProjectClone = 2;
            unitInfo.ProjectGen = 3;
            unitInfo.ProteinName = "Protein";
            unitInfo.ProteinTag = "ProteinTag";
            unitInfo.UnitResult = WorkUnitResult.CoreOutdated;
            // values set by SetUnitFrame() below
            //unitInfo.RawFramesComplete = 7500;
            //unitInfo.RawFramesTotal = 250000;

            unitInfo.FramesObserved = 4;
            for (int j = 0; j < unitInfo.FramesObserved; j++)
            {
               var unitFrame = new UnitFrame { RawFramesComplete = 2500 * j, RawFramesTotal = 250000, FrameID = j, TimeOfFrame = TimeSpan.FromMinutes((j + 2) * (j + 0.75)) };
               unitInfo.SetUnitFrame(unitFrame);
            }

            list.Add(unitInfo);
         }

         return list;
      }
   }
}
