
using System;
using System.Linq;

namespace HFM.Core.Plugins
{
   internal class FileSerializerPluginManager<T> : PluginManager<IFileSerializer<T>> where T : class, new()
   {
      protected override bool ValidatePlugin(IFileSerializer<T> serializer)
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
