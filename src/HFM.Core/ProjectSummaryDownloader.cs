/*
 * HFM.NET - Project Summary Downloader Class
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.Net.Cache;
using System.Threading.Tasks;

using Castle.Core.Logging;

using harlam357.Core;
using harlam357.Core.Net;

using HFM.Preferences;

namespace HFM.Core
{
   public interface IProjectSummaryDownloader
   {
      /// <summary>
      /// Gets the destination path for the psummary file.
      /// </summary>
      string FilePath { get; }

      /// <summary>
      /// Resets the data that halts redundant download calls.
      /// </summary>
      void ResetDownloadParameters();

      /// <summary>
      /// Downloads the project information.
      /// </summary>
      void Download();

      /// <summary>
      /// Downloads the project information.
      /// </summary>
      void Download(IProgress<ProgressInfo> progress);
   }

   public sealed class ProjectSummaryDownloader : IProjectSummaryDownloader
   {
      #region Properties

      /// <summary>
      /// Gets the time of last successful download.
      /// </summary>
      public DateTime LastDownloadTime { get; private set; }

      /// <summary>
      /// Gets or sets the destination path for the psummary file.
      /// </summary>
      public string FilePath { get; set; }

      /// <summary>
      /// Gets or sets the preferences service instance.
      /// </summary>
      public IPreferenceSet Prefs { get; set; }

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      #endregion

      public ProjectSummaryDownloader()
      {
         ResetDownloadParameters();
      }

      /// <summary>
      /// Resets the data that halts redundant download calls.
      /// </summary>
      public void ResetDownloadParameters()
      {
         LastDownloadTime = DateTime.MinValue;
      }

      private static readonly object DownloadSyncRoot = new object();

      /// <summary>
      /// Downloads the project information.
      /// </summary>
      /// <remarks>Access to the Download method is synchronized.</remarks>
      public void Download()
      {
         Download(null);
      }

      /// <summary>
      /// Downloads the project information.
      /// </summary>
      /// <remarks>Access to the Download method is synchronized.</remarks>
      public void Download(IProgress<ProgressInfo> progress)
      {
         Debug.Assert(Prefs != null);

         lock (DownloadSyncRoot)
         {
            if (!CheckLastDownloadTime())
            {
               return;
            }

            Logger.Info("Downloading new project data from Stanford...");

            IWebOperation httpWebOperation = WebOperation.Create(Prefs.Get<string>(Preference.ProjectDownloadUrl));
            if (progress != null)
            {
               httpWebOperation.ProgressChanged += (sender, e) =>
               {
                  int progressPercentage = Convert.ToInt32(e.Length / (double)e.TotalLength * 100);
                  string message = String.Format(CultureInfo.InvariantCulture, "Downloading {0} of {1} bytes...", e.Length, e.TotalLength);
                  progress.Report(new ProgressInfo(progressPercentage, message));
               };
            }
            httpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            httpWebOperation.WebRequest.Proxy = Prefs.GetWebProxy();
            httpWebOperation.Download(FilePath);

            LastDownloadTime = DateTime.Now;
         }
      }

      private bool CheckLastDownloadTime()
      {
         // if a download was attempted in the last hour, don't execute again
         TimeSpan lastDownloadDifference = DateTime.Now.Subtract(LastDownloadTime);
         if (lastDownloadDifference.TotalHours > 1)
         {
            return true;
         }
         
         Logger.DebugFormat("Download executed {0:0} minutes ago.", lastDownloadDifference.TotalMinutes);
         return false;
      }
   }
}
