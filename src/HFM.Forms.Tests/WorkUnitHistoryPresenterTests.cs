using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Serializers;
using HFM.Core.WorkUnits;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;
using HFM.Preferences;

using Moq;

using NUnit.Framework;

namespace HFM.Forms
{
    [TestFixture]
    public class WorkUnitHistoryPresenterTests
    {
        [Test]
        public void WorkUnitHistoryPresenter_Show_ShowsViewAndBringsToFront()
        {
            // Arrange
            var presenter = CreatePresenter();
            // Act
            presenter.Show();
            // Assert
            Assert.IsTrue(presenter.MockForm.Shown);
            Assert.AreEqual(1, presenter.MockForm.Invocations.Count(x => x.Name == nameof(IWin32Form.BringToFront)));
        }

        [Test]
        public void WorkUnitHistoryPresenter_Show_ShowsViewAndSetsWindowStateToNormal_Test()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.Show();
            Assert.IsTrue(presenter.MockForm.Shown);
            Assert.AreEqual(FormWindowState.Normal, presenter.Form.WindowState);
            presenter.Form.WindowState = FormWindowState.Minimized;
            // Act
            presenter.Show();
            // Assert
            Assert.AreEqual(FormWindowState.Normal, presenter.Form.WindowState);
        }

        [Test]
        public void WorkUnitHistoryPresenter_Close_RaisesClosedEvent()
        {
            var presenter = CreatePresenter();
            presenter.Show();
            bool presenterClosedFired = false;
            presenter.Closed += (s, e) => presenterClosedFired = true;
            // Act
            presenter.Close();
            // Assert
            Assert.AreEqual(true, presenterClosedFired);
        }

