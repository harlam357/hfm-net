/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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

using System.Diagnostics.CodeAnalysis;

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using harlam357.Windows.Forms;

namespace HFM.Forms.Configuration
{
   [ExcludeFromCodeCoverage]
   public class ContainerInstaller : IWindsorInstaller
   {
      #region IWindsorInstaller Members

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         #region MVP

         // MainPresenter - Singleton
         container.Register(
            Component.For<MainPresenter>(),
            Component.For<IMainView, System.ComponentModel.ISynchronizeInvoke>()
               .ImplementedBy<MainForm>(),
            Component.For<Models.MainGridModel>(),
            Component.For<Models.UserStatsDataModel>());

         // IFahClientSetupPresenter - Transient
         container.Register(
            Component.For<IFahClientSetupPresenter>()
               .ImplementedBy<FahClientSetupPresenter>()
                  .Named("FahClientSetupPresenter")
                     .LifeStyle.Transient);

         // HistoryPresenterModel - Transient
         container.Register(
            Component.For<HistoryPresenter>()
               .Named("HistoryPresenter")
                  .LifeStyle.Transient,
            Component.For<IHistoryView>()
               .ImplementedBy<HistoryForm>()
                  .LifeStyle.Transient,
            Component.For<Models.HistoryPresenterModel>()
               .LifeStyle.Transient);

         container.Register(
            Component.For<IPresenterFactory>()
               .AsFactory());

         // ProteinCalculatorModel - Transient
         container.Register(
            Component.For<Models.ProteinCalculatorModel>()
               .LifeStyle.Transient);

         #endregion

         #region View Interfaces

         // IFahClientSetupView - Transient
         container.Register(
            Component.For<IFahClientSetupView>()
               .ImplementedBy<FahClientSetupDialog>()
               .LifeStyle.Transient);

         // Singleton Views
         container.Register(
            Component.For<IMessagesView>()
               .ImplementedBy<MessagesForm>(),
            Component.For<IMessageBoxView>()
               .ImplementedBy<MessageBoxView>());

         // Transient Views
         container.Register(
            Component.For<IOpenFileDialogView>()
               .ImplementedBy<OpenFileDialogView>()
                  .Named("OpenFileDialogView")
                     .LifeStyle.Transient,
            Component.For<ISaveFileDialogView>()
               .ImplementedBy<SaveFileDialogView>()
                  .Named("SaveFileDialogView")
                     .LifeStyle.Transient,
            Component.For<IFolderBrowserView>()
               .ImplementedBy<FolderBrowserView>()
                  .Named("FolderBrowserView")
                     .LifeStyle.Transient,
            Component.For<IProgressDialogAsyncView>()
               .ImplementedBy<ProgressDialogAsync>()
                  .Named("ProgressDialogAsync")
                     .LifeStyle.Transient,
            Component.For<IQueryView>()
               .ImplementedBy<QueryDialog>()
                  .Named("QueryDialog")
                     .LifeStyle.Transient,
            Component.For<IBenchmarksView>()
               .ImplementedBy<BenchmarksForm>()
                  .Named("BenchmarksForm")
                     .LifeStyle.Transient,
            Component.For<IPreferencesView>()
               .ImplementedBy<PreferencesDialog>()
                  .Named("PreferencesDialog")
                     .LifeStyle.Transient,
            Component.For<IProteinCalculatorView>()
               .ImplementedBy<ProteinCalculatorForm>()
                  .Named("ProteinCalculatorForm")
                     .LifeStyle.Transient,
            Component.For<IViewFactory>()
               .AsFactory());

         #endregion

         #region Service Interfaces

         // IAutoRun - Singleton
         container.Register(
            Component.For<IAutoRun>()
               .ImplementedBy<AutoRun>());

         // IExternalProcessStarter - Singleton
         container.Register(
            Component.For<IExternalProcessStarter>()
               .ImplementedBy<ExternalProcessStarter>());

         // IUpdateLogic - Singleton
         container.Register(
            Component.For<IUpdateLogic>()
               .ImplementedBy<UpdateLogic>());

         #endregion

         // ClientSettingsManager - Singleton
         container.Register(
            Component.For<ClientSettingsManager>());
      }

      #endregion
   }
}
