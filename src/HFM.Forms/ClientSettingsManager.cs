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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HFM.Core.Client;
using HFM.Core.DataTypes;
using HFM.Core.Serializers;

namespace HFM.Forms
{
   public class ClientSettingsManager
   {
      /// <summary>
      /// Current File Name
      /// </summary>
      public string FileName { get; private set; }

      /// <summary>
      /// Current Filter Index
      /// </summary>
      public int FilterIndex { get; private set; }

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

            return _serializers[FilterIndex - 1].FileExtension;
         }
      }

      public string FileTypeFilters
      {
         get 
         { 
            var sb = new System.Text.StringBuilder();
            foreach (var serializer in _serializers)
            {
               if (sb.Length > 0)
               {
                  sb.Append("|");
               }
               sb.Append(serializer.FileTypeFilter);
            }
            return sb.ToString();
         }
      }

      private readonly List<IFileSerializer<List<ClientSettings>>> _serializers;

      public ClientSettingsManager()
      {
         _serializers = new List<IFileSerializer<List<ClientSettings>>>
         {
            new ClientSettingsFileSerializer()
         };

         ClearFileName();
      }

      /// <summary>
      /// Clear the Configuration Filename
      /// </summary>
      public void ClearFileName()
      {
         FilterIndex = 1;
         FileName = String.Empty;
      }
   
      /// <summary>
      /// Reads a collection of client settings from a file.
      /// </summary>
      /// <param name="path">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      public virtual IEnumerable<ClientSettings> Read(string path, int filterIndex)
      {
         if (path == null) throw new ArgumentNullException(nameof(path));
         if (filterIndex < 1 || filterIndex > _serializers.Count) throw new ArgumentOutOfRangeException(nameof(filterIndex));

         var serializer = _serializers[filterIndex - 1];
         List<ClientSettings> settings = serializer.Deserialize(path);

         if (settings.Count != 0)
         {
            // update the serializer index only if something was loaded
            FilterIndex = filterIndex;
            FileName = path;
         }

         return settings;
      }

      /// <summary>
      /// Writes a collection of client settings to a file.
      /// </summary>
      /// <param name="settings">Settings collection.</param>
      /// <param name="path">Path to config file.</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based).</param>
      public virtual void Write(IEnumerable<ClientSettings> settings, string path, int filterIndex)
      {
         if (settings == null) throw new ArgumentNullException(nameof(settings));
         if (path == null) throw new ArgumentNullException(nameof(path));
         if (filterIndex < 1 || filterIndex > _serializers.Count) throw new ArgumentOutOfRangeException(nameof(filterIndex));

         var serializer = _serializers[filterIndex - 1];
         serializer.Serialize(path, settings.ToList());

         FilterIndex = filterIndex;
         FileName = path;
      }
   }
}
