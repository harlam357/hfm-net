/*
 * HFM.NET - Client Configuration Class
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
   public interface IClientConfiguration
   {
      event EventHandler<ConfigurationChangedEventArgs> DictionaryChanged;
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
      /// Gets the number of clients in the configuration.
      /// </summary>
      int Count { get; }

      /// <summary>
      /// Gets all the clients in the configuration.
      /// </summary>
      /// <returns></returns>
      IEnumerable<IClient> GetClients();

      /// <summary>
      /// Gets the client with the specified key value.
      /// </summary>
      /// <param name="key">The key of the client to get.</param>
      /// <returns>The client with the specified key value or null if the key does not exist.</returns>
      IClient Get(string key);

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

      /// <summary>
      /// Removes the <see cref="T:HFM.Core.IClient"/> element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <returns>
      /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </returns>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event when successful.</remarks>
      /// <param name="key">The key of the element to remove.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
      bool Remove(string key);

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to false and raises the DictionaryChanged event if values existed before this call.</remarks>
      void Clear();
   }

   public sealed class ClientConfiguration : IClientConfiguration
   {
      #region Events

      public event EventHandler<ConfigurationChangedEventArgs> DictionaryChanged;

      private void OnDictionaryChanged(ConfigurationChangedEventArgs e)
      {
         var handler = DictionaryChanged;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      public event EventHandler<ClientEditedEventArgs> ClientEdited;

      private void OnClientEdited(ClientEditedEventArgs e)
      {
         var handler = ClientEdited;
         if (handler != null)
         {
            handler(this, e);
         }
      }

      public event EventHandler ClientDataInvalidated;

      private void OnClientDataInvalidated(EventArgs e)
      {
         var handler = ClientDataInvalidated;
         if (handler != null)
         {
            handler(this, e);
         }   
      }

      #endregion

      #region Properties

      public bool IsDirty { get; set; }

      #endregion

      private readonly IClientFactory _factory;
      private readonly Dictionary<string, IClient> _clientDictionary;
      private readonly ReaderWriterLockSlim _syncLock;

      public ClientConfiguration(IClientFactory factory)
      {
         if (factory == null) throw new ArgumentNullException("factory");

         _factory = factory;
         _clientDictionary = new Dictionary<string, IClient>();
         _syncLock = new ReaderWriterLockSlim();
      }

      public void Load(IEnumerable<ClientSettings> settingsCollection)
      {
         if (settingsCollection == null) throw new ArgumentNullException("settingsCollection");

         Clear();

         int added = 0;
         // don't enter write lock before Clear(), would result in deadlock
         _syncLock.EnterWriteLock();
         try
         {
            // add each instance to the collection
            foreach (var client in _factory.CreateCollection(settingsCollection))
            {
               if (client != null)
               {
                  client.SlotsChanged += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
                  client.RetrievalFinished += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
                  _clientDictionary.Add(client.Settings.Name, client);
                  added++;
               }
            }
         }
         finally
         {
            _syncLock.ExitWriteLock();
         }

         if (added != 0)
         {
            OnDictionaryChanged(new ConfigurationChangedEventArgs(ConfigurationChangedType.Add, null));
         }
      }

      public IEnumerable<SlotModel> Slots
      {
         get
         {
            _syncLock.EnterReadLock();
            try
            {
               return _clientDictionary.Values.SelectMany(client => client.Slots).ToArray();
            }
            finally
            {
               _syncLock.ExitReadLock();               
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

         IClient client;
         ClientEditedEventArgs e;

         _syncLock.EnterWriteLock();
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
         
            client = _clientDictionary[key];
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
            client.SlotsChanged += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
            client.RetrievalFinished += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
            
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
            _syncLock.ExitWriteLock();
         }

         IsDirty = true;
         OnClientEdited(e);
         OnDictionaryChanged(new ConfigurationChangedEventArgs(ConfigurationChangedType.Edit, client));
      }

      /// <summary>
      /// Adds an <see cref="T:HFM.Core.IClient"/> element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event.</remarks>
      /// <param name="key">The string to use as the key of the element to add.</param>
      /// <param name="value">The <see cref="T:HFM.Core.IClient"/> object to use as the value of the element to add.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
      internal void Add(string key, IClient value)
      {
         if (key == null) throw new ArgumentNullException("key");
         if (value == null) throw new ArgumentNullException("value");

         _syncLock.EnterWriteLock();
         try
         {
            value.SlotsChanged += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
            value.RetrievalFinished += (sender, args) => OnClientDataInvalidated(EventArgs.Empty);
            _clientDictionary.Add(key, value);
         }
         finally
         {
            _syncLock.ExitWriteLock();
         }

         IsDirty = true;
         OnDictionaryChanged(new ConfigurationChangedEventArgs(ConfigurationChangedType.Add, value));
      }

      [CoverageExclude]
      public bool ContainsKey(string key)
      {
         _syncLock.EnterReadLock();
         try
         {
            return _clientDictionary.ContainsKey(key);
         }
         finally
         {
            _syncLock.ExitReadLock();
         }
      }

      public bool Remove(string key)
      {
         if (key == null) throw new ArgumentNullException("key");

         bool result;
         IClient client = null;

         _syncLock.EnterWriteLock();
         try
         {
            if (_clientDictionary.ContainsKey(key))
            {
               client = _clientDictionary[key];
               client.ClearEventSubscriptions();
               client.Abort();
               // Release from the Factory
               _factory.Release(client);
            }
            result = _clientDictionary.Remove(key);
         }
         finally
         {
            _syncLock.ExitWriteLock();
         }

         if (result)
         {
            IsDirty = true;
            OnDictionaryChanged(new ConfigurationChangedEventArgs(ConfigurationChangedType.Remove, client));
         }
         return result;
      }

      [CoverageExclude]
      public IEnumerable<IClient> GetClients()
      {
         _syncLock.EnterReadLock();
         try
         {
            return _clientDictionary.Values;
         }
         finally
         {
            _syncLock.ExitReadLock();
         }
      }

      [CoverageExclude]
      public IClient Get(string key)
      {
         _syncLock.EnterReadLock();
         try
         {
            return _clientDictionary[key];
         }
         finally
         {
            _syncLock.ExitReadLock();
         }
      }

      public void Clear()
      {
         bool hasValues;

         _syncLock.EnterWriteLock();
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
            _syncLock.ExitWriteLock();
         }

         IsDirty = false;
         if (hasValues)
         {
            OnDictionaryChanged(new ConfigurationChangedEventArgs(ConfigurationChangedType.Clear, null));
         }
      }

      public int Count
      {
         [CoverageExclude]
         get
         {
            _syncLock.EnterReadLock();
            try
            {
               return _clientDictionary.Count;
            }
            finally
            {
               _syncLock.ExitReadLock();
            }
         }
      }
   }

   public enum ConfigurationChangedType
   {
      Add,
      Remove,
      Edit,
      Clear
   }

   public sealed class ConfigurationChangedEventArgs : EventArgs
   {
      private readonly ConfigurationChangedType _type;

      public ConfigurationChangedType ChangedType
      {
         get { return _type; }
      }

      private readonly IClient _client;

      public IClient Client
      {
         get { return _client; }
      }

      public ConfigurationChangedEventArgs(ConfigurationChangedType type, IClient client)
      {
         _type = type;
         _client = client;
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
