
using System.Threading.Tasks;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.Configuration
{
   internal static class ClientConfigurationExtensions
   {
      internal static void SubscribeToEvents(this IClientConfiguration configuration, IProteinBenchmarkService benchmarkService)
      {
         configuration.ClientEdited += (s, e) => UpdateBenchmarkData(benchmarkService, e);

         configuration.ConfigurationChanged += (s, e) =>
         {
            switch (e.Action)
            {
               case ConfigurationChangedAction.Add:
               case ConfigurationChangedAction.Edit:
                  if (e.Client != null)
                  {
                     Task.Run(() => e.Client.Retrieve());
                  }
                  break;
            }
         };
      }

      private static void UpdateBenchmarkData(IProteinBenchmarkService benchmarkService, ClientEditedEventArgs e)
      {
         // the name changed
         if (e.OldName != e.NewName)
         {
            // update the Names in the benchmark collection
            benchmarkService.UpdateOwnerName(e.OldName, e.OldPath, e.NewName);
         }
         // the path changed
         if (!Internal.FileSystem.PathsEqual(e.OldPath, e.NewPath))
         {
            // update the Paths in the benchmark collection
            benchmarkService.UpdateOwnerPath(e.NewName, e.OldPath, e.NewPath);
         }
      }
   }
}
