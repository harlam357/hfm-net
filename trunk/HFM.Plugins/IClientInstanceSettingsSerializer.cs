/*
 * HFM.NET - Client Instance Settings Serializer Interface
 * Copyright (C) 2010 Ryan Harlamert (harlam357)
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

namespace HFM.Plugins
{
   public interface IClientInstanceSettingsSerializer
   {
      event EventHandler<SettingsSerializerMessageEventArgs> WarningMessage;
      
      string FileExtension { get; }
      
      string FileTypeFilter { get; }
      
      IInstanceCollectionDataInterface DataInterface { set; }

      /// <summary>
      /// Serialize a collection of Client Instances to disk
      /// </summary>
      /// <param name="fileName">Path of File to Serialize</param>
      void Serialize(string fileName);

      /// <summary>
      /// Loads a collection of Client Instances from disk
      /// </summary>
      /// <param name="fileName">Path of File to Deserialize</param>
      void Deserialize(string fileName);
   }
   
   public class SettingsSerializerMessageEventArgs : EventArgs
   {
      private string _message;
   
      public string Message
      {
         get { return _message; }
         set { _message = value == null ? String.Empty : value.Trim(); }
      }
      
      public SettingsSerializerMessageEventArgs()
      {
         Message = String.Empty;
      }
      
      public SettingsSerializerMessageEventArgs(string message)
      {
         Message = message;
      }
   }
}
