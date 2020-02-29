/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using Castle.Core.Logging;
using harlam357.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Serializers;
using HFM.Preferences;

namespace HFM.Forms
{
    [TestFixture]
    public class HistoryPresenterTests
    {
        private HistoryPresenter CreatePresenter()
        {
            var repository = MockRepository.GenerateMock<IWorkUnitRepository>();
            repository.Stub(x => x.Connected).Return(true);
            return new HistoryPresenter(MockRepository.GenerateMock<ILogger>(),
                                        new InMemoryPreferenceSet(), 
                                        new WorkUnitQueryDataContainer(), 
                                        MockRepository.GenerateMock<IHistoryView>(), 
                                        MockRepository.GenerateStub<IViewFactory>(), 
                                        MockRepository.GenerateMock<IMessageBoxView>(), 
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
            presenter.MessageBoxView.Expect(x => x.ShowError(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments();
            // Act
            presenter.NewQueryClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
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
            presenter.MessageBoxView.Expect(x => x.ShowError(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments();
            // Act
            presenter.EditQueryClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
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
            var presenter = CreatePresenter();
            presenter.Model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, presenter.Model.QueryBindingSource.Count);
            presenter.MessageBoxView.Expect(x => x.AskYesNoQuestion(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
            // Act
            presenter.DeleteQueryClick();
            // Assert
            Assert.AreEqual(1, presenter.Model.QueryBindingSource.Count);
            presenter.MessageBoxView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndExitsAfterNoAnswer()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.MessageBoxView.Expect(x => x.AskYesNoQuestion(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
            // Act
            presenter.DeleteQueryClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_DeleteQueryClick_AsksYesNoQuestionAndFailsToDeleteSelectAllQuery()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.MessageBoxView.Expect(x => x.AskYesNoQuestion(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
            presenter.MessageBoxView.Expect(x => x.ShowError(presenter.HistoryView, String.Empty, String.Empty)).IgnoreArguments();
            // Act
            presenter.DeleteQueryClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_DeleteWorkUnitClick_AsksYesNoQuestionAndDeletesSelectedRow()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.Model.HistoryBindingSource.Add(new WorkUnitRow { ID = 1 });
            presenter.Model.HistoryBindingSource.ResetBindings(false);

            presenter.MessageBoxView.Expect(x => x.AskYesNoQuestion(null, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
            presenter.WorkUnitRepository.Expect(x => x.Delete(null)).IgnoreArguments().Return(1);
            // Act
            presenter.DeleteWorkUnitClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
            presenter.WorkUnitRepository.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_DeleteWorkUnitClick_ShowsMessageBoxWhenNoRowIsSelected()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.MessageBoxView.Expect(x => x.ShowInformation(null, String.Empty, String.Empty)).IgnoreArguments();
            // Act
            presenter.DeleteWorkUnitClick();
            // Assert
            presenter.MessageBoxView.VerifyAllExpectations();
        }

        [Test]
        public void HistoryPresenter_ExportClick_ProvidesSerializerWithFileNameAndRows()
        {
            // Arrange
            var presenter = CreatePresenter();
            presenter.ViewFactory.Stub(x => x.GetSaveFileDialogView()).Return(CreateSaveFileDialogView());
            var workUnitRows = new[] { new WorkUnitRow() };
            presenter.WorkUnitRepository.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(workUnitRows);
            var serializer = new WorkUnitRowFileSerializerSavesFileNameAndRows();
            // Act
            presenter.ExportClick(new List<IFileSerializer<List<WorkUnitRow>>> { serializer });
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
            presenter.ViewFactory.Stub(x => x.GetSaveFileDialogView()).Return(CreateSaveFileDialogView());
            presenter.WorkUnitRepository.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(new WorkUnitRow[0]);

            var logger = presenter.Logger;
            logger.Expect(x => x.ErrorFormat((Exception)null, "", new object[0])).IgnoreArguments();
            presenter.MessageBoxView.Expect(x => x.ShowError(null, null, null)).IgnoreArguments();
            // Act
            presenter.ExportClick(new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowFileSerializerThrows() });
            // Assert
            logger.VerifyAllExpectations();
            presenter.MessageBoxView.VerifyAllExpectations();
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

        private static ISaveFileDialogView CreateSaveFileDialogView()
        {
            var saveFileDialogView = MockRepository.GenerateStub<ISaveFileDialogView>();
            saveFileDialogView.FileName = "test.csv";
            saveFileDialogView.Stub(x => x.FilterIndex).Return(1);
            saveFileDialogView.Stub(x => x.ShowDialog()).Return(DialogResult.OK);
            return saveFileDialogView;
        }
    }
}
