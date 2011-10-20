/*
 * HFM.NET - Instance Collection Class
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
using System.Linq;

namespace HFM.Instances
{
   public sealed class InstanceCollection : IDictionary<string, ClientInstance>
   {
      private readonly Dictionary<string, ClientInstance> _instanceCollection;

      public bool Dirty { get; private set; }

      public InstanceCollection()
      {
         _instanceCollection = new Dictionary<string, ClientInstance>();
      }

      /// <summary>
      /// Method to load ClientInstance objects from file.  This method will set the dirty flag to false.
      /// </summary>
      /// <param name="instances">Enumerable collection of ClientInstance objects.</param>
      public void LoadInstances(IEnumerable<ClientInstance> instances)
      {
         if (instances == null) throw new ArgumentNullException("instances");

         Clear();

         int added = 0;
         // add each instance to the collection
         foreach (var instance in instances)
         {
            if (instance != null)
            {
               _instanceCollection.Add(instance.Settings.InstanceName, instance);
               added++;
            }
         }
         if (added != 0)
         {
            OnCollectionChanged(EventArgs.Empty);
         }
      }

      public event EventHandler CollectionChanged;

      private void OnCollectionChanged(EventArgs e)
      {
         if (CollectionChanged != null)
         {
            CollectionChanged(this, e);
         }
      }

      #region IDictionary<string,ClientInstance> Implementation

      #region IDictionary<string,ClientInstance> Members

      public void Add(string key, ClientInstance value)
      {
         _instanceCollection.Add(key, value);

         Dirty = true;
         OnCollectionChanged(EventArgs.Empty);
      }

      public bool ContainsKey(string key)
      {
         return _instanceCollection.ContainsKey(key);
      }

      public ICollection<string> Keys
      {
         get { return _instanceCollection.Keys; }
      }

      public bool Remove(string key)
      {
         bool result = _instanceCollection.Remove(key);
         if (result)
         {
            Dirty = true;
            OnCollectionChanged(EventArgs.Empty);   
         }
         return result;
      }

      public bool TryGetValue(string key, out ClientInstance value)
      {
         return _instanceCollection.TryGetValue(key, out value);
      }

      public ICollection<ClientInstance> Values
      {
         get { return _instanceCollection.Values; }
      }

      public ClientInstance this[string key]
      {
         get { return _instanceCollection[key]; }
         set { _instanceCollection[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<string,ClientInstance>> Members

      void ICollection<KeyValuePair<string,ClientInstance>>.Add(KeyValuePair<string, ClientInstance> item)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Removes all keys and values from the collection and set the dirty flag to false.
      /// </summary>
      public void Clear()
      {
         bool hasValues = Count != 0;
         _instanceCollection.Clear();

         Dirty = false;
         if (hasValues)
         {
            OnCollectionChanged(EventArgs.Empty);   
         }
      }

      bool ICollection<KeyValuePair<string,ClientInstance>>.Contains(KeyValuePair<string, ClientInstance> item)
      {
         throw new NotImplementedException();
      }

      void ICollection<KeyValuePair<string,ClientInstance>>.CopyTo(KeyValuePair<string, ClientInstance>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get { return _instanceCollection.Count; }
      }

      bool ICollection<KeyValuePair<string,ClientInstance>>.IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      bool ICollection<KeyValuePair<string,ClientInstance>>.Remove(KeyValuePair<string, ClientInstance> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<string,ClientInstance>> Members

      IEnumerator<KeyValuePair<string, ClientInstance>> IEnumerable<KeyValuePair<string,ClientInstance>>.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         throw new NotImplementedException();
      }

      #endregion

      #endregion
   }
}
