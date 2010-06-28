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

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Proteins
{
   public class Protein : IProtein
   {
      public Protein()
      {
      
      }

      public Protein(int projectNumber)
      {
         _projectNumber = projectNumber;
      }

      private int _projectNumber;
      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber
      {
         get { return _projectNumber; }
         set { _projectNumber = value < 1 ? 0 : value; }
      }

      private String _serverIp = "0.0.0.0";
      /// <summary>
      /// Server IP Address
      /// </summary>
      public String ServerIP
      {
         get { return _serverIp; }
         set { _serverIp = value; }
      }

      private String _workUnitName = "Unknown";
      /// <summary>
      /// Work Unit Name
      /// </summary>
      public String WorkUnitName
      {
         get { return _workUnitName; }
         set { _workUnitName = value; }
      }

      private int _numAtoms;
      /// <summary>
      /// Number of Atoms
      /// </summary>
      public int NumAtoms
      {
         get { return _numAtoms; }
         set { _numAtoms = value < 0 ? 0 : value; }
      }

      private double _preferredDays;
      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      public double PreferredDays
      {
         get { return _preferredDays; }
         set { _preferredDays = value < 0 ? 0 : value; }
      }

      private double _maxDays;
      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      public double MaxDays
      {
         get { return _maxDays; }
         set { _maxDays = value < 0 ? 0 : value; }
      }

      private double _credit;
      /// <summary>
      /// Work Unit Credit
      /// </summary>
      public double Credit
      {
         get { return _credit; }
         set { _credit = value < 1 ? 0 : value; }
      }

      private int _frames = 100;
      /// <summary>
      /// Number of Frames
      /// </summary>
      public int Frames
      {
         get { return _frames; }
         set { _frames = value < 1 ? 100 : value; }
      }

      private String _core = "Unknown";
      /// <summary>
      /// Core Identification String
      /// </summary>
      public String Core
      {
         get { return _core; }
         set { _core = value; }
      }

      private String _description = Constants.UnassignedDescription;
      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      public String Description
      {
         get { return _description; }
         set { _description = value; }
      }

      private String _contact = "Unknown";
      /// <summary>
      /// Project Research Contact
      /// </summary>
      public String Contact
      {
         get { return _contact; }
         set { _contact = value; }
      }
      
      private double _kFactor;
      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      public double KFactor
      {
         get { return _kFactor; }
         set { _kFactor = value; }
      }
      
      /// <summary>
      /// Flag Denoting if Project Number is Unknown
      /// </summary>
      public bool IsUnknown
      {
         get { return ProjectNumber == 0; }
      }

      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      public double GetPPD(TimeSpan frameTime)
      {
         return GetPPD(frameTime, String.Empty);
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
         return GetPPD(frameTime, estTimeOfUnit, String.Empty);
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

         double basePPD = GetUPD(frameTime)*Credit;
         double bonusMulti = GetBonusMultiplier(estTimeOfUnit);
         double bonusPPD = Math.Round((basePPD * bonusMulti), Constants.MaxDecimalPlaces);
         
         var messages = new List<string>(9);

         if (String.IsNullOrEmpty(instanceName) == false)
         {
            messages.Add(String.Format(CultureInfo.CurrentCulture, "{0} ({1})", HfmTrace.FunctionName, instanceName));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Frame Time ----- : {0}", frameTime));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Credit --------- : {0}", Credit));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Base PPD ------- : {0}", basePPD));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - KFactor -------- : {0}", KFactor));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Estimated Time - : {0}", estTimeOfUnit));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Preferred Time - : {0}", TimeSpan.FromDays(PreferredDays)));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Bonus Multiplier : {0}", bonusMulti));
            messages.Add(String.Format(CultureInfo.CurrentCulture, " - Bonus PPD ------ : {0}", bonusPPD));
            
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
