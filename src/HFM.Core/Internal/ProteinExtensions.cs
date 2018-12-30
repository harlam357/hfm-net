
using HFM.Proteins;

namespace HFM.Core.Internal
{
   internal static class ProteinExtensions
   {
      internal static bool IsUnknown(this Protein protein)
      {
         return protein.ProjectNumber == 0;
      }
   }
}
