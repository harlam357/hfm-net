/*
 * HFM.NET - Unit Collection Data Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;

using Newtonsoft.Json.Linq;

using HFM.Client.Converters;

namespace HFM.Client.DataTypes
{
   public sealed class UnitCollection : TypedMessageCollection, IList<Unit>
   {
      private readonly List<Unit> _units;

      public UnitCollection()
      {
         _units = new List<Unit>();
      }

      /// <summary>
      /// Fill the UnitCollection object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Fill<Unit>(message);
      }

      /// <summary>
      /// Fill the UnitCollection object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill<T>(JsonMessage message)
      {
         Debug.Assert(message != null);

         var jsonArray = JArray.Parse(message.Value);
         foreach (var token in jsonArray)
         {
            if (!token.HasValues)
            {
               continue;
            }

            var unit = Activator.CreateInstance<T>() as Unit;
            if (unit == null)
            {
               throw new InvalidCastException(String.Format(CultureInfo.CurrentCulture,
                  "Type {0} cannot be converted to type Unit.", typeof(T)));
            }

            var propertySetter = new MessagePropertySetter(unit);
            foreach (var prop in JObject.Parse(token.ToString()).Properties())
            {
               propertySetter.SetProperty(prop);
            }
            Add(unit);
         }
         SetMessageValues(message);
      }

      #region IList<Unit> Members

      /// <summary>
      /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
      /// </summary>
      /// <returns>
      /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
      /// </returns>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
      public int IndexOf(Unit item)
      {
         return _units.IndexOf(item);
      }

      /// <summary>
      /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public void Insert(int index, Unit item)
      {
         _units.Insert(index, item);
      }

      /// <summary>
      /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public void RemoveAt(int index)
      {
         _units.RemoveAt(index);
      }

      /// <summary>
      /// Gets or sets the element at the specified index.
      /// </summary>
      /// <returns>
      /// The element at the specified index.
      /// </returns>
      /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public Unit this[int index]
      {
         get { return _units[index]; }
         set { _units[index] = value; }
      }

      #endregion

      #region ICollection<Unit> Members

      /// <summary>
      /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
      public void Add(Unit item)
      {
         _units.Add(item);
      }

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
      public void Clear()
      {
         _units.Clear();
      }

      /// <summary>
      /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
      /// </returns>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Contains(Unit item)
      {
         return _units.Contains(item);
      }

#pragma warning disable 1584,1711,1572,1581,1580

      /// <summary>
      /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
      public void CopyTo(Unit[] array, int arrayIndex)
      {
         _units.CopyTo(array, arrayIndex);
      }

#pragma warning restore 1584,1711,1572,1581,1580

      /// <summary>
      /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      public int Count
      {
         get { return _units.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </summary>
      /// <returns>
      /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
      /// </returns>
      bool ICollection<Unit>.IsReadOnly
      {
         get { return ((ICollection<Unit>)_units).IsReadOnly; }
      }

      /// <summary>
      /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
      public bool Remove(Unit item)
      {
         return _units.Remove(item);
      }

      #endregion

      #region IEnumerable<Unit> Members

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public IEnumerator<Unit> GetEnumerator()
      {
         return _units.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }

   public class Unit : ITypedMessageObject
   {
      public Unit()
      {
         _errors = new List<MessagePropertyConversionError>();
      }

      #region Properties

      [MessageProperty("id")]
      public int Id { get; set; }

      // same value in Slot.Status
      [MessageProperty("state")]
      public string State { get; set; }

      [MessageProperty("state", typeof(SlotStatusConverter))]
      public SlotStatus StateEnum { get; set; }

      [MessageProperty("project")]
      public int Project { get; set; }

      [MessageProperty("run")]
      public int Run { get; set; }

      [MessageProperty("clone")]
      public int Clone { get; set; }

      [MessageProperty("gen")]
      public int Gen { get; set; }

      [MessageProperty("core")]
      public string Core { get; set; }

      [MessageProperty("unit")]
      public string UnitId { get; set; }

      [MessageProperty("percentdone")]
      public string PercentDone { get; set; }

      [MessageProperty("totalframes")]
      public int TotalFrames { get; set; }

      [MessageProperty("framesdone")]
      public int FramesDone { get; set; }

      // v7.1.25 - has ISO formatted values
      [MessageProperty("assigned")]
      public string Assigned { get; set; }

      [MessageProperty("assigned", typeof(DateTimeConverter))]
      public DateTime? AssignedDateTime { get; set; }

      // v7.1.25 - has ISO formatted values
      [MessageProperty("timeout")]
      public string Timeout { get; set; }

      [MessageProperty("timeout", typeof(DateTimeConverter))]
      public DateTime? TimeoutDateTime { get; set; }

      // v7.1.25 - has ISO formatted values
      [MessageProperty("deadline")]
      public string Deadline { get; set; }

      [MessageProperty("deadline", typeof(DateTimeConverter))]
      public DateTime? DeadlineDateTime { get; set; }

      [MessageProperty("ws")]
      public string WorkServer { get; set; }

      [MessageProperty("ws", typeof(IPAddressConverter))]
      public IPAddress WorkServerIPAddress { get; set; }

      [MessageProperty("cs")]
      public string CollectionServer { get; set; }

      [MessageProperty("cs", typeof(IPAddressConverter))]
      public IPAddress CollectionServerIPAddress { get; set; }

      [MessageProperty("waitingon")]
      public string WaitingOn { get; set; }

      [MessageProperty("attempts")]
      public int Attempts { get; set; }

      [MessageProperty("nextattempt")]
      public string NextAttempt { get; set; }

      [MessageProperty("nextattempt", typeof(UnitTimeSpanConverter))]
      public TimeSpan? NextAttemptTimeSpan { get; set; }

      [MessageProperty("slot")]
      public int Slot { get; set; }

      [MessageProperty("eta")]
      public string Eta { get; set; }

      // not exactly the same value seen in SimulationInfo.EtaTimeSpan
      [MessageProperty("eta", typeof(UnitTimeSpanConverter))]
      public TimeSpan? EtaTimeSpan { get; set; }

      [MessageProperty("ppd")]
      public double Ppd { get; set; }

      [MessageProperty("tpf")]
      public string Tpf { get; set; }

      [MessageProperty("tpf", typeof(UnitTimeSpanConverter))]
      public TimeSpan? TpfTimeSpan { get; set; }

      [MessageProperty("basecredit")]
      public double BaseCredit { get; set; }

      [MessageProperty("creditestimate")]
      public double CreditEstimate { get; set; }

      #endregion

      #region ITypedMessageObject Members

      private readonly List<MessagePropertyConversionError> _errors;
      /// <summary>
      /// Read-only list of property type conversion errors.
      /// </summary>
      public IEnumerable<MessagePropertyConversionError> Errors
      {
         get { return _errors.AsReadOnly(); }
      }

      void ITypedMessageObject.AddError(MessagePropertyConversionError error)
      {
         _errors.Add(error);
      }

      #endregion
   }
}
