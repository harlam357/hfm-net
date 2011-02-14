/*
 * HFM.NET - Instance Configuration Manager Interface
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace HFM.Framework
{
   public interface IInstanceConfigurationManager
   {
      /// <summary>
      /// Client Configuration Filename
      /// </summary>
      string ConfigFilename { get; }

      /// <summary>
      /// Current Plugin Index
      /// </summary>
      int SettingsPluginIndex { get; }

      /// <summary>
      /// Number of Settings Plugins
      /// </summary>
      int SettingsPluginsCount { get; }

      /// <summary>
      /// Client Configuration has Filename defined
      /// </summary>
      bool HasConfigFilename { get; }

      /// <summary>
      /// Current Config File Extension or the Default File Extension
      /// </summary>
      string ConfigFileExtension { get; }

      /// <summary>
      /// String Representation of the File Type Filters used in an Open File Dialog
      /// </summary>
      string FileTypeFilters { get; }

      /// <summary>
      /// Clear the Configuration Filename
      /// </summary>
      void ClearConfigFilename();
   }
}
