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
   // TODO: Split this interface up and document
   public interface IFahClient : IDisposable
   {
      #region Properties

      bool Connected { get; }

      int ConnectTimeout { get; set; }

      int SendBufferSize { get; set; }

      int ReceiveBufferSize { get; set; }

      #endregion

      #region Events

      event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
      event EventHandler<DataEventArgs> DataReceived;
      event EventHandler<DataEventArgs> DataSent;
      event EventHandler<MessageReceivedEventArgs> MessageReceived;
      event EventHandler<StatusMessageEventArgs> StatusMessage;
      event EventHandler UpdateFinished;

      #endregion

      #region Methods

      void Connect(string host, int port, string password);

      void Close();

      void SendCommand(string command);

      #endregion
   }

   /// <summary>
   /// Folding@Home client typed message connection class.  Provides functionality for parsing JSON messages into strongly typed objects.
   /// </summary>
   public class TypedMessageConnection : JsonMessageConnection, IFahClient
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
      public TypedMessageConnection()
         : this(new TcpClientFactory())
      {

      }

      /// <summary>
      /// Initializes a new instance of the FahClient class.
      /// </summary>
      internal TypedMessageConnection(ITcpClientFactory tcpClientFactory)
         : base(tcpClientFactory)
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
