using System.IO;

namespace HFM.Core.Serializers
{
    public class ProtoBufFileSerializer<T> : IFileSerializer<T> where T : class, new()
    {
        public string FileExtension => "dat";

        public string FileTypeFilter => "HFM Data Files|*.dat";

        public T Deserialize(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ProtoBuf.Serializer.Deserialize<T>(fileStream);
            }
        }

        public void Serialize(string path, T value)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                ProtoBuf.Serializer.Serialize(fileStream, value);
            }
        }
    }
}
