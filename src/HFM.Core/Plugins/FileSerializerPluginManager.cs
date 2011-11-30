/*
 * HFM.NET - File Serializer Plugin Manager
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
using System.Linq;
using System.Text;

namespace HFM.Core.Plugins
{
   public interface IFileSerializerPluginManager<T> : IEnumerable<PluginInfo<IFileSerializer<T>>> where T : class, new()
   {
      /// <summary>
      /// Gets the file type filter string for the loaded plugins.
      /// </summary>
      string FileTypeFilters { get; }

      /// <summary>
      /// Returns a PluginInfo from the PluginManager.
      /// </summary>
      /// <param name="index">Plugin index.</param>
      /// <returns>A PluginInfo from the PluginManager if it exists or null if it does not.</returns>
      PluginInfo<IFileSerializer<T>> this[int index] { get; }

      /// <summary>
      /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the PluginManager.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the PluginManager.
      /// </returns>
      IEnumerable<string> Keys { get; }

      /// <summary>
      /// Returns a PluginInfo from the PluginManager.
      /// </summary>
      /// <param name="key">The <paramref name="key"/> of the plugin to retrieve.</param>
      /// <returns>A PluginInfo from the PluginManager if it exists or null if it does not.</returns>
      PluginInfo<IFileSerializer<T>> GetPlugin(string key);

      /// <summary>
      /// Load all plugins from the specified path and return an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing PluginLoadInfo for each plugin found.
      /// </summary>
      /// <param name="path">A string specifying the path from which to load plugins.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="path"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="path"/> contains invalid characters.</exception>
      /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. The specified path, file name, or both are too long.</exception>
      /// <exception cref="T:System.IO.DirectoryNotFoundException">The <paramref name="path"/> is invalid (for example, it is on an unmapped drive).</exception>
      /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing PluginLoadInfo for each plugin found.</returns>
      IEnumerable<PluginLoadInfo> LoadAllPlugins(string path);

      /// <summary>
      /// Load a plugin from the specified file name and return a PluginLoadInfo result.
      /// </summary>
      /// <param name="fileName">A string specifying the plugin file name to load.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="fileName"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">The file name is empty, contains only white spaces, or contains invalid characters.</exception>
      /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
      /// <exception cref="T:System.NotSupportedException"><paramref name="fileName"/> contains a colon (:) in the middle of the string.</exception>
      /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
      /// <exception cref="T:System.UnauthorizedAccessException">Access to fileName is denied.</exception>
      /// <returns>A PluginLoadInfo result.</returns>
      PluginLoadInfo LoadPlugin(string fileName);

      /// <summary>
      /// Register an existing plugin instance.
      /// </summary>
      /// <param name="name">The plugin name.</param>
      /// <param name="plugin">The plugin instance.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="name"/> or <paramref name="plugin"/> is null.</exception>
      void RegisterPlugin(string name, IFileSerializer<T> plugin);
   }

   internal class FileSerializerPluginManager<T> : PluginManager<IFileSerializer<T>>, IFileSerializerPluginManager<T> where T : class, new()
   {
      protected override bool ValidatePlugin(IFileSerializer<T> serializer)
      {
         if (String.IsNullOrEmpty(serializer.FileExtension) ||
             String.IsNullOrEmpty(serializer.FileTypeFilter))
         {
            // extention filter string, too many bar characters
            return false;
         }

         var numOfBarChars = serializer.FileTypeFilter.Count(x => x == '|');
         if (numOfBarChars != 1)
         {
            // too many bar characters
            return false;
         }

         return true;
      }

      public string FileTypeFilters
      {
         get
         {
            var sb = new StringBuilder();
            foreach (var plugin in this)
            {
               sb.Append(plugin.Interface.FileTypeFilter);
               sb.Append("|");
            }

            sb.Length = sb.Length - 1;
            return sb.ToString();
         }
      }
   }
}
