/*
 * HFM.NET - Work Unit History Presenter Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Windows.Forms;

using HFM.Forms.Models;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Tests
{
   [TestFixture]
   public class HistoryPresenterTests
   {
      private MockRepository _mocks;
   
      private IPreferenceSet _prefs;
      private IUnitInfoDatabase _database;
      private IQueryParameterContainer _queryContainer;
      private IHistoryView _view;
      private IQueryView _queryView;
      private IProgressDialogView _unitImportView;
      private ICompletedUnitsFileReader _completedUnitsReader;
      private IOpenFileDialogView _openFileView;
      private ISaveFileDialogView _saveFileView;
      private IMessageBoxView _messageBoxView;
      private IHistoryPresenterModel _model;
      private HistoryPresenter _presenter;
   
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         _database = _mocks.DynamicMock<IUnitInfoDatabase>();
         _queryContainer = _mocks.DynamicMock<IQueryParameterContainer>();
         var list = new List<QueryParameters> { new QueryParameters() };
         SetupResult.For(_queryContainer.QueryList).Return(list);
         _view = _mocks.DynamicMock<IHistoryView>();
         _queryView = _mocks.DynamicMock<IQueryView>();
         _unitImportView = _mocks.DynamicMock<IProgressDialogView>();
         _completedUnitsReader = _mocks.DynamicMock<ICompletedUnitsFileReader>();
         SetupResult.For(_unitImportView.ProcessRunner).Return(_completedUnitsReader);
         _openFileView = _mocks.DynamicMock<IOpenFileDialogView>();
         _saveFileView = _mocks.DynamicMock<ISaveFileDialogView>();
         _messageBoxView = _mocks.DynamicMock<IMessageBoxView>();
         // let's use the real thing
         _model = new HistoryPresenterModel(_prefs);
      }
      
      private HistoryPresenter NewPresenter()
      {
         return new HistoryPresenter(_prefs, _database, _queryContainer, _view, _queryView, _unitImportView,
                                     _completedUnitsReader, _openFileView, _saveFileView, _messageBoxView, _model);
      }
   
      [Test]
      public void InitializeTest()
      {
         // use a mock for this test
         _model = _mocks.DynamicMock<IHistoryPresenterModel>();
      
         Expect.Call(_prefs.ApplicationDataFolderPath).Return(String.Empty).Repeat.Any();
         Expect.Call(() => _view.AttachPresenter(null)).IgnoreArguments();
         Expect.Call(() => _model.LoadPreferences());
         Expect.Call(() => _view.DataBindModel(_model));
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.Initialize();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void ShowTest()
      {
         Expect.Call(() => _view.Show());
         Expect.Call(() => _view.BringToFront());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.Show();
         _mocks.VerifyAll();
      }

      [Test]
      public void ShowFromMinimizedTest()
      {
         Expect.Call(() => _view.Show());
         Expect.Call(_view.WindowState).Return(FormWindowState.Minimized);
         Expect.Call(_view.WindowState = FormWindowState.Normal);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.Show();
         _mocks.VerifyAll();
      }

      [Test]
      public void CloseTest()
      {
         bool presenterClosedFired = false;
         
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.PresenterClosed += delegate { presenterClosedFired = true; };
         _presenter.Close();
         _mocks.VerifyAll();
         
         Assert.AreEqual(true, presenterClosedFired);
      }

      [Test]
      public void OnClosingTest()
      {
         var p = new Point();
         var s = new Size();
         var columns = new StringCollection();
      
         Expect.Call(_view.Location).Return(p);
         Expect.Call(_view.Size).Return(s);
         Expect.Call(_view.GetColumnSettings()).Return(columns);
      
         Expect.Call(_view.WindowState).Return(FormWindowState.Normal);
         Expect.Call(_model.FormLocation = p);
         Expect.Call(_model.FormSize = s);
         Expect.Call(_model.FormColumns = columns);
         Expect.Call(() => _model.SavePreferences());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.OnViewClosing();
         _mocks.VerifyAll();
      }

      [Test]
      public void OnClosingWhileNotNormalWindowTest()
      {
         var r = new Rectangle();
         var columns = new StringCollection();

         Expect.Call(_view.RestoreBounds).Return(r);
         Expect.Call(_view.GetColumnSettings()).Return(columns);

         Expect.Call(_view.WindowState).Return(FormWindowState.Minimized);
         Expect.Call(_model.FormLocation = r.Location);
         Expect.Call(_model.FormSize = r.Size);
         Expect.Call(_model.FormColumns = columns);
         Expect.Call(() => _model.SavePreferences());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.OnViewClosing();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void RefreshClickedTest()
      {
         SetupResult.For(_view.QueryComboSelectedIndex).Return(0);
         var entries = new List<HistoryEntry>();
         Expect.Call(_database.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         Expect.Call(() => _view.DataGridSetDataSource(0, null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.RefreshClicked();
         _mocks.VerifyAll();
      }

      [Test]
      public void AddQueryTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });

         Expect.Call(() => _queryContainer.Write());
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuerySelectAllTest()
      {
         var parameters = new QueryParameters();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNameAlreadyExistsTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });

         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNoQueryFieldsTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQueryNoQueryFieldValueTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      public void ReplaceQueryTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });

         Expect.Call(() => _queryContainer.Write()).Repeat.Twice();
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.ReplaceQuery(1, parameters2);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ReplaceQueryNameExistsTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         var parameters3 = new QueryParameters { Name = "Test2" };
         parameters3.Fields.Add(new QueryField { Value = 6606 });

         Expect.Call(() => _queryContainer.Write()).Repeat.Twice();
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters2);
         Assert.AreEqual(3, _presenter.NumberOfQueries);
         _presenter.ReplaceQuery(1, parameters3);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ReplaceQueryIndexZeroTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.ReplaceQuery(0, new QueryParameters());
         _mocks.VerifyAll();
      }

      [Test]
      public void RemoveQueryTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });

         Expect.Call(() => _queryContainer.Write()).Repeat.Twice();
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery(new QueryParameters { Name = "DoesNotExist" }); // this is forgiving
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery(parameters);
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void RemoveQueryFailedTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.RemoveQuery(new QueryParameters());
      }
      
      [Test]
      public void NewQueryClickTest()
      {
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.OK);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         SetupResult.For(_queryView.Query).Return(parameters);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.NewQueryClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void NewQueryClickCancelTest()
      {
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.Cancel);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.NewQueryClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void NewQueryClickFailedTest()
      {
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.OK);
         SetupResult.For(_queryView.Query).Return(new QueryParameters());
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.NewQueryClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void EditQueryClickTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         SetupResult.For(_view.QueryComboSelectedValue).Return(parameters);
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.OK);
         SetupResult.For(_view.QueryComboSelectedIndex).Return(1);
         SetupResult.For(_queryView.Query).Return(parameters2);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.EditQueryClick();
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      public void EditQueryClickCancelTest()
      {
         SetupResult.For(_view.QueryComboSelectedValue).Return(new QueryParameters());
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.Cancel);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.EditQueryClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void EditQueryClickFailedTest()
      {
         SetupResult.For(_view.QueryComboSelectedValue).Return(new QueryParameters());
         Expect.Call(_queryView.ShowDialog(_view)).Return(DialogResult.OK);
         SetupResult.For(_view.QueryComboSelectedIndex).Return(1);
         SetupResult.For(_queryView.Query).Return(new QueryParameters());
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.EditQueryClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void DeleteQueryClickTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         SetupResult.For(_view.QueryComboSelectedValue).Return(parameters);
         Expect.Call(_messageBoxView.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.DeleteQueryClick();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      public void DeleteQueryClickNoTest()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         SetupResult.For(_view.QueryComboSelectedValue).Return(parameters);
         Expect.Call(_messageBoxView.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.DeleteQueryClick();
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      public void DeleteQueryClickFailedTest()
      {
         SetupResult.For(_view.QueryComboSelectedValue).Return(new QueryParameters());
         Expect.Call(_messageBoxView.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.DeleteQueryClick();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void DeleteWorkUnitClickTest()
      {
         SetupResult.For(_view.DataGridSelectedHistoryEntry).Return(new HistoryEntry { ID = 1 });
         Expect.Call(_messageBoxView.AskYesNoQuestion(null, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         Expect.Call(_database.DeleteUnitInfo(1)).Return(1);
         var entries = new List<HistoryEntry>();
         Expect.Call(_database.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         Expect.Call(() => _view.DataGridSetDataSource(0, null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.DeleteWorkUnitClick();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void DeleteWorkUnitClickNoSelectionTest()
      {
         SetupResult.For(_view.DataGridSelectedHistoryEntry).Return(null);
         Expect.Call(() => _messageBoxView.ShowInformation(null, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.DeleteWorkUnitClick();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void SelectQueryTest()
      {
         var entries = new List<HistoryEntry>();
         Expect.Call(_database.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         Expect.Call(() => _view.DataGridSetDataSource(0, null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SelectQuery(0);
         _mocks.VerifyAll();
      }

      [Test]
      public void SelectQueryShowFirstCheckedTest()
      {
         var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
         Expect.Call(_database.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         Expect.Call(() => _view.DataGridSetDataSource(0, null)).IgnoreArguments();
         _model.ShowFirstChecked = true;
         _model.ShowEntriesValue = 1;
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SelectQuery(0);
         // should be able to assert that only one entry appears 
         // in the shown entries list, but it's currently not exposed
         _mocks.VerifyAll();
      }

      [Test]
      public void SelectQueryShowLastCheckedTest()
      {
         var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
         Expect.Call(_database.QueryUnitData(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
         Expect.Call(() => _view.DataGridSetDataSource(0, null)).IgnoreArguments();
         _model.ShowLastChecked = true;
         _model.ShowEntriesValue = 1;
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SelectQuery(0);
         // should be able to assert that only one entry appears 
         // in the shown entries list, but it's currently not exposed
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void QueryComboIndexChangedOutOfRangeTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SelectQuery(1);
      }
      
      [Test]
      public void SaveSortSettingsTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(null, _model.SortColumnName);
         Assert.AreEqual(SortOrder.None, _model.SortOrder);
         _presenter.SaveSortSettings("Test", SortOrder.Ascending);
         Assert.AreEqual("Test", _model.SortColumnName);
         Assert.AreEqual(SortOrder.Ascending, _model.SortOrder);
      }
      
      [Test]
      public void ImportCompletedUnitsClickTest()
      {
         Expect.Call(_openFileView.ShowDialog(_view)).Return(DialogResult.OK);
         var result = new CompletedUnitsReadResult();
         Expect.Call(() => _unitImportView.Process());
         SetupResult.For(_completedUnitsReader.Result).Return(result);
         Expect.Call(() => _database.ImportCompletedUnits(result.Entries));
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.ImportCompletedUnitsClick();
         _mocks.VerifyAll();
      }

      [Test]
      public void ImportCompletedUnitsClickImportFailedTest()
      {
         Expect.Call(_openFileView.ShowDialog(_view)).Return(DialogResult.OK);
         Expect.Call(() => _unitImportView.Process());
         SetupResult.For(_completedUnitsReader.Exception).Return(new IOException());
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.ImportCompletedUnitsClick();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void ShowImportResultDialogTest()
      {
         Expect.Call(() => _messageBoxView.ShowInformation(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.ShowImportResultDialog(new CompletedUnitsReadResult());
         _mocks.VerifyAll();
      }

      [Test]
      public void ShowImportResultDialogWithErrorsTest()
      {
         Expect.Call(_messageBoxView.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         Expect.Call(_saveFileView.ShowDialog(_view)).Return(DialogResult.OK);
         Expect.Call(() => _completedUnitsReader.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         var result = new CompletedUnitsReadResult();
         result.ErrorLines.Add("error");
         result.Duplicates++;
         _presenter.ShowImportResultDialog(result);
         _mocks.VerifyAll();
      }

      [Test]
      public void ShowImportResultDialogWithErrorsWriteFailedTest()
      {
         Expect.Call(_messageBoxView.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         Expect.Call(_saveFileView.ShowDialog(_view)).Return(DialogResult.OK);
         Expect.Call(() => _completedUnitsReader.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments().Throw(new IOException());
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         var result = new CompletedUnitsReadResult();
         result.ErrorLines.Add("error");
         _presenter.ShowImportResultDialog(result);
         _mocks.VerifyAll();
      }
   }
}
