using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public enum FahLogType
   {
      Legacy,
      FahClient
   }

   public abstract class FahLog : IEnumerable<LogLine>
   {
      public static FahLog Create(FahLogType fahLogType)
      {
         switch (fahLogType)
         {
            case FahLogType.Legacy:
               return new LegacyLog();
            case FahLogType.FahClient:
               return new FahClientLog();
         }
         throw new ArgumentException("FahLogType unknown", "fahLogType");
      }

      public static FahLog Read(IEnumerable<string> lines, FahLogType fahLogType)
      {
         if (lines == null) throw new ArgumentNullException("lines");

         var fahLog = Create(fahLogType);
         fahLog.AddRange(lines);
         return fahLog;
      }

      private readonly FahLogType _fahLogType;

      public FahLogType FahLogType
      {
         get { return _fahLogType; }
      }

      protected FahLog(FahLogType fahLogType)
      {
         _fahLogType = fahLogType;
      }

      private Stack<ClientRun> _clientRuns;

      public Stack<ClientRun> ClientRuns
      {
         get { return _clientRuns ?? (_clientRuns = new Stack<ClientRun>()); }
      }

      private int _lineIndex;

      public void AddRange(IEnumerable<string> lines)
      {
         foreach (var line in lines)
         {
            AddInternal(line);
         }
         Finish();
      }

      public void Add(string line)
      {
         AddInternal(line);
         Finish();
      }

      private void AddInternal(string line)
      {
         LogLineType lineType = LogLineIdentifier.GetLogLineType(line, FahLogType);
         var logLine = new LogLine { LineRaw = line, LineType = lineType, LineIndex = _lineIndex };
         LogLineParser.SetLogLineParser(logLine, FahLogType);
         AddLogLine(logLine);
         _lineIndex++;
      }

      public void Clear()
      {
         if (_clientRuns != null)
         {
            _clientRuns.Clear();
         }
         _lineIndex = 0;
      }

      protected abstract void AddLogLine(LogLine logLine);

      protected abstract void Finish();

      public IEnumerator<LogLine> GetEnumerator()
      {
         return ClientRuns.Reverse().SelectMany(x => x).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   public class LegacyLog : FahLog
   {
      private const int FoldingSlot = 0;

      private LogLineType _currentLineType;
      private UnitIndexData _unitIndexData;

      private List<LogLine> _logBuffer;

      public LegacyLog()
         : base(FahLogType.Legacy)
      {
         _unitIndexData.Initialize();
      }

      protected override void AddLogLine(LogLine logLine)
      {
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
            case LogLineType.WorkUnitStart:
               HandleWorkUnitStart(logLine);
               break;
            case LogLineType.WorkUnitRunning:
               HandleWorkUnitRunning(logLine);
               break;
            case LogLineType.WorkUnitPaused:
               HandleWorkUnitPaused(logLine);
               break;
         }
      }

      protected override void Finish()
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
               lastUnitRun.EndIndex = _logBuffer[_logBuffer.Count - 1].LineIndex;
               foreach (var logLine in _logBuffer.Where(x => x.LineIndex < lastUnitRun.StartIndex))
               {
                  clientRun.LogLines.Add(logLine);
               }
               foreach (var logLine in _logBuffer.Where(x => x.LineIndex >= lastUnitRun.StartIndex && x.LineIndex <= lastUnitRun.EndIndex))
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
         EnsureSlotRunExists(logLine.LineIndex, FoldingSlot, true);
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

         // Otherwise, if we see a LogHeader and the preceeding line was not
         // a LogOpen or a LogHeader, then we use this as a signal to create
         // a new ClientRun.  This is a backup option and I don't expect this
         // situtation to happen at all if the log file is not corrupt.
         EnsureSlotRunExists(logLine.LineIndex, FoldingSlot, true);

         _currentLineType = logLine.LineType;
      }

      private void HandleWorkUnitProcessing(LogLine logLine)
      {
         // If we have not found a ProcessingIndex (== -1) then set it.
         // Othwerwise, if ProcessingIndex (!= -1) and a CoreDownloadIndex
         // has been observerd and is greater than the current ProcessingIndex,
         // then update the ProcessingIndex to bypass the CoreDownload section
         // of the log file.
         if (_unitIndexData.ProcessingIndex == -1 ||
             (_unitIndexData.ProcessingIndex != -1 &&
              _unitIndexData.CoreDownloadIndex > _unitIndexData.ProcessingIndex))
         {
            _unitIndexData.ProcessingIndex = logLine.LineIndex;
         }

         _currentLineType = logLine.LineType;
      }

      private void HandleWorkUnitCoreDownload(LogLine logLine)
      {
         _unitIndexData.CoreDownloadIndex = logLine.LineIndex;
         _currentLineType = logLine.LineType;
      }

      private void HandleWorkUnitQueueIndex(LogLine logLine)
      {
         _unitIndexData.QueueSlotIndex = (int)logLine.LineData;
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
            _unitIndexData.WorkingIndex = logLine.LineIndex;
            _currentLineType = logLine.LineType;
         }
      }

      private void HandleWorkUnitStart(LogLine logLine)
      {
         _unitIndexData.StartIndex = logLine.LineIndex;
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
            EnsureUnitRunExists(logLine.LineIndex, _unitIndexData.QueueSlotIndex);
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
            Finish();
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
            foreach (var logLine in _logBuffer.Where(x => x.LineIndex < previousUnitRun.StartIndex))
            {
               clientRun.LogLines.Add(logLine);
            }
            foreach (var logLine in _logBuffer.Where(x => x.LineIndex >= previousUnitRun.StartIndex && x.LineIndex <= previousUnitRun.EndIndex))
            {
               previousUnitRun.LogLines.Add(logLine);
            }
            _logBuffer.RemoveAll(x => x.LineIndex <= previousUnitRun.EndIndex);
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

   public class FahClientLog : FahLog
   {
      public FahClientLog()
         : base(FahLogType.FahClient)
      {

      }

      protected override void AddLogLine(LogLine logLine)
      {
         bool isWorkUnitLogLine = SetLogLineProperties(logLine);
         if (!isWorkUnitLogLine)
         {
            var clientRun = EnsureClientRunExists(logLine.LineIndex);
            clientRun.LogLines.Add(logLine);
         }
         else
         {
            var unitRun = EnsureUnitRunExists(logLine.LineIndex, logLine.FoldingSlot, logLine.QueueIndex);
            if (logLine.LineType == LogLineType.WorkUnitCoreReturn)
            {
               var result = logLine.LineData != null ? (WorkUnitResult)logLine.LineData : default(WorkUnitResult);
               // FinishedUnit and BadWorkUnit results are the only terminating results identified in test logs
               if (result != WorkUnitResult.FinishedUnit && result != WorkUnitResult.BadWorkUnit)
               {
                  // NOT a terminating result...
                  unitRun.EndIndex = logLine.LineIndex;
                  unitRun.IsComplete = true;
               }
            }
            else if (logLine.LineType == LogLineType.WorkUnitCleaningUp)
            {
               unitRun.EndIndex = logLine.LineIndex;
               unitRun.IsComplete = true;
            }
            unitRun.LogLines.Add(logLine);
         }
      }

      protected override void Finish()
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
               unitRun.EndIndex = unitRun.LogLines[unitRun.LogLines.Count - 1].LineIndex;
            }
         }
      }

      private static bool SetLogLineProperties(LogLine logLine)
      {
         Match workUnitRunningMatch;
         if ((workUnitRunningMatch = FahLogRegex.FahClient.WorkUnitRunningRegex.Match(logLine.LineRaw)).Success)
         {
            logLine.QueueIndex = Int32.Parse(workUnitRunningMatch.Groups["UnitIndex"].Value);
            logLine.FoldingSlot = Int32.Parse(workUnitRunningMatch.Groups["FoldingSlot"].Value);
            logLine.TimeStamp = LogLineParser.Common.ParseTimeStamp(workUnitRunningMatch.Groups["Timestamp"].Value);
            return true;
         }
         return false;
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
