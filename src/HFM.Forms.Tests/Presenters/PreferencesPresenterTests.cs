
using System;
using System.Linq;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Core.Services;
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModelThrowsOnSave()))
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                presenter.Model.ScheduledTasksModel.WebRoot = @"foo\";
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
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
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration())))
            {
                var dialog = new MockFolderDialogPresenter(window => DialogResult.OK);
                dialog.SelectedPath = @"foo\";
                // Act
                presenter.BrowseWebFolderClicked(dialog);
                // Assert
                Assert.AreEqual(@"foo\", presenter.Model.ScheduledTasksModel.WebRoot);
            }
        }

        [Test]
        public void PreferencesPresenter_TestEmailClicked_ShowsMessageBoxWhenModelHasError()
        {
            // Arrange
            var model = new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration());
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new NoDialogPreferencesPresenter(model, messageBox))
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
            using (var presenter = new NoDialogPreferencesPresenter(model, messageBox))
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
            using (var presenter = new NoDialogPreferencesPresenter(model, messageBox))
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

        private class NoDialogPreferencesPresenter : PreferencesPresenter
        {
            public NoDialogPreferencesPresenter(PreferencesModel model) : base(model, null, null, null)
            {
            }

            public NoDialogPreferencesPresenter(PreferencesModel model, MessageBoxPresenter messageBox) : base(model, null, messageBox, null)
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
    }
}
