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
      private MockRepository _mocks;
      
      private IPreferenceSet _prefs;
      private IProteinCollection _proteinCollection;
      private IProteinBenchmarkContainer _benchmarkContainer;
      private IDataAggregator _dataAggregator;
      private IStatusLogic _statusLogic;
   
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;
         
         _mocks = new MockRepository();
         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         _proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         _benchmarkContainer = _mocks.DynamicMock<IProteinBenchmarkContainer>();
         _dataAggregator = _mocks.DynamicMock<IDataAggregator>();
         _statusLogic = _mocks.DynamicMock<IStatusLogic>();
      }

      [Test]
      public void ClientInstancePropertyTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100);
         IPreferenceSet prefs = SetupMockPreferenceSet("harlam357", 32);
         _mocks.ReplayAll();

         // Setup Test Instance
         var instance = new ClientInstance(prefs, proteinCollection, _benchmarkContainer, _statusLogic, _dataAggregator);
         Assert.AreEqual(InstanceType.PathInstance, instance.InstanceHostType);

         Assert.AreEqual(String.Empty, instance.ClientPathAndArguments);
         instance.Path = @"C:\ThePath\To\The\Files\";
         instance.Arguments = "-some -flags";
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", instance.Path, instance.Arguments),
                         instance.ClientPathAndArguments);
                         
         Assert.AreEqual(true, instance.UserIdUnknown);
         instance.UserId = "SOMEUSERID";
         Assert.AreEqual(false, instance.UserIdUnknown);
         
         Assert.AreEqual(0, instance.MachineId);
         instance.MachineId = 1;
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", instance.UserId, instance.MachineId), 
                         instance.UserAndMachineId);

         // True if the defaults are in place
         Assert.AreEqual(true, instance.IsUsernameOk());
         
         instance.FoldingID = "user";
         instance.Team = 3232;
         Assert.AreEqual(true, instance.IsUsernameOk());
         // Status must not be Unknown or Offline for function to evaluate false
         instance.Status = ClientStatus.RunningNoFrameTimes;
         Assert.AreEqual(false, instance.IsUsernameOk());
         
         instance.FoldingID = "harlam357";
         instance.Team = 32;
         Assert.AreEqual(true, instance.IsUsernameOk());
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", instance.FoldingID, instance.Team),
                         instance.FoldingIDAndTeam);
      }

      private IProteinCollection SetupMockProteinCollection(string core, int frames)
      {
         IProtein currentProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(currentProtein.Core).Return(core).Repeat.Any();
         Expect.Call(currentProtein.Frames).Return(frames).Repeat.Any();

         IProtein newProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Frames).Return(frames).Repeat.Any();

         IProteinCollection proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0)).Return(currentProtein).IgnoreArguments().Repeat.Any();
         Expect.Call(proteinCollection.CreateProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
      }

      private IPreferenceSet SetupMockPreferenceSet(string username, int team)
      {
         IPreferenceSet prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.GetPreference<string>(Preference.StanfordId)).Return(username).Repeat.Any();
         Expect.Call(prefs.GetPreference<int>(Preference.TeamId)).Return(team).Repeat.Any();
         Expect.Call(prefs.CacheDirectory).Return(String.Empty).Repeat.Any();
         return prefs;
      }
   }
}
