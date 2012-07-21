/*
 * HFM.NET - Client Factory Tests
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
using System.Linq;

using Castle.MicroKernel.Registration;
using Castle.Windsor;

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ClientInstanceFactoryTests
   {
      private WindsorContainer _container;
   
      [SetUp]
      public void Init()
      {
         _container = new WindsorContainer();
         _container.Kernel.Register(Component.For(typeof(LegacyClient)).LifeStyle.Transient);
         //_container.Kernel.Register(Component.For(typeof(FahClient)).LifeStyle.Transient);
         ServiceLocator.SetContainer(_container);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void CreateCollectionArgumentNullTest()
      {
         var builder = new ClientFactory();
         builder.CreateCollection((null));
      }

      [Test]
      public void CreateCollectionTest()
      {
         var builder = new ClientFactory();
         var result1 = new ClientSettings(ClientType.Legacy)
                       {
                          LegacyClientSubType = LegacyClientSubType.Path,
                          Name = "Client 1",
                          Path = @"\\test\path1\"
                       };
         var result2 = new ClientSettings(ClientType.Legacy)
                       {
                          LegacyClientSubType = LegacyClientSubType.Http,
                          Name = "Client 2",
                          Path = @"\\test\path2\"
                       };
         var result3 = new ClientSettings(ClientType.Legacy)
                       {
                          LegacyClientSubType = LegacyClientSubType.Ftp
                       };
         var instances = builder.CreateCollection(new[] { result1, result2, result3 });
         Assert.AreEqual(2, instances.Count());
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void CreateArgumentNullTest()
      {
         var builder = new ClientFactory();
         builder.Create(null);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreateNameEmptyTest()
      {
         var builder = new ClientFactory();
         builder.Create(new ClientSettings(ClientType.Legacy));
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreatePathEmptyTest()
      {
         var builder = new ClientFactory();
         builder.Create(new ClientSettings(ClientType.Legacy)
                        {
                           Name = "Client 1"
                        });
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void CreateNameFailedCleanupTest()
      {
         var builder = new ClientFactory();
         var invalidChars = System.IO.Path.GetInvalidFileNameChars();
         builder.Create(new ClientSettings(ClientType.Legacy)
                        {
                           Name = " " + invalidChars[0] + invalidChars[1] + " ",
                           Path = @"\\test\path\"
                        });
      }
      
      [Test]
      public void CreateTest()
      {
         var builder = new ClientFactory();
         var settings = new ClientSettings(ClientType.Legacy)
                        {
                           Name = "Client{ 1",
                           Path = @"\\test\path\"
                        };
         var instance = builder.Create(settings);
         Assert.IsNotNull(instance);
         Assert.AreEqual("Client 1", instance.Settings.Name);                       
      }
   }
}
