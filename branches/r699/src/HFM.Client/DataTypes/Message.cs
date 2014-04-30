/*
 * HFM.NET - Message Data Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

using HFM.Client.Converters;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Provides the base functionality for creating a collection of Folding@Home typed client messages.
   /// </summary>
   [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
   public abstract class TypedMessageCollection : TypedMessage
   {
      internal abstract void Fill<T>(JsonMessage message) where T : ITypedMessageObject;
   }

   /// <summary>
   /// Provides the base functionality for creating Folding@Home typed client messages.
   /// </summary>
   public abstract class TypedMessage : Message, ITypedMessageObject
   {
      #region Constructor

      /// <summary>
      /// Initializes a new instance of the TypedMessage class.
      /// </summary>
      protected TypedMessage()
      {
         _errors = new List<MessagePropertyConversionError>();
      }

      #endregion

      internal abstract void Fill(JsonMessage message);

      #region ITypedMessageObject Members

      private readonly List<MessagePropertyConversionError> _errors;
      /// <summary>
      /// Collection of property type conversion errors.
      /// </summary>
      public IEnumerable<MessagePropertyConversionError> Errors
      {
         get { return _errors.AsReadOnly(); }
      }

      void ITypedMessageObject.AddError(MessagePropertyConversionError conversionError)
      {
         _errors.Add(conversionError);
      }

      #endregion
   }

   /// <summary>
   /// Folding@Home client JSON message.
   /// </summary>
   public class JsonMessage : Message
   {
      internal JsonMessage()
      {
         Value = new StringBuilder();
      }

      /// <summary>
      /// Message value.
      /// </summary>
      public StringBuilder Value { get; private set; }

      /// <summary>
      /// Gets a formatted string that represents the metadata of the message.
      /// </summary>
      /// <returns>A formatted string that represents the metadata of the message.</returns>
      public override string GetHeader()
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} - Length: {1}", base.GetHeader(), Value.Length);
      }

      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.AppendLine(GetHeader());
         sb.AppendLine(Value.ToString());
         sb.AppendLine();
         return sb.ToString();
      }
   }

   /// <summary>
   /// Provides the base functionality for creating Folding@Home client messages.
   /// </summary>
   public abstract class Message
   {
      /// <summary>
      /// Message key.
      /// </summary>
      public string Key { get; internal set; }

      /// <summary>
      /// Received time stamp.
      /// </summary>
      public DateTime Received { get; internal set; }

      internal void SetMessageValues(Message message)
      {
         Key = message.Key;
         Received = message.Received;
      }

      /// <summary>
      /// Gets a formatted string that represents the metadata of the message.
      /// </summary>
      /// <returns>A formatted string that represents the metadata of the message.</returns>
      public virtual string GetHeader()
      {
         return String.Format(CultureInfo.CurrentCulture, "Message Key: {0} - Received at: {1}", Key, Received);
      }

      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return GetHeader();
      }
   }

   /// <summary>
   /// Specifies that the property is populated by a Folding@Home client message property. This class cannot be inherited.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class MessagePropertyAttribute : Attribute
   {
      private readonly string[] _names;
      /// <summary>
      /// Gets the names of the Folding@Home client message property.
      /// </summary>
      public IEnumerable<string> Names
      {
         get { return _names; }
      }

      private readonly Type _converterType;
      /// <summary>
      /// Gets the type converter for this Folding@Home client message property.
      /// </summary>
      public Type ConverterType
      {
         get { return _converterType; }
      }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name">The message property name.</param>
      public MessagePropertyAttribute(string name)
      {
         _names = new[] { name };
      }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name1">The first possible message property name.</param>
      /// <param name="name2">The second possible message property name.</param>
      public MessagePropertyAttribute(string name1, string name2)
      {
         _names = new[] { name1, name2 };
      }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name">The message property name.</param>
      /// <param name="converterType">The type of the message property converter.</param>
      public MessagePropertyAttribute(string name, Type converterType)
      {
         _names = new[] { name };
         _converterType = converterType;
      }
   }

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

         var properties = GetProperties(jProperty.Name);
         if (properties.Count() == 0)
         {
            return;
         }

         if (jProperty.Value.Type.Equals(JTokenType.Object))
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
                  if (jProperty.Value.Type.Equals(JTokenType.String))
                  {
                     SetPropertyValue(classProperty, (string)jProperty);
                  }
                  else if (jProperty.Value.Type.Equals(JTokenType.Integer))
                  {
                     SetPropertyValue(classProperty, (int)jProperty);
                  }
               }
               catch (Exception ex)
               {
                  var typedMessageObject = _message as ITypedMessageObject;
                  if (typedMessageObject != null)
                  {
                     typedMessageObject.AddError(new MessagePropertyConversionError(classProperty.Name, ex.Message));
                  }
               }
            }
         }
      }

      internal void SetProperty(string key, string value)
      {
         if (key == null) return;

         var properties = GetProperties(key);
         if (properties.Count() == 0)
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
               var typedMessageObject = _message as ITypedMessageObject;
               if (typedMessageObject != null)
               {
                  typedMessageObject.AddError(new MessagePropertyConversionError(classProperty.Name, ex.Message));
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
         if (converterType != null && converterType.GetInterface("IConversionProvider") != null)
         {
            return (IConversionProvider)Activator.CreateInstance(converterType);
         }
         return null;
      }
   }

   /// <summary>
   /// Folding@Home message property conversion error. This class cannot be inherited.
   /// </summary>
   public sealed class MessagePropertyConversionError
   {
      private readonly string _propertyName;
      /// <summary>
      /// Gets the class property name the conversion failed to populate.
      /// </summary>
      public string PropertyName
      {
         get { return _propertyName; }
      }

      private readonly string _message;
      /// <summary>
      /// Gets the error message generated by the conversion failure.
      /// </summary>
      public string Message
      {
         get { return _message; }
      }

      internal MessagePropertyConversionError(string propertyName, string message)
      {
         _propertyName = propertyName;
         _message = message;
      }
   }

   /// <summary>
   /// Provides functionality to a typed message to add to and return a collection of error messages.
   /// </summary>
   public interface ITypedMessageObject
   {
      /// <summary>
      /// Collection of property type conversion errors.
      /// </summary>
      IEnumerable<MessagePropertyConversionError> Errors { get; }

      /// <summary>
      /// Add a message property conversion error.
      /// </summary>
      /// <param name="conversionError">The conversion error.</param>
      void AddError(MessagePropertyConversionError conversionError);
   }
}
