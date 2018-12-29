
namespace HFM.Log
{
   /// <summary>
   /// Defines an error returned from a log line data parsing function.
   /// </summary>
   public class LogLineDataParserError
   {
      public LogLineDataParserError()
      {
         
      }

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