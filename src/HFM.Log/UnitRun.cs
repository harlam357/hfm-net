
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HFM.Log
{
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