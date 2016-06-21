/*
 * HFM.NET - JsonMessageConnection Class Tests
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
using System.IO;

using NUnit.Framework;

namespace HFM.Client.Tests
{
   [TestFixture]
   public class JsonMessageConnectionTests
   {
      [Test]
      public void JsonMessageConnection_JsonMessage_Test1()
      {
         using (var connection = new JsonMessageConnection())
         {
            MessageReceivedEventArgs e = null;
            connection.MessageReceived += (sender, args) => e = args;
            bool updateFinishedRaised = false;
            connection.UpdateFinished += (sender, args) => updateFinishedRaised = true;

            connection.ProcessData(TestData.QueueInfo, TestData.QueueInfo.Length);

            Assert.AreEqual(MessageKey.QueueInfo, e.JsonMessage.Key);
            Assert.IsTrue(e.JsonMessage.Value.Length > 0);
            Assert.IsNull(e.TypedMessage);
            Assert.IsNull(e.DataType);
            Assert.IsTrue(updateFinishedRaised);
         }
      }

      [Test]
      public void JsonMessageConnection_GetNextJsonMessage_Test1()
      {
         string message = null;
         Assert.IsNull(JsonMessageConnection.GetNextJsonMessage(ref message));
      }

      [Test]
      public void JsonMessageConnection_GetNextJsonMessage_Test2()
      {
         // no PyON header
         string message = String.Empty;
         Assert.IsNull(JsonMessageConnection.GetNextJsonMessage(ref message));
      }

      [Test]
      public void JsonMessageConnection_GetNextJsonMessage_Test3()
      {
         // nothing but PyON header
         string message = "PyON 1 ";
         Assert.IsNull(JsonMessageConnection.GetNextJsonMessage(ref message));
      }

      [Test]
      public void JsonMessageConnection_GetNextJsonMessage_Test4()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\partial-info.txt");
         Assert.IsNull(JsonMessageConnection.GetNextJsonMessage(ref message));
      }

      [Test]
      public void JsonMessageConnection_GetNextJsonMessage_Test5()
      {
         string message = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
         var jsonMessage = JsonMessageConnection.GetNextJsonMessage(ref message);
         Assert.AreEqual("info", jsonMessage.Key);
         Assert.GreaterOrEqual(DateTime.UtcNow, jsonMessage.Received);
         Assert.IsNotNull(jsonMessage.Value);
      }
   }
}
