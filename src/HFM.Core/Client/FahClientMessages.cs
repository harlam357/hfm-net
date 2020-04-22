
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Log.FahClient;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public class FahClientMessages
    {
        private readonly IFahClient _client;

        public FahClientMessages(IFahClient client)
        {
            _client = client;
        }

        public FahClientMessage Heartbeat { get; private set; }

        public Info Info { get; private set; }

        public Options Options { get; private set; }

        public SlotCollection SlotCollection { get; private set; }

        public IList<SlotOptions> SlotOptionsCollection { get; } = new List<SlotOptions>();

        public UnitCollection UnitCollection { get; private set; }

        public FahClientLog Log { get; } = new FahClientLog();

        private StringBuilder LogBuffer { get; set; } = new StringBuilder();

        /// <summary>
        /// Gets a value indicating when the full log has been retrieved.
        /// </summary>
        public bool LogIsRetrieved => LogBuffer is null;

        public void Clear()
        {
            Heartbeat = null;
            Info = null;
            Options = null;
            SlotCollection = null;
            SlotOptionsCollection.Clear();
            UnitCollection = null;
            SetupLogAssetsForLogRestart();
        }

        private void SetupLogAssetsForLogRestart()
        {
            Log.Clear();
            if (LogBuffer is null || LogBuffer.Length > 0)
            {
                LogBuffer = new StringBuilder();
            }
        }

        public const int HeartbeatInterval = 60;

        public bool IsHeartbeatOverdue()
        {
            if (Heartbeat == null) return false;

            return DateTime.UtcNow.Subtract(Heartbeat.Identifier.Received).TotalMinutes >
                   TimeSpan.FromSeconds(HeartbeatInterval * 3).TotalMinutes;
        }

        public async Task SetupClientToSendMessageUpdatesAsync()
        {
            var heartbeatCommandText = String.Format(CultureInfo.InvariantCulture, "updates add 0 {0} $heartbeat", HeartbeatInterval);

            await _client.Connection.CreateCommand("updates clear").ExecuteAsync().ConfigureAwait(false);
            await _client.Connection.CreateCommand("log-updates restart").ExecuteAsync().ConfigureAwait(false);
            await _client.Connection.CreateCommand(heartbeatCommandText).ExecuteAsync().ConfigureAwait(false);
            await _client.Connection.CreateCommand("updates add 1 1 $info").ExecuteAsync().ConfigureAwait(false);
            await _client.Connection.CreateCommand("updates add 2 1 $(options -a)").ExecuteAsync().ConfigureAwait(false);
            await _client.Connection.CreateCommand("updates add 3 1 $slot-info").ExecuteAsync().ConfigureAwait(false);
            // get an initial queue reading
            await _client.Connection.CreateCommand("queue-info").ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the cached messages with a message received from the client.
        /// </summary>
        /// <param name="message">The message received from the client.</param>
        public async Task<FahClientMessagesActions> UpdateMessageAsync(FahClientMessage message)
        {
            bool slotCollectionChanged = false;
            bool unitCollectionChanged = false;
            bool logIsRetrieved = LogIsRetrieved;
            
            switch (message.Identifier.MessageType)
            {
                case FahClientMessageType.Heartbeat:
                    Heartbeat = message;
                    break;
                case FahClientMessageType.Info:
                    Info = Info.Load(message.MessageText);
                    break;
                case FahClientMessageType.Options:
                    Options = Options.Load(message.MessageText);
                    break;
                case FahClientMessageType.SlotInfo:
                    await UpdateSlotCollectionFromMessage(message).ConfigureAwait(false);
                    break;
                case FahClientMessageType.SlotOptions:
                    slotCollectionChanged = UpdateSlotOptionsCollectionFromMessage(message);
                    break;
                case FahClientMessageType.QueueInfo:
                    unitCollectionChanged = UpdateUnitCollectionFromMessage(message);
                    break;
                case FahClientMessageType.LogRestart:
                case FahClientMessageType.LogUpdate:
                    await UpdateLogFromMessage(message).ConfigureAwait(false);
                    break;
            }

            bool executeRetrieval = logIsRetrieved != LogIsRetrieved ||
                                    LogIsRetrieved && (slotCollectionChanged || unitCollectionChanged);
            return new FahClientMessagesActions(slotCollectionChanged, executeRetrieval);
        }

        public const string DefaultSlotOptions = "slot-options {0} cpus client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage";

        private async Task UpdateSlotCollectionFromMessage(FahClientMessage message)
        {
            SlotCollection = SlotCollection.Load(message.MessageText);
            SlotOptionsCollection.Clear();
            foreach (var slot in SlotCollection)
            {
                var slotOptionsCommandText = String.Format(CultureInfo.InvariantCulture, DefaultSlotOptions, slot.ID);
                await _client.Connection.CreateCommand(slotOptionsCommandText).ExecuteAsync().ConfigureAwait(false);
            }
        }

        private bool UpdateSlotOptionsCollectionFromMessage(FahClientMessage message)
        {
            SlotOptionsCollection.Add(SlotOptions.Load(message.MessageText));
            if (SlotCollection != null && SlotCollection.Count == SlotOptionsCollection.Count)
            {
                foreach (var slotOptions in SlotOptionsCollection)
                {
                    if (Int32.TryParse(slotOptions[Options.MachineID], out var machineID))
                    {
                        var slot = SlotCollection.First(x => x.ID == machineID);
                        slot.SlotOptions = slotOptions;
                    }
                }

                return true;
            }

            return false;
        }

        private bool UpdateUnitCollectionFromMessage(FahClientMessage message)
        {
            var existingUnitCollection = UnitCollection;
            UnitCollection = UnitCollection.Load(message.MessageText);
            return existingUnitCollection is null || !UnitCollectionEqualityComparer.Instance.Equals(existingUnitCollection, UnitCollection);
        }

        private async Task UpdateLogFromMessage(FahClientMessage message)
        {
            bool messageIsLogRestart = message.Identifier.MessageType == FahClientMessageType.LogRestart;
            if (messageIsLogRestart)
            {
                SetupLogAssetsForLogRestart();
            }

            var logUpdate = LogUpdate.Load(message.MessageText);
            if (LogIsRetrieved)
            {
                await UpdateLogAssetsFromStringBuilder(logUpdate.Value, FileMode.Append).ConfigureAwait(false);
            }
            else
            {
                AppendToLogBuffer(logUpdate.Value);
                if (message.MessageText.Length < UInt16.MaxValue)
                {
                    await UpdateLogAssetsFromStringBuilder(LogBuffer, FileMode.Create).ConfigureAwait(false);
                    LogBuffer = null;
                }
            }
        }

        private void AppendToLogBuffer(StringBuilder source)
        {
            foreach (var chunk in source.GetChunks())
            {
                LogBuffer.Append(chunk);
            }
        }

        private async Task UpdateLogAssetsFromStringBuilder(StringBuilder value, FileMode mode)
        {
            await UpdateLogFromStringBuilder(value).ConfigureAwait(false);
            await WriteCachedLogFileFromStringBuilder(value, mode).ConfigureAwait(false);
            await _client.Connection.CreateCommand("queue-info").ExecuteAsync().ConfigureAwait(false);
        }

        private async Task UpdateLogFromStringBuilder(StringBuilder value)
        {
            using (var textReader = new StringBuilderReader(value))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await Log.ReadAsync(reader).ConfigureAwait(false);
            }
        }
        
        private async Task WriteCachedLogFileFromStringBuilder(StringBuilder logUpdateValue, FileMode mode)
        {
            const int sleep = 100;
            const int timeout = 60 * 1000;

            var cacheDirectory = _client.Preferences.Get<string>(Preference.CacheDirectory);
            string fahLogPath = Path.Combine(cacheDirectory, _client.Settings.ClientLogFileName);

            try
            {
                if (!Directory.Exists(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                using (var stream = Internal.FileSystem.TryFileOpen(fahLogPath, mode, FileAccess.Write, FileShare.Read, sleep, timeout))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var chunk in logUpdateValue.GetChunks())
                    {
                        await writer.WriteAsync(chunk).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _client.Logger.Warn($"Failed to write to {fahLogPath}", ex);
            }
        }

        // Compare the work unit collections for equality, ignoring point and frame time properties.
        // This equality compare is used to determine when a unit's progress changes, state changes,
        // or when a work units are added/removed from the queue.
        private class UnitCollectionEqualityComparer : IEqualityComparer<UnitCollection>
        {
            public static UnitCollectionEqualityComparer Instance { get; } = new UnitCollectionEqualityComparer();

            public bool Equals(UnitCollection x, UnitCollection y)
            {
                return x == null
                    ? y == null
                    : y != null && (ReferenceEquals(x, y) || x.SequenceEqual(y, UnitEqualityComparer.Instance));
            }

            public int GetHashCode(UnitCollection obj)
            {
                throw new NotImplementedException();
            }

            private class UnitEqualityComparer : IEqualityComparer<Unit>
            {
                public static UnitEqualityComparer Instance { get; } = new UnitEqualityComparer();

                public bool Equals(Unit x, Unit y)
                {
                    return x == null
                        ? y == null
                        : y != null && EqualsInternal(x, y);
                }

                private static bool EqualsInternal(Unit x, Unit y)
                {
                    Debug.Assert(x != null);
                    Debug.Assert(y != null);

                    int xPercentDone = GetPercentDone(x.PercentDone);
                    int yPercentDone = GetPercentDone(y.PercentDone);

                    return x.ID == y.ID &&
                           x.State == y.State &&
                           x.Project == y.Project &&
                           x.Run == y.Run &&
                           x.Clone == y.Clone &&
                           x.Gen == y.Gen &&
                           x.Core == y.Core &&
                           x.UnitHex == y.UnitHex &&
                           xPercentDone == yPercentDone &&
                           x.TotalFrames == y.TotalFrames &&
                           x.FramesDone == y.FramesDone &&
                           x.Assigned == y.Assigned &&
                           x.Timeout == y.Timeout &&
                           x.Deadline == y.Deadline &&
                           x.WorkServer == y.WorkServer &&
                           x.CollectionServer == y.CollectionServer &&
                           x.WaitingOn == y.WaitingOn &&
                           x.Attempts == y.Attempts &&
                           x.NextAttempt == y.NextAttempt &&
                           x.Slot == y.Slot;
                }

                private static int GetPercentDone(string value)
                {
                    if (value == null) return 0;

                    return Double.TryParse(value.TrimEnd('%'), out var result)
                        ? (int)Math.Round(result, MidpointRounding.AwayFromZero)
                        : 0;
                }

                public int GetHashCode(Unit obj)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }

    public readonly struct FahClientMessagesActions
    {
        public FahClientMessagesActions(bool slotsUpdated, bool executeRetrieval)
        {
            SlotsUpdated = slotsUpdated;
            ExecuteRetrieval = executeRetrieval;
        }

        public bool SlotsUpdated { get; }

        public bool ExecuteRetrieval { get; }
    }
}
