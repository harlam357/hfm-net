using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HFM.Log
{
   /// <summary>
   /// Represents a Folding@Home client log.
   /// </summary>
   public abstract class FahLog : IEnumerable<LogLine>
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

      private Stack<ClientRun> _clientRuns;
      /// <summary>
      /// Gets the collection of <see cref="ClientRun"/> objects.
      /// </summary>
      public Stack<ClientRun> ClientRuns
      {
         get { return _clientRuns ?? (_clientRuns = new Stack<ClientRun>()); }
      }

      /// <summary>
      /// Reads Folding@Home log line data from the <see cref="FahLogReader"/>.
      /// </summary>
      /// <param name="reader">The <see cref="FahLogReader"/> that reads the Folding@Home log line data.</param>
      public void Read(FahLogReader reader)
      {
         if (reader == null) throw new ArgumentNullException("reader");

         LogLine logLine;
         while ((logLine = reader.ReadLine()) != null)
         {
            OnLogLineRead(logLine);
         }
         OnReadFinished();
      }

      /// <summary>
      /// Clears the log data.
      /// </summary>
      public void Clear()
      {
         if (_clientRuns != null)
         {
            _clientRuns.Clear();
         }
      }

      /// <summary>
      /// Occurs after a <see cref="LogLine"/> was read from the <see cref="FahLogReader"/>.
      /// </summary>
      /// <param name="logLine">The <see cref="LogLine"/> that was read from the <see cref="FahLogReader"/>.</param>
      protected abstract void OnLogLineRead(LogLine logLine);

      /// <summary>
      /// Occurs after all log lines have been read from the <see cref="FahLogReader"/>.
      /// </summary>
      protected abstract void OnReadFinished();

      /// <summary>
      /// Returns an enumerator that iterates through the collection of log lines.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection of log lines.</returns>
      public IEnumerator<LogLine> GetEnumerator()
      {
         return ClientRuns.Reverse().SelectMany(x => x).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
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
            : this(LegacyRunDataAggregator.Instance)
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyLog"/> class.
         /// </summary>
         /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
         protected LegacyLog(RunDataAggregator runDataAggregator)
            : base(runDataAggregator)
         {
            _unitIndexData.Initialize();
         }

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
         /// Occurs after all log lines have been read from the <see cref="FahLogReader"/>.
         /// </summary>
         protected override void OnReadFinished()
         {
            var clientRun = ClientRuns.FirstOrDefault();
            if (clientRun == null)
            {
               return;
            }
            var slotRun = clientRun.SlotRuns.Count != 0 ? clientRun.SlotRuns[FoldingSlot] : null;
            var lastUnitRun = slotRun != null ? slotRun.UnitRuns.FirstOrDefault() : null;
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
               OnReadFinished();
            }
            if (createNew || ClientRuns.Count == 0)
            {
               ClientRuns.Push(new ClientRun(this, lineIndex));
            }
            return ClientRuns.Peek();
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
            var unitRun = new UnitRun(slotRun) { QueueIndex = queueIndex, StartIndex = lineIndex };
            var previousUnitRun = slotRun.UnitRuns.FirstOrDefault();
            if (previousUnitRun != null)
            {
               previousUnitRun.EndIndex = lineIndex - 1;
               previousUnitRun.IsComplete = true;

               var clientRun = ClientRuns.Peek();
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
            slotRun.UnitRuns.Push(unitRun);
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
            : this(FahClientRunDataAggregator.Instance)
         {
            
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientLog"/> class.
         /// </summary>
         /// <param name="runDataAggregator">The <see cref="RunDataAggregator"/> that will be used to generate <see cref="ClientRunData"/>, <see cref="SlotRunData"/>, and <see cref="UnitRunData"/> objects.</param>
         protected FahClientLog(RunDataAggregator runDataAggregator)
            : base(runDataAggregator)
         {

         }

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
               if (logLine.LineType == LogLineType.WorkUnitCoreReturn)
               {
                  var result = logLine.Data != null ? (WorkUnitResult)logLine.Data : default(WorkUnitResult);
                  // FinishedUnit and BadWorkUnit results are the only terminating results identified in test logs
                  if (result != WorkUnitResult.FinishedUnit && result != WorkUnitResult.BadWorkUnit)
                  {
                     // NOT a terminating result...
                     unitRun.EndIndex = logLine.Index;
                     unitRun.IsComplete = true;
                  }
               }
               else if (logLine.LineType == LogLineType.WorkUnitCleaningUp)
               {
                  unitRun.EndIndex = logLine.Index;
                  unitRun.IsComplete = true;
               }

               unitRun.LogLines.Add(logLine);
            }
         }

         /// <summary>
         /// Occurs after all log lines have been read from the <see cref="FahLogReader"/>.
         /// </summary>
         protected override void OnReadFinished()
         {
            var clientRun = ClientRuns.FirstOrDefault();
            if (clientRun == null)
            {
               return;
            }

            foreach (var unitRun in clientRun.SlotRuns.Values.SelectMany(x => x.UnitRuns))
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
               ClientRuns.Push(new ClientRun(this, lineIndex));
            }

            return ClientRuns.Peek();
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

         private UnitRun EnsureUnitRunExists(int lineIndex, int foldingSlot, int queueIndex)
         {
            var slotRun = EnsureSlotRunExists(lineIndex, foldingSlot);
            var unitRun = GetMostRecentUnitRun(slotRun, queueIndex);
            if (unitRun == null)
            {
               unitRun = new UnitRun(slotRun) { QueueIndex = queueIndex, StartIndex = lineIndex };
               slotRun.UnitRuns.Push(unitRun);
            }

            return unitRun;
         }

         private static UnitRun GetMostRecentUnitRun(SlotRun slotRun, int queueIndex)
         {
            return slotRun.UnitRuns.FirstOrDefault(x => x.QueueIndex == queueIndex && !x.IsComplete);
         }
      }
   }
}
