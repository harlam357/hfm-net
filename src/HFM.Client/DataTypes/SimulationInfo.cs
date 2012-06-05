/*
 * HFM.NET - Simulation Info Data Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

using HFM.Client.Converters;

namespace HFM.Client.DataTypes
{
   /// <summary>
   /// Folding@Home client simulation info message.
   /// </summary>
   public class SimulationInfo : TypedMessage
   {
      #region Properties

      #pragma warning disable 1591

      [MessageProperty("user")]
      public string User { get; set; }

      [MessageProperty("team")]
      public int Team { get; set; }

      [MessageProperty("project")]
      public int Project { get; set; }

      [MessageProperty("run")]
      public int Run { get; set; }

      [MessageProperty("clone")]
      public int Clone { get; set; }

      [MessageProperty("gen")]
      public int Gen { get; set; }

      [MessageProperty("core_type")]
      public int CoreType { get; set; }

      [MessageProperty("core")]
      public string Core { get; set; }

      [MessageProperty("description")]
      public string Description { get; set; }

      [MessageProperty("total_iterations")]
      public int TotalIterations { get; set; }

      [MessageProperty("iterations_done")]
      public int IterationsDone { get; set; }

      [MessageProperty("energy")]
      public int Energy { get; set; }

      [MessageProperty("temperature")]
      public int Temperature { get; set; }

      [MessageProperty("start_time")]
      public string StartTime { get; set; }

      [MessageProperty("start_time", typeof(DateTimeConverter))]
      public DateTime? StartTimeDateTime { get; set; }
      
      [MessageProperty("timeout")]
      public int Timeout { get; set; }

      [MessageProperty("timeout", typeof(SimulationInfoDateTimeConverter))]
      public DateTime? TimeoutDateTime { get; set; }

      [MessageProperty("deadline")]
      public int Deadline { get; set; }

      [MessageProperty("deadline", typeof(SimulationInfoDateTimeConverter))]
      public DateTime? DeadlineDateTime { get; set; }

      // could be TimeSpan type
      [MessageProperty("run_time")]
      public int RunTime { get; set; }

      [MessageProperty("run_time", typeof(SimulationInfoTimeSpanConverter))]
      public TimeSpan? RunTimeTimeSpan { get; set; }

      [MessageProperty("simulation_time")]
      public int SimulationTime { get; set; }

      [MessageProperty("eta")]
      public int Eta { get; set; }

      [MessageProperty("eta", typeof(SimulationInfoTimeSpanConverter))]
      public TimeSpan? EtaTimeSpan { get; set; }

      [MessageProperty("news")]
      public string News { get; set; }

      #pragma warning restore 1591

      #endregion

      /// <summary>
      /// Fill the SimulationInfo object with data from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      internal override void Fill(JsonMessage message)
      {
         Debug.Assert(message != null);

         var propertySetter = new MessagePropertySetter(this);
         foreach (var prop in JObject.Parse(message.Value.ToString()).Properties())
         {
            propertySetter.SetProperty(prop);
         }
         SetMessageValues(message);
      }
   }
}
