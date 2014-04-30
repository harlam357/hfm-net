
using System;

namespace HFM.Core.Plugins
{
   public enum PluginLoadResult
   {
      Success,
      Failure
   }

   public sealed class PluginLoadInfo
   {
      private readonly string _filePath;
      /// <summary>
      /// Plugin File Path
      /// </summary>
      public string FilePath
      {
         get { return _filePath; }
      }

      private readonly PluginLoadResult _result;

      public PluginLoadResult Result
      {
         get { return _result; }
      }

      private readonly string _message = String.Empty;

      public string Message
      {
         get { return _exception != null ? _exception.Message : _message; }
      }

      private readonly Exception _exception;

      public Exception Exception
      {
         get { return _exception; }
      }

      internal PluginLoadInfo(string filePath, PluginLoadResult result)
      {
         _filePath = filePath;
         _result = result;
      }

      internal PluginLoadInfo(string filePath, PluginLoadResult result, string message)
      {
         _filePath = filePath;
         _result = result;
         _message = message;
      }

      internal PluginLoadInfo(string filePath, PluginLoadResult result, Exception ex)
      {
         _filePath = filePath;
         _result = result;
         _exception = ex;
      }
   }
}
