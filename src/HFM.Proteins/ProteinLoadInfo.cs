/*
 * HFM.NET - Protein Load Info Metadata Class
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
      private readonly int _projectNumber;
      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber
      {
         get { return _projectNumber; }
      }

      private readonly ProteinLoadResult _result;

      public ProteinLoadResult Result
      {
         get { return _result; }
      }

      private readonly IEnumerable<ProteinPropertyChange> _changes;

      public IEnumerable<ProteinPropertyChange> Changes
      {
         get { return _changes; }
      }

      internal ProteinLoadInfo(int projectNumber, ProteinLoadResult result)
         : this(projectNumber, result, null)
      {

      }

      internal ProteinLoadInfo(int projectNumber, ProteinLoadResult result, IEnumerable<ProteinPropertyChange> changes)
      {
         _projectNumber = projectNumber;
         _result = result;
         _changes = changes;
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.Append("Project: ");
         sb.Append(_projectNumber);
         sb.Append(" - ");
         sb.Append(_result);
         if (_changes != null)
         {
            foreach (var change in _changes)
            {
               sb.Append(" / ");
               sb.Append(change.ToString());
            }            
         }
         return sb.ToString();
      }
   }

   public sealed class ProteinPropertyChange
   {
      private readonly string _name;
      public string Name
      {
         get { return _name; }
      }

      private readonly string _oldValue;
      public string OldValue
      {
         get { return _oldValue; }
      }

      private readonly string _newValue;
      public string NewValue
      {
         get { return _newValue; }
      }

      internal ProteinPropertyChange(string name, string oldValue, string newValue)
      {
         _name = name;
         _oldValue = oldValue;
         _newValue = newValue;
      }

      public override string ToString()
      {
         return String.Format(CultureInfo.CurrentCulture, "{0} - {1} > {2}", _name, _oldValue, _newValue);
      }
   }
}
