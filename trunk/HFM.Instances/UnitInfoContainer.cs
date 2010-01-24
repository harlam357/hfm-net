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
using System.Text;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class UnitInfoContainer : IUnitInfoContainer
   {
      #region Constants
      private const string DataStoreFilename = "UnitInfoCache.dat";
      private const string CompletedUnitsCSV = "CompletedUnits.csv";
      private const string COMMA = ",";
      #endregion

      #region Members
      /// <summary>
      /// UnitInfo Collection
      /// </summary>
      private UnitInfoCollection _collection;
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs; 
      #endregion
      
      #region Constructor
      public UnitInfoContainer(IPreferenceSet Prefs)
      {
         _Prefs = Prefs;
      } 
      #endregion

      #region Implementation
      /// <summary>
      /// Add to the Container
      /// </summary>
      /// <param name="unit"></param>
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
      /// <param name="instanceName">ClientInstance Name</param>
      /// <param name="instancePath">ClientInstance Path</param>
      public IUnitInfo RetrieveUnitInfo(string instanceName, string instancePath)
      {
         UnitInfo findUnit = _collection.UnitInfoList.Find(delegate(UnitInfo unit)
         {
            return unit.OwningInstanceName == instanceName &&
                   unit.OwningInstancePath == instancePath;
         });
         return findUnit;
      }
      #endregion

      #region Write Completed Unit Info
      public static void WriteCompletedUnitInfo(IUnitInfoLogic unit)
      {
         UpgradeUnitInfoCsvFile();

         // Open CSV file and append completed unit info to file
         StreamWriter csvFile = null;
         try
         {
            bool bWriteHeader = false;

            string fileName = Path.Combine(InstanceProvider.GetInstance<IPreferenceSet>().GetPreference<string>(
                                              Preference.ApplicationDataFolderPath), CompletedUnitsCSV);

            if (File.Exists(fileName) == false)
            {
               bWriteHeader = true;
            }

            csvFile = new StreamWriter(fileName, true);

            if (bWriteHeader)
            {
               csvFile.WriteLine(GetUnitCsvHeader());
            }

            csvFile.WriteLine(GetUnitCsvLine(unit));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            if (csvFile != null)
            {
               csvFile.Close();
            }
         }
      }

      private static void UpgradeUnitInfoCsvFile()
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         string ApplicationDataFolderPath = Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);

         string oldFilePath = Path.Combine(Prefs.ApplicationPath, CompletedUnitsCSV);
         string oldFilePath022 = Path.Combine(Prefs.ApplicationPath, CompletedUnitsCSV.Replace(".csv", ".0_2_2.csv"));
         string newFilePath = Path.Combine(ApplicationDataFolderPath, CompletedUnitsCSV);
         string newFilePath022 = Path.Combine(ApplicationDataFolderPath, CompletedUnitsCSV.Replace(".csv", ".0_2_2.csv"));

         // If file does not exist in new location but does exist in old location
         if (File.Exists(newFilePath) == false && File.Exists(oldFilePath))
         {
            try
            {
               // Try to copy it from the old to the new
               File.Copy(oldFilePath, newFilePath);
               File.Delete(oldFilePath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
         }

         // If file does not exist in new location but does exist in old location
         if (File.Exists(newFilePath022) == false && File.Exists(oldFilePath022))
         {
            try
            {
               // Try to copy it from the old to the new
               File.Copy(oldFilePath022, newFilePath022);
               File.Delete(oldFilePath022);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
         }

         StreamReader csvFile = null;
         try
         {
            if (File.Exists(newFilePath))
            {
               // Open the current file and read the first line (header)
               csvFile = new StreamReader(newFilePath);
               string headerLine = csvFile.ReadLine();
               csvFile.Close();
               csvFile = null;

               // Split the line on Comma and check the resulting array length
               string[] headerSplit = headerLine.Split(new string[] { COMMA }, StringSplitOptions.None);
               // If less than 19 items this file was created before v0.3.0, last release version
               // before v0.3.0 is v0.2.2.  Rename the current file with last release version.
               if (headerSplit.Length < 19)
               {
                  File.Move(newFilePath, newFilePath.Replace(".csv", ".0_2_2.csv"));
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            if (csvFile != null)
            {
               csvFile.Close();
            }
         }
      }

      private static string GetUnitCsvHeader()
      {
         StringBuilder sbldr = new StringBuilder();
         sbldr.Append("ProjectID");
         sbldr.Append(COMMA);
         sbldr.Append("Work Unit Name");
         sbldr.Append(COMMA);
         sbldr.Append("Instance Name");
         sbldr.Append(COMMA);
         sbldr.Append("Instance Path");
         sbldr.Append(COMMA);
         sbldr.Append("Username");
         sbldr.Append(COMMA);
         sbldr.Append("Team");
         sbldr.Append(COMMA);
         sbldr.Append("Client Type");
         sbldr.Append(COMMA);
         sbldr.Append("Core Name");
         sbldr.Append(COMMA);
         sbldr.Append("Core Version");
         sbldr.Append(COMMA);
         sbldr.Append("Frame Time (Average)");
         sbldr.Append(COMMA);
         sbldr.Append("PPD");
         sbldr.Append(COMMA);
         sbldr.Append("Download Date");
         sbldr.Append(COMMA);
         sbldr.Append("Download Time");
         sbldr.Append(COMMA);
         sbldr.Append("Completion Date (Observed)");
         sbldr.Append(COMMA);
         sbldr.Append("Completion Time (Observed)");
         sbldr.Append(COMMA);
         sbldr.Append("Credit");
         sbldr.Append(COMMA);
         sbldr.Append("Frames");
         sbldr.Append(COMMA);
         sbldr.Append("Atoms");
         sbldr.Append(COMMA);
         sbldr.Append("Run/Clone/Gen");

         return sbldr.ToString();
      }

      private static string GetUnitCsvLine(IUnitInfoLogic unit)
      {
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();

         StringBuilder sbldr = new StringBuilder();
         sbldr.Append(unit.ProjectID);
         sbldr.Append(COMMA);
         sbldr.Append(unit.WorkUnitName);
         sbldr.Append(COMMA);
         sbldr.Append(unit.OwningInstanceName);
         sbldr.Append(COMMA);
         sbldr.Append(unit.OwningInstancePath);
         sbldr.Append(COMMA);
         sbldr.Append(unit.FoldingID);
         sbldr.Append(COMMA);
         sbldr.Append(unit.Team);
         sbldr.Append(COMMA);
         sbldr.Append(unit.TypeOfClient.ToString());
         sbldr.Append(COMMA);
         sbldr.Append(unit.Core);
         sbldr.Append(COMMA);
         sbldr.Append(unit.CoreVersion);
         sbldr.Append(COMMA);
         // Issue 43 - Use Time Per All Sections and not unit.PPD
         sbldr.Append(unit.TimePerAllSections.ToString());
         sbldr.Append(COMMA);
         // Issue 43 - Use Time Per All Sections and not unit.PPD
         sbldr.Append(Math.Round(unit.PPDPerAllSections, Prefs.GetPreference<int>(Preference.DecimalPlaces)));
         sbldr.Append(COMMA);
         sbldr.Append(unit.DownloadTime.ToShortDateString());
         sbldr.Append(COMMA);
         sbldr.Append(unit.DownloadTime.ToShortTimeString());
         sbldr.Append(COMMA);
         if (unit.FinishedTime.Equals(DateTime.MinValue))
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Writing CompletedUnitInfo using DateTime.Now.", true);
            sbldr.Append(DateTime.Now.ToShortDateString());
            sbldr.Append(COMMA);
            sbldr.Append(DateTime.Now.ToShortTimeString());
            sbldr.Append(COMMA);
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Writing CompletedUnitInfo using UnitInfo.FinishedTime.", true);
            sbldr.Append(unit.FinishedTime.ToShortDateString());
            sbldr.Append(COMMA);
            sbldr.Append(unit.FinishedTime.ToShortTimeString());
            sbldr.Append(COMMA);
         }
         // Write Bonus Credit if enabled - Issue 125
         if (Prefs.GetPreference<bool>(Preference.CalculateBonus))
         {
            sbldr.Append(unit.GetBonusCredit());
         }
         else
         {
            sbldr.Append(unit.Credit);
         }
         sbldr.Append(COMMA);
         sbldr.Append(unit.Frames);
         sbldr.Append(COMMA);
         sbldr.Append(unit.NumAtoms);
         sbldr.Append(COMMA);
         sbldr.Append(String.Format("({0}/{1}/{2})", unit.ProjectRun, unit.ProjectClone, unit.ProjectGen));

         return sbldr.ToString();
      }
      #endregion

      #region Serialization Support
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         string FilePath = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);
         
         _collection = DeserializeLegacy(FilePath);
         if (_collection == null)
         {
            _collection = Deserialize(FilePath);
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
         Serialize(_collection, Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }

      private static readonly object _serializeLock = typeof(UnitInfoCollection);

      public static void Serialize(UnitInfoCollection collection, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         lock (_serializeLock)
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  ProtoBuf.Serializer.Serialize(fileStream, collection);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }

      public static UnitInfoCollection Deserialize(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         UnitInfoCollection collection = null;
         using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
         {
            try
            {
               collection = ProtoBuf.Serializer.Deserialize<UnitInfoCollection>(fileStream);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);

         return collection;
      }

      public static UnitInfoCollection DeserializeLegacy(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         UnitInfoCollection collection = null;

         FileStream fileStream = null;
         BinaryFormatter formatter = new BinaryFormatter();
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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);

         return collection;
      }
      #endregion
   }
}
