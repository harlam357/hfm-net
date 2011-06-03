
using System.IO;

using NUnit.Framework;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class SlotsTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt");
         var slots = Slots.Parse(Messages.GetNextMessage(ref message));
         Assert.AreEqual(1, slots.Count);
      }
   }
}
