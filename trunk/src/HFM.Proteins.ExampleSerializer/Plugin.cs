
using System;
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Proteins.ExampleSerializer
{
   public class Plugin : IFileSerializer<List<Protein>>
   {
      public string FileExtension
      {
         get { return "xml"; }
         //get { return null; }
      }

      /// <summary>
      /// File Type Filter for File Dialogs (example "Xml Files|*.xml")
      /// </summary>
      public string FileTypeFilter
      {
         get { return "Xml Files|*.xml"; }
      }

      public List<Protein> Deserialize(string fileName)
      {
         throw new NotImplementedException();
      }

      public void Serialize(string fileName, List<Protein> values)
      {
         throw new NotImplementedException();
      }
   }
}
