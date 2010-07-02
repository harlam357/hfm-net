/*
 * HFM.NET - Data Retriever Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public interface IDataRetriever
   {
      IClientInstanceSettings Settings { get; set; }

      /// <summary>
      /// Retrieve the log and unit info files from the configured Local path
      /// </summary>
      void RetrievePathInstance();

      /// <summary>
      /// Retrieve the log and unit info files from the configured HTTP location
      /// </summary>
      void RetrieveHttpInstance();

      /// <summary>
      /// Retrieve the log and unit info files from the configured FTP location
      /// </summary>
      void RetrieveFtpInstance();
   }

   public class DataRetriever : IDataRetriever
   {
      public IClientInstanceSettings Settings { get; set; }

      private readonly IPreferenceSet _prefs;
      
      public DataRetriever(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }
   
      /// <summary>
      /// Retrieve the log and unit info files from the configured Local path
      /// </summary>
      public void RetrievePathInstance()
      {
         DateTime start = HfmTrace.ExecStart;

         try
         {
            var remoteFahLogFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.RemoteFAHLogFilename));
            string fahLogPath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogName);
            var cachedFahLogFileInfo = new FileInfo(fahLogPath);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "FAHlog copy (start)");

            if (remoteFahLogFileInfo.Exists)
            {
               if (cachedFahLogFileInfo.Exists == false || remoteFahLogFileInfo.Length != cachedFahLogFileInfo.Length)
               {
                  remoteFahLogFileInfo.CopyTo(fahLogPath, true);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "FAHlog copy (success)");
               }
               else
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "FAHlog copy (file has not changed)");
               }
            }
            else
            {
               throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
                  "The path {0} is inaccessible.", remoteFahLogFileInfo.FullName));
            }

            // Retrieve unitinfo.txt (or equivalent)
            var unitInfoFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.RemoteUnitInfoFilename));
            string unitInfoPath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoName);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "unitinfo copy (start)");

            if (unitInfoFileInfo.Exists)
            {
               // If file size is too large, do not copy it and delete the current cached copy - Issue 2
               if (unitInfoFileInfo.Length < Constants.UnitInfoMax)
               {
                  unitInfoFileInfo.CopyTo(unitInfoPath, true);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "unitinfo copy (success)");
               }
               else
               {
                  if (File.Exists(unitInfoPath))
                  {
                     File.Delete(unitInfoPath);
                  }
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "unitinfo copy (file is too big: {0} bytes).", unitInfoFileInfo.Length));
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            else
            {
               if (File.Exists(unitInfoPath))
               {
                  File.Delete(unitInfoPath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "The path {0} is inaccessible.", unitInfoFileInfo.FullName));
            }

            // Retrieve queue.dat (or equivalent)
            var queueFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.RemoteQueueFilename));
            string queuePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueName);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "queue copy (start)");

            if (queueFileInfo.Exists)
            {
               queueFileInfo.CopyTo(queuePath, true);
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "queue copy (success)");
            }
            /*** Remove Requirement for Queue to be Present ***/
            else
            {
               if (File.Exists(queuePath))
               {
                  File.Delete(queuePath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "The path {0} is inaccessible.", queueFileInfo.FullName));
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured HTTP location
      /// </summary>
      public void RetrieveHttpInstance()
      {
         DateTime start = HfmTrace.ExecStart;

         var net = new NetworkOps();

         try
         {
            string httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.RemoteFAHLogFilename);
            string localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogName);
            net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);

            try
            {
               httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.RemoteUnitInfoFilename);
               localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoName);

               long length = net.GetHttpDownloadLength(httpPath, Settings.Username, Settings.Password);
               if (length < Constants.UnitInfoMax)
               {
                  net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);
               }
               else
               {
                  if (File.Exists(localFile))
                  {
                     File.Delete(localFile);
                  }

                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "unitinfo download (file is too big: {0} bytes).", length));
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFile))
               {
                  File.Delete(localFile);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "unitinfo download failed: {0}", ex.Message));
            }

            try
            {
               httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.RemoteQueueFilename);
               localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueName);
               net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFile))
               {
                  File.Delete(localFile);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "queue download failed: {0}", ex.Message));
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }

      /// <summary>
      /// Retrieve the log and unit info files from the configured FTP location
      /// </summary>
      public void RetrieveFtpInstance()
      {
         DateTime start = HfmTrace.ExecStart;

         var net = new NetworkOps();

         try
         {
            string localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogName);
            net.FtpDownloadHelper(Settings.Server, Settings.Path, Settings.RemoteFAHLogFilename, localFilePath,
                                  Settings.Username, Settings.Password, Settings.FtpMode);

            try
            {
               localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoName);

               long length = net.GetFtpDownloadLength(Settings.Server, Settings.Path, Settings.RemoteUnitInfoFilename,
                                                      Settings.Username, Settings.Password, Settings.FtpMode);
               if (length < Constants.UnitInfoMax)
               {
                  net.FtpDownloadHelper(Settings.Server, Settings.Path, Settings.RemoteUnitInfoFilename, localFilePath,
                                        Settings.Username, Settings.Password, Settings.FtpMode);
               }
               else
               {
                  if (File.Exists(localFilePath))
                  {
                     File.Delete(localFilePath);
                  }

                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                     "unitinfo download (file is too big: {0} bytes).", length));
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFilePath))
               {
                  File.Delete(localFilePath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "unitinfo download failed: {0}.", ex.Message));
            }

            try
            {
               localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueName);
               net.FtpDownloadHelper(Settings.Server, Settings.Path, Settings.RemoteQueueFilename, localFilePath,
                                     Settings.Username, Settings.Password, Settings.FtpMode);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFilePath))
               {
                  File.Delete(localFilePath);
               }
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, Settings.InstanceName, String.Format(CultureInfo.CurrentCulture,
                  "queue download failed: {0}", ex.Message));
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }   
   }
}
