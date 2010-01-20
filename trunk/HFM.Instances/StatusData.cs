/*
 * HFM.NET - Status Data Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Framework;

namespace HFM.Instances
{
   public class StatusData
   {
      private string _InstanceName;
      public string InstanceName
      {
         get { return _InstanceName; }
         set { _InstanceName = value; }
      }

      private ClientType _TypeOfClient;
      public ClientType TypeOfClient
      {
         get { return _TypeOfClient; }
         set { _TypeOfClient = value; }
      }

      private DateTime _LastRetrievalTime;
      public DateTime LastRetrievalTime
      {
         get { return _LastRetrievalTime; }
         set { _LastRetrievalTime = value; }
      }

      private bool _ClientIsOnVirtualMachine;
      public bool ClientIsOnVirtualMachine
      {
         get { return _ClientIsOnVirtualMachine; }
         set { _ClientIsOnVirtualMachine = value; }
      }

      private int _ClientTimeOffset;
      public int ClientTimeOffset
      {
         get { return _ClientTimeOffset; }
         set { _ClientTimeOffset = value; }
      }

      private DateTime _TimeOfLastUnitStart;
      public DateTime TimeOfLastUnitStart
      {
         get { return _TimeOfLastUnitStart; }
         set { _TimeOfLastUnitStart = value; }
      }

      private DateTime _TimeOfLastFrameProgress;
      public DateTime TimeOfLastFrameProgress
      {
         get { return _TimeOfLastFrameProgress; }
         set { _TimeOfLastFrameProgress = value; }
      }

      private ClientStatus _CurrentStatus;
      public ClientStatus CurrentStatus
      {
         get { return _CurrentStatus; }
         set { _CurrentStatus = value; }
      }

      private ClientStatus _ReturnedStatus;
      public ClientStatus ReturnedStatus
      {
         get { return _ReturnedStatus; }
         set { _ReturnedStatus = value; }
      }

      private int _FrameTime;
      public int FrameTime
      {
         get { return _FrameTime; }
         set { _FrameTime = value; }
      }

      private TimeSpan _AverageFrameTime;
      public TimeSpan AverageFrameTime
      {
         get { return _AverageFrameTime; }
         set { _AverageFrameTime = value; }
      }

      private TimeSpan _TimeOfLastFrame;
      public TimeSpan TimeOfLastFrame
      {
         get { return _TimeOfLastFrame; }
         set { _TimeOfLastFrame = value; }
      }

      private TimeSpan _UnitStartTimeStamp;
      public TimeSpan UnitStartTimeStamp
      {
         get { return _UnitStartTimeStamp; }
         set { _UnitStartTimeStamp = value; }
      }

      private bool _AllowRunningAsync = true;
      public bool AllowRunningAsync
      {
         get { return _AllowRunningAsync; }
         set { _AllowRunningAsync = value; }
      }
   }
}
