/*
 * HFM.NET - Protein Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class Protein
   {
      public Protein()
      {
         ServerIP = "0.0.0.0";
         WorkUnitName = "Unknown";
         Frames = 100;
         Core = "Unknown";
         Description = "Unassigned Description";
         Contact = "Unknown";
      }

      /// <summary>
      /// Project Number
      /// </summary>
      [DataMember(Order = 1)]
      public int ProjectNumber { get; set; }

      /// <summary>
      /// Server IP Address
      /// </summary>
      [DataMember(Order = 2)]
      public string ServerIP { get; set; }

      /// <summary>
      /// Work Unit Name
      /// </summary>
      [DataMember(Order = 3)]
      public string WorkUnitName { get; set; }

      /// <summary>
      /// Number of Atoms
      /// </summary>
      [DataMember(Order = 4)]
      public int NumberOfAtoms { get; set; }

      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      [DataMember(Order = 5)]
      public double PreferredDays { get; set; }

      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      [DataMember(Order = 6)]
      public double MaximumDays { get; set; }

      /// <summary>
      /// Work Unit Credit
      /// </summary>
      [DataMember(Order = 7)]
      public double Credit { get; set; }

      /// <summary>
      /// Number of Frames
      /// </summary>
      [DataMember(Order = 8)]
      public int Frames { get; set; }

      /// <summary>
      /// Core Identification string
      /// </summary>
      [DataMember(Order = 9)]
      public string Core { get; set; }

      /// <summary>
      /// Project Description (usually a URL)
      /// </summary>
      [DataMember(Order = 10)]
      public string Description { get; set; }

      /// <summary>
      /// Project Research Contact
      /// </summary>
      [DataMember(Order = 11)]
      public string Contact { get; set; }

      /// <summary>
      /// Bonus (K) Factor
      /// </summary>
      [DataMember(Order = 12)]
      public double KFactor { get; set; }

      public bool IsUnknown
      {
         get { return ProjectNumber == 0; }
      }

      public bool IsValid
      {
         get
         {
            return ProjectNumber > 0 &&
                   PreferredDays > 0 &&
                   MaximumDays > 0 &&
                   Credit > 0 &&
                   Frames > 0 &&
                   KFactor >= 0;
         }
      }
   }
}
