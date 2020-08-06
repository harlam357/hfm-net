using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using HFM.Forms.Controls;

namespace HFM.Forms.Views
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
            lblVersion.Text = $"Version {Core.Application.FullVersion}";
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
