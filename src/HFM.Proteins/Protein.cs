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
   /// <summary>
   /// Represents Folding@Home project (protein) information.
   /// </summary>
   public class Protein
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="Protein"/> class.
      /// </summary>
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
      /// Gets or sets the project number.
      /// </summary>
      public int ProjectNumber { get; set; }

      /// <summary>
      /// Gets or sets the server IP address.
      /// </summary>
      public string ServerIP { get; set; }

      /// <summary>
      /// Gets or sets the name of the work unit (project).
      /// </summary>
      public string WorkUnitName { get; set; }

      /// <summary>
      /// Gets or sets the number of atoms defined by the project.
      /// </summary>
      public int NumberOfAtoms { get; set; }

      /// <summary>
      /// Gets or sets the preferred deadline in days.
      /// </summary>
      public double PreferredDays { get; set; }

      /// <summary>
      /// Gets or sets the maximum deadline in days.
      /// </summary>
      public double MaximumDays { get; set; }

      /// <summary>
      /// Gets or sets the project credit value.
      /// </summary>
      public double Credit { get; set; }

      /// <summary>
      /// Gets or sets the number of project frames.
      /// </summary>
      public int Frames { get; set; }

      /// <summary>
      /// Gets or sets the core identification string.
      /// </summary>
      public string Core { get; set; }

      /// <summary>
      /// Gets or sets the project description (usually a URL).
      /// </summary>
      public string Description { get; set; }

      /// <summary>
      /// Gets or sets the project contact.
      /// </summary>
      public string Contact { get; set; }

      /// <summary>
      /// Gets or sets the bonus K factor.
      /// </summary>
      public double KFactor { get; set; }

      public bool IsUnknown
      {
         get { return ProjectNumber == 0; }
      }

      /// <summary>
      /// Returns true if this <see cref="Protein"/> has valid values for <see cref="ProjectNumber"/>, <see cref="PreferredDays"/>, <see cref="MaximumDays"/>, <see cref="Credit"/>, <see cref="Frames"/>, and <see cref="KFactor"/>; otherwise, false.
      /// </summary>
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

      /// <summary>
      /// Returns a new <see cref="Protein"/> object containing the data copied from this object.
      /// </summary>
      public Protein Copy()
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
