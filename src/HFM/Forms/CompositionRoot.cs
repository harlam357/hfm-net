﻿using HFM.Core.Logging;
using HFM.Forms.Services;
using HFM.Forms.Presenters;
using HFM.Forms.Views;

using LightInject;

namespace HFM.Forms
{
    public class CompositionRoot : ICompositionRoot
    {
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
                    var localProcess = factory.GetInstance<LocalProcessService>();
                    var properties = new Dictionary<string, string>
                    {
                        { "Application", Core.Application.NameAndVersion },
                        { "OS Version", Environment.OSVersion.VersionString }
                    };
                    return new DefaultExceptionPresenterFactory(logger, messageBox, localProcess, properties, Core.Application.SupportForumUrl);
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
            serviceRegistry.RegisterInstance(LocalProcessService.Default);
            serviceRegistry.Register<IAutoRunConfiguration, RegistryAutoRunConfiguration>(new PerContainerLifetime());
        }
    }
}
