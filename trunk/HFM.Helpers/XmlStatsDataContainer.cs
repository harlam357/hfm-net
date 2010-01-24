/*
 * HFM.NET - XML Stats Data Container
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
using System.Globalization;
using System.IO;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Helpers
{
   public class XmlStatsDataContainer : IXmlStatsDataContainer
   {
      #region Constants
      private const string DataStoreFilename = "UserStatsCache.dat";
      #endregion
   
      #region Members
      private XmlStatsData _data;
      /// <summary>
      /// User Stats Data
      /// </summary>
      public IXmlStatsData Data
      {
         get { return _data; }
      }
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs;
      #endregion
      
      #region Constructor
      public XmlStatsDataContainer(IPreferenceSet Prefs)
      {
         _Prefs = Prefs;
      }
      #endregion
      
      #region Methods
      /// <summary>
      /// Is it Time for a Stats Update?
      /// </summary>
      public bool TimeForUpdate()
      {
         return TimeForNextUpdate(Data.LastUpdated, DateTime.UtcNow, DateTime.Now.IsDaylightSavingTime());
      }

      /// <summary>
      /// Is it Time for a Stats Update?
      /// </summary>
      public static bool TimeForNextUpdate(DateTime LastUpdated, DateTime UtcNow, bool IsDaylightSavingTime)
      {
         // No Last Updated Value
         if (LastUpdated.Equals(DateTime.MinValue))
         {
            return true;
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture,
            "{0} Current Time: {1} (UTC)", HfmTrace.FunctionName, UtcNow));

         DateTime NextUpdateTime = GetNextUpdateTime(LastUpdated, IsDaylightSavingTime);
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture,
            "{0} Next Update Time: {1} (UTC)", HfmTrace.FunctionName, NextUpdateTime));

         if (UtcNow > NextUpdateTime)
         {
            return true;
         }

         return false;
      }

      public static DateTime GetNextUpdateTime(DateTime LastUpdated, bool IsDaylightSavingTime)
      {
         // What I really need to know is if it is Daylight Savings Time
         // in the Central Time Zone, not the local machines Time Zone.

         int offset = 0;
         if (IsDaylightSavingTime)
         {
            offset = 1;
         }

         DateTime nextUpdateTime = LastUpdated.Date;

         int hours = 24;
         for (int i = 0; i < 9; i++)
         {
            if (LastUpdated.TimeOfDay >= TimeSpan.FromHours(hours - offset))
            {
               nextUpdateTime = nextUpdateTime.Add(TimeSpan.FromHours(hours + 3 - offset));
               break;
            }

            hours -= 3;
         }

         return nextUpdateTime;
      }
      #endregion
      
      #region Serialization Support
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         string FilePath = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);

         _data = Deserialize(FilePath);

         if (_data == null)
         {
            _data = new XmlStatsData();
         }
      }

      /// <summary>
      /// Write Binary File
      /// </summary>
      public void Write()
      {
         Serialize(_data, Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }
      
      private static readonly object _serializeLock = typeof(XmlStatsDataContainer);

      public static void Serialize(XmlStatsData data, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         lock (_serializeLock)
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  ProtoBuf.Serializer.Serialize(fileStream, data);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }

      public static XmlStatsData Deserialize(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         XmlStatsData data = null;
         try
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               data = ProtoBuf.Serializer.Deserialize<XmlStatsData>(fileStream);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);

         return data;
      }
      #endregion
   }
}
