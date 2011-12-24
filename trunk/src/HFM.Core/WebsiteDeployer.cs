/*
 * HFM.NET - Website Deployer Class
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using Castle.Core.Logging;

namespace HFM.Core
{
   public interface IWebsiteDeployer
   {
      void DeployWebsite(IEnumerable<string> htmlFilePaths, IEnumerable<string> xmlFilePaths, IEnumerable<SlotModel> slots);
   }

   public class WebsiteDeployer : IWebsiteDeployer
   {
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      private readonly INetworkOps _networkOps;
      
      public WebsiteDeployer(IPreferenceSet prefs, INetworkOps networkOps)
      {
         _prefs = prefs;
         _networkOps = networkOps;
      }
   
      public void DeployWebsite(IEnumerable<string> htmlFilePaths, IEnumerable<string> xmlFilePaths, IEnumerable<SlotModel> slots)
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));

         var copyHtml = _prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
         var copyXml = _prefs.GetPreference<bool>(Preference.WebGenCopyXml);
         //var copyClientData = _prefs.GetPreference<bool>(Preference.WebGenCopyClientData);

         Match match = Validate.MatchFtpWithUserPassUrl(_prefs.GetPreference<string>(Preference.WebRoot));
         if (match.Success)
         {
            string server = match.Result("${domain}");
            string ftpPath = match.Result("${file}");
            string username = match.Result("${username}");
            string password = match.Result("${password}");

            if (copyHtml && htmlFilePaths != null)
            {
               FtpHtmlUpload(server, ftpPath, username, password, htmlFilePaths, slots);
            }
            if (copyXml && xmlFilePaths != null)
            {
               FtpXmlUpload(server, ftpPath, username, password, xmlFilePaths);
            }
            //if (copyClientData && clientDataFilePath != null)
            //{
            //   // Issue 79
            //   _networkOps.FtpUploadHelper(server, ftpPath, clientDataFilePath, username, password, _prefs.GetPreference<FtpType>(Preference.WebGenFtpMode));
            //}
         }
         else
         {
            var webRoot = _prefs.GetPreference<string>(Preference.WebRoot);
            var cssFile = _prefs.GetPreference<string>(Preference.CssFile);

            // Create the web folder (just in case)
            if (Directory.Exists(webRoot) == false)
            {
               Directory.CreateDirectory(webRoot);
            }

            if (copyHtml && htmlFilePaths != null)
            {
               // Copy the CSS file to the output directory
               string cssFilePath = Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName), cssFile);
               if (File.Exists(cssFilePath))
               {
                  File.Copy(cssFilePath, Path.Combine(webRoot, cssFile), true);
               }

               foreach (string filePath in htmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }

               if (_prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
               {
                  foreach (var slot in slots)
                  {
                     string cachedFahlogPath = Path.Combine(_prefs.CacheDirectory, slot.Settings.CachedFahLogFileName());
                     if (File.Exists(cachedFahlogPath))
                     {
                        File.Copy(cachedFahlogPath, Path.Combine(webRoot, slot.Settings.CachedFahLogFileName()), true);
                     }
                  }
               }
            }
            if (copyXml && xmlFilePaths != null)
            {
               foreach (string filePath in xmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }
            }
            //if (copyClientData && clientDataFilePath != null)
            //{
            //   // Issue 79
            //   File.Copy(clientDataFilePath, Path.Combine(webRoot, Path.GetFileName(clientDataFilePath)), true);
            //}
         }
      }

      /// <summary>
      /// Upload Web Site Files.
      /// </summary>
      /// <param name="server">Server Name</param>
      /// <param name="ftpPath">Path from FTP Server Root</param>
      /// <param name="username">FTP Server Username</param>
      /// <param name="password">FTP Server Password</param>
      /// <param name="htmlFilePaths">HTML File Paths</param>
      /// <param name="slots">Client Instance Collection</param>
      private void FtpHtmlUpload(string server, string ftpPath, string username, string password, IEnumerable<string> htmlFilePaths, IEnumerable<SlotModel> slots)
      {
         // Time FTP Upload Conversation - Issue 52
         DateTime start = Instrumentation.ExecStart;

         try
         {
            // Get the FTP Type
            var ftpMode = _prefs.GetPreference<FtpType>(Preference.WebGenFtpMode);

            // Upload CSS File
            _networkOps.FtpUploadHelper(server, ftpPath, Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName),
               _prefs.GetPreference<string>(Preference.CssFile)), username, password, ftpMode);

            // Upload each HTML File
            foreach (string filePath in htmlFilePaths)
            {
               _networkOps.FtpUploadHelper(server, ftpPath, filePath, username, password, ftpMode);
            }

            if (_prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
            {
               int maximumLength = _prefs.GetPreference<bool>(Preference.WebGenLimitLogSize)
                                    ? _prefs.GetPreference<int>(Preference.WebGenLimitLogSizeLength) * 1024
                                    : -1;
               // Upload the FAHlog.txt File for each Client Instance
               foreach (var slot in slots)
               {
                  string cachedFahlogPath = Path.Combine(_prefs.CacheDirectory, slot.Settings.CachedFahLogFileName());
                  if (File.Exists(cachedFahlogPath))
                  {
                     _networkOps.FtpUploadHelper(server, ftpPath, cachedFahlogPath, maximumLength, username, password, ftpMode);
                  }
               }
            }
         }
         finally
         {
            // Time FTP Upload Conversation - Issue 52
            Logger.Info("HTML upload finished in {0}", Instrumentation.GetExecTime(start));
         }
      }

      /// <summary>
      /// Upload XML Files.
      /// </summary>
      /// <param name="server">Server Name</param>
      /// <param name="ftpPath">Path from FTP Server Root</param>
      /// <param name="username">FTP Server Username</param>
      /// <param name="password">FTP Server Password</param>
      /// <param name="xmlFilePaths">XML File Paths</param>
      private void FtpXmlUpload(string server, string ftpPath, string username, string password, IEnumerable<string> xmlFilePaths)
      {
         // Time FTP Upload Conversation - Issue 52
         DateTime start = Instrumentation.ExecStart;

         try
         {
            // Get the FTP Type
            var ftpMode = _prefs.GetPreference<FtpType>(Preference.WebGenFtpMode);

            // Upload each XML File
            foreach (string filePath in xmlFilePaths)
            {
               _networkOps.FtpUploadHelper(server, ftpPath, filePath, username, password, ftpMode);
            }
         }
         finally
         {
            // Time FTP Upload Conversation - Issue 52
            Logger.Info("XML upload finished in {0}", Instrumentation.GetExecTime(start));
         }
      }
   }
}
