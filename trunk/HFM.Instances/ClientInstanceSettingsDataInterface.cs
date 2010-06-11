
using System;
using System.Globalization;

using HFM.Framework;
using HFM.Plugins;

namespace HFM.Instances
{
   public class ClientInstanceSettingsDataInterface : IClientInstanceSettingsDataInterface
   {
      private readonly IClientInstanceSettings _settings;

      public ClientInstanceSettingsDataInterface()
         : this (new ClientInstanceSettings())
      {
         
      }
   
      public ClientInstanceSettingsDataInterface(IClientInstanceSettings settings)
      {
         _settings = settings;
      }
   
      #region IClientInstanceSettingsDataInterface Members

      public object GetSetting(ClientInstanceSettingsKeys key)
      {
         switch (key)
         {
            case ClientInstanceSettingsKeys.InstanceType:
               return GetInstanceType(_settings.InstanceHostType);
            case ClientInstanceSettingsKeys.InstanceName:
               return _settings.InstanceName;
            case ClientInstanceSettingsKeys.ClientMhz:
               return _settings.ClientProcessorMegahertz;
            case ClientInstanceSettingsKeys.FahLogFileName:
               return _settings.RemoteFAHLogFilename;
            case ClientInstanceSettingsKeys.UnitInfoFileName:
               return _settings.RemoteUnitInfoFilename;
            case ClientInstanceSettingsKeys.QueueFileName:
               return _settings.RemoteQueueFilename;
            case ClientInstanceSettingsKeys.Path:
               return _settings.Path;
            case ClientInstanceSettingsKeys.Server:
               return _settings.Server;
            case ClientInstanceSettingsKeys.Username:
               return _settings.Username;
            case ClientInstanceSettingsKeys.Password:
               return _settings.Password;
            case ClientInstanceSettingsKeys.FtpMode:
               return GetFtpType(_settings.FtpMode);
            case ClientInstanceSettingsKeys.UtcOffsetIsZero:
               return _settings.ClientIsOnVirtualMachine;
            case ClientInstanceSettingsKeys.ClientTimeOffset:
               return _settings.ClientTimeOffset;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                 "Client Instance Setting {0} is not valid.", key));
         }
      }

      public void SetSetting(ClientInstanceSettingsKeys key, object value)
      {
         switch (key)
         {
            case ClientInstanceSettingsKeys.InstanceType:
               _settings.InstanceHostType = VerifyInstanceType(value);
               break;
            case ClientInstanceSettingsKeys.InstanceName:
               _settings.InstanceName = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.ClientMhz:
               _settings.ClientProcessorMegahertz = VerifyInt32(value);
               break;
            case ClientInstanceSettingsKeys.FahLogFileName:
               _settings.RemoteFAHLogFilename = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.UnitInfoFileName:
               _settings.RemoteUnitInfoFilename = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.QueueFileName:
               _settings.RemoteQueueFilename = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.Path:
               _settings.Path = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.Server:
               _settings.Server = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.Username:
               _settings.Username = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.Password:
               _settings.Password = VerifyString(value);
               break;
            case ClientInstanceSettingsKeys.FtpMode:
               _settings.FtpMode = VerifyFtpType(value);
               break;
            case ClientInstanceSettingsKeys.UtcOffsetIsZero:
               _settings.ClientIsOnVirtualMachine = VerifyBool(value);
               break;
            case ClientInstanceSettingsKeys.ClientTimeOffset:
               _settings.ClientTimeOffset = VerifyInt32(value);
               break;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                 "Client Instance Setting {0} is not valid.", key));
         }
      }

      #endregion
      
      private static string VerifyString(object value)
      {
         if (value is string)
         {
            return value.ToString();
         }
         
         throw new FormatException("Given value is not of type String.");
      }

      private static int VerifyInt32(object value)
      {
         if (value is Int32)
         {
            return (int)value;
         }

         throw new FormatException("Given value is not of type Int32.");
      }
      
      private static bool VerifyBool(object value)
      {
         if (value is Boolean)
         {
            return (bool)value;
         }

         throw new FormatException("Given value is not of type Boolean.");
      }
      
      private static string GetInstanceType(InstanceType type)
      {
         switch (type)
         {
            case InstanceType.PathInstance:
               return "PathInstance";
            case InstanceType.HttpInstance:
               return "HttpInstance";
            case InstanceType.FtpInstance:
               return "FtpInstance";
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                 "Instance Type {0} is not valid.", type)); 
         }
      }

      private static InstanceType VerifyInstanceType(object value)
      {
         var stringValue = VerifyString(value);

         switch (stringValue.ToUpperInvariant())
         {
            case "PATHINSTANCE":
               return InstanceType.PathInstance;
            case "HTTPINSTANCE":
               return InstanceType.HttpInstance;
            case "FTPINSTANCE":
               return InstanceType.FtpInstance;
            default:
               throw new FormatException("Given InstanceType value must be either 'PathInstance', 'HttpInstance', or 'FtpInstance'.");
         }
      }

      private static string GetFtpType(FtpType type)
      {
         switch (type)
         {
            case FtpType.Passive:
               return "Passive";
            case FtpType.Active:
               return "Active";
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                                                                 "Ftp Type {0} is not valid.", type));
         }
      }
      
      private static FtpType VerifyFtpType(object value)
      {
         var stringValue = VerifyString(value);

         switch (stringValue.ToUpperInvariant())
         {
            case "PASSIVE":
               return FtpType.Passive;
            case "ACTIVE":
               return FtpType.Active;
            default:
               throw new FormatException("Given FtpType value must be either 'Passive' or 'Active'.");
         }
      }
   }
}
