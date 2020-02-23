/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Mail;

using HFM.Preferences;

namespace HFM.Core.Net
{
   public interface INetworkOps
   {
      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or localFilePath is null or empty.</exception>
      void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode);

      void FtpUploadHelper(string server, int port, string ftpPath, string remoteFileName, Stream localStream, int maximumLength, string username, string password, FtpMode ftpMode);
   }

   /// <summary>
   /// Network Operations Class
   /// </summary>
   public class NetworkOps : INetworkOps
   {
      private readonly IPreferenceSet _prefs;

      public NetworkOps(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      public void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(server, port, ftpPath, localFilePath, -1, username, password, ftpMode);
      }

      public void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         FtpUploadHelper(WebOperation.Create(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}{3}",
            server, port, ftpPath, Path.GetFileName(localFilePath)))), localFilePath, maximumLength, username, password, ftpMode);
      }

      internal void FtpUploadHelper(IWebOperation ftpWebOperation, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(ftpWebOperation, localFilePath, -1, username, password, ftpMode);
      }

      internal void FtpUploadHelper(IWebOperation ftpWebOperation, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         ftpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         ((IFtpWebRequest)ftpWebOperation.WebRequest).SetFtpMode(ftpMode);
         ftpWebOperation.WebRequest.SetNetworkCredentials(username, password);
         ftpWebOperation.Upload(localFilePath, maximumLength);
      }

      public void FtpUploadHelper(string server, int port, string ftpPath, string remoteFileName, Stream localStream, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(remoteFileName)) throw new ArgumentException("Argument 'remoteFileName' cannot be a null or empty string.");
         if (localStream == null) throw new ArgumentNullException("localStream");

         FtpUploadHelper(WebOperation.Create(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}{3}",
            server, port, ftpPath, remoteFileName))), localStream, maximumLength, username, password, ftpMode);
      }

      internal void FtpUploadHelper(IWebOperation ftpWebOperation, Stream localStream, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(ftpWebOperation, localStream, -1, username, password, ftpMode);
      }

      internal void FtpUploadHelper(IWebOperation ftpWebOperation, Stream localStream, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation");
         if (localStream == null) throw new ArgumentNullException("localStream");

         ftpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         ((IFtpWebRequest)ftpWebOperation.WebRequest).SetFtpMode(ftpMode);
         ftpWebOperation.WebRequest.SetNetworkCredentials(username, password);

         if (maximumLength >= 0 && localStream.Length >= maximumLength)
         {
            localStream.Position = localStream.Length - maximumLength;
         }
         ftpWebOperation.Upload(localStream);
      }

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is null or empty.</exception>
      public void FtpCheckConnection(string server, int port, string ftpPath, string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");

         var ftpWebOperation = WebOperation.Create(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}", server, port, ftpPath)));
         FtpCheckConnection(ftpWebOperation, username, password, ftpMode);
      }

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if ftpWebOperation is null.</exception>
      public void FtpCheckConnection(IWebOperation ftpWebOperation, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation");

         var ftpWebRequest = (IFtpWebRequest)ftpWebOperation.WebRequest;
         ftpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         ftpWebRequest.KeepAlive = false; // Close the Request
         ftpWebRequest.Timeout = 5000; // 5 second timeout
         ftpWebRequest.SetFtpMode(ftpMode);

         ftpWebOperation.WebRequest.SetNetworkCredentials(username, password);

         ftpWebOperation.CheckConnection();
      }

      /// <summary>
      /// Sends an e-mail message
      /// </summary>
      public static void SendEmail(bool enableSsl, string messageFrom, string messageTo, string messageSubject,
                                   string messageBody, string smtpHost, int smtpPort, string smtpHostUsername,
                                   string smtpHostPassword)
      {
         using (var message = new MailMessage(messageFrom, messageTo, messageSubject, messageBody))
         {
            var client = new SmtpClient(smtpHost, smtpPort)
            {
               Credentials = NetworkCredentialFactory.Create(smtpHostUsername, smtpHostPassword),
               EnableSsl = enableSsl
            };
            client.Send(message);
         }
      }
   }

   public static class NetworkCredentialFactory
   {
      /// <summary>
      /// Creates and returns a new NetworkCredential object.
      /// </summary>
      /// <param name="username">The username literal or in domain\username format.</param>
      /// <param name="password">The password literal.</param>
      public static NetworkCredential Create(string username, string password)
      {
         if (Validate.UsernamePasswordPair(username, password))
         {
            if (username.Contains("\\"))
            {
               string[] userParts = username.Split('\\');
               return new NetworkCredential(userParts[1], password, userParts[0]);
            }
            
            return new NetworkCredential(username, password);
         }

         return null;
      }
   }

   public static class WebRequestExtensions
   {
      public static void SetFtpMode(this IFtpWebRequest request, FtpMode ftpMode)
      {
         Debug.Assert(request != null);

         switch (ftpMode)
         {
            case FtpMode.Passive:
               request.UsePassive = true;
               break;
            case FtpMode.Active:
               request.UsePassive = false;
               break;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "FTP Type '{0}' is not valid.", ftpMode));
         }
      }

      /// <summary>
      /// Sets the Credentials property with the username and password.
      /// </summary>
      /// <param name="request">The object that makes a request to a Uniform Resource Identifier (URI).</param>
      /// <param name="username">The login username.</param>
      /// <param name="password">The login password.</param>
      /// <exception cref="ArgumentNullException">request is null.</exception>
      public static void SetNetworkCredentials(this IWebRequest request, string username, string password)
      {
         if (request == null) throw new ArgumentNullException("request", "Argument 'Request' cannot be null.");

         NetworkCredential credentials = NetworkCredentialFactory.Create(username, password);
         if (credentials != null)
         {
            request.Credentials = credentials;
         }
      }
   }
}
