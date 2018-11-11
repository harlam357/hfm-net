
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

      internal IList<LogLine> LogLines
      {
         get { return _logLines; }
      }

      private ClientRunData _data;
      /// <summary>
      /// Gets the data object containing information aggregated from the <see cref="LogLine"/> objects assigned to this client run.
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
   public class ClientRunData
   {
      /// <summary>
      /// Gets or sets the client start time.
      /// </summary>
      public DateTime StartTime { get; set; }

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