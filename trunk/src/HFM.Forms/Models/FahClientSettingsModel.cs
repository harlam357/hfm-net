/*
 * HFM.NET - FAH Client Settings Model
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.ComponentModel;

using HFM.Client.DataTypes;
using HFM.Core;

namespace HFM.Forms.Models
{
   public class FahClientSettingsModel : INotifyPropertyChanged
   {
      public FahClientSettingsModel()
      {
         _slots = new List<FahClientSettingsSlotModel>();
      }

      public bool Error
      {
         get
         {
            return NameError ||
                   ServerError ||
                   PortError;
                   //PasswordError;
         }
      }

      private string _name = String.Empty;

      public string Name
      {
         get { return _name; }
         set
         {
            if (_name != value)
            {
               _name = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Name");
            }
         }
      }

      public bool NameEmpty
      {
         get { return Name.Length == 0; }
      }

      public bool NameError
      {
         get { return !Validate.ClientName(Name); }
      }

      private string _server = String.Empty;

      public string Server
      {
         get { return _server; }
         set
         {
            if (_server != value)
            {
               _server = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Server");
            }
         }
      }

      public bool ServerError
      {
         get { return !Validate.ServerName(Server); }
      }

      private int _port = Constants.DefaultFahClientPort;

      public int Port
      {
         get { return _port; }
         set
         {
            if (_port != value)
            {
               _port = value;
               OnPropertyChanged("Port");               
            }
         }
      }

      public bool PortError
      {
         get { return !Validate.ServerPort(Port); }
      }

      private string _password = String.Empty;

      public string Password
      {
         get { return _password; }
         set
         {
            if (_password != value)
            {
               _password = value == null ? String.Empty : value.Trim();
               OnPropertyChanged("Password");
            }
         }
      }

      //public bool PasswordError
      //{
      //   get { return Password.Length == 0; }
      //}

      private readonly List<FahClientSettingsSlotModel> _slots;

      public IEnumerable<FahClientSettingsSlotModel> Slots
      {
         get { return _slots.AsReadOnly(); }
      }

      public void RefreshSlots(SlotCollection slots)
      {
         _slots.Clear();
         foreach (var slot in slots)
         {
            _slots.Add(new FahClientSettingsSlotModel { ID = slot.Id, SlotType = slot.SlotOptions.FahClientSubTypeEnum.ToString() });
         }
         OnPropertyChanged("Slots");
      }

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged(string propertyName)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      #endregion
   }

   public class FahClientSettingsSlotModel
   {
      public int ID { get; set; }

      public string SlotType { get; set; }
   }
}
