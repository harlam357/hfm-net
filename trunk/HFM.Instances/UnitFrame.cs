/*
 * HFM.NET - Unit Frame Data Class
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

namespace HFM.Instances
{
   [Serializable]
   public class UnitFrame
   {
      private readonly Int32 _FramePercent;
      public Int32 FramePercent
      {
         get { return _FramePercent; }
      }
   
      private readonly TimeSpan _TimeOfFrame;
      public TimeSpan TimeOfFrame
      {
         get { return _TimeOfFrame; }
      }
      
      private TimeSpan _FrameDuration;
      public TimeSpan FrameDuration
      {
         get { return _FrameDuration; }
         set { _FrameDuration = value; }
      }
   
      public UnitFrame(Int32 framePercent, TimeSpan frameTime)
      {
         _FramePercent = framePercent;
         _TimeOfFrame = frameTime;
      }
   }
}