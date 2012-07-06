/*
 * HFM.NET - Client Settings Manager Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Core
{
   public interface IClientSettingsManager
   {
      /// <summary>
      /// Client Configuration Filename
      /// </summary>
      string FileName { get; }

      /// <summary>
      /// Current Plugin Index
      /// </summary>
      int FilterIndex { get; }

      /// <summary>
      /// Number of Settings Plugins
      /// </summary>
      int PluginCount { get; }

      /// <summary>
      /// Current Config File Extension or the Default File Extension
      /// </summary>
      string FileExtension { get; }

      string FileTypeFilters { get; }

      /// <summary>
      /// Clear the Configuration Filename
      /// </summary>
      void ClearFileName();

      /// <summary>
      /// Reads a collection of client settings from a file.
      /// </summary>
      /// <param name="filePath">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      IEnumerable<ClientSettings> Read(string filePath, int filterIndex);

      /// <summary>
      /// Writes a collection of client settings to a file.
      /// </summary>
      /// <param name="settingsCollection">Settings collection.</param>
      /// <param name="filePath">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      void Write(IEnumerable<ClientSettings> settingsCollection, string filePath, int filterIndex);
   }

   public sealed class ClientSettingsManager : IClientSettingsManager
   {
      private readonly IFileSerializerPluginManager<List<ClientSettings>> _settingsPlugins;
      
      /// <summary>
      /// Current File Name
      /// </summary>
      public string FileName { get; private set; }

      private int _filterIndex;
      /// <summary>
      /// Current Filter Index
      /// </summary>
      public int FilterIndex
      {
         get
         {
            if (_filterIndex < 1 || _filterIndex > PluginCount)
            {
               return 1;
            }
            return _filterIndex;
         }
         private set { _filterIndex = value; }
      }

      /// <summary>
      /// Number of Plugins
      /// </summary>
      public int PluginCount
      {
         get { return _settingsPlugins.Count(); }
      }

      /// <summary>
      /// Current Config File Extension or the Default File Extension
      /// </summary>
      public string FileExtension
      {
         get
         {
            if (!String.IsNullOrEmpty(FileName))
            {
               return Path.GetExtension(FileName);
            }

            Debug.Assert(_settingsPlugins.Keys.Count() != 0);
            return _settingsPlugins.GetPlugin(_settingsPlugins.Keys.First()).Interface.FileExtension;
         }
      }

      public string FileTypeFilters
      {
         get { return _settingsPlugins.FileTypeFilters; }
      }

      public ClientSettingsManager(IFileSerializerPluginManager<List<ClientSettings>> settingsPlugins)
      {
         _settingsPlugins = settingsPlugins;

         ClearFileName();
      }

      /// <summary>
      /// Clear the Configuration Filename
      /// </summary>
      public void ClearFileName()
      {
         FileName = String.Empty;
      }
   
      /// <summary>
      /// Reads a collection of client settings from a file.
      /// </summary>
      /// <param name="filePath">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      public IEnumerable<ClientSettings> Read(string filePath, int filterIndex)
      {
         if (filePath == null) throw new ArgumentNullException("filePath");

         Debug.Assert(filterIndex <= _settingsPlugins.Count());

         var serializer = _settingsPlugins[filterIndex - 1].Interface;
         List<ClientSettings> settings = serializer.Deserialize(filePath);

         if (settings.Count != 0)
         {
            // update the settings plugin index only if something was loaded
            FilterIndex = filterIndex;
            FileName = filePath;
         }

         return settings.AsReadOnly();
      }

      /// <summary>
      /// Writes a collection of client settings to a file.
      /// </summary>
      /// <param name="settingsCollection">Settings collection.</param>
      /// <param name="filePath">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      public void Write(IEnumerable<ClientSettings> settingsCollection, string filePath, int filterIndex)
      {
         if (settingsCollection == null) throw new ArgumentNullException("settingsCollection");
         if (filePath == null) throw new ArgumentNullException("filePath");
         if (filterIndex < 1 || filterIndex > _settingsPlugins.Count()) throw new ArgumentOutOfRangeException("filterIndex");

         var serializer = _settingsPlugins[filterIndex - 1].Interface;
         serializer.Serialize(filePath, settingsCollection.ToList());

         FilterIndex = filterIndex;
         FileName = filePath;
      }
   }
}
