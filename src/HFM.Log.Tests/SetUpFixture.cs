
using System;

using NUnit.Framework;

namespace HFM.Log
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
