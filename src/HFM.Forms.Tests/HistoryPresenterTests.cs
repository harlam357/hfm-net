/*
 * HFM.NET - Work Unit History Presenter Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
      private IQueryParametersCollection _queryCollection;
      private IHistoryView _view;
      private IQueryView _queryView;
      private IOpenFileDialogView _openFileView;
      private ISaveFileDialogView _saveFileView;
      private IMessageBoxView _messageBoxView;

      private IUnitInfoDatabase _database;
      private HistoryPresenterModel _model;
      
      private HistoryPresenter _presenter;
   
      [SetUp]
      public void Init()
      {
         _prefs = MockRepository.GenerateStub<IPreferenceSet>();
         _queryCollection = MockRepository.GenerateStub<IQueryParametersCollection>();
         _view = MockRepository.GenerateMock<IHistoryView>();
         _queryView = MockRepository.GenerateMock<IQueryView>();
         _openFileView = MockRepository.GenerateMock<IOpenFileDialogView>();
         _saveFileView = MockRepository.GenerateMock<ISaveFileDialogView>();
         _messageBoxView = MockRepository.GenerateMock<IMessageBoxView>();

         _database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         _model = new HistoryPresenterModel(_database);
      }
      
      private HistoryPresenter CreatePresenter()
      {
         return new HistoryPresenter(_prefs, _queryCollection, _view, _queryView,
                                     _openFileView, _saveFileView, _messageBoxView, _model);
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
         // Arrange
         var p = new Point();
         var s = new Size();
         var columns = new StringCollection();
         _view.Expect(x => x.Location).Return(p);
         _view.Expect(x => x.Size).Return(s);
         _view.Expect(x => x.GetColumnSettings()).Return(columns);
         _view.Expect(x => x.WindowState).Return(FormWindowState.Normal);
         // Act
         _presenter = CreatePresenter();
         _presenter.OnViewClosing();
         // Assert
         _view.VerifyAllExpectations();
         Assert.AreEqual(p, _model.FormLocation);
         Assert.AreEqual(s, _model.FormSize);
         Assert.AreEqual(columns, _model.FormColumns);
      }

      [Test]
      public void OnClosingWhileNotNormalWindowTest()
      {
         // Arrange
         var r = new Rectangle();
         var columns = new StringCollection();
         _view.Expect(x => x.RestoreBounds).Return(r);
         _view.Expect(x => x.GetColumnSettings()).Return(columns);
         _view.Expect(x => x.WindowState).Return(FormWindowState.Minimized);
         // Act
         _presenter = CreatePresenter();
         _presenter.OnViewClosing();
         // Assert
         _view.VerifyAllExpectations();
         Assert.AreEqual(r.Location, _model.FormLocation);
         Assert.AreEqual(r.Size, _model.FormSize);
         Assert.AreEqual(columns, _model.FormColumns);
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
         _model.AddQuery(parameters);

         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         _queryView.Stub(x => x.Query).Return(parameters2);
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _presenter.EditQueryClick();
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         Assert.AreEqual("Test2", _model.SelectedQuery.Name);
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void EditQueryClickCancelTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);

         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.Cancel);
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         _queryView.VerifyAllExpectations();
      }

      [Test]
      public void EditQueryClickFailedTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);

         _queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         _queryView.Stub(x => x.Query).Return(new QueryParameters());
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         _queryView.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);

         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _presenter.DeleteQueryClick();
         // Assert
         Assert.AreEqual(1, _model.QueryBindingSource.Count);
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickNoTest()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);

         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _presenter.DeleteQueryClick();
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void DeleteQueryClickFailedTest()
      {
         // Arrange
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_view, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteQueryClick();
         // Assert
         _messageBoxView.VerifyAllExpectations();
      }
      
      [Test]
      public void DeleteWorkUnitClickTest()
      {
         // Arrange
         _model.HistoryBindingSource.Add(new HistoryEntry { ID = 1 });

         _messageBoxView.Expect(x => x.AskYesNoQuestion(null, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _database.Expect(x => x.Delete(null)).IgnoreArguments().Return(1);
         _database.Expect(x => x.Fetch(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(new List<HistoryEntry>());
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _messageBoxView.VerifyAllExpectations();
         _database.VerifyAllExpectations();
      }
      
      [Test]
      public void DeleteWorkUnitClickNoSelectionTest()
      {
         // Arrange
         _messageBoxView.Expect(x => x.ShowInformation(null, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _messageBoxView.VerifyAllExpectations();
      }
   }
}
