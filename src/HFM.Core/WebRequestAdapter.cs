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
using System.Net.Cache;
using System.Net.Security;
using System.Security.Principal;

namespace HFM.Core
{
   /// <summary>
   /// Represents an object that makes a request to a Uniform Resource Identifier (URI).
   /// </summary>
   public interface IWebRequest
   {
      #region Properties

      /// <summary>
      /// Gets or sets values indicating the level of authentication and impersonation used for this request.
      /// </summary>
      AuthenticationLevel AuthenticationLevel { get; set; }

      /// <summary>
      /// Gets or sets the cache policy for this request.
      /// </summary>
      RequestCachePolicy CachePolicy { get; set; }

      /// <summary>
      /// Gets or sets the name of the connection group for the request.
      /// </summary>
      string ConnectionGroupName { get; set; }

      /// <summary>
      /// Gets or sets the content length of the request data being sent.
      /// </summary>
      long ContentLength { get; set; }

      /// <summary>
      /// Gets or sets the content type of the request data being sent.
      /// </summary>
      string ContentType { get; set; }

      /// <summary>
      /// Gets or sets the network credentials used for authenticating the request with the Internet resource.
      /// </summary>
      ICredentials Credentials { get; set; }

      /// <summary>
      /// Gets or sets the collection of header name/value pairs associated with the request.
      /// </summary>
      WebHeaderCollection Headers { get; set; }

      /// <summary>
      /// Gets or sets the impersonation level for the current request.
      /// </summary>
      TokenImpersonationLevel ImpersonationLevel { get; set; }

      /// <summary>
      /// Gets or sets the protocol method to use in this request.
      /// </summary>
      string Method { get; set; }

      /// <summary>
      /// Indicates whether to pre-authenticate the request.
      /// </summary>
      bool PreAuthenticate { get; set; }

      /// <summary>
      /// Gets or sets the network proxy to use to access this Internet resource.
      /// </summary>
      IWebProxy Proxy { get; set; }

      /// <summary>
      /// Gets the URI of the Internet resource associated with the request.
      /// </summary>
      Uri RequestUri { get; }

      /// <summary>
      /// Gets or sets the length of time, in milliseconds, before the request times out.
      /// </summary>
      int Timeout { get; set; }

      /// <summary>
      /// Gets or sets a System.Boolean value that controls whether System.Net.CredentialCache.DefaultCredentials are sent with requests.
      /// </summary>
      bool UseDefaultCredentials { get; set; }

      #endregion

      #region Methods

      /// <summary>
      /// Aborts the Request.
      /// </summary>
      void Abort();

      /// <summary>
      /// Provides an asynchronous version of the System.Net.WebRequest.GetRequestStream() method.
      /// </summary>
      /// <param name="callback">The System.AsyncCallback delegate.</param>
      /// <param name="state">An object containing state information for this asynchronous request.</param>
      /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
      IAsyncResult BeginGetRequestStream(AsyncCallback callback, Object state);

      /// <summary>
      /// Returns a System.IO.Stream for writing data to the Internet resource.
      /// </summary>
      /// <param name="asyncResult">An System.IAsyncResult that references a pending request for a stream.</param>
      /// <returns>A System.IO.Stream to write data to.</returns>
      Stream EndGetRequestStream(IAsyncResult asyncResult);

      /// <summary>
      /// Begins an asynchronous request for an Internet resource.
      /// </summary>
      /// <param name="callback">The System.AsyncCallback delegate.</param>
      /// <param name="state">An object containing state information for this asynchronous request.</param>
      /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
      IAsyncResult BeginGetResponse(AsyncCallback callback, Object state);

      /// <summary>
      /// Returns an IWebResponse.
      /// </summary>
      /// <param name="asyncResult">An System.IAsyncResult that references a pending request for a response.</param>
      /// <returns>An IWebResponse that contains a response to the Internet request.</returns>
      IWebResponse EndGetResponse(IAsyncResult asyncResult);

      /// <summary>
      /// Returns a System.IO.Stream for writing data to the Internet resource.
      /// </summary>
      /// <returns>A System.IO.Stream for writing data to the Internet resource.</returns>
      Stream GetRequestStream();

      /// <summary>
      /// Returns a response to an Internet request.
      /// </summary>
      /// <returns>An IWebResponse containing the response to the Internet request.</returns>
      IWebResponse GetResponse();

      #endregion
   }

   /// <summary>
   /// Wraps a System.Net.WebRequest object that makes a request to a Uniform Resource Identifier (URI).
   /// </summary>
   internal class WebRequestAdapter : IWebRequest
   {
      private readonly WebRequest _webRequest;

      /// <summary>
      /// Initializes a new instance of the WebRequestAdapter class.
      /// </summary>
      /// <param name="webRequest">The System.Net.WebRequest instance this object wraps.</param>
      public WebRequestAdapter(WebRequest webRequest)
      {
         _webRequest = webRequest;
      }

      #region Properties

      /// <summary>
      /// Gets or sets values indicating the level of authentication and impersonation used for this request.
      /// </summary>
      public AuthenticationLevel AuthenticationLevel
      {
         get { return _webRequest.AuthenticationLevel; }
         set { _webRequest.AuthenticationLevel = value; }
      }

      /// <summary>
      /// Gets or sets the cache policy for this request.
      /// </summary>
      public RequestCachePolicy CachePolicy
      {
         get { return _webRequest.CachePolicy; }
         set { _webRequest.CachePolicy = value; }
      }

