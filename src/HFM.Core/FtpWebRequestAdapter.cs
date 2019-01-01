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
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace HFM.Core
{
   /// <summary>
   /// Represents an object that implements a File Transfer Protocol (FTP) client.
   /// </summary>
   public interface IFtpWebRequest : IWebRequest
   {
      #region Properties

      /// <summary>
      /// Gets or sets the certificates used for establishing an encrypted connection to the FTP server.
      /// </summary>
      X509CertificateCollection ClientCertificates { get; set; }

      /// <summary>
      /// Gets or sets a byte offset into the file being downloaded by this request.
      /// </summary>
      long ContentOffset { get; set; }

      /// <summary>
      /// Gets or sets a System.Boolean that specifies that an SSL connection should be used.
      /// </summary>
      bool EnableSsl { get; set; }

      /// <summary>
      /// Gets or sets a System.Boolean value that specifies whether the control connection to the FTP server is closed after the request completes.
      /// </summary>
      bool KeepAlive { get; set; }

      /// <summary>
      /// Gets or sets a time-out when reading from or writing to a stream.
      /// </summary>
      int ReadWriteTimeout { get; set; }

      /// <summary>
      /// Gets or sets the new name of a file being renamed.
      /// </summary>
      string RenameTo { get; set; }

      /// <summary>
      /// Gets the System.Net.ServicePoint object used to connect to the FTP server.
      /// </summary>
      ServicePoint ServicePoint { get; }

      /// <summary>
      /// Gets or sets the behavior of a client application's data transfer process.
      /// </summary>
      bool UsePassive { get; set; }

      /// <summary>
      /// Gets or sets a System.Boolean value that specifies the data type for file transfers.
      /// </summary>
      bool UseBinary { get; set; }

      #endregion
   }

   /// <summary>
   /// Wraps a System.Net.FtpWebRequest object that implements a File Transfer Protocol (FTP) client.
   /// </summary>
   internal class FtpWebRequestAdapter : WebRequestAdapter, IFtpWebRequest
   {
      private readonly FtpWebRequest _ftpWebRequest;

      /// <summary>
      /// Initializes a new instance of the FtpWebRequestAdapter class.
      /// </summary>
      /// <param name="ftpWebRequest">The System.Net.FtpWebRequest instance this object wraps.</param>
      public FtpWebRequestAdapter(FtpWebRequest ftpWebRequest)
         : base(ftpWebRequest)
      {
         _ftpWebRequest = ftpWebRequest;
      }

      #region Properties

      /// <summary>
      /// Gets or sets the certificates used for establishing an encrypted connection to the FTP server.
      /// </summary>
      public X509CertificateCollection ClientCertificates
      {
         get { return _ftpWebRequest.ClientCertificates; }
         set { _ftpWebRequest.ClientCertificates = value; }
      }

      /// <summary>
      /// Gets or sets a byte offset into the file being downloaded by this request.
      /// </summary>
      public long ContentOffset
      {
         get { return _ftpWebRequest.ContentOffset; }
         set { _ftpWebRequest.ContentOffset = value; }
      }

      /// <summary>
      /// Gets or sets a System.Boolean that specifies that an SSL connection should be used.
      /// </summary>
      public bool EnableSsl
      {
         get { return _ftpWebRequest.EnableSsl; }
         set { _ftpWebRequest.EnableSsl = value; }
      }

      /// <summary>
      /// Gets or sets a System.Boolean value that specifies whether the control connection to the FTP server is closed after the request completes.
      /// </summary>
      public bool KeepAlive
      {
         get { return _ftpWebRequest.KeepAlive; }
         set { _ftpWebRequest.KeepAlive = value; }
      }

      /// <summary>
      /// Gets or sets a time-out when reading from or writing to a stream.
      /// </summary>
      public int ReadWriteTimeout
      {
         get { return _ftpWebRequest.ReadWriteTimeout; }
         set { _ftpWebRequest.ReadWriteTimeout = value; }
      }

      /// <summary>
      /// Gets or sets the new name of a file being renamed.
      /// </summary>
      public string RenameTo
      {
         get { return _ftpWebRequest.RenameTo; }
         set { _ftpWebRequest.RenameTo = value; }
      }

      /// <summary>
      /// Gets the System.Net.ServicePoint object used to connect to the FTP server.
      /// </summary>
      public ServicePoint ServicePoint
      {
         get { return _ftpWebRequest.ServicePoint; }
      }

      /// <summary>
      /// Gets or sets the behavior of a client application's data transfer process.
      /// </summary>
      public bool UsePassive
      {
         get { return _ftpWebRequest.UsePassive; }
         set { _ftpWebRequest.UsePassive = value; }
      }

      /// <summary>
      /// Gets or sets a System.Boolean value that specifies the data type for file transfers.
      /// </summary>
      public bool UseBinary
      {
         get { return _ftpWebRequest.UseBinary; }
         set { _ftpWebRequest.UseBinary = value; }
      }

      #endregion

      #region Methods

      /// <summary>
      /// Returns an IWebResponse.
      /// </summary>
      /// <param name="asyncResult">An System.IAsyncResult that references a pending request for a response.</param>
      /// <returns>An IWebResponse that contains a response to the Internet request.</returns>
      public override IWebResponse EndGetResponse(IAsyncResult asyncResult)
      {
         return new FtpWebResponseAdapter((FtpWebResponse)_ftpWebRequest.EndGetResponse(asyncResult));
      }

      /// <summary>
      /// Returns a response to an Internet request.
      /// </summary>
      /// <returns>An IWebResponse containing the response to the Internet request.</returns>
      public override IWebResponse GetResponse()
      {
         return new FtpWebResponseAdapter((FtpWebResponse)_ftpWebRequest.GetResponse());
      }

      #endregion
   }
}