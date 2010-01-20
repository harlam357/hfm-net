/*
 * HFM.NET - About Form
 * Copyright (C) 2006-2007 David Rawling
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
using System.Globalization;
using System.Windows.Forms;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Forms
{
   public partial class frmAbout : Classes.FormWrapper
   {
      #region Constructor
      public frmAbout()
      {
         InitializeComponent();

         lblVersion.Text = PlatformOps.LongFormattedApplicationVersionWithRevision;
      } 
      #endregion

      #region Event Handlers
      private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            // Issue 28 - wrap process start in try catch
            Process.Start("http://code.google.com/p/fahlogstats-net/");
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "FAHLogStats.NET Google Code page"));
         }
      } 
      #endregion
   }
}
