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
using System.Linq;
using NUnit.Framework;

using HFM.Core;
using HFM.Core.Client;
using HFM.Core.DataTypes;

namespace HFM.Forms
{
    [TestFixture]
    public class ClientSettingsManagerTests
    {
        [Test]
        public void ClientSettingsManager_VerifyDefaultState()
        {
            // Act
            var manager = new ClientSettingsManager();
            // Assert
            Assert.AreEqual(String.Empty, manager.FileName);
            Assert.AreEqual(1, manager.FilterIndex);
            Assert.AreEqual("hfmx", manager.FileExtension);
            Assert.AreEqual("HFM Configuration Files|*.hfmx", manager.FileTypeFilters);
        }

        [Test]
        public void ClientSettingsManager_Read_ReturnsClientSettingsCollectionAndSetsManagerState()
        {
            // Arrange
            var manager = new ClientSettingsManager();
            // Act
            var settings = manager.Read("..\\..\\TestFiles\\TestClientSettings.hfmx", 1);
            // Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.Count());
            Assert.AreEqual("..\\..\\TestFiles\\TestClientSettings.hfmx", manager.FileName);
            Assert.AreEqual(1, manager.FilterIndex);
            Assert.AreEqual(".hfmx", manager.FileExtension);
        }

        [Test]
        public void ClientSettingsManager_Write_WritesTheClientSettingsToDisk()
        {
            // Arrange
            const string testFile = "..\\..\\TestFiles\\new.ext";
            
            var client = new FahClient();
            client.Settings = new ClientSettings { Name = "test" };
            // TODO: Implement ArtifactFolder
            var manager = new ClientSettingsManager();
            // Act
            try
            {
                manager.Write(new[] { client.Settings }, testFile, 1);
                // Assert
                Assert.AreEqual("..\\..\\TestFiles\\new.ext", manager.FileName);
                Assert.AreEqual(1, manager.FilterIndex);
                Assert.AreEqual(".ext", manager.FileExtension);
            }
            finally
            {
                try
                {
                    File.Delete(testFile);
                }
                catch (Exception)
                {
                    // do nothing
                }
            }
        }
    }
}
