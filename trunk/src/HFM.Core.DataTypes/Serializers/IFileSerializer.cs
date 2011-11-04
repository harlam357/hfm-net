
namespace HFM.Core.DataTypes.Serializers
{
   public interface IFileSerializer<T> where T : class, new()
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
      /// Deserialize the file and return the value.
      /// </summary>
      /// <param name="fileName">A string specifying the file name to deserialize.</param>
      T Deserialize(string fileName);

      /// <summary>
      /// Serialize the value to the file specified by the file name.
      /// </summary>
      /// <param name="fileName">A string specifying the file name to serialize.</param>
      /// <param name="value">A value to serialize.</param>
      void Serialize(string fileName, T value);
   }
}
