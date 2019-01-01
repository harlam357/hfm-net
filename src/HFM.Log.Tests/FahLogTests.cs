
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
         Assert.Throws<ArgumentNullException>(() =>
         {
            FahLog log = new FahClient.FahClientLog();
            log.Read(null);
         });
      }

      [Test]
      public void FahClientLog_ReadAsync_ThrowsWhenFahLogReaderIsNull_Test()
      {
         Assert.ThrowsAsync<ArgumentNullException>(async () =>
         {
            FahLog log = new FahClient.FahClientLog();
            await log.ReadAsync(null);
         });
      }

      [Test]
      public void LegacyLog_Read_ThrowsWhenFahLogReaderIsNull_Test()
      {
         Assert.Throws<ArgumentNullException>(() =>
         {
            FahLog log = new Legacy.LegacyLog();
            log.Read(null);
         });
      }

      [Test]
      public void LegacyLog_ReadAsync_ThrowsWhenFahLogReaderIsNull_Test()
      {
         Assert.ThrowsAsync<ArgumentNullException>(async () =>
         {
            FahLog log = new Legacy.LegacyLog();
            await log.ReadAsync(null);
         });
      }
   }
}
