using System.IO;
using System.Xml.Serialization;

using HFM.Core.Serializers;

namespace HFM.Core
{
    public class ApplicationUpdateSerializer : ISerializer<ApplicationUpdate>
    {
        public void Serialize(Stream stream, ApplicationUpdate value)
        {
            var s = new XmlSerializer(typeof(ApplicationUpdate));
            s.Serialize(stream, value);
        }

        public ApplicationUpdate Deserialize(Stream stream)
        {
            var s = new XmlSerializer(typeof(ApplicationUpdate));
            return (ApplicationUpdate)s.Deserialize(stream);
        }
    }
}
