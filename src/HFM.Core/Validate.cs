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

      /// <summary>
      /// Validate that both Server and Port have been specified
      /// </summary>
      /// <param name="server">Server Value</param>
      /// <param name="port">Port Value</param>
      /// <exception cref="ArgumentException">Throws when either server or port is null or empty but not the other.</exception>
      public static bool ServerPortPair(string server, string port)
      {
         return ValidateValuePair(server, "Port must also be specified when specifying Server.",
                                  port, "Server must also be specified when specifying Port.",
                                  true, "Server and Port must be specified.");
      }

      /// <summary>
      /// Validate a Value Pair
      /// </summary>
      private static bool ValidateValuePair(string value1, string value1Message, string value2, string value2Message, bool throwOnEmpty, string throwOnEmptyMessage)
      {
         if (String.IsNullOrEmpty(value1) && String.IsNullOrEmpty(value2))
         {
            if (throwOnEmpty)
            {
               throw new ArgumentException(throwOnEmptyMessage);
            }
            return false;
         }
         if (String.IsNullOrEmpty(value1) == false && String.IsNullOrEmpty(value2))
         {
            throw new ArgumentException(value1Message);
         }
         if (String.IsNullOrEmpty(value1) && String.IsNullOrEmpty(value2) == false)
         {
            throw new ArgumentException(value2Message);
         }

         return true;
      }
   }
}
