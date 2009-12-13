/*
 * HFM.NET - Network Operations Helper Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Text;

using HFM.Preferences;
using HFM.Instrumentation;

namespace HFM.Helpers
{
   /// <summary>
   /// Log Download Type
   /// </summary>
   public enum DownloadType
   {
      ASCII = 0,
      Binary,
      UnitInfo
   }

   /// <summary>
   /// Network Operations Class
   /// </summary>
   public static class NetworkOps
   {
      /// <summary>
      /// UnitInfo Log File Maximum Download Size.
      /// </summary>
      public const int UnitInfoMax = 1048576; // 1 Megabyte

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, or LocalFilePath is Null or Empty.</exception>
      public static void FtpUploadHelper(string Server, string FtpPath, string LocalFilePath, string Username, string Password)
      {
         FtpUploadHelper(Server, FtpPath, LocalFilePath, Username, Password, true);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="Server">Server Name or IP.</param>
      /// <param name="FtpPath">Path to upload to on remote Ftp server.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="PassiveMode">Passive FTP Mode.</param>
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, or LocalFilePath is Null or Empty.</exception>
      public static void FtpUploadHelper(string Server, string FtpPath, string LocalFilePath, string Username, string Password, bool PassiveMode)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'LocalFilePath' cannot be a null or empty string.");
         }
         
         FtpUploadHelper((FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format(CultureInfo.InvariantCulture, 
            "ftp://{0}{1}{2}", Server, FtpPath, Path.GetFileName(LocalFilePath)))), LocalFilePath, Username, Password, PassiveMode);
      }

      /// <summary>
      /// Upload a File via Ftp.
      /// </summary>
      /// <param name="Request">Ftp Web Request.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="PassiveMode">Passive FTP Mode.</param>
      /// <exception cref="ArgumentException">Throws if LocalFilePath is Null or Empty.</exception>
      public static void FtpUploadHelper(FtpWebRequest Request, string LocalFilePath, string Username, string Password, bool PassiveMode)
      {
         if (String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }
      
         Request.Method = WebRequestMethods.Ftp.UploadFile;
         Request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         Request.UsePassive = PassiveMode;

         SetNetworkCredentials(Request, Username, Password);
         // Don't Set Proxy on FtpWebRequest, Proxy is for Http calls only.
         //SetProxy(request);
         
         using (StreamReader sr1 = new StreamReader(LocalFilePath))
         {
            byte[] fileContents = Encoding.UTF8.GetBytes(sr1.ReadToEnd());
            //request.ContentLength = fileContents.Length;

            using (Stream requestStream = Request.GetRequestStream())
            {
               requestStream.Write(fileContents, 0, fileContents.Length);
            }
         }
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
      /// <param name="type">Type of Download.</param>
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, RemoteFileName, or LocalFilePath is Null or Empty.</exception>
      public static void FtpDownloadHelper(string Server, string FtpPath, string RemoteFileName, string LocalFilePath, string Username, string Password, DownloadType type)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(RemoteFileName))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', and 'RemoteFileName'cannot be a null or empty string.");
         }
      
         FtpDownloadHelper((FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format(CultureInfo.InvariantCulture, 
            "ftp://{0}{1}{2}", Server, FtpPath, RemoteFileName))), LocalFilePath, Username, Password, type);
      }

      /// <summary>
      /// Download a File via Ftp.
      /// </summary>
      /// <param name="Request">Ftp Web Request.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="Username">Ftp Login Username.</param>
      /// <param name="Password">Ftp Login Password.</param>
      /// <param name="type">Type of Download.</param>
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, RemoteFileName, or LocalFilePath is Null or Empty.</exception>
      public static void FtpDownloadHelper(FtpWebRequest Request, string LocalFilePath, string Username, string Password, DownloadType type)
      {
         if (String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Argument 'LocalFilePath' cannot be a null or empty string.");
         }

         Request.Method = WebRequestMethods.Ftp.DownloadFile;
         Request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(Request, Username, Password);
         // Don't Set Proxy on FtpWebRequest, Proxy is for Http calls only.
         //SetProxy(request);

         FtpWebResponse ftpr1 = (FtpWebResponse)Request.GetResponse();

         if (type.Equals(DownloadType.Binary))
         {
            using (Stream sr1 = ftpr1.GetResponseStream())
            {
               using (Stream sw1 = File.Create(LocalFilePath))
               {
                  byte[] buffer = new byte[1024];
                  int bytesRead;

                  do
                  {
                     bytesRead = sr1.Read(buffer, 0, buffer.Length);
                     sw1.Write(buffer, 0, bytesRead);
                  }
                  while (bytesRead > 0);
               }
            }
         }
         else
         {
            using (StreamReader sr1 = new StreamReader(ftpr1.GetResponseStream(), Encoding.ASCII))
            {
               using (StreamWriter sw1 = new StreamWriter(LocalFilePath, false))
               {
                  sw1.Write(sr1.ReadToEnd());
               }
            }
         }
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      public static Stream HttpDownloadHelper(Uri Url)
      {
         if (Url == null) throw new ArgumentNullException("Url", "Argument 'Url' cannot be null.");
      
         WebResponse r1 = HttpDownloadHelper(WebRequest.Create(Url), null, null);
         return r1.GetResponseStream();
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="InstanceName">Name of the Instance object that called this method.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <param name="type">Type of Download.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public static void HttpDownloadHelper(string Url, string LocalFilePath, string InstanceName, string Username, string Password, DownloadType type)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }
         
         HttpDownloadHelper(WebRequest.Create(Url), LocalFilePath, InstanceName, Username, Password, type);
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="Request">Web Request.</param>
      /// <param name="LocalFilePath">Path to local file.</param>
      /// <param name="InstanceName">Name of the Instance object that called this method.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <param name="type">Type of Download.</param>
      /// <exception cref="ArgumentException">Throws if Url, LocalFilePath, or InstanceName is Null or Empty.</exception>
      public static void HttpDownloadHelper(WebRequest Request, string LocalFilePath, string InstanceName, string Username, string Password, DownloadType type)
      {
         if (String.IsNullOrEmpty(LocalFilePath) || String.IsNullOrEmpty(InstanceName))
         {
            throw new ArgumentException("Arguments 'LocalFilePath' and 'InstanceName' cannot be a null or empty string.");
         }
      
         WebResponse r1 = HttpDownloadHelper(Request, Username, Password);
         if (type.Equals(DownloadType.UnitInfo) && r1.ContentLength >= UnitInfoMax)
         {
            if (File.Exists(LocalFilePath))
            {
               File.Delete(LocalFilePath);
            }
            
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, String.Format(CultureInfo.CurrentCulture, 
               "Http download (file is too big: {0} bytes).", r1.ContentLength));
         }
         else if (type.Equals(DownloadType.ASCII) || type.Equals(DownloadType.UnitInfo))
         {
            using (StreamReader sr1 = new StreamReader(r1.GetResponseStream(), Encoding.ASCII))
            {
               using (StreamWriter sw1 = new StreamWriter(LocalFilePath, false))
               {
                  sw1.Write(sr1.ReadToEnd());
               }
            }
         }
         else if (type.Equals(DownloadType.Binary))
         {
            using (Stream sr1 = r1.GetResponseStream())
            {
               using (Stream sw1 = File.Create(LocalFilePath))
               {
                  byte[] buffer = new byte[1024];
                  int bytesRead;

                  do
                  {
                     bytesRead = sr1.Read(buffer, 0, buffer.Length);
                     sw1.Write(buffer, 0, bytesRead);
                  }
                  while (bytesRead > 0);
               }
            }
         }
      }

      /// <summary>
      /// Download a File via Http.
      /// </summary>
      /// <param name="Request">Web Request.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      private static WebResponse HttpDownloadHelper(WebRequest Request, string Username, string Password)
      {
         if (Request == null) throw new ArgumentNullException("Request", "Argument 'Request' cannot be null.");

         Request.Method = WebRequestMethods.Http.Get;
         Request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(Request, Username, Password);
         SetProxy(Request);

         return Request.GetResponse();
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public static string GetProteinDescription(string Url)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         // Stub out if the given URL is an Unassigned Description
         if (Url.Equals(PreferenceSet.UnassignedDescription)) return Url;
      
         return GetProteinDescription(new Uri(Url));
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      public static string GetProteinDescription(Uri Url)
      {
         return GetProteinDescription(Url, String.Empty, String.Empty);
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      public static string GetProteinDescription(Uri Url, string Username, string Password)
      {
         if (Url == null) throw new ArgumentNullException("Url", "Argument 'Url' cannot be null.");
      
         string str;
         
         try
         {
            str = GetProteinDescription(WebRequest.Create(Url), Username, Password);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
            str = Url.AbsoluteUri;
         }
         
         return str;
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Request">Web Request.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Request is Null.</exception>
      public static string GetProteinDescription(WebRequest Request, string Username, string Password)
      {
         if (Request == null) throw new ArgumentNullException("Request", "Argument 'Request' cannot be null.");

         string str;
         string str2;
         string str3;

         using (StreamReader reader = new StreamReader(HttpDownloadHelper(Request, Username, Password).GetResponseStream(), Encoding.ASCII))
         {
            str = reader.ReadToEnd();
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
      /// <param name="MessageFrom"></param>
      /// <param name="MessageTo"></param>
      /// <param name="MessageSubject"></param>
      /// <param name="MessageBody"></param>
      /// <param name="SmtpHost"></param>
      /// <param name="SmtpHostUsername"></param>
      /// <param name="SmtpHostPassword"></param>
      public static void SendEmail(string MessageFrom, string MessageTo, string MessageSubject, string MessageBody, string SmtpHost, string SmtpHostUsername, string SmtpHostPassword)
      {
         MailMessage message = new MailMessage(MessageFrom, MessageTo, MessageSubject, MessageBody);
         SmtpClient client = new SmtpClient(SmtpHost);
         client.Credentials = GetNetworkCredential(SmtpHostUsername, SmtpHostPassword);
         client.EnableSsl = true;

         try
         {
            client.Send(message);
         }
         catch (SmtpException ex) // try again with SSL off
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Trying again with SSL disabled...", true);
            
            client.EnableSsl = false;
            client.Send(message);
         }
      }

      /// <summary>
      /// Set WebRequest Username and Password.
      /// </summary>
      /// <param name="Request">Makes a request to a Uniform Resource Identifier (URI).</param>
      /// <param name="Username">Login Username.</param>
      /// <param name="Password">Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Request is Null.</exception>
      /// <exception cref="ArgumentException">Throws if either Username or Password is an Empty String but not the other.</exception>
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
      
         PreferenceSet Prefs = PreferenceSet.Instance;
         if (Prefs.UseProxy)
         {
            Request.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
            if (Prefs.UseProxyAuth)
            {
               Request.Proxy.Credentials = GetNetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
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
