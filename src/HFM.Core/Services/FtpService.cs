
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;

using HFM.Core.Net;

namespace HFM.Core.Services
{
    public abstract class FtpService
    {
        public static FtpService Default { get; } = DefaultFtpService.Instance;

        // ReSharper disable once EmptyConstructor
        protected FtpService()
        {

        }

        public abstract void Upload(string host, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode);

        public abstract void Upload(string host, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode);

        public abstract void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode);
    }

    public class DefaultFtpService : FtpService
    {
        public static DefaultFtpService Instance { get; } = new DefaultFtpService();

        protected DefaultFtpService()
        {

        }

        public override void Upload(string host, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (ftpPath == null) throw new ArgumentNullException(nameof(ftpPath));
            if (localFilePath == null) throw new ArgumentNullException(nameof(localFilePath));

            string uriString = CreateUriStringForUpload(host, port, ftpPath, localFilePath);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            webOperation.Upload(localFilePath);
        }

        public override void Upload(string host, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode)
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

        public override void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (ftpPath == null) throw new ArgumentNullException(nameof(ftpPath));

            string uriString = String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}", host, port, ftpPath);
            var webOperation = CreateWebOperation(uriString, ftpMode, username, password);
            var ftpWebRequest = (FtpWebRequest)webOperation.WebRequest;
            ftpWebRequest.KeepAlive = false;
            ftpWebRequest.Timeout = 5000;
            webOperation.CheckConnection();
        }

        private static WebOperation CreateWebOperation(string uriString, FtpMode ftpMode, string username, string password)
        {
            var webOperation = WebOperation.Create(new Uri(uriString));
            webOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            SetFtpMode((FtpWebRequest)webOperation.WebRequest, ftpMode);
            SetNetworkCredentials(webOperation.WebRequest, username, password);
            return webOperation;
        }

        private static void SetFtpMode(FtpWebRequest request, FtpMode ftpMode)
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

        private static void SetNetworkCredentials(WebRequest request, string username, string password)
        {
            var credentials = NetworkCredentialFactory.Create(username, password);
            if (credentials != null)
            {
                request.Credentials = credentials;
            }
        }
    }

    public class NullFtpService : FtpService
    {
        public static NullFtpService Instance { get; } = new NullFtpService();

        protected NullFtpService()
        {

        }

        public override void Upload(string host, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
        {
            // do nothing
        }

        public override void Upload(string host, int port, string ftpPath, string remoteFileName, Stream stream, int maximumLength, string username, string password, FtpMode ftpMode)
        {
            // do nothing
        }

        public override void CheckConnection(string host, int port, string ftpPath, string username, string password, FtpMode ftpMode)
        {
            // do nothing
        }
    }
}
