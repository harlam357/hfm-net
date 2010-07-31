/*
 * HFM.NET - Project Download Form Class
 * Copyright (C) 2010 Ryan Harlamert (harlam357)
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
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmProjects : Classes.FormWrapper, IProjectDownloadView
   // ReSharper restore InconsistentNaming
   {
      private readonly frmMain _mainView;
      private readonly IProteinCollection _proteinCollection;

      private readonly System.Timers.Timer _timer;

      public frmProjects(frmMain mainView, IProteinCollection proteinCollection)
      {
         _mainView = mainView;
         _proteinCollection = proteinCollection;

         _timer = new System.Timers.Timer(2000);
         _timer.Elapsed += TimerElapsed;
         
         InitializeComponent();
      }

      #region IProjectDownloadView Members

      public void Download()
      {
         UpdateText(_proteinCollection.Downloader.Prefs.GetPreference<string>(Preference.ProjectDownloadUrl));
         _proteinCollection.Downloader.DownloadProgress += DownloaderProgress;
         _proteinCollection.Downloader.ProjectDownloadFinished += DownloaderProjectDownloadFinished;
         _proteinCollection.BeginDownloadFromStanford();
         ShowDialog(_mainView);
      }

      private void DownloaderProgress(object sender, DownloadProgressEventArgs e)
      {
         UpdateProgress(e.Progress);
         UpdateText(e.ProtienName);
      }

      private void DownloaderProjectDownloadFinished(object sender, EventArgs e)
      {
         _proteinCollection.Downloader.DownloadProgress -= DownloaderProgress;
         _proteinCollection.Downloader.ProjectDownloadFinished -= DownloaderProjectDownloadFinished;

         DoDelayedClose();
      }

      #endregion
      
      private void DoDelayedClose()
      {
         if (Visible)
         {
            _timer.Start();
         }
      }

      private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
         CloseView();
      }

      private void CloseView()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(Close));
            return;
         }

         Close();
      }

      private void frmProjects_FormClosing(object sender, FormClosingEventArgs e)
      {
         _timer.Stop();
      }
      
      private void UpdateProgress(int value)
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(() => UpdateProgress(value)));
            return;
         }

         progressBar1.Value = value;
      }
      
      private void UpdateText(string text)
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(() => UpdateText(text)));
            return;
         }

         lblProject.Text = text; 
      }
   }
}