        [Test]
        public void WorkUnitHistoryPresenter_NewQueryClick_AddsNewQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryPresenter = new MockDialogWorkUnitQueryPresenter(p =>
            {
                p.Query.Name = "Test";
                p.Query.Parameters[0].Value = 6606;
            }, _ => DialogResult.OK);
            // Act
            presenter.NewQueryClick(queryPresenter);
            // Assert
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            Assert.AreEqual("Test", presenter.Model.SelectedWorkUnitQuery.Name);
        }

        [Test]
        public void WorkUnitHistoryPresenter_NewQueryClick_CancelsNewQueryAndExits()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryPresenter = new MockDialogWorkUnitQueryPresenter(_ => DialogResult.Cancel);
            // Act
            presenter.NewQueryClick(queryPresenter);
            // Assert
            Assert.AreEqual(1, presenter.Model.QueryBindingSource.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_NewQueryClick_ShowsMessageBoxWhenAttemptingToEditSelectAllQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            int invocations = 0;
            var queryPresenter = new MockDialogWorkUnitQueryPresenter(_ =>
            {
                switch (invocations++)
                {
                    case 0:
                        return DialogResult.OK;
                    default:
                        return DialogResult.Cancel;
                }
            });
            // Act
            presenter.NewQueryClick(queryPresenter);
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_EditQueryClick_EditsExistingQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.Model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);

            var queryPresenter = new MockDialogWorkUnitQueryPresenter(p =>
            {
                p.Query.Name = "Test2";
            }, _ => DialogResult.OK);
            // Act
            presenter.EditQueryClick(queryPresenter);
            // Assert
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            Assert.AreEqual("Test2", presenter.Model.SelectedWorkUnitQuery.Name);
        }

        [Test]
        public void WorkUnitHistoryPresenter_EditQueryClick_CancelsTheEditAndExits()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryPresenter = new MockDialogWorkUnitQueryPresenter(_ => DialogResult.Cancel);
            // Act
            presenter.EditQueryClick(queryPresenter);
            // Assert
            Assert.AreEqual(1, presenter.Model.QueryBindingSource.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_EditQueryClick_ShowsMessageBoxWhenAttemptingToEditSelectAllQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            int invocations = 0;
            var queryPresenter = new MockDialogWorkUnitQueryPresenter(_ =>
            {
                switch (invocations++)
                {
                    case 0:
                        return DialogResult.OK;
                    default:
                        return DialogResult.Cancel;
                }
            });
            // Act
            presenter.EditQueryClick(queryPresenter);
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndDeletesQuery()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = CreatePresenter(messageBox);
            presenter.Model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            // Act
            presenter.DeleteQueryClick();
            // Assert
            Assert.AreEqual(1, presenter.Model.QueryBindingSource.Count);
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndExitsAfterNoAnswer()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.No);
            var presenter = CreatePresenter(messageBox);
            // Act
            presenter.DeleteQueryClick();
            // Assert
            Assert.AreEqual(1, messageBox.Invocations.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndFailsToDeleteSelectAllQuery()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = CreatePresenter(messageBox);
            // Act
            presenter.DeleteQueryClick();
            // Assert
            Assert.AreEqual(2, messageBox.Invocations.Count);
        }

        [Test]
        public async Task WorkUnitHistoryPresenter_DeleteWorkUnitClick_AsksYesNoQuestionAndDeletesSelectedRow()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = CreatePresenter(messageBox);
            presenter.Model.HistoryBindingSource.Add(new WorkUnitRow { ID = 1 });
            presenter.Model.HistoryBindingSource.ResetBindings(false);

            var mockRepository = Mock.Get(presenter.Model.Repository);
            mockRepository.Setup(x => x.DeleteAsync(It.IsAny<WorkUnitRow>())).Returns(Task.FromResult(1));
            // Act
            await presenter.DeleteWorkUnitClick();
            // Assert
            Assert.AreEqual(1, messageBox.Invocations.Count);
            mockRepository.Verify();
        }

        [Test]
        public async Task WorkUnitHistoryPresenter_DeleteWorkUnitClick_ShowsMessageBoxWhenNoRowIsSelected()
        {
            // Arrange
            var presenter = CreatePresenter();
            // Act
            await presenter.DeleteWorkUnitClick();
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        [Test]
        public void WorkUnitHistoryPresenter_ExportClick_ProvidesSerializerWithFileNameAndRows()
        {
            // Arrange
            var presenter = CreatePresenter();
            var workUnitRows = new[] { new WorkUnitRow() };
            var mockRepository = Mock.Get(presenter.Model.Repository);
            mockRepository.Setup(x => x.Fetch(It.IsAny<WorkUnitQuery>(), It.IsAny<BonusCalculation>())).Returns(workUnitRows);
            var saveFile = CreateSaveFileDialogView();
            var serializer = new WorkUnitRowFileSerializerSavesFileNameAndRows();
            // Act
            presenter.ExportClick(saveFile, new List<IFileSerializer<List<WorkUnitRow>>> { serializer });
            // Assert
            Assert.AreEqual("test.csv", serializer.FileName);
            Assert.IsTrue(workUnitRows.SequenceEqual(serializer.Rows));
        }

        private class WorkUnitRowFileSerializerSavesFileNameAndRows : IFileSerializer<List<WorkUnitRow>>
        {
            public string FileExtension { get; }
            public string FileTypeFilter { get; }

            public List<WorkUnitRow> Deserialize(string path)
            {
                throw new NotImplementedException();
            }

            public string FileName { get; set; }

            public List<WorkUnitRow> Rows { get; set; }

            public void Serialize(string path, List<WorkUnitRow> value)
            {
                FileName = path;
                Rows = value;
            }
        }

        [Test]
        public void WorkUnitHistoryPresenter_ExportClick_ShowsMessageBoxWhenSerializerThrowsException()
        {
            // Arrange
            var presenter = CreatePresenter();
            var mockRepository = Mock.Get(presenter.Model.Repository);
            mockRepository.Setup(x => x.Fetch(It.IsAny<WorkUnitQuery>(), It.IsAny<BonusCalculation>())).Returns(Array.Empty<WorkUnitRow>());
            var saveFile = CreateSaveFileDialogView();
            // Act
            presenter.ExportClick(saveFile, new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowFileSerializerThrows() });
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        private class WorkUnitRowFileSerializerThrows : IFileSerializer<List<WorkUnitRow>>
        {
            public string FileExtension { get; }
            public string FileTypeFilter { get; }

            public List<WorkUnitRow> Deserialize(string path)
            {
                throw new NotImplementedException();
            }

            public void Serialize(string path, List<WorkUnitRow> value)
            {
                throw new IOException();
            }
        }

        private static FileDialogPresenter CreateSaveFileDialogView()
        {
            var saveFile = new MockFileDialogPresenter(owner => DialogResult.OK);
            saveFile.FileName = "test.csv";
            saveFile.FilterIndex = 1;
            return saveFile;
        }

        private class MockFormWorkUnitHistoryPresenter : WorkUnitHistoryPresenter
        {
            public MockFormWorkUnitHistoryPresenter(MessageBoxPresenter messageBox)
                : base(
                    new WorkUnitHistoryModel(
                        new InMemoryPreferencesProvider(),
                        new WorkUnitQueryDataContainer(),
                        Mock.Of<IWorkUnitRepository>()),
                    null,
                    null,
                    messageBox ?? new MockMessageBoxPresenter(),
                    NullProteinService.Instance)
            {

            }

            public MockWin32Form MockForm => Form as MockWin32Form;

            protected override IWin32Form OnCreateForm()
            {
                return new MockWin32Form();
            }
        }

        private static MockFormWorkUnitHistoryPresenter CreatePresenter(MessageBoxPresenter messageBox = null)
        {
            return new MockFormWorkUnitHistoryPresenter(messageBox);
        }

        private class MockDialogWorkUnitQueryPresenter : WorkUnitQueryPresenter
        {
            private readonly Action<WorkUnitQueryPresenter> _presenterAction;
            private readonly Func<IWin32Window, DialogResult> _dialogResultProvider;

            public MockDialogWorkUnitQueryPresenter(Func<IWin32Window, DialogResult> dialogResultProvider) : base(new WorkUnitQuery())
            {
                _dialogResultProvider = dialogResultProvider;
            }

            public MockDialogWorkUnitQueryPresenter(Action<WorkUnitQueryPresenter> presenterAction, Func<IWin32Window, DialogResult> dialogResultProvider) : base(new WorkUnitQuery())
            {
                _presenterAction = presenterAction;
                _dialogResultProvider = dialogResultProvider;
            }

            protected override IWin32Dialog OnCreateDialog() => new MockWin32Dialog<WorkUnitQueryPresenter>(this, _presenterAction, _dialogResultProvider);
        }
    }
}
