using System.Diagnostics;
using System.Globalization;
using System.Text;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Log;

namespace HFM.Core.Client
{
    public interface IWorkUnitMessageSource
    {
        Info Info { get; }

        Options Options { get; }

        UnitCollection UnitCollection { get; }

        ClientRun ClientRun { get; }
    }

    public class FahClientMessages : IWorkUnitMessageSource
    {
        public IFahClient Client { get; }

        public FahClientLogFileWriter LogFileWriter { get; }

        public FahClientMessages(IFahClient client) : this(client, new FahClientLogFileWriter(client))
        {

        }

        public FahClientMessages(IFahClient client, FahClientLogFileWriter logFileWriter)
        {
            Client = client;
            LogFileWriter = logFileWriter;
        }

        public FahClientMessage Heartbeat { get; private set; }

        public Info Info { get; private set; }

        public Options Options { get; private set; }

        public SlotCollection SlotCollection { get; private set; }

        public UnitCollection UnitCollection { get; private set; }

        public FahClientLog Log { get; } = new FahClientLog();

        private StringBuilder LogBuffer { get; set; } = new StringBuilder();

        /// <summary>
        /// Gets a value indicating when the full log has been retrieved.
        /// </summary>
        public bool LogIsRetrieved => Log.ClientRuns.Count > 0;

        public ClientRun ClientRun => Log.ClientRuns.LastOrDefault();

        public SlotRun GetSlotRun(int slotID)
        {
            var clientRun = ClientRun;
            return clientRun != null && clientRun.SlotRuns.TryGetValue(slotID, out var slotRun) ? slotRun : null;
        }

        public void Clear()
        {
            Heartbeat = null;
            Info = null;
            Options = null;
            SlotCollection = null;
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

        /// <summary>
        /// Updates the cached messages with a message received from the client.
        /// </summary>
        /// <param name="message">The message received from the client.</param>
        public async Task<bool> UpdateMessageAsync(FahClientMessage message)
        {
            bool logIsRetrieved = LogIsRetrieved;

            switch (message.Identifier.MessageType)
            {
                case FahClientMessageType.Heartbeat:
                    Heartbeat = message;
                    return true;
                case FahClientMessageType.Info:
                    Info = Info.Load(message.MessageText);
                    return true;
                case FahClientMessageType.Options:
                    Options = Options.Load(message.MessageText);
                    return true;
                case FahClientMessageType.SlotInfo:
                    await UpdateSlotCollectionFromMessage(message).ConfigureAwait(false);
                    return true;
                case FahClientMessageType.SlotOptions:
                    UpdateSlotOptionsFromMessage(message);
                    return true;
                case FahClientMessageType.QueueInfo:
                    return UpdateUnitCollectionFromMessage(message);
                case FahClientMessageType.LogRestart:
                case FahClientMessageType.LogUpdate:
                    await UpdateLogFromMessage(message).ConfigureAwait(false);
                    return logIsRetrieved != LogIsRetrieved;
                default:
                    return false;
            }
        }

        public const string DefaultSlotOptions = "slot-options {0} cpus client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage";

        private async Task UpdateSlotCollectionFromMessage(FahClientMessage message)
        {
            SlotCollection = SlotCollection.Load(message.MessageText);
            var connection = Client.Connection;

            foreach (var slot in SlotCollection)
            {
                var slotOptionsCommandText = String.Format(CultureInfo.InvariantCulture, DefaultSlotOptions, slot.ID);
                // not an expected situation at application runtime but when running some
                // tests the Connection will be null and that's fine in those scenarios
                if (connection != null)
                {
                    await connection.CreateCommand(slotOptionsCommandText).ExecuteAsync().ConfigureAwait(false);
                }
            }
        }

        private void UpdateSlotOptionsFromMessage(FahClientMessage message)
        {
            var slotOptions = SlotOptions.Load(message.MessageText);
            if (SlotCollection != null)
            {
                if (Int32.TryParse(slotOptions[Options.MachineID], out var machineID))
                {
                    var slot = SlotCollection.First(x => x.ID == machineID);
                    slot.SlotOptions = slotOptions;
                }
            }
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

            if (LogFileWriter != null)
            {
                await LogFileWriter.WriteAsync(value, mode).ConfigureAwait(false);
            }

            var connection = Client.Connection;
            // not an expected situation at application runtime but when running some
            // tests the Connection will be null and that's fine in those scenarios
            if (connection != null)
            {
                await Client.Connection.CreateCommand("queue-info").ExecuteAsync().ConfigureAwait(false);
            }
        }

        private async Task UpdateLogFromStringBuilder(StringBuilder value)
        {
            using (var textReader = new Internal.StringBuilderReader(value))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                await Log.ReadAsync(reader).ConfigureAwait(false);
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
}
