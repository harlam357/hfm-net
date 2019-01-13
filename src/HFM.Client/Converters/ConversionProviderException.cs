
using System;

namespace HFM.Client.Converters
{
   public class ConversionProviderException : Exception
   {
      public ConversionProviderException()
      {

      }

      public ConversionProviderException(string message) : base(message)
      {

      }


      public ConversionProviderException(string message, Exception inner) : base(message, inner)
      {

      }

      protected ConversionProviderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
      {

      }
   }
}
