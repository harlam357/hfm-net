
using System;
using System.Diagnostics;
using System.Globalization;

namespace HFM.Client.Tool
{
   public class DebugTypedMessageConnection : TypedMessageConnection
   {
      /// <summary>
      /// Gets or sets the debug flag on the receive buffer.  When true the receive buffer is written to a log file specified by the value of the DebugBufferFileName property.
      /// </summary>
      public bool DebugReceiveBuffer { get; set; }

      /// <summary>
      /// Gets or sets the debug file name value.  When the DebugReceiveBuffer property is true the receive buffer is written to a log file specified by this file name value.
      /// </summary>
      public string DebugBufferFileName { get; set; }

      protected override void ProcessData(string buffer, int totalBytesRead)
      {
         base.ProcessData(buffer, totalBytesRead);
         if (DebugReceiveBuffer && !String.IsNullOrEmpty(DebugBufferFileName))
         {
            try
            {
               System.IO.File.AppendAllText(DebugBufferFileName,
                  buffer.Replace("\n", Environment.NewLine).Replace("\\n", Environment.NewLine));
            }
            catch (Exception ex)
            {
               OnStatusMessage(new StatusMessageEventArgs(String.Format(CultureInfo.CurrentCulture,
                  "Debug buffer write failed: {0}", ex.Message)));
            }
         }
      }
   }
}
