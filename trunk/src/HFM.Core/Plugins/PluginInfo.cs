
using System;
using System.Diagnostics;

namespace HFM.Core.Plugins
{
   public sealed class PluginInfo<T>
   {
      public T Interface { get; set; }
      
      private readonly string _filePath;

      public string FilePath
      {
         get { return _filePath; }
      }

      private readonly string _name;

      public string Name
      {
         get { return _name; }
      }

      private readonly string _fileVersion = String.Empty;

      public string FileVersion
      {
         get { return _fileVersion; }
      }

      private readonly string _description = String.Empty;

      public string Description
      {
         get { return _description; }
      }

      private readonly string _author = String.Empty;

      public string Author
      {
         get { return _author; }
      }

      internal PluginInfo(string filePath, FileVersionInfo fvi)
      {
         if (filePath == null) throw new ArgumentNullException("filePath");
         if (fvi == null) throw new ArgumentNullException("fvi");

         Debug.Assert(fvi.FileDescription != null && fvi.FileDescription.Trim().Length != 0);

         _filePath = filePath;
         _name = fvi.FileDescription;

         if (fvi.FileVersion != null) _fileVersion = fvi.FileVersion;
         if (fvi.Comments != null) _description = fvi.Comments;
         if (fvi.CompanyName != null) _author = fvi.CompanyName;
      }
   }
}
