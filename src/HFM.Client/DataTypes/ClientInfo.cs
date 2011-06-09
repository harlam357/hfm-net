
using System;

using Newtonsoft.Json.Linq;

namespace HFM.Client.DataTypes
{
   public class ClientInfo : TypedMessage
   {
      public static ClientInfo Parse(JsonMessage message)
      {
         var jsonArray = JArray.Parse(message.Value);
         var clientInfo = new ClientInfo();
         foreach (var token in jsonArray)
         {
            if (!token.HasValues)
            {
               continue;
            }

            foreach (var token2 in token)
            {
               if (!token2.HasValues)
               {
                  continue;
               }

               string value = (string)token2[1];
            }

            //var slot = new Slot();
            //foreach (var prop in JObject.Parse(token.ToString()).Properties())
            //{
            //   if (prop.Name.Equals("options"))
            //   {
            //      var optionsValue = prop.ToString();
            //      // have to strip off "options" portion of the JSON
            //      slot.Options = Options.Parse(optionsValue.Substring(optionsValue.IndexOf('{')), message);
            //   }
            //   else
            //   {
            //      FahClient.SetObjectProperty(slot, TypeDescriptor.GetProperties(slot), prop);
            //   }
            //}
            //clientInfo.Add(slot);
         }
         clientInfo.SetMessageValues(message);
         return clientInfo;
      }      
   }
}
