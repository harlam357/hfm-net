using System;

using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Preferences;

using NUnit.Framework;

using Rhino.Mocks;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class WorkUnitHistoryModelTests
    {
        private IWorkUnitRepository _repository;
        private WorkUnitHistoryModel _model;

        [SetUp]
        public void Init()
        {
            _repository = MockRepository.GenerateMock<IWorkUnitRepository>();
            _repository.Stub(x => x.Connected).Return(true);
            _model = new WorkUnitHistoryModel(new InMemoryPreferencesProvider(), new WorkUnitQueryDataContainer(), _repository);
        }

        [Test]
        public void WorkUnitHistoryModel_AddQuery_Test()
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
        public void WorkUnitHistoryModel_AddQuery_SelectAll_Test()
        {
            var query = WorkUnitQuery.SelectAll;
            Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
        }

        [Test]
        public void WorkUnitHistoryModel_AddQuery_NameAlreadyExists_Test()
        {
            var query = new WorkUnitQuery("Test")
               .AddParameter(new WorkUnitQueryParameter { Value = 6606 });

            _model.AddQuery(query);
            Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
        }

        [Test]
        public void WorkUnitHistoryModel_AddQuery_NoQueryFields_Test()
        {
            var query = new WorkUnitQuery("Test");
            Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
        }

        [Test]
        public void WorkUnitHistoryModel_AddQuery_NoQueryFieldValue_Test()
        {
            var query = new WorkUnitQuery("Test")
               .AddParameter(new WorkUnitQueryParameter());
            Assert.Throws<ArgumentException>(() => _model.AddQuery(query));
        }

        [Test]
        public void WorkUnitHistoryModel_ReplaceQuery_Test()
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
        public void WorkUnitHistoryModel_ReplaceQuery_NameExists_Test()
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
        public void WorkUnitHistoryModel_RemoveQuery_Test()
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
        public void WorkUnitHistoryModel_RemoveQuery_Failed_Test()
        {
            Assert.Throws<ArgumentException>(() => _model.RemoveQuery(WorkUnitQuery.SelectAll));
        }

        [Test]
        public void WorkUnitHistoryModel_ResetBindings_Test()
        {
            // Arrange
            Assert.AreEqual(1, _model.QueryBindingSource.Count);

            _model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, _model.QueryBindingSource.Count);

            _repository.Expect(x => x.Page(1, 1, null, BonusCalculation.DownloadTime)).IgnoreArguments().Return(new PetaPoco.Page<WorkUnitRow>());
            // Act
            _model.ResetBindings(true);
            // Assert
            _repository.VerifyAllExpectations();
        }
    }
}
