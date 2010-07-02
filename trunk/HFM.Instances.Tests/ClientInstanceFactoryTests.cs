/*
 * HFM.NET - Client Instance Factory Tests
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceFactoryTests
   {
      private WindsorContainer _container;
      private MockRepository _mocks;
      private IPreferenceSet _prefs;
      private IProteinCollection _proteinCollection;
      private IProteinBenchmarkContainer _benchmarkContainer;
      private IStatusLogic _statusLogic;
   
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;
      
         _container = new WindsorContainer();
         _mocks = new MockRepository();

         _container.Kernel.AddComponentInstance("dataAggregator", typeof(IDataAggregator), MockRepository.GenerateMock<IDataAggregator>());
         InstanceProvider.SetContainer(_container);

         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         _proteinCollection = SetupMockProteinCollection("GRO-A3", 100);
         _benchmarkContainer = _mocks.DynamicMock<IProteinBenchmarkContainer>();
         _statusLogic = _mocks.DynamicMock<IStatusLogic>();
      }
      
      [Test]
      public void HandleImportResultsArgumentNullTest()
      {
         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);

         try
         {
            builder.HandleImportResults(null);
            Assert.Fail();
         }
         catch (ArgumentNullException)
         { }
      }

      [Test]
      public void HandleImportResultsTest()
      {
         _mocks.ReplayAll();

         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);
         var result1 = new ClientInstanceSettings(InstanceType.PathInstance)
                       {
                          InstanceName = "Client 1",
                          Path = @"\\test\path1\"
                       };
         var result2 = new ClientInstanceSettings(InstanceType.HttpInstance)
                       {
                          InstanceName = "Client 2",
                          Path = @"\\test\path2\"
                       };
         var result3 = new ClientInstanceSettings { ImportError = "Import Error Message" };
         var result4 = new ClientInstanceSettings(InstanceType.FtpInstance);
         var resultList = new List<ClientInstanceSettings> { result1, result2, result3, result4 };
         var instances = builder.HandleImportResults(resultList);
         Assert.AreEqual(2, instances.Count);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void CreateArgumentNullTest()
      {
         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);

         try
         {
            builder.Create(null);
            Assert.Fail();
         }
         catch (ArgumentNullException)
         { }
      }
      
      [Test]
      public void CreateNameEmptyTest()
      {
         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);

         try
         {
            builder.Create(new ClientInstanceSettings(InstanceType.PathInstance));
            Assert.Fail();
         }
         catch (InvalidOperationException)
         { }
      }
      
      [Test]
      public void CreateNameFailedCleanupTest()
      {
         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);

         var invalidChars = System.IO.Path.GetInvalidFileNameChars();
         try
         {
            builder.Create(new ClientInstanceSettings(InstanceType.PathInstance)
                           {
                              InstanceName = " " + invalidChars[0] + invalidChars[1] + " "
                           });
            Assert.Fail();
         }
         catch (InvalidOperationException)
         { }
      }
      
      [Test]
      public void CreateTest()
      {
         _mocks.ReplayAll();

         var builder = new ClientInstanceFactory(_prefs, _proteinCollection, _benchmarkContainer, _statusLogic);
         var settings = new ClientInstanceSettings(InstanceType.PathInstance)
                        {
                           InstanceName = "Client{ 1",
                           Path = @"\\test\path\"
                        };
         var instance = builder.Create(settings);
         Assert.IsNotNull(instance);
         Assert.AreEqual("Client 1", instance.Settings.InstanceName);                       
      }

      private IProteinCollection SetupMockProteinCollection(string core, int frames)
      {
         var currentProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(currentProtein.Core).Return(core).Repeat.Any();
         Expect.Call(currentProtein.Frames).Return(frames).Repeat.Any();

         var newProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Frames).Return(frames).Repeat.Any();

         var proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0)).Return(currentProtein).IgnoreArguments().Repeat.Any();
         Expect.Call(proteinCollection.CreateProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
      }
   }
}
