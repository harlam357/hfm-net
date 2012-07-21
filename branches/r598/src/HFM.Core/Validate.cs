/*
 * HFM.NET - Static Validate Class
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
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HFM.Core
{
   public static class Validate
   {
      #region Constants
      
      private const string ValidNameFirst =  "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";
      private const string ValidNameMiddle = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\] \\.]";
      private const string ValidNameLast =   "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";

      private const string ValidFileName = @"^[^\\/:*?""<>|\r\n]*$";

      private const string ValidWinPath = @"(?:\b[a-z]:|\\\\[a-z0-9.$_-]+\\[a-z0-9.`~!@#$%^&()_-]+)\\(?:[^\\/:*?""<>|\r\n]+\\)*";
      private const string ValidUnixPath = @"^(?:(/|~)[a-z0-9\-\s._~%!$&'()*+,;=:@/]*)+$";
      
      private const string ValidServer = @"^[a-z0-9\-._%]+$";
      
      private const string ValidHttpUrl = "^(https?|file)://[-A-Z0-9+&@#/%?=~_|$!:,.;]+$";

      private const string ValidMatchHttpOrFtpUrl =       @"\b(?<protocol>https?|ftp)://(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)";
      private const string ValidMatchFtpWithUserPassUrl = @"\b(?<protocol>ftp)://(?<username>[A-Z0-9+&@#/%=~_|!:,.;]+):(?<password>[A-Z0-9+&@#/%=~_|!:,.;]+)@(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*/)";
      
      private const string ValidEmailAddress = @"^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$";
      
      #endregion
   
      #region Methods

      /// <summary>
      /// Validate Client Name
      /// </summary>
      public static bool ClientName(string name)
      {
         if (name == null) return false;

         var rValidName = new Regex(String.Format(CultureInfo.InvariantCulture, 
            "^{0}{1}+{2}$", ValidNameFirst, ValidNameMiddle, ValidNameLast), RegexOptions.Singleline);
         return rValidName.IsMatch(name);
      }

      /// <summary>
      /// Clean Client Name
      /// </summary>
      public static string CleanClientName(string name)
      {
         if (name == null) return null;

         var rValidFirst = new Regex(ValidNameFirst, RegexOptions.Singleline);
         var rValidMiddle = new Regex(ValidNameMiddle, RegexOptions.Singleline);
         var rValidLast = new Regex(ValidNameLast, RegexOptions.Singleline);
         
         var sbldr = new StringBuilder(name.Length);
         for (int i = 0; i < name.Length; i++)
         {
            if (i == 0)
            {
               if (rValidFirst.IsMatch(name[i].ToString()))
               {
                  sbldr.Append(name[i]);
               }
            }
            else if (i == name.Length - 1)
            {
               if (rValidLast.IsMatch(name[i].ToString()))
               {
                  sbldr.Append(name[i]);
               }
            }
            else
            {
               if (rValidMiddle.IsMatch(name[i].ToString()))
               {
                  sbldr.Append(name[i]);
               }
            }
         }
         
         return sbldr.ToString();
      }

      /// <summary>
      /// Validate File Name
      /// </summary>
      public static bool FileName(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;
      
         var validFileName = new Regex(ValidFileName, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validFileName.IsMatch(value);
      }

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
      /// Validate Server Name
      /// </summary>
      public static bool ServerName(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;

         var validServer = new Regex(ValidServer, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validServer.IsMatch(value);
      }

      /// <summary>
      /// Validate Server Port Number
      /// </summary>
      public static bool ServerPort(int value)
      {
         return value > 0 && value < UInt16.MaxValue;
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
      /// Validate Http or Ftp URL
      /// </summary>
      public static bool HttpOrFtpUrl(string value)
      {
         var match = MatchHttpOrFtpUrl(value);
         return match != null && match.Success;
      }

      /// <summary>
      /// Match Http or Ftp URL
      /// </summary>
      public static Match MatchHttpOrFtpUrl(string value)
      {
         if (value == null) return null;
         value = value.Trim();
         if (value.Length == 0) return null;

         var validHttpOrFtpUrl = new Regex(ValidMatchHttpOrFtpUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validHttpOrFtpUrl.Match(value);
      }

      /// <summary>
      /// Validate Ftp URL with Username and Password
      /// </summary>
      public static bool FtpWithUserPassUrl(string value)
      {
         var match = MatchFtpWithUserPassUrl(value);
         return match != null && match.Success;
      }

      /// <summary>
      /// Match FTP URL String with Username and Password
      /// </summary>
      public static Match MatchFtpWithUserPassUrl(string value)
      {
         if (value == null) return null;
         value = value.Trim();
         if (value.Length == 0) return null;

         var validFtpWithUserPassUrl = new Regex(ValidMatchFtpWithUserPassUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validFtpWithUserPassUrl.Match(value);
      }
      
      /// <summary>
      /// Validate Email Address
      /// </summary>
      public static bool EmailAddress(string value)
      {
         if (value == null) return false;
         value = value.Trim();
         if (value.Length == 0) return false;

         var validEmailAddress = new Regex(ValidEmailAddress, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         return validEmailAddress.IsMatch(value);
      }
      
      /// <summary>
      /// Validate that both Username and Password have been specified
      /// </summary>
      /// <param name="username">Username Value</param>
      /// <param name="password">Password Value</param>
      /// <exception cref="ArgumentException">Throws when either username or password is null or empty but not the other.</exception>
      public static bool UsernamePasswordPair(string username, string password)
      {
         return ValidateValuePair(username, "Password must also be specified when specifying Username.",
                                  password, "Username must also be specified when specifying Password.",
                                  false, String.Empty);
      }

      /// <summary>
      /// Validate that both Username and Password have been specified
      /// </summary>
      /// <param name="username">Username Value</param>
      /// <param name="password">Password Value</param>
      /// <param name="throwOnEmpty">Throw Exception if both values are empty.</param>
      /// <exception cref="ArgumentException">Throws when either username or password is null or empty but not the other.</exception>
      public static bool UsernamePasswordPair(string username, string password, bool throwOnEmpty)
      {
         return ValidateValuePair(username, "Password must also be specified when specifying Username.",
                                  password, "Username must also be specified when specifying Password.",
                                  throwOnEmpty, "Username and Password must be specified.");
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

      public static bool Minutes(int minutes)
      {
         if ((minutes > Constants.MaxMinutes) || (minutes < Constants.MinMinutes))
         {
            return false;
         }

         return true;
      } 

      #endregion
   }
}
