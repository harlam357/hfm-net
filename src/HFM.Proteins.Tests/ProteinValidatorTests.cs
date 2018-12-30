
using NUnit.Framework;

namespace HFM.Proteins
{
   [TestFixture]
   public class ProteinValidatorTests
   {
      [Test]
      public void Protein_IsValid_ReturnsTrueWhenAllRequiredPropertiesArePopulated_Test()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3, MaximumDays = 5, Credit = 500, Frames = 100, KFactor = 26.4 };
         Assert.IsTrue(ProteinValidator.IsValid(protein));
      }

      [Test]
      public void Protein_IsValid_ReturnsFalseWhenAllRequiredPropertiesAreNotPopulated_Test()
      {
         var protein = new Protein();
         Assert.IsFalse(ProteinValidator.IsValid(protein));
      }

      [Test]
      public void Protein_IsValid_ReturnsFalseWhenProteinIsNull_Test()
      {
         Assert.IsFalse(ProteinValidator.IsValid(null));
      }
   }
}
