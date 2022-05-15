using System.Runtime.Serialization;
using System.Xml;

namespace HFM.Core.Serializers
{
    public class DataContractFileSerializer<T> : IFileSerializer<T> where T : class, new()
    {
        public string FileExtension => "xml";

        public string FileTypeFilter => "Xml Files|*.xml";

        public T Deserialize(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(fileStream);
            }
        }

        public void Serialize(string path, T value)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(xmlWriter, value);
            }
        }
    }
}
