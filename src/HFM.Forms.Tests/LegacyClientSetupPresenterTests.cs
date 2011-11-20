/*
 * HFM.NET - Instance Settings Presenter Tests
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
using System.Windows.Forms;

using NUnit.Framework;
using Rhino.Mocks;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Tests
{
   [TestFixture]
   public class LegacyClientSetupPresenterTests
   {
      private ILegacyClientSetupView _settingsView;
      private INetworkOps _networkOps;
      private IMessageBoxView _messageBoxView;
      private IFolderBrowserView _folderBrowserView;
      private LegacyClientSetupPresenter _presenter;

      [SetUp]
      public void Init()
      {
         _settingsView = MockRepository.GenerateMock<ILegacyClientSetupView>();
         _settingsView.Stub(x => x.FindValidatingControls()).Return(new List<IValidatingControl>());
         _networkOps = MockRepository.GenerateMock<INetworkOps>();
         _messageBoxView = MockRepository.GenerateMock<IMessageBoxView>();
         _folderBrowserView = MockRepository.GenerateMock<IFolderBrowserView>();
      }

      private LegacyClientSetupPresenter CreatePresenter()
      {
         return new LegacyClientSetupPresenter(_settingsView, _networkOps, _messageBoxView, _folderBrowserView);
      }
      
      [Test]
      public void ClientSettingsPathTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.LegacyClientSubType = LegacyClientSubType.Path;

         _settingsView.Expect(x => x.DataBind(settings));
         _settingsView.Expect(x => x.PathGroupVisible = true);
         _settingsView.Expect(x => x.HttpGroupVisible = false);
         _settingsView.Expect(x => x.FtpGroupVisible = false);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         // Assert
         // check on default Path instance - no need to replicate this assertion
         Assert.AreSame(settings, _presenter.SettingsModel);

         _settingsView.VerifyAllExpectations();
      }

      [Test]
      public void ClientSettingsHttpTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.LegacyClientSubType = LegacyClientSubType.Http;

         _settingsView.Expect(x => x.DataBind(settings));
         _settingsView.Expect(x => x.PathGroupVisible = false);
         _settingsView.Expect(x => x.HttpGroupVisible = true);
         _settingsView.Expect(x => x.FtpGroupVisible = false);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         // Assert
         _settingsView.VerifyAllExpectations();
      }

      [Test]
      public void ClientSettingsFtpTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.LegacyClientSubType = LegacyClientSubType.Ftp;

         _settingsView.Expect(x => x.DataBind(settings));
         _settingsView.Expect(x => x.PathGroupVisible = false);
         _settingsView.Expect(x => x.HttpGroupVisible = false);
         _settingsView.Expect(x => x.FtpGroupVisible = true);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         // Assert
         _settingsView.VerifyAllExpectations();
      }
      
      [Test]
      public void ShowDialogTest()
      {
         // Arrange
         _settingsView.Expect(x => x.ShowDialog(null)).Return(DialogResult.OK);
         // Act
         _presenter = CreatePresenter();
         _presenter.ShowDialog(null);
         // Assert
         _settingsView.VerifyAllExpectations();
      }
      
      [Test]
      public void LocalBrowseClickedTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.Path = "C:\\";
         // Act
         _folderBrowserView.Expect(x => x.SelectedPath = settings.Path);
         _folderBrowserView.Expect(x => x.ShowDialog(_settingsView)).Return(DialogResult.OK);
         _folderBrowserView.Expect(x => x.SelectedPath).Return("D:\\").Repeat.Twice();
         // Assert
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         _presenter.LocalBrowseClicked();
         
         Assert.AreEqual("D:\\", settings.Path);

         _folderBrowserView.VerifyAllExpectations();
      }
      
      [Test]
      public void TestConnectionClickedHttpTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.LegacyClientSubType = LegacyClientSubType.Http;
         _networkOps.Expect(x => x.BeginHttpCheckConnection(null, null, null, null)).IgnoreArguments().Return(null);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         _presenter.TestConnectionClicked();
         // Assert
         _networkOps.VerifyAllExpectations();
      }

      [Test]
      public void TestConnectionClickedFtpTest()
      {
         // Arrange
         var settings = new LegacyClientSettingsModel();
         settings.LegacyClientSubType = LegacyClientSubType.Ftp;
         _networkOps.Expect(x => x.BeginFtpCheckConnection(null, null, null, null, FtpType.Passive, null)).IgnoreArguments().Return(null);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = settings;
         _presenter.TestConnectionClicked();
         // Assert
         _networkOps.VerifyAllExpectations();
      }
      
      [Test]
      public void OkClickedErrorTest()
      {
         // Arrange
         _messageBoxView.Expect(x => x.ShowError(_settingsView, String.Empty, String.Empty)).IgnoreArguments();
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = new LegacyClientSettingsModel();
         _presenter.OkClicked();
         // Assert
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void OkClickedPathErrorNotOkTest()
      {
         // Arrange
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_settingsView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.No);
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = CreateValidModel();
         _presenter.SettingsModel.Path = "[";
         _presenter.OkClicked();
         // Assert
         _messageBoxView.VerifyAllExpectations();
      }

      [Test]
      public void OkClickedPathErrorOkTest()
      {
         // Arrange
         _messageBoxView.Expect(x => x.AskYesNoQuestion(_settingsView, String.Empty, String.Empty)).IgnoreArguments().Return(DialogResult.Yes);
         _settingsView.Expect(x => x.DialogResult = DialogResult.OK);
         _settingsView.Expect(x => x.Close());
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = CreateValidModel();
         _presenter.SettingsModel.Path = "[";
         _presenter.OkClicked();
         // Assert
         _messageBoxView.VerifyAllExpectations();
         _settingsView.VerifyAllExpectations();
      }

      [Test]
      public void OkClickedTest()
      {
         // Arrange
         _settingsView.Expect(x => x.DialogResult = DialogResult.OK);
         _settingsView.Expect(x => x.Close());
         // Act
         _presenter = CreatePresenter();
         _presenter.SettingsModel = CreateValidModel();
         _presenter.OkClicked();
         // Assert
         _settingsView.VerifyAllExpectations();
      }

      private static LegacyClientSettingsModel CreateValidModel()
      {
         var model = new LegacyClientSettingsModel();
         model.LegacyClientSubType = LegacyClientSubType.Path;
         model.Name = "Test";
         model.ClientProcessorMegahertz = 1;
         model.FahLogFileName = "FAHlog.txt";
         model.UnitInfoFileName = "unitinfo.txt";
         model.QueueFileName = "queue.dat";
         model.Path = "C:\\";
         return model;
      }
   }
}
