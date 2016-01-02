
using System.Linq;

using NUnit.Framework;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class JsonSerializerTests
   {
      [Test]
      public void Deserialize_Test1()
      {
         var serializer = new JsonSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\summary.json").ToList();
         Assert.AreEqual(627, proteins.Count);
      }
   }
}
