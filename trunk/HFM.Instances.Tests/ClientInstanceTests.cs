/*
 * HFM.NET - Client Instance Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Log;
using HFM.Queue;
using HFM.Instrumentation;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceTests
   {
      private IWindsorContainer container;
      private MockRepository mocks;
      private IProteinBenchmarkContainer benchmarkCollection;
   
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;

         container = new WindsorContainer();
         mocks = new MockRepository();
         benchmarkCollection = mocks.DynamicMock<IProteinBenchmarkContainer>();
      
         container.AddComponent("DataAggregator", typeof(IDataAggregator), typeof(DataAggregator));
         container.AddComponent("LogReader", typeof(ILogReaderFactory), typeof(LogReaderFactory));
         container.AddComponent("QueueReader", typeof(IQueueReader), typeof(QueueReader));
         container.AddComponent("UnitInfoFactory", typeof(IUnitInfoFactory), typeof(UnitInfoFactory));
         InstanceProvider.SetContainer(container);
      }

      [Test, Category("SMP")]
      public void SMP_3()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP_3";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_3";
         
         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.AreEqual(true, Instance.UserIDUnknown);  // UserID is Unknown (notfred's instance does not log request of UserID)
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk());
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         
         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP_3", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_3", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
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

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_7()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP_7";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_7";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual("25932070F496A89", Instance.UserID);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk());
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP_7", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_7", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
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

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_8_1()
      {
         /*** The Test below shows us that because there is no Project information available
          *   in the FAHlog for the current WU, the UnitLogLines for the Current Queue Index 
          *   cannot be matched against the Current Queue Entry.  As of 1/31/10 we Queue Entries 
          *   are left in tact and the CurrentWorkUnitLogLines are force parsed to match the
          *   Current Queue Index, so now we do know the TypeOfClient and the Project (R/C/G).
          ***/

         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP_8_1";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_8";
         Instance.RemoteUnitInfoFilename = "wrong_file_name.txt";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual("775112477C3C55C2", Instance.UserID);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk());
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("SMP_8_1", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_8", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
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

         mocks.VerifyAll();
      }
      
      [Test, Category("SMP")]
      public void SMP_8_2()
      {
         /*** The Test below now gives us access to the unitinfo file, which will
          *   result in Project information becoming available.  Return the proper
          *   Protein and allow the TypeOfClient to be set correctly.
          ***/

         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "SMP_8_2";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_8";
         // Make the queue.dat unavailable for parsing
         Instance.RemoteQueueFilename = "wrong_file_name.dat";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         // Because Project information was found in the unitinfo file, we were
         // able to assign the correct TypeOfClient
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.TypeOfClient);

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_3()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "GPU2_3";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_3";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(false, Instance.IsUsernameOk()); // This log is from JollySwagman (32)
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("GPU2_3", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\GPU2_3", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
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

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_6()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();
      
         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection, InstanceType.PathInstance);
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.InstanceName = "GPU2_6";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_6";

         // Retrieve Log File and Assert Results
         Instance.Retrieve();
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.AreEqual(false, Instance.UserIDUnknown);
         Assert.AreEqual(String.Format("{0} ({1})", Instance.UserID, Instance.MachineID), Instance.UserAndMachineID);
         Assert.AreEqual(true, Instance.IsUsernameOk());
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));

         Assert.IsNotNull(Instance.CurrentUnitInfo);

         // Check Client Type and Owning Instance Properties
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.TypeOfClient);
         Assert.AreEqual("GPU2_6", Instance.CurrentUnitInfo.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\GPU2_6", Instance.CurrentUnitInfo.OwningInstancePath);

         Assert.AreEqual(false, Instance.CurrentUnitInfo.ProjectIsUnknown);
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

         mocks.VerifyAll();
      }

      private IProteinCollection SetupMockProteinCollection(string Core, int Frames)
      {
         IProtein currentProtein = mocks.DynamicMock<IProtein>();
         Expect.Call(currentProtein.Core).Return(Core).Repeat.Any();
         Expect.Call(currentProtein.Frames).Return(Frames).Repeat.Any();

         IProtein newProtein = mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Frames).Return(Frames).Repeat.Any();

         IProteinCollection proteinCollection = mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0)).Return(currentProtein).IgnoreArguments().Repeat.Any();
         Expect.Call(proteinCollection.GetNewProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
      }

      private IPreferenceSet SetupMockPreferenceSet(string Username, int Team)
      {
         IPreferenceSet Prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(Prefs.GetPreference<string>(Preference.StanfordID)).Return(Username).Repeat.Any();
         Expect.Call(Prefs.GetPreference<int>(Preference.TeamID)).Return(Team).Repeat.Any();
         Expect.Call(Prefs.CacheDirectory).Return(String.Empty).Repeat.Any();
         return Prefs;
      }
   }
}
