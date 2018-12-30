
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Proteins
{
   [TestFixture]
   public class ProjectSummaryJsonDeserializerTests
   {
      [Test]
      public void ProjectSummaryJsonDeserializer_Deserialize_Test()
      {
         var serializer = new ProjectSummaryJsonDeserializer();
         using (var stream = File.OpenRead("..\\..\\TestFiles\\summary.json"))
         {
            var proteins = serializer.Deserialize(stream);
            Assert.AreEqual(627, proteins.Count);
         }
      }

      [Test]
      public async Task ProjectSummaryJsonDeserializer_DeserializeAsync_Test()
      {
         var serializer = new ProjectSummaryJsonDeserializer();
         using (var stream = File.OpenRead("..\\..\\TestFiles\\summary.json"))
         {
            var proteins = await serializer.DeserializeAsync(stream);
            Assert.AreEqual(627, proteins.Count);
         }
      }

      [Test]
      public void ProjectSummaryJsonDeserializer_Deserialize_FromEmptyStream_Test()
      {
         var serializer = new ProjectSummaryJsonDeserializer();
         using (var stream = new MemoryStream())
         {
            var proteins = serializer.Deserialize(stream);
            Assert.AreEqual(0, proteins.Count);
         }
      }
   }
}
