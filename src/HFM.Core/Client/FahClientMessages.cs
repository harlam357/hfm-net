
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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

        public const int HeartbeatInterval = 60;

        public FahClientMessage Heartbeat { get; private set; }

        public Info Info { get; private set; }

        public Options Options { get; private set; }

        public SlotCollection SlotCollection { get; private set; }

        public IList<SlotOptions> SlotOptionsCollection { get; } = new List<SlotOptions>();

        public UnitCollection UnitCollection { get; private set; }

        public FahClientLog Log { get; } = new FahClientLog();

        /// <summary>
        /// Gets a value indicating when the full log has been retrieved.
        /// </summary>
        public bool LogIsRetrieved { get; private set; }

        public void Clear()
        {
            Heartbeat = null;
            Info = null;
            Options = null;
            SlotCollection = null;
            SlotOptionsCollection.Clear();
            UnitCollection = null;
            Log.Clear();
            LogIsRetrieved = false;
        }

        public bool IsHeartbeatOverdue()
        {
            if (Heartbeat == null) return false;

            return DateTime.UtcNow.Subtract(Heartbeat.Identifier.Received).TotalMinutes >
                   TimeSpan.FromSeconds(HeartbeatInterval * 3).TotalMinutes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public FahClientMessagesActions Update(FahClientMessage message)
        {
            bool slotCollectionChanged = false;
            bool unitCollectionChanged = false;

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
                    HandleSlotInfoMessage(message);
                    break;
                case FahClientMessageType.SlotOptions:
                    slotCollectionChanged = HandleSlotOptionsMessage(message);
                    break;
                case FahClientMessageType.QueueInfo:
                    unitCollectionChanged = HandleQueueInfoMessage(message);
                    break;
                case FahClientMessageType.LogRestart:
                case FahClientMessageType.LogUpdate:
                {
                    HandleLogMessage(message);
                    break;
                }
            }

            return new FahClientMessagesActions(slotCollectionChanged, LogIsRetrieved && (slotCollectionChanged || unitCollectionChanged));
        }

        public const string DefaultSlotOptions = "slot-options {0} cpus client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage";

        private void HandleSlotInfoMessage(FahClientMessage message)
        {
            SlotCollection = SlotCollection.Load(message.MessageText);
            SlotOptionsCollection.Clear();
            foreach (var slot in SlotCollection)
            {
                _client.Connection.CreateCommand(String.Format(CultureInfo.InvariantCulture, DefaultSlotOptions, slot.ID)).Execute();
            }
        }

        private bool HandleSlotOptionsMessage(FahClientMessage message)
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

        private bool HandleQueueInfoMessage(FahClientMessage message)
        {
            var existingUnitCollection = UnitCollection;
            UnitCollection = UnitCollection.Load(message.MessageText);
            return existingUnitCollection is null || !UnitCollectionEqualityComparer.Instance.Equals(existingUnitCollection, UnitCollection);
        }

        private void HandleLogMessage(FahClientMessage message)
        {
            bool createNew = message.Identifier.MessageType == FahClientMessageType.LogRestart;
            if (createNew)
            {
                Log.Clear();
                LogIsRetrieved = false;
            }

            var logUpdate = LogUpdate.Load(message.MessageText);
            using (var textReader = new StringBuilderReader(logUpdate.Value))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                Log.Read(reader);
            }
            WriteToCachedLogFile(logUpdate.Value, createNew);

            if (!LogIsRetrieved && message.MessageText.Length < 65536)
            {
                LogIsRetrieved = true;
            }

            if (LogIsRetrieved)
            {
                _client.Connection.CreateCommand("queue-info").Execute();
            }
        }

        private void WriteToCachedLogFile(StringBuilder logText, bool createNew)
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
                
                using (var stream = Internal.FileSystem.TryFileOpen(fahLogPath, createNew ? FileMode.Create : FileMode.Append, FileAccess.Write, FileShare.Read, sleep, timeout))
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var chunk in logText.GetChunks())
                    {
                        writer.Write(chunk);
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