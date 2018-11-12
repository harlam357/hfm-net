
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HFM.Log
{
   /// <summary>
   /// A <see cref="UnitRun"/> encapsulates all the Folding@Home client log information for a single work unit execution (run).
   /// </summary>
   public class UnitRun : IEnumerable<LogLine>
   {
      private readonly SlotRun _parent;
      /// <summary>
      /// Gets the parent <see cref="SlotRun"/> object.
      /// </summary>
      public SlotRun Parent
      {
         get { return _parent; }
      }

      private readonly ObservableCollection<LogLine> _logLines;
      /// <summary>
      /// Gets a collection of <see cref="LogLine"/> assigned to this unit run.
      /// </summary>
      public IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="UnitRun"/> class.
      /// </summary>
      /// <param name="parent">The parent <see cref="SlotRun"/> object.</param>
      public UnitRun(SlotRun parent)
      {
         _parent = parent;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
      }

      // for unit testing only
      internal UnitRun(SlotRun parent, int? queueIndex, int? startIndex, int? endIndex)
      {
         _parent = parent;
         QueueIndex = queueIndex;
         StartIndex = startIndex;
         EndIndex = endIndex;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
      }

      /// <summary>
      /// Gets or sets the queue index.
      /// </summary>
      public int? QueueIndex { get; set; }

      /// <summary>
      /// Gets or sets log line index for the starting line of this unit run.
      /// </summary>
      public int? StartIndex { get; set; }

      /// <summary>
      /// Gets or sets log line index for the ending line of this unit run.
      /// </summary>
      public int? EndIndex { get; set; }

      private UnitRunData _data;
      /// <summary>
      /// Gets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this unit run.
      /// </summary>
      public UnitRunData Data
      {
         get
         {
            if (_data == null || IsDirty)
            {
               IsDirty = false;
               _data = Parent.Parent.Parent.RunDataAggregator.GetUnitRunData(this);
            }
            return _data;
         }
         set
         {
            IsDirty = false;
            _data = value;
         }
      }

      private bool _isDirty = true;
      /// <summary>
      /// Gets or sets a value indicating if the <see cref="Data"/> property value is not current.
      /// </summary>
      public bool IsDirty
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

      /// <summary>
      /// Returns an enumerator that iterates through the collection of log lines.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection of log lines.</returns>
      public IEnumerator<LogLine> GetEnumerator()
      {
         return _logLines.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   /// <summary>
   /// Represents data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="UnitRun"/> object.
   /// </summary>
   public class UnitRunData
   {
      /// <summary>
      /// Gets or sets the time stamp of the start of the work unit.
      /// </summary>
      public TimeSpan? UnitStartTimeStamp { get; set; }

      /// <summary>
      /// Gets or sets the number of frames observed (completed) since last unit start or resume from pause.
      /// </summary>
      public int FramesObserved { get; set; }

      /// <summary>
      /// Gets or sets the core version number.
      /// </summary>
      public float CoreVersion { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Number).
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Run).
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Clone).
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Gen).
      /// </summary>
      public int ProjectGen { get; set; }

      /// <summary>
      /// Gets or sets the number of threads specified in the call to the FahCore process.
      /// </summary>
      public int Threads { get; set; }

      /// <summary>
      /// Gets or sets the work unit result.
      /// </summary>
      public string WorkUnitResult { get; set; }

      /// <summary>
      /// Gets or sets a value indicating if this work unit failed with a client-core communications error.
      /// </summary>
      public bool ClientCoreCommunicationsError { get; set; }

      /// <summary>
      /// Gets or sets the total number of completed units for the life of the slot.
      /// </summary>
      public int? TotalCompletedUnits { get; set; }
   }
}