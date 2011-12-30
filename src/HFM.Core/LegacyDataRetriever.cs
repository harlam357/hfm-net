/*
 * HFM.NET - Legacy Data Retriever Class
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
using System.Globalization;
using System.IO;
using System.Net;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IDataRetriever
   {
      ClientSettings Settings { get; }

      /// <summary>
      /// Execute Data Retrieve
      /// </summary>
      void Execute(ClientSettings settings);
   }

   public class LegacyDataRetriever : IDataRetriever
   {
      public ClientSettings Settings { get; private set; }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      
      public LegacyDataRetriever(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      /// <summary>
      /// Execute Data Retrieve
      /// </summary>
      public void Execute(ClientSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");

         Settings = settings;

         switch (Settings.LegacyClientSubType)
         {
            case LegacyClientSubType.Path:
               RetrievePathInstance();
               break;
            case LegacyClientSubType.Http:
               RetrieveHttpInstance();
               break;
            case LegacyClientSubType.Ftp:
               RetrieveFtpInstance();
               break;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Instance Type '{0}' is not implemented", Settings.LegacyClientSubType));
         }
      }

      private void RetrievePathInstance()
      {
         //DateTime start = Instrumentation.ExecStart;

         //try
         //{
            var remoteFahLogFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.FahLogFileName));
            string fahLogPath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogFileName());
            var cachedFahLogFileInfo = new FileInfo(fahLogPath);

            _logger.Debug(Constants.ClientNameFormat, Settings.Name, "FAHlog copy (start)");

            if (remoteFahLogFileInfo.Exists)
            {
               if (cachedFahLogFileInfo.Exists == false || remoteFahLogFileInfo.Length != cachedFahLogFileInfo.Length)
               {
                  remoteFahLogFileInfo.CopyTo(fahLogPath, true);
                  _logger.Debug(Constants.ClientNameFormat, Settings.Name, "FAHlog copy (success)");
               }
               else
               {
                  _logger.Debug(Constants.ClientNameFormat, Settings.Name, "FAHlog copy (file has not changed)");
               }
            }
            else
            {
               throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture,
                  "The path {0} is inaccessible.", remoteFahLogFileInfo.FullName));
            }

            // Retrieve unitinfo.txt (or equivalent)
            var unitInfoFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.UnitInfoFileName));
            string unitInfoPath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoFileName());

            _logger.Debug(Constants.ClientNameFormat, Settings.Name, "unitinfo copy (start)");

            if (unitInfoFileInfo.Exists)
            {
               // If file size is too large, do not copy it and delete the current cached copy - Issue 2
               if (unitInfoFileInfo.Length < Constants.UnitInfoMax)
               {
                  unitInfoFileInfo.CopyTo(unitInfoPath, true);
                  _logger.Debug(Constants.ClientNameFormat, Settings.Name, "unitinfo copy (success)");
               }
               else
               {
                  if (File.Exists(unitInfoPath))
                  {
                     File.Delete(unitInfoPath);
                  }
                  string message = String.Format(CultureInfo.CurrentCulture, "unitinfo copy (file is too big: {0} bytes).", unitInfoFileInfo.Length);
                  _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            else
            {
               if (File.Exists(unitInfoPath))
               {
                  File.Delete(unitInfoPath);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "The path {0} is inaccessible.", unitInfoFileInfo.FullName);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }

            // Retrieve queue.dat (or equivalent)
            var queueFileInfo = new FileInfo(Path.Combine(Settings.Path, Settings.QueueFileName));
            string queuePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueFileName());

            _logger.Debug(Constants.ClientNameFormat, Settings.Name, "queue copy (start)");

            if (queueFileInfo.Exists)
            {
               queueFileInfo.CopyTo(queuePath, true);
               _logger.Debug(Constants.ClientNameFormat, Settings.Name, "queue copy (success)");
            }
            /*** Remove Requirement for Queue to be Present ***/
            else
            {
               if (File.Exists(queuePath))
               {
                  File.Delete(queuePath);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "The path {0} is inaccessible.", queueFileInfo.FullName);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }
         //}
         //finally
         //{
         //   _logger.Info(Constants.ClientNameFormat, Settings.Name, Instrumentation.GetExecTime(start));
         //}
      }

      private void RetrieveHttpInstance()
      {
         var net = new NetworkOps(_prefs);

         //DateTime start = Instrumentation.ExecStart;

         //try
         //{
            string httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.FahLogFileName);
            string localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogFileName());
            net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);

            try
            {
               httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.UnitInfoFileName);
               localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoFileName());

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

                  string message = String.Format(CultureInfo.CurrentCulture, "unitinfo download (file is too big: {0} bytes).", length);
                  _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFile))
               {
                  File.Delete(localFile);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "unitinfo download failed: {0}", ex.Message);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }

            try
            {
               httpPath = String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Settings.Path, "/", Settings.QueueFileName);
               localFile = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueFileName());
               net.HttpDownloadHelper(httpPath, localFile, Settings.Username, Settings.Password);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFile))
               {
                  File.Delete(localFile);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "queue download failed: {0}", ex.Message);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }
         //}
         //finally
         //{
         //   _logger.Info(Constants.ClientNameFormat, Settings.Name, Instrumentation.GetExecTime(start));
         //}
      }

      private void RetrieveFtpInstance()
      {
         var net = new NetworkOps(_prefs);

         //DateTime start = Instrumentation.ExecStart;

         //try
         //{
            string localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedFahLogFileName());
            net.FtpDownloadHelper(Settings.Server, Settings.Port, Settings.Path, Settings.FahLogFileName, localFilePath,
                                  Settings.Username, Settings.Password, Settings.FtpMode);

            try
            {
               localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedUnitInfoFileName());

               long length = net.GetFtpDownloadLength(Settings.Server, Settings.Path, Settings.UnitInfoFileName,
                                                      Settings.Username, Settings.Password, Settings.FtpMode);
               if (length < Constants.UnitInfoMax)
               {
                  net.FtpDownloadHelper(Settings.Server, Settings.Port, Settings.Path, Settings.UnitInfoFileName, localFilePath,
                                        Settings.Username, Settings.Password, Settings.FtpMode);
               }
               else
               {
                  if (File.Exists(localFilePath))
                  {
                     File.Delete(localFilePath);
                  }

                  string message = String.Format(CultureInfo.CurrentCulture, "unitinfo download (file is too big: {0} bytes).", length);
                  _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
               }
            }
            /*** Remove Requirement for UnitInfo to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFilePath))
               {
                  File.Delete(localFilePath);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "unitinfo download failed: {0}.", ex.Message);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }

            try
            {
               localFilePath = Path.Combine(_prefs.CacheDirectory, Settings.CachedQueueFileName());
               net.FtpDownloadHelper(Settings.Server, Settings.Port, Settings.Path, Settings.QueueFileName, localFilePath,
                                     Settings.Username, Settings.Password, Settings.FtpMode);
            }
            /*** Remove Requirement for Queue to be Present ***/
            catch (WebException ex)
            {
               if (File.Exists(localFilePath))
               {
                  File.Delete(localFilePath);
               }
               string message = String.Format(CultureInfo.CurrentCulture, "queue download failed: {0}", ex.Message);
               _logger.Warn(Constants.ClientNameFormat, Settings.Name, message);
            }
         //}
         //finally
         //{
         //   _logger.Info(Constants.ClientNameFormat, Settings.Name, Instrumentation.GetExecTime(start));
         //}
      }   
   }
}
