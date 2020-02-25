/*
 * HFM.NET - Protein Load Info Metadata Class
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HFM.Proteins
{
    /// <summary>
    /// Defines the result of a change to a <see cref="ProteinDictionary"/>.
    /// </summary>
    public enum ProteinDictionaryChangeResult
    {
        /// <summary>
        /// A protein was added.
        /// </summary>
        Added,
        /// <summary>
        /// An existing protein was changed.
        /// </summary>
        Changed,
        /// <summary>
        /// An existing protein was not changed.
        /// </summary>
        NoChange
    }

    /// <summary>
    /// Represents a change to a <see cref="ProteinDictionary"/>.
    /// </summary>
    public class ProteinDictionaryChange
    {
        /// <summary>
        /// Gets the project number.
        /// </summary>
        public int ProjectNumber { get; }

        /// <summary>
        /// Gets the result of the change.
        /// </summary>
        public ProteinDictionaryChangeResult Result { get; }

        /// <summary>
        /// Gets a collection of protein properties that changed.
        /// </summary>
        public IReadOnlyCollection<ProteinPropertyChange> Changes { get; }

        internal ProteinDictionaryChange(int projectNumber, ProteinDictionaryChangeResult result)
           : this(projectNumber, result, null)
        {

        }

        internal ProteinDictionaryChange(int projectNumber, ProteinDictionaryChangeResult result, IReadOnlyCollection<ProteinPropertyChange> changes)
        {
            ProjectNumber = projectNumber;
            Result = result;
            Changes = changes;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ProteinDictionaryChange"/> object.
        /// </summary>
        /// <returns>A string that represents the current <see cref="ProteinDictionaryChange"/> object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Project: ");
            sb.Append(ProjectNumber);
            sb.Append(" - ");
            sb.Append(Result);
            if (Changes != null)
            {
                foreach (var change in Changes)
                {
                    sb.Append(", ");
                    sb.Append(change);
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Represents a change to a <see cref="Protein"/>.
    /// </summary>
    public class ProteinPropertyChange
    {
        /// <summary>
        /// Gets the name of the property that changed.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the previous property value (before the change).
        /// </summary>
        public string Previous { get; }

        /// <summary>
        /// Gets the current property value (after the change).
        /// </summary>
        public string Current { get; }

        internal ProteinPropertyChange(string propertyName, string previous, string current)
        {
            PropertyName = propertyName;
            Previous = previous;
            Current = current;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ProteinPropertyChange"/> object.
        /// </summary>
        /// <returns>A string that represents the current <see cref="ProteinPropertyChange"/> object.</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}: {1} > {2}", PropertyName, Previous, Current);
        }
    }
}
