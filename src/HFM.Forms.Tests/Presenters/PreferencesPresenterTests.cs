
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
using HFM.Preferences;

namespace HFM.Forms
{
    [TestFixture]
    public class PreferencesPresenterTests
    {
        [Test]
        public void PreferencesPresenter_OKClicked_DoesNotCloseWhenModelHasError()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                presenter.Model.WebSettingsModel.ProjectDownloadUrl = "foo";
                presenter.ShowDialog(null);
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
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
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
        public void PreferencesPresenter_BrowseWebFolderClicked_SetsFolderDialogSelectedPathWhenModelPathIsSet()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                presenter.Model.WebGenerationModel.WebRoot = @"foo\";
                var dialog = new MockFolderDialogPresenter(window => default);
                // Act
                presenter.BrowseWebFolderClicked(dialog);
                // Assert
                Assert.AreEqual(@"foo\", dialog.SelectedPath);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseWebFolderClicked_DoesNotSetFolderDialogSelectedPathWhenModelPathIsNotSet()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFolderDialogPresenter(window => default);
                // Act
                presenter.BrowseWebFolderClicked(dialog);
                // Assert
                Assert.IsNull(dialog.SelectedPath);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseWebFolderClicked_SetsModelPathWhenDialogResultIsOK()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFolderDialogPresenter(window => DialogResult.OK);
                dialog.SelectedPath = @"foo\";
                // Act
                presenter.BrowseWebFolderClicked(dialog);
                // Assert
                Assert.AreEqual(@"foo\", presenter.Model.WebGenerationModel.WebRoot);
            }
        }

