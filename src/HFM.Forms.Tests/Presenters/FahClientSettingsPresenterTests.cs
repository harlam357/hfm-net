
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Client;
using HFM.Forms.Mocks;
using HFM.Forms.Models;

namespace HFM.Forms
{
    [TestFixture]
    public class FahClientSettingsPresenterTests
    {
        [Test]
        public void FahClientSettingsPresenter_ShowDialog_DoesNotConnectWhenModelHasError()
        {
            // Arrange
            using (var presenter = new NoDialogFahClientSettingsPresenter(new FahClientSettingsModel()))
            {
                // Act
                presenter.ShowDialog(null);
                // Assert
                Assert.IsNull(presenter.Connection);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ShowDialog_AttemptsConnectionWhenModelHasNoError()
        {
            // Arrange
            using (var presenter = new NoDialogFahClientSettingsPresenter(new FahClientSettingsModel {Name = "foo", Server = "bar"}))
            {
                // Act
                presenter.ShowDialog(null);
                // Assert
                Assert.IsNotNull(presenter.Connection);
                Assert.IsFalse(presenter.Connection.Connected);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ShowDialog_ConnectsWhenModelHasNoError()
        {
            // Arrange
            using (var presenter = new NoDialogFahClientSettingsPresenter(new MockConnectionFahClientSettingsModel {Name = "foo", Server = "bar"}))
            {
                // Act
                presenter.ShowDialog(null);
                // Assert
                Assert.IsNotNull(presenter.Connection);
                Assert.IsTrue(presenter.Connection.Connected);
                Assert.AreEqual(1, presenter.Model.Slots.Count);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ConnectClicked_AttemptsConnectionAndShowsMessageBoxOnFailure()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new NoDialogFahClientSettingsPresenter(new FahClientSettingsModel {Name = "foo", Server = "bar"}, messageBox))
            {
                // Act
                presenter.ConnectClicked();
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ConnectClicked_ConnectsWhenModelHasNoError()
        {
            // Arrange
            using (var presenter = new NoDialogFahClientSettingsPresenter(new MockConnectionFahClientSettingsModel {Name = "foo", Server = "bar"}))
            {
                // Act
                presenter.ConnectClicked();
                // Assert
                Assert.IsNotNull(presenter.Connection);
                Assert.IsTrue(presenter.Connection.Connected);
                Assert.AreEqual(1, presenter.Model.Slots.Count);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_OKClicked_ShowsMessageBoxWhenModelHasError()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new NoDialogFahClientSettingsPresenter(new FahClientSettingsModel(), messageBox))
            {
                presenter.ShowDialog(null);
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.OKClicked();
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
                Assert.IsTrue(presenter.MockDialog.Shown);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_OKClicked_SetsDialogResultAndClosesDialogWhenModelHasNoError()
        {
            // Arrange
            using (var presenter = new NoDialogFahClientSettingsPresenter(new FahClientSettingsModel {Name = "foo", Server = "bar"}))
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

        private class NoDialogFahClientSettingsPresenter : FahClientSettingsPresenter
        {
            public NoDialogFahClientSettingsPresenter(FahClientSettingsModel model) : base(null, model, null)
            {
            }

            public NoDialogFahClientSettingsPresenter(FahClientSettingsModel model, MessageBoxPresenter messageBox) : base(null, model, messageBox)
            {
            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            public override DialogResult ShowDialog(IWin32Window owner)
            {
                ConnectIfModelHasNoError();

                Dialog = new MockWin32Dialog();
                return Dialog.ShowDialog(owner);
            }
        }

        private class MockConnectionFahClientSettingsModel : FahClientSettingsModel
        {
            public override FahClientConnection CreateConnection()
            {
                return new MockFahClientConnection();
            }
        }

        private class MockFahClientConnection : FahClientConnection
        {
            private bool _connected;

            public override bool Connected => _connected;

            public MockFahClientConnection()
                : base("foo", 2000)
            {

            }

            public override void Open()
            {
                _connected = true;
            }

            public override void Close()
            {
                _connected = false;
            }

            public FahClientCommand LastCommand { get; private set; }

            protected override FahClientCommand OnCreateCommand()
            {
                return LastCommand = new MockFahClientCommand(this);
            }

            protected override FahClientReader OnCreateReader()
            {
                return new MockFahClientReader(this);
            }
        }

        private class MockFahClientCommand : FahClientCommand
        {
            public MockFahClientCommand(FahClientConnection connection) : base(connection)
            {

            }

            public override int Execute()
            {
                return 0;
            }

            public override Task<int> ExecuteAsync()
            {
                return Task.FromResult(0);
            }
        }

        private class MockFahClientReader : FahClientReader
        {
            private readonly MockFahClientConnection _connection;

            public MockFahClientReader(MockFahClientConnection connection) : base(connection)
            {
                _connection = connection;
            }

            public override bool Read()
            {
                return SetMessageBasedOnLastCommand();
            }

            public override Task<bool> ReadAsync()
            {
                return Task.FromResult(SetMessageBasedOnLastCommand());
            }

            private bool SetMessageBasedOnLastCommand()
            {
                if (_connection.LastCommand.CommandText == "slot-info")
                {
                    var text = File.ReadAllText(@"..\..\..\TestFiles\Client_v7_12\slots.txt");
                    Message = FahClientMessageExtractor.Default.Extract(new StringBuilder(text));
                    return true;
                }

                if (_connection.LastCommand.CommandText.StartsWith("slot-options"))
                {
                    var text = File.ReadAllText(@"..\..\..\TestFiles\Client_v7_12\slot-options1.txt");
                    Message = FahClientMessageExtractor.Default.Extract(new StringBuilder(text));
                    return true;
                }

                return false;
            }
        }
    }
}
