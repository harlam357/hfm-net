/*
 * HFM.NET - Network Operations Helper Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using harlam357.Net;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public delegate void FtpCheckConnectionDelegate(string server, string ftpPath, string username, string password, FtpType ftpMode);
   public delegate void HttpCheckConnectionDelegate(string url, string username, string password);

   public interface INetworkOps
   {
      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or LocalFilePath is Null or Empty.</exception>
      void FtpUploadHelper(string server, string ftpPath, string localFilePath, string username, string password, FtpType ftpMode);

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or LocalFilePath is Null or Empty.</exception>
      void FtpUploadHelper(string server, string ftpPath, string localFilePath, int maximumLength, string username, string password, FtpType ftpMode);
   
      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is Null or Empty.</exception>
      IAsyncResult BeginFtpCheckConnection(string server, string ftpPath, string username, string password, FtpType ftpMode, AsyncCallback callback);
   
      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is Null or Empty.</exception>
      void FtpCheckConnection(string server, string ftpPath, string username, string password, FtpType ftpMode);

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      IAsyncResult BeginHttpCheckConnection(string url, string username, string password, AsyncCallback callback);

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      void HttpCheckConnection(string url, string username, string password);
   }

   /// <summary>
   /// Network Operations Class
   /// </summary>
   public class NetworkOps : INetworkOps
   {
      private IFtpWebOperation _ftpWebOperation;
      public IFtpWebOperation FtpOperation
      {
         get { return _ftpWebOperation; }
      }
      
      public event EventHandler<WebOperationProgressEventArgs> FtpWebOperationProgress;
      protected void OnFtpWebOperationProgress(object sender, WebOperationProgressEventArgs e)
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
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(string server, string ftpPath, string localFilePath, string username, string password, FtpType ftpMode)
      {
         FtpUploadHelper(server, ftpPath, localFilePath, -1, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="maximumLength"></param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(string server, string ftpPath, string localFilePath, int maximumLength, string username, string password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(server) || String.IsNullOrEmpty(ftpPath) || String.IsNullOrEmpty(localFilePath))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'LocalFilePath' cannot be a null or empty string.");
         }

         FtpUploadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            server, ftpPath, Path.GetFileName(localFilePath))), localFilePath, maximumLength, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public void FtpUploadHelper(Uri resourceUri, string localFilePath, string username, string password, FtpType ftpMode)
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
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public void FtpUploadHelper(Uri resourceUri, string localFilePath, int maximumLength, string username, string password, FtpType ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'ResourceUri' cannot be null.");

         FtpUploadHelper((FtpWebOperation)WebOperation.Create(resourceUri), localFilePath, maximumLength, username, password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(IFtpWebOperation ftpWebOperation, string localFilePath, string username, string password, FtpType ftpMode)
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
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(IFtpWebOperation ftpWebOperation, string localFilePath, int maximumLength, string username, string password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(localFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.WebOperationProgress += OnFtpWebOperationProgress;
         _ftpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(_ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.OperationRequest.Request, username, password);
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
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, or RemoteFileName is Null or Empty.</exception>
      public void FtpDownloadHelper(string server, string ftpPath, string remoteFileName, string localFilePath, 
                                    string username, string password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(server) || String.IsNullOrEmpty(ftpPath) || String.IsNullOrEmpty(remoteFileName))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'RemoteFileName'cannot be a null or empty string.");
         }

         FtpDownloadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            server, ftpPath, remoteFileName)), localFilePath, username, password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public void FtpDownloadHelper(Uri resourceUri, string localFilePath, string username, string password, FtpType ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'ResourceUri' cannot be null.");
      
         FtpDownloadHelper((FtpWebOperation)WebOperation.Create(resourceUri), localFilePath, username, password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void FtpDownloadHelper(IFtpWebOperation ftpWebOperation, string localFilePath, string username, string password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(localFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.WebOperationProgress += OnFtpWebOperationProgress;
         _ftpWebOperation.FtpOperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(_ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.OperationRequest.Request, username, password);
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
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public long GetFtpDownloadLength(string server, string ftpPath, string remoteFileName, string username, string password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(server) || String.IsNullOrEmpty(ftpPath) || String.IsNullOrEmpty(remoteFileName))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'RemoteFileName'cannot be a null or empty string.");
         }

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
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetFtpDownloadLength(Uri resourceUri, string username, string password, FtpType ftpMode)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'ResourceUri' cannot be null.");

         return GetFtpDownloadLength((IFtpWebOperation)WebOperation.Create(resourceUri), username, password, ftpMode);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetFtpDownloadLength(IFtpWebOperation ftpWebOperation, string username, string password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         _ftpWebOperation = ftpWebOperation;
         _ftpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _ftpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_ftpWebOperation.OperationRequest.Request, username, password);
         return _ftpWebOperation.GetDownloadLength();
      }

      private IWebOperation _httpWebOperation;
      public IWebOperation HttpOperation
      {
         get { return _httpWebOperation; }
      }

      public event EventHandler<WebOperationProgressEventArgs> HttpWebOperationProgress;
      protected void OnHttpWebOperationProgress(object sender, WebOperationProgressEventArgs e)
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
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public void HttpDownloadHelper(string url, string localFilePath, string username, string password)
      {
         if (String.IsNullOrEmpty(url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }
         
         HttpDownloadHelper(new Uri(url), localFilePath, username, password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void HttpDownloadHelper(Uri resourceUri, string localFilePath, string username, string password)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'ResourceUri' cannot be null.");
      
         HttpDownloadHelper(WebOperation.Create(resourceUri), localFilePath, username, password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="localFilePath">Path to local file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void HttpDownloadHelper(IWebOperation httpWebOperation, string localFilePath, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(localFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _httpWebOperation = httpWebOperation;
         _httpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _httpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_httpWebOperation.OperationRequest.Request, username, password);
         SetProxy(_httpWebOperation.OperationRequest.Request);

         _httpWebOperation.Download(localFilePath);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public long GetHttpDownloadLength(string url, string username, string password)
      {
         if (String.IsNullOrEmpty(url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         return GetHttpDownloadLength(new Uri(url), username, password);
      }
      
      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="resourceUri">Web Resource Uri.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetHttpDownloadLength(Uri resourceUri, string username, string password)
      {
         if (resourceUri == null) throw new ArgumentNullException("resourceUri", "Argument 'ResourceUri' cannot be null.");
      
         return GetHttpDownloadLength(WebOperation.Create(resourceUri), username, password);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetHttpDownloadLength(IWebOperation httpWebOperation, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         _httpWebOperation = httpWebOperation;
         _httpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _httpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_httpWebOperation.OperationRequest.Request, username, password);
         SetProxy(_httpWebOperation.OperationRequest.Request);

         return _httpWebOperation.GetDownloadLength();
      }

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is Null or Empty.</exception>
      public IAsyncResult BeginFtpCheckConnection(string server, string ftpPath, string username, string password, FtpType ftpMode, AsyncCallback callback)
      {
         var action = new Action(() => FtpCheckConnection(server, ftpPath, username, password, ftpMode));
         return action.BeginInvoke(callback, action);
      }
      
      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="server">Server Name or IP.</param>
      /// <param name="ftpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is Null or Empty.</exception>
      public void FtpCheckConnection(string server, string ftpPath, string username, string password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(server) || String.IsNullOrEmpty(ftpPath))
         {
            throw new ArgumentException("Arguments 'server' and 'ftpPath' cannot be a null or empty string.");
         }

         var ftpWebOperation = (FtpWebOperation)WebOperation.Create(new Uri(
            String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}", server, ftpPath)));
         FtpCheckConnection(ftpWebOperation, username, password, ftpMode);
      }

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="username">Ftp Login Username.</param>
      /// <param name="password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if ftpWebOperation is Null.</exception>
      public void FtpCheckConnection(IFtpWebOperation ftpWebOperation, string username, string password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         ftpWebOperation.FtpOperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         ftpWebOperation.FtpOperationRequest.KeepAlive = false; // Close the Request
         ftpWebOperation.FtpOperationRequest.Timeout = 5000; // 5 second timeout
         SetFtpMode(ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(ftpWebOperation.OperationRequest.Request, username, password);

         ftpWebOperation.CheckConnection();
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="url">Http Url of remote file.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <param name="callback"></param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
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
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public void HttpCheckConnection(string url, string username, string password)
      {
         if (String.IsNullOrEmpty(url))
         {
            throw new ArgumentException("Argument 'url' cannot be a null or empty string.");
         }

         WebOperation webOperation = WebOperation.Create(new Uri(url));
         HttpCheckConnection(webOperation, username, password);
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="username">Http Login Username.</param>
      /// <param name="password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if httpWebOperation is Null.</exception>
      public void HttpCheckConnection(IWebOperation httpWebOperation, string username, string password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         httpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         httpWebOperation.OperationRequest.Timeout = 5000; // 5 second timeout

         SetNetworkCredentials(httpWebOperation.OperationRequest.Request, username, password);
         SetProxy(httpWebOperation.OperationRequest.Request);

         httpWebOperation.CheckConnection();
      }

      private static void SetFtpMode(IFtpWebOperationRequest request, FtpType ftpMode)
      {
         Debug.Assert(request != null);
      
         switch (ftpMode)
         {
            case FtpType.Passive:
               request.UsePassive = true;
               break;
            case FtpType.Active:
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
         var message = new MailMessage(messageFrom, messageTo, messageSubject, messageBody);
         var client = new SmtpClient(smtpHost, smtpPort)
                      {
                         Credentials = GetNetworkCredential(smtpHostUsername, smtpHostPassword),
                         EnableSsl = enableSsl
                      };
         client.Send(message);
      }

      /// <summary>
      /// Set WebRequest Username and Password.
      /// </summary>
      /// <param name="request">Makes a request to a Uniform Resource Identifier (URI).</param>
      /// <param name="username">Login Username.</param>
      /// <param name="password">Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Request is Null.</exception>
      public static void SetNetworkCredentials(WebRequest request, string username, string password)
      {
         if (request == null) throw new ArgumentNullException("request", "Argument 'Request' cannot be null.");

         NetworkCredential credentials = GetNetworkCredential(username, password);
         if (credentials != null)
         {
            request.Credentials = credentials;
         }
      }

      /// <summary>
      /// Set Proxy Information on WebRequest.
      /// </summary>
      /// <param name="request">Makes a request to a Uniform Resource Identifier (URI).</param>
      private static void SetProxy(WebRequest request)
      {
         Debug.Assert(request != null);

         IWebProxy proxy = GetProxy();
         if (proxy != null)
         {
            request.Proxy = proxy;
         }
         // Don't set request.Proxy = null - Issue 49
      }
      
      /// <summary>
      /// Get Web Proxy Interface based on Preferences.
      /// </summary>
      public static IWebProxy GetProxy()
      {
         var prefs = InstanceProvider.GetInstance<IPreferenceSet>();

         if (prefs.GetPreference<bool>(Preference.UseProxy))
         {
            IWebProxy proxy = new WebProxy(prefs.GetPreference<string>(Preference.ProxyServer),
                                           prefs.GetPreference<int>(Preference.ProxyPort));
            if (prefs.GetPreference<bool>(Preference.UseProxyAuth))
            {
               proxy.Credentials = GetNetworkCredential(prefs.GetPreference<string>(Preference.ProxyUser),
                                                        prefs.GetPreference<string>(Preference.ProxyPass));
            }

            return proxy;
         }

         return null;
      }
      
      /// <summary>
      /// Creates and Returns a new Network Credential object.
      /// </summary>
      /// <param name="username">Network Credential Username or Domain\Username</param>
      /// <param name="password">Network Credential Password</param>
      private static NetworkCredential GetNetworkCredential(string username, string password)
      {
         if (StringOps.ValidateUsernamePasswordPair(username, password))
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
