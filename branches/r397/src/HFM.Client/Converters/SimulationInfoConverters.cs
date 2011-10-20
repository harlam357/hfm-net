/*
 * HFM.NET - Simulation Info Data Converters
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

namespace HFM.Client.Converters
{
   internal sealed class SimulationInfoDateTimeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         var inputValue = (int)input;
         if (inputValue == 0)
         {
            // not an error, but no value
            return null;
         }

         return new DateTime(1970, 1, 1).AddSeconds((int)input);
      }
   }

   internal sealed class SimulationInfoTimeSpanConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         return new TimeSpan(0, 0, (int)input);
      }
   }
}
