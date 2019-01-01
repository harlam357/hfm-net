
namespace HFM.Core
{
   public static partial class Validate
   {
      public static bool Minutes(int minutes)
      {
         if ((minutes > MaxMinutes) || (minutes < MinMinutes))
         {
            return false;
         }

         return true;
      }

      public const int MinMinutes = 1;
      public const int MaxMinutes = 180;
   }
}
