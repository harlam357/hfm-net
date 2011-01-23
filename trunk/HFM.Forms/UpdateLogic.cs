/*
 * HFM.NET - Update Logic Class
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
using System.Globalization;
using System.Net;

using harlam357.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms
{
   public interface IUpdateLogic
   {
      System.Windows.Forms.Form Owner { get; set; }
   
      bool CheckInProgress { get; }
      
      string UpdateFilePath { get; }

      void CheckForUpdate();
      
      void CheckForUpdate(bool userInvoked);
   }

   /* Cannot write an effective unit test for this class
    * until I remove the concrete dependencies on NetworkOps
    * and UpdateChecker.  I'd also like to enable a synchronous
    * or asynchronous option since it's more difficult to unit
    * test an asynchronous operation *OR* do like the following:
    * http://stackoverflow.com/questions/1174702/is-there-a-way-to-unit-test-an-async-method
    */

   public sealed class UpdateLogic : IUpdateLogic
   {
      #region Properties
   
      public System.Windows.Forms.Form Owner { get; set; }

      public bool CheckInProgress { get; private set; }
      
      public string UpdateFilePath { get; private set; }
      
      #endregion
      
      #region Fields

      private bool _userInvoked;
      private IWebProxy _proxy;
      
      private readonly IPreferenceSet _prefs;
      private readonly IMessageBoxView _messageBoxView;
      
      #endregion

      public UpdateLogic(IPreferenceSet prefs, IMessageBoxView messageBoxView)
      {
         _prefs = prefs;
         _messageBoxView = messageBoxView;
      }

      public void CheckForUpdate()
      {
         CheckForUpdate(false);
      }

      public void CheckForUpdate(bool userInvoked)
      {
         if (Owner == null)
         {
            throw new InvalidOperationException("Owner property cannot be null.");
         }
      
         if (CheckInProgress)
         {
            throw new InvalidOperationException("Update check already in progress.");
         }
      
         CheckInProgress = true;
         
         // set globals
         _userInvoked = userInvoked;
         _proxy = new NetworkOps(_prefs).GetProxy();
         
         Func<ApplicationUpdate> func = DoCheckForUpdate;
         func.BeginInvoke(CheckForUpdateCallback, func);
      }
   
      private ApplicationUpdate DoCheckForUpdate()
      {
         var updateChecker = new UpdateChecker();
         return updateChecker.CheckForUpdate(Constants.ApplicationName, _proxy);
      }

      private void CheckForUpdateCallback(IAsyncResult result)
      {
         try
         {
            var func = (Func<ApplicationUpdate>)result.AsyncState;
            ApplicationUpdate update = func.EndInvoke(result);
            if (update != null) 
            {
               if (NewVersionAvailable(update.Version))
               {
                  ShowUpdate(update);
               }
               else if (_userInvoked)
               {
                  Owner.Invoke(new Action(() => _messageBoxView.ShowInformation(Owner, String.Format(CultureInfo.CurrentCulture,
                                                   "{0} is already up-to-date.", Constants.ApplicationName), Owner.Text)));
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            if (_userInvoked)
            {
               string message = String.Format(CultureInfo.CurrentCulture, "{0} encountered the following error while checking for an update:{1}{1}{2}.",
                                              Constants.ApplicationName, Environment.NewLine, ex.Message);
               Owner.Invoke(new Action(() => _messageBoxView.ShowError(Owner, message, Owner.Text)));
            }
         }
         finally
         {
            CheckInProgress = false;
         }
      }

      private static bool NewVersionAvailable(string updateVersion)
      {
         return PlatformOps.GetVersionLongFromString(updateVersion) > PlatformOps.VersionNumber;
      }

      private void ShowUpdate(ApplicationUpdate update)
      {
         if (Owner.InvokeRequired)
         {
            Owner.Invoke(new Action(() => ShowUpdate(update)));
            return;
         }

         var updatePresenter = new UpdatePresenter(HfmTrace.WriteToHfmConsole,
            update, _proxy, Constants.ApplicationName, PlatformOps.ApplicationVersionWithRevision);
         updatePresenter.Show(Owner);
         HandleUpdatePresenterResults(updatePresenter);
      }

      private void HandleUpdatePresenterResults(UpdatePresenter presenter)
      {
         if (presenter.UpdateReady &&
             presenter.SelectedUpdate.UpdateType == 0 &&
             PlatformOps.IsRunningOnMono() == false)
         {
            string message = String.Format(CultureInfo.CurrentCulture,
                                           "{0} will install the new version when you exit the application.",
                                           Constants.ApplicationName);
            _messageBoxView.ShowInformation(Owner, message, Owner.Text);
            UpdateFilePath = presenter.LocalFilePath;
         }
      }
   }
}
