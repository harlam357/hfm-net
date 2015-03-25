/*
 * HFM.NET - AutoRun Registry Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using Microsoft.Win32;

using Castle.Core.Logging;

namespace HFM.Core
{
   public interface IAutoRun
   {
      /// <summary>
      /// Is auto run enabled?
      /// </summary>
      bool IsEnabled();

      /// <summary>
      /// Sets the HFM.NET auto run value.
      /// </summary>
      /// <param name="filePath">The file path to HFM.exe executable.</param>
      /// <exception cref="InvalidOperationException">Auto run value cannot be set.</exception>
      void SetFilePath(string filePath);
   }

   public class AutoRun : IAutoRun
   {
      #region Constants

      private const string AutoRunKeyName = "HFM.NET";
      private const string HkCuAutoRunSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

      #endregion

      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      /// <summary>
      /// Is auto run enabled?
      /// </summary>
      public bool IsEnabled()
      {
         try
         {
            object currentHfmAutoRunValue = null;

            using (RegistryKey regHkCuRun = GetHkCuAutoRunKey())
            {
               if (regHkCuRun != null)
               {
                  currentHfmAutoRunValue = regHkCuRun.GetValue(AutoRunKeyName);
               }
            }

            return currentHfmAutoRunValue != null && currentHfmAutoRunValue.ToString().Length > 0;
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
         
         return false;
      }

      /// <summary>
      /// Sets the HFM.NET auto run value.
      /// </summary>
      /// <param name="filePath">The file path to HFM.exe executable.</param>
      /// <exception cref="InvalidOperationException">Auto run value cannot be set.</exception>
      public void SetFilePath(string filePath)
      {
         try
         {
            using (RegistryKey regHkCuRun = GetHkCuAutoRunKey(true))
            {
               if (String.IsNullOrEmpty(filePath))
               {
                  regHkCuRun.DeleteValue(AutoRunKeyName, false);
               }
               else
               {
                  regHkCuRun.SetValue(AutoRunKeyName, WrapInQuotes(filePath), RegistryValueKind.String);
               }
            }
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException("HFM.NET auto run value could not be set.", ex);
         }
      }

      private static RegistryKey GetHkCuAutoRunKey(bool create = false)
      {
         RegistryKey regHkCuRun = Registry.CurrentUser.OpenSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
         if (regHkCuRun != null)
         {
            return regHkCuRun;
         }
         return create ? Registry.CurrentUser.CreateSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree) : null;
      }
      
      /// <summary>
      /// Wraps the given string in quotes.
      /// </summary>
      private static string WrapInQuotes(string value)
      {
         return String.Concat("\"", value, "\"");
      }
   }
}
