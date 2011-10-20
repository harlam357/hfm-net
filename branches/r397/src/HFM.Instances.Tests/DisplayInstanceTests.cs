/*
 * HFM.NET - Display Instance Class Tests
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
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using ProtoBuf;

using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class DisplayInstanceTests
   {
      [Test]
      public void DisplayInstanceSerializeTest()
      {
         var displayInstance = new DisplayInstance
                               {
                                  Settings = new ClientInstanceSettings(),
                                  Arguments = "-verbosity 9",
                                  UserId = "A3B4",
                                  MachineId = 2,
                                  Status = ClientStatus.RunningAsync,
                                  TotalRunFailedUnits = 2,
                                  CurrentLogLines = new List<LogLine> { new LogLine() },
                                  LastRetrievalTime = new DateTime(2010, 1, 1),
                                  ClientVersion = "6.30",
                                  TotalRunCompletedUnits = 15,
                                  TotalClientCompletedUnits = 300,
                                  UnitInfo = new UnitInfo()
                               };
         displayInstance.BuildUnitInfoLogic(new Protein());      

         using (var fileStream = new FileStream("DisplayInstance.dat", FileMode.Create, FileAccess.Write))
         {
            Serializer.Serialize(fileStream, displayInstance);
         }

         DisplayInstance displayInstance2;
         using (var fileStream = new FileStream("DisplayInstance.dat", FileMode.Open, FileAccess.Read))
         {
            displayInstance2 = Serializer.Deserialize<DisplayInstance>(fileStream);
         }
         displayInstance2.BuildUnitInfoLogic(new Protein());

         Assert.IsNotNull(displayInstance2.UnitInfo);
         Assert.IsNotNull(displayInstance2.Settings);
         Assert.AreEqual("-verbosity 9", displayInstance2.Arguments);
         Assert.AreEqual("A3B4", displayInstance2.UserId);
         Assert.AreEqual(2, displayInstance2.MachineId);
         Assert.AreEqual(ClientStatus.RunningAsync, displayInstance2.Status);
         Assert.AreEqual(2, displayInstance2.TotalRunFailedUnits);
         Assert.IsNotNull(displayInstance2.CurrentLogLines);
         Assert.AreEqual(new DateTime(2010, 1, 1), displayInstance2.LastRetrievalTime);
         Assert.AreEqual("6.30", displayInstance2.ClientVersion);
         Assert.AreEqual(15, displayInstance2.TotalRunCompletedUnits);
         Assert.AreEqual(300, displayInstance2.TotalClientCompletedUnits);
      }
   }
}
