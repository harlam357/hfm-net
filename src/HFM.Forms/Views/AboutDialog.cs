﻿using System.Globalization;
using System.Reflection;

using HFM.Core.Logging;
using HFM.Forms.Services;
using HFM.Forms.Controls;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class AboutDialog : FormBase
    {
        public ILogger Logger { get; }
        public MessageBoxPresenter MessageBox { get; }
        public LocalProcessService LocalProcess { get; }

        public AboutDialog(ILogger logger, MessageBoxPresenter messageBox, LocalProcessService localProcess)
        {
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            LocalProcess = localProcess ?? NullLocalProcessService.Instance;

            InitializeComponent();
            EscapeKeyReturnsCancelDialogResult();

            SetVersionLabelText();
            SetBuildDateLabelText();
            SetProjectSiteLinkText();
            SetSupportForumLinkText();
        }

        private void SetVersionLabelText()
        {
            versionLabel.Text = $"Version {Core.Application.Version}";
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
                lblDate.Text += Environment.NewLine;
                lblDate.Text += Environment.NewLine;
                lblDate.Text += System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;
            }
            else
            {
                lblDate.Text = String.Empty;
            }
        }

        private void SetProjectSiteLinkText()
        {
            googleCodeLinkLabel.Text = Core.Application.ProjectSiteUrl;
        }

        private void SetSupportForumLinkText()
        {
            googleGroupLinkLabel.Text = Core.Application.SupportForumUrl;
        }

        private void ProjectSiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LocalProcess.Start(Core.Application.ProjectSiteUrl);
            }
            catch (Exception)
            {
                string text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, $"{Core.Application.Name} GitHub Project");
                MessageBox.ShowError(this, text, Core.Application.NameAndVersion);
            }
        }

        private void SupportForumLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LocalProcess.Start(Core.Application.SupportForumUrl);
            }
            catch (Exception)
            {
                string text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, $"{Core.Application.Name} Google Group");
                MessageBox.ShowError(this, text, Core.Application.NameAndVersion);
            }
        }
    }
}
