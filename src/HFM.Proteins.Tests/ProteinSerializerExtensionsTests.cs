
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
   [TestFixture]
   public class ProteinSerializerExtensionsTests
   {
      [Test]
      public void ProteinSerializerExtensions_ReadFile_Test()
      {
         var serializer = new ProjectSummaryJsonDeserializer();
         var proteins = serializer.ReadFile("..\\..\\TestFiles\\summary.json");
         Assert.AreEqual(627, proteins.Count);
      }

      [Test]
      public async Task ProteinSerializerExtensions_ReadFileAsync_Test()
      {
         var serializer = new ProjectSummaryJsonDeserializer();
         var proteins = await serializer.ReadFileAsync("..\\..\\TestFiles\\summary.json");
         Assert.AreEqual(627, proteins.Count);
      }

      [Test]
      public void ProteinSerializerExtensions_WriteFile_Test()
      {
         var collection = new[] { new Protein() };

         using (var artifacts = new ArtifactFolder())
         {
            var path = Path.Combine(artifacts.Path, "ProjectInfo.tab");
            var serializer = new TabSerializer();
            serializer.WriteFile(path, collection);
            Assert.IsTrue(File.Exists(path));
         }
      }

      [Test]
      public async Task ProteinSerializerExtensions_WriteFileAsync_Test()
      {
         var collection = new[] { new Protein() };

         using (var artifacts = new ArtifactFolder())
         {
            var path = Path.Combine(artifacts.Path, "ProjectInfo.tab");
            var serializer = new TabSerializer();
            await serializer.WriteFileAsync(path, collection);
            Assert.IsTrue(File.Exists(path));
         }
      }
   }
}
