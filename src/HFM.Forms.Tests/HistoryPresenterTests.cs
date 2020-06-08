
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Serializers;
using HFM.Forms.Mocks;
using HFM.Preferences;

namespace HFM.Forms
{
    [TestFixture]
    public class HistoryPresenterTests
    {
        private static HistoryPresenter CreatePresenter(MessageBoxPresenter messageBox = null)
        {
            var repository = MockRepository.GenerateMock<IWorkUnitRepository>();
            repository.Stub(x => x.Connected).Return(true);
            return new HistoryPresenter(MockRepository.GenerateMock<ILogger>(),
                                        new InMemoryPreferenceSet(), 
                                        new WorkUnitQueryDataContainer(), 
                                        MockRepository.GenerateMock<IHistoryView>(), 
                                        MockRepository.GenerateStub<IViewFactory>(), 
                                        messageBox ?? new MockMessageBoxPresenter(), 
                                        repository);
        }

        [Test]
        public void HistoryPresenter_Show_ShowsViewAndBringsToFront()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.HistoryView.Expect(x => x.Show());
            presenter.HistoryView.Expect(x => x.BringToFront());
            // Act
            presenter.Show();
            // Assert
            presenter.HistoryView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_Show_ShowsViewAndSetsWindowStateToNormal_Test()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.HistoryView.Expect(x => x.Show());
            presenter.HistoryView.Expect(x => x.WindowState).Return(FormWindowState.Minimized);
            presenter.HistoryView.Expect(x => x.WindowState = FormWindowState.Normal);
            // Act
            presenter.Show();
            // Assert
            presenter.HistoryView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_Close_RaisesPresenterClosedEvent()
        {
            var presenter = CreatePresenter();
            bool presenterClosedFired = false;
            presenter.PresenterClosed += delegate { presenterClosedFired = true; };
            // Act
            presenter.Close();
            // Assert
            Assert.AreEqual(true, presenterClosedFired);
        }

        [Test]
        public void HistoryPresenter_ViewClosing_SavesFormStateToModel()
        {
            // Arrange
            var presenter = CreatePresenter();
            var p = new Point(10, 10);
            var s = new Size(100, 100);
            var columns = new List<string> { "foo" };
            presenter.HistoryView.Stub(x => x.Location).Return(p);
            presenter.HistoryView.Stub(x => x.Size).Return(s);
            presenter.HistoryView.Stub(x => x.GetColumnSettings()).Return(columns);
            presenter.HistoryView.Stub(x => x.WindowState).Return(FormWindowState.Normal);
            // Act
            presenter.ViewClosing();
            // Assert
            Assert.AreEqual(p, presenter.Model.FormLocation);
            Assert.AreEqual(s, presenter.Model.FormSize);
            Assert.AreEqual(columns, presenter.Model.FormColumns);
        }

        [Test]
        public void HistoryPresenter_ViewClosing_WhileMinimizedSavesFormStateToModel_Test()
        {
            // Arrange
            var presenter = CreatePresenter();
            var r = new Rectangle(new Point(10, 10), new Size(100, 100));
            var columns = new List<string> { "foo" };
            presenter.HistoryView.Expect(x => x.RestoreBounds).Return(r);
            presenter.HistoryView.Expect(x => x.GetColumnSettings()).Return(columns);
            presenter.HistoryView.Expect(x => x.WindowState).Return(FormWindowState.Minimized);
            // Act
            presenter.ViewClosing();
            // Assert
            presenter.HistoryView.VerifyAllExpectations();
            Assert.AreEqual(r.Location, presenter.Model.FormLocation);
            Assert.AreEqual(r.Size, presenter.Model.FormSize);
            Assert.AreEqual(columns, presenter.Model.FormColumns);
        }

        [Test]
        public void HistoryPresenter_ViewClosing_SavesFormStateToPreferences()
        {
            // Arrange
            var presenter = CreatePresenter();
            var p = new Point(10, 10);
            var s = new Size(100, 100);
            var columns = new List<string> { "foo" };
            presenter.HistoryView.Stub(x => x.Location).Return(p);
            presenter.HistoryView.Stub(x => x.Size).Return(s);
            presenter.HistoryView.Stub(x => x.GetColumnSettings()).Return(columns);
            presenter.HistoryView.Stub(x => x.WindowState).Return(FormWindowState.Normal);
            // Act
            presenter.ViewClosing();
            // Assert
            Assert.AreEqual(p, presenter.Preferences.Get<Point>(Preference.HistoryFormLocation));
            Assert.AreEqual(s, presenter.Preferences.Get<Size>(Preference.HistoryFormSize));
            Assert.AreEqual(columns, presenter.Preferences.Get<ICollection<string>>(Preference.HistoryFormColumns));
        }
        
