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
using System.Globalization;

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
      
         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = true;
         Instance.InstanceName = "SMP_3";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_3";
         #endregion
         
         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-local -forceasm -smp 4", Instance.Arguments);
         Assert.AreEqual("", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(1, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(0, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_3", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_3", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(13, 18, 28), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.08", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2677, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(14, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(69, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(39, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(1, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_7()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_7";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_7";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-smp -verbosity 9", Instance.Arguments);
         Assert.AreEqual("25932070F496A89", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(27, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(338, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2009, 10, 3, 7, 52, 7), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 10, 6, 7, 52, 7), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(7, 52, 7), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.10", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2669, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(13, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(159, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(153, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P2669R13C159G153", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(80000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(31, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_8_1()
      {
         /*** The Test below shows us that because there is no Project information available
          *   in the FAHlog for the current WU, the UnitLogLines for the Current Queue Index 
          *   cannot be matched against the Current Queue Entry.  As of 1/31/10 the Queue Entries 
          *   are left in tact and the CurrentWorkUnitLogLines are force parsed to match the
          *   Current Queue Index, so now we do know the TypeOfClient and the Project (R/C/G).
          ***/

         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_8_1";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_8";
         Instance.RemoteUnitInfoFilename = "wrong_file_name.txt";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(3, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-verbosity 9 -smp 8 -bigadv", Instance.Arguments);
         Assert.AreEqual("775112477C3C55C2", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(2, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(2, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_8_1", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_8", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2009, 11, 24, 21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 11, 30, 21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.10", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2683, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(2, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(8, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(24, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P2683R2C8G24", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }
      
      [Test, Category("SMP")]
      public void SMP_8_2()
      {
         /*** The Test below now gives us access to the unitinfo.txt file 
          *   but not the queue.dat.  This will allow us to still parse the
          *   logs but read the Project (R/C/G) from the unitinfo.txt file instead.
          ***/

         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_8_2";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_8";
         // Make the queue.dat unavailable for parsing
         Instance.RemoteQueueFilename = "wrong_file_name.dat";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-verbosity 9 -smp 8 -bigadv", Instance.Arguments);
         Assert.AreEqual("775112477C3C55C2", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(2, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(2, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_8_2", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_8", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 11, 24, 21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 11, 30, 21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(21, 53, 46), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.10", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2683, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(2, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(8, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(24, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P2683R2C8G24", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_9()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_9";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_9";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(5, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-bigadv -smp 7", Instance.Arguments);
         Assert.AreEqual("483863F0D7DA6E3", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("coccola", Instance.FoldingID);
         Assert.AreEqual(86565, Instance.Team);
         Assert.AreEqual(1, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(4, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_9", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_9", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("coccola", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(86565, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2010, 1, 22, 4, 37, 17), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 1, 28, 4, 37, 17), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(4, 37, 17), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.10", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2681, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(9, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(8, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(55, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P2681R9C8G55", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(127500, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(50, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_11() // mangled Project string on current WU
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_11";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_11";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(5, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-smp 4 -verbosity 9", Instance.Arguments);
         Assert.AreEqual("107E28DB39449FEF", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("Felix_Pasqualli", Instance.FoldingID);
         Assert.AreEqual(52523, Instance.Team);
         Assert.AreEqual(13, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(52, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_11", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_11", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("Felix_Pasqualli", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(52523, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2009, 12, 29, 9, 7, 48), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 1, 1, 9, 7, 48), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(9, 7, 48), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.10", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(2671, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(30, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(81, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(165, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P2671R30C81G165", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(150000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(250000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(59, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("SMP")]
      public void SMP_12() // CoreOutdated on last Queue Index 6
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GRO-A3", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "SMP_12";
         Instance.Path = "..\\..\\..\\TestFiles\\SMP_12";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(7, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-smp -verbosity 9 -forceasm", Instance.Arguments);
         Assert.AreEqual("5E8F3E2C4E01B2DB", Instance.UserId);
         Assert.AreEqual(1, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(118, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(1, Instance.TotalRunFailedUnits);
         Assert.AreEqual(118, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("SMP_12", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\SMP_12", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.SMP, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2010, 3, 24, 7, 21, 46), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 3, 30, 7, 21, 46), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(7, 21, 51), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("2.17", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(6024, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(9, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(78, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Gromacs", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P6024R0C9G78", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(485000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(500000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(98, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_3()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "GPU2_3";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_3";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.EuePause, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.EuePause, Instance.Status);
         Assert.AreEqual(false, Instance.ProductionValuesOk);
         Assert.AreEqual("", Instance.Arguments);
         Assert.AreEqual("1D1493BB0A79C9AE", Instance.UserId);
         Assert.AreEqual(2, Instance.MachineId);
         Assert.AreEqual("JollySwagman", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(1, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(5, Instance.TotalRunFailedUnits);
         Assert.AreEqual(224, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("GPU2_3", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\GPU2_3", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("JollySwagman", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(5, 59, 23), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("1.19", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(5756, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(6, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(480, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.UnstableMachine, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_6_1()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "GPU2_6_1";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_6";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(8, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.GettingWorkPacket, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.GettingWorkPacket, Instance.Status);
         Assert.AreEqual(false, Instance.ProductionValuesOk);
         Assert.AreEqual("-gpu 1 -verbosity 9", Instance.Arguments);
         Assert.AreEqual("5E8F3E2C4E01B2DB", Instance.UserId);
         Assert.AreEqual(3, Instance.MachineId);
         Assert.AreEqual("harlam357", Instance.FoldingID);
         Assert.AreEqual(32, Instance.Team);
         Assert.AreEqual(205, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(4907, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("GPU2_6_1", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\GPU2_6", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("harlam357", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(32, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2009, 11, 26, 1, 30, 40), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2009, 11, 29, 1, 30, 40), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(1, 30, 40), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(new DateTime(2009, 11, 26, 2, 32, 20), Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("1.19", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(5770, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(4, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(242, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(1366, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("Protein", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("P5770R4C242G1366", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion         

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_6_2_QueueClearTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "GPU2_6_2";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_6";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion
         
         Assert.IsTrue(Instance.DataAggregator.Queue.DataPopulated);
         Instance.RemoteQueueFilename = "wrong_file_name.dat";

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion
         
         Assert.IsNull(Instance.DataAggregator.Queue);

         mocks.VerifyAll();
      }

      [Test, Category("GPU")]
      public void GPU2_7()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROGPU2", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "GPU2_7";
         Instance.Path = "..\\..\\..\\TestFiles\\GPU2_7";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("", Instance.Arguments);
         Assert.AreEqual("xxxxxxxxxxxxxxxxxxx", Instance.UserId);
         Assert.AreEqual(2, Instance.MachineId);
         Assert.AreEqual("Zagen30", Instance.FoldingID);
         Assert.AreEqual(46301, Instance.Team);
         Assert.AreEqual(0, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(1994, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("GPU2_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\GPU2_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("Zagen30", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(46301, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.GPU, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(1, 57, 21), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("1.31", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(5781, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(2, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(700, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(2, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(5, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(100, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(5, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("Standard")]
      public void Standard_5() // multiple Project strings before WU start
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("ProtoMol", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "Standard_5";
         Instance.Path = "..\\..\\..\\TestFiles\\Standard_5";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNotNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(4, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[2]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[3]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[4]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[5]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[6]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[7]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[8]);
         Assert.IsNull(Instance.DataAggregator.UnitLogLines[9]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-forceasm -verbosity 9 -oneunit", Instance.Arguments);
         Assert.AreEqual("722723950C6887C2", Instance.UserId);
         Assert.AreEqual(3, Instance.MachineId);
         Assert.AreEqual("borden.b", Instance.FoldingID);
         Assert.AreEqual(131, Instance.Team);
         Assert.AreEqual(0, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(0, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("Standard_5", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\Standard_5", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("borden.b", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(131, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.Standard, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(new DateTime(2010, 3, 23, 22, 41, 07), Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 4, 15, 23, 38, 42), Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(0, 41, 07), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("23", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(10002, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(19, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(51, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual("ProtoMol p10002", Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual("-", Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(110000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(1000000, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(5, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test, Category("Standard")]
      public void Standard_7() // new ProtoMol - progress not on percent boundry
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("ProtoMol", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         #region Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;
         // Don't Handle Status
         Instance.HandleStatusOnRetrieve = false;
         Instance.ClientIsOnVirtualMachine = false;
         Instance.InstanceName = "Standard_7";
         Instance.Path = "..\\..\\..\\TestFiles\\Standard_7";
         #endregion

         #region Retrieve Log Files
         Instance.Retrieve();
         Assert.Greater(Instance.LastRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         #endregion

         #region Check Data Aggregator
         Assert.IsNull(Instance.DataAggregator.Queue);
         Assert.AreEqual(1, Instance.DataAggregator.CurrentUnitIndex);
         Assert.IsNotNull(Instance.DataAggregator.CurrentClientRun);
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.DataAggregator.CurrentWorkUnitStatus);
         Assert.IsNotNull(Instance.DataAggregator.CurrentLogLines);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[0]);
         Assert.IsNotNull(Instance.DataAggregator.UnitLogLines[1]);
         Assert.AreEqual(Instance.DataAggregator.CurrentLogLines, Instance.DataAggregator.UnitLogLines[Instance.DataAggregator.CurrentUnitIndex]);
         #endregion

         #region Check Instance Level Values
         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, Instance.Status);
         Assert.AreEqual(true, Instance.ProductionValuesOk);
         Assert.AreEqual("-svcstart -d C:\\Program Files\\Folding@Home\\FAH4", Instance.Arguments);
         Assert.AreEqual("1E0689735BB36054", Instance.UserId);
         Assert.AreEqual(4, Instance.MachineId);
         Assert.AreEqual("NerdZone", Instance.FoldingID);
         Assert.AreEqual(155945, Instance.Team);
         Assert.AreEqual(0, Instance.TotalRunCompletedUnits);
         Assert.AreEqual(0, Instance.TotalRunFailedUnits);
         Assert.AreEqual(0, Instance.TotalClientCompletedUnits);
         #endregion

         #region Check Unit Info Data Values
         Assert.IsNotNull(Instance.CurrentUnitInfo);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData);
         Assert.AreEqual("Standard_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstanceName);
         Assert.AreEqual("..\\..\\..\\TestFiles\\Standard_7", Instance.CurrentUnitInfo.UnitInfoData.OwningInstancePath);
         Assert.Greater(Instance.CurrentUnitInfo.UnitInfoData.UnitRetrievalTime, DateTime.Now.Subtract(TimeSpan.FromMinutes(5)));
         Assert.AreEqual("NerdZone", Instance.CurrentUnitInfo.UnitInfoData.FoldingID);
         Assert.AreEqual(155945, Instance.CurrentUnitInfo.UnitInfoData.Team);
         Assert.AreEqual(ClientType.Standard, Instance.CurrentUnitInfo.UnitInfoData.TypeOfClient);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DownloadTime);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.DueTime);
         Assert.AreEqual(new TimeSpan(23, 36, 17), Instance.CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp);
         Assert.AreEqual(DateTime.MinValue, Instance.CurrentUnitInfo.UnitInfoData.FinishedTime);
         Assert.AreEqual("23", Instance.CurrentUnitInfo.UnitInfoData.CoreVersion);
         Assert.AreEqual(10015, Instance.CurrentUnitInfo.UnitInfoData.ProjectID);
         Assert.AreEqual(3609, Instance.CurrentUnitInfo.UnitInfoData.ProjectRun);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.ProjectClone);
         Assert.AreEqual(0, Instance.CurrentUnitInfo.UnitInfoData.ProjectGen);
         Assert.AreEqual(String.Empty, Instance.CurrentUnitInfo.UnitInfoData.ProteinName);
         Assert.AreEqual(String.Empty, Instance.CurrentUnitInfo.UnitInfoData.ProteinTag);
         Assert.AreEqual(WorkUnitResult.Unknown, Instance.CurrentUnitInfo.UnitInfoData.UnitResult);
         Assert.AreEqual(164800, Instance.CurrentUnitInfo.UnitInfoData.RawFramesComplete);
         Assert.AreEqual(499375, Instance.CurrentUnitInfo.UnitInfoData.RawFramesTotal);
         Assert.AreEqual(34, Instance.CurrentUnitInfo.UnitInfoData.FramesObserved);
         Assert.IsNotNull(Instance.CurrentUnitInfo.UnitInfoData.CurrentFrame);
         #endregion

         mocks.VerifyAll();
      }

      [Test]
      public void ClientInstancePropertyTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet Prefs = SetupMockPreferenceSet("harlam357", 32);
         mocks.ReplayAll();

         // Setup Test Instance
         ClientInstance Instance = new ClientInstance(Prefs, proteinCollection, benchmarkCollection);
         Instance.InstanceHostType = InstanceType.PathInstance;

         Assert.AreEqual(String.Empty, Instance.ClientPathAndArguments);
         Instance.Path = @"C:\ThePath\To\The\Files\";
         Instance.Arguments = "-some -flags";
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Instance.Path, Instance.Arguments),
                         Instance.ClientPathAndArguments);
                         
         Assert.AreEqual(true, Instance.UserIdUnknown);
         Instance.UserId = "SOMEUSERID";
         Assert.AreEqual(false, Instance.UserIdUnknown);
         
         Assert.AreEqual(0, Instance.MachineId);
         Instance.MachineId = 1;
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Instance.UserId, Instance.MachineId), 
                         Instance.UserAndMachineId);

         // True if the defaults are in place
         Assert.AreEqual(true, Instance.IsUsernameOk());
         
         Instance.FoldingID = "user";
         Instance.Team = 3232;
         Assert.AreEqual(true, Instance.IsUsernameOk());
         // Status must not be Unknown or Offline for function to evaluate false
         Instance.Status = ClientStatus.RunningNoFrameTimes;
         Assert.AreEqual(false, Instance.IsUsernameOk());
         
         Instance.FoldingID = "harlam357";
         Instance.Team = 32;
         Assert.AreEqual(true, Instance.IsUsernameOk());
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Instance.FoldingID, Instance.Team),
                         Instance.FoldingIDAndTeam);
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
