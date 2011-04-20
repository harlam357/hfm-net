
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace HFM.Client
{
   public class ClientAccess : IDisposable
   {
      private const int InternalBufferSize = 1024;
      private const int SocketBufferSize = 8196;
      private const int DefaultTimeoutLength = 2000;

      private TcpClient _tcpClient;
      private NetworkStream _stream;
      private readonly StringBuilder _readBuffer;

      #region TcpClient Properties

      public bool Connected
      {
         get { return _tcpClient.Client == null ? false : _tcpClient.Connected; }
      }

      public int ConnectTimeout { get; set; }

      public int SendTimeout { get; set; }

      public int SendBufferSize { get; set; }

      public int ReceiveTimeout { get; set; }

      public int ReceiveBufferSize { get; set; }

      #endregion

      #region Data Properties

      public Slots Slots { get; private set; }

      public Options Options { get; private set; }

      public Queue Queue { get; private set; }

      public string LogFile { get; set; }

      #endregion

      public ClientAccess()
      {
         ConnectTimeout = DefaultTimeoutLength;
         SendTimeout = DefaultTimeoutLength;
         SendBufferSize = SocketBufferSize;
         ReceiveTimeout = DefaultTimeoutLength;
         ReceiveBufferSize = SocketBufferSize;
         _tcpClient = CreateClient();
         _readBuffer = new StringBuilder();
      }

      private TcpClient CreateClient()
      {
         return new TcpClient
                {
                   SendTimeout = SendTimeout,
                   SendBufferSize = SendBufferSize,
                   ReceiveTimeout = ReceiveTimeout,
                   ReceiveBufferSize = ReceiveBufferSize
                };
      }

      public void Connect(string hostname, int port)
      {
         if (_tcpClient.Client == null)
         {
            _tcpClient = CreateClient();
         }

         IAsyncResult ar = _tcpClient.BeginConnect(hostname, port, null, null);
         System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
         try
         {
            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(ConnectTimeout), false))
            {
               _tcpClient.Close();
               throw new TimeoutException("Client connection has timed out.");
            }

            _tcpClient.EndConnect(ar);
         }
         finally
         {
            wh.Close();
         } 
      }

      public void SendCommand(string command)
      {
         if (!Connected) throw new InvalidOperationException("Client is not connected.");

         if (!command.EndsWith("\n"))
         {
            command += "\n";
         }
         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }
         var buffer = Encoding.ASCII.GetBytes(command);
         _stream.Write(buffer, 0, buffer.Length);
      }

      public void Update()
      {
         if (!Connected) throw new InvalidOperationException("Client is not connected.");

         if (_stream == null)
         {
            _stream = _tcpClient.GetStream();
         }
         var buffer = new byte[InternalBufferSize];

         //int bytesRead = _stream.Read(buffer, 0, buffer.Length);
         //while (bytesRead != 0)
         //{
         //   _readBuffer.Append(Encoding.ASCII.GetString(buffer));
         //   bytesRead = _stream.Read(buffer, 0, buffer.Length);
         //}

         while (_stream.DataAvailable)
         {
            _stream.Read(buffer, 0, buffer.Length);
            _readBuffer.Append(Encoding.ASCII.GetString(buffer));
         }
         ProcessBuffer();
      }

      private void ProcessBuffer()
      {
         string value = _readBuffer.ToString();
         JsonMessage json;
         while ((json = GetNextJsonValue(value)) != null)
         {
            ProcessJsonMessage(json);
            value = json.NextStartIndex < value.Length ? value.Substring(json.NextStartIndex) : String.Empty;
         }
         _readBuffer.Clear();
         _readBuffer.Append(value);
      }

      private void ProcessJsonMessage(JsonMessage json)
      {
         switch (json.Name)
         {
            case "slots":
               Slots = Slots.Parse(json.Value);
               break;
            case "options":
               Options = Options.Parse(json.Value);
               break;
            case "units":
               Queue = Queue.Parse(json.Value);
               break;
            case "log-restart":
               // set the platform specific new line character(s)
               string logFile = json.Value.Replace("\\" + "n", Environment.NewLine);
               LogFile = logFile;
               break;
         }
      }

      private static JsonMessage GetNextJsonValue(string value)
      {
         Debug.Assert(value != null);

         // find the header
         int messageIndex = value.IndexOf("PyON ");
         if (messageIndex < 0)
         {
            return null;
         }
         // "PyON " plus version number and another space - i.e. "PyON 1 "
         messageIndex += 7;

         int startIndex = value.IndexOf('\n', messageIndex);
         if (startIndex < 0) return null;
         
         // find the footer
         int endIndex = value.IndexOf("---\n", startIndex);
         if (endIndex < 0) return null;

         var jsonMessage = new JsonMessage();
         // get the PyON message name
         jsonMessage.Name = value.Substring(messageIndex, startIndex - messageIndex);

         // get the PyON message
         string pyon = value.Substring(startIndex, endIndex - startIndex);
         // replace PyON values with JSON values
         jsonMessage.Value = pyon.Replace("[\n", String.Empty).Replace("]\n", String.Empty).Replace(": None", ": null");
         // set the index so we know where to trim the string
         jsonMessage.NextStartIndex = endIndex + 4;
         return jsonMessage;
      }

      #region IDisposable Members

      private bool _disposed;

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
         if (!_disposed)
         {
            if (disposing)
            {
               _tcpClient.Close();
            }
         }

         _disposed = true;
      }

      ~ClientAccess()
      {
         Dispose(false);
      }

      #endregion
   }

   public class JsonMessage
   {
      public string Name { get; set; }

      public string Value { get; set; }

      public int NextStartIndex { get; set; }
   }
}
