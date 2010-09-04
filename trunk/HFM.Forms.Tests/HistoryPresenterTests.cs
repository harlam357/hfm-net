
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Instances;
using HFM.Models;

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
         SetupResult.For(_queryContainer.QueryList).Return(QueryParameterContainer.NewQueryList());
         _view = _mocks.DynamicMock<IHistoryView>();
         _queryView = _mocks.DynamicMock<IQueryView>();
         _openFileView = _mocks.Stub<IOpenFileDialogView>();
         _saveFileView = _mocks.Stub<ISaveFileDialogView>();
         _messageBoxView = _mocks.DynamicMock<IMessageBoxView>();
         _model = _mocks.Stub<IHistoryPresenterModel>();
      }
      
      private HistoryPresenter NewPresenter()
      {
         return new HistoryPresenter(_prefs, _database, _queryContainer, _view, _queryView, _openFileView, _saveFileView, _messageBoxView, _model);
      }
   
      [Test]
      public void InitializeTest()
      {
         Expect.Call(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty).Repeat.Any();
         Expect.Call(() => _view.AttachPresenter(null)).IgnoreArguments();
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
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.Show();
         _mocks.VerifyAll();
      }

      [Test]
      public void CloseTest()
      {
         Expect.Call(() => _view.Close());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.Close();
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
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.DeleteQueryClick();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      public void DeleteQueryClickFailedTest()
      {
         SetupResult.For(_view.QueryComboSelectedValue).Return(new QueryParameters());
         Expect.Call(() => _messageBoxView.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.DeleteQueryClick();
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
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void QueryComboIndexChangedOutOfRangeTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SelectQuery(1);
      }
      
      [Test]
      public void ImportCompletedUnitsClickTest()
      {
         Expect.Call(_openFileView.ShowDialog(_view)).Return(DialogResult.OK);
         var result = new CompletedUnitsReadResult();
         Expect.Call(_database.ReadCompletedUnits(String.Empty)).IgnoreArguments().Return(result);
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
         Expect.Call(_database.ReadCompletedUnits(String.Empty)).IgnoreArguments().Throw(new IOException());
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
         Expect.Call(() => _database.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments();
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
         Expect.Call(() => _database.WriteCompletedUnitErrorLines(String.Empty, null)).IgnoreArguments().Throw(new IOException());
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
