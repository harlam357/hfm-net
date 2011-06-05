/*
 * HFM.NET - FahClient Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.ComponentModel;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class FahClient : Messages
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
