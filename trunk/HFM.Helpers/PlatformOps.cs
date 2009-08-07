/*
 * HFM.NET - Platform Operations Helper Class
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

namespace HFM.Helpers
{
   public static class PlatformOps
   {
      public static bool IsRunningOnMono()
      {
         return Type.GetType("Mono.Runtime") != null;
      }
      
      public static string GetApplicationVersion()
      {
         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         return String.Format("v{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                  fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
      }

      public static string GetApplicationLabelVersion()
      {
         FileVersionInfo fileVersionInfo =
            FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
         return String.Format("{0}.{1}.{2} - Build {3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                                         fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
      }
   }
}
