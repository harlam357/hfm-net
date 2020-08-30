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
        /// Returns a new <see cref="Protein"/> object containing the data this object.
        /// </summary>
        public Protein Copy()
        {
            return new Protein
            {
                ProjectNumber = ProjectNumber,
                ServerIP = ServerIP,
                WorkUnitName = WorkUnitName,
                NumberOfAtoms = NumberOfAtoms,
                PreferredDays = PreferredDays,
                MaximumDays = MaximumDays,
                Credit = Credit,
                Frames = Frames,
                Core = Core,
                Description = Description,
                Contact = Contact,
                KFactor = KFactor
            };
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

        /// <summary>
        /// Returns true if this <see cref="Protein"/> has valid values for <see cref="ProjectNumber"/>, <see cref="PreferredDays"/>, <see cref="MaximumDays"/>, <see cref="Credit"/>, <see cref="Frames"/>, and <see cref="KFactor"/>; otherwise, false.
        /// </summary>
        public static bool IsValid(Protein protein)
        {
            if (protein == null) return false;

            return protein.ProjectNumber > 0 &&
                   protein.PreferredDays > 0 &&
                   protein.MaximumDays > 0 &&
                   protein.Credit > 0 &&
                   protein.Frames > 0 &&
                   protein.KFactor >= 0;
        }
    }
}
