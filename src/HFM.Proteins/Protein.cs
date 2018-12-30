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
   }
}
