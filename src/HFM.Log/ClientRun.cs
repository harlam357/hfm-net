/*
 * HFM.NET - Client Run Class
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

using HFM.Core.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// Data Class for a single Client Run (client start to client stop).
   /// </summary>
   public class ClientRun
   {
      private readonly int _clientStartIndex;
      /// <summary>
      /// Log line index of the starting line for this Client Run.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _clientStartIndex; }
      }

      private readonly List<UnitIndex> _unitIndexes;
      /// <summary>
      /// List of Unit Indexes in this Client Run.
      /// </summary>
      public List<UnitIndex> UnitIndexes
      {
         get { return _unitIndexes; }
      }
   
      #region Constructor
      
      /// <summary>
      /// ClientRun Constructor
      /// </summary>
      /// <param name="clientStartIndex">Log line index of the starting line for this Client Run.</param>
      public ClientRun(int clientStartIndex)
      {
         _clientStartIndex = clientStartIndex;
         _unitIndexes = new List<UnitIndex>();

         ClientVersion = String.Empty;
         Arguments = String.Empty;
         FoldingID = String.Empty;
         Team = 0;
         UserID = String.Empty;
         Status = SlotStatus.Unknown;
      }
      
      #endregion
   
      #region Properties

      /// <summary>
      /// Client Version Number
      /// </summary>
      public string ClientVersion { get; set; }

      /// <summary>
      /// Client Command Line Arguments
      /// </summary>
      public string Arguments { get; set; }

      /// <summary>
      /// Folding ID (User name)
      /// </summary>
      public string FoldingID { get; set; }

      /// <summary>
      /// Team Number
      /// </summary>
      public int Team { get; set; }

      /// <summary>
      /// User ID (unique hexadecimal value)
      /// </summary>
      public string UserID { get; set; }

      /// <summary>
      /// Machine ID
      /// </summary>
      public int MachineID { get; set; }

      /// <summary>
      /// Number of Completed Units for this Client Run
      /// </summary>
      public int CompletedUnits { get; set; }

      /// <summary>
      /// Number of Failed Units for this Client Run
      /// </summary>
      public int FailedUnits { get; set; }

      /// <summary>
      /// Total Number of Completed Units (for the life of the client - as reported in the FAHlog.txt file)
      /// </summary>
      public int TotalCompletedUnits { get; set; }

      /// <summary>
      /// Client Status
      /// </summary>
      public SlotStatus Status { get; set; }

      #endregion
   }
   
   public struct UnitIndex
   {
      private readonly int _queueIndex;
      /// <summary>
      /// Queue index of this work unit.
      /// </summary>
      public int QueueIndex
      {
         get { return _queueIndex; }
      }
   
      private readonly int _startIndex;
      /// <summary>
      /// Log line index of the starting line of this work unit.
      /// </summary>
      public int StartIndex
      {
         get { return _startIndex; }
      }

      private readonly int _endIndex;
      /// <summary>
      /// Log line index of the ending line of this work unit.
      /// </summary>
      public int EndIndex
      {
         get { return _endIndex; }
      }
      
      /// <summary>
      /// UnitIndex Constructor
      /// </summary>
      /// <param name="queueIndex">Queue index of this work unit.</param>
      /// <param name="startIndex">Log line index of the starting line of this work unit.</param>
      public UnitIndex(int queueIndex, int startIndex)
         : this(queueIndex, startIndex, -1)
      {

      }

      /// <summary>
      /// UnitIndex Constructor
      /// </summary>
      /// <param name="queueIndex">Queue index of this work unit.</param>
      /// <param name="startIndex">Log line index of the starting line of this work unit.</param>
      /// <param name="endIndex">Log line index of the ending line of this work unit.</param>
      public UnitIndex(int queueIndex, int startIndex, int endIndex)
      {
         _queueIndex = queueIndex;
         _startIndex = startIndex;
         _endIndex = endIndex;
      }     
   }
}
