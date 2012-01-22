/*
 * HFM.NET - Client Dictionary Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Linq;
using System.Threading;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IClientDictionary : IDictionary<string, IClient>
   {
      event EventHandler DictionaryChanged;
      event EventHandler<ClientDataDirtyEventArgs> ClientDataDirty;
      event EventHandler ClientRemoved;
      event EventHandler<ClientEditedEventArgs> ClientEdited;
      event EventHandler ClientDataInvalidated;

      bool IsDirty { get; set; }

      /// <summary>
      /// Clears the dictionary and loads a collection of ClientSettings objects.
      /// </summary>
      /// <remarks>This method will clear the dictionary, per implementaiton of the Clear() method, and raise the DictionaryChanged event if items were loaded.</remarks>
      /// <param name="settingsCollection"><see cref="T:System.Collections.Generic.IEnumerable`1"/> collection of ClientSettings objects.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="settingsCollection"/> is null.</exception>
      void Load(IEnumerable<ClientSettings> settingsCollection);

      /// <summary>
      /// Gets an enumerable collection of all slots.
      /// </summary>
      IEnumerable<SlotModel> Slots { get; }

      /// <summary>
      /// Adds an <see cref="T:HFM.Core.IClient"/> element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event.</remarks>
      /// <param name="settings">The <see cref="T:HFM.Core.DataTypes.ClientSettings"/> object to use as the value of the element to add.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="settings"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/> or the settings are not valid.</exception>
      void Add(ClientSettings settings);

      /// <summary>
      /// Edits an <see cref="T:HFM.Core.IClient"/> element with the provided key and in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event.</remarks>
      /// <param name="key">The string to use as the key of the element to edit.</param>
      /// <param name="settings">The <see cref="T:HFM.Core.DataTypes.ClientSettings"/> object to use as the value of the element to edit.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="settings"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">If the settings name changed, an element with the new settings name already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
      void Edit(string key, ClientSettings settings);

      #region IDictionary<string,IClient> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Adds an <see cref="T:HFM.Core.IClient"/> element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event.</remarks>
      /// <param name="key">The string to use as the key of the element to add.</param>
      /// <param name="value">The <see cref="T:HFM.Core.IClient"/> object to use as the value of the element to add.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
      new void Add(string key, IClient value);

      /// <summary>
      /// Removes the <see cref="T:HFM.Core.IClient"/> element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <returns>
      /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </returns>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event when successful.</remarks>
      /// <param name="key">The key of the element to remove.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
      new bool Remove(string key);
      
      #endregion

      #region ICollection<KeyValuePair<string,IClient>> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to false and raises the DictionaryChanged event if values existed before this call.</remarks>
      new void Clear();

      #endregion
   }

   public sealed class ClientDictionary : IClientDictionary
   {
      #region Events

      public event EventHandler DictionaryChanged;

      private void OnDictionaryChanged(EventArgs e)
      {
         if (DictionaryChanged != null)
         {
            DictionaryChanged(this, e);
         }
      }

      public event EventHandler<ClientDataDirtyEventArgs> ClientDataDirty;

      private void OnClientDataDirty(ClientDataDirtyEventArgs e)
      {
         if (ClientDataDirty != null)
         {
            ClientDataDirty(this, e);
         }
      }

      public event EventHandler ClientRemoved;

      private void OnClientRemoved(EventArgs e)
      {
         if (ClientRemoved != null)
         {
            ClientRemoved(this, e);
         }
      }

      public event EventHandler<ClientEditedEventArgs> ClientEdited;

      private void OnClientEdited(ClientEditedEventArgs e)
      {
         if (ClientEdited != null)
         {
            ClientEdited(this, e);
         }
      }

      public event EventHandler ClientDataInvalidated;

      private void OnClientDataInvalidated(EventArgs e)
      {
         if (ClientDataInvalidated != null)
         {
            ClientDataInvalidated(this, e);
         }   
      }

      #endregion

      #region Properties

      public bool IsDirty { get; set; }

      #endregion

      private readonly IClientFactory _factory;
      private readonly Dictionary<string, IClient> _clientDictionary;
      private readonly ReaderWriterLockSlim _cacheLock;

      public ClientDictionary(IClientFactory factory)
      {
         _factory = factory;
         _clientDictionary = new Dictionary<string, IClient>();
         _cacheLock = new ReaderWriterLockSlim();
      }

      public void Load(IEnumerable<ClientSettings> settingsCollection)
      {
         if (settingsCollection == null) throw new ArgumentNullException("settingsCollection");

         Clear();

         int added = 0;
         // don't enter write lock before Clear(), would result in deadlock
         _cacheLock.EnterWriteLock();
         try
         {
            // add each instance to the collection
            foreach (var client in _factory.CreateCollection(settingsCollection))
            {
               if (client != null)
               {
                  client.SlotsChanged += delegate { OnClientDataInvalidated(EventArgs.Empty); };
                  client.RetrievalFinished += delegate { OnClientDataInvalidated(EventArgs.Empty); };
                  _clientDictionary.Add(client.Settings.Name, client);
                  added++;
               }
            }
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         if (added != 0)
         {
            OnDictionaryChanged(EventArgs.Empty);
            OnClientDataDirty(new ClientDataDirtyEventArgs());
         }
      }

      public IEnumerable<SlotModel> Slots
      {
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return _clientDictionary.Values.SelectMany(client => client.Slots); // .ToList().AsReadOnly(); }
            }
            finally
            {
               _cacheLock.ExitReadLock();               
            }
         } 
      }

      public void Add(ClientSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");

         // cacheLock handled in Add(string, IClient)
         Add(settings.Name, _factory.Create(settings));
      }

      public void Edit(string key, ClientSettings settings)
      {
         if (key == null) throw new ArgumentNullException("key");
         if (settings == null) throw new ArgumentNullException("settings");

         // Edit is only called after a client setup dialog
         // has returned.  At this point the client name
         // has already been validated.  Just make sure we
         // have a value here.
         Debug.Assert(!String.IsNullOrEmpty(settings.Name));

         ClientEditedEventArgs e;

         _cacheLock.EnterWriteLock();
         try
         {
            bool keyChanged = key != settings.Name;
            if (keyChanged && _clientDictionary.ContainsKey(settings.Name))
            {
               // would like to eventually use the exact same
               // exception message that would be used by
               // the inner dictionary object.
               throw new ArgumentException("An element with the same key already exists.");
            }
         
            IClient client = _clientDictionary[key];
            string existingName = client.Settings.Name;
            string existingPath = client.Settings.DataPath();
            
            client.ClearEventSubscriptions();
            // update the settings
            client.Settings = settings;
            // if the key changed the client object
            // needs removed and readded with the 
            // correct key
            if (keyChanged)
            {
               _clientDictionary.Remove(key);
               _clientDictionary.Add(settings.Name, client);
            }
            client.SlotsChanged += delegate { OnClientDataInvalidated(EventArgs.Empty); };
            client.RetrievalFinished += delegate { OnClientDataInvalidated(EventArgs.Empty); };
            
            if (settings.IsFahClient() || settings.IsLegacy())
            {
               e = new ClientEditedEventArgs(existingName, settings.Name, existingPath, settings.DataPath());
            }
            else
            {
               // no External support yet
               throw new InvalidOperationException("Client type is not supported.");
            }
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         IsDirty = true;
         OnDictionaryChanged(EventArgs.Empty);
         OnClientEdited(e);
         OnClientDataDirty(new ClientDataDirtyEventArgs(settings.Name));
      }

      #region IDictionary<string,IClient> Members

      public void Add(string key, IClient value)
      {
         if (key == null) throw new ArgumentNullException("key");
         if (value == null) throw new ArgumentNullException("value");

         _cacheLock.EnterWriteLock();
         try
         {
            value.SlotsChanged += delegate { OnClientDataInvalidated(EventArgs.Empty); };
            value.RetrievalFinished += delegate { OnClientDataInvalidated(EventArgs.Empty); };
            _clientDictionary.Add(key, value);
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         IsDirty = true;
         OnDictionaryChanged(EventArgs.Empty);
         OnClientDataDirty(new ClientDataDirtyEventArgs(key));
      }

      [CoverageExclude]
      public bool ContainsKey(string key)
      {
         _cacheLock.EnterReadLock();
         try
         {
            return _clientDictionary.ContainsKey(key);
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public ICollection<string> Keys
      {
         [CoverageExclude]
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return _clientDictionary.Keys;
            }
            finally
            {
               _cacheLock.ExitReadLock();
            }
         }
      }

      public bool Remove(string key)
      {
         if (key == null) throw new ArgumentNullException("key");

         bool result;

         _cacheLock.EnterWriteLock();
         try
         {
            if (_clientDictionary.ContainsKey(key))
            {
               _clientDictionary[key].ClearEventSubscriptions();
               _clientDictionary[key].Abort();
            }
            result = _clientDictionary.Remove(key);
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         if (result)
         {
            IsDirty = true;
            OnDictionaryChanged(EventArgs.Empty);
            OnClientRemoved(EventArgs.Empty);
         }
         return result;
      }

      [CoverageExclude]
      public bool TryGetValue(string key, out IClient value)
      {
         _cacheLock.EnterReadLock();
         try
         {
            return _clientDictionary.TryGetValue(key, out value);
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public ICollection<IClient> Values
      {
         [CoverageExclude]
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return _clientDictionary.Values;
            }
            finally
            {
               _cacheLock.ExitReadLock();
            }
         }
      }

      public IClient this[string key]
      {
         [CoverageExclude]
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return _clientDictionary[key];
            }
            finally
            {
               _cacheLock.ExitReadLock();
            }
         }
         [CoverageExclude]
         set { throw new NotImplementedException(); }
      }

      #endregion

      #region ICollection<KeyValuePair<string,IClient>> Members

      [CoverageExclude]
      void ICollection<KeyValuePair<string, IClient>>.Add(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         bool hasValues;

         _cacheLock.EnterWriteLock();
         try
         {
            hasValues = _clientDictionary.Count != 0;
            // clear subscriptions
            foreach (var client in _clientDictionary.Values)
            {
               client.ClearEventSubscriptions();
               client.Abort();
            }
            _clientDictionary.Clear();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         IsDirty = false;
         if (hasValues)
         {
            OnDictionaryChanged(EventArgs.Empty);
         }
      }

      [CoverageExclude]
      bool ICollection<KeyValuePair<string, IClient>>.Contains(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      [CoverageExclude]
      void ICollection<KeyValuePair<string, IClient>>.CopyTo(KeyValuePair<string, IClient>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         [CoverageExclude]
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return _clientDictionary.Count;
            }
            finally
            {
               _cacheLock.ExitReadLock();
            }
         }
      }

      bool ICollection<KeyValuePair<string,IClient>>.IsReadOnly
      {
         [CoverageExclude]
         get { return false; }
      }

      [CoverageExclude]
      bool ICollection<KeyValuePair<string, IClient>>.Remove(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<string,IClient>> Members

      [CoverageExclude]
      public IEnumerator<KeyValuePair<string, IClient>> GetEnumerator()
      {
         _cacheLock.EnterReadLock();
         try
         {
            return _clientDictionary.GetEnumerator();
         }
         finally 
         {
            _cacheLock.ExitReadLock();
         }
      }

      #endregion

      #region IEnumerable Members

      [CoverageExclude]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }

   public sealed class ClientDataDirtyEventArgs : EventArgs
   {
      private readonly string _name;

      public string Name
      {
         get { return _name; }
      }

      public ClientDataDirtyEventArgs()
      {
         _name = null;
      }

      public ClientDataDirtyEventArgs(string name)
      {
         _name = name;
      }
   }

   public sealed class ClientEditedEventArgs : EventArgs
   {
      private readonly string _previousName;
      public string PreviousName
      {
         get { return _previousName; }
      }

      private readonly string _newName;
      public string NewName
      {
         get { return _newName; }
      }

      private readonly string _previousPath;
      public string PreviousPath
      {
         get { return _previousPath; }
      }

      private readonly string _newPath;
      public string NewPath
      {
         get { return _newPath; }
      }

      public ClientEditedEventArgs(string previousName, string newName,
                                   string previousPath, string newPath)
      {
         _previousName = previousName;
         _newName = newName;
         _previousPath = previousPath;
         _newPath = newPath;
      }
   }
}
