/*
 * HFM.NET - FahClient Class
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
   // TODO: Split this interface up and document
   public interface IFahClient : IDisposable
   {
      #region Properties

      bool Connected { get; }

      int ConnectTimeout { get; set; }

      int SendBufferSize { get; set; }

      int ReceiveBufferSize { get; set; }

      double ReceiveLoopTime { get; set; }

      bool UpdateEnabled { get; set; }

      bool DataAvailable { get; }

      #endregion

      #region Events

      event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
      event EventHandler<DataLengthEventArgs> DataLengthReceived;
      event EventHandler<DataLengthEventArgs> DataLengthSent;
      event EventHandler<MessageUpdatedEventArgs> MessageUpdated;
      event EventHandler<StatusMessageEventArgs> StatusMessage;
      event EventHandler UpdateFinished;

      #endregion

      #region Methods

      void Connect(string host, int port, string password);

      void Close();

      void ClearBuffer();

      void SendCommand(string command);

      JsonMessage GetJsonMessage(string key);

      T GetMessage<T>() where T : TypedMessage, new();

      T GetMessage<T, TCollectionType>()
         where T : TypedMessageCollection, new()
         where TCollectionType : ITypedMessageObject, new();

      #endregion
   }

   /// <summary>
   /// Folding@Home client class.  Provides functionality for accessing strongly typed objects that represent the JSON messages returned by the Folding@Home client.
   /// </summary>
   public class FahClient : MessageCache, IFahClient
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
      /// Initializes a new instance of the FahClient class.
      /// </summary>
      [CoverageExclude]
      public FahClient()
         : this(new TcpClientFactory())
      {

      }

      /// <summary>
      /// Initializes a new instance of the FahClient class.
      /// </summary>
      internal FahClient(ITcpClientFactory tcpClientFactory)
         : base(tcpClientFactory)
      {

      }

      /// <summary>
      /// Get a Folding@Home client message as a strongly typed object.  Returns null if the requested message is not available in the cache.
      /// </summary>
      /// <typeparam name="T">Message type to get.</typeparam>
      /// <returns>Message value as a strongly typed object.</returns>
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
      /// Get a Folding@Home client message as a strongly typed object.  Returns null if the requested message is not available in the cache.
      /// </summary>
      /// <typeparam name="T">Collection message type to get.</typeparam>
      /// <typeparam name="TItemType">Collection item type used to populate the collection.</typeparam>
      /// <returns>Message value as a strongly typed object.</returns>
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      public T GetMessage<T, TItemType>() where T : TypedMessageCollection, new() where TItemType : ITypedMessageObject, new()
      {
         var jsonMessage = GetJsonMessage(GetKey(typeof(T)));
         if (jsonMessage != null)
         {
            var typedMessageCollection = Activator.CreateInstance<T>();
            typedMessageCollection.Fill<TItemType>(jsonMessage);
            return typedMessageCollection;
         }

         return null;
      }

      private static string GetKey(Type type)
      {
         return TypeMap.FirstOrDefault(x => type.Equals(x.Value) || type.IsSubclassOf(x.Value)).Key;
      }

      /// <summary>
      /// Raise the MessageUpdated event.
      /// </summary>
      /// <param name="e">Event arguments (if null the event is cancelled).</param>
      protected override void OnMessageUpdated(MessageUpdatedEventArgs e)
      {
         if (e == null) return;

         e.DataType = TypeMap.ContainsKey(e.Key) ? TypeMap[e.Key] : null;
         base.OnMessageUpdated(e);
      }
   }
}
