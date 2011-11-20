/*
 * HFM.NET - AutoRun Registry Class
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
using Microsoft.Win32;

using Castle.Core.Logging;

namespace HFM.Core
{
   public interface IAutoRun
   {
      ILogger Logger { get; set; }

      /// <summary>
      /// Does an Auto Run value exist?
      /// </summary>
      bool IsEnabled();

      /// <summary>
      /// Set HFM.NET Auto Run Status.
      /// </summary>
      /// <param name="filePath">File Path to HFM.exe executable.</param>
      /// <exception cref="InvalidOperationException">When Auto Run value cannot be set.</exception>
      void SetFilePath(string filePath);
   }

   public class AutoRun : IAutoRun
   {
      #region Constants

      private const string DefaultHfmAutoRunName = "HFM.NET";
      private const string HkCuAutoRunSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

      #endregion

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         get { return _logger; }
         set { _logger = value; }
      }
     
      /// <summary>
      /// Does an Auto Run value exist?
      /// </summary>
      public bool IsEnabled()
      {
         RegistryKey regHkCuRun = null;
         object currentHfmAutoRunValue = null;
      
         try
         {
            regHkCuRun = GetHkCuAutoRunKey();
            if (regHkCuRun == null)
            {
               return false;
            }
         
            currentHfmAutoRunValue = regHkCuRun.GetValue(DefaultHfmAutoRunName);
         }
         catch (InvalidOperationException ex)
         {
            // just write a warning if this fails 
            _logger.WarnFormat(ex, "{0}", ex.Message);
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }
         finally
         {
            if (regHkCuRun != null)
            {
               regHkCuRun.Close();
            }
         }

         // value should be null
         if (currentHfmAutoRunValue == null)
         {
            return false;
         }
         // if it's an empty string, try to remove the value
         // the value should exist with something other than an empty string, or it should not exist at all
         if (currentHfmAutoRunValue.ToString().Length == 0) // FxCop: CA1820
         {
            try
            {
               SetFilePath(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
               // just write a warning if this fails 
               _logger.WarnFormat(ex, "{0}", ex.Message);
            }

            return false;
         }
         
         return true;
      }

      /// <summary>
      /// Set HFM.NET Auto Run Status.
      /// </summary>
      /// <param name="filePath">File Path to HFM.exe executable.</param>
      /// <exception cref="InvalidOperationException">When Auto Run value cannot be set.</exception>
      public void SetFilePath(string filePath)
      {
         RegistryKey regHkCuRun = null;
         
         try
         {
            regHkCuRun = GetHkCuAutoRunKey(true);
         
            if (String.IsNullOrEmpty(filePath))
            {
               regHkCuRun.DeleteValue(DefaultHfmAutoRunName, false);
            }
            else
            {
               regHkCuRun.SetValue(DefaultHfmAutoRunName, WrapInQuotes(filePath), RegistryValueKind.String);
            }
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException("HFM.NET Auto Run value could not be set.", ex);
         }
         finally
         {
            if (regHkCuRun != null)
            {
               regHkCuRun.Close();
            }
         }
      }

      /// <summary>
      /// Gets the HKCU Run RegistryKey.
      /// </summary>
      /// <exception cref="InvalidOperationException">When Registry Key cannot be opened.</exception>
      private static RegistryKey GetHkCuAutoRunKey()
      {
         return GetHkCuAutoRunKey(false);
      }

      /// <summary>
      /// Gets the HKCU Run RegistryKey.
      /// </summary>
      /// <exception cref="InvalidOperationException">When Registry Key cannot be opened.</exception>
      private static RegistryKey GetHkCuAutoRunKey(bool createIfNotExist)
      {
         RegistryKey regHkCuRun;
      
         try
         {
            regHkCuRun = Registry.CurrentUser.OpenSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException("Registry could not be opened.  Please check your user permissions.", ex);
         }
         
         if (regHkCuRun == null && createIfNotExist)
         {
            try
            {
               regHkCuRun = Registry.CurrentUser.CreateSubKey(HkCuAutoRunSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
               if (regHkCuRun == null)
               {
                  throw new InvalidOperationException("Registry could not be opened.  Please check your user permissions.");
               }
            }
            catch (Exception ex)
            {
               throw new InvalidOperationException("Registry could not be opened.  Please check your user permissions.", ex);
            }
         }
         
         return regHkCuRun;   
      }
      
      /// <summary>
      /// Wrap the given string in quotes
      /// </summary>
      private static string WrapInQuotes(string value)
      {
         return String.Concat("\"", value, "\"");
      }
   }
}
