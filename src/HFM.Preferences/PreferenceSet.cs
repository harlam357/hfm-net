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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

using HFM.Preferences.Data;

namespace HFM.Preferences
{
   public class PreferenceSet : PreferenceSetBase
   {
      public PreferenceSet(string applicationPath, string applicationDataFolderPath, string applicationVersion) 
         : base(applicationPath, applicationDataFolderPath, applicationVersion)
      {

      }

      protected override PreferenceData OnMigrateFromUserSettings()
      {
         try
         {
            var data = MigrateFromUserSettings.Execute();
            if (data != null)
            {
               Write(data);
            }
            return data;
         }
         catch (Exception)
         {
            return null;
         }
      }

      protected override IEnumerable<PreferenceUpgrade> OnEnumerateUpgrades()
      {
         yield return new PreferenceUpgrade
         {
            Version = new Version(0, 9, 13),
            Action = data => data.ApplicationSettings.ProjectDownloadUrl = "https://apps.foldingathome.org/psummary.json"
         };
      }

      protected override PreferenceData OnRead()
      {
         string path = Path.Combine(ApplicationDataFolderPath, "config.xml");
         if (File.Exists(path))
         {
            try
            {
               using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
               {
                  var serializer = new DataContractSerializer(typeof(PreferenceData));
                  var data = (PreferenceData)serializer.ReadObject(fileStream);
                  return data;
               }
            }
            catch (Exception)
            {
               return null;
            }
         }
         
         return null;
      }

      private void EnsureApplicationDataFolderExists()
      {
         if (!Directory.Exists(ApplicationDataFolderPath))
         {
            Directory.CreateDirectory(ApplicationDataFolderPath);
         }
      }

      protected override void OnWrite(PreferenceData data)
      {
         EnsureApplicationDataFolderExists();
         string path = Path.Combine(ApplicationDataFolderPath, "config.xml");
         using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
         using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
         {
            var serializer = new DataContractSerializer(typeof(PreferenceData));
            serializer.WriteObject(xmlWriter, data);
         }
      }
   }
}
