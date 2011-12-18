/*
 * HFM.NET - Client Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IClient
   {
      #region Events

      /// <summary>
      /// Fired when the client slot layout has changed.
      /// </summary>
      event EventHandler SlotsChanged;
      
      /// <summary>
      /// Fired when the Retrieve method finishes.
      /// </summary>
      event EventHandler RetrievalFinished;

      /// <summary>
      /// Clears all event subscriptions from this client.
      /// </summary>
      void ClearEventSubscriptions();

      #endregion

      /// <summary>
      /// Settings that define this client's behavior.
      /// </summary>
      ClientSettings Settings { get; set; }

      /// <summary>
      /// Enumeration of client slots.
      /// </summary>
      IEnumerable<SlotModel> Slots { get; }

      /// <summary>
      /// Last successful retrieval time.
      /// </summary>
      DateTime LastRetrievalTime { get; }

      /// <summary>
      /// Flag set to true when retrieval is in progress.
      /// </summary>
      bool RetrievalInProgress { get; }

      /// <summary>
      /// Abort retrieval processes.
      /// </summary>
      void Abort();

      /// <summary>
      /// Start retrieval processes.
      /// </summary>
      void Retrieve();
   }

   public abstract class Client : IClient
   {
      #region Events

      public event EventHandler SlotsChanged;

      protected virtual void OnSlotsChanged(EventArgs e)
      {
         if (SlotsChanged != null)
         {
            SlotsChanged(this, e);
         }
      }

      public event EventHandler RetrievalFinished;

      protected virtual void OnRetrievalFinished(EventArgs e)
      {
         if (RetrievalFinished != null)
         {
            RetrievalFinished(this, e);
         }
      }

      public void ClearEventSubscriptions()
      {
         SlotsChanged = null;
         RetrievalFinished = null;
      }

      #endregion

      #region Injection Properties

      public IPreferenceSet Prefs { get; set; }

      public IProteinDictionary ProteinDictionary { get; set; }

      public IUnitInfoCollection UnitInfoCollection { get; set; }

      public IProteinBenchmarkCollection BenchmarkCollection { get; set; }

      public IUnitInfoDatabase UnitInfoDatabase { get; set; }
      
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      #endregion

      public abstract ClientSettings Settings { get; set; }

      public abstract IEnumerable<SlotModel> Slots { get; }

      // should be init to DateTime.MinValue
      public DateTime LastRetrievalTime { get; protected set; } 
      
      private volatile bool _retrievalInProgress;

      public bool RetrievalInProgress
      {
         get { return _retrievalInProgress; }
         protected set { _retrievalInProgress = value; }
      }

      #region Constructor

      protected Client()
      {
         LastRetrievalTime = DateTime.MinValue;
      }

      #endregion

      protected bool AbortFlag { get; set; }

      public virtual void Abort()
      {
         AbortFlag = true;
      }

      public void Retrieve()
      {
         try
         {
            // Don't allow this to fire more than once at a time
            if (RetrievalInProgress) return;

            RetrievalInProgress = true;
            AbortFlag = false;

            // perform the client specific retrieval
            RetrieveInternal();
         }
         finally
         {
            RetrievalInProgress = false;
            AbortFlag = false;
         }
      }

      protected abstract void RetrieveInternal();
   }
}
