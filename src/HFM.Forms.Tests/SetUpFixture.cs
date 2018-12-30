
using System;

using NUnit.Framework;

namespace HFM.Forms
{
   [SetUpFixture]
   public class SetUpFixture
   {
      [OneTimeSetUp]
      public void SetEnvironmentCurrentDirectory()
      {
         Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
      }
   }
}
