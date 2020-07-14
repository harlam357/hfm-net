
using System;
using System.Windows.Forms;

using NUnit.Framework;

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

        private class NoDialogPreferencesPresenter : PreferencesPresenter
        {
            public NoDialogPreferencesPresenter(PreferencesModel model) : base(model, null, null, null)
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
    }
}
