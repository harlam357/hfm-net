/*
 * HFM.NET - Client Instance Class Tests
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using NUnit.Framework;

using HFM.Proteins;
using HFM.Preferences;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceTests
   {
      [Test, Category("SMP")]
      public void TestSmpPathInstance()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP Test FAHlog 3";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "SMP Test FAHlog 3.txt";
         
         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(true, Instance.UserIDUnknown);  // UserID is Unknown (notfred's instance does not log request of UserID)
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk()); // Prefs default is harlam357 (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         
         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP Test FAHlog 3", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(1, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.LastUnitFramePercent);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.RawFramesTotal);
         
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
      }

      [Test, Category("SMP")]
      public void TestSmpPathInstance2()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP Test FAHlog INTERRUPTED Bad WUs 2";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "SMP Test INTERRUPTED Bad WUs FAHlog 2.txt";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk()); // Prefs default is harlam357 (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP Test FAHlog INTERRUPTED Bad WUs 2", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(6, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(7, Instance.CurrentUnitInfo.LastUnitFramePercent);
         Assert.AreEqual(140000, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(2000000, Instance.CurrentUnitInfo.RawFramesTotal);

         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(1180, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(1179, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(1179, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
      }

      [Test, Category("GPU")]
      public void TestGpuPathInstance()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "GPU Test FAHlog 3 EUE Pause";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "GPU Test FAHlog 3 EUE Pause.txt";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(false, Instance.IsUsernameOk()); // This log is from JollySwagman (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("GPU Test FAHlog 3 EUE Pause", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.LastUnitFramePercent);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.RawFramesTotal);

         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(119, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(119, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(94, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.EuePause, Instance.Status);
      }
   }
}
