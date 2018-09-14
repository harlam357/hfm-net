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
   public enum ProteinDictionaryChangeResult
   {
      Added,
      Changed,
      NoChange
   }

   public sealed class ProteinDictionaryChange
   {
      public int ProjectNumber { get; private set; }

      public ProteinDictionaryChangeResult Result { get; private set; }

      public IReadOnlyCollection<ProteinPropertyChange> Changes { get; private set; }

      internal ProteinDictionaryChange(int projectNumber, ProteinDictionaryChangeResult result)
         : this(projectNumber, result, null)
      {

      }

      internal ProteinDictionaryChange(int projectNumber, ProteinDictionaryChangeResult result, IReadOnlyCollection<ProteinPropertyChange> changes)
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
      public string PropertyName { get; private set; }

      public string Previous { get; private set; }

      public string Current { get; private set; }

      internal ProteinPropertyChange(string propertyName, string previous, string current)
      {
         PropertyName = propertyName;
         Previous = previous;
         Current = current;
      }

      public override string ToString()
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} - {1} > {2}", PropertyName, Previous, Current);
      }
   }
}
