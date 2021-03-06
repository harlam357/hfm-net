﻿/*
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
    public abstract class WebOperation
    {
        private const int DefaultBufferSize = 1024;

        /// <summary>
        /// Occurs when the operation successfully transfers some data.
        /// </summary>
        public event EventHandler<WebOperationProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Gets or sets the inner WebRequest object.
        /// </summary>
        public WebRequest WebRequest { get; protected set; }

        private WebOperationState _state = WebOperationState.Idle;
        /// <summary>
        /// Gets the state of the operation.
        /// </summary>
        public WebOperationState State
        {
            get => _state;
            protected set
            {
                _state = value;
                if (_state == WebOperationState.Idle)
                {
                    _cancel = false;
                }
            }
        }

        /// <summary>
        /// Gets the result of the last operation.
        /// </summary>
        public WebOperationResult Result { get; protected set; } = WebOperationResult.None;

        /// <summary>
        /// Indicates whether the buffer will be automatically sized.
        /// </summary>
        public bool AutoSizeBuffer { get; set; } = true;

        /// <summary>
        /// Gets or sets the internal buffer size.
        /// </summary>
        public int Buffer { get; set; } = DefaultBufferSize;

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
            WebRequest.Method = GetWebDownloadMethod();

            long totalBytesRead = 0;
            long totalLength;

            WebResponse response = WebRequest.GetResponse();
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
            WebRequest.Method = GetWebDownloadMethod();

            OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, 0, State, Result));
            WebResponse response = WebRequest.GetResponse();
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
            using (Stream fileStream = File.OpenRead(fileName))
            {
                Upload(fileStream);
            }

            if (Result == WebOperationResult.Canceled)
            {
                if (WebRequest is FileWebRequest && File.Exists(WebRequest.RequestUri.LocalPath))
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
            WebRequest.Method = GetWebUploadMethod();

            long totalBytesRead = 0;
            long totalLength = stream.Length - stream.Position;

            if (AutoSizeBuffer)
            {
                Buffer = CalculateBufferSize(totalLength);
            }

            using (Stream requestStream = WebRequest.GetRequestStream())
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

            WebResponse response = WebRequest.GetResponse();
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
            WebRequest.Method = GetWebCheckConnectionMethod();

            WebResponse response = null;
            try
            {
                OnWebOperationProgress(new WebOperationProgressChangedEventArgs(0, 0, State, Result));
                response = WebRequest.GetResponse();
                Result = WebOperationResult.Completed;
            }
            finally
            {
                response?.Close();

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

            ProgressChanged?.Invoke(this, e);
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
            if (requestUriString == null) throw new ArgumentNullException(nameof(requestUriString));
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
            return CreateWebOperation(WebRequest.Create(requestUri));
        }

        private static WebOperation CreateWebOperation(WebRequest webRequest)
        {
            if (webRequest is FileWebRequest fileWebRequest)
            {
                return new FileWebOperation(fileWebRequest);
            }

            if (webRequest is HttpWebRequest httpWebRequest)
            {
                return new HttpWebOperation(httpWebRequest);
            }

            if (webRequest is FtpWebRequest ftpWebRequest)
            {
                return new FtpWebOperation(ftpWebRequest);
            }

            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Web Request Type '{0}' is not valid.", webRequest.GetType()));
        }

        private class FileWebOperation : WebOperation
        {
            public FileWebOperation(WebRequest webRequest)
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

        private class HttpWebOperation : WebOperation
        {
            public HttpWebOperation(WebRequest webRequest)
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
            public FtpWebOperation(FtpWebRequest webRequest)
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
        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Gets the total number of bytes to receive.
        /// </summary>
        public long TotalLength { get; }

        /// <summary>
        /// Indicates the current state of the operation.
        /// </summary>
        public WebOperationState State { get; }

        /// <summary>
        /// Indicates the result of the operation.
        /// </summary>
        public WebOperationResult Result { get; }

        /// <summary>
        /// Initializes a new instance of the WebOperationProgressChangedEventArgs class.
        /// </summary>
        /// <param name="length">The number of bytes received</param>
        /// <param name="totalLength">The total number of bytes to receive.</param>
        /// <param name="state">The current state of the operation.</param>
        /// <param name="result">The result of the operation.</param>
        public WebOperationProgressChangedEventArgs(long length, long totalLength, WebOperationState state, WebOperationResult result)
        {
            Length = length;
            TotalLength = totalLength;
            State = state;
            Result = result;
        }
    }
}
