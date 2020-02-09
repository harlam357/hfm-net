
namespace HFM.Core.ScheduledTasks
{
   public static class Interval
   {
      public static bool Validate(int interval)
      {
         if (interval > MaxInterval || interval < MinInterval)
         {
            return false;
         }

         return true;
      }

      public const int MinInterval = 1;
      public const int MaxInterval = 180;
   }
}
