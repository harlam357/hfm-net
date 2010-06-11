
using System;

namespace HFM.Plugins
{
   public interface IClientInstanceSettingsSerializer
   {
      event EventHandler<SettingsSerializerMessageEventArgs> WarningMessage;
      
      string FileExtension { get; }
      
      string FileTypeDescription { get; }
      
      IInstanceCollectionDataInterface DataInterface { set; }

      /// <summary>
      /// Serialize a collection of Client Instances to disk
      /// </summary>
      /// <param name="fileName">Path of File to Serialize</param>
      void Serialize(string fileName);

      /// <summary>
      /// Loads a collection of Client Instances from disk
      /// </summary>
      /// <param name="fileName">Path of File to Deserialize</param>
      void Deserialize(string fileName);
   }
   
   public class SettingsSerializerMessageEventArgs : EventArgs
   {
      private string _message;
   
      public string Message
      {
         get { return _message; }
         set { _message = value == null ? String.Empty : value.Trim(); }
      }
      
      public SettingsSerializerMessageEventArgs()
      {
         Message = String.Empty;
      }
      
      public SettingsSerializerMessageEventArgs(string message)
      {
         Message = message;
      }
   }
}
