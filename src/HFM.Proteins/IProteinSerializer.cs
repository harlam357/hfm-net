
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Proteins
{
   public interface IProteinSerializer
   {
      void Serialize(Stream stream, ICollection<Protein> collection);

      Task SerializeAsync(Stream stream, ICollection<Protein> collection);

      ICollection<Protein> Deserialize(Stream stream);

      Task<ICollection<Protein>> DeserializeAsync(Stream stream);
   }
}
