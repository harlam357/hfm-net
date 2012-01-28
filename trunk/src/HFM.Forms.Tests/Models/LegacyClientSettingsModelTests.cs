/*
 * HFM.NET - Legacy Client Settings Model Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.IO;

using NUnit.Framework;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Tests.Models
{
   [TestFixture]
   public class LegacyClientSettingsModelTests
   {
      [Test]
      public void DefaultValueTest()
      {
         var model = new LegacyClientSettingsModel();
         Assert.AreEqual(LegacyClientSubType.Path, model.LegacyClientSubType);
         Assert.AreEqual(1, model.ClientProcessorMegahertz);
         Assert.AreEqual(Default.FahLogFileName, model.FahLogFileName);
         Assert.AreEqual(Default.UnitInfoFileName, model.UnitInfoFileName);
         Assert.AreEqual(Default.QueueFileName, model.QueueFileName);
         Assert.AreEqual(String.Empty, model.Server);
         Assert.AreEqual(Default.FtpPort, model.Port);
         Assert.AreEqual(String.Empty, model.Username);
         Assert.AreEqual(String.Empty, model.Password);
         Assert.AreEqual(FtpType.Passive, model.FtpMode);
         Assert.AreEqual(false, model.UtcOffsetIsZero);
         Assert.AreEqual(0, model.ClientTimeOffset);
         Assert.AreEqual(String.Empty, model.Dummy);
      }

      [Test]
      public void LegacyClientSubTypeTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.AreEqual(LegacyClientSubType.Path, model.LegacyClientSubType);
         model.LegacyClientSubType = LegacyClientSubType.Http;
         Assert.AreEqual(LegacyClientSubType.Http, model.LegacyClientSubType);
         model.LegacyClientSubType = LegacyClientSubType.None;
         Assert.AreEqual(LegacyClientSubType.Http, model.LegacyClientSubType);
      }

      [Test]
      public void LegacyClientSubTypeTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.AreEqual(LegacyClientSubType.Ftp, model.LegacyClientSubType);
         Assert.AreEqual("/path/", model.Path);
         Assert.AreEqual("123.123.123.123", model.Server);
         Assert.AreEqual(45000, model.Port);
         Assert.AreEqual("ftpuser", model.Username);
         Assert.AreEqual("ftppass", model.Password);
         Assert.AreEqual(FtpType.Active, model.FtpMode);

         model.LegacyClientSubType = LegacyClientSubType.Path;
         Assert.AreEqual(LegacyClientSubType.Path, model.LegacyClientSubType);
         Assert.AreEqual(String.Empty, model.Path);
         Assert.AreEqual(String.Empty, model.Server);
         Assert.AreEqual(Default.FtpPort, model.Port);
         Assert.AreEqual(String.Empty, model.Username);
         Assert.AreEqual(String.Empty, model.Password);
         Assert.AreEqual(FtpType.Passive, model.FtpMode);
      }

      [Test]
      public void LegacyClientSubTypeTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         Assert.AreEqual(LegacyClientSubType.Http, model.LegacyClientSubType);

         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "Dummy");
         model.LegacyClientSubType = LegacyClientSubType.Path;
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.LegacyClientSubType = LegacyClientSubType.Path;
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void NameTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.NameEmpty);
         Assert.IsFalse(model.NameError);
         Assert.IsFalse(model.Error);

         model.Name = String.Empty;
         Assert.AreEqual(String.Empty, model.Name);
         Assert.IsTrue(model.NameEmpty);
         Assert.IsTrue(model.NameError);
         Assert.IsTrue(model.Error);

         model.Name = null;
         Assert.AreEqual(String.Empty, model.Name);
         Assert.IsTrue(model.NameEmpty);
         Assert.IsTrue(model.NameError);
         Assert.IsTrue(model.Error);

         model.Name = "   ";
         Assert.AreEqual(String.Empty, model.Name);
         Assert.IsTrue(model.NameEmpty);
         Assert.IsTrue(model.NameError);
         Assert.IsTrue(model.Error);

         model.Name = "client*name";
         Assert.AreEqual("client*name", model.Name);
         Assert.IsFalse(model.NameEmpty);
         Assert.IsTrue(model.NameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void NameTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "Name");
         model.Name = "Some other name";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false; 
         model.Name = "Some other name";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void ClientProcessorMegahertzTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.ClientProcessorMegahertzError);
         Assert.IsFalse(model.Error);

         model.ClientProcessorMegahertz = 0;
         Assert.IsTrue(model.ClientProcessorMegahertzError);
         Assert.IsTrue(model.Error);

         model.ClientProcessorMegahertz = -1;
         Assert.IsTrue(model.ClientProcessorMegahertzError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void ClientProcessorMegahertzTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "ClientProcessorMegahertz");
         model.ClientProcessorMegahertz = 1000;
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.ClientProcessorMegahertz = 1000;
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void FahLogFileNameTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.FahLogFileNameError);
         Assert.IsFalse(model.Error);

         model.FahLogFileName = String.Empty;
         Assert.AreEqual(String.Empty, model.FahLogFileName);
         Assert.IsTrue(model.FahLogFileNameError);
         Assert.IsTrue(model.Error);

         model.FahLogFileName = null;
         Assert.AreEqual(String.Empty, model.FahLogFileName);
         Assert.IsTrue(model.FahLogFileNameError);
         Assert.IsTrue(model.Error);

         model.FahLogFileName = "  ";
         Assert.AreEqual(String.Empty, model.FahLogFileName);
         Assert.IsTrue(model.FahLogFileNameError);
         Assert.IsTrue(model.Error);

         model.FahLogFileName = "FAHl*g.txt";
         Assert.AreEqual("FAHl*g.txt", model.FahLogFileName);
         Assert.IsTrue(model.FahLogFileNameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void FahLogFileNameTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "FahLogFileName");
         model.FahLogFileName = "log file name.txt";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.FahLogFileName = "log file name.txt";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void UnitInfoFileNameTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.UnitInfoFileNameError);
         Assert.IsFalse(model.Error);

         model.UnitInfoFileName = String.Empty;
         Assert.AreEqual(String.Empty, model.UnitInfoFileName);
         Assert.IsTrue(model.UnitInfoFileNameError);
         Assert.IsTrue(model.Error);

         model.UnitInfoFileName = null;
         Assert.AreEqual(String.Empty, model.UnitInfoFileName);
         Assert.IsTrue(model.UnitInfoFileNameError);
         Assert.IsTrue(model.Error);

         model.UnitInfoFileName = "  ";
         Assert.AreEqual(String.Empty, model.UnitInfoFileName);
         Assert.IsTrue(model.UnitInfoFileNameError);
         Assert.IsTrue(model.Error);

         model.UnitInfoFileName = "unitin:o.txt";
         Assert.AreEqual("unitin:o.txt", model.UnitInfoFileName);
         Assert.IsTrue(model.UnitInfoFileNameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void UnitInfoFileNameTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "UnitInfoFileName");
         model.UnitInfoFileName = "log file name.txt";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.UnitInfoFileName = "log file name.txt";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void QueueFileNameTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.QueueFileNameError);
         Assert.IsFalse(model.Error);

         model.QueueFileName = String.Empty;
         Assert.AreEqual(String.Empty, model.QueueFileName);
         Assert.IsTrue(model.QueueFileNameError);
         Assert.IsTrue(model.Error);

         model.QueueFileName = null;
         Assert.AreEqual(String.Empty, model.QueueFileName);
         Assert.IsTrue(model.QueueFileNameError);
         Assert.IsTrue(model.Error);

         model.QueueFileName = "  ";
         Assert.AreEqual(String.Empty, model.QueueFileName);
         Assert.IsTrue(model.QueueFileNameError);
         Assert.IsTrue(model.Error);

         model.QueueFileName = "que?e.dat";
         Assert.AreEqual("que?e.dat", model.QueueFileName);
         Assert.IsTrue(model.QueueFileNameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void QueueFileNameTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "QueueFileName");
         model.QueueFileName = "queue file name.dat";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.QueueFileName = "queue file name.dat";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void PathTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.PathEmpty);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = String.Empty;
         Assert.AreEqual(String.Empty, model.Path);
         Assert.IsTrue(model.PathEmpty);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);

         model.Path = null;
         Assert.AreEqual(String.Empty, model.Path);
         Assert.IsTrue(model.PathEmpty);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);

         model.Path = "  ";
         Assert.AreEqual(String.Empty, model.Path);
         Assert.IsTrue(model.PathEmpty);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PathTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = "bad)path";
         Assert.AreEqual("bad)path\\", model.Path);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PathTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = "a";
         Assert.AreEqual("a", model.Path);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PathTest4()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = "htp:/badurl";
         Assert.AreEqual("htp:/badurl", model.Path);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PathTest5()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = "/";
         Assert.AreEqual("/", model.Path);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);
      }

      [Test]
      public void PathTest6()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.PathError);
         Assert.IsFalse(model.Error);

         model.Path = "\\badpath\\";
         Assert.AreEqual("\\badpath\\", model.Path);
         Assert.IsTrue(model.PathError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PathTest7()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "Path");
         model.Path = "C:\\newpath\\";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.Path = "C:\\newpath\\";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void PathTest8()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         string existingPath = model.Path;

         model.Path = Path.Combine(existingPath, "FAHlog.txt");
         Assert.IsTrue(Paths.Equal(existingPath, model.Path));

         model.Path = Path.Combine(existingPath, "unitinfo.txt");
         Assert.IsTrue(Paths.Equal(existingPath, model.Path));

         model.Path = Path.Combine(existingPath, "queue.dat");
         Assert.IsTrue(Paths.Equal(existingPath, model.Path));
      }

      [Test]
      public void ServerTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.ServerError);
         Assert.IsFalse(model.Error);

         model.Server = String.Empty;
         Assert.AreEqual(String.Empty, model.Server);
         Assert.IsTrue(model.ServerError);
         Assert.IsTrue(model.Error);

         model.Server = null;
         Assert.AreEqual(String.Empty, model.Server);
         Assert.IsTrue(model.ServerError);
         Assert.IsTrue(model.Error);

         model.Server = "  ";
         Assert.AreEqual(String.Empty, model.Server);
         Assert.IsTrue(model.ServerError);
         Assert.IsTrue(model.Error);

         model.Server = "ser^er.name";
         Assert.AreEqual("ser^er.name", model.Server);
         Assert.IsTrue(model.ServerError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void ServerTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.ServerError);
         Assert.IsFalse(model.Error);

         model = CreateValidModel(LegacyClientSubType.Http);
         Assert.IsFalse(model.ServerError);
         Assert.IsFalse(model.Error);
      }

      [Test]
      public void ServerTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "Server");
         model.Server = "new.server.name";
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.Server = "new.server.name";
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void PortTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.PortError);
         Assert.IsFalse(model.Error);

         model.Port = 0;
         Assert.AreEqual(0, model.Port);
         Assert.IsTrue(model.PortError);
         Assert.IsTrue(model.Error);

         model.Port = -1;
         Assert.AreEqual(-1, model.Port);
         Assert.IsTrue(model.PortError);
         Assert.IsTrue(model.Error);

         model.Port = UInt16.MaxValue;
         Assert.AreEqual(UInt16.MaxValue, model.Port);
         Assert.IsTrue(model.PortError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PortTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.PortError);
         Assert.IsFalse(model.Error);

         model = CreateValidModel(LegacyClientSubType.Http);
         Assert.IsFalse(model.PortError);
         Assert.IsFalse(model.Error);
      }

      [Test]
      public void PortTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         bool propertyChangedFired = false;
         model.PropertyChanged += (sender, e) => propertyChangedFired = (e.PropertyName == "Port");
         model.Port = 357;
         Assert.IsTrue(propertyChangedFired);

         propertyChangedFired = false;
         model.Port = 357;
         Assert.IsFalse(propertyChangedFired);
      }

      [Test]
      public void UsernameTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         Assert.IsFalse(model.CredentialsError);
         Assert.IsFalse(model.CredentialsErrorMessage.Length != 0);
         Assert.IsFalse(model.UsernameError);
         Assert.IsFalse(model.Error);

         model.Username = String.Empty;
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);

         model.Username = null;
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);

         model.Username = "   ";
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void UsernameTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.CredentialsError);
         Assert.IsFalse(model.CredentialsErrorMessage.Length != 0);
         Assert.IsFalse(model.UsernameError);
         Assert.IsFalse(model.Error);

         model.Username = String.Empty;
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);

         model.Username = null;
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);

         model.Username = "   ";
         Assert.AreEqual(String.Empty, model.Username);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.UsernameError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void UsernameTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         bool passwordPropertyChangedFired = false;
         bool usernamePropertyChangedFired = false;
         model.PropertyChanged += (sender, e) =>
                                  {
                                     if (e.PropertyName == "Password") passwordPropertyChangedFired = true;
                                     if (e.PropertyName == "Username") usernamePropertyChangedFired = true;
                                  };
         model.Username = "new user";
         Assert.IsTrue(passwordPropertyChangedFired);
         Assert.IsTrue(usernamePropertyChangedFired);

         passwordPropertyChangedFired = false;
         usernamePropertyChangedFired = false;
         model.Username = "new user";
         Assert.IsFalse(passwordPropertyChangedFired);
         Assert.IsFalse(usernamePropertyChangedFired);
      }

      [Test]
      public void UsernameTest4()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         bool passwordPropertyChangedFired = false;
         bool usernamePropertyChangedFired = false;
         model.PropertyChanged += (sender, e) =>
                                  {
                                     if (e.PropertyName == "Password") passwordPropertyChangedFired = true;
                                     if (e.PropertyName == "Username") usernamePropertyChangedFired = true;
                                  };
         model.Username = "new user";
         Assert.IsTrue(passwordPropertyChangedFired);
         Assert.IsTrue(usernamePropertyChangedFired);

         passwordPropertyChangedFired = false;
         usernamePropertyChangedFired = false;
         model.Username = "new user";
         Assert.IsFalse(passwordPropertyChangedFired);
         Assert.IsFalse(usernamePropertyChangedFired);
      }

      [Test]
      public void PasswordTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         Assert.IsFalse(model.CredentialsError);
         Assert.IsFalse(model.CredentialsErrorMessage.Length != 0);
         Assert.IsFalse(model.PasswordError);
         Assert.IsFalse(model.Error);

         model.Password = String.Empty;
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);

         model.Password = null;
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);

         model.Password = "   ";
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PasswordTest2()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         Assert.IsFalse(model.CredentialsError);
         Assert.IsFalse(model.CredentialsErrorMessage.Length != 0);
         Assert.IsFalse(model.PasswordError);
         Assert.IsFalse(model.Error);

         model.Password = String.Empty;
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);

         model.Password = null;
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);

         model.Password = "   ";
         Assert.AreEqual(String.Empty, model.Password);
         Assert.IsTrue(model.CredentialsError);
         Assert.IsTrue(model.CredentialsErrorMessage.Length != 0);
         Assert.IsTrue(model.PasswordError);
         Assert.IsTrue(model.Error);
      }

      [Test]
      public void PasswordTest3()
      {
         var model = CreateValidModel(LegacyClientSubType.Http);
         bool passwordPropertyChangedFired = false;
         bool usernamePropertyChangedFired = false;
         model.PropertyChanged += (sender, e) =>
         {
            if (e.PropertyName == "Username") usernamePropertyChangedFired = true;
            if (e.PropertyName == "Password") passwordPropertyChangedFired = true;
         };
         model.Password = "new pass";
         Assert.IsTrue(passwordPropertyChangedFired);
         Assert.IsTrue(usernamePropertyChangedFired);

         passwordPropertyChangedFired = false;
         usernamePropertyChangedFired = false;
         model.Password = "new pass";
         Assert.IsFalse(passwordPropertyChangedFired);
         Assert.IsFalse(usernamePropertyChangedFired);
      }

      [Test]
      public void PasswordTest4()
      {
         var model = CreateValidModel(LegacyClientSubType.Ftp);
         bool passwordPropertyChangedFired = false;
         bool usernamePropertyChangedFired = false;
         model.PropertyChanged += (sender, e) =>
         {
            if (e.PropertyName == "Username") usernamePropertyChangedFired = true;
            if (e.PropertyName == "Password") passwordPropertyChangedFired = true;
         };
         model.Password = "new pass";
         Assert.IsTrue(passwordPropertyChangedFired);
         Assert.IsTrue(usernamePropertyChangedFired);

         passwordPropertyChangedFired = false;
         usernamePropertyChangedFired = false;
         model.Password = "new pass";
         Assert.IsFalse(passwordPropertyChangedFired);
         Assert.IsFalse(usernamePropertyChangedFired);
      }

      [Test]
      public void ClientTimeOffsetTest1()
      {
         var model = CreateValidModel(LegacyClientSubType.Path);
         Assert.IsFalse(model.ClientTimeOffsetError);

         model.ClientTimeOffset = -721;
         Assert.IsTrue(model.ClientTimeOffsetError);

         model.ClientTimeOffset = 721;
         Assert.IsTrue(model.ClientTimeOffsetError);
      }

      private static LegacyClientSettingsModel CreateValidModel(LegacyClientSubType type)
      {
         var model = new LegacyClientSettingsModel();
         model.LegacyClientSubType = type;
         model.Name = "Test";

         switch (type)
         {
            case LegacyClientSubType.Path:
               model.Path = "C:\\Folding";
               break;
            case LegacyClientSubType.Http:
               model.Path = "http://www.folding.com/";
               model.Username = "user";
               model.Password = "pass";
               break;
            case LegacyClientSubType.Ftp:
               model.Path = "/path";
               model.Server = "123.123.123.123";
               model.Port = 45000;
               model.Username = "ftpuser";
               model.Password = "ftppass";
               model.FtpMode = FtpType.Active;
               break;
         }

         Assert.IsFalse(model.Error);

         return model;
      }
   }
}
