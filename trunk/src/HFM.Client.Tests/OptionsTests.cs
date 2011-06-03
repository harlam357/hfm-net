
using System;
using System.IO;

using NUnit.Framework;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class OptionsTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt");
         var options = Options.Parse(Messages.GetNextMessage(ref message));
      }
   }
}
