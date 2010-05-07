/*
 * HFM.NET - About Form
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmAbout : Classes.FormWrapper
   // ReSharper restore InconsistentNaming
   {
      #region Constructor
      public frmAbout()
      {
         InitializeComponent();
         lblVersion.Text = PlatformOps.LongFormattedApplicationVersionWithRevision;
         string assemblyLocation = Assembly.GetExecutingAssembly().Location;
         if (String.IsNullOrEmpty(assemblyLocation) == false)
         {
            DateTime buildDate = new FileInfo(assemblyLocation).LastWriteTime;
            lblDate.Text = "Built on: " + buildDate.ToLongDateString();
         }
         else
         {
            lblDate.Text = String.Empty;
         }
      } 
      #endregion

      private void lnkHfmGoogleCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start("http://code.google.com/p/hfm-net/");
         }
         catch (Exception)
         {
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
               Properties.Resources.ProcessStartError, "HFM.NET Google Code page"));
         }
      }

      private void lnkHfmGoogleGroup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start("http://groups.google.com/group/hfm-net/");
         }
         catch (Exception)
         {
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, 
               Properties.Resources.ProcessStartError, "HFM.NET Google Group"));
         }
      }

      private void btnClose_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }
   }
}
