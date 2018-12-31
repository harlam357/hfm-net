
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Log
{
   /// <summary>
   /// A <see cref="SlotRun"/> encapsulates all the Folding@Home client log information for a single slot execution (run) of the client.
   /// </summary>
   public class SlotRun
   {
      /// <summary>
      /// Gets the parent <see cref="ClientRun"/> object.
      /// </summary>
      public ClientRun Parent { get; }

      /// <summary>
      /// Gets the folding slot number.
      /// </summary>
      public int FoldingSlot { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="SlotRun"/> class.
      /// </summary>
      /// <param name="parent">The parent <see cref="ClientRun"/> object.</param>
      /// <param name="foldingSlot">The folding slot number.</param>
      public SlotRun(ClientRun parent, int foldingSlot)
      {
         Parent = parent;
         FoldingSlot = foldingSlot;
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
   }

   /// <summary>
   /// Represents data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="SlotRun"/> object.
   /// </summary>
   public abstract class SlotRunData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="SlotRunData"/> class.
      /// </summary>
      protected SlotRunData()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="SlotRunData"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
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
         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientSlotRunData"/> class.
         /// </summary>
         public FahClientSlotRunData()
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientSlotRunData"/> class.
         /// </summary>
         /// <param name="other">The other instance from which data will be copied.</param>
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
         /// <summary>
         /// Initializes a new instance of the <see cref="LegacySlotRunData"/> class.
         /// </summary>
         public LegacySlotRunData()
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacySlotRunData"/> class.
         /// </summary>
         /// <param name="other">The other instance from which data will be copied.</param>
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
         public LegacySlotStatus Status { get; set; }
      }
   }
}