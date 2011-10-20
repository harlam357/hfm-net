/*
 * HFM.NET - FahClient Class
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

using System;
using System.Collections.Generic;
using System.Linq;

using HFM.Client.DataTypes;

namespace HFM.Client
{
   public class FahClient : MessageCache
   {
      private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>
                                                                 {
                                                                    { JsonMessageKey.Heartbeat, typeof(Heartbeat) },
                                                                    { JsonMessageKey.Info, typeof(Info) },
                                                                    { JsonMessageKey.Options, typeof(Options) },
                                                                    { JsonMessageKey.SimulationInfo, typeof(SimulationInfo) },
                                                                    { JsonMessageKey.SlotInfo, typeof(SlotCollection) },
                                                                    { JsonMessageKey.SlotOptions, typeof(SlotOptions) },
                                                                    { JsonMessageKey.QueueInfo, typeof(UnitCollection) },
                                                                    { JsonMessageKey.LogRestart, typeof(LogRestart) },
                                                                    { JsonMessageKey.LogUpdate, typeof(LogUpdate) }
                                                                 };

      /// <summary>
      /// Create a FahClient.
      /// </summary>
      public FahClient()
         : this(new TcpClientFactory())
      {

      }

      /// <summary>
      /// Create a FahClient.
      /// </summary>
      internal FahClient(ITcpClientFactory tcpClientFactory)
         : base(tcpClientFactory)
      {

      }

      /// <summary>
      /// Get a Typed Server Message.  Returns null if the message is not in the cache.
      /// </summary>
      public T GetMessage<T>() where T : TypedMessage, new()
      {
         var jsonMessage = GetJsonMessage(GetKey(typeof(T)));
         if (jsonMessage != null)
         {
            var typedMessage = Activator.CreateInstance<T>();
            typedMessage.Fill(jsonMessage);
            return typedMessage;
         }

         return null;
      }

      /// <summary>
      /// Get a Typed Server Message.  Returns null if the message is not in the cache.
      /// </summary>
      public T GetMessage<T, TCollectionType>() where T : TypedMessageCollection, new() where TCollectionType : ITypedMessageObject, new()
      {
         var jsonMessage = GetJsonMessage(GetKey(typeof(T)));
         if (jsonMessage != null)
         {
            var typedMessageCollection = Activator.CreateInstance<T>();
            typedMessageCollection.Fill<TCollectionType>(jsonMessage);
            return typedMessageCollection;
         }

         return null;
      }

      private static string GetKey(Type type)
      {
         return TypeMap.FirstOrDefault(x => type.Equals(x.Value) || type.IsSubclassOf(x.Value)).Key;
      }

      /// <summary>
      /// Raise the MessageUpdated Event.
      /// </summary>
      /// <param name="e">Event Arguments (if null the event is cancelled)</param>
      protected override void OnMessageUpdated(MessageUpdatedEventArgs e)
      {
         if (e == null) return;

         e.DataType = TypeMap.ContainsKey(e.Key) ? TypeMap[e.Key] : null;
         base.OnMessageUpdated(e);
      }
   }
}
