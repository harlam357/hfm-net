/*
 * HFM.NET - Instance Settings Presenter Tests
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
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Windows.Forms;

using HFM.Forms.Models;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms.Tests
{
   [TestFixture]
   public class InstanceSettingsPresenterTests
   {
      private MockRepository _mocks;
      private IInstanceSettingsView _settingsView;
      private INetworkOps _networkOps;
      private IMessageBoxView _messageBoxView;
      private IFolderBrowserView _folderBrowserView;
      private InstanceSettingsPresenter _presenter;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _settingsView = _mocks.DynamicMock<IInstanceSettingsView>();
         Expect.Call(_settingsView.FindValidatingControls()).Return(new List<IValidatingControl>());
         _networkOps = _mocks.DynamicMock<INetworkOps>();
         _messageBoxView = _mocks.DynamicMock<IMessageBoxView>();
         _folderBrowserView = _mocks.DynamicMock<IFolderBrowserView>();
      }

      private InstanceSettingsPresenter NewPresenter()
      {
         return new InstanceSettingsPresenter(_settingsView, _networkOps, _messageBoxView, _folderBrowserView);
      }
      
      [Test]
      public void ClientInstanceSettingsPathTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         Expect.Call(() => _settingsView.DataBind(settings));
         Expect.Call(() => settings.PropertyChanged += null).Constraints(Rhino.Mocks.Constraints.Is.NotNull());
         Expect.Call(() => _settingsView.PathGroupVisible = true);
         Expect.Call(() => _settingsView.HttpGroupVisible = false);
         Expect.Call(() => _settingsView.FtpGroupVisible = false);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _mocks.VerifyAll();
         
         // check on default Path instance - no need to replicate this assertion
         Assert.AreSame(settings, _presenter.SettingsModel);
      }

      [Test]
      public void ClientInstanceSettingsHttpTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         settings.InstanceHostType = InstanceType.HttpInstance;
         Expect.Call(() => _settingsView.DataBind(settings));
         Expect.Call(() => settings.PropertyChanged += null).Constraints(Rhino.Mocks.Constraints.Is.NotNull());
         Expect.Call(() => _settingsView.PathGroupVisible = false);
         Expect.Call(() => _settingsView.HttpGroupVisible = true);
         Expect.Call(() => _settingsView.FtpGroupVisible = false);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _mocks.VerifyAll();
      }

      [Test]
      public void ClientInstanceSettingsFtpTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         settings.InstanceHostType = InstanceType.FtpInstance;
         Expect.Call(() => _settingsView.DataBind(settings));
         Expect.Call(() => settings.PropertyChanged += null).Constraints(Rhino.Mocks.Constraints.Is.NotNull());
         Expect.Call(() => _settingsView.PathGroupVisible = false);
         Expect.Call(() => _settingsView.HttpGroupVisible = false);
         Expect.Call(() => _settingsView.FtpGroupVisible = true);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _mocks.VerifyAll();
      }
      
      [Test]
      public void ShowDialogTest()
      {
         Expect.Call(_settingsView.ShowDialog(null)).Return(DialogResult.OK);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.ShowDialog(null);
         _mocks.VerifyAll();
      }
      
      [Test]
      public void LocalBrowseClickedTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         settings.Path = "C:\\";
         Expect.Call(_folderBrowserView.SelectedPath = settings.Path);
         Expect.Call(_folderBrowserView.ShowDialog(_settingsView)).Return(DialogResult.OK);
         Expect.Call(_folderBrowserView.SelectedPath).Return("D:\\").Repeat.Twice();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.LocalBrowseClicked();
         _mocks.VerifyAll();
         
         Assert.AreEqual("D:\\", settings.Path);
      }
      
      [Test]
      public void TestConnectionClickedHttpTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         settings.InstanceHostType = InstanceType.HttpInstance;
         Expect.Call(_networkOps.BeginHttpCheckConnection(null, null, null, null)).IgnoreArguments().Return(null);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.TestConnectionClicked();
         _mocks.VerifyAll();
      }

      [Test]
      public void TestConnectionClickedFtpTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         settings.InstanceHostType = InstanceType.FtpInstance;
         Expect.Call(_networkOps.BeginFtpCheckConnection(null, null, null, null, FtpType.Passive, null)).IgnoreArguments().Return(null);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.TestConnectionClicked();
         _mocks.VerifyAll();
      }
      
      [Test]
      public void OkClickedErrorTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         SetupResult.For(settings.InstanceNameError).Return(true);
         Expect.Call(() => _messageBoxView.ShowError(_settingsView, String.Empty, String.Empty)).IgnoreArguments();
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.OkClicked();
         _mocks.VerifyAll();
      }

      [Test]
      public void OkClickedPathErrorNotOkTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         SetupResult.For(settings.PathError).Return(true);
         Expect.Call(_messageBoxView.AskYesNoQuestion(_settingsView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.OkClicked();
         _mocks.VerifyAll();
      }

      [Test]
      public void OkClickedPathErrorOkTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         SetupResult.For(settings.PathError).Return(true);
         Expect.Call(_messageBoxView.AskYesNoQuestion(_settingsView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         Expect.Call(_settingsView.DialogResult = DialogResult.OK);
         Expect.Call(() => _settingsView.Close());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.OkClicked();
         _mocks.VerifyAll();
      }

      [Test]
      public void OkClickedTest()
      {
         var settings = _mocks.Stub<IClientInstanceSettingsModel>();
         Expect.Call(_settingsView.DialogResult = DialogResult.OK);
         Expect.Call(() => _settingsView.Close());
         _mocks.ReplayAll();
         _presenter = NewPresenter();
         _presenter.SettingsModel = settings;
         _presenter.OkClicked();
         _mocks.VerifyAll();
      }
   }
}
