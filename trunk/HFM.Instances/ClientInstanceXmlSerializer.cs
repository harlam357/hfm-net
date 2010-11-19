/*
 * HFM.NET - Client Instance Xml Serializer
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Framework;
using HFM.Plugins;

namespace HFM.Instances
{
   public class ClientInstanceXmlSerializer : IClientInstanceSettingsSerializer
   {
      #region Constants
      // Xml Serialization Constants
      // ReSharper disable InconsistentNaming
      private const string xmlNodeInstance = "Instance";
      private const string xmlAttrName = "Name";
      private const string xmlNodeExternal = "External";
      private const string xmlNodeExternalFile = "ExternalFile";
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

      #region Members
      // Encryption Key and Initialization Vector
      private readonly Data _iv = new Data("zX!1=D,^7K@u33+d");
      private readonly Data _symmetricKey = new Data("cNx/7+,?%ubm*?j8");

      private IInstanceCollectionDataInterface _dataInterface;
      #endregion
      
      #region XML Serialization
      /// <summary>
      /// Serialize this Client Instance to Xml
      /// </summary>
      /// <exception cref="ArgumentException">Throws if fileName is null, empty, or not a rooted path.</exception>
      /// <exception cref="XmlException">Throws if serialization fails.  Inspect InnerException for original Exception.</exception>
      public void Serialize(string fileName)
      {
         if (String.IsNullOrEmpty(fileName))
         {
            throw new ArgumentException("Argument 'fileName' cannot be a null or empty string.", "fileName");
         }

         // Save the XML stream to the file
         if (Path.IsPathRooted(fileName) == false)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "Argument 'fileName' must be a rooted path.  Given path '{0}'.", fileName), "fileName");
         }

         try
         {
            var xmlData = new XmlDocument();

            // Create the XML Declaration (well formed)
            XmlDeclaration xmlDeclaration = xmlData.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlData.InsertBefore(xmlDeclaration, xmlData.DocumentElement);

            // Create the root element
            XmlElement xmlRoot = xmlData.CreateElement("Hosts");

            ICollection<string> instanceNames = _dataInterface.InstanceNames;
            // Loop through the collection and serialize the lot
            foreach (var instanceName in instanceNames)
            {
               XmlDocument xmlDoc = Serialize(_dataInterface.GetExistingDataInterface(instanceName));
               foreach (XmlNode xn in xmlDoc.ChildNodes)
               {
                  xmlRoot.AppendChild(xmlData.ImportNode(xn, true));
               }
            }

            xmlData.AppendChild(xmlRoot);
            xmlData.Save(fileName);
         }
         catch (Exception ex)
         {
            throw new XmlException("Failed to serialize collection.", ex);
         }
      }

      /// <summary>
      /// Serialize this Client Instance to Xml
      /// </summary>
      private XmlDocument Serialize(IClientInstanceSettingsDataInterface dataInterface)
      {
         var xmlData = new XmlDocument();

         XmlElement xmlRoot = xmlData.CreateElement(xmlNodeInstance);
         string instanceName = dataInterface.GetSetting(ClientInstanceSettingsKeys.InstanceName).ToString();
         xmlRoot.SetAttribute(xmlAttrName, instanceName);
         xmlData.AppendChild(xmlRoot);

         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeExternal, 
            dataInterface.GetSetting(ClientInstanceSettingsKeys.ExternalInstance).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeExternalFile,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.ExternalFileName).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeFAHLog,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.FahLogFileName).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeUnitInfo, 
            dataInterface.GetSetting(ClientInstanceSettingsKeys.UnitInfoFileName).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeQueue,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.QueueFileName).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeClientMHz,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.ClientMhz).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeClientVM,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.UtcOffsetIsZero).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlNodeClientOffset,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.ClientTimeOffset).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropType, 
            dataInterface.GetSetting(ClientInstanceSettingsKeys.InstanceType).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropPath,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.Path).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropServ,
            dataInterface.GetSetting(ClientInstanceSettingsKeys.Server).ToString()));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropUser, 
            dataInterface.GetSetting(ClientInstanceSettingsKeys.Username).ToString()));

         var symetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         string encryptedPassword = String.Empty;
         string clearPassword = dataInterface.GetSetting(ClientInstanceSettingsKeys.Password).ToString();
         if (clearPassword.Length > 0)
         {
            try
            {
               symetricProvider.IntializationVector = _iv;
               encryptedPassword = symetricProvider.Encrypt(new Data(clearPassword), _symmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               OnWarningMessage(String.Concat(instanceName, " failed to encrypt Server Password... saving clear value."));
               encryptedPassword = clearPassword;
            }
         }
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropPass, encryptedPassword));
         xmlData.ChildNodes[0].AppendChild(XmlOps.CreateXmlNode(xmlData, xmlPropPassiveFtpMode, 
            dataInterface.GetSetting(ClientInstanceSettingsKeys.FtpMode).ToString()));

         return xmlData;
      }

      /// <summary>
      /// Loads a collection of Host Instances from disk
      /// </summary>
      /// <param name="fileName">XML File Name</param>
      /// <exception cref="ArgumentException">Throws if fileName is null or empty.</exception>
      /// <exception cref="XmlException">Throws if deserialization fails.  Inspect InnerException for original Exception.</exception>
      public void Deserialize(string fileName)
      {
         if (String.IsNullOrEmpty(fileName))
         {
            throw new ArgumentException("Argument 'fileName' cannot be a null or empty string.", "fileName");
         }

         try
         {
            var xmlData = new XmlDocument();
            xmlData.Load(fileName);

            // xmlData now contains the collection of Nodes. Hopefully.
            xmlData.RemoveChild(xmlData.ChildNodes[0]);

            foreach (XmlNode xn in xmlData.ChildNodes[0])
            {
               var settingsDataInterface = _dataInterface.GetNewDataInterface();
               Deserialize(xn, settingsDataInterface);
            }
         }
         catch (Exception ex)
         {
            throw new XmlException("Failed to deserialize collection.", ex);
         }
      }

      /// <summary>
      /// Deserialize Xml data into this Client Instance
      /// </summary>
      /// <param name="xmlData">XmlNode containing the client instance data</param>
      /// <param name="dataInterface">Client Instance Settings Interface</param>
      private void Deserialize(XmlNode xmlData, IClientInstanceSettingsDataInterface dataInterface)
      {
         dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceType, xmlData.SelectSingleNode(xmlPropType).InnerText);
         dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceName, xmlData.Attributes[xmlAttrName].ChildNodes[0].Value);
         dataInterface.SetSetting(ClientInstanceSettingsKeys.Path, xmlData.SelectSingleNode(xmlPropPath).InnerText);
         
         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ExternalInstance, Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeExternal).InnerText));
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load external instance flag, defaulting to false.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ExternalInstance, false);
         }
         catch (FormatException)
         {
            OnWarningMessage("Cannot load external instance flag, defaulting to false.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ExternalInstance, false);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ExternalFileName, xmlData.SelectSingleNode(xmlNodeExternalFile).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load remote external filename.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ExternalFileName, Constants.LocalExternal);
         }
         
         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientMhz, Int32.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText));
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load client MHz, defaulting to 1 MHz.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientMhz, 1);
         }
         catch (FormatException)
         {
            OnWarningMessage("Could not parse client MHz, defaulting to 1 MHz.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientMhz, 1);
         }
         
         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.FahLogFileName, xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load remote FAHlog.txt filename.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.FahLogFileName, Constants.LocalFahLog);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.UnitInfoFileName, xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load remote unitinfo.txt filename.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.UnitInfoFileName, Constants.LocalUnitInfo);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.QueueFileName, xmlData.SelectSingleNode(xmlNodeQueue).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load remote queue.dat filename.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.QueueFileName, Constants.LocalQueue);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Server, xmlData.SelectSingleNode(xmlPropServ).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load client server.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Server, String.Empty);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Username, xmlData.SelectSingleNode(xmlPropUser).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load server username.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Username, String.Empty);
         }

         var symetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Password, String.Empty);
            if (xmlData.SelectSingleNode(xmlPropPass).InnerText.Length > 0)
            {
               try
               {
                  symetricProvider.IntializationVector = _iv;
                  dataInterface.SetSetting(ClientInstanceSettingsKeys.Password, symetricProvider.Decrypt(new Data(Utils.FromBase64(
                     xmlData.SelectSingleNode(xmlPropPass).InnerText)), _symmetricKey).ToString());
               }
               catch (FormatException)
               {
                  OnWarningMessage("Server Password is not Base64 encoded... loading clear value.");
                  dataInterface.SetSetting(ClientInstanceSettingsKeys.Password, xmlData.SelectSingleNode(xmlPropPass).InnerText);
               }
               catch (CryptographicException)
               {
                  OnWarningMessage("Cannot decrypt Server Password... loading clear value.");
                  dataInterface.SetSetting(ClientInstanceSettingsKeys.Password, xmlData.SelectSingleNode(xmlPropPass).InnerText);
               }
            }
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load server password.");
         }
         
         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.FtpMode, xmlData.SelectSingleNode(xmlPropPassiveFtpMode).InnerText);
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load ftp mode flag, defaulting to passive.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.FtpMode, FtpType.Passive.ToString());
         }
         catch (FormatException)
         {
            OnWarningMessage("Could not parse ftp mode flag, defaulting to passive.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.FtpMode, FtpType.Passive.ToString());
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.UtcOffsetIsZero, Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText));
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load client VM flag, defaulting to false.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.UtcOffsetIsZero, false);
         }
         catch (FormatException)
         {
            OnWarningMessage("Could not parse client VM flag, defaulting to false.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.UtcOffsetIsZero, false);
         }

         try
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientTimeOffset, Int32.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText));
         }
         catch (NullReferenceException)
         {
            OnWarningMessage("Cannot load client time offset, defaulting to 0.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientTimeOffset, 0);
         }
         catch (FormatException)
         {
            OnWarningMessage("Could not parse client time offset, defaulting to 0.");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.ClientTimeOffset, 0);
         }
      }
      #endregion

      public event EventHandler<SettingsSerializerMessageEventArgs> WarningMessage;
      
      private void OnWarningMessage(string message)
      {
         if (WarningMessage != null)
         {
            WarningMessage(this, new SettingsSerializerMessageEventArgs(message));
         }
      }

      public string FileExtension
      {
         get { return "hfm"; }
      }

      public string FileTypeFilter
      {
         get { return "HFM Configuration Files|*.hfm"; }
      }

      public IInstanceCollectionDataInterface DataInterface
      {
         get { return _dataInterface; }
         set { _dataInterface = value; }
      }
   }
}
