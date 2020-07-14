
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using HFM.Core.Logging;

namespace HFM.Forms.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ContainerInstaller : IWindsorInstaller
    {
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

            // HistoryPresenterModel - Transient
            container.Register(
                Component.For<HistoryPresenter>()
                    .Named("HistoryPresenter")
                    .LifeStyle.Transient,
                Component.For<IHistoryView>()
                    .ImplementedBy<HistoryForm>()
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

            // Singleton Views
            container.Register(
                Component.For<IMessagesView>()
                    .ImplementedBy<MessagesForm>(),
                Component.For<MessageBoxPresenter>()
                    .Instance(MessageBoxPresenter.Default),
                Component.For<ExceptionPresenterFactory>()
                    .UsingFactoryMethod(kernel =>
                    {
                        var logger = kernel.Resolve<ILogger>();
                        var messageBox = kernel.Resolve<MessageBoxPresenter>();
                        var properties = new Dictionary<string, string>
                        {
                            { "Application", Core.Application.NameAndFullVersion }, 
                            { "OS Version", Environment.OSVersion.VersionString }
                        };
                        return new DefaultExceptionPresenterFactory(logger, messageBox, properties, Core.Application.SupportForumUrl);
                    }));

            // Transient Views
            container.Register(
                Component.For<IQueryView>()
                    .ImplementedBy<QueryDialog>()
                    .Named("QueryDialog")
                    .LifeStyle.Transient,
                Component.For<IBenchmarksView>()
                    .ImplementedBy<BenchmarksForm>()
                    .Named("BenchmarksForm")
                    .LifeStyle.Transient,
                Component.For<IProteinCalculatorView>()
                    .ImplementedBy<ProteinCalculatorForm>()
                    .Named("ProteinCalculatorForm")
                    .LifeStyle.Transient,
                Component.For<IViewFactory>()
                    .AsFactory());

            #endregion

            #region Service Interfaces

            // IExternalProcessStarter - Singleton
            container.Register(
                Component.For<IExternalProcessStarter>()
                    .ImplementedBy<ExternalProcessStarter>());

            // IUpdateLogic - Singleton
            container.Register(
                Component.For<IUpdateLogic>()
                    .ImplementedBy<UpdateLogic>());

            #endregion
        }
    }
}
