/*
 * HFM.NET
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

// TODO: Figure out how to move this to Client namespace and maintain compatibility with existing hfmx files
namespace HFM.Core.DataTypes
{
    /// <summary>
    /// Client Types
    /// </summary>
    public enum ClientType
    {
        FahClient,
        [Obsolete("Do not use Legacy.")]
        Legacy
    }

    [DataContract]
    public class ClientSettings
    {
        public ClientSettings()
        {
            Port = DefaultPort;
        }

        /// <summary>
        /// Client Type
        /// </summary>
        [DataMember(Order = 1)]
        public ClientType ClientType { get; set; }

        /// <summary>
        /// The name assigned to this client instance
        /// </summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Server name or IP Address
        /// </summary>
        [DataMember(Order = 3)]
        public string Server { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        [DataMember(Order = 4)]
        public int Port { get; set; }

        /// <summary>
        /// Password on remote server
        /// </summary>
        [DataMember(Order = 5)]
        public string Password { get; set; }

        public string ClientLogFileName => String.Format(CultureInfo.InvariantCulture, "{0}-{1}", Name, Constants.FahClientLogFileName);

        public string ClientPath => String.Format(CultureInfo.InvariantCulture, "{0}-{1}", Server, Port);

        /// <summary>
        /// The default Folding@Home client port.
        /// </summary>
        public const int DefaultPort = 36330;

        private const string NameFirstCharPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";
        private const string NameMiddleCharsPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\] \\.]";
        private const string NameLastCharPattern = "[a-zA-Z0-9\\+=\\-_\\$&^\\[\\]]";

        /// <summary>
        /// Validates the client settings name.
        /// </summary>
        public static bool ValidateName(string name)
        {
            if (name == null) return false;

            string pattern = String.Format(CultureInfo.InvariantCulture,
                "^{0}{1}+{2}$", NameFirstCharPattern, NameMiddleCharsPattern, NameLastCharPattern);
            return Regex.IsMatch(name, pattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// Removes invalid characters from the client name string.
        /// </summary>
        public static string CleanName(string name)
        {
            if (name == null) return null;

            var first = new Regex(NameFirstCharPattern, RegexOptions.Singleline);
            var middle = new Regex(NameMiddleCharsPattern, RegexOptions.Singleline);
            var last = new Regex(NameLastCharPattern, RegexOptions.Singleline);

            var sb = new StringBuilder(name.Length);
            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];
                if (i == 0)
                {
                    if (first.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
                else if (i == name.Length - 1)
                {
                    if (last.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (middle.IsMatch(c.ToString()))
                    {
                        sb.Append(c);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
