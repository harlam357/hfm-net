
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Xml;

using HFM.Core.Internal;
using HFM.Core.Logging;
using HFM.Core.Serializers;

namespace HFM.Core.Client
{
    public class ClientSettingsFileSerializer : IFileSerializer<List<ClientSettings>>
    {
        #region Fields

        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger ?? (_logger = NullLogger.Instance);
            set => _logger = value;
        }

        // Encryption Key and Initialization Vector
        private const string IV = "CH/&QE;NsT.2z+Me";
        private const string SymmetricKey = "usPP'/Cb5?NWC*60";

        #endregion

        public const string DefaultFileExtension = "hfmx";

        public string FileExtension => DefaultFileExtension;

        public const string DefaultFileTypeFilter = "HFM Configuration Files|*.hfmx";

        public string FileTypeFilter => DefaultFileTypeFilter;

        public List<ClientSettings> Deserialize(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = XmlReader.Create(fileStream))
            using (var noNamespaceReader = new NoNamespaceXmlReader(reader))
            {
                var serializer = new DataContractSerializer(typeof(List<ClientSettings>));
                var value = (List<ClientSettings>)serializer.ReadObject(noNamespaceReader);
                Decrypt(value);
                return value;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public void Serialize(string path, List<ClientSettings> value)
        {
            // for configurations without Guid values, add them when saving
            GenerateRequiredGuids(value);

            // copy the values before encrypting, otherwise the ClientSettings
            // objects will retain the encrypted value from here on out...
            var valueCopy = ProtoBuf.Serializer.DeepClone(value);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
            {
                var serializer = new DataContractSerializer(typeof(List<ClientSettings>));
                Encrypt(valueCopy);
                serializer.WriteObject(xmlWriter, valueCopy);
            }
        }

        private static void GenerateRequiredGuids(IEnumerable<ClientSettings> collection)
        {
            foreach (var settings in collection.Where(x => x.Guid == Guid.Empty))
            {
                settings.Guid = Guid.NewGuid();
            }
        }

        #region Encryption

        private void Encrypt(IEnumerable<ClientSettings> value)
        {
            var cryptography = new Cryptography(SymmetricKey, IV);
            foreach (var settings in value)
            {
                if (String.IsNullOrWhiteSpace(settings.Password)) continue;

                try
                {
                    settings.Password = cryptography.EncryptValue(settings.Password);
                }
                catch (CryptographicException)
                {
                    Logger.Warn(String.Format(Logging.Logger.NameFormat, settings.Name, "Failed to encrypt password... saving clear value."));
                }
            }
        }

        private void Decrypt(IEnumerable<ClientSettings> value)
        {
            var cryptography = new Cryptography(SymmetricKey, IV);
            foreach (var settings in value)
            {
                if (String.IsNullOrWhiteSpace(settings.Password)) continue;

                try
                {
                    settings.Password = cryptography.DecryptValue(settings.Password);
                }
                catch (FormatException)
                {
                    Logger.Warn(String.Format(Logging.Logger.NameFormat, settings.Name, "Failed to decrypt password... loading clear value."));
                }
                catch (CryptographicException)
                {
                    Logger.Warn(String.Format(Logging.Logger.NameFormat, settings.Name, "Failed to decrypt password... loading clear value."));
                }
            }
        }

        #endregion

        private class NoNamespaceXmlReader : XmlReader
        {
            private readonly XmlReader _inner;

            public NoNamespaceXmlReader(XmlReader inner)
            {
                _inner = inner;
            }

            public override int AttributeCount => _inner.AttributeCount;

            public override string BaseURI => _inner.BaseURI;

            public override void Close()
            {
                _inner.Close();
            }

            public override int Depth => _inner.Depth;

            public override bool EOF => _inner.EOF;

            public override string GetAttribute(int i)
            {
                return _inner.GetAttribute(i);
            }

            public override string GetAttribute(string name, string namespaceURI)
            {
                return _inner.GetAttribute(name, namespaceURI);
            }

            public override string GetAttribute(string name)
            {
                return _inner.GetAttribute(name);
            }

            public override bool IsEmptyElement => _inner.IsEmptyElement;

            public override string LocalName => _inner.LocalName;

            public override string LookupNamespace(string prefix)
            {
                return _inner.LookupNamespace(prefix);
            }

            public override bool MoveToAttribute(string name, string ns)
            {
                return _inner.MoveToAttribute(name, ns);
            }

            public override bool MoveToAttribute(string name)
            {
                return _inner.MoveToAttribute(name);
            }

            public override bool MoveToElement()
            {
                return _inner.MoveToElement();
            }

            public override bool MoveToFirstAttribute()
            {
                return _inner.MoveToFirstAttribute();
            }

            public override bool MoveToNextAttribute()
            {
                return _inner.MoveToNextAttribute();
            }

            public override XmlNameTable NameTable => _inner.NameTable;

            public override string NamespaceURI => String.Empty;

            public override XmlNodeType NodeType => _inner.NodeType;

            public override string Prefix => _inner.Prefix;

            public override bool Read()
            {
                return _inner.Read();
            }

            public override bool ReadAttributeValue()
            {
                return _inner.ReadAttributeValue();
            }

            public override ReadState ReadState => _inner.ReadState;

            public override void ResolveEntity()
            {
                _inner.ResolveEntity();
            }

            public override string Value => _inner.Value;
        }
    }
}
