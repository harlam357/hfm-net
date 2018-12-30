
using NUnit.Framework;

using HFM.Proteins;

namespace HFM.Core.Internal
{
   [TestFixture]
   public class ProteinExtensionsTests
   {
      [Test]
      public void ProteinExtensions_IsUnknown_ReturnsFalseWhenProjectNumberIsNotZero_Test()
      {
         var protein = new Protein { ProjectNumber = 1 };
         Assert.IsFalse(protein.IsUnknown());
      }

      [Test]
      public void ProteinExtensions_IsUnknown_ReturnsTrueWhenProjectNumberIsZero_Test()
      {
         var protein = new Protein();
         Assert.IsTrue(protein.IsUnknown());
      }
   }
}
