using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Client;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class FahClientSettingsPresenterTests
    {
        [Test]
        public void FahClientSettingsPresenter_ShowDialog_DoesNotConnectWhenModelHasError()
        {
            // Arrange
            using (var presenter = new MockDialogFahClientSettingsPresenter(new FahClientSettingsModel()))
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
            using (var presenter = new MockDialogFahClientSettingsPresenter(new FahClientSettingsModel { Name = "foo", Server = "bar" }))
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
            using (var connection = new MockFahClientConnectionWithMessages())
            {
                var model = new MockConnectionFahClientSettingsModel(connection) { Name = "foo", Server = "bar" };
                using (var presenter = new MockDialogFahClientSettingsPresenter(model))
                {
                    // Act
                    presenter.ShowDialog(null);
                    // Assert
                    Assert.IsNotNull(presenter.Connection);
                    Assert.IsTrue(presenter.Connection.Connected);
                    Assert.AreEqual(1, presenter.Model.Slots.Count);
                }
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ConnectClicked_AttemptsConnectionAndShowsMessageBoxOnFailure()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogFahClientSettingsPresenter(new FahClientSettingsModel { Name = "foo", Server = "bar" }, messageBox))
            {
                // Act
                presenter.ConnectClicked();
                // Assert
                Assert.AreEqual(1, messageBox.Invocations.Count);
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ConnectClicked_ConnectsAndShowsMessageBoxWhenFahClientReaderFailsToRead()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter();
            using (var connection = new MockFahClientConnection())
            {
                var model = new MockConnectionFahClientSettingsModel(connection) { Name = "foo", Server = "bar" };
                using (var presenter = new MockDialogFahClientSettingsPresenter(model, messageBox))
                {
                    // Act
                    presenter.ConnectClicked();
                    // Assert
                    Assert.AreEqual(1, messageBox.Invocations.Count);
                    Assert.IsNotNull(presenter.Connection);
                    Assert.IsFalse(presenter.Connection.Connected);
                    Assert.IsTrue(presenter.Model.ConnectEnabled);
                    Assert.AreEqual(0, presenter.Model.Slots.Count);
                }
            }
        }

        [Test]
        public void FahClientSettingsPresenter_ConnectClicked_ConnectsWhenModelHasNoError()
        {
            // Arrange
            using (var connection = new MockFahClientConnectionWithMessages())
            {
                var model = new MockConnectionFahClientSettingsModel(connection) { Name = "foo", Server = "bar" };
                using (var presenter = new MockDialogFahClientSettingsPresenter(model))
                {
                    // Act
                    presenter.ConnectClicked();
                    // Assert
                    Assert.IsNotNull(presenter.Connection);
                    Assert.IsTrue(presenter.Connection.Connected);
                    Assert.AreEqual(1, presenter.Model.Slots.Count);
                }
            }
        }

        [Test]
        public void FahClientSettingsPresenter_OKClicked_ShowsMessageBoxWhenModelHasError()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter();
            using (var presenter = new MockDialogFahClientSettingsPresenter(new FahClientSettingsModel(), messageBox))
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
            using (var presenter = new MockDialogFahClientSettingsPresenter(new FahClientSettingsModel { Name = "foo", Server = "bar" }))
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

        private class MockDialogFahClientSettingsPresenter : FahClientSettingsPresenter
        {
            public MockDialogFahClientSettingsPresenter(FahClientSettingsModel model) : base(model, null, null)
            {
            }

            public MockDialogFahClientSettingsPresenter(FahClientSettingsModel model, MessageBoxPresenter messageBox) : base(model, null, messageBox)
            {
            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            protected override IWin32Dialog OnCreateDialog() => new MockWin32Dialog();
        }

        private class MockConnectionFahClientSettingsModel : FahClientSettingsModel
        {
            private readonly FahClientConnection _connection;

            public MockConnectionFahClientSettingsModel(FahClientConnection connection)
            {
                _connection = connection;
            }

            public override FahClientConnection CreateConnection()
            {
                return _connection;
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

            protected override FahClientCommand OnCreateCommand()
            {
                return new MockFahClientCommand(this);
            }

            protected override FahClientReader OnCreateReader()
            {
                return new MockFahClientReader(this);
            }
        }

        private class MockFahClientConnectionWithMessages : MockFahClientConnection
        {
            public FahClientCommand LastCommand { get; private set; }

            protected override FahClientCommand OnCreateCommand()
            {
                return LastCommand = new MockFahClientCommand(this);
            }

            protected override FahClientReader OnCreateReader()
            {
                return new MockFahClientReaderWithMessages(this);
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
            public MockFahClientReader(FahClientConnection connection) : base(connection)
            {
            }

            public override bool Read()
            {
                return false;
            }

            public override Task<bool> ReadAsync()
            {
                return Task.FromResult(false);
            }
        }

        private class MockFahClientReaderWithMessages : FahClientReader
        {
            private readonly MockFahClientConnectionWithMessages _connection;

            public MockFahClientReaderWithMessages(MockFahClientConnectionWithMessages connection) : base(connection)
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
                    var text = File.ReadAllText(@"..\..\..\..\TestFiles\Client_v7_12\slots.txt");
                    Message = FahClientMessageExtractor.Default.Extract(new StringBuilder(text));
                    return true;
                }

                if (_connection.LastCommand.CommandText.StartsWith("slot-options"))
                {
                    var text = File.ReadAllText(@"..\..\..\..\TestFiles\Client_v7_12\slot-options1.txt");
                    Message = FahClientMessageExtractor.Default.Extract(new StringBuilder(text));
                    return true;
                }

                return false;
            }
        }
    }
}
