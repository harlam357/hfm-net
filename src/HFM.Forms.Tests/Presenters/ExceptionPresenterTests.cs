
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Forms.Mocks;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class ExceptionPresenterTests
    {
        [Test]
        public void ExceptionPresenter_BuildExceptionText_FromPropertiesAndException()
        {
            // Arrange
            var properties = new Dictionary<string, string> { { "Fizz", "Bizz" } };
            using (var presenter = new MockDialogExceptionPresenter(properties))
            {
                presenter.ShowDialog(null, new Exception("Foo"), false);
                // Act
                string text = presenter.BuildExceptionText();
                // Assert
                Assert.AreNotEqual(String.Empty, text);
            }
        }

        [Test]
        public void ExceptionPresenter_ContinueClicked_SetsDialogResultAndClosesDialog()
        {
            // Arrange
            using (var presenter = new MockDialogExceptionPresenter(null))
            {
                presenter.ShowDialog(null, new Exception("Foo"), false);
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.ContinueClicked();
                // Assert
                Assert.AreEqual(DialogResult.Ignore, presenter.Dialog.DialogResult);
                Assert.IsFalse(presenter.MockDialog.Shown);
            }
        }

        private class MockDialogExceptionPresenter : DefaultExceptionPresenter
        {
            public MockDialogExceptionPresenter(IDictionary<string, string> properties) : base(null, null, properties, null)
            {
            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            public override DialogResult ShowDialog(IWin32Window owner, Exception exception, bool mustTerminate)
            {
                Exception = exception ?? throw new ArgumentNullException(nameof(exception));
                MustTerminate = mustTerminate;

                Dialog = new MockWin32Dialog();
                return Dialog.ShowDialog(owner);
            }
        }
    }
}
