/*
 * HFM.NET - Registry Operations Helper Class
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

using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace HFM.Framework
{
   public static class RegistryOps
   {
      private const string DefaultHfmAutoRunName = "HFM.NET";
      private const string HkCuAutoRunSubKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
      
      /// <summary>
      /// Does an Auto Run Value Exist?
      /// </summary>
      public static bool IsHfmAutoRunSet()
      {
         return IsHfmAutoRunSet(DefaultHfmAutoRunName);
      }

      /// <summary>
      /// Does an Auto Run Value Exist?
      /// </summary>
      /// <param name="name">Name of Key Value to check.</param>
      private static bool IsHfmAutoRunSet(string name)
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
         
            currentHfmAutoRunValue = regHkCuRun.GetValue(name);
         }
         catch (InvalidOperationException ex)
         {
            // just write a warning if this fails 
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
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
         else if (currentHfmAutoRunValue.ToString().Length == 0) // FxCop: CA1820
         {
            try
            {
               SetHfmAutoRun(String.Empty);
            }
            catch (InvalidOperationException ex)
            {
               // just write a warning if this fails 
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
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
      public static void SetHfmAutoRun(string filePath)
      {
         SetHfmAutoRun(filePath, DefaultHfmAutoRunName);
      }

      /// <summary>
      /// Set HFM.NET Auto Run Status.
      /// </summary>
      /// <param name="filePath">File Path to HFM.exe executable.</param>
      /// <param name="name">Name of Value to add to the HKCU Run RegistryKey.</param>
      /// <exception cref="InvalidOperationException">When Auto Run value cannot be set.</exception>
      private static void SetHfmAutoRun(string filePath, string name)
      {
         RegistryKey regHkCuRun = null;
         
         try
         {
            regHkCuRun = GetHkCuAutoRunKey(true);
         
            if (String.IsNullOrEmpty(filePath))
            {
               regHkCuRun.DeleteValue(name, false);
            }
            else
            {
               regHkCuRun.SetValue(name, WrapInQuotes(filePath), RegistryValueKind.String);
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
