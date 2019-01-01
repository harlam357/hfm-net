
namespace HFM.Log
{
   /// <summary>
   /// Defines an error returned from a log line data parsing function.
   /// </summary>
   public class LogLineDataParserError
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="LogLineDataParserError"/> class.
      /// </summary>
      public LogLineDataParserError()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="LogLineDataParserError"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
      public LogLineDataParserError(LogLineDataParserError other)
      {
         if (other == null) return;

         Message = other.Message;
      }

      /// <summary>
      /// Gets or sets the error message.
      /// </summary>
      public string Message { get; set; }

      /// <summary>
      /// Returns a string that represents the current <see cref="LogLineDataParserError"/> object.
      /// </summary>
      /// <returns>A string that represents the current <see cref="LogLineDataParserError"/> object.</returns>
      public override string ToString()
      {
         return Message;
      }
   }
}