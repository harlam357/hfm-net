
using System.Threading.Tasks;

namespace HFM.Core
{
   internal static class ClientConfigurationExtensions
   {
      internal static void SubscribeToEvents(this IClientConfiguration configuration, IProteinBenchmarkCollection benchmarkCollection)
      {
         configuration.ClientEdited += (s, e) => UpdateBenchmarkData(benchmarkCollection, e);

         configuration.DictionaryChanged += (s, e) =>
         {
            switch (e.ChangedType)
            {
               case ConfigurationChangedType.Add:
               case ConfigurationChangedType.Edit:
                  if (e.Client != null)
                  {
                     Task.Factory.StartNew(e.Client.Retrieve);
                  }
                  break;
            }
         };
      }

      private static void UpdateBenchmarkData(IProteinBenchmarkCollection benchmarkCollection, ClientEditedEventArgs e)
      {
         // the name changed
         if (e.PreviousName != e.NewName)
         {
            // update the Names in the benchmark collection
            benchmarkCollection.UpdateOwnerName(e.PreviousName, e.PreviousPath, e.NewName);
         }
         // the path changed
         if (!Paths.Equal(e.PreviousPath, e.NewPath))
         {
            // update the Paths in the benchmark collection
            benchmarkCollection.UpdateOwnerPath(e.NewName, e.PreviousPath, e.NewPath);
         }
      }
   }
}
