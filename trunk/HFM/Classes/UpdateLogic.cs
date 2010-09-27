/*
 * HFM.NET - Update Logic Class
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
using System.Globalization;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;

namespace HFM.Classes
{
   public class UpdateLogic
   {
      private Action<ApplicationUpdate> _showUpdateCallback;
      private bool _userInvoked;
      
      public bool CheckInProgress { get; private set; }

      private readonly Form _owner;
      private readonly IMessageBoxView _messageBoxView;

      public UpdateLogic(Form owner, IMessageBoxView messageBoxView)
      {
         _owner = owner;
         _messageBoxView = messageBoxView;
      }
   
      public void BeginCheckForUpdate(Action<ApplicationUpdate> showUpdateCallback, bool userInvoked)
      {
         CheckInProgress = true;
         _showUpdateCallback = showUpdateCallback;
         _userInvoked = userInvoked;
         Func<ApplicationUpdate> func = CheckForUpdate;
         func.BeginInvoke(CheckForUpdateCallback, func);
      }
   
      private static ApplicationUpdate CheckForUpdate()
      {
         var updateChecker = new UpdateChecker();
         return updateChecker.CheckForUpdate(Constants.ApplicationName, NetworkOps.GetProxy());
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
                  _showUpdateCallback(update);
               }
               else if (_userInvoked)
               {
                  _owner.Invoke(new MethodInvoker(() => _messageBoxView.ShowInformation(_owner, String.Format(CultureInfo.CurrentCulture,
                                                           "{0} is already up-to-date.", Constants.ApplicationName), _owner.Text)));
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
               _owner.Invoke(new MethodInvoker(() => _messageBoxView.ShowError(_owner, message, _owner.Text)));
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
   }
}
