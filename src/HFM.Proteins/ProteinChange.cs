using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HFM.Proteins
{
    /// <summary>
    /// Defines the action taken to change a <see cref="ProteinCollection"/>.
    /// </summary>
    public enum ProteinChangeAction
    {
        /// <summary>
        /// The protein was not changed.
        /// </summary>
        None,
        /// <summary>
        /// The protein was added.
        /// </summary>
        Add,
        /// <summary>
        /// Protein properties were changed.
        /// </summary>
        Property
    }

    public class ProteinChange
    {
        /// <summary>
        /// Gets the project number.
        /// </summary>
        public int ProjectNumber { get; }

        /// <summary>
        /// Gets the change action.
        /// </summary>
        public ProteinChangeAction Action { get; }

        /// <summary>
        /// Gets a collection of protein properties that changed.
        /// </summary>
        public IReadOnlyList<ProteinPropertyChange> PropertyChanges { get; }

        private ProteinChange(int projectNumber, ProteinChangeAction action, IEnumerable<ProteinPropertyChange> propertyChanges)
        {
            ProjectNumber = projectNumber;
            Action = action;
            PropertyChanges = propertyChanges as IReadOnlyList<ProteinPropertyChange> ?? propertyChanges?.ToList().AsReadOnly();
        }

        /// <summary>
        /// The protein was not changed.
        /// </summary>
        public static ProteinChange None(int projectNumber) => new ProteinChange(projectNumber, ProteinChangeAction.None, null);

        /// <summary>
        /// The protein was added.
        /// </summary>
        public static ProteinChange Add(int projectNumber) => new ProteinChange(projectNumber, ProteinChangeAction.Add, null);

        /// <summary>
        /// Protein properties were changed.
        /// </summary>
        public static ProteinChange Property(int projectNumber, IEnumerable<ProteinPropertyChange> propertyChanges) =>
            new ProteinChange(projectNumber, ProteinChangeAction.Property, propertyChanges);

        /// <summary>
        /// Returns a string that represents the current <see cref="ProteinChange"/> object.
        /// </summary>
        /// <returns>A string that represents the current <see cref="ProteinChange"/> object.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Project: ");
            sb.Append(ProjectNumber);
            sb.Append(" - ");
            sb.Append(Action);
            if (PropertyChanges != null)
            {
                foreach (var change in PropertyChanges)
                {
                    sb.Append(", ");
                    sb.Append(change);
                }
            }
            return sb.ToString();
        }
    }
}
