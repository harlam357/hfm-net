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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core;
using HFM.Core.Data;

namespace HFM.Forms.Models
{
   [TestFixture]
   public class HistoryPresenterModelTests
   {
      private IWorkUnitRepository _repository;
      private HistoryPresenterModel _model;

      [SetUp]
      public void Init()
      {
         _repository = MockRepository.GenerateMock<IWorkUnitRepository>();
         _repository.Stub(x => x.Connected).Return(true);
         _model = new HistoryPresenterModel(_repository);
      }

      [Test]
      public void HistoryPresenterModel_AddQuery_Test()
      {
         // Arrange
         var query = new WorkUnitQuery("Test")
            .AddParameter(new WorkUnitQueryParameter { Value = 6606 });
         // Act
         Assert.AreEqual(1, _model.QueryBindingSource.Count);
         _model.AddQuery(query);
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
      }

      [Test]
      public void HistoryPresenterModel_AddQuery_SelectAll_Test()
      {
         var query = new WorkUnitQuery();
         Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
      }

      [Test]
      public void HistoryPresenterModel_AddQuery_NameAlreadyExists_Test()
      {
         var query = new WorkUnitQuery("Test")
            .AddParameter(new WorkUnitQueryParameter { Value = 6606 });

         _model.AddQuery(query);
         Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
      }

      [Test]
      public void HistoryPresenterModel_AddQuery_NoQueryFields_Test()
      {
         var query = new WorkUnitQuery("Test");
         Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
      }

      [Test]
      public void HistoryPresenterModel_AddQuery_NoQueryFieldValue_Test()
      {
         var query = new WorkUnitQuery("Test")
            .AddParameter(new WorkUnitQueryParameter());
         Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
      }

      [Test]
      public void HistoryPresenterModel_ReplaceQuery_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         _model.AddQuery(new WorkUnitQuery("Test")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         var newQuery = new WorkUnitQuery("Test2")
            .AddParameter(new WorkUnitQueryParameter { Value = 6606 });
         // Act
         _model.ReplaceQuery(newQuery);
         // Assert
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         Assert.AreEqual("Test2", _model.SelectedWorkUnitQuery.Name);
      }

      [Test]
      public void HistoryPresenterModel_ReplaceQuery_NameExists_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         _model.AddQuery(new WorkUnitQuery("Test")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         _model.AddQuery(new WorkUnitQuery("Test2")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         Assert.AreEqual(3, _model.QueryBindingSource.Count);

         var newQuery = new WorkUnitQuery("Test2")
            .AddParameter(new WorkUnitQueryParameter { Value = 6606 });
         // Act
         _model.QueryBindingSource.Position = 1;
         Assert.Throws<ArgumentException>(() => _model.ReplaceQuery(newQuery));
      }

      [Test]
      public void HistoryPresenterModel_RemoveQuery_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         _model.AddQuery(new WorkUnitQuery("Test")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         // Act
         _model.RemoveQuery(new WorkUnitQuery("DoesNotExist")); // this is forgiving
         Assert.AreEqual(2, _model.QueryBindingSource.Count);
         _model.RemoveQuery(new WorkUnitQuery("Test")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         // Assert
         Assert.AreEqual(1, _model.QueryBindingSource.Count);
      }

      [Test]
      public void HistoryPresenterModel_RemoveQuery_Failed_Test()
      {
         Assert.Throws<ArgumentException>(() => _model.RemoveQuery(new WorkUnitQuery()));
      }

      [Test]
      public void HistoryPresenterModel_ResetBindings_Test()
      {
         // Arrange
         Assert.AreEqual(1, _model.QueryBindingSource.Count);

         _model.AddQuery(new WorkUnitQuery("Test")
             .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
         Assert.AreEqual(2, _model.QueryBindingSource.Count);

         _repository.Expect(x => x.Page(1, 1, null, BonusCalculationType.DownloadTime)).IgnoreArguments().Return(new PetaPoco.Page<WorkUnitRow>());
         // Act
         _model.ResetBindings(true);
         // Assert
         _repository.VerifyAllExpectations();
      }

      [Test]
      public void HistoryPresenterModel_FetchSelectedQuery_Test()
      {
         // Arrange
         _repository.Expect(x => x.Fetch(_model.SelectedWorkUnitQuery, _model.BonusCalculation));
         // Act
         _model.FetchSelectedQuery();
         // Assert
         _repository.VerifyAllExpectations();
      }
   }
}
