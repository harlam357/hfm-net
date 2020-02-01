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

namespace HFM.Core.Net
{
   /// <summary>
   /// Represents an object that encapsulates a File Transfer Protocol (FTP) server's response to a request.
   /// </summary>
   public interface IFtpWebResponse : IWebResponse
   {
      /// <summary>
      /// Gets the message sent by the FTP server when a connection is established prior to logon.
      /// </summary>
      string BannerMessage { get; }

      /// <summary>
      /// Gets the message sent by the server when the FTP session is ending.
      /// </summary>
      string ExitMessage { get; }

      /// <summary>
      /// Gets the date and time that a file on an FTP server was last modified.
      /// </summary>
      DateTime LastModified { get; }

      /// <summary>
      /// Gets the most recent status code sent from the FTP server.
      /// </summary>
      FtpStatusCode StatusCode { get; }

      /// <summary>
      /// Gets text that describes a status code sent from the FTP server.
      /// </summary>
      string StatusDescription { get; }

      /// <summary>
      /// Gets the message sent by the FTP server when authentication is complete.
      /// </summary>
      string WelcomeMessage { get; }
   }

   /// <summary>
   /// Wraps a System.Net.FtpWebResponse object that encapsulates a File Transfer Protocol (FTP) server's response to a request.
   /// </summary>
   internal class FtpWebResponseAdapter : WebResponseAdapter, IFtpWebResponse
   {
      private readonly FtpWebResponse _ftpWebResponse;

      /// <summary>
      /// Initializes a new instance of the FtpWebResponseAdapter class.
      /// </summary>
      /// <param name="ftpWebResponse">The System.Net.FtpWebResponse instance this object wraps.</param>
      public FtpWebResponseAdapter(FtpWebResponse ftpWebResponse)
         : base(ftpWebResponse)
      {
         _ftpWebResponse = ftpWebResponse;
      }

      #region Properties

      /// <summary>
      /// Gets the message sent by the FTP server when a connection is established prior to logon.
      /// </summary>
      public string BannerMessage
      {
         get { return _ftpWebResponse.BannerMessage; }
      }

      /// <summary>
      /// Gets the message sent by the server when the FTP session is ending.
      /// </summary>
      public string ExitMessage
      {
         get { return _ftpWebResponse.ExitMessage; }
      }

      /// <summary>
      /// Gets the date and time that a file on an FTP server was last modified.
      /// </summary>
      public DateTime LastModified
      {
         get { return _ftpWebResponse.LastModified; }
      }

      /// <summary>
      /// Gets the most recent status code sent from the FTP server.
      /// </summary>
      public FtpStatusCode StatusCode
      {
         get { return _ftpWebResponse.StatusCode; }
      }

      /// <summary>
      /// Gets text that describes a status code sent from the FTP server.
      /// </summary>
      public string StatusDescription
      {
         get { return _ftpWebResponse.StatusDescription; }
      }

      /// <summary>
      /// Gets the message sent by the FTP server when authentication is complete.
      /// </summary>
      public string WelcomeMessage
      {
         get { return _ftpWebResponse.WelcomeMessage; }
      }

      #endregion
   }
}