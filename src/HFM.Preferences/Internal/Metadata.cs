/*
 * Based on code by Bryan Watts
 * http://stackoverflow.com/questions/353126/c-multiple-generic-types-in-one-list/1351071
 */

using System;
using System.Linq.Expressions;
using System.Reflection;

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
            DefaultPreferenceDataPropertyIfNull(data, propertyExpression);
            _getter = propertyExpression.Compile();
            if (!readOnly)
            {
                _setter = propertyExpression.ToSetter();
            }
        }

        private static void DefaultPreferenceDataPropertyIfNull(PreferenceData data, LambdaExpression propertyExpression)
        {
            var pi = GetPreferenceDataPropertyInfo(propertyExpression);
            if (pi != null && pi.PropertyType != typeof(string))
            {
                var value = pi.GetValue(data);
                if (value is null)
                {
                    pi.SetValue(data, Activator.CreateInstance(pi.PropertyType));
                }
            }
        }

        private static PropertyInfo GetPreferenceDataPropertyInfo(LambdaExpression propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                while (memberExpression != null)
                {
                    if (memberExpression.Member is PropertyInfo pi && pi.DeclaringType == typeof(PreferenceData))
                    {
                        return pi;
                    }
                    memberExpression = memberExpression.Expression as MemberExpression;
                }
            }
            return null;
        }

        public Type DataType => typeof(T);

        object IMetadata.Data
        {
            get => Data;
            set => Data = (T)value;
        }

        public virtual T Data
        {
            get => _getter(_data);
            set
            {
                if (_setter == null)
                {
                    throw new InvalidOperationException("Preference is read-only.");
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
            get => Cryptography.DecryptValue(base.Data);
            set => base.Data = Cryptography.EncryptValue(value);
        }
    }
}
