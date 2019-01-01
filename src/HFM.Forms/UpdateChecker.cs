/*
 * harlam357.Net - Update Checker Class
 * Copyright (C) 2010-2014 Ryan Harlamert (harlam357)
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
 
using System;
using System.IO;
using System.Net;
using System.Net.Cache;

using HFM.Core;

namespace HFM.Forms
{
   public class UpdateChecker
   {
      private const string ApplicationUpdateXml = "ApplicationUpdate.xml";

      public ApplicationUpdate CheckForUpdate(string applicationId)
      {
         return CheckForUpdate(applicationId, Properties.Settings.Default.UpdateUrl, null);
      }

      public ApplicationUpdate CheckForUpdate(string applicationId, IWebProxy proxy)
      {
         return CheckForUpdate(applicationId, Properties.Settings.Default.UpdateUrl, proxy);
      }

      public ApplicationUpdate CheckForUpdate(string applicationId, string updateUrl)
      {
         return CheckForUpdate(applicationId, updateUrl, null);
      }

      public ApplicationUpdate CheckForUpdate(string applicationId, string updateUrl, IWebProxy proxy)
      {
         string localFilePath = Path.Combine(Path.GetTempPath(), String.Concat(applicationId, ApplicationUpdateXml));
         
         IWebOperation web = WebOperation.Create(updateUrl);
         if (proxy != null) web.WebRequest.Proxy = proxy;
         web.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         web.Download(localFilePath);
         return ApplicationUpdateSerializer.DeserializeFromXml(localFilePath);
      }
   }
}
