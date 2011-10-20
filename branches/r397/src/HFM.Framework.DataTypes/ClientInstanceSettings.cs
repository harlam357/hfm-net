/*
 * HFM.NET - Client Instance Settings Class
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
using System.Globalization;

using ProtoBuf;

namespace HFM.Framework.DataTypes
{
   public interface IClientInstanceSettings
   {
      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      InstanceType InstanceHostType { get; }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      string InstanceName { get; }

      bool ExternalInstance { get; }

      /// <summary>
      /// External data file name
      /// </summary>
      string RemoteExternalFilename { get; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      int ClientProcessorMegahertz { get; }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      string RemoteFAHLogFilename { get; }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      string RemoteUnitInfoFilename { get; }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      string RemoteQueueFilename { get; }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      string Path { get; }

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      string Server { get; }

      /// <summary>
      /// Username on remote server
      /// </summary>
      string Username { get; }

      /// <summary>
      /// Password on remote server
      /// </summary>
      string Password { get; }

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      FtpType FtpMode { get; }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      bool ClientIsOnVirtualMachine { get; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      int ClientTimeOffset { get; }

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      string CachedFahLogName { get; }

      /// <summary>
      /// Cached UnitInfo Filename for this instance
      /// </summary>
      string CachedUnitInfoName { get; }

      /// <summary>
      /// Cached Queue Filename for this instance
      /// </summary>
      string CachedQueueName { get; }

      /// <summary>
      /// Cached External Filename for this instance
      /// </summary>
      string CachedExternalName { get; }
   }

   [ProtoContract]
   public class ClientInstanceSettings : IClientInstanceSettings
   {
      public string ImportError { get; set; }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      [ProtoMember(1)]
      public InstanceType InstanceHostType { get; set; }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      [ProtoMember(2)]
      public string InstanceName { get; set; }

      [ProtoMember(14)]
      public bool ExternalInstance { get; set; }

      /// <summary>
      /// External data file name
      /// </summary>
      [ProtoMember(15)]
      public string RemoteExternalFilename { get; set; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      [ProtoMember(3)]
      public int ClientProcessorMegahertz { get; set; }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      [ProtoMember(4)]
      public string RemoteFAHLogFilename { get; set; }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      [ProtoMember(5)]
      public string RemoteUnitInfoFilename { get; set; }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      [ProtoMember(6)]
      public string RemoteQueueFilename { get; set; }

      private string _path;
      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      [ProtoMember(7)]
      public string Path
      {
         get { return _path; }
         set 
         { 
            _path = value;
            if (_path != null)
            {
               switch (InstanceHostType)
               {
                  case InstanceType.PathInstance:
                     // if the path is of sufficient length but does not
                     // end with a directory separator character (for any
                     // filesystem), then append the current platform
                     // separator character
                     if (_path.Length > 2 &&
                        (_path.EndsWith("\\", StringComparison.OrdinalIgnoreCase) ||
                         _path.EndsWith("/", StringComparison.OrdinalIgnoreCase)) == false)
                     {
                        _path = String.Concat(_path, System.IO.Path.DirectorySeparatorChar);
                     }
                     break;
                  case InstanceType.HttpInstance:
                  case InstanceType.FtpInstance:
                     if (_path.EndsWith("/", StringComparison.OrdinalIgnoreCase) == false)
                     {
                        _path = String.Concat(_path, "/");
                     }
                     break;
               }
            }
         }
      }

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      [ProtoMember(8)]
      public string Server { get; set; }

      /// <summary>
      /// Username on remote server
      /// </summary>
      [ProtoMember(9)]
      public string Username { get; set; }

      /// <summary>
      /// Password on remote server
      /// </summary>
      [ProtoMember(10)]
      public string Password { get; set; }

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      [ProtoMember(11)]
      public FtpType FtpMode { get; set; }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      [ProtoMember(12)]
      public bool ClientIsOnVirtualMachine { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      [ProtoMember(13)]
      public int ClientTimeOffset { get; set; }
      
      public ClientInstanceSettings()
         : this(InstanceType.PathInstance)
      {
         
      }
      
      public ClientInstanceSettings(InstanceType hostType)
      {
         ImportError = String.Empty;
         InstanceHostType = hostType;
         InstanceName = String.Empty;
         ExternalInstance = false;
         RemoteExternalFilename = Default.ExternalDataFileName;
         ClientProcessorMegahertz = 1;
         RemoteFAHLogFilename = Default.FahLogFileName;
         RemoteUnitInfoFilename = Default.UnitInfoFileName;
         RemoteQueueFilename = Default.QueueFileName;
         Path = String.Empty;
         Server = String.Empty;
         Username = String.Empty;
         Password = String.Empty;
         FtpMode = FtpType.Passive;
         ClientIsOnVirtualMachine = false;
         ClientTimeOffset = 0;
      }

      public ClientInstanceSettings DeepClone()
      {
         return Serializer.DeepClone(this);
      }

      #region Cached Log File Name Properties

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      public string CachedFahLogName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Default.FahLogFileName); }
      }

      /// <summary>
      /// Cached UnitInfo Filename for this instance
      /// </summary>
      public string CachedUnitInfoName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Default.UnitInfoFileName); }
      }

      /// <summary>
      /// Cached Queue Filename for this instance
      /// </summary>
      public string CachedQueueName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Default.QueueFileName); }
      }

      /// <summary>
      /// Cached External Filename for this instance
      /// </summary>
      public string CachedExternalName
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", InstanceName, Default.ExternalDataFileName); }
      }

      #endregion
   }
}
