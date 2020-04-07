/*
 * HFM.NET - About Dialog
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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using HFM.Core;
using HFM.Forms.Controls;

namespace HFM.Forms
{
    public partial class AboutDialog : FormWrapper
    {
        public AboutDialog()
        {
            InitializeComponent();

            SetVersionLabelText();
            SetBuildDateLabelText();
            SetProjectSiteLinkText();
            SetSupportForumLinkText();
        }

        private void SetVersionLabelText()
        {
            lblVersion.Text = $"Version {Core.Application.VersionWithRevision}";
        }

        private void SetBuildDateLabelText()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (String.IsNullOrEmpty(assemblyLocation) == false)
            {
                DateTime buildDate = new FileInfo(assemblyLocation).LastWriteTime;
                // When localizing use ToLongDateString() instead.
                //lblDate.Text = "Built on: " + buildDate.ToLongDateString();
                lblDate.Text = "Built on: " + buildDate.ToString("D", CultureInfo.InvariantCulture);
            }
            else
            {
                lblDate.Text = String.Empty;
            }
        }

        private void SetProjectSiteLinkText()
        {
            lnkHfmGoogleCode.Text = Core.Application.ProjectSiteUrl;
        }

        private void SetSupportForumLinkText()
        {
            lnkHfmGoogleGroup.Text = Core.Application.SupportForumUrl;
        }

        private void ProjectSiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(Core.Application.ProjectSiteUrl);
            }
            catch (Exception)
            {
                MessageBox.Show(String.Format(CultureInfo.CurrentCulture,
                   Properties.Resources.ProcessStartError, "HFM.NET GitHub Project"));
            }
        }

        private void SupportForumLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(Core.Application.SupportForumUrl);
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
