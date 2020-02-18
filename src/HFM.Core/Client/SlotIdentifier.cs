
using System;
using System.Globalization;

namespace HFM.Core.Client
{
    public struct SlotIdentifier : IEquatable<SlotIdentifier>, IComparable<SlotIdentifier>, IComparable
    {
        public static SlotIdentifier AllSlots => new SlotIdentifier(0, "All Slots");

        private SlotIdentifier(int ordinal, string name)
        {
            Ordinal = ordinal;
            OwningClientName = name;
            OwningSlotId = -1;
            OwningClientPath = null;
        }

        public SlotIdentifier(string owningClientName, int owningSlotId, string owningClientPath)
        {
            Ordinal = Int32.MaxValue;
            OwningClientName = owningClientName;
            OwningSlotId = owningSlotId;
            OwningClientPath = owningClientPath;
        }

        public string OwningSlotName => OwningClientName.AppendSlotId(OwningSlotId);

        public int Ordinal { get; }
        
        public string OwningClientName { get; }

        public int OwningSlotId { get; }

        public string OwningClientPath { get; }

        public override string ToString()
        {
            return String.IsNullOrWhiteSpace(OwningClientPath) 
                ? OwningSlotName 
                : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", OwningSlotName, OwningClientPath.TrimEnd('\\', '/'));
        }

        public bool Equals(SlotIdentifier other)
        {
            return Ordinal == other.Ordinal && 
                   OwningClientName == other.OwningClientName && 
                   OwningSlotId == other.OwningSlotId && OwningClientPath == 
                   other.OwningClientPath;
        }

        public override bool Equals(object obj)
        {
            return obj is SlotIdentifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ordinal;
                hashCode = (hashCode * 397) ^ (OwningClientName != null ? OwningClientName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ OwningSlotId;
                hashCode = (hashCode * 397) ^ (OwningClientPath != null ? OwningClientPath.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(SlotIdentifier left, SlotIdentifier right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SlotIdentifier left, SlotIdentifier right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(SlotIdentifier other)
        {
            var ordinalComparison = Ordinal.CompareTo(other.Ordinal);
            if (ordinalComparison != 0) return ordinalComparison;
            var owningClientNameComparison = string.Compare(OwningClientName, other.OwningClientName, StringComparison.Ordinal);
            if (owningClientNameComparison != 0) return owningClientNameComparison;
            var owningSlotIdComparison = OwningSlotId.CompareTo(other.OwningSlotId);
            if (owningSlotIdComparison != 0) return owningSlotIdComparison;
            return string.Compare(OwningClientPath, other.OwningClientPath, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            return obj is SlotIdentifier other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(SlotIdentifier)}");
        }

        public static bool operator <(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(SlotIdentifier left, SlotIdentifier right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
