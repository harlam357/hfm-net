/*
 * HFM.NET - Protein Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace HFM.Framework.DataTypes
{
   public interface IProtein
   {
      /// <summary>
      /// Project Number
      /// </summary>
      int ProjectNumber { get; }

      /// <summary>
      /// Server IP Address
      /// </summary>
      String ServerIP { get; }

      /// <summary>
      /// Work Unit Name
      /// </summary>
      String WorkUnitName { get; }

      /// <summary>
      /// Number of Atoms
      /// </summary>
      int NumAtoms { get; }

      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      double PreferredDays { get; }

      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      double MaxDays { get; }

      /// <summary>
      /// Work Unit Credit
      /// </summary>
      double Credit { get; }

      /// <summary>
      /// Number of Frames
      /// </summary>
      int Frames { get; }

      /// <summary>
      /// Core Identification String
      /// </summary>
      String Core { get; }

      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      String Description { get; }

      /// <summary>
      /// Project Research Contact
      /// </summary>
      String Contact { get; }

      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      double KFactor { get; }

      /// <summary>
      /// Flag Denoting if Project Number is Unknown
      /// </summary>
      bool IsUnknown { get; }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      double GetPPD(TimeSpan frameTime);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="instanceName">Calling Instance Name</param>
      double GetPPD(TimeSpan frameTime, string instanceName);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit);

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      /// <param name="instanceName">Calling Instance Name</param>
      double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit, string instanceName);

      /// <summary>
      /// Get Units Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      double GetUPD(TimeSpan frameTime);

      /// <summary>
      /// Get the Credit of the Unit (including bonus)
      /// </summary>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      double GetBonusCredit(TimeSpan estTimeOfUnit);
   }

   public class Protein : IProtein
   {
      public Protein()
      {
         ServerIP = "0.0.0.0";
         WorkUnitName = "Unknown";
         Frames = 100;
         Core = "Unknown";
         Description = Constants.UnassignedDescription;
         Contact = "Unknown";
      }

      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber { get; set; }

      /// <summary>
      /// Server IP Address
      /// </summary>
      public string ServerIP { get; set; }

      /// <summary>
      /// Work Unit Name
      /// </summary>
      public string WorkUnitName { get; set; }

      /// <summary>
      /// Number of Atoms
      /// </summary>
      public int NumAtoms { get; set; }

      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      public double PreferredDays { get; set; }

      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      public double MaxDays { get; set; }

      /// <summary>
      /// Work Unit Credit
      /// </summary>
      public double Credit { get; set; }

      /// <summary>
      /// Number of Frames
      /// </summary>
      public int Frames { get; set; }

      /// <summary>
      /// Core Identification string
      /// </summary>
      public string Core { get; set; }

      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      public string Description { get; set; }

      /// <summary>
      /// Project Research Contact
      /// </summary>
      public string Contact { get; set; }

      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      public double KFactor { get; set; }

      /// <summary>
      /// Flag Denoting if Project Number is Unknown
      /// </summary>
      public bool IsUnknown
      {
         get { return ProjectNumber == 0; }
      }
      
      public bool Valid
      {
         get
         {
            return (ProjectNumber > 0 &&
                    PreferredDays > 0 &&
                    MaxDays > 0 &&
                    Credit > 0 &&
                    Frames > 0 &&
                    KFactor >= 0);
         }
      }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      public double GetPPD(TimeSpan frameTime)
      {
         return GetPPD(frameTime, string.Empty);
      }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="instanceName">Calling Instance Name</param>
      public double GetPPD(TimeSpan frameTime, string instanceName)
      {
         return GetPPD(frameTime, TimeSpan.Zero, instanceName);
      }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      public double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit)
      {
         return GetPPD(frameTime, estTimeOfUnit, string.Empty);
      }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      /// <param name="instanceName">Calling Instance Name</param>
      public double GetPPD(TimeSpan frameTime, TimeSpan estTimeOfUnit, string instanceName)
      {
         if (frameTime.Equals(TimeSpan.Zero))
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, instanceName, "Frame Time is zero... returning 0 PPD.");
            return 0;
         }

         double basePPD = GetUPD(frameTime) * Credit;
         double bonusMulti = GetBonusMultiplier(estTimeOfUnit);
         double bonusPPD = Math.Round((basePPD * bonusMulti), Constants.MaxDecimalPlaces);
         
         var messages = new List<string>(9);

         if (string.IsNullOrEmpty(instanceName) == false)
         {
            messages.Add(string.Format(CultureInfo.CurrentCulture, "{0} ({1})", HfmTrace.FunctionName, instanceName));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Frame Time ----- : {0}", frameTime));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Credit --------- : {0}", Credit));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Base PPD ------- : {0}", basePPD));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - KFactor -------- : {0}", KFactor));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Estimated Time - : {0}", estTimeOfUnit));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Preferred Time - : {0}", TimeSpan.FromDays(PreferredDays)));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Bonus Multiplier : {0}", bonusMulti));
            messages.Add(string.Format(CultureInfo.CurrentCulture, " - Bonus PPD ------ : {0}", bonusPPD));
            
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, messages);
         }

         return bonusPPD;
      }

      /// <summary>
      /// Get Units Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      public double GetUPD(TimeSpan frameTime)
      {
         if (frameTime.Equals(TimeSpan.Zero))
         {
            return 0.0;
         }
         return 86400 / (frameTime.TotalSeconds * Frames);
      }
      
      /// <summary>
      /// Get the Credit of the Unit (including bonus)
      /// </summary>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      public double GetBonusCredit(TimeSpan estTimeOfUnit)
      {
         double bonusMulti = GetBonusMultiplier(estTimeOfUnit);
         return Math.Round((Credit * bonusMulti), 0);
      }

      /// <summary>
      /// Get the Bonus Multiplier
      /// </summary>
      /// <param name="estTimeOfUnit">Estimated Time of the Unit</param>
      private double GetBonusMultiplier(TimeSpan estTimeOfUnit)
      {
         // Make sure the given TimeSpan is not negative
         if (KFactor > 0 && estTimeOfUnit.CompareTo(TimeSpan.Zero) > 0)
         {
            if (estTimeOfUnit <= TimeSpan.FromDays(PreferredDays))
            {
               return Math.Sqrt((MaxDays * KFactor) / estTimeOfUnit.TotalDays);
            }
         }
         
         return 1;
      }
   }
}
