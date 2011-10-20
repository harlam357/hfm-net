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
using System.Diagnostics;

using Castle.Core;
using Castle.Windsor;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceFactoryTests
   {
      private WindsorContainer _container;
   
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;
      
         _container = new WindsorContainer();
         _container.Kernel.AddComponent("clientInstance", typeof(ClientInstance), LifestyleType.Transient);
         _container.Kernel.AddComponentInstance("preferenceSet", typeof(IPreferenceSet), MockRepository.GenerateStub<IPreferenceSet>());
         _container.Kernel.AddComponentInstance("proteinCollection", typeof(IProteinCollection), SetupStubProteinCollection("GRO-A3", 100));
         _container.Kernel.AddComponentInstance("benchmarkContainer", typeof(IProteinBenchmarkContainer), MockRepository.GenerateStub<IProteinBenchmarkContainer>());
         _container.Kernel.AddComponentInstance("statusLogic", typeof(IStatusLogic), MockRepository.GenerateStub<IStatusLogic>());
         _container.Kernel.AddComponentInstance("dataRetriever", typeof(IDataRetriever), MockRepository.GenerateStub<IDataRetriever>());
         _container.Kernel.AddComponentInstance("dataAggregator", typeof(IDataAggregator), MockRepository.GenerateStub<IDataAggregator>());
         InstanceProvider.SetContainer(_container);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void HandleImportResultsArgumentNullTest()
      {
         var builder = new ClientInstanceFactory();
         builder.HandleImportResults(null);
      }

      [Test]
      public void HandleImportResultsTest()
      {
         var builder = new ClientInstanceFactory();
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
         var instances = builder.HandleImportResults(new[] { result1, result2, result3, result4 });
         Assert.AreEqual(2, instances.Count);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void CreateArgumentNullTest()
      {
         var builder = new ClientInstanceFactory();
         builder.Create(null);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreateNameEmptyTest()
      {
         var builder = new ClientInstanceFactory();
         builder.Create(new ClientInstanceSettings(InstanceType.PathInstance));
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreatePathEmptyTest()
      {
         var builder = new ClientInstanceFactory();
         builder.Create(new ClientInstanceSettings(InstanceType.PathInstance)
                        {
                           InstanceName = "Client 1"
                        });
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreateNameFailedCleanupTest()
      {
         var builder = new ClientInstanceFactory();
         var invalidChars = System.IO.Path.GetInvalidFileNameChars();
         builder.Create(new ClientInstanceSettings(InstanceType.PathInstance)
                        {
                           InstanceName = " " + invalidChars[0] + invalidChars[1] + " ",
                           Path = @"\\test\path\"
                        });
      }
      
      [Test]
      public void CreateTest()
      {
         var builder = new ClientInstanceFactory();
         var settings = new ClientInstanceSettings(InstanceType.PathInstance)
                        {
                           InstanceName = "Client{ 1",
                           Path = @"\\test\path\"
                        };
         var instance = builder.Create(settings);
         Assert.IsNotNull(instance);
         Assert.AreEqual("Client 1", instance.Settings.InstanceName);                       
      }

      private static IProteinCollection SetupStubProteinCollection(string core, int frames)
      {
         var currentProtein = new Protein { Core = core, Frames = frames };
         var proteinCollection = MockRepository.GenerateStub<IProteinCollection>();
         proteinCollection.Stub(x => x.GetProtein(0, true)).Return(currentProtein).IgnoreArguments();

         return proteinCollection;
      }
   }
}
