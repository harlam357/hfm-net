/*
 * HFM.NET - String (Regex) Helper Class
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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HFM.Framework
{
   public static class StringOps
   {
      #region Constants
      private const string ValidNameFirst =  "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";
      private const string ValidNameMiddle = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\] \\.]";
      private const string ValidNameLast =   "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";

      private const string ValidFileName = @"^[^\\/:*?""<>|\r\n]*$";

      private const string ValidWinPath = @"(?:\b[a-z]:|\\\\[a-z0-9.$_-]+\\[a-z0-9.`~!@#$%^&()_-]+)\\(?:[^\\/:*?""<>|\r\n]+\\)*";
      private const string ValidUnixPath = @"^(?:(/|~)[a-z0-9\-\s._~%!$&'()*+,;=:@/]*)*$";
      
      private const string ValidServer = @"^[a-z0-9\-._%]+$";
      
      private const string ValidHttpUrl = "(https?|file)://[-A-Z0-9+&@#/%?=~_|$!:,.;]*";

      private const string ValidMatchHttpOrFtpUrl =       @"\b(?<protocol>https?|ftp)://(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)";
      private const string ValidMatchFtpWithUserPassUrl = @"\b(?<protocol>ftp)://(?<username>[A-Z0-9+&@#/%=~_|!:,.;]+):(?<password>[A-Z0-9+&@#/%=~_|!:,.;]+)@(?<domain>[-A-Z0-9.]+)(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*/)";
      
      private const string ValidEmailAddress = @"^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$";

      // Work Unit Result Strings
      private const string FinishedUnit = "FINISHED_UNIT";
      private const string EarlyUnitEnd = "EARLY_UNIT_END";
      private const string UnstableMachine = "UNSTABLE_MACHINE";
      private const string Interrupted = "INTERRUPTED";
      private const string BadWorkUnit = "BAD_WORK_UNIT";
      private const string CoreOutdated = "CORE_OUTDATED";
      #endregion
   
      #region Methods
      /// <summary>
      /// Validate Instance Name
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateInstanceName(string val)
      {
         Regex rValidName = new Regex(String.Format(CultureInfo.InvariantCulture, 
            "^{0}{1}+{2}$", ValidNameFirst, ValidNameMiddle, ValidNameLast), RegexOptions.Singleline);
         return rValidName.IsMatch(val);
      }

      /// <summary>
      /// Clean Instance Name
      /// </summary>
      /// <param name="val">String to clean</param>
      /// <returns>Cleaned Instance Name</returns>
      public static string CleanInstanceName(string val)
      {
         Regex rValidFirst = new Regex(ValidNameFirst, RegexOptions.Singleline);
         Regex rValidMiddle = new Regex(ValidNameMiddle, RegexOptions.Singleline);
         Regex rValidLast = new Regex(ValidNameLast, RegexOptions.Singleline);
         
         StringBuilder sbldr = new StringBuilder(val.Length);
         for (int i = 0; i < val.Length; i++)
         {
            if (i == 0)
            {
               if (rValidFirst.IsMatch(val[i].ToString()))
               {
                  sbldr.Append(val[i]);
               }
            }
            else if (i == val.Length - 1)
            {
               if (rValidLast.IsMatch(val[i].ToString()))
               {
                  sbldr.Append(val[i]);
               }
            }
            else
            {
               if (rValidMiddle.IsMatch(val[i].ToString()))
               {
                  sbldr.Append(val[i]);
               }
            }
         }
         
         return sbldr.ToString();
      }

      /// <summary>
      /// Validate File Name
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateFileName(string val)
      {
         if (val == null) return false;
         if (val.Trim().Length == 0) return false;
      
         Regex rValidFileName = new Regex(ValidFileName, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         
         return rValidFileName.IsMatch(val);
      }

      /// <summary>
      /// Validate PathInstance Path
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidatePathInstancePath(string val)
      {
         System.Diagnostics.Debug.WriteLine(Environment.OSVersion.VersionString);
         
         Regex rValidPathWin = new Regex(ValidWinPath, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         Regex rValidPathUnix = new Regex(ValidUnixPath, RegexOptions.Singleline | RegexOptions.IgnoreCase);

         return (rValidPathWin.IsMatch(val) || rValidPathUnix.IsMatch(val));
      }

      /// <summary>
      /// Validate FTP Server Name
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateServerName(string val)
      {
         Regex rValidServer = new Regex(ValidServer, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         
         return rValidServer.IsMatch(val);
      }

      /// <summary>
      /// Validate FtpInstance Path
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateFtpPath(string val)
      {
         Regex rValidPathUnix = new Regex(ValidUnixPath, RegexOptions.Singleline | RegexOptions.IgnoreCase); 
         
         return rValidPathUnix.IsMatch(val);
      }

      /// <summary>
      /// Validate HttpInstance URL
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateHttpUrl(string val)
      {
         Regex rValidHttpURL = new Regex(ValidHttpUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);

         return rValidHttpURL.IsMatch(val);
      }

      /// <summary>
      /// Validate Http or Ftp URL
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateHttpOrFtpUrl(string val)
      {
         return MatchHttpOrFtpUrl(val).Success;
      }

      /// <summary>
      /// Match HTTP or FTP URL String
      /// </summary>
      /// <param name="val"></param>
      public static Match MatchHttpOrFtpUrl(string val)
      {
         Regex rValidHttpOrFtpUrl = new Regex(ValidMatchHttpOrFtpUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);
         
         return rValidHttpOrFtpUrl.Match(val);
      }

      /// <summary>
      /// Validate FTP URL String with Username and Password
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateFtpWithUserPassUrl(string val)
      {
         return MatchFtpWithUserPassUrl(val).Success;
      }

      /// <summary>
      /// Match FTP URL String with Username and Password
      /// </summary>
      /// <param name="val">String to validate</param>
      public static Match MatchFtpWithUserPassUrl(string val)
      {
         Regex rValidFtpWithUserPassUrl = new Regex(ValidMatchFtpWithUserPassUrl, RegexOptions.Singleline | RegexOptions.IgnoreCase);

         return rValidFtpWithUserPassUrl.Match(val);
      }
      
      /// <summary>
      /// Validate Email Address String
      /// </summary>
      /// <param name="val">String to validate</param>
      public static bool ValidateEmailAddress(string val)
      {
         Regex rValidEmailAddress = new Regex(ValidEmailAddress, RegexOptions.Singleline | RegexOptions.IgnoreCase);

         return rValidEmailAddress.IsMatch(val);
      }
      
      /// <summary>
      /// Validate that both Username and Password have been specified
      /// </summary>
      /// <param name="username">Username Value</param>
      /// <param name="password">Password Value</param>
      /// <exception cref="ArgumentException">Throws when either username or password is null or empty but not the other.</exception>
      public static bool ValidateUsernamePasswordPair(string username, string password)
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
      public static bool ValidateUsernamePasswordPair(string username, string password, bool throwOnEmpty)
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
      public static bool ValidateServerPortPair(string server, string port)
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

      /// <summary>
      /// Get the WorkUnitResult Enum representation of the given result string.
      /// </summary>
      /// <param name="result">Work Unit Result as String.</param>
      public static WorkUnitResult WorkUnitResultFromString(string result)
      {
         switch (result)
         {
            case FinishedUnit:
               return WorkUnitResult.FinishedUnit;
            case EarlyUnitEnd:
               return WorkUnitResult.EarlyUnitEnd;
            case UnstableMachine:
               return WorkUnitResult.UnstableMachine;
            case Interrupted:
               return WorkUnitResult.Interrupted;
            case BadWorkUnit:
               return WorkUnitResult.BadWorkUnit;
            case CoreOutdated:
               return WorkUnitResult.CoreOutdated;
            default:
               return WorkUnitResult.Unknown;
         }
      }
      
      /// <summary>
      /// Are two Client Instance Paths Equal?
      /// </summary>
      public static bool PathsEqual(string path1, string path2)
      {
         ICollection<string> path1Variations = GetPathVariations(path1);
         ICollection<string> path2Variations = GetPathVariations(path2);

         foreach (var variation1 in path1Variations)
         {
            foreach (var variation2 in path2Variations)
            {
               if (variation1.Equals(variation2))
               {
                  return true;
               }
            }
         }

         return false;
      }
      
      private static ICollection<string> GetPathVariations(string path)
      {
         var pathVariations = new List<string>(2);
         if (path.EndsWith("\\") || path.EndsWith("/"))
         {
            pathVariations.Add(path.ToUpperInvariant());
         }
         else
         {
            pathVariations.Add(String.Concat(path.ToUpperInvariant(), "\\"));
            pathVariations.Add(String.Concat(path.ToUpperInvariant(), "/"));
         }
         return pathVariations;
      }
      #endregion
   }
}
