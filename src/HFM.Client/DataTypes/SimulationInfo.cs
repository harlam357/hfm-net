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

using Newtonsoft.Json.Linq;

namespace HFM.Client.DataTypes
{
   public class SimulationInfo : TypedMessage
   {
      #region Properties

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

      [MessageProperty("simulation_time")]
      public int SimulationTime { get; set; }

      [MessageProperty("eta")]
      public int Eta { get; set; }

      [MessageProperty("eta", typeof(SimulationInfoTimeSpanConverter))]
      public TimeSpan? EtaTimeSpan { get; set; }

      [MessageProperty("news")]
      public string News { get; set; }

      #endregion

      /// <summary>
      /// Create an SimulationInfo object from the given JsonMessage.
      /// </summary>
      /// <param name="message">Message object containing JSON value and meta-data.</param>
      /// <exception cref="ArgumentNullException">Throws if message parameter is null.</exception>
      public static SimulationInfo Parse(JsonMessage message)
      {
         if (message == null) throw new ArgumentNullException("message");

         var simulationInfo = new SimulationInfo();
         var propertySetter = new MessagePropertySetter(simulationInfo);
         foreach (var prop in JObject.Parse(message.Value).Properties())
         {
            propertySetter.SetProperty(prop);
         }
         simulationInfo.SetMessageValues(message);
         return simulationInfo;
      }
   }

   #region IConversionProvider Classes

   internal sealed class SimulationInfoDateTimeConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         return new DateTime(1970, 1, 1).AddSeconds((int)input);
      }
   }

   internal sealed class SimulationInfoTimeSpanConverter : IConversionProvider
   {
      public object Convert(object input)
      {
         return new TimeSpan(0, 0, (int)input);
      }
   }

   #endregion
}
