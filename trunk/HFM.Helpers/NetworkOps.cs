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
using System.Text;

using HFM.Preferences;
using Debug = HFM.Instrumentation.Debug;

namespace HFM.Helpers
{
   public enum DownloadType
   {
      FAHLog = 0,
      UnitInfo
   }

   public static class NetworkOps
   {
      // Log File Size Constants
      public const int UnitInfoMax = 1048576; // 1 Megabyte

      public static void FtpUploadHelper(string Server, string FtpPath, string LocalFile, string Username, string Password)
      {
         FtpWebRequest ftpc1 = (FtpWebRequest)FtpWebRequest.Create(String.Format("ftp://{0}{1}{2}", Server, FtpPath, Path.GetFileName(LocalFile)));
         ftpc1.Method = WebRequestMethods.Ftp.UploadFile;
         ftpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(ftpc1, Username, Password);
         SetProxy(ftpc1);

         StreamReader sr1 = null;
         Stream requestStream = null;
         try
         {
            sr1 = new StreamReader(LocalFile);
            byte[] fileContents = Encoding.UTF8.GetBytes(sr1.ReadToEnd());
            ftpc1.ContentLength = fileContents.Length;

            requestStream = ftpc1.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
         }
         finally
         {
            if (sr1 != null)
            {
               sr1.Close();
            }
            if (requestStream != null)
            {
               requestStream.Close();
            }
         }
      }

      /// <summary>
      /// Makes the Ftp connection and downloads the specified files
      /// </summary>
      public static bool FtpDownloadHelper(string Server, string FtpPath, string RemoteFile, string LocalFile, string Username, string Password)
      {
         FtpWebRequest ftpc1 = (FtpWebRequest)FtpWebRequest.Create(String.Format("ftp://{0}{1}{2}", Server, FtpPath, RemoteFile));
         ftpc1.Method = WebRequestMethods.Ftp.DownloadFile;
         ftpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(ftpc1, Username, Password);
         SetProxy(ftpc1);

         StreamReader sr1 = null;
         StreamWriter sw1 = null;
         try
         {
            FtpWebResponse ftpr1 = (FtpWebResponse)ftpc1.GetResponse();
            sr1 = new StreamReader(ftpr1.GetResponseStream(), Encoding.ASCII);
            sw1 = new StreamWriter(LocalFile, false);
            sw1.Write(sr1.ReadToEnd());
         }
         finally
         {
            if (sr1 != null)
            {
               sr1.Close();
            }
            if (sw1 != null)
            {
               sw1.Flush();
               sw1.Close();
            }
         }

         return true;
      }

      /// <summary>
      /// Makes the Http connection and downloads the specified files
      /// </summary>
      /// <param name="type">Type of Download (FAHLog or UnitInfo)</param>
      public static bool HttpDownloadHelper(string HttpPath, string LocalFile, string Username, string Password, string InstanceName, DownloadType type)
      {
         WebRequest httpc1 = WebRequest.Create(HttpPath);
         httpc1.Method = WebRequestMethods.Http.Get;
         httpc1.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

         SetNetworkCredentials(httpc1, Username, Password);
         SetProxy(httpc1);

         StreamWriter sw1 = null;
         StreamReader sr1 = null;
         try
         {
            WebResponse r1 = httpc1.GetResponse();
            if (type.Equals(DownloadType.UnitInfo) && r1.ContentLength >= UnitInfoMax)
            {
               if (File.Exists(LocalFile))
               {
                  File.Delete(LocalFile);
               }
               Debug.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} ({1}) UnitInfo HTTP download (file is too big: {2} bytes).", Debug.FunctionName,
                                                     InstanceName, r1.ContentLength));
            }
            else
            {
               sr1 = new StreamReader(r1.GetResponseStream(), Encoding.ASCII);
               sw1 = new StreamWriter(LocalFile, false);
               sw1.Write(sr1.ReadToEnd());
            }
         }
         finally
         {
            if (sr1 != null)
            {
               sr1.Close();
            }
            if (sw1 != null)
            {
               sw1.Flush();
               sw1.Close();
            }
         }

         return true;
      }

      private static void SetNetworkCredentials(WebRequest request, string Username, string Password)
      {
         if (String.IsNullOrEmpty(Username) == false)
         {
            if (Username.Contains("\\"))
            {
               String[] UserParts = Username.Split('\\');
               request.Credentials = new NetworkCredential(UserParts[1], Password, UserParts[0]);
            }
            else
            {
               request.Credentials = new NetworkCredential(Username, Password);
               //ftpc1.Credentials = new NetworkCredential("anonymous", "somepass");
            }
         }
      }

      private static void SetProxy(WebRequest request)
      {
         PreferenceSet Prefs = PreferenceSet.Instance;
         if (Prefs.UseProxy)
         {
            request.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
            if (Prefs.UseProxyAuth)
            {
               request.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
            }
         }
         // Don't set request.Proxy = null - Issue 49
      }
   }
}
