
using System.IO;
using System.Runtime.Serialization;

using HFM.Plugins;

namespace HFM.Core.Serializers
{
   public class XmlFileSerializer<T> : IFileSerializer<T> where T : class, new()
   {
      #region IProteinBenchmarkSerializer Members

      public string FileExtension
      {
         get { return "xml"; }
      }

      public string FileTypeFilter
      {
         get { return "Xml Files|*.xml"; }
      }

      public T Deserialize(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(fileStream);
         }
      }

      public void Serialize(string fileName, T value)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
         {
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(fileStream, value);
         }
      }

      #endregion
   }
}
