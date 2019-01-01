
namespace HFM.Proteins
{
   /// <summary>
   /// Validates a <see cref="Protein"/> object's values are valid and can be used to calculate work unit production.
   /// </summary>
   public static class ProteinValidator
   {
      /// <summary>
      /// Returns true if this <see cref="Protein"/> has valid values for <see cref="Protein.ProjectNumber"/>, <see cref="Protein.PreferredDays"/>, <see cref="Protein.MaximumDays"/>, <see cref="Protein.Credit"/>, <see cref="Protein.Frames"/>, and <see cref="Protein.KFactor"/>; otherwise, false.
      /// </summary>
      public static bool IsValid(Protein protein)
      {
         if (protein == null) return false;

         return protein.ProjectNumber > 0 &&
                protein.PreferredDays > 0 &&
                protein.MaximumDays > 0 &&
                protein.Credit > 0 &&
                protein.Frames > 0 &&
                protein.KFactor >= 0;
      }
   }
}
