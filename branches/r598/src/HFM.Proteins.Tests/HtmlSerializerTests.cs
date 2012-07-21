
using System.Linq;

using NUnit.Framework;

namespace HFM.Proteins.Tests
{
   [TestFixture]   
   public class HtmlSerializerTests
   {
      [Test]
      public void DeserializeTest1()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummary.html").ToList();
         Assert.AreEqual(696, proteins.Count);
      }

      [Test]
      public void DeserializeTest2()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryB.html").ToList();
         Assert.AreEqual(196, proteins.Count);
      }

      [Test]
      public void DeserializeTest3()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryC.html").ToList();
         Assert.AreEqual(712, proteins.Count);
      }
   }
}
