
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class FahLogTests
   {
      [Test]
      public void FahLog_Read_ArgumentNullException_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () => FahLog.Read(null, FahLogType.Legacy));
      }
   }
}
