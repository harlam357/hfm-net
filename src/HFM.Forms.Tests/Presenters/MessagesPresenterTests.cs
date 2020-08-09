using System.Linq;
using System.Windows.Forms;

using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Views;

using NUnit.Framework;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class MessagesPresenterTests
    {
        [Test]
        public void MessagesPresenter_Show_ShowsViewAndBringsToFront()
        {
            // Arrange
            var presenter = new MockFormMessagesPresenter();
            // Act
            presenter.Show();
            // Assert
            Assert.IsTrue(presenter.MockForm.Shown);
            Assert.AreEqual(1, presenter.MockForm.Invocations.Count(x => x.Name == nameof(IWin32Form.BringToFront)));
        }

        [Test]
        public void MessagesPresenter_Show_ShowsViewAndSetsWindowStateToNormal_Test()
        {
            // Arrange
            var presenter = new MockFormMessagesPresenter();
            presenter.Show();
            Assert.IsTrue(presenter.MockForm.Shown);
            Assert.AreEqual(FormWindowState.Normal, presenter.Form.WindowState);
            presenter.Form.WindowState = FormWindowState.Minimized;
            // Act
            presenter.Show();
            // Assert
            Assert.AreEqual(FormWindowState.Normal, presenter.Form.WindowState);
        }

        private class MockFormMessagesPresenter : MessagesPresenter
        {
            public MockFormMessagesPresenter() : base(new MessagesModel(null, null))
            {

            }

            public MockWin32Form MockForm => Form as MockWin32Form;

            protected override IWin32Form OnCreateForm()
            {
                return new MockWin32Form();
            }
        }
    }
}
