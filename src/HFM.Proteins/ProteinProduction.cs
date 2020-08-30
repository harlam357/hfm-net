using System;

namespace HFM.Proteins
{
    /// <summary>
    /// Represents all protein production measurements.
    /// </summary>
    public readonly struct ProteinProduction : IEquatable<ProteinProduction>
    {
        public ProteinProduction(double upd, double multiplier, double credit, double ppd)
        {
            UPD = upd;
            Multiplier = multiplier;
            Credit = credit;
            PPD = ppd;
        }

        /// <summary>
        /// Gets or sets the units per day (UPD) measurement.
        /// </summary>
        public double UPD { get; }

        /// <summary>
        /// Gets or sets the bonus multiplier measurement.
        /// </summary>
        public double Multiplier { get; }

        /// <summary>
        /// Gets or sets the work unit credit measurement.
        /// </summary>
        public double Credit { get; }

        /// <summary>
        /// Gets or sets the points per day (PPD) measurement.
        /// </summary>
        public double PPD { get; }

        public bool Equals(ProteinProduction other) => UPD.Equals(other.UPD) && Multiplier.Equals(other.Multiplier) && Credit.Equals(other.Credit) && PPD.Equals(other.PPD);

        public override bool Equals(object obj) => obj is ProteinProduction other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UPD.GetHashCode();
                hashCode = (hashCode * 397) ^ Multiplier.GetHashCode();
                hashCode = (hashCode * 397) ^ Credit.GetHashCode();
                hashCode = (hashCode * 397) ^ PPD.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ProteinProduction left, ProteinProduction right) => left.Equals(right);

        public static bool operator !=(ProteinProduction left, ProteinProduction right) => !left.Equals(right);
    }
}
