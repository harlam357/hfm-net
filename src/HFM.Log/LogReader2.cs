
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public abstract class FahLog
   {
      public static FahLog Create(LogFileType logFileType)
      {
         switch (logFileType)
         {
            case LogFileType.Legacy:
               return new LegacyLog();
            case LogFileType.FahClient:
               return new FahClientLog();
         }
         throw new ArgumentException("LogFileType unknown", "logFileType");
      }

      public static FahLog Read(IEnumerable<string> lines, LogFileType logFileType)
      {
         if (lines == null) throw new ArgumentNullException("lines");

         var fahLog = Create(logFileType);
         fahLog.AddRange(lines);
         return fahLog;
      }

      private readonly LogFileType _logFileType;

      public LogFileType LogFileType
      {
         get { return _logFileType; }
      }

      protected FahLog(LogFileType logFileType)
      {
         _logFileType = logFileType;
      }

      private Stack<ClientRun2> _clientRuns;

      public Stack<ClientRun2> ClientRuns
      {
         get { return _clientRuns ?? (_clientRuns = new Stack<ClientRun2>()); }
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
         LogLineType lineType = LogLineIdentifier.GetLogLineType(line, LogFileType);
         var logLine = new LogLine { LineRaw = line, LineType = lineType, LineIndex = _lineIndex };
         LogLineParser2.SetLogLineParser(logLine, LogFileType);
         AddLogLine(logLine);
         _lineIndex++;
      }

      protected abstract void AddLogLine(LogLine logLine);

      protected abstract void Finish();
   }

   public class LegacyLog : FahLog
   {
      private const int FoldingSlot = 0;

      private LogLineType _currentLineType;
      private UnitIndexData _unitIndexData;

      private List<LogLine> _logBuffer;

      public LegacyLog()
         : base(LogFileType.Legacy)
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
         var clientRun = ClientRuns.PeekOrDefault();
         if (clientRun == null)
         {
            return;
         }
         var slotRun = clientRun.SlotRuns.Count != 0 ? clientRun.SlotRuns[FoldingSlot] : null;
         var lastUnitRun = slotRun != null ? slotRun.UnitRuns.PeekOrDefault() : null;
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

      private ClientRun2 EnsureClientRunExists(int lineIndex, bool createNew)
      {
         if (createNew && ClientRuns.Count != 0)
         {
            Finish();
         }
         if (createNew || ClientRuns.Count == 0)
         {
            ClientRuns.Push(new ClientRun2(this, lineIndex));
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
         var previousUnitRun = slotRun.UnitRuns.PeekOrDefault();
         if (previousUnitRun != null)
         {
            previousUnitRun.EndIndex = lineIndex - 1;

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
         : base(LogFileType.FahClient)
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
            if (logLine.LineType == LogLineType.WorkUnitCleaningUp)
            {
               unitRun.EndIndex = logLine.LineIndex;
            }
            unitRun.LogLines.Add(logLine);
         }
      }

      protected override void Finish()
      {
         var clientRun = ClientRuns.PeekOrDefault();
         if (clientRun == null)
         {
            return;
         }
         foreach (var slotRun in clientRun.SlotRuns.Values)
         {
            var lastUnitRun = slotRun.UnitRuns.PeekOrDefault();
            if (lastUnitRun != null)
            {
               lastUnitRun.EndIndex = lastUnitRun.LogLines[lastUnitRun.LogLines.Count - 1].LineIndex;
            }
         }
      }

      private static bool SetLogLineProperties(LogLine logLine)
      {
         Match workUnitRunningMatch;
         if ((workUnitRunningMatch = LogRegex.FahClient.WorkUnitRunningRegex.Match(logLine.LineRaw)).Success)
         {
            logLine.QueueIndex = Int32.Parse(workUnitRunningMatch.Groups["UnitIndex"].Value);
            logLine.FoldingSlot = Int32.Parse(workUnitRunningMatch.Groups["FoldingSlot"].Value);
            logLine.TimeStamp = LogLineParser2.Common.ParseTimeStamp(workUnitRunningMatch.Groups["Timestamp"].Value);
            return true;
         }
         return false;
      }

      private ClientRun2 EnsureClientRunExists(int lineIndex)
      {
         if (ClientRuns.Count == 0)
         {
            ClientRuns.Push(new ClientRun2(this, lineIndex));
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
         return slotRun.UnitRuns.FirstOrDefault(x => x.QueueIndex == queueIndex && !x.EndIndex.HasValue);
      }
   }

   public class ClientRun2
   {
      private readonly FahLog _parent;

      public FahLog Parent
      {
         get { return _parent; }
      }

      private readonly int _clientStartIndex;
      /// <summary>
      /// Log line index of the starting line for this Client Run.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _clientStartIndex; }
      }

      /// <summary>
      /// ClientRun Constructor
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="clientStartIndex">Log line index of the starting line for this Client Run.</param>
      public ClientRun2(FahLog parent, int clientStartIndex)
      {
         _parent = parent;
         _clientStartIndex = clientStartIndex;
      }

      private Dictionary<int, SlotRun> _slotRuns;

      public IDictionary<int, SlotRun> SlotRuns
      {
         get { return _slotRuns ?? (_slotRuns = new Dictionary<int, SlotRun>()); }
      }

      private List<LogLine> _logLines;

      public IList<LogLine> LogLines
      {
         get { return _logLines ?? (_logLines = new List<LogLine>()); }
      }

      private ClientRun2Data _data;

      public ClientRun2Data Data
      {
         get
         {
            if (_data == null)
            {
               Process();
            }
            return _data;
         }
         internal set { _data = value; }
      }

      public void Process()
      {
         _data = LogInterpreter2.GetClientRunData(this);
      }
   }

   public class ClientRun2Data
   {
      /// <summary>
      /// Gets or sets the client start time.
      /// </summary>
      public DateTime StartTime { get; set; }

      /// <summary>
      /// Client Version Number
      /// </summary>
      public string ClientVersion { get; set; }

      /// <summary>
      /// Client Command Line Arguments
      /// </summary>
      public string Arguments { get; set; }

      /// <summary>
      /// Folding ID (User name)
      /// </summary>
      public string FoldingID { get; set; }

      /// <summary>
      /// Team Number
      /// </summary>
      public int Team { get; set; }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      public string UserID { get; set; }

      /// <summary>
      /// Machine ID
      /// </summary>
      public int MachineID { get; set; }
   }

   public class SlotRun
   {
      private readonly ClientRun2 _parent;

      public ClientRun2 Parent
      {
         get { return _parent; }
      }

      private readonly int _foldingSlot;

      public int FoldingSlot
      {
         get { return _foldingSlot; }
      }

      public SlotRun(ClientRun2 parent, int foldingSlot)
      {
         _parent = parent;
         _foldingSlot = foldingSlot;
      }

      private Stack<UnitRun> _unitRuns;
      /// <summary>
      /// Gets a stack of unit runs.
      /// </summary>
      public Stack<UnitRun> UnitRuns
      {
         get { return _unitRuns ?? (_unitRuns = new Stack<UnitRun>()); }
      }

      private SlotRunData _data;

      public SlotRunData Data
      {
         get
         {
            if (_data == null)
            {
               Process();
            }
            return _data;
         }
         internal set { _data = value; }
      }

      public void Process()
      {
         _data = LogInterpreter2.GetSlotRunData(this);
      }
   }

   public class SlotRunData
   {
      /// <summary>
      /// Number of Completed Units for this Client Run
      /// </summary>
      public int CompletedUnits { get; set; }

      /// <summary>
      /// Number of Failed Units for this Client Run
      /// </summary>
      public int FailedUnits { get; set; }

      /// <summary>
      /// Total Number of Completed Units (for the life of the client - as reported in the FAHlog.txt file)
      /// </summary>
      public int? TotalCompletedUnits { get; set; }

      /// <summary>
      /// Client Status
      /// </summary>
      public SlotStatus Status { get; set; }
   }

   public class UnitRun
   {
      private readonly SlotRun _parent;

      public SlotRun Parent
      {
         get { return _parent; }
      }

      public UnitRun(SlotRun parent)
      {
         _parent = parent;
      }

      internal UnitRun(SlotRun parent, int? queueIndex, int? startIndex, int? endIndex)
      {
         _parent = parent;
         QueueIndex = queueIndex;
         StartIndex = startIndex;
         EndIndex = endIndex;
      }

      private readonly List<LogLine> _logLines = new List<LogLine>();

      public IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      public int? FoldingSlot
      {
         get { return _parent != null ? (int?)_parent.FoldingSlot : null; }
      }

      public int? QueueIndex { get; set; }

      public int? StartIndex { get; set; }

      public int? EndIndex { get; set; }

      private UnitRunData _data;

      public UnitRunData Data
      {
         get
         {
            if (_data == null)
            {
               Process();
            }
            return _data;
         }
         internal set { _data = value; }
      }

      public void Process()
      {
         _data = LogInterpreter2.GetUnitRunData(this);
      }
   }

   public class UnitRunData : IProjectInfo
   {
      public UnitRunData()
      {
         ProjectInfoList = new List<IProjectInfo>();
      }

      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      public TimeSpan? UnitStartTimeStamp { get; set; }

      ///// <summary>
      ///// List of Log Lines containing Frame Data
      ///// </summary>
      //public IList<LogLine> FrameDataList { get; set; }

      /// <summary>
      /// Number of Frames Observed since Last Unit Start or Resume from Pause
      /// </summary>
      public int FramesObserved { get; set; }

      /// <summary>
      /// Core Version
      /// </summary>
      public float CoreVersion { get; set; }

      /// <summary>
      /// Project Info List Current Index
      /// </summary>
      public int? ProjectInfoIndex { get; set; }

      /// <summary>
      /// Project Info List
      /// </summary>
      public IList<IProjectInfo> ProjectInfoList { get; private set; }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID
      {
         get
         {
            if (ProjectInfoIndex == null)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectID;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex.Value].ProjectID;
         }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public int ProjectRun
      {
         get
         {
            if (ProjectInfoIndex == null)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectRun;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex.Value].ProjectRun;
         }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public int ProjectClone
      {
         get
         {
            if (ProjectInfoIndex == null)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectClone;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex.Value].ProjectClone;
         }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public int ProjectGen
      {
         get
         {
            if (ProjectInfoIndex == null)
            {
               if (ProjectInfoList.Count > 0)
               {
                  return ProjectInfoList[ProjectInfoList.Count - 1].ProjectGen;
               }
               return 0;
            }

            return ProjectInfoList[ProjectInfoIndex.Value].ProjectGen;
         }
      }

      /// <summary>
      /// Number of threads specified in the call to the FahCore process.
      /// </summary>
      public int Threads { get; set; }

      public WorkUnitResult WorkUnitResult { get; set; }

      public int? TotalCompletedUnits { get; set; }
   }
}
