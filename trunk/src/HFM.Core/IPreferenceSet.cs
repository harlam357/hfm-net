/*
 * HFM.NET - User Preferences Interface
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
      /// Log File Cache Directory
      /// </summary>
      string CacheDirectory { get; }

      /// <summary>
      /// Url to EOC User Xml File
      /// </summary>
      string EocUserXml { get; }

      /// <summary>
      /// Url to EOC User Page
      /// </summary>
      Uri EocUserUrl { get; }

      /// <summary>
      /// Url to EOC Team Page
      /// </summary>
      Uri EocTeamUrl { get; }

      /// <summary>
      /// Url to Stanford User Page
      /// </summary>
      Uri StanfordUserUrl { get; }

      void Reset();

      /// <summary>
      /// Get a Preference of Type T
      /// </summary>
      /// <typeparam name="T">Preference Data Type</typeparam>
      /// <param name="key">Preference Key</param>
      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      T Get<T>(Preference key);

      /// <summary>
      /// Set a Preference of Type T
      /// </summary>
      /// <typeparam name="T">Preference Data Type</typeparam>
      /// <param name="key">Preference Key</param>
      /// <param name="value">Preference Value</param>
      void Set<T>(Preference key, T value);

      /// <summary>
      /// Revert to the previously saved settings
      /// </summary>
      void Discard();

      /// <summary>
      /// Save the Preferences Set
      /// </summary>
      void Save();

      /// <summary>
      /// Form Show Style Settings Changed
      /// </summary>
      event EventHandler FormShowStyleSettingsChanged;

      /// <summary>
      /// Background Timer (Refresh or Web) Settings Changed
      /// </summary>
      event EventHandler TimerSettingsChanged;

      /// <summary>
      /// Offline Last Setting Changed
      /// </summary>
      event EventHandler OfflineLastChanged;

      /// <summary>
      /// PPD Calculation Type Changed
      /// </summary>
      event EventHandler PpdCalculationChanged;

      /// <summary>
      /// Debug Message Level Changed
      /// </summary>
      event EventHandler MessageLevelChanged;

      /// <summary>
      /// PPD Decimal Places Setting Changed
      /// </summary>
      event EventHandler DecimalPlacesChanged;

      /// <summary>
      /// Show User Statistics Setting Changed
      /// </summary>
      event EventHandler ShowUserStatsChanged;

      /// <summary>
      /// Color Log File Setting Changed
      /// </summary>
      event EventHandler ColorLogFileChanged;

      /// <summary>
      /// Calculate Bonus Credit and PPD Setting Changed
      /// </summary>
      event EventHandler CalculateBonusChanged;

      /// <summary>
      /// PPD String Formatter
      /// </summary>
      string PpdFormatString { get; }
   }
}