        [Test]
        public void HistoryPresenter_NewQueryClick_AddsNewQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryView = new QueryViewModifiesQueryAndReturnsOk(q =>
            {
                q.Name = "Test";
                q.Parameters[0].Value = 6606;
            });
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.NewQueryClick();
            // Assert
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            Assert.AreEqual("Test", presenter.Model.SelectedWorkUnitQuery.Name);
        }

        [Test]
        public void HistoryPresenter_NewQueryClick_CancelsNewQueryAndExits()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryView = MockRepository.GenerateMock<IQueryView>();
            queryView.Expect(x => x.ShowDialog(presenter.HistoryView)).Return(DialogResult.Cancel);
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.NewQueryClick();
            // Assert
            queryView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_NewQueryClick_ShowsMessageBoxWhenAttemptingToEditSelectAllQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryView = new QueryViewReturnsOkOnce();
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.NewQueryClick();
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        [Test]
        public void HistoryPresenter_EditQueryClick_EditsExistingQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.Model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);

            var queryView = new QueryViewModifiesQueryAndReturnsOk(q => q.Name = "Test2");
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.EditQueryClick();
            // Assert
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            Assert.AreEqual("Test2", presenter.Model.SelectedWorkUnitQuery.Name);
        }

        [Test]
        public void HistoryPresenter_EditQueryClick_CancelsTheEditAndExits()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryView = MockRepository.GenerateMock<IQueryView>();
            queryView.Expect(x => x.ShowDialog(presenter.HistoryView)).Return(DialogResult.Cancel);
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.EditQueryClick();
            // Assert
            queryView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_EditQueryClick_ShowsMessageBoxWhenAttemptingToEditSelectAllQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            var queryView = new QueryViewReturnsOkOnce();
            presenter.ViewFactory.Stub(x => x.GetQueryDialog()).Return(queryView);
            // Act
            presenter.EditQueryClick();
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        private class QueryViewModifiesQueryAndReturnsOk : IQueryView
        {
            private readonly Action<WorkUnitQuery> _queryAction;

            public QueryViewModifiesQueryAndReturnsOk(Action<WorkUnitQuery> queryAction)
            {
                _queryAction = queryAction;
            }

            public IntPtr Handle { get; }

            public WorkUnitQuery Query { get; set; }

            public bool Visible { get; set; }

            public DialogResult ShowDialog(IWin32Window owner)
            {
                _queryAction(Query);
                return DialogResult.OK;
            }

            public void Close()
            {
                throw new NotImplementedException();
            }
        }

        private class QueryViewReturnsOkOnce : IQueryView
        {
            public IntPtr Handle { get; }

            public WorkUnitQuery Query { get; set; }

            public bool Visible { get; set; }

            private int _count;

            public DialogResult ShowDialog(IWin32Window owner)
            {
                if (++_count > 1) return DialogResult.None;
                return DialogResult.OK;
            }

            public void Close()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void HistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndDeletesQuery()
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
        public void HistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndExitsAfterNoAnswer()
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
        public void HistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndFailsToDeleteSelectAllQuery()
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
        public void HistoryPresenter_DeleteWorkUnitClick_AsksYesNoQuestionAndDeletesSelectedRow()
        {
            // Arrange
            var messageBox = new MockMessageBoxPresenter((o, t, c) => DialogResult.Yes);
            var presenter = CreatePresenter(messageBox);
            presenter.Model.HistoryBindingSource.Add(new WorkUnitRow { ID = 1 });
            presenter.Model.HistoryBindingSource.ResetBindings(false);

            presenter.WorkUnitRepository.Expect(x => x.Delete(null)).IgnoreArguments().Return(1);
            // Act
            presenter.DeleteWorkUnitClick();
            // Assert
            Assert.AreEqual(1, messageBox.Invocations.Count);
            presenter.WorkUnitRepository.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_DeleteWorkUnitClick_ShowsMessageBoxWhenNoRowIsSelected()
        {
            // Arrange
            var presenter = CreatePresenter();
            // Act
            presenter.DeleteWorkUnitClick();
            // Assert
            var mockMessageBox = (MockMessageBoxPresenter)presenter.MessageBox;
            Assert.AreEqual(1, mockMessageBox.Invocations.Count);
        }

        [Test]
        public void HistoryPresenter_ExportClick_ProvidesSerializerWithFileNameAndRows()
        {
            // Arrange
            var presenter = CreatePresenter();
            var workUnitRows = new[] { new WorkUnitRow() };
            presenter.WorkUnitRepository.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(workUnitRows);
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
        public void HistoryPresenter_ExportClick_LogsExceptionAndShowsMessageBoxWhenSerializerThrowsException()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.WorkUnitRepository.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(new WorkUnitRow[0]);
            var logger = presenter.Logger;
            logger.Expect(x => x.Error("", null)).IgnoreArguments();
            var saveFile = CreateSaveFileDialogView();
            // Act
            presenter.ExportClick(saveFile, new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowFileSerializerThrows() });
            // Assert
            logger.VerifyAllExpectations();
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
    }
}
