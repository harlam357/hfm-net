/*
 * HFM.NET - TypedMessageConnection Class Tests
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

using System.Linq;

using NUnit.Framework;

using HFM.Client.DataTypes;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class TypedMessageConnectionTests
   {
      [Test]
      public void TypedMessageConnection_Heartbeat_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.Heartbeat, TestData.Heartbeat.Length);

            Assert.AreEqual(MessageKey.Heartbeat, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.Heartbeat, e.TypedMessage.Key);
            Assert.AreEqual(typeof(Heartbeat), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_Info_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.Info, TestData.Info.Length);

            Assert.AreEqual(MessageKey.Info, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.Info, e.TypedMessage.Key);
            Assert.AreEqual(typeof(Info), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_Options_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.Options, TestData.Options.Length);

            Assert.AreEqual(MessageKey.Options, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.Options, e.TypedMessage.Key);
            Assert.AreEqual(typeof(Options), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_SimulationInfo_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.SimulationInfo, TestData.SimulationInfo.Length);

            Assert.AreEqual(MessageKey.SimulationInfo, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.SimulationInfo, e.TypedMessage.Key);
            Assert.AreEqual(typeof(SimulationInfo), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_SlotCollection_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.SlotInfo, TestData.SlotInfo.Length);

            Assert.AreEqual(MessageKey.SlotInfo, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.SlotInfo, e.TypedMessage.Key);
            Assert.AreEqual(typeof(SlotCollection), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_SlotOptions_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.SlotOptions, TestData.SlotOptions.Length);

            Assert.AreEqual(MessageKey.SlotOptions, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.SlotOptions, e.TypedMessage.Key);
            Assert.AreEqual(typeof(SlotOptions), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_UnitCollection_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.QueueInfo, TestData.QueueInfo.Length);

            Assert.AreEqual(MessageKey.QueueInfo, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.QueueInfo, e.TypedMessage.Key);
            Assert.AreEqual(typeof(UnitCollection), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_LogRestart_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.LogRestart, TestData.LogRestart.Length);

            Assert.AreEqual(MessageKey.LogRestart, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.LogRestart, e.TypedMessage.Key);
            Assert.AreEqual(typeof(LogRestart), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }

      [Test]
      public void TypedMessageConnection_LogUpdate_Test()
      {
         using (var connection = new TypedMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;

            connection.ProcessData(TestData.LogUpdate, TestData.LogUpdate.Length);

            Assert.AreEqual(MessageKey.LogUpdate, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.AreEqual(MessageKey.LogUpdate, e.TypedMessage.Key);
            Assert.AreEqual(typeof(LogUpdate), e.DataType);
            Assert.AreEqual(0, e.TypedMessage.Errors.Count());
         }
      }
   }
}
