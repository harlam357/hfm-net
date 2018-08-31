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

namespace HFM.Proteins
{
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
      public int NumberOfAtoms { get; set; }

      /// <summary>
      /// Deadline - Preferred Days
      /// </summary>
      public double PreferredDays { get; set; }

      /// <summary>
      /// Deadline - Maximum Days
      /// </summary>
      public double MaximumDays { get; set; }

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

      public Protein DeepClone()
      {
         return new Protein
         {
            ProjectNumber = ProjectNumber,
            ServerIP = ServerIP,
            WorkUnitName = WorkUnitName,
            NumberOfAtoms = NumberOfAtoms,
            PreferredDays = PreferredDays,
            MaximumDays = MaximumDays,
            Credit = Credit,
            Frames = Frames,
            Core = Core,
            Description = Description,
            Contact = Contact,
            KFactor = KFactor
         };
      }
   }
}
