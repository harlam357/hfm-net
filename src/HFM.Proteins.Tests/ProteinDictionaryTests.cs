using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinDictionaryTests
    {
        [Test]
        public void ProteinDictionary_Create_ThrowsWhenProteinsIsNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => ProteinDictionary.Create(null));
        }

        [Test]
        public void ProteinDictionary_Create_Test()
        {
            // Arrange
            // build the collection of proteins to load
            var values = new List<Protein>();
            values.Add(CreateValidProtein(1));
            values.Add(CreateValidProtein(2));
            values.Add(new Protein { ProjectNumber = 3 });
            // Act
            var dictionary = ProteinDictionary.Create(values);
            var results = dictionary.Changes.ToList();
            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results[0].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.Added, results[0].Result);
            Assert.IsNull(results[0].Changes);
            Assert.AreEqual(2, results[1].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.Added, results[1].Result);
            Assert.IsNull(results[1].Changes);
        }

        [Test]
        public void ProteinDictionary_CreateFromExisting_ThrowsWhenExistingDictionaryIsNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => ProteinDictionary.CreateFromExisting(null, new Protein[0]));
        }

        [Test]
        public void ProteinDictionary_CreateFromExisting_ThrowsWhenProteinsIsNull_Test()
        {
            Assert.Throws<ArgumentNullException>(() => ProteinDictionary.CreateFromExisting(new ProteinDictionary(), null));
        }

        [Test]
        public void ProteinDictionary_CreateFromExisting_VerifyChanges_Test()
        {
            // Arrange
            var dictionary = new ProteinDictionary();
            // add proteins so we have something that already exists
            dictionary.Add(1, CreateValidProtein(1));
            dictionary.Add(2, CreateValidProtein(2));
            dictionary.Add(3, CreateValidProtein(3));
            // build the collection of proteins to load
            var values = new List<Protein>();
            var protein = CreateValidProtein(1);
            protein.Credit = 100;
            values.Add(protein);
            protein = CreateValidProtein(2);
            protein.MaximumDays = 3;
            protein.KFactor = 26.4;
            values.Add(protein);
            values.Add(CreateValidProtein(3));
            values.Add(CreateValidProtein(4));
            // Act
            dictionary = ProteinDictionary.CreateFromExisting(dictionary, values);
            var results = dictionary.Changes.ToList();
            // Assert
            Assert.AreEqual(4, results.Count);
            // check index 0
            Assert.AreEqual(1, results[0].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.Changed, results[0].Result);
            var changes = results[0].Changes.ToList();
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("Credit", changes[0].PropertyName);
            Assert.AreEqual("1", changes[0].Previous);
            Assert.AreEqual("100", changes[0].Current);
            // check index 1
            Assert.AreEqual(2, results[1].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.Changed, results[1].Result);
            changes = results[1].Changes.ToList();
            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual("MaximumDays", changes[0].PropertyName);
            Assert.AreEqual("1", changes[0].Previous);
            Assert.AreEqual("3", changes[0].Current);
            Assert.AreEqual("KFactor", changes[1].PropertyName);
            Assert.AreEqual("0", changes[1].Previous);
            Assert.AreEqual("26.4", changes[1].Current);
            // check index 2
            Assert.AreEqual(3, results[2].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.NoChange, results[2].Result);
            Assert.IsNull(results[2].Changes);
            // check index 3
            Assert.AreEqual(4, results[3].ProjectNumber);
            Assert.AreEqual(ProteinDictionaryChangeResult.Added, results[3].Result);
            Assert.IsNull(results[3].Changes);
        }

        [Test]
        public void ProteinDictionary_CreateFromExisting_VerifyDictionaryContents_Test()
        {
            // Arrange
            var dictionary = new ProteinDictionary();
            // add proteins so we have something that already exists
            dictionary.Add(1, CreateValidProtein(1));
            dictionary.Add(2, CreateValidProtein(2));
            dictionary.Add(3, CreateValidProtein(3));
            // build the collection of proteins to load
            var values = new List<Protein>();
            var protein = CreateValidProtein(2);
            protein.MaximumDays = 3;
            protein.KFactor = 26.4;
            values.Add(protein);
            values.Add(CreateValidProtein(3));
            // Act
            dictionary = ProteinDictionary.CreateFromExisting(dictionary, values);
            // Assert
            Assert.AreEqual(3, dictionary.Count);
            // check project 1
            Assert.AreEqual(1, dictionary[1].ProjectNumber);
            // check project 2
            Assert.AreEqual(2, dictionary[2].ProjectNumber);
            Assert.AreEqual(3, dictionary[2].MaximumDays);
            Assert.AreEqual(26.4, dictionary[2].KFactor);
            // check project 3
            Assert.AreEqual(3, dictionary[3].ProjectNumber);
        }

        private static Protein CreateValidProtein(int projectNumber)
        {
            return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
        }

        // TODO: use HttpClient
        [Test]
        [Ignore("User example test using external resources")]
        public void ProteinDictionary_Create_FromProjectSummary()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(client.DownloadData(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = deserializer.Deserialize(stream);
                var dictionary = ProteinDictionary.Create(proteins);
                Assert.IsTrue(dictionary.Count > 0);
            }
        }

        [Test]
        [Ignore("User example test using external resources")]
        public async Task ProteinDictionary_Create_FromProjectSummaryAsync()
        {
            // Arrange
            var client = new WebClient();
            using (var stream = new MemoryStream(await client.DownloadDataTaskAsync(ProjectSummaryUrl.Json)))
            {
                stream.Position = 0;
                var deserializer = new ProjectSummaryJsonDeserializer();
                var proteins = deserializer.Deserialize(stream);
                var dictionary = ProteinDictionary.Create(proteins);
                Assert.IsTrue(dictionary.Count > 0);
            }
        }
    }
}
