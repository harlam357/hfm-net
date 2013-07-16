/*
 * HFM.NET - Work Unit History - Binding Model Tests
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Tests.Models
{
   [TestFixture]
   public class HistoryPresenterModelTests
   {
      private IUnitInfoDatabase _database;
      private HistoryPresenterModel _model;

      [SetUp]
      public void Init()
      {
         _database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         _database.Stub(x => x.Connected).Return(true);
         _model = new HistoryPresenterModel(_database);
      }

      [Test]
      public void AddQuery_Test()
      {
         // Arrange
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         // Act
         Assert.AreEqual(1, _model.QueryBindingSource.Count);
         _model.AddQuery(parameters);
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuery_SelectAll_Test()
      {
         var parameters = new QueryParameters();
         _model.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuery_NameAlreadyExists_Test()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });

         _model.AddQuery(parameters);
         _model.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuery_NoQueryFields_Test()
      {
         var parameters = new QueryParameters { Name = "Test" };
         _model.AddQuery(parameters);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void AddQuery_NoQueryFieldValue_Test()
      {
         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField());
         _model.AddQuery(parameters);
      }

      [Test]
      public void ReplaceQuery_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         // Act
         _model.ReplaceQuery(parameters2);
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         Assert.AreEqual("Test2", _model.SelectedQuery.Name);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ReplaceQuery_NameExists_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         
         var parameters2 = new QueryParameters { Name = "Test2" };
         parameters2.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters2);
         Assert.AreEqual(3, _model.QueryBindingSource.Count);

         var parameters3 = new QueryParameters { Name = "Test2" };
         parameters3.Fields.Add(new QueryField { Value = 6606 });
         // Act
         _model.QueryBindingSource.Position = 1;
         _model.ReplaceQuery(parameters3);
      }

      [Test]
      public void RemoveQuery_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         // Act
         _model.RemoveQuery(new QueryParameters { Name = "DoesNotExist" }); // this is forgiving
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _model.RemoveQuery(parameters);
         // Assert
         Assert.AreEqual(1, _model.QueryBindingSource.Count);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void RemoveQuery_Failed_Test()
      {
         _model.RemoveQuery(new QueryParameters());
      }

      [Test]
      public void ResetBindings_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         var parameters = new QueryParameters { Name = "Test" };
         parameters.Fields.Add(new QueryField { Value = 6606 });
         _model.AddQuery(parameters);
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         
         _database.Expect(x => x.Page(1, 1, null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(new PetaPoco.Page<HistoryEntry>());
         // Act
         _model.ResetBindings(true);
         // Assert
         _database.VerifyAllExpectations();
      }

      //[Test]
      //public void ResetBindingsShowFirstCheckedTest()
      //{
      //   // Arrange
      //   var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
      //   _database.Expect(x => x.Fetch(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
      //   // Act
      //   _model.ShowFirstChecked = true;
      //   _model.ShowEntriesValue = 1;
      //   _model.ResetBindings(true);
      //   // Assert
      //   Assert.AreEqual(1, _model.HistoryBindingSource.Count);
      //   _database.VerifyAllExpectations();
      //}

      //[Test]
      //public void ResetBindingsShowLastCheckedTest()
      //{
      //   // Arrange
      //   var entries = new List<HistoryEntry> { new HistoryEntry(), new HistoryEntry() };
      //   _database.Expect(x => x.Fetch(null, HistoryProductionView.BonusDownloadTime)).IgnoreArguments().Return(entries);
      //   // Act
      //   _model.ShowLastChecked = true;
      //   _model.ShowEntriesValue = 1;
      //   _model.ResetBindings(true);
      //   // Assert
      //   Assert.AreEqual(1, _model.HistoryBindingSource.Count);
      //   _database.VerifyAllExpectations();
      //}
   }
}
