
using System.Collections.Generic;
using System.IO;

namespace HFM.Proteins
{
   public interface IProteinSerializer
   {
      void Serialize(Stream stream, List<Protein> value);

      List<Protein> Deserialize(Stream stream);
   }
}
