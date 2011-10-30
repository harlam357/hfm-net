
using System;
using System.Linq;

using HFM.Core.DataTypes.Serializers;

namespace HFM.Core.Plugins
{
   internal class ProteinSerializerPluginManager : PluginManager<IProteinSerializer>
   {
      protected override bool ValidatePlugin(IProteinSerializer serializer)
      {
         if (String.IsNullOrEmpty(serializer.FileExtension) ||
             String.IsNullOrEmpty(serializer.FileTypeFilter))
         {
            // extention filter string, too many bar characters
            return false;
         }

         var numOfBarChars = serializer.FileTypeFilter.Count(x => x == '|');
         if (numOfBarChars != 1)
         {
            // too many bar characters
            return false;
         }

         return true;
      }
   }
}
