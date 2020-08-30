using System;
using System.Globalization;

namespace HFM.Proteins
{
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

        public ProteinPropertyChange(string propertyName, string previous, string current)
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
