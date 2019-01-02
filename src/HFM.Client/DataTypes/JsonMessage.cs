
using System;
using System.Globalization;
using System.Text;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Folding@Home client JSON message.
   /// </summary>
   public class JsonMessage : Message
   {
      internal JsonMessage()
      {
         Value = new StringBuilder();
      }

      /// <summary>
      /// Message value.
      /// </summary>
      public StringBuilder Value { get; }

      /// <summary>
      /// Gets a formatted string that represents the metadata of the message.
      /// </summary>
      /// <returns>A formatted string that represents the metadata of the message.</returns>
      public override string GetHeader()
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} - Length: {1}", base.GetHeader(), Value.Length);
      }

      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.AppendLine(GetHeader());
         sb.AppendLine(Value.ToString());
         sb.AppendLine();
         return sb.ToString();
      }
   }
}