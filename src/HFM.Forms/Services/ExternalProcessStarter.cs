/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Castle.Core.Logging;

using HFM.Core;
using HFM.Preferences;

namespace HFM.Forms
{
   public interface IExternalProcessStarter
   {
      /// <summary>
      /// Show the HFM.NET log file
      /// </summary>
      string ShowHfmLogFile();

      /// <summary>
      /// Show the given cached log file
      /// </summary>
      /// <param name="logFilePath">Path to cached log file</param>
      string ShowCachedLogFile(string logFilePath);

      /// <summary>
      /// Show the given path in the file explorer
      /// </summary>
      /// <param name="path">Path to explore</param>
      string ShowFileExplorer(string path);

      /// <summary>
      /// Show the given URL in the default web browser
      /// </summary>
      /// <param name="url">Website URL</param>
      string ShowWebBrowser(string url);

      /// <summary>
      /// Show the HFM Google Code page
      /// </summary>
      string ShowHfmGitHub();

      /// <summary>
      /// Show the HFM Google Group
      /// </summary>
      string ShowHfmGoogleGroup();

      /// <summary>
      /// Show the configured EOC User Stats page
      /// </summary>
      string ShowEocUserPage();

      /// <summary>
      /// Show the configured EOC Team Stats page
      /// </summary>
      string ShowEocTeamPage();

      /// <summary>
      /// Show the configured Stanford User Stats page
      /// </summary>
      string ShowStanfordUserPage();
   }

   [ExcludeFromCodeCoverage]
   public sealed class ExternalProcessStarter : IExternalProcessStarter
   {
      private readonly IPreferenceSet _prefs;
      private readonly ILogger _logger;

      public ExternalProcessStarter(IPreferenceSet prefs, ILogger logger)
      {
         _prefs = prefs;
         _logger = logger;
      }

      /// <summary>
      /// Show the HFM.NET log file
      /// </summary>
      public string ShowHfmLogFile()
      {
         string logFilePath = Path.Combine(_prefs.Get<string>(Preference.ApplicationDataFolderPath), Core.Logging.Logger.LogFileName);
         string errorMessage = String.Format(CultureInfo.CurrentCulture,
               "An error occured while attempting to show the HFM.log file.{0}{0}Please check the current Log File Viewer defined in the Preferences.",
               Environment.NewLine);
         return RunProcess(_prefs.Get<string>(Preference.LogFileViewer), logFilePath, errorMessage);
      }

      /// <summary>
      /// Show the given cached log file
      /// </summary>
      /// <param name="logFilePath">Path to cached log file</param>
      public string ShowCachedLogFile(string logFilePath)
      {
         string errorMessage = String.Format(CultureInfo.CurrentCulture,
               "An error occured while attempting to show the FAHlog.txt file.{0}{0}Please check the current Log File Viewer defined in the Preferences.",
               Environment.NewLine);
         return RunProcess(_prefs.Get<string>(Preference.LogFileViewer), logFilePath, errorMessage);
      }

      /// <summary>
      /// Show the given path in the file explorer
      /// </summary>
      /// <param name="path">Path to explore</param>
      public string ShowFileExplorer(string path)
      {
         string errorMessage = String.Format(CultureInfo.CurrentCulture,
               "An error occured while attempting to show '{0}'.{1}{1}Please check the current File Explorer defined in the Preferences.",
               path, Environment.NewLine);
         return RunProcess(_prefs.Get<string>(Preference.FileExplorer), path, errorMessage);
      }

      /// <summary>
      /// Show the given URL in the default web browser
      /// </summary>
      /// <param name="url">Website URL</param>
      public string ShowWebBrowser(string url)
      {
         string errorMessage = String.Format(CultureInfo.CurrentCulture,
               "An error occured while attempting to show '{0}'.", url);
         return RunProcess(url, null, errorMessage);
      }

      /// <summary>
      /// Show the HFM Google Code page
      /// </summary>
      public string ShowHfmGitHub()
      {
         const string errorMessage = "An error occured while attempting to show the HFM.NET GitHub Project.";
         return RunProcess(Application.GitHubUrl, null, errorMessage);
      }

      /// <summary>
      /// Show the HFM Google Group
      /// </summary>
      public string ShowHfmGoogleGroup()
      {
         const string errorMessage = "An error occured while attempting to show the HFM.NET Google Group.";
         return RunProcess(Application.GoogleGroupUrl, null, errorMessage);
      }

      private Uri EocUserUrl
      {
         get { return new Uri(String.Concat(EocUrl.UserBaseUrl, _prefs.Get<int>(Preference.EocUserId))); }
      }

      /// <summary>
      /// Show the configured EOC User Stats page
      /// </summary>
      public string ShowEocUserPage()
      {
         const string errorMessage = "An error occured while attempting to show the EOC User Stats page.";
         return RunProcess(EocUserUrl.AbsoluteUri, null, errorMessage);
      }

      private Uri EocTeamUrl
      {
         get { return new Uri(String.Concat(EocUrl.TeamBaseUrl, _prefs.Get<int>(Preference.TeamId))); }
      }

      /// <summary>
      /// Show the configured EOC Team Stats page
      /// </summary>
      public string ShowEocTeamPage()
      {
         const string errorMessage = "An error occured while attempting to show the EOC Team Stats page.";
         return RunProcess(EocTeamUrl.AbsoluteUri, null, errorMessage);
      }

      private Uri StanfordUserUrl
      {
         get { return new Uri(String.Concat(FahUrl.UserBaseUrl, _prefs.Get<string>(Preference.StanfordId))); }
      }

      /// <summary>
      /// Show the configured Stanford User Stats page
      /// </summary>
      public string ShowStanfordUserPage()
      {
         const string errorMessage = "An error occured while attempting to show the Stanford User Stats page.";
         return RunProcess(StanfordUserUrl.AbsoluteUri, null, errorMessage);
      }

      private string RunProcess(string fileName, string arguments, string errorMessage)
      {
         try
         {
            Process.Start(fileName, PrepArguments(arguments));
            return null;
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
            return errorMessage;
         }
      }

      private static string PrepArguments(string arguments)
      {
         return Application.IsRunningOnMono && arguments != null ? String.Format(CultureInfo.InvariantCulture, "\"{0}\"", arguments) : arguments;
      }
   }
}
