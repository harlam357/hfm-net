/*
 * HFM.NET - Legacy Client Settings Serializer
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;

using Castle.Core.Logging;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Core.Serializers
{
   public class HfmLegacyFileSerializer : IFileSerializer<List<ClientSettings>>
   {
      #region Constants

      // ReSharper disable InconsistentNaming

      //private const string xmlNodeInstance = "Instance";
      private const string xmlAttrName = "Name";
      private const string xmlNodeExternal = "External";
      //private const string xmlNodeExternalFile = "ExternalFile";
      private const string xmlNodeFAHLog = "FAHLogFile";
      private const string xmlNodeUnitInfo = "UnitInfoFile";
      private const string xmlNodeQueue = "QueueFile";
      private const string xmlNodeClientMHz = "ClientMHz";
      private const string xmlNodeClientVM = "ClientVM";
      private const string xmlNodeClientOffset = "ClientOffset";
      private const string xmlPropType = "HostType";
      private const string xmlPropPath = "Path";
      private const string xmlPropServ = "Server";
      private const string xmlPropUser = "Username";
      private const string xmlPropPass = "Password";
      private const string xmlPropPassiveFtpMode = "PassiveFtpMode";

      // ReSharper restore InconsistentNaming

      #endregion

      #region Fields

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      // Encryption Key and Initialization Vector
      private readonly Data _iv = new Data("zX!1=D,^7K@u33+d");
      private readonly Data _symmetricKey = new Data("cNx/7+,?%ubm*?j8");

      #endregion
      
      #region IFileSerializer<List<ClientSettings>> Members

      public string FileExtension
      {
         get { return "hfm"; }
      }

      public string FileTypeFilter
      {
         get { return "HFM Legacy Configuration Files|*.hfm"; }
      }

      public List<ClientSettings> Deserialize(string fileName)
      {
         var xmlData = new XmlDocument();
         xmlData.Load(fileName);

         // xmlData now contains the collection of Nodes. Hopefully.
         xmlData.RemoveChild(xmlData.ChildNodes[0]);

         var list = new List<ClientSettings>();
         foreach (XmlNode xn in xmlData.ChildNodes[0])
         {
            ClientSettings settings = Deserialize(xn);
            if (settings != null)
            {
               list.Add(settings);
            }
         }

         return list;
      }

      public void Serialize(string fileName, List<ClientSettings> value)
      {
         throw new NotSupportedException("Legacy serialization is not supported.");
      }

      #endregion

      #region XML Serialization

      private ClientSettings Deserialize(XmlNode xmlData)
      {
         var settings = new ClientSettings(ClientType.Legacy);

         // ReSharper disable PossibleNullReferenceException

         settings.LegacyClientSubType = GetLegacyClientSubType(xmlData.SelectSingleNode(xmlPropType).InnerText);
         settings.Name = xmlData.Attributes[xmlAttrName].ChildNodes[0].Value;
         settings.Path = xmlData.SelectSingleNode(xmlPropPath).InnerText;

         try
         {
            bool external = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeExternal).InnerText);
            if (external)
            {
               _logger.Warn("Cannot migrate legacy external client settings. Skipping these client settings and continuing.");
               return null;
            }
         }
         catch (NullReferenceException)
         {
            // do nothing, we don't care about legacy external settings   
         }
         catch (FormatException)
         {
            _logger.Warn("Cannot load external instance flag. Skipping these client settings and continuing.");
            return null;
         }

         try
         {
            settings.ClientProcessorMegahertz = Int32.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText);
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load client MHz, defaulting to 1 MHz.");
            settings.ClientProcessorMegahertz = 1;
         }
         catch (FormatException)
         {
            _logger.Warn("Cannot load client MHz, defaulting to 1 MHz.");
            settings.ClientProcessorMegahertz = 1;
         }

         try
         {
            settings.FahLogFileName = xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText;
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load FAHlog.txt file name.");
            settings.FahLogFileName = Constants.FahLogFileName;
         }

         try
         {
            settings.UnitInfoFileName = xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText;
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load unitinfo.txt file name.");
            settings.UnitInfoFileName = Constants.UnitInfoFileName;
         }

         try
         {
            settings.QueueFileName = xmlData.SelectSingleNode(xmlNodeQueue).InnerText;
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load queue.dat file name.");
            settings.QueueFileName = Constants.QueueFileName;
         }

         try
         {
            settings.Server = xmlData.SelectSingleNode(xmlPropServ).InnerText;
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load client server.");
            settings.Server = String.Empty;
         }

         try
         {
            settings.Username = xmlData.SelectSingleNode(xmlPropUser).InnerText;
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load server username.");
            settings.Username = String.Empty;
         }

         var symetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         try
         {
            settings.Password = String.Empty;
            if (xmlData.SelectSingleNode(xmlPropPass).InnerText.Length > 0)
            {
               try
               {
                  symetricProvider.IntializationVector = _iv;
                  settings.Password = symetricProvider.Decrypt(new Data(Utils.FromBase64(
                     xmlData.SelectSingleNode(xmlPropPass).InnerText)), _symmetricKey).ToString();
               }
               catch (FormatException)
               {
                  _logger.Warn("Cannot decrypt password... loading clear value.");
                  settings.Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
               catch (CryptographicException)
               {
                  _logger.Warn("Cannot decrypt password... loading clear value.");
                  settings.Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
            }
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load password.");
         }

         try
         {
            settings.FtpMode = GetFtpType(xmlData.SelectSingleNode(xmlPropPassiveFtpMode).InnerText);
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load ftp mode, defaulting to passive.");
            settings.FtpMode = FtpType.Passive;
         }
         catch (FormatException)
         {
            _logger.Warn("Cannot load ftp mode, defaulting to passive.");
            settings.FtpMode = FtpType.Passive;
         }

         try
         {
            settings.UtcOffsetIsZero = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText);
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load client VM flag, defaulting to false.");
            settings.UtcOffsetIsZero = false;
         }
         catch (FormatException)
         {
            _logger.Warn("Cannot load client VM flag, defaulting to false.");
            settings.UtcOffsetIsZero = false;
         }

         try
         {
            settings.ClientTimeOffset = Int32.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText);
         }
         catch (NullReferenceException)
         {
            _logger.Warn("Cannot load client time offset, defaulting to 0.");
            settings.ClientTimeOffset = 0;
         }
         catch (FormatException)
         {
            _logger.Warn("Cannot load client time offset, defaulting to 0.");
            settings.ClientTimeOffset = 0;
         }

         // ReSharper restore PossibleNullReferenceException

         return settings;
      }

      private static LegacyClientSubType GetLegacyClientSubType(string value)
      {
         switch (value.ToUpperInvariant())
         {
            case "PATHINSTANCE":
               return LegacyClientSubType.Path;
            case "HTTPINSTANCE":
               return LegacyClientSubType.Http;
            case "FTPINSTANCE":
               return LegacyClientSubType.Ftp;
            default:
               throw new FormatException("Given InstanceType value must be either 'PathInstance', 'HttpInstance', or 'FtpInstance'.");
         }
      }

      private static FtpType GetFtpType(string value)
      {
         switch (value.ToUpperInvariant())
         {
            case "PASSIVE":
               return FtpType.Passive;
            case "ACTIVE":
               return FtpType.Active;
            default:
               throw new FormatException("Given FtpType value must be either 'Passive' or 'Active'.");
         }
      }

      #endregion
   }
}
