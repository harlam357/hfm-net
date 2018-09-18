
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
}