
using System.IO;

using NUnit.Framework;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class JsonSerializerTests
   {
      [Test]
      public void JsonSerializer_DeserializeFromJsonFile_Test()
      {
         var serializer = new JsonSerializer();
         using (var stream = File.Open("..\\..\\TestFiles\\summary.json", FileMode.Open, FileAccess.Read))
         {
            var proteins = serializer.Deserialize(stream);
            Assert.AreEqual(627, proteins.Count);
         }
      }

      [Test]
      public void JsonSerializer_DeserializeFromEmptyStream_Test()
      {
         var serializer = new JsonSerializer();
         using (var stream = new MemoryStream())
         {
            var proteins = serializer.Deserialize(stream);
            Assert.AreEqual(0, proteins.Count);
         }
      }
   }
}
