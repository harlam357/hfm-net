
using System.Collections.Generic;

namespace HFM.Core.DataTypes.Serializers
{
   public interface IProteinSerializer
   {
      /// <summary>
      /// File Extension (no dot)
      /// </summary>
      string FileExtension { get; }

      /// <summary>
      /// File Type Filter for File Dialogs (example "Xml Files|*.xml")
      /// </summary>
      string FileTypeFilter { get; }

      /// <summary>
      /// Deserialize the file and load proteins via the IProteinLoader.
      /// </summary>
      /// <param name="fileName">A string specifying the file name to deserialize.</param>
      IEnumerable<Protein> Deserialize(string fileName);

      /// <summary>
      /// Serialize the values to the file specified by the file name.
      /// </summary>
      /// <param name="fileName">A string specifying the file name to serialize.</param>
      /// <param name="values">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing protein values to serialize.</param>
      void Serialize(string fileName, IEnumerable<Protein> values);
   }
}
