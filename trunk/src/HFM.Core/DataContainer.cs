
using System;

using Castle.Core.Logging;

using HFM.Core.DataTypes.Serializers;

namespace HFM.Core
{
   public abstract class DataContainer<T> where T : class, new()
   {
      private T _data;

      public T Data
      {
         get { return _data; }
         set { _data = value ?? new T(); }
      }

      public ILogger Logger { get; set; }

      public string FileName { get; set; }

      public IFileSerializer<T> Serializer { get; set; }

      protected DataContainer()
      {
         Data = new T();
         Logger = NullLogger.Instance;
      }

      #region Serialization Support

      private static readonly object SerializeLock = new object();

      /// <summary>
      /// Read data file.
      /// </summary>
      public void Read()
      {
         //DateTime start = HfmTrace.ExecStart;

         T data = null;
         try
         {
            data = Serializer.Deserialize(FileName);
         }
         catch (Exception ex)
         {
            Logger.ErrorFormat(ex, "{0}", ex.Message);
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         Data = data ?? new T();
      }

      /// <summary>
      /// Write data file.
      /// </summary>
      public void Write()
      {
         //DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            try
            {
               Serializer.Serialize(FileName, Data);
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }
         }

         //HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      #endregion
   }
}
