using System.Globalization;

namespace HFM.Core.WorkUnits
{
    public interface IProteinBenchmarkDetailSource
    {
        string Processor { get; }

        int? Threads { get; }
    }

    public readonly struct ProteinBenchmarkIdentifier : IEquatable<ProteinBenchmarkIdentifier>, IComparable<ProteinBenchmarkIdentifier>, IComparable
    {
        internal const string NoProcessor = null;
        public const int NoThreads = 0;

        public ProteinBenchmarkIdentifier(int projectID)
        {
            ProjectID = projectID;
            Processor = NoProcessor;
            Threads = NoThreads;
        }

        public ProteinBenchmarkIdentifier(int projectID, string processor, int threads)
        {
            ProjectID = projectID;
            Processor = processor;
            Threads = threads;
        }

        public int ProjectID { get; }

        public string Processor { get; }

        public bool HasProcessor => !String.IsNullOrWhiteSpace(Processor);

        public int Threads { get; }

        public bool HasThreads => Threads > 0;

        public override string ToString()
        {
            return !HasProcessor
                ? ProjectID.ToString(CultureInfo.InvariantCulture)
                : HasThreads
                    ? String.Format(CultureInfo.InvariantCulture, "{0} {1}:{2}", ProjectID, Processor, Threads)
                    : String.Format(CultureInfo.InvariantCulture, "{0} {1}", ProjectID, Processor);
        }

        public bool Equals(ProteinBenchmarkIdentifier other)
        {
            return ProjectID == other.ProjectID && Processor == other.Processor && Threads == other.Threads;
        }

        public override bool Equals(object obj)
        {
            return obj is ProteinBenchmarkIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ProjectID;
                hashCode = (hashCode * 397) ^ (Processor != null ? Processor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Threads;
                return hashCode;
            }
        }

        public static bool operator ==(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ProteinBenchmarkIdentifier other)
        {
            var projectIDComparison = ProjectID.CompareTo(other.ProjectID);
            if (projectIDComparison != 0) return projectIDComparison;
            var processorComparison = string.Compare(Processor, other.Processor, StringComparison.Ordinal);
            if (processorComparison != 0) return processorComparison;
            return Threads.CompareTo(other.Threads);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is ProteinBenchmarkIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(ProteinBenchmarkIdentifier)}");
        }

        public static bool operator <(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ProteinBenchmarkIdentifier left, ProteinBenchmarkIdentifier right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
