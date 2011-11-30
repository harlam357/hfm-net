/*
 * HFM.NET - Slot Options Data Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Diagnostics;

using Newtonsoft.Json.Linq;

using HFM.Client.Converters;

namespace HFM.Client.DataTypes
{
   public class SlotOptions : TypedMessage
   {
      #region Properties

      [MessageProperty("client-type")]
      public string ClientType { get; set; }

      [MessageProperty("client-type", typeof(ClientTypeConverter))]
      public FahClientType FahClientTypeEnum { get; set; }

      [MessageProperty("client-subtype")]
      public string ClientSubType { get; set; }

      [MessageProperty("client-subtype", typeof(ClientSubTypeConverter))]
      public FahClientSubType FahClientSubTypeEnum { get; set; }

      [MessageProperty("machine-id")]
      public int? MachineId { get; set; }

      [MessageProperty("max-packet-size")]
      public string MaxPacketSize { get; set; }

      [MessageProperty("max-packet-size", typeof(MaxPacketSizeConverter))]
      public MaxPacketSize MaxPacketSizeEnum { get; set; }

      [MessageProperty("core-priority")]
      public string CorePriority { get; set; }

      [MessageProperty("core-priority", typeof(CorePriorityConverter))]
      public CorePriority CorePriorityEnum { get; set; }

      [MessageProperty("next-unit-percentage")]
      public int? NextUnitPercentage { get; set; }

      [MessageProperty("max-units")]
      public int? MaxUnits { get; set; }

      [MessageProperty("checkpoint")]
      public int? Checkpoint { get; set; }

      [MessageProperty("pause-on-start")]
      public bool? PauseOnStart { get; set; }

      [MessageProperty("gpu-vendor-id")]
      public string GpuVendorId { get; set; }

      [MessageProperty("gpu-device-id")]
      public string GpuDeviceId { get; set; }

      #endregion

      /// <summary>
      /// Fill the SlotOptions object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Debug.Assert(message != null);

         var propertySetter = new MessagePropertySetter(this);
         foreach (var prop in JObject.Parse(message.Value).Properties())
         {
            propertySetter.SetProperty(prop);
         }
         SetMessageValues(message);
      }
   }
}
