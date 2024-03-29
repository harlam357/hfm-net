﻿using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Preferences;

using Moq;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class WorkUnitHistoryModelTests
    {
        private Mock<IWorkUnitRepository> _mockRepository;
        private WorkUnitHistoryModel _model;

        [SetUp]
        public void Init()
        {
            _mockRepository = new Mock<IWorkUnitRepository>();
            _model = new WorkUnitHistoryModel(new InMemoryPreferencesProvider(), new WorkUnitQueryDataContainer(), _mockRepository.Object);
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
        public async Task WorkUnitHistoryModel_ResetBindings_Test()
        {
            // Arrange
            Assert.AreEqual(1, _model.QueryBindingSource.Count);

            _model.AddQuery(new WorkUnitQuery("Test")
                .AddParameter(new WorkUnitQueryParameter { Value = 6606 }));
            Assert.AreEqual(2, _model.QueryBindingSource.Count);

            _mockRepository
                .Setup(x => x.PageAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<WorkUnitQuery>(),
                    It.IsAny<BonusCalculation>()))
                .Returns(Task.FromResult(new Page<WorkUnitRow>()));
            // Act
            await _model.ResetBindings(true);
            // Assert
            _mockRepository.Verify();
        }
    }
}
