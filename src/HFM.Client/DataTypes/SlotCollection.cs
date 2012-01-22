/*
 * HFM.NET - Slot Collection Data Class
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
using HFM.Client.Converters;
using Newtonsoft.Json.Linq;

namespace HFM.Client.DataTypes
{
   public sealed class SlotCollection : TypedMessageCollection, IList<Slot>
   {
      private readonly List<Slot> _slots;

      public SlotCollection()
      {
         _slots = new List<Slot>();
      }

      /// <summary>
      /// Fill the SlotCollection object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Fill<Slot>(message);
      }

      /// <summary>
      /// Fill the SlotCollection object with data from the given JsonMessage.
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

            var slot = Activator.CreateInstance<T>() as Slot;
            if (slot == null)
            {
               throw new InvalidCastException(String.Format(CultureInfo.CurrentCulture, 
                  "Type {0} cannot be converted to type Slot.", typeof(T)));
            }

            var propertySetter = new MessagePropertySetter(slot);
            foreach (var prop in JObject.Parse(token.ToString()).Properties())
            {
               propertySetter.SetProperty(prop);
            }
            Add(slot);
         }
         SetMessageValues(message);
         foreach (var slot in this)
         {
            slot.SlotOptions.SetMessageValues(message);
         }
      }

      #region IList<Slot> Members

      /// <summary>
      /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
      /// </summary>
      /// <returns>
      /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
      /// </returns>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
      public int IndexOf(Slot item)
      {
         return _slots.IndexOf(item);
      }

      /// <summary>
      /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param><param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public void Insert(int index, Slot item)
      {
         _slots.Insert(index, item);
      }

      /// <summary>
      /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
      /// </summary>
      /// <param name="index">The zero-based index of the item to remove.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public void RemoveAt(int index)
      {
         _slots.RemoveAt(index);
      }

      /// <summary>
      /// Gets or sets the element at the specified index.
      /// </summary>
      /// <returns>
      /// The element at the specified index.
      /// </returns>
      /// <param name="index">The zero-based index of the element to get or set.</param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IList`1"/> is read-only.</exception>
      public Slot this[int index]
      {
         get { return _slots[index]; }
         set { _slots[index] = value; }
      }

      #endregion

      #region ICollection<Slot> Members

      /// <summary>
      /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
      public void Add(Slot item)
      {
         _slots.Add(item);
      }

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
      public void Clear()
      {
         _slots.Clear();
      }

      /// <summary>
      /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
      /// </returns>
      /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Contains(Slot item)
      {
         return _slots.Contains(item);
      }

#pragma warning disable 1584,1711,1572,1581,1580

      /// <summary>
      /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
      public void CopyTo(Slot[] array, int arrayIndex)
      {
         _slots.CopyTo(array, arrayIndex);
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
         get { return _slots.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </summary>
      /// <returns>
      /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
      /// </returns>
      bool ICollection<Slot>.IsReadOnly
      {
         get { return ((ICollection<Slot>)_slots).IsReadOnly; }
      }

      /// <summary>
      /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
      public bool Remove(Slot item)
      {
         return _slots.Remove(item);
      }

      #endregion

      #region IEnumerable<Slot> Members

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public IEnumerator<Slot> GetEnumerator()
      {
         return _slots.GetEnumerator();
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

   public class Slot : ITypedMessageObject
   {
      public Slot()
      {
         SlotOptions = new SlotOptions();
         _errors = new List<MessagePropertyConversionError>();
      }

      #region Properties

      [MessageProperty("id")]
      public int Id { get; set; }

      // same value in Unit.State
      [MessageProperty("status")]
      public string Status { get; set; }

      [MessageProperty("status", typeof(SlotStatusConverter))]
      public FahSlotStatus StatusEnum { get; set; }

      [MessageProperty("description")]
      public string Description { get; set; }

      [MessageProperty("options")]
      public SlotOptions SlotOptions { get; set; }

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