      /// <summary>
      /// Gets or sets the name of the connection group for the request.
      /// </summary>
      public string ConnectionGroupName
      {
         get { return _webRequest.ConnectionGroupName; }
         set { _webRequest.ConnectionGroupName = value; }
      }

      /// <summary>
      /// Gets or sets the content length of the request data being sent.
      /// </summary>
      public long ContentLength
      {
         get { return _webRequest.ContentLength; }
         set { _webRequest.ContentLength = value; }
      }

      /// <summary>
      /// Gets or sets the content type of the request data being sent.
      /// </summary>
      public string ContentType
      {
         get { return _webRequest.ContentType; }
         set { _webRequest.ContentType = value; }
      }

      /// <summary>
      /// Gets or sets the network credentials used for authenticating the request with the Internet resource.
      /// </summary>
      public ICredentials Credentials
      {
         get { return _webRequest.Credentials; }
         set { _webRequest.Credentials = value; }
      }

      /// <summary>
      /// Gets or sets the collection of header name/value pairs associated with the request.
      /// </summary>
      public WebHeaderCollection Headers
      {
         get { return _webRequest.Headers; }
         set { _webRequest.Headers = value; }
      }

      /// <summary>
      /// Gets or sets the impersonation level for the current request.
      /// </summary>
      public TokenImpersonationLevel ImpersonationLevel
      {
         get { return _webRequest.ImpersonationLevel; }
         set { _webRequest.ImpersonationLevel = value; }
      }

      /// <summary>
      /// Gets or sets the protocol method to use in this request.
      /// </summary>
      public string Method
      {
         get { return _webRequest.Method; }
         set { _webRequest.Method = value; }
      }

      /// <summary>
      /// Indicates whether to pre-authenticate the request.
      /// </summary>
      public bool PreAuthenticate
      {
         get { return _webRequest.PreAuthenticate; }
         set { _webRequest.PreAuthenticate = value; }
      }

      /// <summary>
      /// Gets or sets the network proxy to use to access this Internet resource.
      /// </summary>
      public IWebProxy Proxy
      {
         get { return _webRequest.Proxy; }
         set { _webRequest.Proxy = value; }
      }

      /// <summary>
      /// Gets the URI of the Internet resource associated with the request.
      /// </summary>
      public Uri RequestUri
      {
         get { return _webRequest.RequestUri; }
      }

      /// <summary>
      /// Gets or sets the length of time, in milliseconds, before the request times out.
      /// </summary>
      public int Timeout
      {
         get { return _webRequest.Timeout; }
         set { _webRequest.Timeout = value; }
      }

      /// <summary>
      /// Gets or sets a System.Boolean value that controls whether System.Net.CredentialCache.DefaultCredentials are sent with requests.
      /// </summary>
      public bool UseDefaultCredentials
      {
         get { return _webRequest.UseDefaultCredentials; }
         set { _webRequest.UseDefaultCredentials = value; }
      }

      #endregion

      #region Methods

      /// <summary>
      /// Aborts the Request.
      /// </summary>
      public void Abort()
      {
         _webRequest.Abort();
      }

      /// <summary>
      /// Provides an asynchronous version of the System.Net.WebRequest.GetRequestStream() method.
      /// </summary>
      /// <param name="callback">The System.AsyncCallback delegate.</param>
      /// <param name="state">An object containing state information for this asynchronous request.</param>
      /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
      public IAsyncResult BeginGetRequestStream(AsyncCallback callback, Object state)
      {
         return _webRequest.BeginGetRequestStream(callback, state);
      }

      /// <summary>
      /// Returns a System.IO.Stream for writing data to the Internet resource.
      /// </summary>
      /// <param name="asyncResult">An System.IAsyncResult that references a pending request for a stream.</param>
      /// <returns>A System.IO.Stream to write data to.</returns>
      public Stream EndGetRequestStream(IAsyncResult asyncResult)
      {
         return _webRequest.EndGetRequestStream(asyncResult);
      }

      /// <summary>
      /// Begins an asynchronous request for an Internet resource.
      /// </summary>
      /// <param name="callback">The System.AsyncCallback delegate.</param>
      /// <param name="state">An object containing state information for this asynchronous request.</param>
      /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
      public IAsyncResult BeginGetResponse(AsyncCallback callback, Object state)
      {
         return _webRequest.BeginGetResponse(callback, state);
      }

      /// <summary>
      /// Returns an IWebResponse.
      /// </summary>
      /// <param name="asyncResult">An System.IAsyncResult that references a pending request for a response.</param>
      /// <returns>An IWebResponse that contains a response to the Internet request.</returns>
      public virtual IWebResponse EndGetResponse(IAsyncResult asyncResult)
      {
         return new WebResponseAdapter(_webRequest.EndGetResponse(asyncResult));
      }

      /// <summary>
      /// Returns a System.IO.Stream for writing data to the Internet resource.
      /// </summary>
      /// <returns>A System.IO.Stream for writing data to the Internet resource.</returns>
      public Stream GetRequestStream()
      {
         return _webRequest.GetRequestStream();
      }

      /// <summary>
      /// Returns a response to an Internet request.
      /// </summary>
      /// <returns>An IWebResponse containing the response to the Internet request.</returns>
      public virtual IWebResponse GetResponse()
      {
         return new WebResponseAdapter(_webRequest.GetResponse());
      }

      #endregion
   }
}