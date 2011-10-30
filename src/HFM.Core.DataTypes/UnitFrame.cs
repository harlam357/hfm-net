/*
 * HFM.NET - Unit Frame Class
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

using ProtoBuf;

namespace HFM.Core.DataTypes
{
   public interface IUnitFrame : IComparable<IUnitFrame>, IEquatable<IUnitFrame>
   {
      int FrameID { get; }
   
      TimeSpan TimeOfFrame { get; }

      TimeSpan FrameDuration { get; }
   }

   [ProtoContract]
   public sealed class UnitFrame : IUnitFrame
   {
      public int RawFramesComplete { get; set; }

      public int RawFramesTotal { get; set; }
   
      [ProtoMember(1)]
      public int FrameID { get; set; }

      [ProtoMember(2)]
      public TimeSpan TimeOfFrame { get; set; }

      [ProtoMember(3)]
      public TimeSpan FrameDuration { get; set; }

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
         var frame = obj as IUnitFrame;
         return frame != null ? Equals(frame) : base.Equals(obj);
      }

      ///<summary>
      ///Determines whether the specified <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see> is equal to the current <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see>.
      ///</summary>
      ///<returns>
      ///true if the specified <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see> is equal to the current <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see>; otherwise, false.
      ///</returns>
      ///<param name="other">The <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see> to compare with the current <see cref="T:HFM.Framework.DataTypes.IUnitFrame"></see>.</param>
      public bool Equals(IUnitFrame other)
      {
         if (other == null) return false;

         return FrameID.Equals(other.FrameID) &&
                TimeOfFrame.Equals(other.TimeOfFrame) &&
                FrameDuration.Equals(other.FrameDuration);
      }

      public static bool operator == (UnitFrame uf1, UnitFrame uf2)
      {
         return ReferenceEquals(uf1, null) ? ReferenceEquals(uf2, null) : uf1.Equals(uf2);
      }

      public static bool operator != (UnitFrame uf1, UnitFrame uf2)
      {
         return !(uf1 == uf2);
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
         return uf1 == null ? uf2 != null : uf1.CompareTo(uf2) < 0;
      }

      public static bool operator > (UnitFrame uf1, UnitFrame uf2)
      {
         return uf2 == null ? uf1 != null : uf2.CompareTo(uf1) < 0;
      }
   }
}
