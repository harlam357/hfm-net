
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

      private List<UnitRun> _unitRuns;
      /// <summary>
      /// Gets the collection of <see cref="UnitRun"/> objects.
      /// </summary>
      public IList<UnitRun> UnitRuns
      {
         get { return _unitRuns ?? (_unitRuns = new List<UnitRun>()); }
      }

      private SlotRunData _data;
      /// <summary>
      /// Gets or sets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this slot run.
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
   public abstract class SlotRunData
   {
      protected SlotRunData()
      {
         
      }

      protected SlotRunData(SlotRunData other)
      {
         if (other == null) return;

         CompletedUnits = other.CompletedUnits;
         FailedUnits = other.FailedUnits;
      }

      /// <summary>
      /// Gets or sets the number of completed units.
      /// </summary>
      public int CompletedUnits { get; set; }

      /// <summary>
      /// Gets or sets the number of failed units.
      /// </summary>
      public int FailedUnits { get; set; }
   }

   namespace FahClient
   {
      /// <summary>
      /// Represents v7 or newer client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="SlotRun"/> object.
      /// </summary>
      public class FahClientSlotRunData : SlotRunData
      {
         public FahClientSlotRunData()
         {

         }

         public FahClientSlotRunData(FahClientSlotRunData other)
            : base(other)
         {
            //if (other == null) return;
         }
      }
   }

   namespace Legacy
   {
      /// <summary>
      /// Represents v6 or prior client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="SlotRun"/> object.
      /// </summary>
      public class LegacySlotRunData : SlotRunData
      {
         public LegacySlotRunData()
         {

         }

         public LegacySlotRunData(LegacySlotRunData other)
            : base(other)
         {
            if (other == null) return;

            TotalCompletedUnits = other.TotalCompletedUnits;
            Status = other.Status;
         }

         /// <summary>
         /// Gets or sets the total number of completed units for the life of the slot.
         /// </summary>
         public int? TotalCompletedUnits { get; set; }

         /// <summary>
         /// Gets or sets the client status.
         /// </summary>
         public LogSlotStatus Status { get; set; }
      }
   }
}