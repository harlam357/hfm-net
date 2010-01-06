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

using HFM.Framework;
using HFM.Preferences;

namespace HFM.Proteins
{
   [Serializable]
   public class Protein : IProtein
   {
      private int _ProjectNumber = 0;
      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber
      {
         get { return _ProjectNumber; }
         set
         {
            if (value < 1)
            {
               _ProjectNumber = 0;
               throw new ArgumentException("Project Number must be greater than 0.");
            }
            else
            {
               _ProjectNumber = value;
            }
         }
      }

      private String _ServerIP = "0.0.0.0";
      /// <summary>
      /// Server IP Address
      /// </summary>
      public String ServerIP
      {
         get { return _ServerIP; }
         set { _ServerIP = value; }
      }

      private String _WorkUnitName = "Unknown";
      /// <summary>
      /// Work Unit Name
      /// </summary>
      public String WorkUnitName
      {
         get { return _WorkUnitName; }
         set { _WorkUnitName = value; }
      }

      private int _NumAtoms = 0;
      /// <summary>
      /// Number of Atoms
      /// </summary>
      public int NumAtoms
      {
         get { return _NumAtoms; }
         set
         {
            if (value < 0)
            {
               _NumAtoms = 0;
               throw new ArgumentException("Number of Atoms must be greater than or equal to 0.");
            }
            else
            {
               _NumAtoms = value;
            }
         }
      }

      private double _PreferredDays = 0;
      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      public double PreferredDays
      {
         get { return _PreferredDays; }
         set
         {
            if (value < 0)
            {
               _PreferredDays = 0;
               throw new ArgumentException("Preferred Days must be greater than 0, or 0 for Timeless units.");
            }
            else
            {
               _PreferredDays = value;
            }
         }
      }

      private double _MaxDays = 0;
      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      public double MaxDays
      {
         get { return _MaxDays; }
         set
         {
            if (value < 0)
            {
               _MaxDays = 0;
               throw new ArgumentException("Maximum Days must be greater than 0, or 0 for Timeless units.");
            }
            else
            {
               _MaxDays = value;
            }
         }
      }

      private double _Credit = 0;
      /// <summary>
      /// Work Unit Credit
      /// </summary>
      public double Credit
      {
         get { return _Credit; }
         set
         {
            if (value < 1)
            {
               _Credit = 0;
               throw new ArgumentException("Credit must be greater than 0.");
            }
            else
            {
               _Credit = value;
            }
         }
      }

      private int _Frames = 100;
      /// <summary>
      /// Number of Frames
      /// </summary>
      public int Frames
      {
         get { return _Frames; }
         set
         {
            if (value < 1)
            {
               _Frames = 100;
               throw new ArgumentException("Number of frames must be greater than 0.");
            }
            else
            {
               _Frames = value;
            }
         }
      }

      private String _Core = "Unknown";
      /// <summary>
      /// Core Identification String
      /// </summary>
      public String Core
      {
         get { return _Core; }
         set { _Core = value; }
      }

      private String _Description = PreferenceSet.UnassignedDescription;
      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      public String Description
      {
         get { return _Description; }
         set { _Description = value; }
      }

      private String _Contact = "Unknown";
      /// <summary>
      /// Project Research Contact
      /// </summary>
      public String Contact
      {
         get { return _Contact; }
         set { _Contact = value; }
      }
      
      private double _KFactor = 0;
      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      public double KFactor
      {
         get { return _KFactor; }
         set { _KFactor = value; }
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
         return GetPPD(frameTime, TimeSpan.Zero);
      }
      
      /// <summary>
      /// Get Points Per Day based on given Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      /// <param name="EstTimeOfUnit">Estimated Time of the Unit</param>
      public double GetPPD(TimeSpan frameTime, TimeSpan EstTimeOfUnit)
      {
         if (frameTime.Equals(TimeSpan.Zero))
         {
            return 0.0;
         }
         
         double basePPD = GetUPD(frameTime) * Credit;
         double bonusMulti = GetBonusMultiplier(EstTimeOfUnit);
         return Math.Round((basePPD * bonusMulti), PreferenceSet.MaxDecimalPlaces);
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
      /// <param name="EstTimeOfUnit">Estimated Time of the Unit</param>
      public double GetBonusCredit(TimeSpan EstTimeOfUnit)
      {
         double bonusMulti = GetBonusMultiplier(EstTimeOfUnit);
         return Math.Round((Credit * bonusMulti), 0);
      }

      /// <summary>
      /// Get the Bonus Multiplier
      /// </summary>
      /// <param name="EstTimeOfUnit">Estimated Time of the Unit</param>
      private double GetBonusMultiplier(TimeSpan EstTimeOfUnit)
      {
         // Make sure the given TimeSpan is not negative
         if (KFactor > 0 && EstTimeOfUnit.CompareTo(TimeSpan.Zero) > 0)
         {
            if (EstTimeOfUnit <= TimeSpan.FromDays(PreferredDays))
            {
               return Math.Sqrt((MaxDays * KFactor) / EstTimeOfUnit.TotalDays);
            }
         }
         
         return 1;
      }
   }
}
