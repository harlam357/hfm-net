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

using HFM.Forms.Controls;
using HFM.Framework;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmAbout : FormWrapper
   // ReSharper restore InconsistentNaming
   {
      #region Constructor
      public frmAbout()
      {
         InitializeComponent();
         int[] versions = PlatformOps.GetVersionNumbers();
         Debug.Assert(versions.Length == 4);
         lblVersion.Text = String.Format(CultureInfo.InvariantCulture, "Version {0}.{1}.{2} - Revision {3}",
                                         versions[0], versions[1], versions[2], versions[3]);
         string assemblyLocation = Assembly.GetExecutingAssembly().Location;
         if (String.IsNullOrEmpty(assemblyLocation) == false)
         {
            DateTime buildDate = new FileInfo(assemblyLocation).LastWriteTime;
            // TODO: When localizing use ToLongDateString() instead.
            //lblDate.Text = "Built on: " + buildDate.ToLongDateString();
            lblDate.Text = "Built on: " + buildDate.ToString("D", CultureInfo.InvariantCulture);
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
