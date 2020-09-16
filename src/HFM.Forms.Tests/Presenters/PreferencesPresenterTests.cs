using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Core;
using HFM.Core.Mocks;
using HFM.Core.Net;
using HFM.Core.Services;
using HFM.Core.SlotXml;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;
using HFM.Preferences;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class PreferencesPresenterTests
    {
        [Test]
        public void PreferencesPresenter_OKClicked_DoesNotCloseWhenModelHasError()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                presenter.ShowDialog(null);
                presenter.Model.WebProxyModel.Enabled = true;
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.OKClicked();
                // Assert
                Assert.IsTrue(presenter.MockDialog.Shown);
            }
        }

        [Test]
        public void PreferencesPresenter_OKClicked_AttemptsSaveAndShowsExceptionOnFailure()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModelThrowsOnSave()))
            {
                presenter.ShowDialog(null);
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.OKClicked();
                // Assert
                Assert.AreEqual(DialogResult.Ignore, presenter.Dialog.DialogResult);
                Assert.IsFalse(presenter.MockDialog.Shown);
            }
        }

        [Test]
        public void PreferencesPresenter_OKClicked_SetsDialogResultAndClosesDialogWhenModelHasNoError()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                presenter.ShowDialog(null);
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.OKClicked();
                // Assert
                Assert.AreEqual(DialogResult.OK, presenter.Dialog.DialogResult);
                Assert.IsFalse(presenter.MockDialog.Shown);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForWebGenerationPath_SetsFolderDialogSelectedPathWhenModelPathIsSet()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                presenter.Model.WebGenerationModel.Path = @"foo\";
                var dialog = new MockFolderDialogPresenter(_ => default);
                // Act
                presenter.BrowseForWebGenerationPath(dialog);
                // Assert
                Assert.AreEqual(@"foo\", dialog.SelectedPath);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForWebGenerationPath_DoesNotSetFolderDialogSelectedPathWhenModelPathIsNotSet()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFolderDialogPresenter(_ => default);
                // Act
                presenter.BrowseForWebGenerationPath(dialog);
                // Assert
                Assert.IsNull(dialog.SelectedPath);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForWebGenerationPath_SetsModelPathWhenDialogResultIsOK()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFolderDialogPresenter(_ => DialogResult.OK);
                dialog.SelectedPath = @"foo\";
                // Act
                presenter.BrowseForWebGenerationPath(dialog);
                // Assert
                Assert.AreEqual(@"foo\", presenter.Model.WebGenerationModel.Path);
            }
        }

        [Test]
        public void PreferencesPresenter_TestReportingEmail_ShowsMessageBoxWhenModelHasError()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.Enabled = true;
                // Act
                presenter.TestReportingEmail(NullSendMailService.Instance);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestReportingEmail_ShowsMessageBoxWhenTestEmailSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.Enabled = true;
                presenter.Model.ReportingModel.FromAddress = "me@home.com";
                presenter.Model.ReportingModel.ToAddress = "you@yourhouse.com";
                presenter.Model.ReportingModel.Server = "foo";
                presenter.Model.ReportingModel.Port = 25;
                // Act
                presenter.TestReportingEmail(NullSendMailService.Instance);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowInformation), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestReportingEmail_ShowsMessageBoxWhenTestEmailFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.Enabled = true;
                presenter.Model.ReportingModel.FromAddress = "me@home.com";
                presenter.Model.ReportingModel.ToAddress = "you@yourhouse.com";
                presenter.Model.ReportingModel.Server = "foo";
                presenter.Model.ReportingModel.Port = 25;
                // Act
                presenter.TestReportingEmail(new SendMailServiceThrows());
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestExtremeOverclockingUser_StartsLocalProcess()
        {
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            using (var presenter = new MockDialogPreferencesPresenter(model))
            {
                model.OptionsModel.EocUserID = 12345;
                // Act
                var localProcess = new MockLocalProcessService();
                presenter.TestExtremeOverclockingUser(localProcess);
                // Assert
                Assert.AreEqual(1, localProcess.Invocations.Count);
                Assert.IsTrue(localProcess.Invocations.First().FileName.EndsWith("12345"));
            }
        }

        [Test]
        public void PreferencesPresenter_TestExtremeOverclockingUser_ShowsMessageBoxWhenLocalProcessFailsToStart()
        {
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                // Act
                presenter.TestExtremeOverclockingUser(new LocalProcessServiceThrows());
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_DoesNotThrowWhenFtpTestConnectionSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.Enabled = true;
                presenter.Model.WebGenerationModel.WebDeploymentType = WebDeploymentType.Ftp;
                // Act & Assert
                Assert.DoesNotThrowAsync(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_ThrowsWhenFtpTestConnectionFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.Enabled = true;
                presenter.Model.WebGenerationModel.WebDeploymentType = WebDeploymentType.Ftp;
                // Act & Assert
                Assert.ThrowsAsync<WebException>(async () => await presenter.TestWebGenerationConnection(new FtpServiceThrowsOnCheckConnection()));
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_DoesNotThrowWhenPathTestConnectionSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.Enabled = true;
                presenter.Model.WebGenerationModel.WebDeploymentType = WebDeploymentType.Path;
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebGenerationModel.Path = artifacts.Path;
                    // Act & Assert
                    Assert.DoesNotThrowAsync(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
                }
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_ThrowsWhenPathTestConnectionFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.Enabled = true;
                presenter.Model.WebGenerationModel.WebDeploymentType = WebDeploymentType.Path;
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebGenerationModel.Path = Path.Combine(artifacts.Path, "DoesNotExist");
                    // Act & Assert
                    Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFile_SetsFolderDialogInitialDirectoryAndFileNameWhenModelPathIsFileAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    string path = artifacts.GetRandomFilePath();
                    File.WriteAllText(path, String.Empty);
                    presenter.Model.ClientsModel.DefaultConfigFile = path;
                    var dialog = new MockFileDialogPresenter(_ => default);
                    // Act
                    presenter.BrowseForConfigurationFile(dialog);
                    // Assert
                    Assert.AreEqual(Path.GetDirectoryName(path), dialog.InitialDirectory);
                    Assert.AreEqual(Path.GetFileName(path), dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFile_SetsFolderDialogInitialDirectoryWhenModelPathIsDirectoryAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.ClientsModel.DefaultConfigFile = artifacts.Path;
                    var dialog = new MockFileDialogPresenter(_ => default);
                    // Act
                    presenter.BrowseForConfigurationFile(dialog);
                    // Assert
                    Assert.AreEqual(artifacts.Path, dialog.InitialDirectory);
                    Assert.AreEqual(null, dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFile_SetsModelPathWhenDialogResultIsOK()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFileDialogPresenter(_ => DialogResult.OK);
                string path = @"C:\foo\bar.hfmx";
                dialog.FileName = path;
                // Act
                presenter.BrowseForConfigurationFile(dialog);
                // Assert
                Assert.AreEqual(path, presenter.Model.ClientsModel.DefaultConfigFile);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsFolderDialogInitialDirectoryAndFileNameWhenModelPathIsFileAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    string path = artifacts.GetRandomFilePath();
                    File.WriteAllText(path, String.Empty);
                    presenter.Model.WebVisualStylesModel.OverviewXsltPath = path;
                    var dialog = new MockFileDialogPresenter(_ => default);
                    // Act
                    presenter.BrowseForOverviewTransform(dialog);
                    // Assert
                    Assert.AreEqual(Path.GetDirectoryName(path), dialog.InitialDirectory);
                    Assert.AreEqual(Path.GetFileName(path), dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsFolderDialogInitialDirectoryAndFileNameWhenModelPathIsFileAndExistsInDefaultXsltPath()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var preferences = new InMemoryPreferencesProvider(artifacts.Path, null, null);
                using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(preferences, new InMemoryAutoRunConfiguration())))
                {
                    string defaultXsltPath = Path.Combine(artifacts.Path, Core.Application.XsltFolderName);
                    Directory.CreateDirectory(defaultXsltPath);
                    string path = Path.Combine(defaultXsltPath, "foo.xslt");
                    File.WriteAllText(path, String.Empty);

                    presenter.Model.WebVisualStylesModel.OverviewXsltPath = Path.GetFileName(path);
                    var dialog = new MockFileDialogPresenter(_ => default);
                    // Act
                    presenter.BrowseForOverviewTransform(dialog);
                    // Assert
                    Assert.AreEqual(defaultXsltPath, dialog.InitialDirectory);
                    Assert.AreEqual(Path.GetFileName(path), dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsFolderDialogInitialDirectoryWhenModelPathIsDirectoryAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebVisualStylesModel.OverviewXsltPath = artifacts.Path;
                    var dialog = new MockFileDialogPresenter(_ => default);
                    // Act
                    presenter.BrowseForOverviewTransform(dialog);
                    // Assert
                    Assert.AreEqual(artifacts.Path, dialog.InitialDirectory);
                    Assert.AreEqual(null, dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsModelPathWhenDialogResultIsOK()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFileDialogPresenter(_ => DialogResult.OK);
                string path = @"C:\foo\overview.xslt";
                dialog.FileName = path;
                // Act
                presenter.BrowseForOverviewTransform(dialog);
                // Assert
                Assert.AreEqual(path, presenter.Model.WebVisualStylesModel.OverviewXsltPath);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsModelFileNameWhenDialogResultIsOK()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var preferences = new InMemoryPreferencesProvider(artifacts.Path, null, null);
                using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(preferences, new InMemoryAutoRunConfiguration())))
                {
                    string defaultXsltPath = Path.Combine(artifacts.Path, Core.Application.XsltFolderName);

                    var dialog = new MockFileDialogPresenter(_ => DialogResult.OK);
                    string path = Path.Combine(defaultXsltPath, "overview.xslt");
                    dialog.FileName = path;
                    // Act
                    presenter.BrowseForOverviewTransform(dialog);
                    // Assert
                    Assert.AreEqual(Path.GetFileName(path), presenter.Model.WebVisualStylesModel.OverviewXsltPath);
                }
            }
        }

        private class MockDialogPreferencesPresenter : PreferencesPresenter
        {
            public MockDialogPreferencesPresenter(PreferencesModel model) : base(model, null, null, null)
            {
            }

            public MockDialogPreferencesPresenter(PreferencesModel model, MessageBoxPresenter messageBox) : base(model, null, messageBox, null)
            {
            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            protected override IWin32Dialog OnCreateDialog() => new MockWin32Dialog();
        }

        private class PreferencesModelThrowsOnSave : PreferencesModel
        {
            public PreferencesModelThrowsOnSave() : base(new InMemoryPreferencesProvider(), new InMemoryAutoRunConfiguration())
            {
            }

            public override void Save()
            {
                throw new Exception("Save failed.");
            }
        }

        private class SendMailServiceThrows : SendMailService
        {
            public override void SendEmail(string mailFrom, string mailTo, string subject, string body,
                string host, int port, string username, string password, bool enableSsl)
            {
                throw new Exception("Send mail failed.");
            }
        }

        private class FtpServiceThrowsOnCheckConnection : NullFtpService
        {
            public override void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode)
            {
                throw new WebException("Check connection failed.");
            }
        }

        private class LocalProcessServiceThrows : LocalProcessService
        {
            public override LocalProcess Start(string fileName)
            {
                throw new Exception("Process start failed.");
            }

            public override LocalProcess Start(string fileName, string arguments)
            {
                throw new Exception("Process start failed.");
            }
        }
    }
}
