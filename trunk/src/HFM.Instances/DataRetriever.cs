/*
 * HFM.NET - Data Retriever Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public interface IDataRetriever
   {
      IClientInstanceSettings Settings { get; }

      /// <summary>
      /// Execute Data Retrieve
      /// </summary>
      void Execute(IClientInstanceSettings settings);
   }

   public class DataRetriever : IDataRetriever
   {
      public IClientInstanceSettings Settings { get; private set; }

      private readonly IPreferenceSet _prefs;
      
      public DataRetriever(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      /// <summary>
      /// Execute Data Retrieve
      /// </summary>
      public void Execute(IClientInstanceSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");

         Settings = settings;

         switch (Settings.InstanceHostType)
         {
            case InstanceType.PathInstance:
               RetrievePathInstance();
               break;
            case InstanceType.HttpInstance:
               RetrieveHttpInstance();
               break;
            case InstanceType.FtpInstance:
               RetrieveFtpInstance();
               break;
            case InstanceType.Version7:
               // No Version 7 Retrieval Logic
               break;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Instance Type '{0}' is not implemented", Settings.InstanceHostType));
         }
      }

      private void RetrievePathInstance()
      {
         if (Settings.ExternalInstance)
         {
            RetrieveExternalPathInstance();
            return;
         }
      
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

      private void RetrieveExternalPathInstance()
      {
         DateTime start = HfmTrace.ExecStart;

         try
         {
            var fileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.RemoteExternalFilename));
            string filePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedExternalName);

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "External Data copy (start)");

            if (fileInfo.Exists)
            {
               fileInfo.CopyTo(filePath, true);
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Settings.InstanceName, "External Data copy (success)");
            }
            else
            {
               throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
                  "The path {0} is inaccessible.", fileInfo.FullName));
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }

      private void RetrieveHttpInstance()
      {
         if (Settings.ExternalInstance)
         {
            RetrieveExternalHttpInstance();
            return;
         }
      
         DateTime start = HfmTrace.ExecStart;

         var net = new NetworkOps(_prefs);

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

      private void RetrieveExternalHttpInstance()
      {
         DateTime start = HfmTrace.ExecStart;
         
         var net = new NetworkOps(_prefs);

         try
         {
            string httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.RemoteExternalFilename);
            string localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedExternalName);
            net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }

      private void RetrieveFtpInstance()
      {
         if (Settings.ExternalInstance)
         {
            RetrieveExternalFtpInstance();
            return;
         }
      
         DateTime start = HfmTrace.ExecStart;

         var net = new NetworkOps(_prefs);

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

      private void RetrieveExternalFtpInstance()
      {
         DateTime start = HfmTrace.ExecStart;

         var net = new NetworkOps(_prefs);

         try
         {
            string localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedExternalName);
            net.FtpDownloadHelper(Settings.Server, Settings.Path, Settings.RemoteExternalFilename, localFilePath,
                                  Settings.Username, Settings.Password, Settings.FtpMode);
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Settings.InstanceName, start);
         }
      }
   }
}
