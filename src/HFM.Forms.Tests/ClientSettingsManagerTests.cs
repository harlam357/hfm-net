/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.IO;

using NUnit.Framework;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
   [TestFixture]
   public class ClientSettingsManagerTests
   {
      [Test]
      public void ClientSettingsManager_Defaults_Test()
      {
         var configurationManager = new ClientSettingsManager();
         Assert.AreEqual(String.Empty, configurationManager.FileName);
         Assert.AreEqual(1, configurationManager.FilterIndex);
         Assert.AreEqual("hfmx", configurationManager.FileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfmx", configurationManager.FileTypeFilters);
      }
      
      [Test]
      public void ClientSettingsManager_Read_Test()
      {
         var configurationManager = new ClientSettingsManager();
         configurationManager.Read("..\\..\\TestFiles\\TestClientSettings.hfmx", 1);
         Assert.AreEqual("..\\..\\TestFiles\\TestClientSettings.hfmx", configurationManager.FileName);
         Assert.AreEqual(1, configurationManager.FilterIndex);
         Assert.AreEqual(".hfmx", configurationManager.FileExtension);
      }
      
      [Test]
      public void ClientSettingsManager_Write_Test()
      {
         var instance1 = new LegacyClient();
         instance1.Settings = new ClientSettings(ClientType.Legacy) { Name = "test" };

         const string testFile = "..\\..\\TestFiles\\new.ext";

         var configurationManager = new ClientSettingsManager();
         configurationManager.Write(new[] { instance1.Settings }, testFile);
         Assert.AreEqual("..\\..\\TestFiles\\new.ext", configurationManager.FileName);
         Assert.AreEqual(1, configurationManager.FilterIndex);
         Assert.AreEqual(".ext", configurationManager.FileExtension);

         File.Delete(testFile);
      }
   }
}
