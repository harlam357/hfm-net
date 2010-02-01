/*
 * HFM.NET - Client Run Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using HFM.Framework;

namespace HFM.Log
{
   /// <summary>
   /// Holds Run Level Data for a Single Client Run.
   /// </summary>
   public class ClientRun : IClientRun
   {
      #region Members
      private string _Arguments = String.Empty;
      public string Arguments
      {
         get { return _Arguments; }
         set { _Arguments = value; }
      }

      private string _FoldingID = String.Empty;
      public string FoldingID
      {
         get { return _FoldingID; }
         set { _FoldingID = value; }
      }

      private int _Team;
      public int Team
      {
         get { return _Team; }
         set { _Team = value; }
      }

      private string _UserID = String.Empty;
      public string UserID
      {
         get { return _UserID; }
         set { _UserID = value; }
      }

      private int _MachineID;
      public int MachineID
      {
         get { return _MachineID; }
         set { _MachineID = value; }
      }

      private int _NumberOfCompletedUnits;
      public int NumberOfCompletedUnits
      {
         get { return _NumberOfCompletedUnits; }
         set { _NumberOfCompletedUnits = value; }
      }

      private int _NumberOfFailedUnits;
      public int NumberOfFailedUnits
      {
         get { return _NumberOfFailedUnits; }
         set { _NumberOfFailedUnits = value; }
      }

      private int _NumberOfTotalUnitsCompleted;
      public int NumberOfTotalUnitsCompleted
      {
         get { return _NumberOfTotalUnitsCompleted; }
         set { _NumberOfTotalUnitsCompleted = value; }
      }

      /// <summary>
      /// Line index of client start position.
      /// </summary>
      private readonly int _ClientStartPosition;
      /// <summary>
      /// Line index of client start position.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _ClientStartPosition; }
      }

      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      private readonly List<int> _UnitStartIndex = new List<int>();
      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      public List<int> UnitStartIndex
      {
         get { return _UnitStartIndex; }
      }

      /// <summary>
      /// List of Queue Indexes that correspond to the Unit Start Indexes for this client run.
      /// </summary>
      private readonly List<int> _UnitQueueIndex = new List<int>();
      /// <summary>
      /// List of Queue Indexes that correspond to the Unit Start Indexes for this client run.
      /// </summary>
      public List<int> UnitQueueIndex
      {
         get { return _UnitQueueIndex; }
      }
      #endregion

      #region CTOR
      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="ClientStartIndex">Line index of client start.</param>
      public ClientRun(int ClientStartIndex)
      {
         _ClientStartPosition = ClientStartIndex;
      }
      #endregion
   }
}
