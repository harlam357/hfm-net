
using System.Collections.ObjectModel;

namespace HFM.Plugins
{
   public interface IInstanceCollectionDataInterface
   {
      ReadOnlyCollection<string> InstanceNames { get; }

      IClientInstanceSettingsDataInterface GetNewDataInterface();

      IClientInstanceSettingsDataInterface GetExistingDataInterface(string instanceName);

      void ThrowAwayNewDataInterface(string reason);
   }
}
