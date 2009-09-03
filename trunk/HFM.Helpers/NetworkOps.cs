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
      FAHLog = 0,
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

         FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format("ftp://{0}{1}{2}", Server, FtpPath, Path.GetFileName(LocalFilePath))));
         request.Method = WebRequestMethods.Ftp.UploadFile;
         request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         request.UsePassive = PassiveMode;

         SetNetworkCredentials(request, Username, Password);
         // Don't Set Proxy on FtpWebRequest, Proxy is for Http calls only.
         //SetProxy(request);
         
         using (StreamReader sr1 = new StreamReader(LocalFilePath))
         {
            byte[] fileContents = Encoding.UTF8.GetBytes(sr1.ReadToEnd());
            //request.ContentLength = fileContents.Length;
            
            using (Stream requestStream = request.GetRequestStream())
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
      /// <exception cref="ArgumentException">Throws if Server, FtpPath, RemoteFileName, or LocalFilePath is Null or Empty.</exception>
      public static void FtpDownloadHelper(string Server, string FtpPath, string RemoteFileName, string LocalFilePath, string Username, string Password)
      {
         if (String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(FtpPath) || String.IsNullOrEmpty(RemoteFileName) || String.IsNullOrEmpty(LocalFilePath))
         {
            throw new ArgumentException("Arguments 'Server', 'FtpPath', 'RemoteFileName', and 'LocalFilePath' cannot be a null or empty string.");
         }
      
         FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format("ftp://{0}{1}{2}", Server, FtpPath, RemoteFileName)));
         request.Method = WebRequestMethods.Ftp.DownloadFile;
         request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(request, Username, Password);
         // Don't Set Proxy on FtpWebRequest, Proxy is for Http calls only.
         //SetProxy(request);

         FtpWebResponse ftpr1 = (FtpWebResponse)request.GetResponse();

         using (StreamReader sr1 = new StreamReader(ftpr1.GetResponseStream(), Encoding.ASCII))
         {
            using (StreamWriter sw1 = new StreamWriter(LocalFilePath, false))
            {
               sw1.Write(sr1.ReadToEnd());
            }
         }
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
      /// <exception cref="ArgumentException">Throws if Url, LocalFilePath, or InstanceName is Null or Empty.</exception>
      public static void HttpDownloadHelper(string Url, string LocalFilePath, string InstanceName, string Username, string Password, DownloadType type)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }
      
         HttpDownloadHelper(new Uri(Url), LocalFilePath, InstanceName, Username, Password, type);
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
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LocalFilePath or InstanceName is Null or Empty.</exception>
      public static void HttpDownloadHelper(Uri Url, string LocalFilePath, string InstanceName, string Username, string Password, DownloadType type)
      {
         if (Url == null) throw new ArgumentNullException("Url", "Argument 'Url' cannot be null.");
      
         if (String.IsNullOrEmpty(LocalFilePath) || String.IsNullOrEmpty(InstanceName))
         {
            throw new ArgumentException("Arguments 'Url', 'LocalFilePath', and 'InstanceName' cannot be a null or empty string.");
         }
      
         WebRequest request = WebRequest.Create(Url);
         request.Method = WebRequestMethods.Http.Get;
         request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(request, Username, Password);
         SetProxy(request);

         WebResponse r1 = request.GetResponse();
         if (type.Equals(DownloadType.UnitInfo) && r1.ContentLength >= UnitInfoMax)
         {
            if (File.Exists(LocalFilePath))
            {
               File.Delete(LocalFilePath);
            }
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) UnitInfo Http download (file is too big: {2} bytes).", HfmTrace.FunctionName,
                                                     InstanceName, r1.ContentLength));
         }
         else
         {
            using (StreamReader sr1 = new StreamReader(r1.GetResponseStream(), Encoding.ASCII))
            {
               using (StreamWriter sw1 = new StreamWriter(LocalFilePath, false))
               {
                  sw1.Write(sr1.ReadToEnd());
               }
            }
         }
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <exception cref="ArgumentException">Throws if Url is Null or Empty.</exception>
      public static string ProteinDescriptionFromUrl(string Url)
      {
         if (String.IsNullOrEmpty(Url))
         {
            throw new ArgumentException("Argument 'Url' cannot be a null or empty string.");
         }

         // Stub out if the given URL is an Unassigned Description
         if (Url.Equals(PreferenceSet.UnassignedDescription)) return Url;
      
         return ProteinDescriptionFromUrl(new Uri(Url));
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      public static string ProteinDescriptionFromUrl(Uri Url)
      {
         return ProteinDescriptionFromUrl(Url, String.Empty, String.Empty);
      }

      /// <summary>
      /// Get the Protein Description from the Url.
      /// </summary>
      /// <param name="Url">Http Url of remote file.</param>
      /// <param name="Username">Http Login Username.</param>
      /// <param name="Password">Http Login Password.</param>
      /// <exception cref="ArgumentNullException">Throws if Url is Null.</exception>
      public static string ProteinDescriptionFromUrl(Uri Url, string Username, string Password)
      {
         if (Url == null) throw new ArgumentNullException("Url", "Argument 'Url' cannot be null.");

         string str;

         try
         {
            WebRequest request = WebRequest.Create(Url);
            request.Method = WebRequestMethods.Http.Get;
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

            SetNetworkCredentials(request, Username, Password);
            SetProxy(request);

            string str2;
            string str3;
            
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.ASCII))
            {
               str = reader.ReadToEnd();
            }

            // FxCop: CA1307 (Specify StringComparison)
            //        CA1309 (For non-linguistic comparisons, StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase)

            int index = str.IndexOf("<TABLE", StringComparison.Ordinal);
            int length = str.LastIndexOf("</TABLE>");
            
            str = str.Substring(index, (length - index) + 8);
            length = str.IndexOf("<FORM ", StringComparison.Ordinal);
            index = str.IndexOf("</FORM>", StringComparison.Ordinal);
            
            if ((index >= 0) && (length >= 0))
            {
               str2 = str.Substring(0, length);
               str3 = str.Substring(index + 7);
               str = str2 + str3;
            }

            index = str.IndexOf("<font", StringComparison.Ordinal);
            length = str.IndexOf(">", index, StringComparison.Ordinal);
            
            if ((index >= 0) && (length >= 0))
            {
               str2 = str.Substring(0, index);
               str3 = str.Substring(length + 1);
               str = str2 + "<font size=\"3\">" + str3;
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
            str = Url.AbsoluteUri;
         }

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
               //return new NetworkCredential("anonymous", "somepass");
            }
         }
         
         return null;
      }
   }
}
