
namespace HFM.Core.Configuration
{
   public static class TypeDescriptionProviderSetup
   {
      [CoverageExclude]
      public static void Execute()
      {
         Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(SlotModel));
         Hyper.ComponentModel.HyperTypeDescriptionProvider.Add(typeof(DataTypes.HistoryEntry));
      }
   }
}
