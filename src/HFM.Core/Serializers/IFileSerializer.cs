
namespace HFM.Core.Serializers
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
      /// <param name="path">A string specifying the path of the file to deserialize.</param>
      T Deserialize(string path);

      /// <summary>
      /// Serialize the value to the file specified by the file name.
      /// </summary>
      /// <param name="path">A string specifying the path of the file to serialize.</param>
      /// <param name="value">A value to serialize.</param>
      void Serialize(string path, T value);
   }
}