        [Test]
        public void PreferencesPresenter_TestEmailClicked_ShowsMessageBoxWhenModelHasError()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.ReportingEnabled = true;
                // Act
                presenter.TestEmailClicked(NullSendMailService.Instance);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestEmailClicked_ShowsMessageBoxWhenTestEmailSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.ReportingEnabled = true;
                presenter.Model.ReportingModel.FromAddress = "me@home.com";
                presenter.Model.ReportingModel.ToAddress = "you@yourhouse.com";
                presenter.Model.ReportingModel.ServerAddress = "foo";
                presenter.Model.ReportingModel.ServerPort = 25;
                // Act
                presenter.TestEmailClicked(NullSendMailService.Instance);
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowInformation), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestEmailClicked_ShowsMessageBoxWhenTestEmailFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.ReportingModel.ReportingEnabled = true;
                presenter.Model.ReportingModel.FromAddress = "me@home.com";
                presenter.Model.ReportingModel.ToAddress = "you@yourhouse.com";
                presenter.Model.ReportingModel.ServerAddress = "foo";
                presenter.Model.ReportingModel.ServerPort = 25;
                // Act
                presenter.TestEmailClicked(new SendMailServiceThrows());
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestExtremeOverclockingUserClicked_StartsLocalProcess()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            using (var presenter = new MockDialogPreferencesPresenter(model))
            {
                model.OptionsModel.EocUserID = 12345;
                // Act
                var localProcess = new MockLocalProcessService();
                presenter.TestExtremeOverclockingUserClicked(localProcess);
                // Assert
                Assert.AreEqual(1, localProcess.Invocations.Count);
                Assert.IsTrue(localProcess.Invocations.First().FileName.EndsWith("12345"));
            }
        }

        [Test]
        public void PreferencesPresenter_TestExtremeOverclockingUserClicked_ShowsMessageBoxWhenLocalProcessFailsToStart()
        {
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                // Act
                presenter.TestExtremeOverclockingUserClicked(new LocalProcessServiceThrows());
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.AreEqual(nameof(MessageBoxPresenter.ShowError), messageBox.Invocations.First().Name);
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_DoesNotThrowWhenFtpTestConnectionSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.GenerateWeb = true;
                presenter.Model.WebGenerationModel.WebGenType = WebDeploymentType.Ftp;
                // Act & Assert
                Assert.DoesNotThrowAsync(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_ThrowsWhenFtpTestConnectionFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.GenerateWeb = true;
                presenter.Model.WebGenerationModel.WebGenType = WebDeploymentType.Ftp;
                // Act & Assert
                Assert.ThrowsAsync<WebException>(async () => await presenter.TestWebGenerationConnection(new FtpServiceThrowsOnCheckConnection()));
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_DoesNotThrowWhenPathTestConnectionSucceeds()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.GenerateWeb = true;
                presenter.Model.WebGenerationModel.WebGenType = WebDeploymentType.Path;
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebGenerationModel.WebRoot = artifacts.Path;
                    // Act & Assert
                    Assert.DoesNotThrowAsync(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
                }
            }
        }

        [Test]
        public void PreferencesPresenter_TestWebGenerationConnection_ThrowsWhenPathTestConnectionFails()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogPreferencesPresenter(model, messageBox))
            {
                presenter.Model.WebGenerationModel.GenerateWeb = true;
                presenter.Model.WebGenerationModel.WebGenType = WebDeploymentType.Path;
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebGenerationModel.WebRoot = Path.Combine(artifacts.Path, "DoesNotExist");
                    // Act & Assert
                    Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await presenter.TestWebGenerationConnection(NullFtpService.Instance));
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFileClicked_SetsFolderDialogInitialDirectoryAndFileNameWhenModelPathIsFileAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    string path = artifacts.GetRandomFilePath();
                    File.WriteAllText(path, String.Empty);
                    presenter.Model.ClientsModel.DefaultConfigFile = path;
                    var dialog = new MockFileDialogPresenter(window => default);
                    // Act
                    presenter.BrowseForConfigurationFileClicked(dialog);
                    // Assert
                    Assert.AreEqual(Path.GetDirectoryName(path), dialog.InitialDirectory);
                    Assert.AreEqual(Path.GetFileName(path), dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFileClicked_SetsFolderDialogInitialDirectoryWhenModelPathIsDirectoryAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.ClientsModel.DefaultConfigFile = artifacts.Path;
                    var dialog = new MockFileDialogPresenter(window => default);
                    // Act
                    presenter.BrowseForConfigurationFileClicked(dialog);
                    // Assert
                    Assert.AreEqual(artifacts.Path, dialog.InitialDirectory);
                    Assert.AreEqual(null, dialog.FileName);
                }
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForConfigurationFileClicked_SetsModelPathWhenDialogResultIsOK()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFileDialogPresenter(window => DialogResult.OK);
                string path = @"C:\foo\bar.hfmx";
                dialog.FileName = path;
                // Act
                presenter.BrowseForConfigurationFileClicked(dialog);
                // Assert
                Assert.AreEqual(path, presenter.Model.ClientsModel.DefaultConfigFile);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsFolderDialogInitialDirectoryAndFileNameWhenModelPathIsFileAndExists()
        {
            // Arrange
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    string path = artifacts.GetRandomFilePath();
                    File.WriteAllText(path, String.Empty);
                    presenter.Model.WebVisualStylesModel.WebOverview = path;
                    var dialog = new MockFileDialogPresenter(window => default);
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
                var preferences = new InMemoryPreferenceSet(artifacts.Path, null, null);
                using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(preferences, new InMemoryAutoRunConfiguration())))
                {
                    string defaultXsltPath = Path.Combine(artifacts.Path, Core.Application.XsltFolderName);
                    Directory.CreateDirectory(defaultXsltPath);
                    string path = Path.Combine(defaultXsltPath, "foo.xslt");
                    File.WriteAllText(path, String.Empty);

                    presenter.Model.WebVisualStylesModel.WebOverview = Path.GetFileName(path);
                    var dialog = new MockFileDialogPresenter(window => default);
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
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                using (var artifacts = new ArtifactFolder())
                {
                    presenter.Model.WebVisualStylesModel.WebOverview = artifacts.Path;
                    var dialog = new MockFileDialogPresenter(window => default);
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
            using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFileDialogPresenter(window => DialogResult.OK);
                string path = @"C:\foo\overview.xslt";
                dialog.FileName = path;
                // Act
                presenter.BrowseForOverviewTransform(dialog);
                // Assert
                Assert.AreEqual(path, presenter.Model.WebVisualStylesModel.WebOverview);
            }
        }

        [Test]
        public void PreferencesPresenter_BrowseForOverviewTransform_SetsModelFileNameWhenDialogResultIsOK()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var preferences = new InMemoryPreferenceSet(artifacts.Path, null, null);
                using (var presenter = new MockDialogPreferencesPresenter(new PreferencesModel(preferences, new InMemoryAutoRunConfiguration())))
                {
                    string defaultXsltPath = Path.Combine(artifacts.Path, Core.Application.XsltFolderName);

                    var dialog = new MockFileDialogPresenter(window => DialogResult.OK);
                    string path = Path.Combine(defaultXsltPath, "overview.xslt");
                    dialog.FileName = path;
                    // Act
                    presenter.BrowseForOverviewTransform(dialog);
                    // Assert
                    Assert.AreEqual(Path.GetFileName(path), presenter.Model.WebVisualStylesModel.WebOverview);
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

            public override DialogResult ShowDialog(IWin32Window owner)
            {
                Dialog = new MockWin32Dialog();
                return Dialog.ShowDialog(owner);
            }
        }

        private class PreferencesModelThrowsOnSave : PreferencesModel
        {
            public PreferencesModelThrowsOnSave() : base(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())
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
