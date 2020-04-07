/*
 * HFM.NET - Static Validate Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Text.RegularExpressions;

namespace HFM.Core
{
   public static class Validate
   {
      private const string ValidWinPath = @"(?:\b[a-z]:|\\\\[a-z0-9.$_-]+\\[a-z0-9.`~!@#$%^&()_-]+)\\(?:[^\\/:*?""<>|\r\n]+\\)*";
      private const string ValidUnixPath = @"^(?:(/|~)[a-z0-9\-\s._~%!$&'()*+,;=:@/]*)+$";

      private const string ValidHttpUrl = "^(https?|file)://[-A-Z0-9+&@#/%?=~_|$!:,.;]+$";

      /// <summary>
      /// Validate Path
      /// </summary>
      public static bool Path(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;

         System.Diagnostics.Debug.WriteLine(Environment.OSVersion.VersionString);

         var validPathWin = new Regex(ValidWinPath, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         var validUnixPath = new Regex(ValidUnixPath, RegexOptions.Singleline | RegexOptions.IgnoreCase);

         return (validPathWin.IsMatch(value) || validUnixPath.IsMatch(value));
      }

      /// <summary>
      /// Validate Ftp Path
      /// </summary>
      public static bool FtpPath(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;
         if (value == "/") return true;

         var validUnixPath = new Regex(ValidUnixPath, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validUnixPath.IsMatch(value);
      }

      /// <summary>
      /// Validate Http URL
      /// </summary>
      public static bool HttpUrl(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;

         var validHttpUrl = new Regex(ValidHttpUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validHttpUrl.IsMatch(value);
      }
   }
}
