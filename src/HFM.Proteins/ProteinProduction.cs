namespace HFM.Proteins
{
    /// <summary>
    /// Represents all protein production measurements.
    /// </summary>
    public struct ProteinProduction
    {
        /// <summary>
        /// Gets or sets the units per day (UPD) measurement.
        /// </summary>
        public double UPD { get; set; }

        /// <summary>
        /// Gets or sets the bonus multiplier measurement.
        /// </summary>
        public double Multiplier { get; set; }

        /// <summary>
        /// Gets or sets the work unit credit measurement.
        /// </summary>
        public double Credit { get; set; }

        /// <summary>
        /// Gets or sets the points per day (PPD) measurement.
        /// </summary>
        public double PPD { get; set; }
    }
}
