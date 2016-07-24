/*
 * HFM.NET - User Preferences Interface
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;

namespace HFM.Core
{
   public interface IPreferenceSet
   {
      string ApplicationPath { get; }

      string ApplicationDataFolderPath { get; }

      /// <summary>
      /// Gets the local client log cache directory.
      /// </summary>
      string CacheDirectory { get; }

      void Reset();

      void Initialize();

      void Save();

      /// <summary>
      /// Gets a preference.
      /// </summary>
      /// <typeparam name="T">The type of the preference value.</typeparam>
      /// <param name="key">The preference key.</param>
      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      T Get<T>(Preference key);

      /// <summary>
      /// Sets a preference.
      /// </summary>
      /// <typeparam name="T">The type of the preference value.</typeparam>
      /// <param name="key">The preference key.</param>
      /// <param name="value">The preference value.</param>
      void Set<T>(Preference key, T value);

      /// <summary>
      /// Raised when a preference value is changed.
      /// </summary>
      event EventHandler<PreferenceChangedEventArgs> PreferenceChanged;
   }

   public sealed class PreferenceChangedEventArgs : EventArgs
   {
      public Preference Preference { get; private set; }

      public PreferenceChangedEventArgs(Preference preference)
      {
         Preference = preference;
      }
   }
}
