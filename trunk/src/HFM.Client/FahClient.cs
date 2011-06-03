
using System;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class FahClient
   {
      public static void SetObjectProperty(object component, PropertyDescriptorCollection componentProperties, JProperty jsonProperty)
      {
         foreach (PropertyDescriptor optionsProperty in componentProperties)
         {
            var messageProperty = (MessagePropertyAttribute)optionsProperty.Attributes[typeof(MessagePropertyAttribute)];
            if (messageProperty != null)
            {
               if (messageProperty.Name == jsonProperty.Name)
               {
                  optionsProperty.SetValue(component, Convert.ChangeType((string)jsonProperty, optionsProperty.PropertyType));
                  break;
               }
            }
         }
      }
   }
}
