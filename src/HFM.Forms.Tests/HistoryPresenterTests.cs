/*
 * HFM.NET - Work Unit History Presenter Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Tests
{
   [TestFixture]
   public class HistoryPresenterTests
   {
      private IPreferenceSet _prefs;
      private IUnitInfoDatabase _database;
      private IQueryParametersCollection _queryCollection;
      private IHistoryView _view;
      private IQueryView _queryView;
      //private IProgressDialogView _unitImportView;
      //private ICompletedUnitsFileReader _completedUnitsReader;
      private IOpenFileDialogView _openFileView;
      private ISaveFileDialogView _saveFileView;
      private IMessageBoxView _messageBoxView;
      private IHistoryPresenterModel _model;
      private HistoryPresenter _presenter;
   
      [SetUp]
      public void Init()
      {
         _prefs = MockRepository.GenerateStub<IPreferenceSet>();
         _database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         _queryCollection = new QueryParametersCollectionExt();
         _view = MockRepository.GenerateMock<IHistoryView>();
         _queryView = MockRepository.GenerateMock<IQueryView>();
         //_unitImportView = MockRepository.GenerateMock<IProgressDialogView>();
         //_completedUnitsReader = MockRepository.GenerateMock<ICompletedUnitsFileReader>();
         //SetupResult.For(_unitImportView.ProcessRunner).Return(_completedUnitsReader);
         _openFileView = MockRepository.GenerateMock<IOpenFileDialogView>();
         _saveFileView = MockRepository.GenerateMock<ISaveFileDialogView>();
         _messageBoxView = MockRepository.GenerateMock<IMessageBoxView>();
         // let's use the real thing
         _model = new HistoryPresenterModel(_prefs);
      }
      
      private HistoryPresenter CreatePresenter()
      {
         return new HistoryPresenter(_prefs, _database, _queryCollection, _view, _queryView, // _unitImportView, _completedUnitsReader, 
                                     _openFileView, _saveFileView, _messageBoxView, _model);
      }
   
      [Test]
      public void InitializeTest()
      {
         // use a mock for this test
         _model = MockRepository.GenerateMock<IHistoryPresenterModel>();
         // Arrange
         _prefs.Stub(x => x.ApplicationDataFolderPath).Return(String.Empty);
         _view.Expect(x => x.AttachPresenter(null)).IgnoreArguments();
         _model.Expect(x => x.LoadPreferences());
         _view.Expect(x => x.DataBindModel(_model));
         _view.Expect(x => x.QueryComboRefreshList(null)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.Initialize();
         // Assert
         _view.VerifyAllExpectations();
         _model.VerifyAllExpectations();
      }
      
      [Test]
      public void ShowTest()
      {
         // Arrange
         _view.Expect(x => x.Show());
         _view.Expect(x => x.BringToFront());
         // Act
         _presenter = CreatePresenter();
         _presenter.Show();
         // Assert
         _view.VerifyAllExpectations();
      }

      [Test]
      public void ShowFromMinimizedTest()
      {
         // Arrange
         _view.Expect(x => x.Show());
         _view.Expect(x => x.WindowState).Return(FormWindowState.Minimized);
         _view.Expect(x => x.WindowState = FormWindowState.Normal);
         // Act
         _presenter = CreatePresenter();
         _presenter.Show();
         // Assert
         _view.VerifyAllExpectations();
      }

      [Test]
      public void CloseTest()
      {
         bool presenterClosedFired = false;
         // Act
         _presenter = CreatePresenter();
         _presenter.PresenterClosed += delegate { presenterClosedFired = true; };
         _presenter.Close();
         // Assert
         Assert.AreEqual(true, presenterClosedFired);
      }

      [Test]
      public void OnClosingTest()
      {
         // use a mock for this test
         _model = MockRepository.GenerateMock<IHistoryPresenterModel>();
         // Arrange
         var p = new Point();
         var s = new Size();
         var columns = new StringCollection();
         _view.Expect(x => x.Location).Return(p);
         _view.Expect(x => x.Size).Return(s);
         _view.Expect(x => x.GetColumnSettings()).Return(columns);
         _view.Expect(x => x.WindowState).Return(FormWindowState.Normal);

         _model.Expect(x => x.FormLocation = p);
         _model.Expect(x => x.FormSize = s);
         _model.Expect(x => x.FormColumns = columns);
         _model.Expect(x => x.SavePreferences());
         // Act
         _presenter = CreatePresenter();
         _presenter.OnViewClosing();
         // Assert
         _view.VerifyAllExpectations();
         _model.VerifyAllExpectations();
      }

      [Test]
      public void OnClosingWhileNotNormalWindowTest()
      {
         // use a mock for this test
         _model = MockRepository.GenerateMock<IHistoryPresenterModel>();
         // Arrange
         var r = new Rectangle();
         var columns = new StringCollection();
         _view.Expect(x => x.RestoreBounds).Return(r);
         _view.Expect(x => x.GetColumnSettings()).Return(columns);
         _view.Expect(x => x.WindowState).Return(FormWindowState.Minimized);

         _model.Expect(x => x.FormLocation = r.Location);
         _model.Expect(x => x.FormSize = r.Size);
         _model.Expect(x => x.FormColumns = columns);
         _model.Expect(x => x.SavePreferences());
         // Act
         _presenter = CreatePresenter();
         _presenter.OnViewClosing();
         // Assert
         _view.VerifyAllExpectations();
         _model.VerifyAllExpectations();
      }
      
      [Test]
      public void RefreshClickedTest()
      {
         // Arrange
         _view.Stub(x => x.QueryComboSelectedIndex).Return(0);
         _database.Expect(x => x.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(new List<HistoryEntry>());
         _view.Expect(x => x.DataGridSetDataSource(0, null)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.RefreshClicked();
         // Assert
         _view.VerifyAllExpectations();
         _database.VerifyAllExpectations();
      }

      [Test]
      public void AddQueryTest()
      {
         // Arrange
         _view.Expect(x => x.QueryComboRefreshList(null)).IgnoreArguments();
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         // Assert
         _view.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuerySelectAllTest()
      {
         var parameters = new QueryParameters();
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNameAlreadyExistsTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });

         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNoQueryFieldsTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNoQueryFieldValueTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField());
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      public void ReplaceQueryTest()
      {
         // Arrange
         _view.Expect(x => x.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         _presenter.ReplaceQuery(1, parameters2);
         // Assert
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _view.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ReplaceQueryNameExistsTest()
      {
         _presenter = CreatePresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         _presenter.AddQuery(parameters2);
         Assert.AreEqual(3, _presenter.NumberOfQueries);
         var parameters3 = new QueryParameters { Name = "Test2" };
         parameters3.Fields.Add(new QueryField { Value = 6606 });
         _presenter.ReplaceQuery(1, parameters3);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ReplaceQueryIndexZeroTest()
      {
         _presenter = CreatePresenter();
         _presenter.ReplaceQuery(0, new QueryParameters());
      }

      [Test]
      public void RemoveQueryTest()
      {
         // Arrange
         _view.Expect(x => x.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery(new QueryParameters { Name = "DoesNotExist" }); // this is forgiving
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery(parameters);
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         // Assert
         _view.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void RemoveQueryFailedTest()
      {
         _presenter = CreatePresenter();
         _presenter.RemoveQuery(new QueryParameters());
      }
      
      [Test]
      public void NewQueryClickTest()
      {
         // Arrange
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _queryView.Stub(x => x.Query).Return(parameters);
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void NewQueryClickCancelTest()
      {
         // Arrange
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.Cancel);
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void NewQueryClickFailedTest()
      {
         // Arrange
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         _queryView.Stub(x => x.Query).Return(new QueryParameters());
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         _queryView.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void EditQueryClickTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _view.Stub(x => x.QueryComboSelectedValue).Return(parameters);
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         _view.Stub(x => x.QueryComboSelectedIndex).Return(1);
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         _queryView.Stub(x => x.Query).Return(parameters2);
         // Act
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.EditQueryClick();
         // Assert
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _view.VerifyAllExpectations();
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void EditQueryClickCancelTest()
      {
         // Arrange
         _view.Stub(x => x.QueryComboSelectedValue).Return(new QueryParameters());
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.Cancel);
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         _view.VerifyAllExpectations();
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void EditQueryClickFailedTest()
      {
         // Arrange
         _view.Stub(x => x.QueryComboSelectedValue).Return(new QueryParameters());
         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         _view.Stub(x => x.QueryComboSelectedIndex).Return(1);
         _queryView.Stub(x => x.Query).Return(new QueryParameters());
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         _view.VerifyAllExpectations();
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _view.Stub(x => x.QueryComboSelectedValue).Return(parameters);
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         // Act
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.DeleteQueryClick();
         // Assert
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _view.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickNoTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _view.Stub(x => x.QueryComboSelectedValue).Return(parameters);
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
         // Act
         _presenter = CreatePresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.DeleteQueryClick();
         // Assert
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _view.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickFailedTest()
      {
         // Arrange
         _view.Stub(x => x.QueryComboSelectedValue).Return(new QueryParameters());
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteQueryClick();
         // Assert
         _view.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }
      
      [Test]
      public void DeleteWorkUnitClickTest()
      {
         // Arrange
         _view.Stub(x => x.DataGridSelectedHistoryEntry).Return(new HistoryEntry { ID = 1 });
         _messageBoxView.Expect(x => x.AskYesNoQuestion(null, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _database.Expect(x => x.DeleteUnitInfo(1)).Return(1);
         _database.Expect(x => x.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(new List<HistoryEntry>());
         _view.Expect(x => x.DataGridSetDataSource(0, null)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _view.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
         _database.VerifyAllExpectations();
      }
      
      [Test]
      public void DeleteWorkUnitClickNoSelectionTest()
      {
         // Arrange
         _view.Stub(x => x.DataGridSelectedHistoryEntry).Return(null);
         _messageBoxView.Expect(x => x.ShowInformation(null, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _view.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }
      
      [Test]
      public void SelectQueryTest()
      {
         // Arrange
         _database.Expect(x => x.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(new List<HistoryEntry>());
         _view.Expect(x => x.DataGridSetDataSource(0, null)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.SelectQuery(0);
         // Assert
         _database.VerifyAllExpectations();
         _view.VerifyAllExpectations();
      }

      [Test]
      public void SelectQueryShowFirstCheckedTest()
      {
         // Arrange
         var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
         _database.Expect(x => x.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         _view.Expect(x => x.DataGridSetDataSource(0, null)).IgnoreArguments();
         // Act
         _model.ShowFirstChecked = true;
         _model.ShowEntriesValue = 1;
         _presenter = CreatePresenter();
         _presenter.SelectQuery(0);

         // should be able to assert that only one entry appears 
         // in the shown entries list, but it's currently not exposed

         // Assert
         _database.VerifyAllExpectations();
         _view.VerifyAllExpectations();
      }

      [Test]
      public void SelectQueryShowLastCheckedTest()
      {
         // Arrange
         var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
         _database.Expect(x => x.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         _view.Expect(x => x.DataGridSetDataSource(0, null)).IgnoreArguments();
         // Act
         _model.ShowLastChecked = true;
         _model.ShowEntriesValue = 1;
         _presenter = CreatePresenter();
         _presenter.SelectQuery(0);

         // should be able to assert that only one entry appears 
         // in the shown entries list, but it's currently not exposed

         // Assert
         _database.VerifyAllExpectations();
         _view.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void QueryComboIndexChangedOutOfRangeTest()
      {
         _presenter = CreatePresenter();
         _presenter.SelectQuery(1);
      }
      
      [Test]
      public void SaveSortSettingsTest()
      {
         _presenter = CreatePresenter();
         Assert.AreEqual(null, _model.SortColumnName);
         Assert.AreEqual(SortOrder.None, _model.SortOrder);
         _presenter.SaveSortSettings("Test", SortOrder.Ascending);
         Assert.AreEqual("Test", _model.SortColumnName);
         Assert.AreEqual(SortOrder.Ascending, _model.SortOrder);
      }
      
      //[Test]
      //public void ImportCompletedUnitsClickTest()
      //{
      //   Expect.Call(_openFileView.ShowDialog(_view)).Return(DialogResult.OK);
      //   var result = new CompletedUnitsReadResult();
      //   Expect.Call(() => _unitImportView.Process());
      //   SetupResult.For(_completedUnitsReader.Result).Return(result);
      //   Expect.Call(() => _database.ImportCompletedUnits(result.Entries));
      //   _mocks.ReplayAll();
      //   _presenter = CreatePresenter();
      //   _presenter.ImportCompletedUnitsClick();
      //   _mocks.VerifyAll();
      //}

      //[Test]
      //public void ImportCompletedUnitsClickImportFailedTest()
      //{
      //   Expect.Call(_openFileView.ShowDialog(_view)).Return(DialogResult.OK);
      //   Expect.Call(() => _unitImportView.Process());
      //   SetupResult.For(_completedUnitsReader.Exception).Return(new IOException());
      //   _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
      //   _mocks.ReplayAll();
      //   _presenter = CreatePresenter();
      //   _presenter.ImportCompletedUnitsClick();
      //   _mocks.VerifyAll();
      //}
      
      //[Test]
      //public void ShowImportResultDialogTest()
      //{
      //   _messageBoxView.Expect(x => x.ShowInformation(_view, String.Empty, String.Empty)).IgnoreArguments();
      //   _mocks.ReplayAll();
      //   _presenter = CreatePresenter();
      //   _presenter.ShowImportResultDialog(new CompletedUnitsReadResult());
      //   _mocks.VerifyAll();
      //}

      //[Test]
      //public void ShowImportResultDialogWithErrorsTest()
      //{
      //   _messageBoxView.Stub(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
      //   Expect.Call(_saveFileView.ShowDialog(_view)).Return(DialogResult.OK);
      //   Expect.Call(() => _completedUnitsReader.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments();
      //   _mocks.ReplayAll();
      //   _presenter = CreatePresenter();
      //   var result = new CompletedUnitsReadResult();
      //   result.ErrorLines.Add("error");
      //   result.Duplicates++;
      //   _presenter.ShowImportResultDialog(result);
      //   _mocks.VerifyAll();
      //}

      //[Test]
      //public void ShowImportResultDialogWithErrorsWriteFailedTest()
      //{
      //   _messageBoxView.Stub(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
      //   Expect.Call(_saveFileView.ShowDialog(_view)).Return(DialogResult.OK);
      //   Expect.Call(() => _completedUnitsReader.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments().Throw(new IOException());
      //   _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
      //   _mocks.ReplayAll();
      //   _presenter = CreatePresenter();
      //   var result = new CompletedUnitsReadResult();
      //   result.ErrorLines.Add("error");
      //   _presenter.ShowImportResultDialog(result);
      //   _mocks.VerifyAll();
      //}
   }

   public class QueryParametersCollectionExt : QueryParametersCollection
   {
      public override void Write()
      {
         // do nothing
      }
   }
}
