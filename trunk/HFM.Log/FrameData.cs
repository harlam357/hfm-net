/*
 * HFM.NET - Frame Data Class
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

using HFM.Framework;

namespace HFM.Log
{
   public class FrameData : IFrameData
   {
      private string _TimeStampString;
      public string TimeStampString
      {
         get { return _TimeStampString; }
         set { _TimeStampString = value; }
      }

      private int _RawFramesComplete;
      public int RawFramesComplete
      {
         get { return _RawFramesComplete; }
         set { _RawFramesComplete = value; }
      }

      private int _RawFramesTotal;
      public int RawFramesTotal
      {
         get { return _RawFramesTotal; }
         set { _RawFramesTotal = value; }
      }

      private int _FrameID;
      public int FrameID
      {
         get { return _FrameID; }
         set { _FrameID = value; }
      }
   }
}
