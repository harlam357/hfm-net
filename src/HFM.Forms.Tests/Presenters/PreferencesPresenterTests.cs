
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
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new NoDialogPreferencesPresenter(new PreferencesModel(new InMemoryPreferenceSet(), new InMemoryAutoRunConfiguration()), messageBox))
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

        private class NoDialogPreferencesPresenter : PreferencesPresenter
        {
            public NoDialogPreferencesPresenter(PreferencesModel model) : base(model, null, null)
            {
            }

            public NoDialogPreferencesPresenter(PreferencesModel model, MessageBoxPresenter messageBox) : base(model, null, messageBox)
            {
            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            public override DialogResult ShowDialog(IWin32Window owner)
            {
                Dialog = new MockWin32Dialog();
                return Dialog.ShowDialog(owner);
            }
        }
    }
}
