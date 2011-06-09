
using System;
using System.ComponentModel;

using Newtonsoft.Json.Linq;

namespace HFM.Client.DataTypes
{
   public abstract class TypedMessage : Message
   {
      
   }

   public class JsonMessage : Message
   {
      internal JsonMessage()
      {
         
      }

      /// <summary>
      /// Message Value
      /// </summary>
      public string Value { get; set; }
   }

   public abstract class Message
   {
      /// <summary>
      /// Message Key
      /// </summary>
      public string Key { get; set; }

      /// <summary>
      /// Received Time Stamp
      /// </summary>
      public DateTime Received { get; set; }

      internal void SetMessageValues(Message message)
      {
         Key = message.Key;
         Received = message.Received;
      }
   }

   public sealed class MessagePropertyAttribute : Attribute
   {
      private readonly string _name;

      public string Name
      {
         get { return _name; }
      }

      public MessagePropertyAttribute(string name)
      {
         _name = name;
      }
   }

   internal static class MessagePropertySetter
   {
      internal static void SetJProperty(object component, PropertyDescriptorCollection componentProperties, JProperty jsonProperty)
      {
         foreach (PropertyDescriptor optionsProperty in componentProperties)
         {
            var messageProperty = (MessagePropertyAttribute)optionsProperty.Attributes[typeof(MessagePropertyAttribute)];
            if (messageProperty != null)
            {
               if (messageProperty.Name == jsonProperty.Name)
               {
                  if (jsonProperty.Value.Type.Equals(JTokenType.String))
                  {
                     optionsProperty.SetValue(component, Convert.ChangeType((string)jsonProperty, optionsProperty.PropertyType));
                  }
                  else if (jsonProperty.Value.Type.Equals(JTokenType.Integer))
                  {
                     optionsProperty.SetValue(component, (int)jsonProperty);
                  }
                  break;
               }
            }
         }
      }
   }
}
