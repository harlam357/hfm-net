/*
 * HFM.NET - Plugin Manager
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
using System.Reflection;

namespace HFM.Core.Plugins
{
   internal abstract class PluginManager<T> where T : class
   {
      private readonly Dictionary<string, PluginInfo<T>> _plugins;

      protected PluginManager()
      {
         _plugins = new Dictionary<string, PluginInfo<T>>();
      }

      /// <summary>
      /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the PluginManager.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the PluginManager.
      /// </returns>
      public IEnumerable<string> Keys
      {
         get { return _plugins.Keys; }
      }

      /// <summary>
      /// Returns a PluginInfo from the PluginManager.
      /// </summary>
      /// <param name="key">The <paramref name="key"/> of the plugin to retrieve.</param>
      /// <returns>A PluginInfo from the PluginManager if it exists or null if it does not.</returns>
      public PluginInfo<T> GetPlugin(string key)
      {
         return _plugins.ContainsKey(key) ? _plugins[key] : null;
      }

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
      public IEnumerable<PluginLoadInfo> LoadAllPlugins(string path)
      {
         var di = new DirectoryInfo(path);
         return LoadPlugins(di.GetFiles("*.dll", SearchOption.AllDirectories));
      }

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
      public PluginLoadInfo LoadPlugin(string fileName)
      {
         return LoadPlugins(new FileInfo(fileName)).First();
      }

      private IEnumerable<PluginLoadInfo> LoadPlugins(params FileInfo[] files)
      {
         var results = new List<PluginLoadInfo>(files.Length);
         
         foreach (FileInfo fi in files)
         {
            FileVersionInfo fvi;
            try
            {
               fvi = FileVersionInfo.GetVersionInfo(fi.FullName);
               if (fvi.FileDescription == null || fvi.FileDescription.Trim().Length == 0)
               {
                  results.Add(new PluginLoadInfo(fi.FullName, PluginLoadResult.Failure, "File Description is empty."));
                  continue;
               }
            }
            catch (FileNotFoundException ex)
            {
               results.Add(new PluginLoadInfo(fi.FullName, PluginLoadResult.Failure, ex));
               continue;
            }

            try
            {
               var pluginInfo = new PluginInfo<T>(fi.FullName, fvi);
               pluginInfo.Interface = CreatePluginInstance(pluginInfo.FilePath);
               if (ValidatePlugin(pluginInfo.Interface))
               {
                  _plugins.Add(pluginInfo.Name, pluginInfo);
                  results.Add(new PluginLoadInfo(fi.FullName, PluginLoadResult.Success));
               }
               else
               {
                  results.Add(new PluginLoadInfo(fi.FullName, PluginLoadResult.Failure, "Plugin failed to validate.  Implement FileTypeFilter and FileExtension properly."));
               }
            }
            catch (Exception ex)
            {
               results.Add(new PluginLoadInfo(fi.FullName, PluginLoadResult.Failure, ex));
            }
         }

         return results.AsReadOnly();
      }

      private static T CreatePluginInstance(string assemblyFile)
      {
         Debug.Assert(assemblyFile != null);

         Assembly asm = Assembly.LoadFrom(assemblyFile);

         // Loop through each type in the DLL
         foreach (Type t in asm.GetTypes())
         {
            // Only look at public types
            if (t.IsPublic && (t.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
            {
               // See if this type implements our interface
               var interfaceType = t.GetInterface(typeof(T).Name, false);
               if (interfaceType != null)
               {
                  return (T)Activator.CreateInstance(t);
               }
            }
         }

         return null;
      }

      protected virtual bool ValidatePlugin(T plugin)
      {
         return true;
      }
   }
}
