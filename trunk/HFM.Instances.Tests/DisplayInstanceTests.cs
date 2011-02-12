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
using Rhino.Mocks;
using ProtoBuf;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class DisplayInstanceTests
   {
      private MockRepository _mocks;
      private IPreferenceSet _prefs;
      private IProteinCollection _proteinCollection;
   
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         _proteinCollection = _mocks.DynamicMock<IProteinCollection>();
      }
   
      [Test]
      public void DisplayInstanceSerializeTest()
      {
         Expect.Call(_prefs.GetPreference<string>(Preference.StanfordId)).Return("harlam357").Repeat.Any();
         Expect.Call(_prefs.GetPreference<int>(Preference.TeamId)).Return(32).Repeat.Any();
         
         var protein = _mocks.DynamicMock<IProtein>();
         SetupResult.For(protein.Core).Return("GROCVS");
         SetupResult.For(protein.Frames).Return(100);
         SetupResult.For(protein.Credit).Return(300);
         SetupResult.For(protein.PreferredDays).Return(3);
         SetupResult.For(protein.IsUnknown).Return(false);
         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(5000).Repeat.Any();
         Expect.Call(protein.GetUPD(TimeSpan.Zero)).IgnoreArguments().Return(3.5).Repeat.Any();
         Expect.Call(_proteinCollection.GetProtein(0, false)).IgnoreArguments().Return(protein).Repeat.Any();

         _mocks.ReplayAll();
      
         var unitInfo = new UnitInfo
                        {
                           // These three properties are set in the UnitInfoLogic constructor
                           //OwningInstanceName
                           //OwningInstancePath
                           //UnitRetrievalTime
                           FoldingID = "harlam357",
                           Team = 32,
                           TypeOfClient = ClientType.SMP,
                           DownloadTime = new DateTime(2009, 12, 30),
                           DueTime = new DateTime(2010, 1, 3),
                           UnitStartTimeStamp = TimeSpan.FromMinutes(5),
                           FinishedTime = new DateTime(2010, 1, 2),
                           CoreVersion = "2.09",
                           ProjectID = 2669,
                           ProjectRun = 1,
                           ProjectClone = 2,
                           ProjectGen = 3,
                           ProteinName = "The Protein Name",
                           ProteinTag = "The Protein Tag",
                           UnitResult = WorkUnitResult.FinishedUnit,
                           RawFramesComplete = 10000,
                           RawFramesTotal = 250000,
                           FramesObserved = 4,
                           CoreID = "A2"
                        };
         unitInfo.UnitFrames.Add(0, new UnitFrame { FrameID = 0, TimeOfFrame = TimeSpan.FromMinutes(0) });
         unitInfo.UnitFrames.Add(1, new UnitFrame { FrameID = 1, TimeOfFrame = TimeSpan.FromMinutes(5) });
         unitInfo.UnitFrames.Add(2, new UnitFrame { FrameID = 2, TimeOfFrame = TimeSpan.FromMinutes(10) });
         unitInfo.UnitFrames.Add(3, new UnitFrame { FrameID = 3, TimeOfFrame = TimeSpan.FromMinutes(15) });
         unitInfo.UnitFrames.Add(4, new UnitFrame { FrameID = 4, TimeOfFrame = TimeSpan.FromMinutes(20) });
         foreach (var frame in unitInfo.UnitFrames.Values)
         {
            frame.FrameDuration = TimeSpan.FromMinutes(5);
         }

         var settings = new ClientInstanceSettings
                        {
                           InstanceName = "ClientName",
                           ClientProcessorMegahertz = 1000,
                           RemoteFAHLogFilename = "FAHlog.txt",
                           RemoteUnitInfoFilename = "unitinfo.txt",
                           RemoteQueueFilename = "queue.dat",
                           Path = "SomePath",
                           Server = "ServerName",
                           Username = "username",
                           Password = "password",
                           FtpMode = FtpType.Passive,
                           ClientIsOnVirtualMachine = true,
                           ClientTimeOffset = 10
                        };

         var displayInstance = new DisplayInstance
                               {
                                  ProteinCollection = _proteinCollection,
                                  Settings = settings,
                                  Arguments = "-verbosity 9",
                                  UserId = "A3B4",
                                  MachineId = 2,
                                  Status = ClientStatus.RunningAsync,
                                  TotalRunFailedUnits = 2,
                                  CurrentLogLines =
                                     new List<LogLine>
                                     {
                                        new LogLine
                                        {
                                           LineIndex = 0,
                                           LineType = LogLineType.LogOpen,
                                           LineRaw = "LogOpen",
                                           LineData = 0
                                        }
                                     },
                                  LastRetrievalTime = new DateTime(2010, 1, 1),
                                  ClientVersion = "6.30",
                                  UserIdIsDuplicate = true,
                                  ProjectIsDuplicate = true,
                                  TotalRunCompletedUnits = 15,
                                  TotalClientCompletedUnits = 300,
                                  UnitInfo = unitInfo
                               };
         displayInstance.BuildUnitInfoLogic();      

         using (var fileStream = new FileStream("DisplayInstance.dat", FileMode.Create, FileAccess.Write))
         {
            Serializer.Serialize(fileStream, displayInstance);
         }

         DisplayInstance displayInstance2;
         using (var fileStream = new FileStream("DisplayInstance.dat", FileMode.Open, FileAccess.Read))
         {
            displayInstance2 = Serializer.Deserialize<DisplayInstance>(fileStream);
         }
         displayInstance2.Prefs = _prefs;
         displayInstance2.ProteinCollection = _proteinCollection;
         displayInstance2.BuildUnitInfoLogic();

         Assert.AreEqual("-verbosity 9", displayInstance2.Arguments);
         Assert.AreEqual("A3B4", displayInstance2.UserId);
         Assert.AreEqual(false, displayInstance2.UserIdUnknown);
         Assert.AreEqual(2, displayInstance2.MachineId);
         Assert.AreEqual("A3B4 (2)", displayInstance2.UserAndMachineId);
         Assert.AreEqual("harlam357", displayInstance2.FoldingID);
         Assert.AreEqual(32, displayInstance2.Team);
         Assert.AreEqual("harlam357 (32)", displayInstance2.FoldingIDAndTeam);
         Assert.AreEqual(ClientStatus.RunningAsync, displayInstance2.Status);
         Assert.AreEqual(0.04f, displayInstance2.Progress);
         Assert.AreEqual("ClientName", displayInstance2.Name);
         Assert.AreEqual("SMP", displayInstance2.ClientType);
         Assert.AreEqual(TimeSpan.FromMinutes(5), displayInstance2.TPF);
         Assert.AreEqual(5000, displayInstance2.PPD);
         Assert.AreEqual(3.5, displayInstance2.UPD);
         Assert.AreEqual(1000, displayInstance2.MHz);
         Assert.AreEqual(5, displayInstance2.PPD_MHz);
         Assert.AreEqual(TimeSpan.FromHours(8), displayInstance2.ETA);
         Assert.AreEqual("GROCVS", displayInstance2.Core);
         Assert.AreEqual("A2", displayInstance2.CoreID);
         Assert.AreEqual("P2669 (R1, C2, G3)", displayInstance2.ProjectRunCloneGen);
         Assert.AreEqual(300, displayInstance2.Credit);
         Assert.AreEqual(300, displayInstance2.Complete);
         Assert.AreEqual(2, displayInstance2.TotalRunFailedUnits);
         Assert.AreEqual("harlam357 (32)", displayInstance2.Username);
         Assert.AreEqual(new DateTime(2009, 12, 29, 23, 50, 0), displayInstance2.DownloadTime);
         Assert.AreEqual(new DateTime(2010, 1, 1, 23, 50, 0), displayInstance2.PreferredDeadline);
         Assert.AreSame(protein, displayInstance2.CurrentProtein);
         Assert.AreEqual(0, displayInstance2.CurrentLogLines[0].LineIndex);
         Assert.AreEqual(LogLineType.LogOpen, displayInstance2.CurrentLogLines[0].LineType);
         Assert.AreEqual("LogOpen", displayInstance2.CurrentLogLines[0].LineRaw);
         Assert.AreEqual(null, displayInstance2.CurrentLogLines[0].LineData);
         Assert.AreEqual(new DateTime(2010, 1, 1), displayInstance2.LastRetrievalTime);
         Assert.AreEqual(true, displayInstance2.ProductionValuesOk);
         Assert.AreEqual(InstanceType.PathInstance, displayInstance2.InstanceHostType);
         Assert.AreEqual("SomePath\\", displayInstance2.Path);
         Assert.AreEqual("SomePath\\ (-verbosity 9)", displayInstance2.ClientPathAndArguments);
         Assert.AreEqual(true, displayInstance2.ClientIsOnVirtualMachine);
         Assert.AreEqual(4, displayInstance2.FramesComplete);
         Assert.AreEqual(4, displayInstance2.PercentComplete);
         Assert.AreEqual(ClientType.SMP, displayInstance2.TypeOfClient);
         Assert.AreEqual("6.30", displayInstance2.ClientVersion);
         Assert.AreEqual("GROCVS", displayInstance2.CoreName);
         Assert.AreEqual("2.09", displayInstance2.CoreVersion);
         Assert.AreEqual(2669, displayInstance2.ProjectID);
         Assert.AreEqual(false, displayInstance2.UserIdIsDuplicate);
         Assert.AreEqual(false, displayInstance2.ProjectIsDuplicate);
         Assert.AreEqual(true, displayInstance2.UsernameOk);
         Assert.AreEqual(15, displayInstance2.TotalRunCompletedUnits);
         Assert.AreEqual(300, displayInstance2.TotalClientCompletedUnits);
         Assert.AreEqual("ClientName-FAHlog.txt", displayInstance2.CachedFahLogName);
         Assert.AreEqual(new DateTime(2010, 1, 1, 8, 0, 0), displayInstance2.EtaDate);
         
         _mocks.VerifyAll();
      }
   }
}
