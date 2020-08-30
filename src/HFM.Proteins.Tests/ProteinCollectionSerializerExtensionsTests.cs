
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinCollectionSerializerExtensionsTests
    {
        [Test]
        public void ProteinCollectionSerializerExtensions_ReadFile_Test()
        {
            var serializer = new ProjectSummaryJsonDeserializer();
            var proteins = serializer.ReadFile("..\\..\\TestFiles\\summary.json");
            Assert.AreEqual(627, proteins.Count);
        }

        [Test]
        public async Task ProteinCollectionSerializerExtensions_ReadFileAsync_Test()
        {
            var serializer = new ProjectSummaryJsonDeserializer();
            var proteins = await serializer.ReadFileAsync("..\\..\\TestFiles\\summary.json");
            Assert.AreEqual(627, proteins.Count);
        }

        [Test]
        public void ProteinCollectionSerializerExtensions_WriteFile_Test()
        {
            var collection = new[] { new Protein() };

            using (var artifacts = new ArtifactFolder())
            {
                var path = Path.Combine(artifacts.Path, "ProjectInfo.tab");
                var serializer = new TabDelimitedTextSerializer();
                serializer.WriteFile(path, collection);
                Assert.IsTrue(File.Exists(path));
            }
        }

        [Test]
        public async Task ProteinCollectionSerializerExtensions_WriteFileAsync_Test()
        {
            var collection = new[] { new Protein() };

            using (var artifacts = new ArtifactFolder())
            {
                var path = Path.Combine(artifacts.Path, "ProjectInfo.tab");
                var serializer = new TabDelimitedTextSerializer();
                await serializer.WriteFileAsync(path, collection);
                Assert.IsTrue(File.Exists(path));
            }
        }
    }
}
