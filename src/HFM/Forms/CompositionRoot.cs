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
                .Register<Models.MainModel>(new PerContainerLifetime());

            // Singleton Models
            serviceRegistry
                .Register<Models.MessagesModel>(new PerContainerLifetime());

            // Singleton Views
            serviceRegistry
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
                .Register<Models.BenchmarksModel>(new PerRequestLifeTime())
                .Register<Models.BenchmarksReport, Models.TextBenchmarksReport>(
                    Models.TextBenchmarksReport.KeyName, new PerRequestLifeTime())
                .Register<Models.BenchmarksReport, Models.FrameTimeZedGraphBenchmarksReport>(
                    Models.FrameTimeZedGraphBenchmarksReport.KeyName, new PerRequestLifeTime())
                .Register<Models.BenchmarksReport, Models.ProductionZedGraphBenchmarksReport>(
                    Models.ProductionZedGraphBenchmarksReport.KeyName, new PerRequestLifeTime())
                .Register<Models.BenchmarksReport, Models.ProjectComparisonZedGraphBenchmarksReport>(
                    Models.ProjectComparisonZedGraphBenchmarksReport.KeyName, new PerRequestLifeTime())
                .Register<Models.PreferencesModel>(new PerRequestLifeTime())
                .Register<Models.ProteinCalculatorModel>(new PerRequestLifeTime())
                .Register<Models.WorkUnitHistoryModel>(new PerRequestLifeTime())
                .Register<Core.Data.WorkUnitQuery>(new PerRequestLifeTime());

            // Scope Views
            serviceRegistry
                .Register<AboutDialog>(new PerScopeLifetime())
                .Register<ApplicationUpdatePresenterFactory>(new PerScopeLifetime())
                .Register<BenchmarksPresenter>(new PerScopeLifetime())
                .Register<FahClientSettingsPresenterFactory>(new PerScopeLifetime())
                .Register<MessagesPresenter>(new PerScopeLifetime())
                .Register<PreferencesPresenter>(new PerScopeLifetime())
                .Register<ProteinCalculatorForm>(new PerScopeLifetime())
                .Register<WorkUnitHistoryPresenter>(new PerScopeLifetime())
                .Register<WorkUnitQueryPresenter>(new PerScopeLifetime());

            // Singleton Services
            serviceRegistry.Register<IAutoRunConfiguration, RegistryAutoRunConfiguration>(new PerContainerLifetime());
        }
    }
}
