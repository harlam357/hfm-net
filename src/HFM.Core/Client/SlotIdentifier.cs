using System.Globalization;
using System.Text.RegularExpressions;

namespace HFM.Core.Client
{
    public readonly struct SlotIdentifier : IEquatable<SlotIdentifier>, IComparable<SlotIdentifier>, IComparable
    {
        public const int NoSlotID = -1;

        public static SlotIdentifier AllSlots => new(0, "All Slots");

        public bool IsAllSlots() => Equals(AllSlots);

        private SlotIdentifier(int ordinal, string name)
        {
            Ordinal = ordinal;
            ClientIdentifier = new ClientIdentifier(name, null, ClientSettings.NoPort, Guid.Empty);
            SlotID = NoSlotID;
        }

        public SlotIdentifier(ClientIdentifier clientIdentifier, int slotID)
        {
            Ordinal = Int32.MaxValue;
            ClientIdentifier = clientIdentifier;
            SlotID = slotID;
        }

        public int Ordinal { get; }

        public ClientIdentifier ClientIdentifier { get; }

        public int SlotID { get; }

        public bool HasSlotID => SlotID != NoSlotID;

        public string Name => AppendSlotID(ClientIdentifier.Name, SlotID);

        private static string AppendSlotID(string name, int slotId) =>
            slotId >= 0 ? String.Format(CultureInfo.InvariantCulture, "{0} Slot {1:00}", name, slotId) : name;

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(ClientIdentifier.Server)) return Name;
            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Name, ClientIdentifier.ToConnectionString());
        }

        public bool Equals(SlotIdentifier other) =>
            Ordinal == other.Ordinal && ClientIdentifier.Equals(other.ClientIdentifier) && SlotID == other.SlotID;

        public override bool Equals(object obj) => obj is SlotIdentifier other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ordinal;
                hashCode = (hashCode * 397) ^ ClientIdentifier.GetHashCode();
                hashCode = (hashCode * 397) ^ SlotID;
                return hashCode;
            }
        }

        public static bool operator ==(SlotIdentifier left, SlotIdentifier right) => left.Equals(right);

        public static bool operator !=(SlotIdentifier left, SlotIdentifier right) => !left.Equals(right);

        public int CompareTo(SlotIdentifier other)
        {
            var ordinalComparison = Ordinal.CompareTo(other.Ordinal);
            if (ordinalComparison != 0) return ordinalComparison;
            var clientComparison = ClientIdentifier.CompareTo(other.ClientIdentifier);
            if (clientComparison != 0) return clientComparison;
            return SlotID.CompareTo(other.SlotID);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is SlotIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(SlotIdentifier)}");
        }

        public static bool operator <(SlotIdentifier left, SlotIdentifier right) => left.CompareTo(right) < 0;

        public static bool operator >(SlotIdentifier left, SlotIdentifier right) => left.CompareTo(right) > 0;

        public static bool operator <=(SlotIdentifier left, SlotIdentifier right) => left.CompareTo(right) <= 0;

        public static bool operator >=(SlotIdentifier left, SlotIdentifier right) => left.CompareTo(right) >= 0;

        private static readonly Regex NameSlotRegex = new(@"(?<Name>.+) Slot (?<Slot>\d\d)$", RegexOptions.ExplicitCapture);

        public static SlotIdentifier FromName(string name, string connectionString, Guid guid)
        {
            var match = name is null ? null : NameSlotRegex.Match(name);
            return match is { Success: true }
                ? new SlotIdentifier(ClientIdentifier.FromConnectionString(match.Groups["Name"].Value, connectionString, guid), Int32.Parse(match.Groups["Slot"].Value))
                : new SlotIdentifier(ClientIdentifier.FromConnectionString(name, connectionString, guid), NoSlotID);
        }
    }
}
