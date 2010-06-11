
namespace HFM.Plugins
{
   public enum ClientInstanceSettingsKeys
   {
      InstanceType,
      InstanceName,
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
