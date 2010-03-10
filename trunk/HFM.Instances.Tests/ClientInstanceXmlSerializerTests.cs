/*
 * HFM.NET - Client Instance Xml Serializer Tests
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
using System.IO;

using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ClientInstanceXmlSerializerTests
   {
      private IWindsorContainer container;
      private MockRepository mocks;
      private IPreferenceSet _Prefs;
      private IProteinCollection _ProteinCollection;
      private IProteinBenchmarkContainer _BenchmarkContainer;
      private IDataAggregator _DataAggregator;
   
      [SetUp]
      public void Init()
      {
         container = new WindsorContainer();
         mocks = new MockRepository();
         
         _Prefs = mocks.DynamicMock<IPreferenceSet>();
         _ProteinCollection = SetupMockProteinCollection();
         _BenchmarkContainer = mocks.DynamicMock<IProteinBenchmarkContainer>();
         _DataAggregator = mocks.DynamicMock<IDataAggregator>();
         
         mocks.ReplayAll();

         container.Kernel.AddComponentInstance<IDataAggregator>(typeof(IDataAggregator), _DataAggregator);
         InstanceProvider.SetContainer(container);
      }

      [Test]
      public void SerializationTest()
      {
         IClientInstanceSettings instance1 = new ClientInstance(_Prefs, _ProteinCollection, _BenchmarkContainer);
         instance1.InstanceHostType = InstanceType.PathInstance;
         instance1.InstanceName = "Test1";
         instance1.ClientProcessorMegahertz = 1000;
         instance1.RemoteFAHLogFilename = "FAHlog.txt";
         instance1.RemoteUnitInfoFilename = "unitinfo.txt";
         instance1.RemoteQueueFilename = "queue.dat";
         instance1.Path = @"C:\Test\Path\One\";
         instance1.Server = "ServerName";
         instance1.Username = "username";
         instance1.Password = "password";
         instance1.FtpMode = FtpType.Passive;
         instance1.ClientIsOnVirtualMachine = false;
         instance1.ClientTimeOffset = 60;

         IClientInstanceSettings instance2 = new ClientInstance(_Prefs, _ProteinCollection, _BenchmarkContainer);
         instance2.InstanceHostType = InstanceType.FTPInstance;
         instance2.InstanceName = "Test2";
         instance2.ClientProcessorMegahertz = 3800;
         instance2.RemoteFAHLogFilename = "FAHlog-Test2.txt";
         instance2.RemoteUnitInfoFilename = "unitinfo-Test2.txt";
         instance2.RemoteQueueFilename = "queue-Test2.dat";
         instance2.Path = "/root/folder1/folder2/";
         instance2.Server = "some.server.com";
         instance2.Username = "user1";
         instance2.Password = "pass2";
         instance2.FtpMode = FtpType.Active;
         instance2.ClientIsOnVirtualMachine = true;
         instance2.ClientTimeOffset = -180;
         
         ClientInstanceXmlSerializer serializer = new ClientInstanceXmlSerializer(
            new ClientInstanceFactory(_Prefs, _ProteinCollection, _BenchmarkContainer));
         
         ICollection<ClientInstance> collection1 = new List<ClientInstance>();
         collection1.Add((ClientInstance)instance1);
         collection1.Add((ClientInstance)instance2);
         
         serializer.Serialize(Path.Combine(Environment.CurrentDirectory, "Test.xml"), collection1);

         IList<ClientInstance> collection2 = serializer.Deserialize("Test.xml");

         IClientInstanceSettings instance3 = collection2[0];
         Assert.AreEqual("Test1", instance3.InstanceName);
         Assert.AreEqual(1000, instance3.ClientProcessorMegahertz);
         Assert.AreEqual("FAHlog.txt", instance3.RemoteFAHLogFilename);
         Assert.AreEqual("unitinfo.txt", instance3.RemoteUnitInfoFilename);
         Assert.AreEqual("queue.dat", instance3.RemoteQueueFilename);
         Assert.AreEqual(InstanceType.PathInstance, instance3.InstanceHostType);
         Assert.AreEqual(@"C:\Test\Path\One\", instance3.Path);
         Assert.AreEqual("ServerName", instance3.Server);
         Assert.AreEqual("username", instance3.Username);
         Assert.AreEqual("password", instance3.Password);
         Assert.AreEqual(FtpType.Passive, instance3.FtpMode);
         Assert.AreEqual(false, instance3.ClientIsOnVirtualMachine);
         Assert.AreEqual(60, instance3.ClientTimeOffset);

         IClientInstanceSettings instance4 = collection2[1];
         Assert.AreEqual("Test2", instance4.InstanceName);
         Assert.AreEqual(3800, instance4.ClientProcessorMegahertz);
         Assert.AreEqual("FAHlog-Test2.txt", instance4.RemoteFAHLogFilename);
         Assert.AreEqual("unitinfo-Test2.txt", instance4.RemoteUnitInfoFilename);
         Assert.AreEqual("queue-Test2.dat", instance4.RemoteQueueFilename);
         Assert.AreEqual(InstanceType.FTPInstance, instance4.InstanceHostType);
         Assert.AreEqual("/root/folder1/folder2/", instance4.Path);
         Assert.AreEqual("some.server.com", instance4.Server);
         Assert.AreEqual("user1", instance4.Username);
         Assert.AreEqual("pass2", instance4.Password);
         Assert.AreEqual(FtpType.Active, instance4.FtpMode);
         Assert.AreEqual(true, instance4.ClientIsOnVirtualMachine);
         Assert.AreEqual(-180, instance4.ClientTimeOffset);
         
         mocks.VerifyAll();
      }

      private IProteinCollection SetupMockProteinCollection()
      {
         IProtein newProtein = mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Core).Return(String.Empty).Repeat.Any();

         IProteinCollection proteinCollection = mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0)).Return(newProtein).IgnoreArguments().Repeat.Any();
         Expect.Call(proteinCollection.GetNewProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
      }
   }
}
