
namespace HFM.Core
{
#if HFMCORE
   public static partial class Validate
#else
   internal static partial class Validate
#endif
   {
#if HFMCORE
      public static bool Minutes(int minutes)
#else
      internal static bool Minutes(int minutes)
#endif
      {
         if ((minutes > MaxMinutes) || (minutes < MinMinutes))
         {
            return false;
         }

         return true;
      }

#if HFMCORE
      public const int MinMinutes = 1;
      public const int MaxMinutes = 180;
#else
      internal const int MinMinutes = 1;
      internal const int MaxMinutes = 180;
#endif
   }
}
