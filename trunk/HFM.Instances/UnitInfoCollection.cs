/*
 * HFM.NET - UnitInfo Collection Helper Class
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Text;

using HFM.Preferences;
using HFM.Instrumentation;

namespace HFM.Instances
{
   [Serializable]
   public class UnitInfoCollection
   {
      #region Members
      private const string DataStoreFilename = "UnitInfoCache.dat";
      private const string CompletedUnitsCSV = "CompletedUnits.csv";
      private const string COMMA = ",";

      private readonly List<UnitInfo> _unitInfoList = new List<UnitInfo>(); 
      #endregion

      #region Implementation
      public void Add(UnitInfo unit)
      {
         _unitInfoList.Add(unit);
      }

      public void Clear()
      {
         _unitInfoList.Clear();
      }

      public UnitInfo RetrieveUnitInfo(string instanceName, string instancePath)
      {
         UnitInfo findUnit = _unitInfoList.Find(delegate(UnitInfo unit)
                                                   {
                                                      return unit.OwningInstanceName == instanceName &&
                                                             unit.OwningInstancePath == instancePath;
                                                   });
         return findUnit;
      } 
      #endregion

      #region Write Completed Unit Info
      public static void WriteCompletedUnitInfo(UnitInfo unit)
      {
         UpgradeUnitInfoCsvFile();
      
         // Open CSV file and append completed unit info to file
         StreamWriter csvFile = null;
         try
         {
            bool bWriteHeader = false;

            string fileName = Path.Combine(PreferenceSet.Instance.AppDataPath, CompletedUnitsCSV);
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
         string oldFilePath = Path.Combine(PreferenceSet.AppPath, CompletedUnitsCSV);
         string oldFilePath022 = Path.Combine(PreferenceSet.AppPath, CompletedUnitsCSV.Replace(".csv", ".0_2_2.csv"));
         string newFilePath = Path.Combine(PreferenceSet.Instance.AppDataPath, CompletedUnitsCSV);
         string newFilePath022 = Path.Combine(PreferenceSet.Instance.AppDataPath, CompletedUnitsCSV.Replace(".csv", ".0_2_2.csv"));
         
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

      private static string GetUnitCsvLine(UnitInfo unit)
      {
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
         sbldr.Append(Math.Round(unit.PPDPerAllSections, PreferenceSet.Instance.DecimalPlaces));
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
         // Write Bonus Credit (if applicable)
         sbldr.Append(unit.GetCredit());
         sbldr.Append(COMMA);
         sbldr.Append(unit.Frames);
         sbldr.Append(COMMA);
         sbldr.Append(unit.NumAtoms);
         sbldr.Append(COMMA);
         sbldr.Append(String.Format("({0}/{1}/{2})", unit.ProjectRun, unit.ProjectClone, unit.ProjectGen));

         return sbldr.ToString();
      }
      #endregion

      #region Singleton Support
      private static UnitInfoCollection _Instance;
      private static readonly object classLock = typeof(UnitInfoCollection);

      public static UnitInfoCollection Instance
      {
         get
         {
            lock (classLock)
            {
               if (_Instance == null)
               {
                  _Instance = Deserialize(Path.Combine(PreferenceSet.Instance.AppDataPath, DataStoreFilename));
               }
               if (_Instance == null)
               {
                  _Instance = new UnitInfoCollection();
               }
            }
            return _Instance;
         }
      } 
      #endregion
      
      #region Constructor
      /// <summary>
      /// Private Constructor to enforce Singleton pattern; loads preferences
      /// </summary>
      private UnitInfoCollection()
      {

      } 
      #endregion
      
      #region Serialization Support
      public static void Serialize()
      {
         Serialize(Instance, Path.Combine(PreferenceSet.Instance.AppDataPath, DataStoreFilename));
      }

      private static UnitInfoCollection Deserialize(string filePath)
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

      private static readonly object _serializeLock = typeof(UnitInfoCollection);

      private static void Serialize(UnitInfoCollection collection, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;
      
         lock (_serializeLock)
         {
            FileStream fileStream = null;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
               fileStream = new FileStream(filePath, FileMode.Create);
               formatter.Serialize(fileStream, collection);
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
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
      #endregion
   }
}
