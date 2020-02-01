/*
 * harlam357.Net
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
using System.IO;
using System.Net;

namespace HFM.Core.Net
{
   /// <summary>
   /// Represents an object that provides a response from a Uniform Resource Identifier (URI).
   /// </summary>
   public interface IWebResponse
   {
      #region Properties

      /// <summary>
      /// Gets or sets the content length of data being received.
      /// </summary>
      long ContentLength { get; set; }

      /// <summary>
      /// Gets or sets the content type of the data being received.
      /// </summary>
      string ContentType { get; set; }

      /// <summary>
      /// Gets a collection of header name-value pairs associated with this request.
      /// </summary>
      WebHeaderCollection Headers { get; }

      /// <summary>
      /// Gets a System.Boolean value that indicates whether this response was obtained from the cache.
      /// </summary>
      bool IsFromCache { get; }

      /// <summary>
      /// Gets a System.Boolean value that indicates whether mutual authentication occurred.
      /// </summary>
      bool IsMutuallyAuthenticated { get; }

      /// <summary>
      /// Gets the URI of the Internet resource that actually responded to the request.
      /// </summary>
      Uri ResponseUri { get; }

      #endregion

      #region Methods

      /// <summary>
      /// Closes the response stream.
      /// </summary>
      void Close();

      /// <summary>
      /// Returns the data stream from the Internet resource.
      /// </summary>
      /// <returns>An instance of the System.IO.Stream class for reading data from the Internet resource.</returns>
      Stream GetResponseStream();

      #endregion
   }

   /// <summary>
   /// Wraps a System.Net.WebResponse object that provides a response from a Uniform Resource Identifier (URI).
   /// </summary>
   internal class WebResponseAdapter : IWebResponse
   {
      private readonly WebResponse _webResponse;

      /// <summary>
      /// Initializes a new instance of the WebResponseAdapter class.
      /// </summary>
      /// <param name="webResponse">The System.Net.WebResponse instance this object wraps.</param>
      public WebResponseAdapter(WebResponse webResponse)
      {
         _webResponse = webResponse;
      }

      #region Properties

      /// <summary>
      /// Gets or sets the content length of data being received.
      /// </summary>
      public long ContentLength
      {
         get { return _webResponse.ContentLength; }
         set { _webResponse.ContentLength = value; }
      }

      /// <summary>
      /// Gets or sets the content type of the data being received.
      /// </summary>
      public string ContentType
      {
         get { return _webResponse.ContentType; }
         set { _webResponse.ContentType = value; }
      }

      /// <summary>
      /// Gets a collection of header name-value pairs associated with this request.
      /// </summary>
      public WebHeaderCollection Headers
      {
         get { return _webResponse.Headers; }
      }

      /// <summary>
      /// Gets a System.Boolean value that indicates whether this response was obtained from the cache.
      /// </summary>
      public bool IsFromCache
      {
         get { return _webResponse.IsFromCache; }
      }

      /// <summary>
      /// Gets a System.Boolean value that indicates whether mutual authentication occurred.
      /// </summary>
      public bool IsMutuallyAuthenticated
      {
         get { return _webResponse.IsMutuallyAuthenticated; }
      }

      /// <summary>
      /// Gets the URI of the Internet resource that actually responded to the request.
      /// </summary>
      public Uri ResponseUri
      {
         get { return _webResponse.ResponseUri; }
      }

      #endregion

      #region Methods

      /// <summary>
      /// Closes the response stream.
      /// </summary>
      public void Close()
      {
         _webResponse.Close();
      }

      /// <summary>
      /// Returns the data stream from the Internet resource.
      /// </summary>
      /// <returns>An instance of the System.IO.Stream class for reading data from the Internet resource.</returns>
      public Stream GetResponseStream()
      {
         return _webResponse.GetResponseStream();
      }

      #endregion
   }
}