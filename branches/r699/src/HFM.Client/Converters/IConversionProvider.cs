/*
 * HFM.NET - Conversion Provider Interface
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

namespace HFM.Client.Converters
{
   /// <summary>
   /// Provides functionality to convert a Folding@Home message property value to another type.  Types specified as the ConverterType of a MessagePropertyAttribute must implement this interface.
   /// </summary>
   public interface IConversionProvider
   {
      /// <summary>
      /// Returns an object whose value has been converted from the specified input object.
      /// </summary>
      /// <param name="input">Conversion input value.</param>
      /// <returns>Converted value.</returns>
      object Convert(object input);
   }
}
