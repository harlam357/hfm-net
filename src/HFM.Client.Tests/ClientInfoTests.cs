
using System;
using System.IO;

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class ClientInfoTests
   {
      [Test]
      public void ParseTest1()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
         var info = ClientInfo.Parse(Messages.GetNextJsonMessage(ref message));
      }
   }
}
