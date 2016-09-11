
namespace HFM.Core.DataTypes
{
   public static partial class Extensions
   {
      public static bool IsTerminating(this WorkUnitResult result)
      {
         switch (result)
         {
            case WorkUnitResult.FinishedUnit:
            case WorkUnitResult.EarlyUnitEnd:
            case WorkUnitResult.UnstableMachine:
            case WorkUnitResult.BadWorkUnit:
            case WorkUnitResult.ClientCoreError:
               return true;
            default:
               return false;
         }
      }
   }
}