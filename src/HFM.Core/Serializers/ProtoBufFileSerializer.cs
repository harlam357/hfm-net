
using System.IO;

using HFM.Core.Plugins;

namespace HFM.Core.Serializers
{
   public class ProtoBufFileSerializer<T> : IFileSerializer<T> where T : class, new()
   {
      public virtual string FileExtension
      {
         get { return "dat"; }
      }

      public virtual string FileTypeFilter
      {
         get { return "HFM Data Files|*.dat"; }
      }

      public T Deserialize(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            return ProtoBuf.Serializer.Deserialize<T>(fileStream);
         }
      }

      public void Serialize(string fileName, T value)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
         {
            ProtoBuf.Serializer.Serialize(fileStream, value);
         }
      }
   }
}
