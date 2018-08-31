
using System.IO;

namespace HFM.Core.Serializers
{
   public interface ISerializer<T>
   {
      void Serialize(Stream stream, T value);

      T Deserialize(Stream stream);
   }
}
