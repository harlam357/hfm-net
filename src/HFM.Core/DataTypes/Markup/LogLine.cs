
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes.Markup
{
   [DataContract(Namespace = "")]
   public class LogLine
   {
      [DataMember(Order = 1)]
      public string LineType { get; set; }

      [DataMember(Order = 2)]
      public int LineIndex { get; set; }

      [DataMember(Order = 3)]
      public string LineRaw { get; set; }

      public override string ToString()
      {
         return LineRaw;
      }
   }
}
