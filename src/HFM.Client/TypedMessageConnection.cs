/*
 * HFM.NET - TypedMessageConnection Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

using HFM.Client.DataTypes;

namespace HFM.Client
{
   /// <summary>
   /// Folding@Home client typed message connection class.  Provides functionality for parsing JSON messages into strongly typed objects.
   /// </summary>
   public class TypedMessageConnection : JsonMessageConnection
   {
      private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>
                                                                 {
                                                                    { MessageKey.Heartbeat, typeof(Heartbeat) },
                                                                    { MessageKey.Info, typeof(Info) },
                                                                    { MessageKey.Options, typeof(Options) },
                                                                    { MessageKey.SimulationInfo, typeof(SimulationInfo) },
                                                                    { MessageKey.SlotInfo, typeof(SlotCollection) },
                                                                    { MessageKey.SlotOptions, typeof(SlotOptions) },
                                                                    { MessageKey.QueueInfo, typeof(UnitCollection) },
                                                                    { MessageKey.LogRestart, typeof(LogRestart) },
                                                                    { MessageKey.LogUpdate, typeof(LogUpdate) }
                                                                 };

      /// <summary>
      /// Initializes a new instance of the TypedMessageConnection class.
      /// </summary>
      [CoverageExclude]
      public TypedMessageConnection()
      {

      }

      /// <summary>
      /// Raise the MessageReceived event.
      /// </summary>
      /// <param name="e">Event arguments (if null the event is cancelled).</param>
      protected override void OnMessageReceived(MessageReceivedEventArgs e)
      {
         if (e == null) return;

         var dataType = TypeMap.ContainsKey(e.JsonMessage.Key) ? TypeMap[e.JsonMessage.Key] : null;
         if (dataType != null)
         {
            var typedMessage = (TypedMessage)Activator.CreateInstance(dataType);
            typedMessage.Fill(e.JsonMessage);
            e.TypedMessage = typedMessage;
         }
         base.OnMessageReceived(e);
      }
   }
}
