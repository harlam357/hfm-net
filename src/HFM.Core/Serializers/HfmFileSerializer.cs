/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;

using Castle.Core.Logging;

using harlam357.Core.Security;
using harlam357.Core.Security.Cryptography;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Core.Serializers
{
   public class HfmFileSerializer : IFileSerializer<List<ClientSettings>>
   {
      #region Fields

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      // Encryption Key and Initialization Vector
      private readonly SymmetricKeyData _iv = new SymmetricKeyData("CH/&QE;NsT.2z+Me");
      private readonly SymmetricKeyData _symmetricKey = new SymmetricKeyData("usPP'/Cb5?NWC*60");

      #endregion

      public string FileExtension
      {
         get { return "hfmx"; }
      }

      public string FileTypeFilter
      {
         get { return "HFM Configuration Files|*.hfmx"; }
      }

      public List<ClientSettings> Deserialize(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            var serializer = new DataContractSerializer(typeof(List<ClientSettings>));
            var value = (List<ClientSettings>)serializer.ReadObject(fileStream);
            Decrypt(value);
            return value;
         }
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      public void Serialize(string fileName, List<ClientSettings> value)
      {
         // copy the values before encrypting, otherwise the ClientSettings
         // objects will retain the encrypted value from here on out...
         var valueCopy = ProtoBuf.Serializer.DeepClone(value);

         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
         using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
         {
            var serializer = new DataContractSerializer(typeof(List<ClientSettings>));
            Encrypt(valueCopy);
            serializer.WriteObject(xmlWriter, valueCopy);
         }
      }

      #region Encryption

      private void Encrypt(IEnumerable<ClientSettings> value)
      {
         var symetricProvider = new Symmetric(SymmetricProvider.Rijndael, false) { IntializationVector = _iv };
         foreach (var settings in value)
         {
            if (settings.Password.Length == 0) continue;

            try
            {
               settings.Password = symetricProvider.Encrypt(new Data(settings.Password), _symmetricKey).Bytes.ToBase64();
            }
            catch (CryptographicException)
            {
               Logger.WarnFormat(Constants.ClientNameFormat, settings.Name, "Failed to encrypt password... saving clear value.");
            }
         }
      }

      private void Decrypt(IEnumerable<ClientSettings> value)
      {
         var symetricProvider = new Symmetric(SymmetricProvider.Rijndael, false) { IntializationVector = _iv };
         foreach (var settings in value)
         {
            if (settings.Password.Length == 0) continue;

            try
            {
               settings.Password = symetricProvider.Decrypt(new Data(settings.Password.FromBase64()), _symmetricKey).ToString();
            }
            catch (FormatException)
            {
               Logger.WarnFormat(Constants.ClientNameFormat, settings.Name, "Failed to decrypt password... loading clear value.");
            }
            catch (CryptographicException)
            {
               Logger.WarnFormat(Constants.ClientNameFormat, settings.Name, "Failed to decrypt password... loading clear value.");
            }
         }
      }

      #endregion
   }
}
