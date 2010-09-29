/*
 * HFM.NET - UnitInfo Container Class
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   public interface IUnitInfoContainer
   {
      /// <summary>
      /// Add to the Container
      /// </summary>
      void Add(IUnitInfo unit);

      /// <summary>
      /// Clear the Container
      /// </summary>
      void Clear();

      /// <summary>
      /// Retrieve from the Container
      /// </summary>
      UnitInfo RetrieveUnitInfo(ClientInstance instance);

      /// <summary>
      /// Read Binary File
      /// </summary>
      void Read();

      /// <summary>
      /// Write Binary File
      /// </summary>
      void Write();
   }

   public class UnitInfoContainer : IUnitInfoContainer
   {
      #region Fields
      
      /// <summary>
      /// UnitInfo Collection
      /// </summary>
      private UnitInfoCollection _collection;
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _prefs; 
      
      #endregion
      
      #region Constructor
      
      public UnitInfoContainer(IPreferenceSet prefs)
      {
         _prefs = prefs;
      } 
      
      #endregion

      #region Implementation
      
      /// <summary>
      /// Add to the Container
      /// </summary>
      public void Add(IUnitInfo unit)
      {
         _collection.UnitInfoList.Add((UnitInfo)unit);
      }

      /// <summary>
      /// Clear the Container
      /// </summary>
      public void Clear()
      {
         _collection.UnitInfoList.Clear();
      }

      /// <summary>
      /// Retrieve from the Container
      /// </summary>
      /// <param name="instance">Client Instance</param>
      public UnitInfo RetrieveUnitInfo(ClientInstance instance)
      {
         return _collection.UnitInfoList.Find(instance.DisplayInstance.Owns);
      }
      
      #endregion

      #region Serialization Support
      
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         string filePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), Constants.UnitInfoCacheFileName);
         
         _collection = DeserializeLegacy(filePath);
         if (_collection == null)
         {
            _collection = Deserialize(filePath);
         }
         if (_collection == null)
         {
            _collection = new UnitInfoCollection();
         }
      }

      /// <summary>
      /// Write Binary File
      /// </summary>
      public void Write()
      {
         Serialize(_collection, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), Constants.UnitInfoCacheFileName));
      }

      private static readonly object SerializeLock = typeof(UnitInfoCollection);

      public static void Serialize(UnitInfoCollection collection, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  Serializer.Serialize(fileStream, collection);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static UnitInfoCollection Deserialize(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         UnitInfoCollection collection = null;
         try
         {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               collection = Serializer.Deserialize<UnitInfoCollection>(fileStream);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return collection;
      }

      public static UnitInfoCollection DeserializeLegacy(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         UnitInfoCollection collection = null;

         FileStream fileStream = null;
         var formatter = new BinaryFormatter();
         try
         {
            fileStream = new FileStream(filePath, FileMode.Open);
            collection = (UnitInfoCollection)formatter.Deserialize(fileStream);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            if (fileStream != null)
            {
               fileStream.Close();
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return collection;
      }
      
      #endregion
   }
}
