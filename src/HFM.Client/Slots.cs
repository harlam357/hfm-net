
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class Slots : Dictionary<string, string>
   {
      public const string ID = "id";
      public const string Status = "status";
      public const string Description = "description";

      public Options Options { get; private set; }

      public static Slots Parse(string value)
      {
         var o = JObject.Parse(value);
         var slots = new Slots();
         foreach (var prop in o.Properties())
         {
            if (prop.Name.Equals("options"))
            {
               var optionsValue = prop.ToString();
               // have to strip off "options" portion of the JSON
               slots.Options = Options.Parse(optionsValue.Substring(optionsValue.IndexOf('{')));
            }
            else
            {
               slots.Add(prop.Name, (string)prop);
            }
         }
         return slots;
      }
   }
}
