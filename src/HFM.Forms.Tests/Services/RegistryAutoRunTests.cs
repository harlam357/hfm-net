
using System;

using NUnit.Framework;

using HFM.Core.Logging;

namespace HFM.Forms
{
   [TestFixture]
   public class RegistryAutoRunTests
   {
      [Test]
      public void RegistryAutoRun_Test()
      {
         var autoRun = new RegistryAutoRun(NullLogger.Instance);
         autoRun.SetFilePath(System.Reflection.Assembly.GetExecutingAssembly().Location);
         Assert.AreEqual(true, autoRun.IsEnabled());
         autoRun.SetFilePath(String.Empty);
         Assert.AreEqual(false, autoRun.IsEnabled());
      }
   }
}
