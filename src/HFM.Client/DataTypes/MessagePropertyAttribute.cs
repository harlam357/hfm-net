
using System;
using System.Collections.Generic;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Specifies that the property is populated by a Folding@Home client message property. This class cannot be inherited.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property)]
   public sealed class MessagePropertyAttribute : Attribute
   {
      /// <summary>
      /// Gets the names of the Folding@Home client message property.
      /// </summary>
      public ICollection<string> Names { get; }

      /// <summary>
      /// Gets the type converter for this Folding@Home client message property.
      /// </summary>
      public Type ConverterType { get; }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name">The message property name.</param>
      public MessagePropertyAttribute(string name)
      {
         Names = new[] { name };
      }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name1">The first possible message property name.</param>
      /// <param name="name2">The second possible message property name.</param>
      public MessagePropertyAttribute(string name1, string name2)
      {
         Names = new[] { name1, name2 };
      }

      /// <summary>
      /// Initializes a new instance of the MessagePropertyAttribute class.
      /// </summary>
      /// <param name="name">The message property name.</param>
      /// <param name="converterType">The type of the message property converter.</param>
      public MessagePropertyAttribute(string name, Type converterType)
      {
         Names = new[] { name };
         ConverterType = converterType;
      }
   }
}