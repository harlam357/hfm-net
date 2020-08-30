using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinCollectionTests
    {
        [Test]
        public void ProteinCollection_Ctor_ThrowsWhenProteinsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProteinCollection(null));
        }

        [Test]
        public void ProteinCollection_ContainsKey_ReturnsFalseWhenTheCollectionIsEmpty()
        {
            // Arrange
            var collection = new ProteinCollection();
            // Act
            bool result = collection.ContainsKey(1);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ProteinCollection_TryGetValue_ReturnsFalseWhenTheCollectionIsEmpty()
        {
            // Arrange
            var collection = new ProteinCollection();
            // Act
            bool result = collection.TryGetValue(1, out var protein);
            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(protein);
        }

        [Test]
        public void ProteinCollection_Ctor_AddsValidProteins()
        {
            // Arrange
            var proteins = new List<Protein>();
            proteins.Add(CreateValidProtein(1));
            proteins.Add(CreateValidProtein(2));
            proteins.Add(new Protein { ProjectNumber = 3 });
            // Act
            var collection = new ProteinCollection(proteins);
            // Assert
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, collection[1].ProjectNumber);
            Assert.AreEqual(2, collection[2].ProjectNumber);
        }

        [Test]
        public void ProteinCollection_Update_AddsValidProteins()
        {
            // Arrange
            var proteins = new List<Protein>();
            proteins.Add(CreateValidProtein(1));
            proteins.Add(CreateValidProtein(2));
            proteins.Add(new Protein { ProjectNumber = 3 });
            // Act
            var collection = new ProteinCollection();
            var changes = collection.Update(proteins);
            // Assert
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(1, changes[0].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[0].Action);
            Assert.IsNull(changes[0].PropertyChanges);
            Assert.AreEqual(2, changes[1].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[1].Action);
            Assert.IsNull(changes[1].PropertyChanges);
        }

        [Test]
        public void ProteinCollection_Update_ThrowsWhenProteinsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ProteinCollection().Update(null));
        }

        [Test]
        public void ProteinCollection_Update_VerifyChanges()
        {
            // Arrange
            var collection = new ProteinCollection();
            // add proteins so we have something that already exists
            collection.Add(CreateValidProtein(1));
            collection.Add(CreateValidProtein(2));
            collection.Add(CreateValidProtein(3));
            // build the collection of proteins to load
            var proteins = new List<Protein>();
            var protein = CreateValidProtein(1);
            protein.Credit = 100;
            proteins.Add(protein);
            protein = CreateValidProtein(2);
            protein.MaximumDays = 3;
            protein.KFactor = 26.4;
            proteins.Add(protein);
            proteins.Add(CreateValidProtein(3));
            proteins.Add(CreateValidProtein(4));
            // Act
            var changes = collection.Update(proteins);
            // Assert
            Assert.AreEqual(4, changes.Count);
            // check index 0    
            Assert.AreEqual(1, changes[0].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Property, changes[0].Action);
            var propertyChanges = changes[0].PropertyChanges;
            Assert.AreEqual(1, propertyChanges.Count);
            Assert.AreEqual("Credit", propertyChanges[0].PropertyName);
            Assert.AreEqual("1", propertyChanges[0].Previous);
            Assert.AreEqual("100", propertyChanges[0].Current);
            // check index 1
            Assert.AreEqual(2, changes[1].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Property, changes[1].Action);
            propertyChanges = changes[1].PropertyChanges;
            Assert.AreEqual(2, propertyChanges.Count);
            Assert.AreEqual("MaximumDays", propertyChanges[0].PropertyName);
            Assert.AreEqual("1", propertyChanges[0].Previous);
            Assert.AreEqual("3", propertyChanges[0].Current);
            Assert.AreEqual("KFactor", propertyChanges[1].PropertyName);
            Assert.AreEqual("0", propertyChanges[1].Previous);
            Assert.AreEqual("26.4", propertyChanges[1].Current);
            // check index 2
            Assert.AreEqual(3, changes[2].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.None, changes[2].Action);
            Assert.IsNull(changes[2].PropertyChanges);
            // check index 3
            Assert.AreEqual(4, changes[3].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.Add, changes[3].Action);
            Assert.IsNull(changes[3].PropertyChanges);
        }

        [Test]
        public void ProteinCollection_Update_VerifyCollectionContents_Test()
        {
            // Arrange
            var collection = new ProteinCollection();
            // add proteins so we have something that already exists
            collection.Add(CreateValidProtein(1));
            collection.Add(CreateValidProtein(2));
            collection.Add(CreateValidProtein(3));
            // build the collection of proteins to load
            var proteins = new List<Protein>();
            var protein = CreateValidProtein(2);
            protein.MaximumDays = 3;
            protein.KFactor = 26.4;
            proteins.Add(protein);
            proteins.Add(CreateValidProtein(3));
            // Act
            var changes = collection.Update(proteins);
            // Assert
            Assert.AreEqual(3, collection.Count);
            // check project 1
            Assert.AreEqual(1, collection[1].ProjectNumber);
            // check project 2
            Assert.AreEqual(2, collection[2].ProjectNumber);
            Assert.AreEqual(3, collection[2].MaximumDays);
            Assert.AreEqual(26.4, collection[2].KFactor);
            Assert.AreEqual(ProteinChangeAction.Property, changes[0].Action);
            // check project 3
            Assert.AreEqual(3, collection[3].ProjectNumber);
            Assert.AreEqual(ProteinChangeAction.None, changes[1].Action);
        }

        private static Protein CreateValidProtein(int projectNumber)
        {
            return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
        }

        [Test]
        [Ignore("User example test using external resources")]
        public void ProteinCollection_Ctor_FromProjectSummary()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(client.DownloadData(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = deserializer.Deserialize(stream);
                var collection = new ProteinCollection(proteins);
                Assert.IsTrue(collection.Count > 0);
            }
        }

        [Test]
        [Ignore("User example test using external resources")]
        public async Task ProteinCollection_Create_FromProjectSummaryAsync()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(await client.DownloadDataTaskAsync(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = deserializer.Deserialize(stream);
                var collection = new ProteinCollection(proteins);
                Assert.IsTrue(collection.Count > 0);
            }
        }
    }
}
