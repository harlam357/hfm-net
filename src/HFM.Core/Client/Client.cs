/*
 * HFM.NET
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using HFM.Core.Data;
using HFM.Core.DataTypes;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client
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

      #endregion

      #region Injection Properties

      public IPreferenceSet Prefs { get; set; }

      public IProteinService ProteinService { get; set; }

      public IProteinBenchmarkService BenchmarkService { get; set; }

      public IWorkUnitRepository WorkUnitRepository { get; set; }

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      #endregion

      public abstract ClientSettings Settings { get; set; }

      public abstract IEnumerable<SlotModel> Slots { get; }

      // should be init to DateTime.MinValue
      public DateTime LastRetrievalTime { get; protected set; }

      #region Constructor

      protected Client()
      {
         LastRetrievalTime = DateTime.MinValue;
      }

      #endregion

      protected bool AbortFlag { get; private set; }

      public virtual void Abort()
      {
         AbortFlag = true;
      }

      private readonly object _retrieveLock = new object();

      public void Retrieve()
      {
         if (!Monitor.TryEnter(_retrieveLock))
         {
            Debug.WriteLine(Logging.Logger.NameFormat, Settings.Name, "Retrieval already in progress...");
            return;
         }
         try
         {
            AbortFlag = false;

            // perform the client specific retrieval
            RetrieveInternal();
         }
         finally
         {
            AbortFlag = false;
            Monitor.Exit(_retrieveLock);
         }
      }

      protected abstract void RetrieveInternal();

      protected void InsertCompletedWorkUnit(WorkUnitModel workUnitModel)
      {
         // Update history database
         if (WorkUnitRepository != null && WorkUnitRepository.Connected)
         {
            try
            {
               if (WorkUnitRepository.Insert(workUnitModel))
               {
                  if (Logger.IsDebugEnabled)
                  {
                     string message = $"Inserted {workUnitModel.Data.ToProjectString()} into database.";
                     Logger.Debug(String.Format(Logging.Logger.NameFormat, workUnitModel.Data.OwningSlotName, message));
                  }
               }
            }
            catch (Exception ex)
            {
               Logger.Error(ex.Message, ex);
            }
         }
      }
   }
}
