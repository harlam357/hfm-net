
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class FahLogTests
   {
      [Test]
      public void FahClientLog_Read_ArgumentNullException_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () =>
         {
            FahLog log = new FahClientLog();
            log.Read(null);
         });
      }

      [Test]
      public void LegacyLog_Read_ArgumentNullException_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () =>
         {
            FahLog log = new LegacyLog();
            log.Read(null);
         });
      }
   }
}
