
using System.IO;

namespace HFM.Core.DataTypes
{
   public interface ISerializer<T>
   {
      void Serialize(Stream stream, T value);

      T Deserialize(Stream stream);
   }
}
