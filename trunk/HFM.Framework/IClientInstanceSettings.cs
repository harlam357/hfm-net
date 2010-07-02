/*
 * HFM.NET - Client Instance Settings Interface
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
using System.ComponentModel;

namespace HFM.Framework
{
   public interface IClientInstanceSettings : INotifyPropertyChanged
   {
      bool Error { get; }
   
      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      InstanceType InstanceHostType { get; set; }

      /// <summary>
      /// The name assigned to this client instance
      /// </summary>
      string InstanceName { get; set; }

      bool InstanceNameEmpty { get; }

      bool InstanceNameError { get; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      Int32 ClientProcessorMegahertz { get; set; }

      bool ClientProcessorMegahertzError { get; }

      /// <summary>
      /// Remote client log file name
      /// </summary>
      string RemoteFAHLogFilename { get; set; }

      bool RemoteFAHLogFilenameError { get; }

      /// <summary>
      /// Remote client unit info log file name
      /// </summary>
      string RemoteUnitInfoFilename { get; set; }

      bool RemoteUnitInfoFilenameError { get; }

      /// <summary>
      /// Remote client queue.dat file name
      /// </summary>
      string RemoteQueueFilename { get; set; }

      bool RemoteQueueFilenameError { get; }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      string Path { get; set; }

      bool PathEmpty { get; }

      bool PathError { get; }

      /// <summary>
      /// FTP Server name or IP Address
      /// </summary>
      string Server { get; set; }

      bool ServerError { get; }

      /// <summary>
      /// Username on remote server
      /// </summary>
      string Username { get; set; }

      /// <summary>
      /// Password on remote server
      /// </summary>
      string Password { get; set; }

      bool CredentialsError { get; }

      string CredentialsErrorMessage { get; }

      /// <summary>
      /// Specifies the FTP Communication Mode for this client
      /// </summary>
      FtpType FtpMode { get; set; }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      bool ClientIsOnVirtualMachine { get; set; }

      /// <summary>
      /// Specifies the number of minutes (+/-) this client's clock differentiates
      /// </summary>
      Int32 ClientTimeOffset { get; set; }

      bool ClientTimeOffsetError { get; }
      
      string Dummy { get; }

      void LoadSettings(IClientInstanceSettings settings);
      
      IClientInstanceSettings Clone();

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
   }
}