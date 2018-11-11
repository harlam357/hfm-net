
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HFM.Log.Internal;

namespace HFM.Log
{
   /// <summary>
   /// A <see cref="SlotRun"/> encapsulates all the Folding@Home client log information for a single slot execution (run) of the client.
   /// </summary>
   public class SlotRun : IEnumerable<LogLine>
   {
      private readonly ClientRun _parent;
      /// <summary>
      /// Gets the parent <see cref="ClientRun"/> object.
      /// </summary>
      public ClientRun Parent
      {
         get { return _parent; }
      }

      private readonly int _foldingSlot;
      /// <summary>
      /// Gets the folding slot number.
      /// </summary>
      public int FoldingSlot
      {
         get { return _foldingSlot; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SlotRun"/> class.
      /// </summary>
      /// <param name="parent">The parent <see cref="ClientRun"/> object.</param>
      /// <param name="foldingSlot">The folding slot number.</param>
      public SlotRun(ClientRun parent, int foldingSlot)
      {
         _parent = parent;
         _foldingSlot = foldingSlot;
      }

      private Stack<UnitRun> _unitRuns;
      /// <summary>
      /// Gets the collection of <see cref="UnitRun"/> objects.
      /// </summary>
      public Stack<UnitRun> UnitRuns
      {
         get { return _unitRuns ?? (_unitRuns = new Stack<UnitRun>()); }
      }

      private SlotRunData _data;
      /// <summary>
      /// Gets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this slot run.
      /// </summary>
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

      /// <summary>
      /// Returns an enumerator that iterates through the collection of log lines.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection of log lines.</returns>
      public IEnumerator<LogLine> GetEnumerator()
      {
         return _unitRuns.SelectMany(x => x.LogLines).OrderBy(x => x.Index).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   /// <summary>
   /// Represents data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="SlotRun"/> object.
   /// </summary>
   public class SlotRunData
   {
      /// <summary>
      /// Gets or sets the number of completed units.
      /// </summary>
      public int CompletedUnits { get; set; }

      /// <summary>
      /// Gets or sets the number of failed units.
      /// </summary>
      public int FailedUnits { get; set; }

      /// <summary>
      /// Gets or sets the total number of completed units for the life of the slot.
      /// </summary>
      public int? TotalCompletedUnits { get; set; }

      /// <summary>
      /// Gets or sets the client status.
      /// </summary>
      public LogSlotStatus Status { get; set; }

      internal void AddWorkUnitResult(UnitRunData unitRunData)
      {
         if (unitRunData.WorkUnitResult == WorkUnitResult.FinishedUnit)
         {
            CompletedUnits++;
         }
         else if (IsFailedWorkUnit(unitRunData.WorkUnitResult))
         {
            FailedUnits++;
         }
         else if (unitRunData.ClientCoreCommunicationsError)
         {
            FailedUnits++;
         }
      }

      private static bool IsFailedWorkUnit(string result)
      {
         switch (result)
         {
            case WorkUnitResult.EarlyUnitEnd:
            case WorkUnitResult.UnstableMachine:
            case WorkUnitResult.BadWorkUnit:
               return true;
            default:
               return false;
         }
      }
   }
}