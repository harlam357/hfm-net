/*
 * HFM.NET - Network Operations Helper Class
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Mail;

using harlam357.Core.Net;

namespace HFM.Core
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

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or localFilePath is null or empty.</exception>
      void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode);

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is null or empty.</exception>
      IAsyncResult BeginFtpCheckConnection(string server, int port, string ftpPath, string username, string password, FtpMode ftpMode, AsyncCallback callback);

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
      void FtpCheckConnection(string server, int port, string ftpPath, string username, string password, FtpMode ftpMode);

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      IAsyncResult BeginHttpCheckConnection(string url, string username, string password, AsyncCallback callback);

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      void HttpCheckConnection(string url, string username, string password);
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

      private IWebOperation _ftpWebOperation;

      public IWebOperation FtpOperation
      {
         get { return _ftpWebOperation; }
      }

      public event EventHandler<WebOperationProgressChangedEventArgs> FtpWebOperationProgress;

      protected void OnFtpWebOperationProgress(object sender, WebOperationProgressChangedEventArgs e)
      {
         if (FtpWebOperationProgress != null)
         {
            FtpWebOperationProgress(this, e);
         }
      }

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
      public void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(server, port, ftpPath, localFilePath, -1, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or localFilePath is null or empty.</exception>
      public void FtpUploadHelper(string server, int port, string ftpPath, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         FtpUploadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}{3}",
            server, port, ftpPath, Path.GetFileName(localFilePath))), localFilePath, maximumLength, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public void FtpUploadHelper(Uri resourceUri, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(resourceUri, localFilePath, -1, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public void FtpUploadHelper(Uri resourceUri, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri");

         FtpUploadHelper(WebOperation.Create(resourceUri), localFilePath, maximumLength, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is null.</exception>
      /// <exception cref="ArgumentException">Throws if localFilePath is null or empty.</exception>
      public void FtpUploadHelper(IWebOperation ftpWebOperation, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         FtpUploadHelper(ftpWebOperation, localFilePath, -1, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is null.</exception>
      /// <exception cref="ArgumentException">Throws if localFilePath is null or empty.</exception>
      public void FtpUploadHelper(IWebOperation ftpWebOperation, string localFilePath, int maximumLength, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.ProgressChanged += OnFtpWebOperationProgress;
         _ftpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode((IFtpWebRequest)_ftpWebOperation.WebRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.WebRequest, username, password);
         _ftpWebOperation.Upload(localFilePath, maximumLength);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to download from on remote Ftp server.</param>
      /// <param name="remoteFileName">Remote file to download.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if server, ftpPath, or remoteFileName is null or empty.</exception>
      public void FtpDownloadHelper(string server, string ftpPath, string remoteFileName, string localFilePath, 
                                    string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(remoteFileName)) throw new ArgumentException("Argument 'remoteFileName' cannot be a null or empty string.");

         FtpDownloadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            server, ftpPath, remoteFileName)), localFilePath, username, password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="port">Server Port.</param>
      /// <param name="ftpPath">Path to download from on remote Ftp server.</param>
      /// <param name="remoteFileName">Remote file to download.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if server, ftpPath, or remoteFileName is null or empty.</exception>
      public void FtpDownloadHelper(string server, int port, string ftpPath, string remoteFileName, string localFilePath,
                                    string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(remoteFileName)) throw new ArgumentException("Argument 'remoteFileName' cannot be a null or empty string.");

         FtpDownloadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}:{1}{2}{3}",
            server, port, ftpPath, remoteFileName)), localFilePath, username, password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public void FtpDownloadHelper(Uri resourceUri, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri");

         FtpDownloadHelper(WebOperation.Create(resourceUri), localFilePath, username, password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is null.</exception>
      /// <exception cref="ArgumentException">Throws if localFilePath is null or empty.</exception>
      public void FtpDownloadHelper(IWebOperation ftpWebOperation, string localFilePath, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.ProgressChanged += OnFtpWebOperationProgress;
         _ftpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode((IFtpWebRequest)_ftpWebOperation.WebRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.WebRequest, username, password);
         _ftpWebOperation.Download(localFilePath);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to download from on remote Ftp server.</param>
      /// <param name="remoteFileName">Remote file to download.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      public long GetFtpDownloadLength(string server, string ftpPath, string remoteFileName, string username, string password, FtpMode ftpMode)
      {
         if (String.IsNullOrEmpty(server)) throw new ArgumentException("Argument 'server' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(ftpPath)) throw new ArgumentException("Argument 'ftpPath' cannot be a null or empty string.");
         if (String.IsNullOrEmpty(remoteFileName)) throw new ArgumentException("Argument 'remoteFileName' cannot be a null or empty string.");

         return GetFtpDownloadLength(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            server, ftpPath, remoteFileName)), username, password, ftpMode);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public long GetFtpDownloadLength(Uri resourceUri, string username, string password, FtpMode ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'resourceUri' cannot be null.");

         return GetFtpDownloadLength(WebOperation.Create(resourceUri), username, password, ftpMode);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public long GetFtpDownloadLength(IWebOperation ftpWebOperation, string username, string password, FtpMode ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.ProgressChanged += OnHttpWebOperationProgress;
         _ftpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode((IFtpWebRequest)ftpWebOperation.WebRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.WebRequest, username, password);
         return _ftpWebOperation.GetDownloadLength();
      }

      private IWebOperation _httpWebOperation;

      public IWebOperation HttpOperation
      {
         get { return _httpWebOperation; }
      }

      public event EventHandler<WebOperationProgressChangedEventArgs> HttpWebOperationProgress;

      protected void OnHttpWebOperationProgress(object sender, WebOperationProgressChangedEventArgs e)
      {
         if (HttpWebOperationProgress != null)
         {
            HttpWebOperationProgress(this, e);
         }
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      public void HttpDownloadHelper(string url, string localFilePath, string username, string password)
      {
         if (String.IsNullOrEmpty(url)) throw new ArgumentException("Argument 'url' cannot be a null or empty string.");

         HttpDownloadHelper(new Uri(url), localFilePath, username, password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      /// <exception cref="ArgumentException">Throws if localFilePath is null or empty.</exception>
      public void HttpDownloadHelper(Uri resourceUri, string localFilePath, string username, string password)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri");

         HttpDownloadHelper(WebOperation.Create(resourceUri), localFilePath, username, password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      /// <exception cref="ArgumentException">Throws if localFilePath is null or empty.</exception>
      public void HttpDownloadHelper(IWebOperation httpWebOperation, string localFilePath, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation");
         if (String.IsNullOrEmpty(localFilePath)) throw new ArgumentException("Argument 'localFilePath' cannot be a null or empty string.");

         _httpWebOperation = httpWebOperation;
         _httpWebOperation.ProgressChanged += OnHttpWebOperationProgress;
         _httpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_httpWebOperation.WebRequest, username, password);
         SetProxy(_httpWebOperation.WebRequest);

         _httpWebOperation.Download(localFilePath);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      public long GetHttpDownloadLength(string url, string username, string password)
      {
         if (String.IsNullOrEmpty(url)) throw new ArgumentException("Argument 'url' cannot be a null or empty string.");

         return GetHttpDownloadLength(new Uri(url), username, password);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public long GetHttpDownloadLength(Uri resourceUri, string username, string password)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri");

         return GetHttpDownloadLength(WebOperation.Create(resourceUri), username, password);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if resourceUri is null.</exception>
      public long GetHttpDownloadLength(IWebOperation httpWebOperation, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation");

         _httpWebOperation = httpWebOperation;
         _httpWebOperation.ProgressChanged += OnHttpWebOperationProgress;
         _httpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_httpWebOperation.WebRequest, username, password);
         SetProxy(_httpWebOperation.WebRequest);

         return _httpWebOperation.GetDownloadLength();
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
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is null or empty.</exception>
      public IAsyncResult BeginFtpCheckConnection(string server, int port, string ftpPath, string username, string password, FtpMode ftpMode, AsyncCallback callback)
      {
         var action = new Action(() => FtpCheckConnection(server, port, ftpPath, username, password, ftpMode));
         return action.BeginInvoke(callback, action);
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
         SetFtpMode(ftpWebRequest, ftpMode);

         SetNetworkCredentials(ftpWebOperation.WebRequest, username, password);

         ftpWebOperation.CheckConnection();
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      public IAsyncResult BeginHttpCheckConnection(string url, string username, string password, AsyncCallback callback)
      {
         var action = new Action(() => HttpCheckConnection(url, username, password));
         return action.BeginInvoke(callback, action);
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is null or empty.</exception>
      public void HttpCheckConnection(string url, string username, string password)
      {
         if (String.IsNullOrEmpty(url)) throw new ArgumentException("Argument 'url' cannot be a null or empty string.");

         WebOperation webOperation = WebOperation.Create(new Uri(url));
         HttpCheckConnection(webOperation, username, password);
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if httpWebOperation is null.</exception>
      public void HttpCheckConnection(IWebOperation httpWebOperation, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         httpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         httpWebOperation.WebRequest.Timeout = 5000; // 5 second timeout

         SetNetworkCredentials(httpWebOperation.WebRequest, username, password);
         SetProxy(httpWebOperation.WebRequest);

         httpWebOperation.CheckConnection();
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
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "FTP Type '{0}' is not valid.", ftpMode));
         }
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

      /// <summary>
      /// Sets the Credentials property with the username and password.
      /// </summary>
      /// <param name="request">The object that makes a request to a Uniform Resource Identifier (URI).</param>
      /// <param name="username">The login username.</param>
      /// <param name="password">The login password.</param>
      /// <exception cref="ArgumentNullException">request is null.</exception>
      private static void SetNetworkCredentials(IWebRequest request, string username, string password)
      {
         if (request == null) throw new ArgumentNullException("request", "Argument 'Request' cannot be null.");

         NetworkCredential credentials = NetworkCredentialFactory.Create(username, password);
         if (credentials != null)
         {
            request.Credentials = credentials;
         }
      }

      /// <summary>
      /// Set Proxy Information on WebRequest.
      /// </summary>
      /// <param name="request">Makes a request to a Uniform Resource Identifier (URI).</param>
      private void SetProxy(IWebRequest request)
      {
         Debug.Assert(request != null);

         IWebProxy proxy = _prefs.GetWebProxy();
         if (proxy != null)
         {
            request.Proxy = proxy;
         }
         // Don't set request.Proxy = null - Issue 49
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
               String[] userParts = username.Split('\\');
               return new NetworkCredential(userParts[1], password, userParts[0]);
            }
            
            return new NetworkCredential(username, password);
         }

         return null;
      }
   }
}
