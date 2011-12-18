/*
 * HFM.NET - Project Summary Downloader Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

namespace HFM.Core
{
   public interface IProjectSummaryDownloader : IProgressProcessRunner
   {
      /// <summary>
      /// Time of Last Successful Download
      /// </summary>
      DateTime LastDownloadTime { get; }

      /// <summary>
      /// Destination Path for the psummary file
      /// </summary>
      string DownloadFilePath { get; set; }

      /// <summary>
      /// Reset the Last Download Time
      /// </summary>
      void ResetLastDownloadTime();

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      void DownloadFromStanford();

      /// <summary>
      /// Download project information from HFM.NET website (THREAD SAFE)
      /// </summary>
      void DownloadFromHfmWeb();
   }

   public sealed class ProjectSummaryDownloader : IProjectSummaryDownloader
   {
      #region Fields
      
      /// <summary>
      /// Collection Load Class Lock
      /// </summary>
      private static readonly object DownloadLock = new object();

      /// <summary>
      /// Time of Last Successful Download
      /// </summary>
      public DateTime LastDownloadTime { get; private set; }

      /// <summary>
      /// Destination Path for the psummary file
      /// </summary>
      public string DownloadFilePath { get; set; }

      /// <summary>
      /// Preferences Interface
      /// </summary>
      public IPreferenceSet Prefs { get; set; }

      private bool _processing;
      bool IProgressProcessRunner.Processing
      {
         get { return _processing; }
      }

      Exception IProgressProcessRunner.Exception
      {
         get { throw new NotImplementedException(); }
      }

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      #endregion

      public ProjectSummaryDownloader()
      {
         LastDownloadTime = DateTime.MinValue;
      }

      #region Events and Event Wrappers

      public event EventHandler<ProgressEventArgs> ProgressChanged;
      private void OnProgressChanged(ProgressEventArgs e)
      {
         if (ProgressChanged != null)
         {
            ProgressChanged(this, e);
         }
      }

      public event EventHandler ProcessFinished;
      private void OnProcessFinished(EventArgs e)
      {
         if (ProcessFinished != null)
         {
            ProcessFinished(this, e);
         }
      }
      
      #endregion

      /// <summary>
      /// Reset the Last Download Time
      /// </summary>
      public void ResetLastDownloadTime()
      {
         LastDownloadTime = DateTime.MinValue;
      }

      void IProgressProcessRunner.Process()
      {
         _processing = true;
         try
         {
            DownloadFromStanford();
         }
         finally
         {
            _processing = false;
            OnProcessFinished(EventArgs.Empty);
         }
      }

      bool IProgressProcessRunner.SupportsCancellation
      {
         get { return false; }
      }

      void IProgressProcessRunner.Cancel()
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public void DownloadFromStanford()
      {
         lock (DownloadLock)
         {
            if (!CheckLastDownloadTime())
            {
               return;
            }

            _logger.Info("Downloading new project data from Stanford...");
            try
            {
               PerformDownload(Prefs.Get<string>(Preference.ProjectDownloadUrl));
               LastDownloadTime = DateTime.Now;
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
               OnProgressChanged(new ProgressEventArgs(0, ex.Message));
            }
         }
      }

      /// <summary>
      /// Download project information from HFM.NET website (THREAD SAFE)
      /// </summary>
      public void DownloadFromHfmWeb()
      {
         lock (DownloadLock)
         {
            if (!CheckLastDownloadTime())
            {
               return;
            }

            _logger.Info("Downloading new project data from HFM website...");
            try
            {
               PerformDownload("http://www.htm-net.com/ProjectInfo/ProjectInfo.xml");
               LastDownloadTime = DateTime.Now;
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
               OnProgressChanged(new ProgressEventArgs(0, ex.Message));
            }
         }
      }

      private void PerformDownload(string downloadUrl)
      {
         Debug.Assert(Prefs != null);

         var net = new NetworkOps(Prefs);
         net.HttpWebOperationProgress += HttpWebOperationProgress;
         net.HttpDownloadHelper(downloadUrl, DownloadFilePath, String.Empty, String.Empty);
      }

      private void HttpWebOperationProgress(object sender, harlam357.Net.WebOperationProgressEventArgs e)
      {
         int progress = Convert.ToInt32((e.Length / (double)e.TotalLength) * 100);
         string message = String.Format(CultureInfo.InvariantCulture, "Downloading {0} of {1} bytes...", e.Length, e.TotalLength);
         OnProgressChanged(new ProgressEventArgs(progress, message));
      }

      private bool CheckLastDownloadTime()
      {
         // if a download was attempted in the last hour, don't execute again
         TimeSpan lastDownloadDifference = DateTime.Now.Subtract(LastDownloadTime);
         if (lastDownloadDifference.TotalHours > 1)
         {
            return true;
         }
         
         _logger.Debug("Download executed {0:0} minutes ago.", lastDownloadDifference.TotalMinutes);
         return false;
      }
   }
}
