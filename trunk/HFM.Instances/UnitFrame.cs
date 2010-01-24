/*
 * HFM.NET - Unit Frame Class
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

using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   [Serializable]
   [ProtoContract]
   public class UnitFrame : IUnitFrame
   {
      // Would have liked to change the member to _FrameID, bitten by the BinaryFormatter
      // Look into migrating these pieces of data into something using data-contracts.
      private Int32 _FramePercent;
      // Changed the Property Name to FrameID - 11/22/09
      [ProtoMember(1)]
      public Int32 FrameID
      {
         get { return _FramePercent; }
         set { _FramePercent = value; }
      }
   
      private TimeSpan _TimeOfFrame;
      [ProtoMember(2)]
      public TimeSpan TimeOfFrame
      {
         get { return _TimeOfFrame; }
         set { _TimeOfFrame = value; }
      }
      
      private TimeSpan _FrameDuration;
      [ProtoMember(3)]
      public TimeSpan FrameDuration
      {
         get { return _FrameDuration; }
         set { _FrameDuration = value; }
      }

      public UnitFrame()
      {
      
      }
   
      public UnitFrame(Int32 framePercent, TimeSpan frameTime)
      {
         _FramePercent = framePercent;
         _TimeOfFrame = frameTime;
      }

      ///<summary>
      ///Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
      ///</summary>
      ///<returns>
      ///A hash code for the current <see cref="T:System.Object"></see>.
      ///</returns>
      ///<filterpriority>2</filterpriority>
      public override int GetHashCode()
      {
         return FrameID.GetHashCode() ^
                TimeOfFrame.GetHashCode() ^
                FrameDuration.GetHashCode();
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
      ///</returns>
      ///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
      ///<filterpriority>2</filterpriority>
      public override bool Equals(object obj)
      {
         IUnitFrame frame = obj as IUnitFrame;
         if (frame != null)
         {
            return Equals(frame);
         }

         return base.Equals(obj);
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:HFM.Framework.IUnitFrame"></see> is equal to the current <see cref="T:HFM.Framework.IUnitFrame"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:HFM.Framework.IUnitFrame"></see> is equal to the current <see cref="T:HFM.Framework.IUnitFrame"></see>; otherwise, false.
      ///</returns>
      ///<param name="frame">The <see cref="T:HFM.Framework.IUnitFrame"></see> to compare with the current <see cref="T:HFM.Framework.IUnitFrame"></see>.</param>
      public bool Equals(IUnitFrame frame)
      {
         if (frame == null)
         {
            return false;
         }

         if (FrameID.Equals(frame.FrameID) &&
             TimeOfFrame.Equals(frame.TimeOfFrame) &&
             FrameDuration.Equals(frame.FrameDuration))
         {
            return true;
         }

         return false;
      }

      ///<summary>
      ///Compares the current object with another object of the same type.
      ///</summary>
      ///<returns>
      ///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other. 
      ///</returns>
      ///<param name="other">An object to compare with this object.</param>
      public int CompareTo(IUnitFrame other)
      {
         if (other == null)
         {
            return 1;
         }

         if (FrameID.Equals(other.FrameID))
         {
            if (TimeOfFrame.Equals(other.TimeOfFrame))
            {
               if (FrameDuration.Equals(other.FrameDuration))
               {
                  return 0;
               }

               return FrameDuration.CompareTo(other.FrameDuration);
            }

            return TimeOfFrame.CompareTo(other.TimeOfFrame);
         }

         return FrameID.CompareTo(other.FrameID);
      }

      public static bool operator < (UnitFrame uf1, UnitFrame uf2)
      {
         return (uf1.CompareTo(uf2) < 0);
      }

      public static bool operator > (UnitFrame uf1, UnitFrame uf2)
      {
         return (uf1.CompareTo(uf2) > 0);
      }
   }
}