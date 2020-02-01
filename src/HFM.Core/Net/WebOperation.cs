/*
 * harlam357.Net - Web Operation Class
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;

namespace HFM.Core.Net
{
   /// <summary>
   /// Specifies the current state of the operation.
   /// </summary>
   public enum WebOperationState
   {
      /// <summary>
      /// No operation is running.
      /// </summary>
      Idle,
      /// <summary>
      /// An operation is in progress.
      /// </summary>
      InProgress,
   }
   
   /// <summary>
   /// Specifies the result of the operation.
   /// </summary>
   public enum WebOperationResult
   {
      /// <summary>
      /// No result.
      /// </summary>
      None,
      /// <summary>
      /// The operation completed.
      /// </summary>
      Completed,
      /// <summary>
      /// The operation was canceled.
      /// </summary>
      Canceled
   }

   /// <summary>
   /// Provides common methods for sending and receiving data using a WebRequest.
   /// </summary>
   public interface IWebOperation
   {
      /// <summary>
      /// Occurs when the operation successfully transfers some data.
      /// </summary>
      event EventHandler<WebOperationProgressChangedEventArgs> ProgressChanged;

      /// <summary>
      /// Gets the inner WebRequest object.
      /// </summary>
      IWebRequest WebRequest { get; }

      /// <summary>
      /// Gets the state of the operation.
      /// </summary>
      WebOperationState State { get; }

      /// <summary>
      /// Gets the result of the last operation.
      /// </summary>
      WebOperationResult Result { get; }

      /// <summary>
      /// Indicates whether the buffer will be automatically sized.
      /// </summary>
      bool AutoSizeBuffer { get; set; }

      /// <summary>
      /// Gets or sets the internal buffer size.
      /// </summary>
      int Buffer { get; set; }

      /// <summary>
      /// Downloads the resource to a local file.
      /// </summary>
      /// <param name="fileName">The name of the local file that is to receive the data.</param>
      void Download(string fileName);

      /// <summary>
      /// Downloads the resource to a stream.
      /// </summary>
      /// <param name="stream">The stream used to receive the data.</param>
      void Download(Stream stream);

      /// <summary>
      /// Gets the length of the data being received.
      /// </summary>
      /// <returns>The length of the data being received in bytes.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      long GetDownloadLength();

      /// <summary>
      /// Uploads the specified local file to the resource.
      /// </summary>
      /// <param name="fileName">The file to send to the resource.</param>
      void Upload(string fileName);

      /// <summary>
      /// Uploads the specified local file to the resource.
      /// </summary>
      /// <param name="fileName">The file to send to the resource.</param>
      /// <param name="maximumLength">The maximum number of bytes to upload.  If the value is 0 or less the entire file will be uploaded.</param>
      void Upload(string fileName, int maximumLength);

      /// <summary>
      /// Uploads the specified stream to the resource.
      /// </summary>
      /// <param name="stream">The stream to send to the resource.</param>
      void Upload(Stream stream);

      /// <summary>
      /// Returns a value indicating if a connection can be made to the resource.
      /// </summary>
      void CheckConnection();

      /// <summary>
      /// Cancels the operation.
      /// </summary>
      void Cancel();
   }

   /// <summary>
   /// Provides common methods for sending and receiving data using a WebRequest.
   /// </summary>
   public abstract class WebOperation : IWebOperation
   {
      private const int DefaultBufferSize = 1024;

      /// <summary>
      /// Occurs when the operation successfully transfers some data.
      /// </summary>
      public event EventHandler<WebOperationProgressChangedEventArgs> ProgressChanged;

      private IWebRequest _webRequest;
      /// <summary>
      /// Gets or sets the inner WebRequest object.
      /// </summary>
      public IWebRequest WebRequest
      {
         get { return _webRequest; }
         protected set { _webRequest = value; }
      }
      
      private WebOperationState _state = WebOperationState.Idle;
      /// <summary>
      /// Gets the state of the operation.
      /// </summary>
      public WebOperationState State
      {
         get { return _state; }
         protected set
         {
            _state = value;
            if (_state == WebOperationState.Idle)
            {
               _cancel = false;
            }
         }
      }

      private WebOperationResult _result = WebOperationResult.None;
      /// <summary>
      /// Gets the result of the last operation.
      /// </summary>
      public WebOperationResult Result
      {
         get { return _result; }
         protected set
         {
            _result = value;
         }
      }
      
      private bool _autoSizeBuffer = true;
      /// <summary>
      /// Indicates whether the buffer will be automatically sized.
      /// </summary>
      public bool AutoSizeBuffer
      {
         get { return _autoSizeBuffer; }
         set { _autoSizeBuffer = value; }
      }

      private int _buffer = DefaultBufferSize;
      /// <summary>
      /// Gets or sets the internal buffer size.
      /// </summary>
      public int Buffer
      {
         get { return _buffer; }
         set { _buffer = value; }
      }

      private bool _cancel;

      /// <summary>
      /// Downloads the resource to a local file.
      /// </summary>
      /// <param name="fileName">The name of the local file that is to receive the data.</param>
      public void Download(string fileName)
      {
         if (State != WebOperationState.Idle)
         {
            return;
         }

         // Create the directory if it does not exist
         string localPath = Path.GetDirectoryName(fileName);
         if (localPath != null && !Directory.Exists(localPath))
         {
            Directory.CreateDirectory(localPath);
         }

         using (Stream fileStream = File.Create(fileName))
         {
            Download(fileStream);
         }

         if (Result == WebOperationResult.Canceled)
         {
            File.Delete(fileName);
         }
      }

      /// <summary>
      /// Downloads the resource to a stream.
      /// </summary>
      /// <param name="stream">The stream used to receive the data.</param>
      public void Download(Stream stream)
      {
         if (State != WebOperationState.Idle)
         {
            return;
         }

         State = WebOperationState.InProgress;
         Result = WebOperationResult.None;
         _webRequest.Method = GetWebDownloadMethod();

         long totalBytesRead = 0;
         long totalLength;

         IWebResponse response = _webRequest.GetResponse();
         using (Stream responseStream = response.GetResponseStream())
         {
            totalLength = response.ContentLength;
            if (AutoSizeBuffer)
            {
               Buffer = CalculateBufferSize(totalLength);
            }

            var buffer = new byte[Buffer];
            int bytesRead = 0;

            do
            {
               if (_cancel)
               {
                  Result = WebOperationResult.Canceled;
                  break;
               }

               if (responseStream != null)
               {
                  bytesRead = responseStream.Read(buffer, 0, buffer.Length);
               }
               stream.Write(buffer, 0, bytesRead);

               totalBytesRead += bytesRead;
               OnWebOperationProgress(new WebOperationProgressChangedEventArgs(totalBytesRead, totalLength, State, Result));
            }
            while (bytesRead > 0);
         }

         if (Result != WebOperationResult.Canceled && (totalLength < 0 || totalBytesRead == totalLength))
         {
            Result = WebOperationResult.Completed;
         }
         
         // Close the Response Stream
         response.Close();

         State = WebOperationState.Idle;
         OnWebOperationProgress(new WebOperationProgressChangedEventArgs(totalBytesRead, totalLength, State, Result));
      }

      /// <summary>
      /// Gets the length of the data being received.
      /// </summary>
      /// <returns>The length of the data being received in bytes.</returns>
      public long GetDownloadLength()
      {
         if (State != WebOperationState.Idle)
         {
            return 0;
         }

         State = WebOperationState.InProgress;
         Result = WebOperationResult.None;
         _webRequest.Method = GetWebDownloadMethod();

         OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, 0, State, Result));
         IWebResponse response = _webRequest.GetResponse();
         long length = response.ContentLength;
         response.Close();

         State = WebOperationState.Idle;
         Result = WebOperationResult.Completed;
         OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, length, State, Result));

         return length;
      }

      /// <summary>
      /// Uploads the specified local file to the resource.
      /// </summary>
      /// <param name="fileName">The file to send to the resource.</param>
      public void Upload(string fileName)
      {
         Upload(fileName, -1);
      }

      /// <summary>
      /// Uploads the specified local file to the resource.
      /// </summary>
      /// <param name="fileName">The file to send to the resource.</param>
      /// <param name="maximumLength">The maximum number of bytes to upload.  If the value is 0 or less the entire file will be uploaded.</param>
      public void Upload(string fileName, int maximumLength)
      {
         using (Stream fileStream = File.OpenRead(fileName))
         {
            if (maximumLength >= 0 && fileStream.Length >= maximumLength)
            {
               fileStream.Position = fileStream.Length - maximumLength;
            }
            Upload(fileStream);
         }

         if (Result == WebOperationResult.Canceled)
         {
            if (WebRequest is FileWebRequestAdapter && File.Exists(WebRequest.RequestUri.LocalPath))
            {
               File.Delete(WebRequest.RequestUri.LocalPath);
            }
         }
      }

      /// <summary>
      /// Uploads the specified stream to the resource.
      /// </summary>
      /// <param name="stream">The stream to send to the resource.</param>
      public void Upload(Stream stream)
      {
         if (State != WebOperationState.Idle)
         {
            return;
         }

         State = WebOperationState.InProgress;
         Result = WebOperationResult.None;
         _webRequest.Method = GetWebUploadMethod();

         long totalBytesRead = 0;
         long totalLength = stream.Length - stream.Position;

         if (AutoSizeBuffer)
         {
            Buffer = CalculateBufferSize(totalLength);
         }

         using (Stream requestStream = _webRequest.GetRequestStream())
         {
            var buffer = new byte[Buffer];
            int bytesRead;

            do
            {
               if (_cancel)
               {
                  Result = WebOperationResult.Canceled;
                  break;
               }

               bytesRead = stream.Read(buffer, 0, buffer.Length);
               requestStream.Write(buffer, 0, bytesRead);

               totalBytesRead += bytesRead;
               OnWebOperationProgress(new WebOperationProgressChangedEventArgs(totalBytesRead, totalLength, State, Result));

            } while (bytesRead > 0);
         }

         if (Result != WebOperationResult.Canceled && totalBytesRead == totalLength)
         {
            Result = WebOperationResult.Completed;
         }

         IWebResponse response = _webRequest.GetResponse();
         response.Close();

         State = WebOperationState.Idle;
         OnWebOperationProgress(new WebOperationProgressChangedEventArgs(totalBytesRead, totalLength, State, Result));
      }

      /// <summary>
      /// Returns a value indicating if a connection can be made to the resource.
      /// </summary>
      public void CheckConnection()
      {
         if (State != WebOperationState.Idle)
         {
            return;
         }

         State = WebOperationState.InProgress;
         Result = WebOperationResult.None;
         _webRequest.Method = GetWebCheckConnectionMethod();

         IWebResponse response = null;
         try
         {
            OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, 0, State, Result));
            response = _webRequest.GetResponse();
            Result = WebOperationResult.Completed;
         }
         finally
         {
            if (response != null)
            {
               response.Close();
            }

            State = WebOperationState.Idle;
            OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, 0, State, Result));
         }
      }

      /// <summary>
      /// Cancels the operation.
      /// </summary>
      public void Cancel()
      {
         if (State == WebOperationState.InProgress)
         {
            _cancel = true;
         }
      }

      /// <summary>
      /// Calculates a buffer size approximately 0.5% the size of the stream length.
      /// </summary>
      /// <param name="streamLength">The length of the stream.</param>
      /// <returns>The buffer size in bytes.</returns>
      private static int CalculateBufferSize(long streamLength)
      {
         long autoBuffer = streamLength / 200;
         if (autoBuffer <= Int32.MaxValue && autoBuffer > DefaultBufferSize)
         {
            return (int)autoBuffer;
         }
         
         return DefaultBufferSize;
      }

      /// <summary>
      /// Raises the WebOperationProgress event.
      /// </summary>
      /// <param name="e">A WebOperationProgressEventArgs object containing event data.  If e is null the event will be canceled.</param>
      private void OnWebOperationProgress(WebOperationProgressChangedEventArgs e)
      {
         Debug.Assert(e != null);

         var handler = ProgressChanged;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      /// <summary>
      /// When overridden in a descendant class, returns a string specifying the download method name.
      /// </summary>
      /// <returns>A string specifying the download method name.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      protected abstract string GetWebDownloadMethod();

      /// <summary>
      /// When overridden in a descendant class, returns a string specifying the upload method name.
      /// </summary>
      /// <returns>A string specifying the upload method name.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      protected abstract string GetWebUploadMethod();

      /// <summary>
      /// When overridden in a descendant class, returns a string specifying the check connection method name.
      /// </summary>
      /// <returns>A string specifying the check connection method name.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      protected abstract string GetWebCheckConnectionMethod();

      /// <summary>
      /// Initializes a new WebOperation instance for the specified URI scheme.
      /// </summary>
      /// <param name="requestUriString">The URI that identifies the Internet resource.</param>
      /// <returns>A WebOperation descendant for the specific URI scheme.</returns>
      public static WebOperation Create(string requestUriString)
      {
         if (requestUriString == null) throw new ArgumentNullException("requestUriString");
         if (requestUriString.Contains("://") == false)
         {
            requestUriString = "file://" + requestUriString;
         }
         return Create(new Uri(requestUriString));
      }

      /// <summary>
      /// Initializes a new WebOperation instance for the specified URI scheme.
      /// </summary>
      /// <param name="requestUri">A System.Uri containing the URI of the requested resource.</param>
      /// <returns>A WebOperation descendant for the specific URI scheme.</returns>
      public static WebOperation Create(Uri requestUri)
      {
         return CreateWebOperation(System.Net.WebRequest.Create(requestUri));
      }

      private static WebOperation CreateWebOperation(WebRequest webRequest)
      {
         var fileWebRequest = webRequest as FileWebRequest;
         if (fileWebRequest != null)
         {
            return new FileWebOperation(new FileWebRequestAdapter(fileWebRequest));
         }

         var httpWebRequest = webRequest as HttpWebRequest;
         if (httpWebRequest != null)
         {
            return new HttpWebOperation(new WebRequestAdapter(httpWebRequest));
         }

         var ftpWebRequest = webRequest as FtpWebRequest;
         if (ftpWebRequest != null)
         {
            return new FtpWebOperation(new FtpWebRequestAdapter(ftpWebRequest));
         }

         throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
            "Web Request Type '{0}' is not valid.", webRequest.GetType()));
      }

      private class FileWebOperation : WebOperation
      {
         public FileWebOperation(IWebRequest webRequest)
         {
            WebRequest = webRequest;
         }

         protected override string GetWebDownloadMethod()
         {
            return WebRequestMethods.File.DownloadFile;
         }

         protected override string GetWebUploadMethod()
         {
            return WebRequestMethods.File.UploadFile;
         }

         protected override string GetWebCheckConnectionMethod()
         {
            return WebRequestMethods.File.DownloadFile;
         }
      }

      /// <summary>
      /// Represents an object that makes a request to a file system Uniform Resource Identifier (URI).
      /// </summary>
      private class FileWebRequestAdapter : WebRequestAdapter
      {
         /// <summary>
         /// Initializes a new instance of the FileWebRequestAdapter class.
         /// </summary>
         /// <param name="fileWebRequest">The System.Net.WebRequest instance this object wraps.</param>
         public FileWebRequestAdapter(FileWebRequest fileWebRequest)
            : base(fileWebRequest)
         {

         }
      }

      private class HttpWebOperation : WebOperation
      {
         public HttpWebOperation(IWebRequest webRequest)
         {
            WebRequest = webRequest;
         }

         protected override string GetWebDownloadMethod()
         {
            return WebRequestMethods.Http.Get;
         }

         protected override string GetWebUploadMethod()
         {
            return WebRequestMethods.Http.Post;
         }

         protected override string GetWebCheckConnectionMethod()
         {
            return WebRequestMethods.Http.Head;
         }
      }

      private class FtpWebOperation : WebOperation
      {
         public FtpWebOperation(IFtpWebRequest webRequest)
         {
            WebRequest = webRequest;
         }

         protected override string GetWebDownloadMethod()
         {
            return WebRequestMethods.Ftp.DownloadFile;
         }

         protected override string GetWebUploadMethod()
         {
            return WebRequestMethods.Ftp.UploadFile;
         }

         protected override string GetWebCheckConnectionMethod()
         {
            return WebRequestMethods.Ftp.ListDirectory;
         }
      }
   }

   /// <summary>
   /// Provides data for the WebOperation.ProgressChanged event of a WebOperation.
   /// </summary>
   public class WebOperationProgressChangedEventArgs : EventArgs
   {
      private readonly long _length;
      /// <summary>
      /// Gets the number of bytes received.
      /// </summary>
      public long Length
      {
         get { return _length; }
      }

      private readonly long _totalLength;
      /// <summary>
      /// Gets the total number of bytes to receive.
      /// </summary>
      public long TotalLength
      {
         get { return _totalLength; }
      }

      private readonly WebOperationState _state;
      /// <summary>
      /// Indicates the current state of the operation.
      /// </summary>
      public WebOperationState State
      {
         get { return _state; }
      }

      private readonly WebOperationResult _result;
      /// <summary>
      /// Indicates the result of the operation.
      /// </summary>
      public WebOperationResult Result
      {
         get { return _result; }
      }

      /// <summary>
      /// Initializes a new instance of the WebOperationProgressChangedEventArgs class.
      /// </summary>
      /// <param name="length">The number of bytes received</param>
      /// <param name="totalLength">The total number of bytes to receive.</param>
      /// <param name="state">The current state of the operation.</param>
      /// <param name="result">The result of the operation.</param>
      public WebOperationProgressChangedEventArgs(long length, long totalLength, WebOperationState state, WebOperationResult result)
      {
         _length = length;
         _totalLength = totalLength;
         _state = state;
         _result = result;
      }
   }
}
