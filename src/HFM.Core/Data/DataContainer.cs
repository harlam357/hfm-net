using System;

using HFM.Core.Logging;

namespace HFM.Core.Data
{
    public abstract class DataContainer<T> where T : class, new()
    {
        private T _data;

        public T Data
        {
            get => _data;
            set => _data = value ?? new T();
        }

        public ILogger Logger { get; }

        public string FilePath { get; set; }

        public abstract Serializers.IFileSerializer<T> DefaultSerializer { get; }

        protected DataContainer(ILogger logger)
        {
            Logger = logger ?? NullLogger.Instance;
            Data = new T();
        }

        #region Serialization Support

        private readonly object _serializeLock = new object();

        /// <summary>
        /// Read data file.
        /// </summary>
        public virtual void Read()
        {
            T data = null;

            lock (_serializeLock)
            {
                try
                {
                    data = DefaultSerializer.Deserialize(FilePath);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }

            Data = data ?? new T();
        }

        /// <summary>
        /// Write data file.
        /// </summary>
        public virtual void Write()
        {
            lock (_serializeLock)
            {
                try
                {
                    DefaultSerializer.Serialize(FilePath, Data);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                }
            }
        }

        #endregion
    }
}
