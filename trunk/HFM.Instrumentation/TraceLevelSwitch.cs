/*
 * HFM.NET - Instrumentation Class
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
using System.Diagnostics;

namespace HFM.Instrumentation
{
   public sealed class TraceLevelSwitch
   {
      private static TraceSwitch _Instance;
      private static readonly Object classLock = typeof(TraceSwitch);

      // FxCop: CA1053 - StaticHolderTypesShouldNotHaveConstructors
      private TraceLevelSwitch()
      {
      
      }

      public static TraceSwitch Instance
      {
         get
         {
            lock (classLock)
            {
               if (_Instance == null)
               {
                  _Instance = new TraceSwitch("TraceLevelSwitch", "TraceLevelSwitch");
               }
            }
            return _Instance;
         }
      }

      public static TraceSwitch Switch
      {
         get { return Instance; }
      }
   }
}
