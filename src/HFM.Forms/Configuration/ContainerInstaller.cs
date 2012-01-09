/*
 * HFM.NET - Forms Container Installer
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

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using harlam357.Windows.Forms;

using HFM.Core;

namespace HFM.Forms.Configuration
{
   [CoverageExclude]
   public class ContainerInstaller : IWindsorInstaller
   {
      #region IWindsorInstaller Members

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         #region Data Models

         // UserStatsDataModel - Singleton
         container.Register(
            Component.For<Models.UserStatsDataModel>());

         // HistoryPresenterModel - Transient
         container.Register(
            Component.For<Models.HistoryPresenterModel>()
               .LifeStyle.Transient);

         #endregion

         #region View Interfaces

         // IMessageBoxView - Transient
         container.Register(
            Component.For<IMessageBoxView>()
               .ImplementedBy<MessageBoxView>()
               .LifeStyle.Transient);

         // IOpenFileDialogView - Transient
         container.Register(
            Component.For<IOpenFileDialogView>()
               .ImplementedBy<OpenFileDialogView>()
               .LifeStyle.Transient);

         // ISaveFileDialogView - Transient
         container.Register(
            Component.For<ISaveFileDialogView>()
               .ImplementedBy<SaveFileDialogView>()
               .LifeStyle.Transient);

         // IFolderBrowserView - Transient
         container.Register(
            Component.For<IFolderBrowserView>()
               .ImplementedBy<FolderBrowserView>()
               .LifeStyle.Transient);

         // IBenchmarksView - Transient
         container.Register(
            Component.For<IBenchmarksView>()
               .ImplementedBy<BenchmarksForm>()
               .LifeStyle.Transient);

         // IHistoryView - Transient
         container.Register(
            Component.For<IHistoryView>()
               .ImplementedBy<HistoryForm>()
               .LifeStyle.Transient);

         // HistoryPresenter - Transient
         container.Register(
            Component.For<HistoryPresenter>()
               .LifeStyle.Transient);

         // IFahClientSetupView - Transient
         container.Register(
            Component.For<IFahClientSetupView>()
               .ImplementedBy<FahClientSetupDialog>()
               .LifeStyle.Transient);

         // IFahClientSetupPresenter - Transient
         container.Register(
            Component.For<IFahClientSetupPresenter>()
               .ImplementedBy<FahClientSetupPresenter>()
               .LifeStyle.Transient);

         // ILegacyClientSetupView - Transient
         container.Register(
            Component.For<ILegacyClientSetupView>()
               .ImplementedBy<LegacyClientSetupDialog>()
               .LifeStyle.Transient);

         // ILegacyClientSetupPresenter - Transient
         container.Register(
            Component.For<ILegacyClientSetupPresenter>()
               .ImplementedBy<LegacyClientSetupPresenter>()
               .LifeStyle.Transient);

         // IMainView - Singleton
         container.Register(
            Component.For<IMainView>()
               .ImplementedBy<MainForm>());

         // IMainView - Singleton
         container.Register(
            Component.For<IMessagesView>()
               .ImplementedBy<MessagesForm>());

         // IQueryView - Transient
         container.Register(
            Component.For<IQueryView>()
               .ImplementedBy<QueryDialog>()
               .LifeStyle.Transient);

         // IProgressDialogView - Transient
         container.Register(
            Component.For<IProgressDialogView>()
               .ImplementedBy<ProjectDownloadDialog>()
               .LifeStyle.Transient);

         #endregion

         #region Presenters

         // MainPresenter - Singleton
         container.Register(
            Component.For<MainPresenter>());

         #endregion

         #region Service Interfaces

         // IAutoRun - Singleton
         container.Register(
            Component.For<IAutoRun>()
               .ImplementedBy<AutoRun>());

         // IUpdateLogic - Singleton
         container.Register(
            Component.For<IUpdateLogic>()
               .ImplementedBy<UpdateLogic>());

         #endregion

         // PreferencesDialog - Transient
         container.Register(
            Component.For<PreferencesDialog>()
               .LifeStyle.Transient);

         // RetrievalLogic - Singleton
         container.Register(
            Component.For<RetrievalLogic>());
      }

      #endregion
   }
}
