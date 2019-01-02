using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using HFM.Client.Converters;

using Newtonsoft.Json.Linq;

namespace HFM.Client.DataTypes
{
   internal class MessagePropertySetter
   {
      private readonly object _message;
      private readonly PropertyDescriptorCollection _properties;

      internal MessagePropertySetter(object message)
      {
         _message = message;
         _properties = TypeDescriptor.GetProperties(message);
      }

      internal void SetProperty(JProperty jProperty)
      {
         if (jProperty == null) return;

         var properties = GetProperties(jProperty.Name).ToList();
         if (!properties.Any())
         {
            return;
         }

         if (jProperty.Value.Type == JTokenType.Object)
         {
            var propertyValue = GetPropertyValue(jProperty.Name);
            if (propertyValue != null)
            {
               var propertySetter = new MessagePropertySetter(propertyValue);
               foreach (var prop in JObject.Parse(jProperty.Value.ToString()).Properties())
               {
                  propertySetter.SetProperty(prop);
               }
            }
         }
         else
         {
            foreach (var classProperty in properties)
            {
               try
               {
                  if (jProperty.Value.Type == JTokenType.String)
                  {
                     SetPropertyValue(classProperty, (string)jProperty);
                  }
                  else if (jProperty.Value.Type == JTokenType.Integer)
                  {
                     SetPropertyValue(classProperty, (int)jProperty);
                  }
               }
               catch (Exception ex)
               {
                  if (_message is TypedMessageBase typedMessage)
                  {
                     typedMessage.AddError(new TypedMessagePropertyError(classProperty.Name, ex.Message));
                  }
               }
            }
         }
      }

      internal void SetProperty(string key, string value)
      {
         if (key == null) return;

         var properties = GetProperties(key).ToList();
         if (!properties.Any())
         {
            return;
         }

         foreach (var classProperty in properties)
         {
            try
            {
               SetPropertyValue(classProperty, value);
            }
            catch (Exception ex)
            {
               if (_message is TypedMessageBase typedMessage)
               {
                  typedMessage.AddError(new TypedMessagePropertyError(classProperty.Name, ex.Message));
               }
            }
         }
      }

      private void SetPropertyValue(PropertyDescriptor classProperty, object value)
      {
         IConversionProvider conversionProvider = GetConversionProvider(classProperty);
         if (conversionProvider != null)
         {
            classProperty.SetValue(_message, conversionProvider.Convert(value));
         }
         else
         {
            Type propertyType = classProperty.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
               propertyType = propertyType.GetGenericArguments().First();
            }
            // ReSharper disable AssignNullToNotNullAttribute
            classProperty.SetValue(_message, Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture));
            // ReSharper restore AssignNullToNotNullAttribute
         }
      }

      internal object GetPropertyValue(string key)
      {
         var classProperty = GetProperties(key).FirstOrDefault();
         return classProperty == null ? null : classProperty.GetValue(_message);
      }

      private IEnumerable<PropertyDescriptor> GetProperties(string key)
      {
         return (from PropertyDescriptor classProperty in _properties
                 let messageProperty = (MessagePropertyAttribute)classProperty.Attributes[typeof(MessagePropertyAttribute)]
                 where messageProperty != null
                 where messageProperty.Names.Contains(key)
                 select classProperty);
      }

      private static IConversionProvider GetConversionProvider(MemberDescriptor classProperty)
      {
         Type converterType = ((MessagePropertyAttribute)classProperty.Attributes[typeof(MessagePropertyAttribute)]).ConverterType;
         if (converterType != null && converterType.GetInterface(nameof(IConversionProvider)) != null)
         {
            return (IConversionProvider)Activator.CreateInstance(converterType);
         }
         return null;
      }
   }
}