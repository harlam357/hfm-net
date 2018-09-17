
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HFM.Log
{
   public class ClientRun : IEnumerable<LogLine>
   {
      private readonly FahLog _parent;

      public FahLog Parent
      {
         get { return _parent; }
      }

      private readonly int _clientStartIndex;
      /// <summary>
      /// Gets the log line index for the starting line of this run.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _clientStartIndex; }
      }

      /// <summary>
      ///
      /// </summary>
      /// <param name="parent"></param>
      /// <param name="clientStartIndex">The log line index for the starting line of this run.</param>
      public ClientRun(FahLog parent, int clientStartIndex)
      {
         _parent = parent;
         _clientStartIndex = clientStartIndex;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
      }

      private Dictionary<int, SlotRun> _slotRuns;

      public IDictionary<int, SlotRun> SlotRuns
      {
         get { return _slotRuns ?? (_slotRuns = new Dictionary<int, SlotRun>()); }
      }

      private readonly ObservableCollection<LogLine> _logLines;

      internal IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      private ClientRunData _data;

      public ClientRunData Data
      {
         get
         {
            if (_data == null || IsDirty)
            {
               IsDirty = false;
               _data = Parent.LogLineDataInterpreter.GetClientRunData(this);
            }
            return _data;
         }
         internal set
         {
            IsDirty = false;
            _data = value;
         }
      }

      private bool _isDirty = true;

      internal bool IsDirty
      {
         get { return _isDirty; }
         set { _isDirty = value; }
      }

      public IEnumerator<LogLine> GetEnumerator()
      {
         return _logLines.Concat(_slotRuns.Values.SelectMany(x => x.UnitRuns).SelectMany(x => x.LogLines)).OrderBy(x => x.Index).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   public class ClientRunData
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

   public class SlotRun : IEnumerable<LogLine>
   {
      private readonly ClientRun _parent;

      public ClientRun Parent
      {
         get { return _parent; }
      }

      private readonly int _foldingSlot;

      public int FoldingSlot
      {
         get { return _foldingSlot; }
      }

      public SlotRun(ClientRun parent, int foldingSlot)
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
            if (_data == null || IsDirty)
            {
               IsDirty = false;
               _data = Parent.Parent.LogLineDataInterpreter.GetSlotRunData(this);
            }
            return _data;
         }
         internal set
         {
            IsDirty = false;
            _data = value;
         }
      }

      private bool _isDirty = true;

      internal bool IsDirty
      {
         get { return _isDirty; }
         set
         {
            _isDirty = value;
            // don't push dirty flag up to ClientRun at this time
            // there is no ClientRunData that depends on SlotRun
            // or further child LogLine data
            //if (Parent != null && _isDirty)
            //{
            //   Parent.IsDirty = true;
            //}
         }
      }

      public IEnumerator<LogLine> GetEnumerator()
      {
         return _unitRuns.SelectMany(x => x.LogLines).OrderBy(x => x.Index).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
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
      public LogSlotStatus Status { get; set; }

      internal void AddWorkUnitResult(WorkUnitResult workUnitResult)
      {
         if (workUnitResult == WorkUnitResult.FinishedUnit)
         {
            CompletedUnits++;
         }
         else if (IsFailedWorkUnit(workUnitResult))
         {
            FailedUnits++;
         }
      }

      private static bool IsFailedWorkUnit(WorkUnitResult result)
      {
         switch (result)
         {
            case WorkUnitResult.EarlyUnitEnd:
            case WorkUnitResult.UnstableMachine:
            case WorkUnitResult.BadWorkUnit:
            case WorkUnitResult.ClientCoreError:
               return true;
            default:
               return false;
         }
      }
   }

   public class UnitRun : IEnumerable<LogLine>
   {
      private readonly SlotRun _parent;

      public SlotRun Parent
      {
         get { return _parent; }
      }

      private readonly ObservableCollection<LogLine> _logLines;

      internal IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      public UnitRun(SlotRun parent)
      {
         _parent = parent;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
      }

      internal UnitRun(SlotRun parent, int? queueIndex, int? startIndex, int? endIndex)
      {
         _parent = parent;
         QueueIndex = queueIndex;
         StartIndex = startIndex;
         EndIndex = endIndex;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
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
            if (_data == null || IsDirty)
            {
               IsDirty = false;
               _data = Parent.Parent.Parent.LogLineDataInterpreter.GetUnitRunData(this);
            }
            return _data;
         }
         internal set
         {
            IsDirty = false;
            _data = value;
         }
      }

      private bool _isDirty = true;

      internal bool IsDirty
      {
         get { return _isDirty; }
         set
         {
            _isDirty = value;
            if (Parent != null && _isDirty)
            {
               Parent.IsDirty = true;
            }
         }
      }

      internal bool IsComplete { get; set; }

      public IEnumerator<LogLine> GetEnumerator()
      {
         return _logLines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   public class UnitRunData
   {
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
      /// Project ID Number
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public int ProjectGen { get; set; }

      /// <summary>
      /// Number of threads specified in the call to the FahCore process.
      /// </summary>
      public int Threads { get; set; }

      public WorkUnitResult WorkUnitResult { get; set; }

      public int? TotalCompletedUnits { get; set; }
   }
}
