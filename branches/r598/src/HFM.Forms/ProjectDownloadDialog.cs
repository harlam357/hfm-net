/*
 * HFM.NET - Project Download Dialog
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

using harlam357.Windows.Forms;

using HFM.Core;

namespace HFM.Forms
{
   [CoverageExclude]
   public partial class ProjectDownloadDialog : ProgressDialog
   {
      private readonly System.Timers.Timer _timer;

      public ProjectDownloadDialog()
      {
         _timer = new System.Timers.Timer(2000);
         _timer.Elapsed += TimerElapsed;
         
         InitializeComponent();
      }

      protected override void ProcessRunnerProcessFinished(object sender, EventArgs e)
      {
         ProcessRunner.ProgressChanged -= base.ProcessRunnerProgressChanged;
         ProcessRunner.ProcessFinished -= base.ProcessRunnerProcessFinished;
      
         DoDelayedClose();
      }

      private void DoDelayedClose()
      {
         if (Visible)
         {
            _timer.Start();
         }
      }

      private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
         _timer.Stop();
         Close();
      }
   }
}
