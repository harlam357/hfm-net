
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
   internal static class ProteinExtensions
   {
      internal static bool IsUnknown(this Protein protein)
      {
         return protein.ProjectNumber == 0;
      }
   }
}
