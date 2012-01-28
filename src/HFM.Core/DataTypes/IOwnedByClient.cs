/*
 * HFM.NET - Owning Client Interface
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

namespace HFM.Core.DataTypes
{
   public interface IOwnedByClient
   {
      /// <summary>
      /// Fully qualified name of the folding slot that owns this object (includes "Slot" designation).
      /// </summary>
      string OwningSlotName { get; }

      /// <summary>
      /// Name of the folding client that owns this object (name given during client setup).
      /// </summary>
      string OwningClientName { get; }

      /// <summary>
      /// Path of the folding client that own this object.
      /// </summary>
      string OwningClientPath { get; }

      /// <summary>
      /// Identification number of the folding slot on the folding client that owns this object.
      /// </summary>
      int OwningSlotId { get; }

      /// <summary>
      /// Project ID
      /// </summary>
      int ProjectID { get; }
   }
}
