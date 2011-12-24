/*
 * HFM.NET - Client Settings Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class ClientSettings
   {
      /// <summary>
      /// Client Type (FahClient, Legacy, or External)
      /// </summary>
      [DataMember(Order = 1)]
      public ClientType ClientType { get; private set; }

      /// <summary>
      /// Sub Type (Path, Http, Ftp, or None)
      /// </summary>
      [DataMember(Order = 2)]
      public LegacyClientSubType LegacyClientSubType { get; set; }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      [DataMember(Order = 3)]
      public string Name { get; set; }

      /// <summary>
      /// Server name or IP Address
      /// </summary>
      [DataMember(Order = 4)]
      public string Server { get; set; }

      /// <summary>
      /// Server port
      /// </summary>
      [DataMember(Order = 5)]
      public int Port { get; set; }

      /// <summary>
      /// Username on remote server
      /// </summary>
      [DataMember(Order = 6)]
      public string Username { get; set; }

      /// <summary>
      /// Password on remote server
      /// </summary>
      [DataMember(Order = 7)]
      public string Password { get; set; }

      #region Legacy Data

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      [DataMember(Order = 8)]
      public int ClientProcessorMegahertz { get; set; }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      [DataMember(Order = 9)]
      public string FahLogFileName { get; set; }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      [DataMember(Order = 10)]
      public string UnitInfoFileName { get; set; }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      [DataMember(Order = 11)]
      public string QueueFileName { get; set; }

      private string _path;
      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      [DataMember(Order = 12)]
      public string Path
      {
         get { return _path; }
         set 
         { 
            _path = value;
            if (_path != null)
            {
               switch (LegacyClientSubType)
               {
                  case LegacyClientSubType.Path:
                     // if the path is of sufficient length but does not
                     // end with a directory separator character (for any
                     // filesystem), then append the current platform
                     // separator character
                     if (_path.Length > 2 &&
                        (_path.EndsWith("\\") ||
                         _path.EndsWith("/")) == false)
                     {
                        _path = String.Concat(_path, System.IO.Path.DirectorySeparatorChar);
                     }
                     break;
                  case LegacyClientSubType.Http:
                  case LegacyClientSubType.Ftp:
                     if (_path.EndsWith("/") == false)
                     {
                        _path = String.Concat(_path, "/");
                     }
                     break;
               }
            }
         }
      }

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      [DataMember(Order = 13)]
      public FtpType FtpMode { get; set; }

      #endregion

      /// <summary>
      /// Specifies that this client reports local time as UTC
      /// </summary>
      [DataMember(Order = 14)]
      public bool UtcOffsetIsZero { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      [DataMember(Order = 15)]
      public int ClientTimeOffset { get; set; }
      
      public ClientSettings()
         : this(ClientType.FahClient)
      {
         
      }
      
      public ClientSettings(ClientType clientType)
      {
         ClientType = clientType;
         LegacyClientSubType = clientType.Equals(ClientType.Legacy) ? LegacyClientSubType.Path : LegacyClientSubType.None;
         Name = String.Empty;
         Server = String.Empty;
         Port = 0;
         Username = String.Empty;
         Password = String.Empty;

         ClientProcessorMegahertz = 1;
         FahLogFileName = Default.FahLogFileName;
         UnitInfoFileName = Default.UnitInfoFileName;
         QueueFileName = Default.QueueFileName;
         Path = String.Empty;
         FtpMode = FtpType.Passive;

         UtcOffsetIsZero = false;
         ClientTimeOffset = 0;
      }

      public ClientSettings DeepClone()
      {
         return ProtoBuf.Serializer.DeepClone(this);
      }
   }
}