
namespace HFM.Proteins
{
    /// <summary>
    /// Represents Folding@Home project (protein) information.
    /// </summary>
    public class Protein
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Protein"/> class.
        /// </summary>
        public Protein()
        {
            
        }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        public int ProjectNumber { get; set; }

        /// <summary>
        /// Gets or sets the server IP address.
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// Gets or sets the name of the work unit (project).
        /// </summary>
        public string WorkUnitName { get; set; }

        /// <summary>
        /// Gets or sets the number of atoms defined by the project.
        /// </summary>
        public int NumberOfAtoms { get; set; }

        /// <summary>
        /// Gets or sets the preferred deadline in days.
        /// </summary>
        public double PreferredDays { get; set; }

        /// <summary>
        /// Gets or sets the maximum deadline in days.
        /// </summary>
        public double MaximumDays { get; set; }

        /// <summary>
        /// Gets or sets the project credit value.
        /// </summary>
        public double Credit { get; set; }

        /// <summary>
        /// Gets or sets the number of project frames.
        /// </summary>
        public int Frames { get; set; } = 100;

        /// <summary>
        /// Gets or sets the core identification string.
        /// </summary>
        public string Core { get; set; }

        /// <summary>
        /// Gets or sets the project description (usually a URL).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the project contact.
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets the bonus K factor.
        /// </summary>
        public double KFactor { get; set; }
    }
}
