/*
 * HFM.NET - Protein Load Info Metadata Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.Text;

namespace HFM.Proteins
{
   public enum ProteinLoadResult
   {
      Added,
      Changed,
      NoChange,
   }

   public sealed class ProteinLoadInfo
   {
      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber { get; private set; }

      public ProteinLoadResult Result { get; private set; }

      public IEnumerable<ProteinPropertyChange> Changes { get; private set; }

      public ProteinLoadInfo(int projectNumber, ProteinLoadResult result)
         : this(projectNumber, result, null)
      {

      }

      public ProteinLoadInfo(int projectNumber, ProteinLoadResult result, IEnumerable<ProteinPropertyChange> changes)
      {
         ProjectNumber = projectNumber;
         Result = result;
         Changes = changes;
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.Append("Project: ");
         sb.Append(ProjectNumber);
         sb.Append(" - ");
         sb.Append(Result);
         if (Changes != null)
         {
            foreach (var change in Changes)
            {
               sb.Append(" / ");
               sb.Append(change);
            }            
         }
         return sb.ToString();
      }
   }

   public sealed class ProteinPropertyChange
   {
      public string Name { get; private set; }

      public string OldValue { get; private set; }

      public string NewValue { get; private set; }

      public ProteinPropertyChange(string name, string oldValue, string newValue)
      {
         Name = name;
         OldValue = oldValue;
         NewValue = newValue;
      }

      public override string ToString()
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} - {1} > {2}", Name, OldValue, NewValue);
      }
   }
}
