using System;
using System.Collections.Generic;

using HFM.Core.Logging;
using HFM.Forms.Presenters;
using HFM.Forms.Views;

using LightInject;

namespace HFM.Forms
{
    public class CompositionRoot : ICompositionRoot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "LightInject ILifetime instances.")]
        public void Compose(IServiceRegistry serviceRegistry)
        {
            // MainPresenter - Singleton
            serviceRegistry
                .Register<MainPresenter>(new PerContainerLifetime())
                .Register<MainForm>(new PerContainerLifetime())
                .Register<IMainView>(factory => factory.GetInstance<MainForm>(), new PerContainerLifetime())
                .Register<System.ComponentModel.ISynchronizeInvoke>(factory => factory.GetInstance<MainForm>(), new PerContainerLifetime())
                .Register<Models.MainGridModel>(new PerContainerLifetime())
                .Register<Models.UserStatsDataModel>(new PerContainerLifetime());

            // Singleton Views
            serviceRegistry
                .Register<IMessagesView, MessagesForm>(new PerContainerLifetime())
                .RegisterInstance(MessageBoxPresenter.Default)
                .Register<ExceptionPresenterFactory>(factory =>
                {
                    var logger = factory.GetInstance<ILogger>();
                    var messageBox = factory.GetInstance<MessageBoxPresenter>();
                    var properties = new Dictionary<string, string>
                    {
                        { "Application", Core.Application.NameAndFullVersion },
                        { "OS Version", Environment.OSVersion.VersionString }
                    };
                    return new DefaultExceptionPresenterFactory(logger, messageBox, properties, Core.Application.SupportForumUrl);
                }, new PerContainerLifetime());

            // Transient Models
            serviceRegistry
                .Register<Models.ProteinCalculatorModel>(new PerRequestLifeTime())
                .Register<Models.WorkUnitHistoryModel>(new PerRequestLifeTime())
                .Register<Core.Data.WorkUnitQuery>(new PerRequestLifeTime());

            // Scope Views
            serviceRegistry
                .Register<IBenchmarksView, BenchmarksForm>(new PerScopeLifetime())
                .Register<WorkUnitHistoryPresenter>(new PerScopeLifetime())
                .Register<IProteinCalculatorView, ProteinCalculatorForm>(new PerScopeLifetime())
                .Register<WorkUnitQueryPresenter>(new PerScopeLifetime());

            // IExternalProcessStarter - Singleton
            serviceRegistry.Register<IExternalProcessStarter, ExternalProcessStarter>(new PerContainerLifetime());
        }
    }
}
