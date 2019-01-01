/*
 * HFM.NET - Client Factory Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   [TestFixture]
   public class ClientFactoryTests
   {
      [Test]
      public void CreateCollectionArgumentNullTest()
      {
         var builder = new ClientFactory();
         Assert.Throws<ArgumentNullException>(() => builder.CreateCollection((null)));
      }

      [Test]
      public void CreateCollectionTest()
      {
         var legacyClientFactory = MockRepository.GenerateStub<ILegacyClientFactory>();
         legacyClientFactory.Stub(x => x.Create()).Return(new LegacyClient());
         var builder = new ClientFactory { LegacyClientFactory = legacyClientFactory };
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
      public void CreateArgumentNullTest()
      {
         var builder = new ClientFactory();
         Assert.Throws<ArgumentNullException>(() => builder.Create(null));
      }

      [Test]
      public void CreateNameEmptyTest()
      {
         var builder = new ClientFactory();
         Assert.Throws<ArgumentException>(() => builder.Create(new ClientSettings(ClientType.Legacy)));
      }

      [Test]
      public void CreatePathEmptyTest()
      {
         var builder = new ClientFactory();
         Assert.Throws<ArgumentException>(() => builder.Create(new ClientSettings(ClientType.Legacy) { Name = "Client 1" }));
      }

      [Test]
      public void CreateNameFailedCleanupTest()
      {
         var builder = new ClientFactory();
         var invalidChars = System.IO.Path.GetInvalidFileNameChars();
         Assert.Throws<ArgumentException>(() => builder.Create(new ClientSettings(ClientType.Legacy)
                                                               {
                                                                  Name = " " + invalidChars[0] + invalidChars[1] + " ",
                                                                  Path = @"\\test\path\"
                                                               }));
      }

      [Test]
      public void CreateTest()
      {
         var legacyClientFactory = MockRepository.GenerateStub<ILegacyClientFactory>();
         legacyClientFactory.Stub(x => x.Create()).Return(new LegacyClient());
         var builder = new ClientFactory { LegacyClientFactory = legacyClientFactory };
         var settings = new ClientSettings(ClientType.Legacy)
                        {
                           Name = "Client{ 1",
                           Path = @"\\test\path\"
                        };
         var instance = builder.Create(settings);
         Assert.IsNotNull(instance);
         Assert.AreEqual("Client 1", instance.Settings.Name);
      }

      [Test]
      public void Create_FahClientFactoryNull_Test()
      {
         var builder = new ClientFactory();
         var settings = new ClientSettings(ClientType.FahClient)
         {
            Name = "FahClient",
            Server = "192.168.100.200"
         };
         var instance = builder.Create(settings);
         Assert.IsNull(instance);
      }

      [Test]
      public void Create_LegacyClientFactoryNull_Test()
      {
         var builder = new ClientFactory();
         var settings = new ClientSettings(ClientType.Legacy)
         {
            Name = "LegacyClient",
            Path = @"\\test\path\"
         };
         var instance = builder.Create(settings);
         Assert.IsNull(instance);
      }
   }
}
