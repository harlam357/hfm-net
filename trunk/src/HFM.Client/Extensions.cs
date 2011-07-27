
using System;
using System.Text;

namespace HFM.Client
{
   public static class Extensions
   {
      // Note: implemented in .NET 4.0 - remove when upgrading
      public static void Clear(this StringBuilder sb)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         sb.Length = 0;
      }

      // Note: implemented in .NET 4.0 - remove when upgrading
      public static bool IsNullOrWhiteSpace(this string value)
      {
         return value == null || value.Trim().Length == 0;
      }
   }
}
