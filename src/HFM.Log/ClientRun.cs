
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HFM.Log
{
   /// <summary>
   /// A <see cref="ClientRun"/> encapsulates all the Folding@Home client log information for a single execution (run) of the client.
   /// </summary>
   public class ClientRun : IEnumerable<LogLine>
   {
      private readonly FahLog _parent;
      /// <summary>
      /// Gets the parent <see cref="FahLog"/> object.
      /// </summary>
      public FahLog Parent
      {
         get { return _parent; }
      }

      private readonly int _clientStartIndex;
      /// <summary>
      /// Gets the log line index for the starting line of this client run.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _clientStartIndex; }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ClientRun"/> class.
      /// </summary>
      /// <param name="parent">The parent <see cref="FahLog"/> object.</param>
      /// <param name="clientStartIndex">The log line index for the starting line of this client run.</param>
      public ClientRun(FahLog parent, int clientStartIndex)
      {
         _parent = parent;
         _clientStartIndex = clientStartIndex;

         _logLines = new ObservableCollection<LogLine>();
         _logLines.CollectionChanged += (s, e) => IsDirty = true;
      }

      private Dictionary<int, SlotRun> _slotRuns;
      /// <summary>
      /// Gets the collection of <see cref="SlotRun"/> objects.
      /// </summary>
      public IDictionary<int, SlotRun> SlotRuns
      {
         get { return _slotRuns ?? (_slotRuns = new Dictionary<int, SlotRun>()); }
      }

      private readonly ObservableCollection<LogLine> _logLines;
      /// <summary>
      /// Gets a collection of <see cref="LogLine"/> assigned to this client run.
      /// </summary>
      public IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      private ClientRunData _data;
      /// <summary>
      /// Gets or sets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this client run.
      /// </summary>
      public ClientRunData Data
      {
         get
         {
            if (_data == null || IsDirty)
            {
               IsDirty = false;
               _data = Parent.RunDataAggregator.GetClientRunData(this);
            }
            return _data;
         }
         set
         {
            IsDirty = false;
            _data = value;
         }
      }

      /// <summary>
      /// Gets or sets a value indicating if the <see cref="Data"/> property value is not current.
      /// </summary>
      public bool IsDirty { get; set; } = true;

      /// <summary>
      /// Returns an enumerator that iterates through the collection of log lines.
      /// </summary>
      /// <returns>An enumerator that can be used to iterate through the collection of log lines.</returns>
      public IEnumerator<LogLine> GetEnumerator()
      {
         return _logLines.Concat(_slotRuns.Values.SelectMany(x => x.UnitRuns).SelectMany(x => x.LogLines)).OrderBy(x => x.Index).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   /// <summary>
   /// Represents data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="ClientRun"/> object.
   /// </summary>
   public abstract class ClientRunData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ClientRunData"/> class.
      /// </summary>
      protected ClientRunData()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ClientRunData"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
      protected ClientRunData(ClientRunData other)
      {
         if (other == null) return;

         StartTime = other.StartTime;
      }

      /// <summary>
      /// Gets or sets the client start time.
      /// </summary>
      public DateTime StartTime { get; set; }
   }

   namespace FahClient
   {
      /// <summary>
      /// Represents v7 or newer client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="ClientRun"/> object.
      /// </summary>
      public class FahClientClientRunData : ClientRunData
      {
         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientClientRunData"/> class.
         /// </summary>
         public FahClientClientRunData()
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="FahClientClientRunData"/> class.
         /// </summary>
         /// <param name="other">The other instance from which data will be copied.</param>
         public FahClientClientRunData(FahClientClientRunData other)
            : base(other)
         {
            //if (other == null) return;
         }
      }
   }

   namespace Legacy
   {
      /// <summary>
      /// Represents v6 or prior client data aggregated from <see cref="LogLine"/> objects assigned to a <see cref="ClientRun"/> object.
      /// </summary>
      public class LegacyClientRunData : ClientRunData
      {
         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyClientRunData"/> class.
         /// </summary>
         public LegacyClientRunData()
         {

         }

         /// <summary>
         /// Initializes a new instance of the <see cref="LegacyClientRunData"/> class.
         /// </summary>
         /// <param name="other">The other instance from which data will be copied.</param>
         public LegacyClientRunData(LegacyClientRunData other)
            : base(other)
         {
            if (other == null) return;

            ClientVersion = other.ClientVersion;
            Arguments = other.Arguments;
            FoldingID = other.FoldingID;
            Team = other.Team;
            UserID = other.UserID;
            MachineID = other.MachineID;
         }

         /// <summary>
         /// Gets or sets the client version number.
         /// </summary>
         public string ClientVersion { get; set; }

         /// <summary>
         /// Gets or sets the client command line arguments.
         /// </summary>
         public string Arguments { get; set; }

         /// <summary>
         /// Gets or sets the Folding ID (Username).
         /// </summary>
         public string FoldingID { get; set; }

         /// <summary>
         /// Gets or sets the team number.
         /// </summary>
         public int Team { get; set; }

         /// <summary>
         /// Gets or sets the user ID (unique hexadecimal value).
         /// </summary>
         public string UserID { get; set; }

         /// <summary>
         /// Gets or sets the machine ID.
         /// </summary>
         public int MachineID { get; set; }
      }
   }
}