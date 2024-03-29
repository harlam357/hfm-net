﻿using System.Diagnostics;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client;

public class ClientConfiguration : IDisposable
{
    public event EventHandler<ClientConfigurationChangedEventArgs> ClientConfigurationChanged;

    protected virtual void OnClientConfigurationChanged(ClientConfigurationChangedEventArgs e) =>
        ClientConfigurationChanged?.Invoke(this, e);

    public bool IsDirty { get; set; }

    public ClientFactory ClientFactory { get; }
    public ClientScheduledTasks ScheduledTasks { get; }

    private readonly Dictionary<string, IClient> _clientDictionary;
    private readonly ReaderWriterLockSlim _syncLock;

    public ClientConfiguration(ILogger logger, IPreferences preferences, ClientFactory clientFactory)
        : this(logger, preferences, clientFactory, ClientScheduledTasks.Factory)
    {

    }

    internal ClientConfiguration(ILogger logger, IPreferences preferences, ClientFactory clientFactory, ClientScheduledTasksFactory clientScheduledTasksFactory)
    {
        if (logger == null) throw new ArgumentNullException(nameof(logger));
        if (preferences == null) throw new ArgumentNullException(nameof(preferences));
        if (clientFactory == null) throw new ArgumentNullException(nameof(clientFactory));
        if (clientScheduledTasksFactory == null) throw new ArgumentNullException(nameof(clientScheduledTasksFactory));

        ClientFactory = clientFactory;
        ScheduledTasks = clientScheduledTasksFactory(logger, preferences, this);

        _clientDictionary = new Dictionary<string, IClient>();
        _syncLock = new ReaderWriterLockSlim();
    }

    private void OnInvalidate(object sender, EventArgs e) =>
        OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Invalidate, null));

    public void Load(IEnumerable<ClientSettings> settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        Clear();

        int added = 0;
        // don't enter write lock before Clear(), would result in deadlock
        _syncLock.EnterWriteLock();
        try
        {
            // add each instance to the collection
            foreach (var client in ClientFactory.CreateCollection(settings))
            {
                if (client != null)
                {
                    client.ClientDataChanged += OnInvalidate;
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
            OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Add, null));
        }
    }

    public ICollection<IClientData> GetClientDataCollection()
    {
        _syncLock.EnterReadLock();
        try
        {
            return _clientDictionary.Values.SelectMany(client => client.ClientDataCollection).ToList();
        }
        finally
        {
            _syncLock.ExitReadLock();
        }
    }

    public void Add(ClientSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        // cacheLock handled in Add(string, IClient)
        Add(settings.Name, ClientFactory.Create(settings));
    }

    public void Edit(string key, ClientSettings settings)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        // Edit is only called after a client setup dialog
        // has returned.  At this point the client name
        // has already been validated.  Just make sure we
        // have a value here.
        Debug.Assert(!String.IsNullOrEmpty(settings.Name));

        IClient client;

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

            client.ClientDataChanged -= OnInvalidate;
            // update the settings
            client.Settings = settings;
            // if the key changed the client object needs removed and re-added with the correct key
            if (keyChanged)
            {
                _clientDictionary.Remove(key);
                _clientDictionary.Add(settings.Name, client);
            }
            client.ClientDataChanged += OnInvalidate;
        }
        finally
        {
            _syncLock.ExitWriteLock();
        }

        IsDirty = true;
        OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Edit, client));
    }

    internal void Add(string key, IClient value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (value == null) throw new ArgumentNullException(nameof(value));

        _syncLock.EnterWriteLock();
        try
        {
            value.ClientDataChanged += OnInvalidate;
            _clientDictionary.Add(key, value);
        }
        finally
        {
            _syncLock.ExitWriteLock();
        }

        IsDirty = true;
        OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Add, value));
    }

    public bool Remove(string key)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        bool result;
        IClient client = null;

        _syncLock.EnterWriteLock();
        try
        {
            if (_clientDictionary.ContainsKey(key))
            {
                client = _clientDictionary[key];
                client.ClientDataChanged -= OnInvalidate;
                client.Close();
            }
            result = _clientDictionary.Remove(key);
            if (client is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        finally
        {
            _syncLock.ExitWriteLock();
        }

        if (result)
        {
            IsDirty = true;
            OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Remove, client));
        }
        return result;
    }

    public ICollection<IClient> GetClients()
    {
        _syncLock.EnterReadLock();
        try
        {
            return _clientDictionary.Values.ToList();
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
            var clients = _clientDictionary.Values.ToList();
            foreach (var client in clients)
            {
                client.ClientDataChanged -= OnInvalidate;
                client.Close();
            }
            _clientDictionary.Clear();
            foreach (var client in clients)
            {
                if (client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        finally
        {
            _syncLock.ExitWriteLock();
        }

        IsDirty = false;
        if (hasValues)
        {
            OnClientConfigurationChanged(new ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction.Clear, null));
        }
    }

    public int Count
    {
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

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _syncLock?.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public enum ClientConfigurationChangedAction
{
    Add,
    Remove,
    Edit,
    Clear,
    Invalidate
}

public class ClientConfigurationChangedEventArgs : EventArgs
{
    public ClientConfigurationChangedAction Action { get; }

    public IClient Client { get; }

    public ClientConfigurationChangedEventArgs(ClientConfigurationChangedAction action, IClient client)
    {
        Action = action;
        Client = client;
    }
}
