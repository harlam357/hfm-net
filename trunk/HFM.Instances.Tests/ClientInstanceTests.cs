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
using System.Diagnostics;

using NUnit.Framework;

using HFM.Proteins;
using HFM.Instrumentation;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceTests
   {
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;
      }

      [Test, Category("SMP")]
      public void TestSmpPathInstance()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         // Inject our own delegate for getting the Protein
         // to isolate from what's actually available in the
         // ProteinCollection Cache (or psummary).
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 2677;
            p.Core = "GROCVS";
            return p;
         };
         
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
         Assert.AreEqual(0, Instance.CurrentUnitInfo.LastUnitFrameID);
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
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         // Inject our own delegate for getting the Protein
         // to isolate from what's actually available in the
         // ProteinCollection Cache (or psummary).
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 5101;
            p.Core = "GRO-SMP";
            return p;
         };
         
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
         Assert.AreEqual(7, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(7, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(7, Instance.CurrentUnitInfo.LastUnitFrameID);
         Assert.AreEqual(140000, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(2000000, Instance.CurrentUnitInfo.RawFramesTotal);

         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(1180, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(1179, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(1179, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
      }

      [Test, Category("SMP")]
      public void TestSmpPathInstance3()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         // Inject our own delegate for getting the Protein
         // to isolate from what's actually available in the
         // ProteinCollection Cache (or psummary).
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 2669;
            p.Core = "GROCVS";
            return p;
         };
         
         Instance.InstanceName = "SMP Test 4";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "SMP Test 4 FAHlog.txt";
         Instance.RemoteQueueFilename = "SMP Test 4 queue.dat";
         Instance.RemoteUnitInfoFilename = "SMP Test 4 unitinfo.txt";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual("25932070F496A89", Instance.UserID);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk()); // Prefs default is harlam357 (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP Test 4", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(31, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.LastUnitFrameID);
         Assert.AreEqual(80000, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.RawFramesTotal);

         //Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(814, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(758, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(788, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
      }

      [Test, Category("SMP")]
      public void TestSmpPathInstance5()
      {
         /*** The Test below shows us that because there is no Project information available
          *   in the FAHlog for the current WU, the CurrentLogLines cannot be matched against
          *   the Current Queue Entry, so the Queue Entries are thrown out and we're left with
          *   only the FAHlog to parse.  Since the CurrentLogLines contain no Project information
          *   and the unitinfo file is unavailable, the TypeOfClient is Unknown, since the
          *   Project cannot be determined.
          ***/
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         /*** Return a new Protein since the Project information
          *   is unavailable.
          ***/
         ProteinCollection.GetProteinHandler = delegate
         {
            return new Protein();
         };

         Instance.InstanceName = "SMP Test 5";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "SMP Test 5 FAHlog.txt";
         Instance.RemoteQueueFilename = "SMP Test 5 queue.dat";
         //Instance.RemoteUnitInfoFilename = "SMP Test 5 unitinfo.txt";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual("775112477C3C55C2", Instance.UserID);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk()); // Prefs default is harlam357 (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.Unknown, Instance.CurrentUnitInfo.TypeOfClient); /* Unknown */
         Assert.AreEqual("SMP Test 5", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(true, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.LastUnitFrameID);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawFramesTotal);

         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
      }
      
      [Test, Category("SMP")]
      public void TestSmpPathInstance5_1()
      {
         /*** The Test below now gives us access to the unitinfo file, which will
          *   result in Project information becoming available.  Return the proper
          *   Protein and allow the TypeOfClient to be set correctly.
          ***/

         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         /*** Return the Protein that matches the data in the
          *   unitinfo file since it will be available.
          ***/
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 2683;
            p.Core = "GROCVS";
            return p;
         };

         Instance.InstanceName = "SMP Test 5_1";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "SMP Test 5 FAHlog.txt";
         //Instance.RemoteQueueFilename = "SMP Test 5 queue.dat";
         // Make the unitinfo available for parsing
         Instance.RemoteUnitInfoFilename = "SMP Test 5 unitinfo.txt";
         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         // Because Project information was found in the unitinfo file, we were
         // able to assign the correct TypeOfClient
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
      }

      [Test, Category("GPU")]
      public void TestGpuPathInstance()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         // Inject our own delegate for getting the Protein
         // to isolate from what's actually available in the
         // ProteinCollection Cache (or psummary).
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 5756;
            p.Core = "GROGPU2";
            return p;
         };
         
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
         Assert.AreEqual(4, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.LastUnitFrameID);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.RawFramesTotal);

         Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(119, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(119, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(94, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.EuePause, Instance.Status);
      }

      [Test, Category("GPU")]
      public void TestGpuPathInstance4()
      {
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         // Inject our own delegate for getting the Protein
         // to isolate from what's actually available in the
         // ProteinCollection Cache (or psummary).
         ProteinCollection.GetProteinHandler = delegate
         {
            Protein p = new Protein();
            p.ProjectNumber = 5770;
            p.Core = "GROGPU2";
            return p;
         };

         Instance.InstanceName = "GPU Test 4";
         Instance.Path = "..\\..\\TestFiles";
         Instance.RemoteFAHLogFilename = "GPU Test 4 FAHlog.txt";
         Instance.RemoteUnitInfoFilename = "GPU Test 4 unitinfo.txt";
         Instance.RemoteQueueFilename = "GPU Test 4 queue.dat";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk()); // Prefs default is harlam357 (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("GPU Test 4", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\TestFiles", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitFrames);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.FramesObserved);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.FramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.PercentComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.LastUnitFrameID);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.RawFramesTotal);

         //Assert.AreEqual(0, Instance.CurrentUnitInfo.RawTimePerUnitDownload);
         Assert.AreEqual(36, Instance.CurrentUnitInfo.RawTimePerAllSections);
         Assert.AreEqual(38, Instance.CurrentUnitInfo.RawTimePerThreeSections);
         Assert.AreEqual(38, Instance.CurrentUnitInfo.RawTimePerLastSection);

         Assert.AreEqual(ClientStatus.GettingWorkPacket, Instance.Status);
      }
   }
}
