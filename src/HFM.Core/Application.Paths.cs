
using System;

namespace HFM.Core
{
   public static partial class Application
   {
      public static string FolderPath;

      public static readonly string DataFolderPath =
         System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ExeName);
   }
}
