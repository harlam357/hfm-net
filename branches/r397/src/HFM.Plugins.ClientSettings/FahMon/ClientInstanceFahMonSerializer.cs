/*
 * HFM.NET - Client Instance FahMon Configuration Serializer
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.IO;
using System.Text.RegularExpressions;

using HFM.Framework;

namespace HFM.Plugins.ClientSettings.FahMon
{
   public class ClientInstanceFahMonSerializer : IClientInstanceSettingsSerializer
   {
      private IInstanceCollectionDataInterface _dataInterface;

      /// <summary>
      /// Read FahMon ClientsTab.txt file and import new instance collection
      /// </summary>
      /// <param name="fileName">Path of ClientsTab.txt to import</param>
      public void Deserialize(string fileName)
      {
         StreamReader fileStream = null;
         try
         {
            // Open File
            fileStream = File.OpenText(fileName);

            // Reader Loop
            while (fileStream.Peek() != -1)
            {
               // Get the line and remove whitespace
               string line = fileStream.ReadLine();
               line = line.Trim();

               // Check for commented or empty line
               if (String.IsNullOrEmpty(line) == false && line.StartsWith("#") == false)
               {
                  // Tokenize the line
                  string[] tokens = line.Split(new[] { '\t' });

                  var settingsDataInterface = _dataInterface.GetNewDataInterface();
                  if (tokens.Length > 1) // we should have at least name and path
                  {
                     if (GetInstanceSettings(tokens, settingsDataInterface))
                     {
                        // Check for Client is on Virtual Machine setting
                        if (tokens.Length > 3)
                        {
                           if (tokens[3].Equals("*"))
                           {
                              settingsDataInterface.SetSetting(ClientInstanceSettingsKeys.UtcOffsetIsZero, true);
                           }
                        }
                     }
                     else
                     {
                        _dataInterface.ThrowAwayNewDataInterface(String.Format("Failed to add FahMon Instance: {0}.", line));
                     }
                  }
                  else
                  {
                     _dataInterface.ThrowAwayNewDataInterface(String.Format("Failed to add FahMon Instance (not tab delimited): {0}.", line));
                  }
               }
            }
         }
         catch (Exception ex)
         {
            throw new IOException("Failed to read FahMon client collection.", ex);
         }
         finally
         {
            if (fileStream != null)
            {
               fileStream.Close();
            }
         }
      }

      /// <summary>
      /// Inspects tokens gathered from FahMon ClientsTab.txt line and attempts to
      /// create an HFM ClientInstanceSettings object based on those tokens
      /// </summary>
      /// <param name="tokens">Tokenized String (String Array)</param>
      /// <param name="dataInterface">Client Instance Settings Data Interface</param>
      private static bool GetInstanceSettings(string[] tokens, IClientInstanceSettingsDataInterface dataInterface)
      {
         // Get the instance name token and validate
         string instanceName = tokens[0].Replace("\"", String.Empty);

         // Get the instance path token
         string instancePath = tokens[1].Replace("\"", String.Empty);
         Match matchUrl = StringOps.MatchHttpOrFtpUrl(instancePath);
         Match matchFtpUserPass = StringOps.MatchFtpWithUserPassUrl(instancePath);

         if (matchUrl.Success) // we have a valid URL
         {
            if (instancePath.StartsWith("http"))
            {
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceType, "HttpInstance");
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceName, instanceName);
               dataInterface.SetSetting(ClientInstanceSettingsKeys.Path, instancePath);
            }
            else if (instancePath.StartsWith("ftp"))
            {
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceType, "FtpInstance");
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceName, instanceName);
               dataInterface.SetSetting(ClientInstanceSettingsKeys.Server, matchUrl.Result("${domain}"));
               dataInterface.SetSetting(ClientInstanceSettingsKeys.Path, matchUrl.Result("${file}"));
            }
            else
            {
               return false;
            }
         }
         else if (matchFtpUserPass.Success) // we have a valid FTP with User Pass
         {
            dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceType, "FtpInstance");
            dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceName, instanceName);
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Server, matchFtpUserPass.Result("${domain}"));
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Path, matchFtpUserPass.Result("${file}"));
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Username, matchFtpUserPass.Result("${username}"));
            dataInterface.SetSetting(ClientInstanceSettingsKeys.Password, matchFtpUserPass.Result("${password}"));
         }
         else // try to validate as a path instance
         {
            if (StringOps.ValidatePathInstancePath(instancePath) ||
                StringOps.ValidatePathInstancePath(instancePath += Path.DirectorySeparatorChar))
            {
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceType, "PathInstance");
               dataInterface.SetSetting(ClientInstanceSettingsKeys.InstanceName, instanceName);
               dataInterface.SetSetting(ClientInstanceSettingsKeys.Path, instancePath);
            }
            else
            {
               return false;
            }
         }

         return true;
      }

      public event EventHandler<SettingsSerializerMessageEventArgs> WarningMessage;

      private void OnWarningMessage(string message)
      {
         if (WarningMessage != null)
         {
            WarningMessage(this, new SettingsSerializerMessageEventArgs(message));
         }
      }

      public string FileExtension
      {
         get { return "txt"; }
      }

      public string FileTypeFilter
      {
         get { return "FahMon Configuration Files|*.txt"; }
      }

      public IInstanceCollectionDataInterface DataInterface
      {
         set { _dataInterface = value; }
      }

      public void Serialize(string fileName)
      {
         throw new NotImplementedException("FahMon clientstab.txt serialization is not supported.");
      }
   }
}