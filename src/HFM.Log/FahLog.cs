
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HFM.Log
{
   /// <summary>
   /// Represents a Folding@Home client log.
   /// </summary>
   public abstract class FahLog
   {
      /// <summary>
      /// Gets the <see cref="RunDataAggregator"/> instance.
      /// </summary>
      protected internal RunDataAggregator RunDataAggregator { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="FahLog"/> class.
      /// </summary>
      /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
      protected FahLog(RunDataAggregator runDataAggregator)
      {
         RunDataAggregator = runDataAggregator ?? throw new ArgumentNullException(nameof(runDataAggregator));
      }

      private List<ClientRun> _clientRuns;
      /// <summary>
      /// Gets the collection of <see cref="ClientRun"/> objects.
      /// </summary>
      public IList<ClientRun> ClientRuns
      {
         get { return _clientRuns ?? (_clientRuns = new List<ClientRun>()); }
      }

      /// <summary>
      /// Reads Folding@Home log line data from the <see cref="FahLogReader"/>.
      /// </summary>
      /// <param name="reader">The <see cref="FahLogReader"/> that reads the Folding@Home log line data.</param>
      public void Read(FahLogReader reader)
      {
         if (reader == null) throw new ArgumentNullException(nameof(reader));

         LogLine logLine;
         while ((logLine = reader.ReadLine()) != null)
         {
            OnLogLineRead(logLine);
         }
         OnClientRunFinished();
      }

      /// <summary>
      /// Reads Folding@Home log line data asynchronously from the <see cref="FahLogReader"/>.
      /// </summary>
      /// <param name="reader">The <see cref="FahLogReader"/> that reads the Folding@Home log line data.</param>
      public async Task ReadAsync(FahLogReader reader)
      {
         if (reader == null) throw new ArgumentNullException(nameof(reader));

         LogLine logLine;
         while ((logLine = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
         {
            OnLogLineRead(logLine);
         }
         OnClientRunFinished();
      }

      /// <summary>
      /// Clears the log data.
      /// </summary>
      public void Clear()
      {
         _clientRuns?.Clear();
      }

      /// <summary>
      /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
      /// </summary>
      /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
      protected abstract void OnLogLineRead(LogLine logLine);

      /// <summary>
      /// Occurs after log information indicates that a <see cref="ClientRun"/> has been finished.
      /// </summary>
      protected abstract void OnClientRunFinished();
   }

   namespace Legacy
   {
      /// <summary>
      /// Represents a Folding@Home client log from a v6 or prior client.
      /// </summary>
      public class LegacyLog : FahLog
      {
         private const int FoldingSlot = 0;

         private int _lineIndex;
         private LogLineType _currentLineType;
         private UnitIndexData _unitIndexData;

         private List<LogLine> _logBuffer;

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyLog"/> class.
         /// </summary>
         public LegacyLog()
            : this(null)
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyLog"/> class.
         /// </summary>
         /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
         protected LegacyLog(RunDataAggregator runDataAggregator)
            : base(runDataAggregator ?? LegacyRunDataAggregator.Instance)
         {
            _unitIndexData.Initialize();
         }

         /// <summary>
         /// Reads the log file from the given path and returns a new <see cref="LegacyLog"/> object.
         /// </summary>
         /// <param name="path">The log file path.</param>
         /// <returns>A new <see cref="LegacyLog"/> object.</returns>
         public static LegacyLog Read(string path)
         {
            using (var textReader = new StreamReader(path))
            using (var reader = new LegacyLogTextReader(textReader))
            {
               var log = new LegacyLog();
               log.Read(reader);
               return log;
            }
         }

         /// <summary>
         /// Reads the log file asynchronously from the given path and returns a new <see cref="LegacyLog"/> object.
         /// </summary>
         /// <param name="path">The log file path.</param>
         /// <returns>A new <see cref="LegacyLog"/> object.</returns>
         public static async Task<LegacyLog> ReadAsync(string path)
         {
            using (var textReader = new StreamReader(path))
            using (var reader = new LegacyLogTextReader(textReader))
            {
               var log = new LegacyLog();
               await log.ReadAsync(reader).ConfigureAwait(false);
               return log;
            }
         }

         /// <summary>
         /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
         /// </summary>
         /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
         protected override void OnLogLineRead(LogLine logLine)
         {
            logLine.Index = _lineIndex++;

            switch (logLine.LineType)
            {
               case LogLineType.LogOpen:
                  HandleLogOpen(logLine);
                  break;
               case LogLineType.LogHeader:
                  HandleLogHeader(logLine);
                  break;
            }

            if (_logBuffer == null)
            {
               _logBuffer = new List<LogLine>();
            }
            _logBuffer.Add(logLine);

            switch (logLine.LineType)
            {
               case LogLineType.WorkUnitProcessing:
                  HandleWorkUnitProcessing(logLine);
                  break;
               case LogLineType.WorkUnitCoreDownload:
                  HandleWorkUnitCoreDownload(logLine);
                  break;
               case LogLineType.WorkUnitIndex:
               case LogLineType.WorkUnitQueueIndex:
                  HandleWorkUnitQueueIndex(logLine);
                  break;
               case LogLineType.WorkUnitWorking:
                  HandleWorkUnitWorking(logLine);
                  break;
               case LogLineType.WorkUnitCoreStart:
                  HandleWorkUnitCoreStart(logLine);
                  break;
               case LogLineType.WorkUnitRunning:
                  HandleWorkUnitRunning(logLine);
                  break;
               case LogLineType.WorkUnitPaused:
                  HandleWorkUnitPaused(logLine);
                  break;
            }
         }

         /// <summary>
         /// Occurs after log information indicates that a <see cref="ClientRun"/> has been finished.
         /// </summary>
         protected override void OnClientRunFinished()
         {
            var clientRun = ClientRuns.LastOrDefault();
            if (clientRun == null)
            {
               return;
            }
            var slotRun = clientRun.SlotRuns.Count != 0 ? clientRun.SlotRuns[FoldingSlot] : null;
            var lastUnitRun = slotRun?.UnitRuns.LastOrDefault();
            if (_logBuffer != null && _logBuffer.Count != 0)
            {
               if (lastUnitRun != null)
               {
                  lastUnitRun.EndIndex = _logBuffer[_logBuffer.Count - 1].Index;
                  foreach (var logLine in _logBuffer.Where(x => x.Index < lastUnitRun.StartIndex))
                  {
                     clientRun.LogLines.Add(logLine);
                  }
                  foreach (var logLine in _logBuffer.Where(x => x.Index >= lastUnitRun.StartIndex && x.Index <= lastUnitRun.EndIndex))
                  {
                     lastUnitRun.LogLines.Add(logLine);
                  }
               }
               else
               {
                  foreach (var logLine in _logBuffer)
                  {
                     clientRun.LogLines.Add(logLine);
                  }
               }
            }
            _logBuffer = null;
         }

         private void HandleLogOpen(LogLine logLine)
         {
            EnsureSlotRunExists(logLine.Index, FoldingSlot, true);
            _currentLineType = logLine.LineType;
         }

         private void HandleLogHeader(LogLine logLine)
         {
            // If the last line observed was a LogOpen or a LogHeader, return
            // and don't use this as a signal to add a new ClientRun.
            if (_currentLineType == LogLineType.LogOpen ||
                _currentLineType == LogLineType.LogHeader)
            {
               return;
            }

            // Otherwise, if we see a LogHeader and the preceding line was not
            // a LogOpen or a LogHeader, then we use this as a signal to create
            // a new ClientRun.  This is a backup option and I don't expect this
            // situation to happen at all if the log file is not corrupt.
            EnsureSlotRunExists(logLine.Index, FoldingSlot, true);

            _currentLineType = logLine.LineType;
         }

         private void HandleWorkUnitProcessing(LogLine logLine)
         {
            // If we have not found a ProcessingIndex (== -1) then set it.
            // Otherwise, if ProcessingIndex (!= -1) and a CoreDownloadIndex
            // has been observed and is greater than the current ProcessingIndex,
            // then update the ProcessingIndex to bypass the CoreDownload section
            // of the log file.
            if (_unitIndexData.ProcessingIndex == -1 ||
                (_unitIndexData.ProcessingIndex != -1 &&
                 _unitIndexData.CoreDownloadIndex > _unitIndexData.ProcessingIndex))
            {
               _unitIndexData.ProcessingIndex = logLine.Index;
            }

            _currentLineType = logLine.LineType;
         }

         private void HandleWorkUnitCoreDownload(LogLine logLine)
         {
            _unitIndexData.CoreDownloadIndex = logLine.Index;
            _currentLineType = logLine.LineType;
         }

         private void HandleWorkUnitQueueIndex(LogLine logLine)
         {
            _unitIndexData.QueueSlotIndex = (int)logLine.Data;
         }

         private void HandleWorkUnitWorking(LogLine logLine)
         {
            // This first check allows us to overlook the "+ Working ..." message
            // that gets written after a client is Paused.  We don't want to key
            // work unit positions based on this log entry.
            if (_currentLineType == LogLineType.WorkUnitPaused)
            {
               // Return to a Running state
               _currentLineType = LogLineType.WorkUnitRunning;
            }
            else
            {
               _unitIndexData.WorkingIndex = logLine.Index;
               _currentLineType = logLine.LineType;
            }
         }

         private void HandleWorkUnitCoreStart(LogLine logLine)
         {
            _unitIndexData.StartIndex = logLine.Index;
            _currentLineType = logLine.LineType;
         }

         private void HandleWorkUnitRunning(LogLine logLine)
         {
            // If we've already seen a WorkUnitRunning line, ignore this one
            if (_currentLineType == LogLineType.WorkUnitRunning)
            {
               return;
            }

            // Not Checking the Queue Slot - we don't care if we found a valid slot or not
            if (_unitIndexData.ProcessingIndex > -1)
            {
               EnsureUnitRunExists(_unitIndexData.ProcessingIndex, _unitIndexData.QueueSlotIndex);
            }
            else if (_unitIndexData.WorkingIndex > -1)
            {
               EnsureUnitRunExists(_unitIndexData.WorkingIndex, _unitIndexData.QueueSlotIndex);
            }
            else if (_unitIndexData.StartIndex > -1)
            {
               EnsureUnitRunExists(_unitIndexData.StartIndex, _unitIndexData.QueueSlotIndex);
            }
            else
            {
               EnsureUnitRunExists(logLine.Index, _unitIndexData.QueueSlotIndex);
            }

            _currentLineType = logLine.LineType;

            // Re-initialize Values
            _unitIndexData.Initialize();
         }

         private void HandleWorkUnitPaused(LogLine logLine)
         {
            _currentLineType = logLine.LineType;
         }

         private ClientRun EnsureClientRunExists(int lineIndex, bool createNew)
         {
            if (createNew && ClientRuns.Count != 0)
            {
               OnClientRunFinished();
            }
            if (createNew || ClientRuns.Count == 0)
            {
               ClientRuns.Add(new ClientRun(this, lineIndex));
            }
            return ClientRuns.Last();
         }

         private SlotRun EnsureSlotRunExists(int lineIndex, int foldingSlot, bool createNew = false)
         {
            var clientRun = EnsureClientRunExists(lineIndex, createNew);
            if (!clientRun.SlotRuns.ContainsKey(foldingSlot))
            {
               clientRun.SlotRuns[foldingSlot] = new SlotRun(clientRun, foldingSlot);
            }
            return clientRun.SlotRuns[foldingSlot];
         }

         private void EnsureUnitRunExists(int lineIndex, int queueIndex)
         {
            var slotRun = EnsureSlotRunExists(lineIndex, FoldingSlot);
            var unitRun = new UnitRun(slotRun, queueIndex, lineIndex);
            var previousUnitRun = slotRun.UnitRuns.LastOrDefault();
            if (previousUnitRun != null)
            {
               previousUnitRun.EndIndex = lineIndex - 1;

               var clientRun = ClientRuns.Last();
               foreach (var logLine in _logBuffer.Where(x => x.Index < previousUnitRun.StartIndex))
               {
                  clientRun.LogLines.Add(logLine);
               }
               foreach (var logLine in _logBuffer.Where(x => x.Index >= previousUnitRun.StartIndex && x.Index <= previousUnitRun.EndIndex))
               {
                  previousUnitRun.LogLines.Add(logLine);
               }
               _logBuffer.RemoveAll(x => x.Index <= previousUnitRun.EndIndex);
            }
            slotRun.UnitRuns.Add(unitRun);
         }

         /// <summary>
         /// Data container for captured unit indexes
         /// </summary>
         private struct UnitIndexData
         {
            public int ProcessingIndex;
            public int CoreDownloadIndex;
            public int QueueSlotIndex;
            public int WorkingIndex;
            public int StartIndex;

            public void Initialize()
            {
               ProcessingIndex = -1;
               CoreDownloadIndex = -1;
               QueueSlotIndex = -1;
               WorkingIndex = -1;
               StartIndex = -1;
            }
         }
      }
   }

   namespace FahClient
   {
      /// <summary>
      /// Represents a Folding@Home client log from a v7 or newer client.
      /// </summary>
      public class FahClientLog : FahLog
      {
         private int _lineIndex;

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientLog"/> class.
         /// </summary>
         public FahClientLog()
            : this(null)
         {
            
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientLog"/> class.
         /// </summary>
         /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
         protected FahClientLog(RunDataAggregator runDataAggregator)
            : base(runDataAggregator ?? FahClientRunDataAggregator.Instance)
         {

         }

         /// <summary>
         /// Reads the log file from the given path and returns a new <see cref="FahClientLog"/> object.
         /// </summary>
         /// <param name="path">The log file path.</param>
         /// <returns>A new <see cref="FahClientLog"/> object.</returns>
         public static FahClientLog Read(string path)
         {
            using (var textReader = new StreamReader(path))
            using (var reader = new FahClientLogTextReader(textReader))
            {
               var log = new FahClientLog();
               log.Read(reader);
               return log;
            }
         }

         /// <summary>
         /// Reads the log file asynchronously from the given path and returns a new <see cref="FahClientLog"/> object.
         /// </summary>
         /// <param name="path">The log file path.</param>
         /// <returns>A new <see cref="FahClientLog"/> object.</returns>
         public static async Task<FahClientLog> ReadAsync(string path)
         {
            using (var textReader = new StreamReader(path))
            using (var reader = new FahClientLogTextReader(textReader))
            {
               var log = new FahClientLog();
               await log.ReadAsync(reader).ConfigureAwait(false);
               return log;
            }
         }

         /// <summary>
         /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
         /// </summary>
         /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
         protected override void OnLogLineRead(LogLine logLine)
         {
            logLine.Index = _lineIndex++;

            var properties = GetLogLineProperties(logLine);
            if (!properties.IsWorkUnitLogLine)
            {
               var clientRun = EnsureClientRunExists(logLine.Index);
               clientRun.LogLines.Add(logLine);
            }
            else
            {
               var unitRun = EnsureUnitRunExists(logLine.Index, properties.FoldingSlot, properties.QueueIndex);
               if (logLine.LineType == LogLineType.WorkUnitCleaningUp)
               {
                  unitRun.EndIndex = logLine.Index;
                  unitRun.IsComplete = true;
               }

               unitRun.LogLines.Add(logLine);
            }
         }

         /// <summary>
         /// Occurs after log information indicates that a <see cref="ClientRun"/> has been finished.
         /// </summary>
         protected override void OnClientRunFinished()
         {
            var clientRun = ClientRuns.LastOrDefault();
            if (clientRun == null)
            {
               return;
            }

            foreach (var unitRun in clientRun.SlotRuns.Values.SelectMany(x => x.UnitRuns).Cast<FahClientUnitRun>())
            {
               if (!unitRun.IsComplete)
               {
                  unitRun.EndIndex = unitRun.LogLines[unitRun.LogLines.Count - 1].Index;
               }
            }
         }

         private static LogLineProperties GetLogLineProperties(LogLine logLine)
         {
            Match workUnitRunningMatch;
            if ((workUnitRunningMatch = Internal.FahLogRegex.FahClient.WorkUnitRunningRegex.Match(logLine.Raw)).Success)
            {
               return new LogLineProperties
               {
                  IsWorkUnitLogLine = true,
                  QueueIndex = Int32.Parse(workUnitRunningMatch.Groups["UnitIndex"].Value),
                  FoldingSlot = Int32.Parse(workUnitRunningMatch.Groups["FoldingSlot"].Value)
               };
            }

            return new LogLineProperties();
         }

         private struct LogLineProperties
         {
            public bool IsWorkUnitLogLine { get; set; }
            public int QueueIndex { get; set; }
            public int FoldingSlot { get; set; }
         }

         private ClientRun EnsureClientRunExists(int lineIndex)
         {
            if (ClientRuns.Count == 0)
            {
               ClientRuns.Add(new ClientRun(this, lineIndex));
            }

            return ClientRuns.Last();
         }

         private SlotRun EnsureSlotRunExists(int lineIndex, int foldingSlot)
         {
            var clientRun = EnsureClientRunExists(lineIndex);
            if (!clientRun.SlotRuns.ContainsKey(foldingSlot))
            {
               clientRun.SlotRuns[foldingSlot] = new SlotRun(clientRun, foldingSlot);
            }

            return clientRun.SlotRuns[foldingSlot];
         }

         private FahClientUnitRun EnsureUnitRunExists(int lineIndex, int foldingSlot, int queueIndex)
         {
            var slotRun = EnsureSlotRunExists(lineIndex, foldingSlot);
            var unitRun = GetMostRecentUnitRun(slotRun, queueIndex);
            if (unitRun == null)
            {
               unitRun = new FahClientUnitRun(slotRun, queueIndex, lineIndex);
               slotRun.UnitRuns.Add(unitRun);
            }

            return unitRun;
         }

         private static FahClientUnitRun GetMostRecentUnitRun(SlotRun slotRun, int queueIndex)
         {
            return slotRun.UnitRuns
               .Cast<FahClientUnitRun>()
               .LastOrDefault(x => x.QueueIndex == queueIndex && !x.IsComplete);
         }

         /// <summary>
         /// A <see cref="FahClientUnitRun"/> encapsulates all the Folding@Home client log information for a single work unit execution (run).
         /// </summary>
         private class FahClientUnitRun : UnitRun
         {
            /// <summary>
            /// Initializes a new instance of the <see cref="FahClientUnitRun"/> class.
            /// </summary>
            /// <param name="parent">The parent <see cref="SlotRun"/> object.</param>
            /// <param name="queueIndex">The queue index.</param>
            /// <param name="startIndex">The log line index for the starting line of this unit run.</param>
            internal FahClientUnitRun(SlotRun parent, int queueIndex, int startIndex)
               : base(parent, queueIndex, startIndex)
            {

            }

            /// <summary>
            /// Gets or sets a value indicating if the <see cref="UnitRun"/> is complete.
            /// </summary>
            internal bool IsComplete { get; set; }
         }
      }
   }
}
