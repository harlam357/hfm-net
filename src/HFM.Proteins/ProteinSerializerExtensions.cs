
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HFM.Proteins
{
   public static class ProteinSerializerExtensions
   {
      public static ICollection<Protein> ReadFile(this IProteinSerializer serializer, string path)
      {
         using (var stream = File.OpenRead(path))
         {
            return serializer.Deserialize(stream);
         }
      }

      public static async Task<ICollection<Protein>> ReadFileAsync(this IProteinSerializer serializer, string path)
      {
         using (var stream = File.OpenRead(path))
         {
            return await serializer.DeserializeAsync(stream).ConfigureAwait(false);
         }
      }

      public static ICollection<Protein> ReadUri(this IProteinSerializer serializer, Uri address)
      {
         var client = new WebClient();
         using (var stream = client.OpenRead(address))
         {
            return serializer.Deserialize(stream);
         }
      }

      public static async Task<ICollection<Protein>> ReadUriAsync(this IProteinSerializer serializer, Uri address)
      {
         var client = new WebClient();
         using (var stream = await client.OpenReadTaskAsync(address))
         {
            return serializer.Deserialize(stream);
         }
      }

      public static void WriteFile(this IProteinSerializer serializer, string path, ICollection<Protein> values)
      {
         using (var stream = File.Create(path))
         {
            serializer.Serialize(stream, values);
         }
      }

      public static async Task WriteFileAsync(this IProteinSerializer serializer, string path, ICollection<Protein> values)
      {
         using (var stream = File.Create(path))
         {
            await serializer.SerializeAsync(stream, values).ConfigureAwait(false);
         }
      }
   }
}
