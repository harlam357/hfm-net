/*
 * HFM.NET - Registry Operations Helper Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using HFM.Instrumentation;

namespace HFM.Helpers
{
   public class RegistryOps
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
      public static bool IsHfmAutoRunSet(string name)
      {
         object CurrentHfmAutoRunValue;
         try
         {
            CurrentHfmAutoRunValue = GetHkCuAutoRunKey().GetValue(name);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            return false;
         }

         // value should be null
         if (CurrentHfmAutoRunValue == null)
         {
            return false;
         }
         // if it's an empty string, try to remove the value
         // the value should exist with something or than an empty string, or it should not exist at all
         else if (CurrentHfmAutoRunValue.ToString().Equals(String.Empty))
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
      /// <param name="FilePath">File Path to HFM.exe executable.</param>
      /// <exception cref="InvalidOperationException">When Auto Run value cannot be set.</exception>
      public static void SetHfmAutoRun(string FilePath)
      {
         SetHfmAutoRun(FilePath, DefaultHfmAutoRunName);
      }

      /// <summary>
      /// Set HFM.NET Auto Run Status.
      /// </summary>
      /// <param name="FilePath">File Path to HFM.exe executable.</param>
      /// <param name="name">Name of Value to add to the HKCU Run RegistryKey.</param>
      /// <exception cref="InvalidOperationException">When Auto Run value cannot be set.</exception>
      public static void SetHfmAutoRun(string FilePath, string name)
      {
         RegistryKey regHkCuRun = GetHkCuAutoRunKey();
         
         try
         {
            if (String.IsNullOrEmpty(FilePath))
            {
               regHkCuRun.DeleteValue(name, false);
            }
            else
            {
               regHkCuRun.SetValue(name, FilePath, RegistryValueKind.String);
            }
         }
         catch (Exception ex)
         {
            throw new InvalidOperationException("HFM.NET Auto Run value could not be set.", ex);
         }
      }

      /// <summary>
      /// Gets the HKCU Run RegistryKey.
      /// </summary>
      /// <exception cref="InvalidOperationException">When Registry Key cannot be opened.</exception>
      private static RegistryKey GetHkCuAutoRunKey()
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
         
         if (regHkCuRun == null)
         {
            throw new InvalidOperationException("Registry could not be opened.  Please check your user permissions.");
         }
         
         return regHkCuRun;   
      }
   }
}
