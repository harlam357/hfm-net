using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HFM.Proteins
{
    /// <summary>
    /// Serializes and deserializes a collection of <see cref="Protein"/> objects.
    /// </summary>
    public interface IProteinCollectionSerializer
    {
        /// <summary>
        /// Serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
        /// </summary>
        void Serialize(Stream stream, ICollection<Protein> collection);

        /// <summary>
        /// Asynchronously serializes a collection of <see cref="Protein"/> objects to a <see cref="Stream"/>.
        /// </summary>
        Task SerializeAsync(Stream stream, ICollection<Protein> collection);

        /// <summary>
        /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        ICollection<Protein> Deserialize(Stream stream);

        /// <summary>
        /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
        /// </summary>
        Task<ICollection<Protein>> DeserializeAsync(Stream stream);
    }
}
