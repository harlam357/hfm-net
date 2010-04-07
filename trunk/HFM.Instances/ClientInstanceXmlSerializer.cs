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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class ClientInstanceXmlSerializer
   {
      #region Constants
      // Xml Serialization Constants
      private const string xmlNodeInstance = "Instance";
      private const string xmlAttrName = "Name";
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
      #endregion

      #region Members
      // Encryption Key and Initialization Vector
      private readonly Data IV = new Data("zX!1=D,^7K@u33+d");
      private readonly Data SymmetricKey = new Data("cNx/7+,?%ubm*?j8");
      
      private readonly ClientInstanceFactory _Factory;
      #endregion
      
      public ClientInstanceXmlSerializer(ClientInstanceFactory factory)
      {
         _Factory = factory;
      }

      #region XML Serialization
      /// <summary>
      /// Serialize this Client Instance to Xml
      /// </summary>
      /// <exception cref="ArgumentException">Throws if xmlDocName is null, empty, or not a rooted path.</exception>
      /// <exception cref="XmlException">Throws if serialization fails.  Inspect InnerException for original Exception.</exception>
      public void Serialize(string xmlDocName, ICollection<ClientInstance> instances)
      {
         if (String.IsNullOrEmpty(xmlDocName))
         {
            throw new ArgumentException("Argument 'xmlDocName' cannot be a null or empty string.", "xmlDocName");
         }

         // Save the XML stream to the file
         if (Path.IsPathRooted(xmlDocName) == false)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
               "Argument 'xmlDocName' must be a rooted path.  Given path '{0}'.", xmlDocName), "xmlDocName");
         }

         DateTime Start = HfmTrace.ExecStart;

         try
         {
            XmlDocument xmlData = new XmlDocument();

            // Create the XML Declaration (well formed)
            XmlDeclaration xmlDeclaration = xmlData.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlData.InsertBefore(xmlDeclaration, xmlData.DocumentElement);

            // Create the root element
            XmlElement xmlRoot = xmlData.CreateElement("Hosts");

            // Loop through the collection and serialize the lot
            foreach (ClientInstance instance in instances)
            {
               XmlDocument xmlDoc = Serialize(instance);
               foreach (XmlNode xn in xmlDoc.ChildNodes)
               {
                  xmlRoot.AppendChild(xmlData.ImportNode(xn, true));
               }
            }

            xmlData.AppendChild(xmlRoot);

            xmlData.Save(xmlDocName);
         }
         catch (Exception ex)
         {
            throw new XmlException("Failed to serialize collection.", ex);
         }
         
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
      }

      /// <summary>
      /// Serialize this Client Instance to Xml
      /// </summary>
      private XmlDocument Serialize(IClientInstanceSettings instance)
      {
         XmlDocument xmlData = new XmlDocument();

         XmlElement xmlRoot = xmlData.CreateElement(xmlNodeInstance);
         xmlRoot.SetAttribute(xmlAttrName, instance.InstanceName);
         xmlData.AppendChild(xmlRoot);

         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeFAHLog, instance.RemoteFAHLogFilename));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeUnitInfo, instance.RemoteUnitInfoFilename));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeQueue, instance.RemoteQueueFilename));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientMHz, instance.ClientProcessorMegahertz.ToString()));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientVM, instance.ClientIsOnVirtualMachine.ToString()));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlNodeClientOffset, instance.ClientTimeOffset.ToString()));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropType, instance.InstanceHostType.ToString()));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPath, instance.Path));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropServ, instance.Server));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropUser, instance.Username));

         Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         string encryptedPassword = String.Empty;
         if (instance.Password.Length > 0)
         {
            try
            {
               SymetricProvider.IntializationVector = IV;
               encryptedPassword = SymetricProvider.Encrypt(new Data(instance.Password), SymmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, instance.InstanceName, "Failed to encrypt Server Password... saving clear value.");
               encryptedPassword = instance.Password;
            }
         }
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPass, encryptedPassword));
         xmlData.ChildNodes[0].AppendChild(XMLOps.createXmlNode(xmlData, xmlPropPassiveFtpMode, instance.FtpMode.ToString()));

         return xmlData;
      }

      /// <summary>
      /// Loads a collection of Host Instances from disk
      /// </summary>
      /// <param name="xmlDocName">XML File Name</param>
      /// <exception cref="ArgumentException">Throws if xmlDocName is null or empty.</exception>
      /// <exception cref="XmlException">Throws if deserialization fails.  Inspect InnerException for original Exception.</exception>
      public IList<ClientInstance> Deserialize(string xmlDocName)
      {
         if (String.IsNullOrEmpty(xmlDocName))
         {
            throw new ArgumentException("Argument 'xmlDocName' cannot be a null or empty string.", "xmlDocName");
         }
      
         DateTime Start = HfmTrace.ExecStart;

         IList<ClientInstance> list;

         try
         {
            XmlDocument xmlData = new XmlDocument();
            xmlData.Load(xmlDocName);

            list = new List<ClientInstance>();

            // xmlData now contains the collection of Nodes. Hopefully.
            xmlData.RemoveChild(xmlData.ChildNodes[0]);

            foreach (XmlNode xn in xmlData.ChildNodes[0])
            {
               string InstanceType = xn.SelectSingleNode("HostType").InnerText;

               if (Enum.IsDefined(typeof (InstanceType), InstanceType))
               {
                  InstanceType type = (InstanceType) Enum.Parse(typeof (InstanceType), InstanceType, false);
                  ClientInstance instance = Deserialize(xn);
                  instance.InstanceHostType = type;

                  list.Add(instance);
               }
               else
               {
                  throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                     "Cannot deserialize Instance Type '{0}'.", InstanceType));
               }
            }
         }
         catch (Exception ex)
         {
            throw new XmlException("Failed to deserialize collection.", ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
         
         return list;
      }

      /// <summary>
      /// Deserialize Xml data into this Client Instance
      /// </summary>
      /// <param name="xmlData">XmlNode containing the client instance data</param>
      private ClientInstance Deserialize(XmlNode xmlData)
      {
         ClientInstance instance = _Factory.Create();

         string instanceName = xmlData.Attributes[xmlAttrName].ChildNodes[0].Value;
         if (StringOps.ValidateInstanceName(instanceName) == false)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format(CultureInfo.CurrentCulture, 
               "Instance Name '{0}' contains invalid characters... cleaning.", instanceName), true);
            instanceName = StringOps.CleanInstanceName(instanceName);
         }
         instance.InstanceName = instanceName;
         
         try
         {
            instance.RemoteFAHLogFilename = xmlData.SelectSingleNode(xmlNodeFAHLog).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote FAHlog Filename."));
            instance.RemoteFAHLogFilename = Constants.LocalFAHLog;
         }

         try
         {
            instance.RemoteUnitInfoFilename = xmlData.SelectSingleNode(xmlNodeUnitInfo).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote Unitinfo Filename."));
            instance.RemoteUnitInfoFilename = Constants.LocalUnitInfo;
         }

         try
         {
            instance.RemoteQueueFilename = xmlData.SelectSingleNode(xmlNodeQueue).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Remote Queue Filename."));
            instance.RemoteQueueFilename = Constants.LocalQueue;
         }

         try
         {
            instance.ClientProcessorMegahertz = int.Parse(xmlData.SelectSingleNode(xmlNodeClientMHz).InnerText);
            if (instance.ClientProcessorMegahertz < 1)
            {
               instance.ClientProcessorMegahertz = 1;
            }
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client MHz, defaulting to 1 MHz."));
            instance.ClientProcessorMegahertz = 1;
         }
         catch (FormatException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client MHz, defaulting to 1 MHz."));
            instance.ClientProcessorMegahertz = 1;
         }

         try
         {
            instance.ClientIsOnVirtualMachine = Convert.ToBoolean(xmlData.SelectSingleNode(xmlNodeClientVM).InnerText);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client VM Flag, defaulting to false."));
            instance.ClientIsOnVirtualMachine = false;
         }
         catch (InvalidCastException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client VM Flag, defaulting to false."));
            instance.ClientIsOnVirtualMachine = false;
         }

         try
         {
            instance.ClientTimeOffset = int.Parse(xmlData.SelectSingleNode(xmlNodeClientOffset).InnerText);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Time Offset, defaulting to 0."));
            instance.ClientTimeOffset = 0;
         }
         catch (FormatException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Client Time Offset, defaulting to 0."));
            instance.ClientTimeOffset = 0;
         }

         try
         {
            instance.Path = xmlData.SelectSingleNode(xmlPropPath).InnerText;
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Path."));
         }

         try
         {
            instance.Server = xmlData.SelectSingleNode(xmlPropServ).InnerText;
         }
         catch (NullReferenceException)
         {
            instance.Server = String.Empty;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Client Server."));
         }

         try
         {
            instance.Username = xmlData.SelectSingleNode(xmlPropUser).InnerText;
         }
         catch (NullReferenceException)
         {
            instance.Username = String.Empty;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Server Username."));
         }

         Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         try
         {
            instance.Password = String.Empty;
            if (xmlData.SelectSingleNode(xmlPropPass).InnerText.Length > 0)
            {
               try
               {
                  SymetricProvider.IntializationVector = IV;
                  instance.Password = SymetricProvider.Decrypt(new Data(Utils.FromBase64(xmlData.SelectSingleNode(xmlPropPass).InnerText)), SymmetricKey).ToString();
               }
               catch (FormatException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, instance.InstanceName, "Server Password is not Base64 encoded... loading clear value.");
                  instance.Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, instance.InstanceName, "Cannot decrypt Server Password... loading clear value.");
                  instance.Password = xmlData.SelectSingleNode(xmlPropPass).InnerText;
               }
            }
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Server Password."));
         }

         try
         {
            string FtpModeText = xmlData.SelectSingleNode(xmlPropPassiveFtpMode).InnerText;
            instance.FtpMode = (FtpType)Enum.Parse(typeof(FtpType), FtpModeText, false);
         }
         catch (NullReferenceException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Cannot load Ftp Mode Flag, defaulting to Passive."));
            instance.FtpMode = FtpType.Passive;
         }
         catch (InvalidCastException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", HfmTrace.FunctionName, "Could not parse Ftp Mode Flag, defaulting to Passive."));
            instance.FtpMode = FtpType.Passive;
         }
         
         return instance;
      }
      #endregion
   }
}
