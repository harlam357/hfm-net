
using System;

namespace HFM.Client
{
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
}
