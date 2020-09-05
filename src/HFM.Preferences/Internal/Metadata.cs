/*
 * Based on code by Bryan Watts
 * http://stackoverflow.com/questions/353126/c-multiple-generic-types-in-one-list/1351071
 */

using System;
using System.Linq.Expressions;

using HFM.Preferences.Data;

namespace HFM.Preferences.Internal
{
    internal interface IMetadata
    {
        Type DataType { get; }

        object Data { get; set; }
    }

    internal interface IMetadata<T> : IMetadata
    {
        new T Data { get; set; }
    }

    // Original Metadata<T> class
    //public class Metadata<T> : IMetadata<T>
    //{
    //   public Metadata()
    //   {
    //      Data = default(T);
    //   }
    //
    //   public Metadata(T data)
    //   {
    //      Data = data;
    //   }
    //
    //   public Type DataType
    //   {
    //      get { return typeof(T); }
    //   }
    //
    //   object IMetadata.Data
    //   {
    //      get { return Data; }
    //   }
    //
    //   public T Data { get; set; }
    //}

    internal class ExpressionMetadata<T> : IMetadata<T>
    {
        private readonly PreferenceData _data;
        private readonly Func<PreferenceData, T> _getter;
        private readonly Action<PreferenceData, T> _setter;

        public ExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, T>> propertyExpression)
           : this(data, propertyExpression, false)
        {

        }

        public ExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, T>> propertyExpression, bool readOnly)
        {
            _data = data;
            _getter = propertyExpression.Compile();
            if (!readOnly)
            {
                _setter = propertyExpression.ToSetter();
            }
        }

        public Type DataType
        {
            get { return typeof(T); }
        }

        object IMetadata.Data
        {
            get { return Data; }
            set { Data = (T)value; }
        }

        public virtual T Data
        {
            get { return _getter(_data); }
            set
            {
                if (_setter == null)
                {
                    throw new InvalidOperationException("Data is read-only.");
                }
                _setter(_data, value);
            }
        }
    }

    internal class EncryptedExpressionMetadata : ExpressionMetadata<string>
    {
        public EncryptedExpressionMetadata(PreferenceData data, Expression<Func<PreferenceData, string>> propertyExpression)
           : base(data, propertyExpression)
        {

        }

        public override string Data
        {
            get { return Cryptography.DecryptValue(base.Data); }
            set { base.Data = Cryptography.EncryptValue(value); }
        }
    }
}
