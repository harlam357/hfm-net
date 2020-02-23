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
using System.Net.Cache;

using HFM.Core.Net;

namespace HFM.Core.Services
{
    public interface IFtpService
    {
        void Upload(string host, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode);

        void Upload(string host, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode);

        void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode);
    }

    public class FtpService : IFtpService
    {
        public void Upload(string host, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (ftpPath == null) throw new ArgumentNullException(nameof(ftpPath));
            if (localFilePath == null) throw new ArgumentNullException(nameof(localFilePath));

            string uriString = CreateUriStringForUpload(host, port, ftpPath, localFilePath);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            webOperation.Upload(localFilePath);
        }

        public void Upload(string host, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (ftpPath == null) throw new ArgumentNullException(nameof(ftpPath));
            if (remoteFileName == null) throw new ArgumentNullException(nameof(remoteFileName));
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            string uriString = CreateUriStringForUpload(host, port, ftpPath, remoteFileName);
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

        public void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (ftpPath == null) throw new ArgumentNullException(nameof(ftpPath));

            string uriString = String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}", host, port, ftpPath);
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
