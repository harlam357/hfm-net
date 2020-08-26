using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HFM.Forms
{
    [DebuggerDisplay("{DisplayMember}, {ValueMember}")]
    public readonly struct ListItem : IEquatable<ListItem>
    {
        public static ListItem Empty { get; } = new ListItem();

        public bool IsEmpty => Equals(Empty);

        /// <summary>
        /// Initializes a new <see cref="ListItem"/> using the <paramref name="value"/> as <see cref="ValueMember"/> and <paramref name="value"/>.ToString() as <see cref="DisplayMember"/>.
        /// </summary>
        public ListItem(object value)
        {
            ValueMember = value ?? throw new ArgumentNullException(nameof(value));
            DisplayMember = value.ToString();
        }

        /// <summary>
        /// Initializes a new <see cref="ListItem"/>.
        /// </summary>
        public ListItem(string display, object value)
        {
            DisplayMember = display ?? throw new ArgumentNullException(nameof(display));
            ValueMember = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string DisplayMember { get; }

        public object ValueMember { get; }

        /// <summary>
        /// Returns <see cref="ValueMember"/> as <typeparamref name="T"/>.
        /// </summary>
        public T GetValue<T>() => (T)ValueMember;

        public bool Equals(ListItem other) => DisplayMember == other.DisplayMember && Equals(ValueMember, other.ValueMember);

        public override bool Equals(object obj) => obj is ListItem other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((DisplayMember != null ? DisplayMember.GetHashCode() : 0) * 397) ^ (ValueMember != null ? ValueMember.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ListItem left, ListItem right) => left.Equals(right);

        public static bool operator !=(ListItem left, ListItem right) => !left.Equals(right);
    }

    [DebuggerDisplay("{Value}")]
    public sealed class ValueItem<T> : IEquatable<ValueItem<T>>
    {
        public T Value { get; }

        public ValueItem(T value)
        {
            if (!typeof(T).IsValueType) throw new ArgumentException("Must be a value type.");

            Value = value;
        }

        public bool Equals(ValueItem<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || (obj is ValueItem<T> other && Equals(other));

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Value);

        public static bool operator ==(ValueItem<T> left, ValueItem<T> right) => Equals(left, right);

        public static bool operator !=(ValueItem<T> left, ValueItem<T> right) => !Equals(left, right);
    }
}
