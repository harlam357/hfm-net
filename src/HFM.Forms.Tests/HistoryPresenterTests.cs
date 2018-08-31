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
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

using Castle.Core.Logging;
using harlam357.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Data.SQLite;
using HFM.Core.DataTypes;
using HFM.Core.Serializers;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
   [TestFixture]
   public class HistoryPresenterTests
   {
      private IPreferenceSet _prefs;
      private IQueryParametersContainer _queryContainer;
      private IHistoryView _view;
      private IViewFactory _viewFactory;
      private IMessageBoxView _messageBoxView;

      private IUnitInfoDatabase _database;
      private HistoryPresenterModel _model;

      private HistoryPresenter _presenter;

      [SetUp]
      public void Init()
      {
         _prefs = MockRepository.GenerateStub<IPreferenceSet>();
         _queryContainer = MockRepository.GenerateStub<IQueryParametersContainer>();
         _view = MockRepository.GenerateMock<IHistoryView>();
         _viewFactory = MockRepository.GenerateMock<IViewFactory>();
         _messageBoxView = MockRepository.GenerateMock<IMessageBoxView>();

         _database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         _database.Stub(x => x.Connected).Return(true);
         _model = new HistoryPresenterModel(_database);
      }

      private HistoryPresenter CreatePresenter()
      {
         return new HistoryPresenter(_prefs, _queryContainer, _view, _viewFactory, _messageBoxView, _database, _model);
      }

      [Test]
      public void HistoryPresenter_Show_Test()
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
      public void HistoryPresenter_Show_FromMinimized_Test()
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
      public void HistoryPresenter_Close_Test()
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
      public void HistoryPresenter_OnClosing_Test()
      {
         // Arrange
         var p = new Point();
         var s = new Size();
         var columns = new List<string>();
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
      public void HistoryPresenter_OnClosing_WhileNotNormalWindow_Test()
      {
         // Arrange
         var r = new Rectangle();
         var columns = new List<string>();
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
      public void HistoryPresenter_NewQueryClick_Test()
      {
         // Arrange
         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         queryView.Stub(x => x.Query).Return(parameters);
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_NewQueryClick_Cancel_Test()
      {
         // Arrange
         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.Cancel);
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_NewQueryClick_Failed_Test()
      {
         // Arrange
         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK).Repeat.Once();
         queryView.Stub(x => x.Query).Return(new QueryParameters());
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.NewQueryClick();
         // Assert
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_EditQueryClick_Test()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);

         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK);
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         queryView.Stub(x => x.Query).Return(parameters2);
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         // Act
         _presenter = CreatePresenter();
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _presenter.EditQueryClick();
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         Assert.AreEqual("Test2", _model.SelectedQuery.Name);
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_EditQueryClick_Cancel_Test()
      {
         // Arrange
         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.Cancel);
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_EditQueryClick_Failed_Test()
      {
         // Arrange
         var queryView = MockRepository.GenerateMock<IQueryView>();
         queryView.Expect(x => x.ShowDialog(_view)).Return(DialogResult.OK).Repeat.Once();
         //queryView.Stub(x => x.Query).Return(new QueryParameters());
         _viewFactory.Expect(x => x.GetQueryDialog()).Return(queryView);
         _messageBoxView.Expect(x => x.ShowError(_view, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.EditQueryClick();
         // Assert
         queryView.VerifyAllExpectations();
         _viewFactory.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_DeleteQueryClick_Test()
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
      public void HistoryPresenter_DeleteQueryClick_No_Test()
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
      public void HistoryPresenter_DeleteQueryClick_Failed_Test()
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
      public void HistoryPresenter_DeleteWorkUnitClick_Test()
      {
         // Arrange
         _model.HistoryBindingSource.Add(new HistoryEntry { ID = 1 });

         _messageBoxView.Expect(x => x.AskYesNoQuestion(null, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _database.Expect(x => x.Delete(null)).IgnoreArguments().Return(1);
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _messageBoxView.VerifyAllExpectations();
         _database.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_DeleteWorkUnitClick_NoSelection_Test()
      {
         // Arrange
         _messageBoxView.Expect(x => x.ShowInformation(null, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.DeleteWorkUnitClick();
         // Assert
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_ExportClick_Test()
      {
         // Arrange
         _database.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(new HistoryEntry[0]);

         var saveFileDialogView = MockRepository.GenerateMock<ISaveFileDialogView>();
         saveFileDialogView.Expect(x => x.FileName).Return("test.csv");
         saveFileDialogView.Expect(x => x.FilterIndex).Return(1);
         saveFileDialogView.Expect(x => x.ShowDialog()).Return(DialogResult.OK);
         _viewFactory.Expect(x => x.GetSaveFileDialogView()).Return(saveFileDialogView);
         _viewFactory.Expect(x => x.Release(saveFileDialogView));

         var serializer = MockRepository.GenerateMock<IFileSerializer<List<HistoryEntry>>>();
         serializer.Expect(x => x.Serialize(null, null)).Constraints(new Equal("test.csv"), new TypeOf(typeof(List<HistoryEntry>)));
         // Act
         _presenter = CreatePresenter();
         _presenter.ExportSerializers = new[] { serializer };
         _presenter.ExportClick();
         // Assert
         _viewFactory.VerifyAllExpectations();
         saveFileDialogView.VerifyAllExpectations();
         serializer.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenter_ExportClick_Exception_Test()
      {
         // Arrange
         _database.Stub(x => x.Fetch(null, 0)).IgnoreArguments().Return(new HistoryEntry[0]);

         var saveFileDialogView = MockRepository.GenerateMock<ISaveFileDialogView>();
         saveFileDialogView.Expect(x => x.FileName).Return("test.csv");
         saveFileDialogView.Expect(x => x.FilterIndex).Return(1);
         saveFileDialogView.Expect(x => x.ShowDialog()).Return(DialogResult.OK);
         _viewFactory.Expect(x => x.GetSaveFileDialogView()).Return(saveFileDialogView);
         _viewFactory.Expect(x => x.Release(saveFileDialogView));

         var exception = new IOException();
         var serializer = MockRepository.GenerateMock<IFileSerializer<List<HistoryEntry>>>();
         serializer.Expect(x => x.Serialize(null, null)).Constraints(new Equal("test.csv"), new TypeOf(typeof(List<HistoryEntry>)))
            .Throw(exception);

         var logger = MockRepository.GenerateMock<ILogger>();
         logger.Expect(x => x.ErrorFormat(exception, "", new object[0])).IgnoreArguments();
         _messageBoxView.Expect(x => x.ShowError(null, null, null)).IgnoreArguments();

         // Act
         _presenter = CreatePresenter();
         _presenter.ExportSerializers = new[] { serializer };
         _presenter.Logger = logger;
         _presenter.ExportClick();
         // Assert
         _viewFactory.VerifyAllExpectations();
         saveFileDialogView.VerifyAllExpectations();
         serializer.VerifyAllExpectations();
         logger.VerifyAllExpectations();
         _messageBoxView.VerifyAllExpectations();
      }
   }
}
