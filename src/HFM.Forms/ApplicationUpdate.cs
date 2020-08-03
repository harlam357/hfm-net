
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace HFM.Forms
{
    [Serializable]
    public class ApplicationUpdate
    {
        public string Version { get; set; }
        public DateTime UpdateDate { get; set; }
        public List<ApplicationUpdateFile> UpdateFiles { get; set; }

        public ApplicationUpdate()
        {
            Version = String.Empty;
            UpdateDate = DateTime.MinValue;
            UpdateFiles = new List<ApplicationUpdateFile>();
        }
    }

    [Serializable]
    public class ApplicationUpdateFile
    {
        // ReSharper disable InconsistentNaming
        public string Name { get; set; }
        public string Description { get; set; }
        public string HttpAddress { get; set; }
        public int Size { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public int UpdateType { get; set; }
        // ReSharper restore InconsistentNaming

        public ApplicationUpdateFile()
        {
            HttpAddress = String.Empty;
            MD5 = String.Empty;
            SHA1 = String.Empty;
        }
    }

    public static class ApplicationUpdateSerializer
    {
        public static void SerializeToXml(ApplicationUpdate update, string filePath)
        {
            using (TextWriter stream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var s = new XmlSerializer(typeof(ApplicationUpdate));
                s.Serialize(stream, update);
            }
        }

        public static ApplicationUpdate DeserializeFromXml(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var s = new XmlSerializer(typeof(ApplicationUpdate));
                return (ApplicationUpdate)s.Deserialize(stream);
            }
        }
    }
}
