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

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Helpers
{
   public delegate void FtpCheckConnectionDelegate(string Server, string FtpPath, string Username, string Password, FtpType ftpMode);
   public delegate void HttpCheckConnectionDelegate(string Url, string Username, string Password);

   /// <summary>
   /// Network Operations Class
   /// </summary>
   public class NetworkOps
   {
      private IFtpWebOperation _FtpWebOperation;
      public IFtpWebOperation FtpOperation
      {
         get { return _FtpWebOperation; }
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
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath, or LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(string Server, string FtpPath, string LocalFilePath, string Username, string Password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'LocalFilePath' cannot be a null or empty string.");
         }
         
         FtpUploadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            Server, FtpPath, Path.GetFileName(LocalFilePath))), LocalFilePath, Username, Password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="ResourceUri">Web Resource Uri.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public void FtpUploadHelper(Uri ResourceUri, string LocalFilePath, string Username, string Password, FtpType ftpMode)
      {
         if (ResourceUri == null) throw new ArgumentNullException("ResourceUri", "Argument 'ResourceUri' cannot be null.");
      
         FtpUploadHelper((FtpWebOperation)WebOperation.Create(ResourceUri), LocalFilePath, Username, Password, ftpMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void FtpUploadHelper(IFtpWebOperation ftpWebOperation, string LocalFilePath, string Username, string Password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _FtpWebOperation = ftpWebOperation;
         _FtpWebOperation.WebOperationProgress += OnFtpWebOperationProgress;
         _FtpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(_FtpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_FtpWebOperation.OperationRequest.Request, Username, Password);
         _FtpWebOperation.Upload(LocalFilePath);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to download from on remote Ftp server.</param>
      /// <param name="RemoteFileName">Remote file to download.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, or RemoteFileName is Null or Empty.</exception>
      public void FtpDownloadHelper(string Server, string FtpPath, string RemoteFileName, string LocalFilePath, 
                                    string Username, string Password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(RemoteFileName))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'RemoteFileName'cannot be a null or empty string.");
         }

         FtpDownloadHelper(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            Server, FtpPath, RemoteFileName)), LocalFilePath, Username, Password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="ResourceUri">Web Resource Uri.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public void FtpDownloadHelper(Uri ResourceUri, string LocalFilePath, string Username, string Password, FtpType ftpMode)
      {
         if (ResourceUri == null) throw new ArgumentNullException("ResourceUri", "Argument 'ResourceUri' cannot be null.");
      
         FtpDownloadHelper((FtpWebOperation)WebOperation.Create(ResourceUri), LocalFilePath, Username, Password, ftpMode);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ftpWebOperation is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void FtpDownloadHelper(IFtpWebOperation ftpWebOperation, string LocalFilePath, string Username, string Password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _FtpWebOperation = ftpWebOperation;
         _FtpWebOperation.WebOperationProgress += OnFtpWebOperationProgress;
         _FtpWebOperation.FtpOperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(_FtpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_FtpWebOperation.OperationRequest.Request, Username, Password);
         _FtpWebOperation.Download(LocalFilePath);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to download from on remote Ftp server.</param>
      /// <param name="RemoteFileName">Remote file to download.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public long GetFtpDownloadLength(string Server, string FtpPath, string RemoteFileName, string Username, string Password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(RemoteFileName))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'RemoteFileName'cannot be a null or empty string.");
         }

         return GetFtpDownloadLength(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}{2}",
            Server, FtpPath, RemoteFileName)), Username, Password, ftpMode);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="ResourceUri">Web Resource Uri.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetFtpDownloadLength(Uri ResourceUri, string Username, string Password, FtpType ftpMode)
      {
         if (ResourceUri == null) throw new ArgumentNullException("ResourceUri", "Argument 'ResourceUri' cannot be null.");

         return GetFtpDownloadLength((IFtpWebOperation)WebOperation.Create(ResourceUri), Username, Password, ftpMode);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetFtpDownloadLength(IFtpWebOperation ftpWebOperation, string Username, string Password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         _FtpWebOperation = ftpWebOperation;
         _FtpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _FtpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         SetFtpMode(ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(_FtpWebOperation.OperationRequest.Request, Username, Password);
         return _FtpWebOperation.GetDownloadLength();
      }

      private IWebOperation _HttpWebOperation;
      public IWebOperation HttpOperation
      {
         get { return _HttpWebOperation; }
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
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public void HttpDownloadHelper(string Url, string LocalFilePath, string Username, string Password)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }
         
         HttpDownloadHelper(new Uri(Url), LocalFilePath, Username, Password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="ResourceUri">Web Resource Uri.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void HttpDownloadHelper(Uri ResourceUri, string LocalFilePath, string Username, string Password)
      {
         if (ResourceUri == null) throw new ArgumentNullException("ResourceUri", "Argument 'ResourceUri' cannot be null.");
      
         HttpDownloadHelper(WebOperation.Create(ResourceUri), LocalFilePath, Username, Password);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public void HttpDownloadHelper(IWebOperation httpWebOperation, string LocalFilePath, string Username, string Password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         if (String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         _HttpWebOperation = httpWebOperation;
         _HttpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _HttpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_HttpWebOperation.OperationRequest.Request, Username, Password);
         SetProxy(_HttpWebOperation.OperationRequest.Request);

         _HttpWebOperation.Download(LocalFilePath);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public long GetHttpDownloadLength(string Url, string Username, string Password)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         return GetHttpDownloadLength(new Uri(Url), Username, Password);
      }
      
      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="ResourceUri">Web Resource Uri.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetHttpDownloadLength(Uri ResourceUri, string Username, string Password)
      {
         if (ResourceUri == null) throw new ArgumentNullException("ResourceUri", "Argument 'ResourceUri' cannot be null.");
      
         return GetHttpDownloadLength(WebOperation.Create(ResourceUri), Username, Password);
      }

      /// <summary>
      /// Get the Length of the Http Download.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public long GetHttpDownloadLength(IWebOperation httpWebOperation, string Username, string Password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         _HttpWebOperation = httpWebOperation;
         _HttpWebOperation.WebOperationProgress += OnHttpWebOperationProgress;
         _HttpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(_HttpWebOperation.OperationRequest.Request, Username, Password);
         SetProxy(_HttpWebOperation.OperationRequest.Request);

         return _HttpWebOperation.GetDownloadLength();
      }
      
      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server or FtpPath is Null or Empty.</exception>
      public void FtpCheckConnection(string Server, string FtpPath, string Username, string Password, FtpType ftpMode)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath))
         {
            throw new ArgumentException("Arguments 'Server' and 'FtpPath' cannot be a null or empty string.");
         }

         FtpWebOperation ftpWebOperation = (FtpWebOperation)WebOperation.Create(new Uri(String.Format(CultureInfo.InvariantCulture, "ftp://{0}{1}", Server, FtpPath)));
         FtpCheckConnection(ftpWebOperation, Username, Password, ftpMode);
      }

      /// <summary>
      /// Check an FTP Connection.
      /// </summary>
      /// <param name="ftpWebOperation">Web Operation.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="ftpMode">Ftp Transfer Mode.</param>
      /// <exception cref="ArgumentException">Throws if ftpWebOperation is Null.</exception>
      public void FtpCheckConnection(IFtpWebOperation ftpWebOperation, string Username, string Password, FtpType ftpMode)
      {
         if (ftpWebOperation == null) throw new ArgumentNullException("ftpWebOperation", "Argument 'ftpWebOperation' cannot be null.");

         ftpWebOperation.FtpOperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         ftpWebOperation.FtpOperationRequest.KeepAlive = false; // Close the Request
         ftpWebOperation.FtpOperationRequest.Timeout = 5000; // 5 second timeout
         SetFtpMode(ftpWebOperation.FtpOperationRequest, ftpMode);

         SetNetworkCredentials(ftpWebOperation.OperationRequest.Request, Username, Password);

         ftpWebOperation.CheckConnection();
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public void HttpCheckConnection(string Url, string Username, string Password)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         WebOperation webOperation = WebOperation.Create(new Uri(Url));

         webOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         webOperation.OperationRequest.Timeout = 5000; // 5 second timeout

         SetNetworkCredentials(webOperation.OperationRequest.Request, Username, Password);
         SetProxy(webOperation.OperationRequest.Request);

         webOperation.CheckConnection();
      }

      /// <summary>
      /// Check an HTTP Connection.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentException">Throws if httpWebOperation is Null.</exception>
      public void HttpCheckConnection(IWebOperation httpWebOperation, string Username, string Password)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");

         httpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         httpWebOperation.OperationRequest.Timeout = 5000; // 5 second timeout

         SetNetworkCredentials(httpWebOperation.OperationRequest.Request, Username, Password);
         SetProxy(httpWebOperation.OperationRequest.Request);

         httpWebOperation.CheckConnection();
      }

      private static void SetFtpMode(IFtpWebOperationRequest Request, FtpType ftpMode)
      {
         Debug.Assert(Request != null);
      
         switch (ftpMode)
         {
            case FtpType.Passive:
               Request.UsePassive = true;
               break;
            case FtpType.Active:
               Request.UsePassive = false;
               break;
            default:
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "FTP Type '{0}' is not valid.", ftpMode));
         }
      }
      
      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public string GetProteinDescription(string Url)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         // Stub out if the given URL is an Unassigned Description
         if (Url.Equals(Constants.UnassignedDescription)) return Url;
      
         return GetProteinDescription(WebOperation.Create(new Uri(Url)));
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="httpWebOperation">Web Operation.</param>
      /// <exception cref="ArgumentNullException">Throws if ResourceUri is Null.</exception>
      public string GetProteinDescription(IWebOperation httpWebOperation)
      {
         if (httpWebOperation == null) throw new ArgumentNullException("httpWebOperation", "Argument 'httpWebOperation' cannot be null.");
      
         string str;

         try
         {
            string tempPath = Path.Combine(Path.GetTempPath(), "protein.html");

            httpWebOperation.OperationRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            httpWebOperation.OperationRequest.Timeout = 5000; // 5 second timeout
            httpWebOperation.Download(tempPath);

            str = GetProteinDescriptionFromFile(tempPath);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
            str = httpWebOperation.OperationRequest.Request.RequestUri.AbsoluteUri;
         }

         return str;
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="FilePath">Web Operation.</param>
      /// <exception cref="ArgumentNullException">Throws if Request is Null.</exception>
      public static string GetProteinDescriptionFromFile(string FilePath)
      {
         Debug.Assert(String.IsNullOrEmpty(FilePath) == false);

         string str;
         string str2;
         string str3;

         using (StreamReader stream = File.OpenText(FilePath))
         {
            str = stream.ReadToEnd();
         }
         
         // FxCop: CA1307 (Specify StringComparison)
         //        CA1309 (For non-linguistic comparisons, StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase)

         // Get the <TABLE> to </TABLE> section of the HTML
         int index = str.IndexOf("<TABLE", StringComparison.Ordinal);
         int length = str.LastIndexOf("</TABLE>");
         str = str.Substring(index, (length - index) + 8);
         
         // Strip the <FORM> to </FORM> section from the HTML
         length = str.IndexOf("<FORM ", StringComparison.Ordinal);
         index = str.IndexOf("</FORM>", StringComparison.Ordinal);
         
         if ((index >= 0) && (length >= 0))
         {
            str2 = str.Substring(0, length);
            str3 = str.Substring(index + 7);
            str = String.Concat(str2, str3);
         }

         // Change the <font> tag size to 3
         index = str.IndexOf("<font", StringComparison.Ordinal);
         length = str.IndexOf(">", index, StringComparison.Ordinal);
         
         if ((index >= 0) && (length >= 0))
         {
            str2 = str.Substring(0, index);
            str3 = str.Substring(length + 1);
            str = String.Concat(str2, "<font size=\"3\">", str3);
         }
         
         // Remove the <p> tag that doesn't conform to HTML 4.01 Transitional
         str = str.Replace("<p align=left>", String.Empty);

         return str;
      }

      /// <summary>
      /// Sends an e-mail message
      /// </summary>
      public static void SendEmail(bool EnableSsl, string MessageFrom, string MessageTo, string MessageSubject, string MessageBody, string SmtpHost, int SmtpPort, string SmtpHostUsername, string SmtpHostPassword)
      {
         MailMessage message = new MailMessage(MessageFrom, MessageTo, MessageSubject, MessageBody);
         SmtpClient client = new SmtpClient(SmtpHost, SmtpPort);
         client.Credentials = GetNetworkCredential(SmtpHostUsername, SmtpHostPassword);
         client.EnableSsl = EnableSsl;
         client.Send(message);
      }

      /// <summary>
      /// Set WebRequest Username and Password.
      /// </summary>
      /// <param name="Request">Makes a request to a Uniform Resource Identifier (URI).</param>
      /// <param name="Username">Login Username.</param>
      /// <param name="Password">Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Request is Null.</exception>
      public static void SetNetworkCredentials(WebRequest Request, string Username, string Password)
      {
         if (Request == null) throw new ArgumentNullException("Request", "Argument 'Request' cannot be null.");

         NetworkCredential credentials = GetNetworkCredential(Username, Password);
         if (credentials != null)
         {
            Request.Credentials = credentials;
         }
      }

      /// <summary>
      /// Set Proxy Information on WebRequest.
      /// </summary>
      /// <param name="Request">Makes a request to a Uniform Resource Identifier (URI).</param>
      private static void SetProxy(WebRequest Request)
      {
         Debug.Assert(Request != null);
         IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
      
         if (Prefs.GetPreference<bool>(Preference.UseProxy))
         {
            Request.Proxy = new WebProxy(Prefs.GetPreference<string>(Preference.ProxyServer), 
                                         Prefs.GetPreference<int>(Preference.ProxyPort));
            if (Prefs.GetPreference<bool>(Preference.UseProxyAuth))
            {
               Request.Proxy.Credentials = GetNetworkCredential(Prefs.GetPreference<string>(Preference.ProxyUser), 
                                                                Prefs.GetPreference<string>(Preference.ProxyPass));
            }
         }
         // Don't set request.Proxy = null - Issue 49
      }
      
      /// <summary>
      /// Creates and Returns a new Network Credential object.
      /// </summary>
      /// <param name="Username">Network Credential Username or Domain\Username</param>
      /// <param name="Password">Network Credential Password</param>
      private static NetworkCredential GetNetworkCredential(string Username, string Password)
      {
         if (StringOps.ValidateUsernamePasswordPair(Username, Password))
         {
            if (Username.Contains("\\"))
            {
               String[] UserParts = Username.Split('\\');
               return new NetworkCredential(UserParts[1], Password, UserParts[0]);
            }
            else
            {
               return new NetworkCredential(Username, Password);
            }
         }
         
         return null;
      }
   }
}
