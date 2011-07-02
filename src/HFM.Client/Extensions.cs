
using System.Text;

namespace HFM.Client
{
   public static class Extensions
   {
      // Note: implemented in .NET 4.0 - remove when upgrading
      public static void Clear(this StringBuilder sb)
      {
         sb.Length = 0;
      }
   }
}
