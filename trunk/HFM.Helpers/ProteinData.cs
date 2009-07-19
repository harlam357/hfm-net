using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

using HFM.Preferences;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Helpers
{
   public static class ProteinData
   {
      public static string DescriptionFromURL(string sURL)
      {
         // Stub out if the given URL is an Unassigned Description
         if (sURL.Equals(HFM.Proteins.Protein.UnassignedDescription)) return sURL;
      
         string str;
         PreferenceSet instance = PreferenceSet.Instance;

         try
         {
            WebRequest request = WebRequest.Create(sURL);
            request.Method = "GET";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
            
            if (instance.UseProxy)
            {
               request.Proxy = new WebProxy(instance.ProxyServer, instance.ProxyPort);
               if (instance.UseProxyAuth)
               {
                  request.Proxy.Credentials = new NetworkCredential(instance.ProxyUser, instance.ProxyPass);
               }
            }
            else
            {
               request.Proxy = null;
            }

            string str2;
            string str3;
            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.ASCII);
            str = reader.ReadToEnd();
            reader.Close();
            int index = str.IndexOf("<TABLE");
            int length = str.LastIndexOf("</TABLE>");
            str = str.Substring(index, (length - index) + 8);
            length = str.IndexOf("<FORM ");
            index = str.IndexOf("</FORM>");
            if ((index >= 0) && (length >= 0))
            {
               str2 = str.Substring(0, length);
               str3 = str.Substring(index);
               str = str2 + str3;
            }
            index = str.IndexOf("<font");
            length = str.IndexOf(">", index);
            if ((index >= 0) && (length >= 0))
            {
               str2 = str.Substring(0, index);
               str3 = str.Substring(length + 1);
               str = str2 + "<font size=\"3\">" + str3;
            }
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning,
                                    String.Format("{0} threw exception {1}", Debug.FunctionName, ex.Message));
            str = sURL;
         }
         
         return str;
      }
   }
}
