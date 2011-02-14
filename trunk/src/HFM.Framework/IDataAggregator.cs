/*
 * HFM.NET - Data Aggregator Interface
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IDataAggregator
   {
      /// <summary>
      /// Instance Name
      /// </summary>
      string InstanceName { get; set; }

      /// <summary>
      /// queue.dat File Path
      /// </summary>
      string QueueFilePath { get; set; }

      /// <summary>
      /// FAHlog.txt File Path
      /// </summary>
      string FahLogFilePath { get; set; }

      /// <summary>
      /// unitinfo.txt File Path
      /// </summary>
      string UnitInfoLogFilePath { get; set; }

      /// <summary>
      /// Client Queue
      /// </summary>
      ClientQueue Queue { get; }

      /// <summary>
      /// Current Index in List of returned UnitInfo and UnitLogLines
      /// </summary>
      int CurrentUnitIndex { get; }

      /// <summary>
      /// Client Run Data for the Current Run
      /// </summary>
      ClientRun CurrentClientRun { get; }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      IList<LogLine> CurrentLogLines { get; }

      /// <summary>
      /// Array of LogLine Lists
      /// </summary>
      IList<LogLine>[] UnitLogLines { get; }

      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      IList<IUnitInfo> AggregateData();
   }
}
