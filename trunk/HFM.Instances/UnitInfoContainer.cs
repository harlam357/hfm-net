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
      private const string CompletedUnitsCsv = "CompletedUnits.csv";
      private const string Comma = ",";
      #endregion

      #region Members
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
         UnitInfo findUnit = _collection.UnitInfoList.Find(unit => unit.OwningInstanceName == instanceName &&
                                                                   unit.OwningInstancePath == instancePath);
         return findUnit;
      }
      #endregion

      #region Write Completed Unit Info
      public static void WriteCompletedUnitInfo(IUnitInfoLogic unit)
      {
         if (unit == null) throw new ArgumentNullException("unit", "Argument 'unit' cannot be null.");
      
         UpgradeUnitInfoCsvFile();

         // Open CSV file and append completed unit info to file
         StreamWriter csvFile = null;
         try
         {
            bool bWriteHeader = false;

            string fileName = Path.Combine(InstanceProvider.GetInstance<IPreferenceSet>().GetPreference<string>(
                                              Preference.ApplicationDataFolderPath), CompletedUnitsCsv);

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
         IPreferenceSet prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         string applicationDataFolderPath = prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);

         string oldFilePath = Path.Combine(prefs.ApplicationPath, CompletedUnitsCsv);
         string oldFilePath022 = Path.Combine(prefs.ApplicationPath, CompletedUnitsCsv.Replace(".csv", ".0_2_2.csv"));
         string newFilePath = Path.Combine(applicationDataFolderPath, CompletedUnitsCsv);
         string newFilePath022 = Path.Combine(applicationDataFolderPath, CompletedUnitsCsv.Replace(".csv", ".0_2_2.csv"));

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
               string[] headerSplit = headerLine.Split(new[] { Comma }, StringSplitOptions.None);
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
         sbldr.Append(Comma);
         sbldr.Append("Work Unit Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Path");
         sbldr.Append(Comma);
         sbldr.Append("Username");
         sbldr.Append(Comma);
         sbldr.Append("Team");
         sbldr.Append(Comma);
         sbldr.Append("Client Type");
         sbldr.Append(Comma);
         sbldr.Append("Core Name");
         sbldr.Append(Comma);
         sbldr.Append("Core Version");
         sbldr.Append(Comma);
         sbldr.Append("Frame Time (Average)");
         sbldr.Append(Comma);
         sbldr.Append("PPD");
         sbldr.Append(Comma);
         sbldr.Append("Download Date");
         sbldr.Append(Comma);
         sbldr.Append("Download Time");
         sbldr.Append(Comma);
         sbldr.Append("Completion Date (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Completion Time (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Credit");
         sbldr.Append(Comma);
         sbldr.Append("Frames");
         sbldr.Append(Comma);
         sbldr.Append("Atoms");
         sbldr.Append(Comma);
         sbldr.Append("Run/Clone/Gen");

         return sbldr.ToString();
      }

      private static string GetUnitCsvLine(IUnitInfoLogic unit)
      {
         IPreferenceSet prefs = InstanceProvider.GetInstance<IPreferenceSet>();

         StringBuilder sbldr = new StringBuilder();
         sbldr.Append(unit.ProjectID);
         sbldr.Append(Comma);
         sbldr.Append(unit.WorkUnitName);
         sbldr.Append(Comma);
         sbldr.Append(unit.OwningInstanceName);
         sbldr.Append(Comma);
         sbldr.Append(unit.OwningInstancePath);
         sbldr.Append(Comma);
         sbldr.Append(unit.FoldingID);
         sbldr.Append(Comma);
         sbldr.Append(unit.Team);
         sbldr.Append(Comma);
         sbldr.Append(unit.TypeOfClient.ToString());
         sbldr.Append(Comma);
         sbldr.Append(unit.Core);
         sbldr.Append(Comma);
         sbldr.Append(unit.CoreVersion);
         sbldr.Append(Comma);
         // Issue 43 - Use Time Per All Sections and not unit.PPD
         sbldr.Append(unit.TimePerAllSections.ToString());
         sbldr.Append(Comma);
         // Issue 43 - Use Time Per All Sections and not unit.PPD
         sbldr.Append(Math.Round(unit.PPDPerAllSections, prefs.GetPreference<int>(Preference.DecimalPlaces)));
         sbldr.Append(Comma);
         sbldr.Append(unit.DownloadTime.ToShortDateString());
         sbldr.Append(Comma);
         sbldr.Append(unit.DownloadTime.ToShortTimeString());
         sbldr.Append(Comma);
         if (unit.FinishedTime.Equals(DateTime.MinValue))
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Writing CompletedUnitInfo using DateTime.Now.", true);
            sbldr.Append(DateTime.Now.ToShortDateString());
            sbldr.Append(Comma);
            sbldr.Append(DateTime.Now.ToShortTimeString());
            sbldr.Append(Comma);
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Writing CompletedUnitInfo using UnitInfo.FinishedTime.", true);
            sbldr.Append(unit.FinishedTime.ToShortDateString());
            sbldr.Append(Comma);
            sbldr.Append(unit.FinishedTime.ToShortTimeString());
            sbldr.Append(Comma);
         }
         // Write Bonus Credit if enabled - Issue 125
         if (prefs.GetPreference<bool>(Preference.CalculateBonus))
         {
            sbldr.Append(unit.GetBonusCredit());
         }
         else
         {
            sbldr.Append(unit.Credit);
         }
         sbldr.Append(Comma);
         sbldr.Append(unit.Frames);
         sbldr.Append(Comma);
         sbldr.Append(unit.NumAtoms);
         sbldr.Append(Comma);
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
         string filePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);
         
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
         Serialize(_collection, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }

      private static readonly object SerializeLock = typeof(UnitInfoCollection);

      public static void Serialize(UnitInfoCollection collection, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static UnitInfoCollection Deserialize(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         UnitInfoCollection collection = null;
         try
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               collection = ProtoBuf.Serializer.Deserialize<UnitInfoCollection>(fileStream);
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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return collection;
      }
      #endregion
   }
}
