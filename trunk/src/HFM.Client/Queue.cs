
using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace HFM.Client
{
   public class Queue : Dictionary<string, object>
   {
      public const string ID = "id";
      public const string State = "state";
      public const string Project = "project";
      public const string Run = "run";
      public const string Clone = "clone";
      public const string Gen = "gen";
      public const string Core = "core";
      public const string Unit = "unit";
      public const string PercentDone = "percentdone";
      public const string TotalFrames = "totalframes";
      public const string FramesDone = "framesdone";
      public const string Assigned = "assigned";
      public const string Timeout = "timeout";
      public const string Deadline = "deadline";
      public const string WorkServer = "ws";
      public const string CollectionServer = "cs";
      public const string WaitingOn = "waitingon";
      public const string Attempts = "attempts";
      public const string NextAttempt = "nextattempt";
      public const string Slot = "slot";
      public const string ETA = "eta";
      public const string PPD = "ppd";
      public const string TPF = "tpf";
      public const string BaseCredit = "basecredit";
      public const string CreditEstimate = "creditestimate";

      public static Queue Parse(string value)
      {
         var o = JObject.Parse(value);
         var queue = new Queue();
         foreach (var prop in o.Properties())
         {
            queue.Add(prop.Name, GetValue(prop));
         }
         return queue;
      }

      private static object GetValue(JProperty prop)
      {
         if (prop.Value.Type.Equals(JTokenType.String))
         {
            return (string)prop;
         }
         if (prop.Value.Type.Equals(JTokenType.Integer))
         {
            return (int)prop;
         }
         return String.Empty;
      }
   }
}
