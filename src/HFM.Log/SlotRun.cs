
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Log
{
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
               _data = Parent.Parent.RunDataAggregator.GetSlotRunData(this);
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
}