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

        void FtpUploadHelper(string server, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode);
    }

    /// <summary>
    /// Network Operations Class
    /// </summary>
    public class NetworkOps : INetworkOps
    {
        public NetworkOps()
        {
            
        }

        public void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
        {
            if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
            if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
            if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

            string uriString = CreateUriStringForUpload(server, port, ftpPath, localFilePath);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            webOperation.Upload(localFilePath);
        }

        public void FtpUploadHelper(string server, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode)
        {
            if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
            if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
            if (String.IsNullOrEmpty(remoteFileName)) throw new ArgumentException("Argument 'remoteFileName' cannot be a null or empty string.");
            if (stream == null) throw new ArgumentNullException("stream");

            string uriString = CreateUriStringForUpload(server, port, ftpPath, remoteFileName);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            if (maximumLength >= 0 && stream.Length >= maximumLength)
            {
                stream.Position = stream.Length - maximumLength;
            }
            webOperation.Upload(stream);
        }

        private static string CreateUriStringForUpload(string server, int port, string ftpPath, string filePath)
        {
            return String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}{3}", server, port, ftpPath, Path.GetFileName(filePath));
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

            string uriString = String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}", server, port, ftpPath);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            var ftpWebRequest = (IFtpWebRequest)webOperation.WebRequest;
            ftpWebRequest.KeepAlive = false;
            ftpWebRequest.Timeout = 5000;
            webOperation.CheckConnection();
        }

        private static WebOperation CreateWebOperation(string uriString, FtpMode ftpMode, string username, string password)
        {
            var webOperation = WebOperation.Create(new Uri(uriString));
            webOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            SetFtpMode((IFtpWebRequest)webOperation.WebRequest, ftpMode);
            SetNetworkCredentials(webOperation.WebRequest, username, password);
            return webOperation;
        }

        private static void SetFtpMode(IFtpWebRequest request, FtpMode ftpMode)
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
                    throw new InvalidOperationException($"FTP Mode {ftpMode} is not valid.");
            }
        }

        private static void SetNetworkCredentials(IWebRequest request, string username, string password)
        {
            var credentials = NetworkCredentialFactory.Create(username, password);
            if (credentials != null)
            {
                request.Credentials = credentials;
            }
        }
    }
}
