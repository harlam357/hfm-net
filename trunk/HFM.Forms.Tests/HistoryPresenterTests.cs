
using System;
using System.IO;
using System.Windows.Forms;
using harlam357.Windows.Forms;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Instances;

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
      private IOpenFileDialogView _openFileView;
      private ISaveFileDialogView _saveFileView;
      private IMessageBoxView _messageBoxView;
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
         _openFileView = _mocks.Stub<IOpenFileDialogView>();
         _saveFileView = _mocks.Stub<ISaveFileDialogView>();
         _messageBoxView = _mocks.DynamicMock<IMessageBoxView>();
      }
      
      private HistoryPresenter NewPresenter()
      {
         return new HistoryPresenter(_prefs, _database, _queryContainer, _view, _openFileView, _saveFileView, _messageBoxView);
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
      public void AddQueryFailedTest()
      {
         var parameters = new QueryParameters();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.AddQuery(parameters);
      }

      [Test]
      public void RemoveQueryTest()
      {
         var parameters = new QueryParameters { Name = "Test" };

         Expect.Call(() => _queryContainer.Write());
         Expect.Call(() => _view.QueryComboRefreshList(null)).IgnoreArguments().Repeat.Twice();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _presenter.AddQuery(parameters);
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery("DoesNotExist"); // this is forgiving
         Assert.AreEqual(2, _presenter.NumberOfQueries);
         _presenter.RemoveQuery("Test");
         Assert.AreEqual(1, _presenter.NumberOfQueries);
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void RemoveQueryFailedTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.RemoveQuery(QueryParameters.SelectAll);
      }
      
      [Test]
      public void QueryComboIndexChangedTest()
      {
         Expect.Call(_database.QueryUnitData(null)).IgnoreArguments().Return(null);
         Expect.Call(() => _view.DataGridSetDataSource(null));
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.QueryComboIndexChanged(0);
         _mocks.VerifyAll();
      }

      [Test]
      [ExpectedException(typeof(ArgumentOutOfRangeException))]
      public void QueryComboIndexChangedOutOfRangeTest()
      {
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.QueryComboIndexChanged(1);
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
