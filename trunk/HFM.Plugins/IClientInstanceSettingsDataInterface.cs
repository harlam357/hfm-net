
namespace HFM.Plugins
{
   public enum ClientInstanceSettingsKeys
   {
      InstanceType,
      InstanceName,
      ExternalInstance,
      ExternalFileName,
      ClientMhz,
      FahLogFileName,
      UnitInfoFileName,
      QueueFileName,
      Path,
      Server,
      Username,
      Password,
      FtpMode,
      UtcOffsetIsZero,
      ClientTimeOffset
   }
   
   public interface IClientInstanceSettingsDataInterface
   {
      object GetSetting(ClientInstanceSettingsKeys key);

      void SetSetting(ClientInstanceSettingsKeys key, object value);
   }
}
