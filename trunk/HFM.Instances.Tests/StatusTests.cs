/*
 * HFM.NET - Status Tests
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using HFM.Preferences;
using NUnit.Framework;

using HFM.Instances;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class StatusTests
   {
      [Test]
      public void StatusTestSet1_RunningNoFrameTimes()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.GettingWorkPacket;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(3, 0, 0));

         statusData.UnitStartTimeStamp = new TimeSpan(2, 55, 0);
         statusData.TimeOfLastFrame = TimeSpan.Zero;
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(3, 0, 0));
         statusData.TimeOfLastFrameProgress = DateTime.MinValue;

         statusData.FrameTime = 0;
         statusData.AverageFrameTime = new TimeSpan(0, 12, 35);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet1_Running()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.Running;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(3, 0, 0));

         statusData.UnitStartTimeStamp = new TimeSpan(1, 55, 0);
         statusData.TimeOfLastFrame = new TimeSpan(2, 50, 0);
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(2, 0, 0));
         statusData.TimeOfLastFrameProgress = Date.Add(new TimeSpan(3, 0, 0));

         statusData.FrameTime = 750;
         statusData.AverageFrameTime = new TimeSpan(0, 12, 35);

         Assert.AreEqual(ClientStatus.Running, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet2_RunningNoFrameTimes_Async()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.GettingWorkPacket;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(3, 0, 0));

         // Client Clock is ~2 Hours Behind this machine
         statusData.UnitStartTimeStamp = new TimeSpan(0, 55, 0);
         statusData.TimeOfLastFrame = TimeSpan.Zero;
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(3, 0, 0));
         statusData.TimeOfLastFrameProgress = DateTime.MinValue;

         statusData.FrameTime = 0;
         statusData.AverageFrameTime = new TimeSpan(0, 12, 35);

         Assert.AreEqual(ClientStatus.RunningNoFrameTimes, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet2_Hung_NoAsync()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.GettingWorkPacket;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(3, 0, 0));

         // Client Clock is ~2 Hours Behind this machine
         statusData.UnitStartTimeStamp = new TimeSpan(0, 55, 0);
         statusData.TimeOfLastFrame = TimeSpan.Zero;
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(3, 0, 0));
         statusData.TimeOfLastFrameProgress = DateTime.MinValue;

         statusData.FrameTime = 0;
         statusData.AverageFrameTime = new TimeSpan(0, 12, 35);

         statusData.AllowRunningAsync = false;
         Assert.AreEqual(ClientStatus.Hung, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet3_RunningAsync()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.RunningAsync;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(12, 0, 0));

         // Client Clock is 4 Hours Behind this machine
         statusData.UnitStartTimeStamp = new TimeSpan(6, 0, 0);
         statusData.TimeOfLastFrame = new TimeSpan(7, 50, 0);
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(10, 0, 0));
         statusData.TimeOfLastFrameProgress = Date.Add(new TimeSpan(11, 50, 0));

         statusData.FrameTime = 633; // 10 Minutes 33 Seconds
         statusData.AverageFrameTime = new TimeSpan(0, 10, 25);

         Assert.AreEqual(ClientStatus.RunningAsync, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet3_AsyncHung()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.Hung;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(12, 0, 0));

         // Client Clock is 4 Hours Behind this machine
         statusData.UnitStartTimeStamp = new TimeSpan(6, 0, 0);
         statusData.TimeOfLastFrame = new TimeSpan(7, 50, 0);
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(10, 0, 0));
         statusData.TimeOfLastFrameProgress = Date.Add(new TimeSpan(10, 50, 0));

         statusData.FrameTime = 633; // 10 Minutes 33 Seconds
         statusData.AverageFrameTime = new TimeSpan(0, 10, 25);

         Assert.AreEqual(ClientStatus.Hung, ClientInstance.HandleReturnedStatus(statusData));
      }

      [Test]
      public void StatusTestSet3_Hung_NoAsync()
      {
         DateTime Date = DateTime.Now.Date;

         StatusData statusData = new StatusData();
         statusData.InstanceName = "Status Test";
         statusData.TypeOfClient = ClientType.SMP;
         statusData.ClientTimeOffset = 0;
         statusData.ClientIsOnVirtualMachine = false;

         statusData.CurrentStatus = ClientStatus.Hung;
         statusData.ReturnedStatus = ClientStatus.RunningNoFrameTimes;

         statusData.LastRetrievalTime = Date.Add(new TimeSpan(12, 0, 0));

         // Client Clock is 4 Hours Behind this machine
         statusData.UnitStartTimeStamp = new TimeSpan(6, 0, 0);
         statusData.TimeOfLastFrame = new TimeSpan(7, 50, 0);
         statusData.TimeOfLastUnitStart = Date.Add(new TimeSpan(10, 0, 0));
         statusData.TimeOfLastFrameProgress = Date.Add(new TimeSpan(11, 50, 0));

         statusData.FrameTime = 633; // 10 Minutes 33 Seconds
         statusData.AverageFrameTime = new TimeSpan(0, 10, 25);

         statusData.AllowRunningAsync = false;
         Assert.AreEqual(ClientStatus.Hung, ClientInstance.HandleReturnedStatus(statusData));
      }
   }
}
