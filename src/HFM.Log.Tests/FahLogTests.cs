
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class FahLogTests
   {
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void FahLog_Read_ArgumentNullException_Test()
      {
         FahLog.Read(null, FahLogType.Legacy);
      }
   }
}
