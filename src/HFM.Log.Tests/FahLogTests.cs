
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class FahLogTests
   {
      [Test]
      public void FahClientLog_Read_ThrowsWhenFahLogReaderIsNull_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () =>
         {
            FahLog log = new FahClient.FahClientLog();
            log.Read(null);
         });
      }

      [Test]
      public void LegacyLog_Read_ThrowsWhenFahLogReaderIsNull_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () =>
         {
            FahLog log = new Legacy.LegacyLog();
            log.Read(null);
         });
      }
   }
}
