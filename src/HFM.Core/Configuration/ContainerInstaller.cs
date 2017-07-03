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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Castle.Core.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core.Configuration
{
   [ExcludeFromCodeCoverage]
   public class ContainerInstaller : IWindsorInstaller
   {
      public string ApplicationPath { get; set; }

      public string ApplicationDataFolderPath { get; set; }

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         container.Register(
            Component.For<ILazyComponentLoader>()
               .ImplementedBy<LazyOfTComponentLoader>());

         #region Service Interfaces

         // ILogger - Singleton
         container.Register(
            Component.For<ILogger>()
               .ImplementedBy<Logging.Logger>()
               .UsingFactoryMethod(() => new Logging.Logger(ApplicationDataFolderPath)));

         // IPreferenceSet - Singleton
         container.Register(
            Component.For<Preferences.IPreferenceSet>()
               .ImplementedBy<Preferences.PreferenceSet>()
               .UsingFactoryMethod(() => new Preferences.PreferenceSet(ApplicationPath, ApplicationDataFolderPath, Application.VersionWithRevision))
               .OnCreate((kernel, instance) =>
               {
                  var logger = (LevelFilteredLogger)kernel.Resolve<ILogger>();
                  instance.PreferenceChanged += (s, e) =>
                  {
                     if (e.Preference == Preferences.Preference.MessageLevel)
                     {
                        var newLevel = (LoggerLevel)instance.Get<int>(Preferences.Preference.MessageLevel);
                        if (newLevel != logger.Level)
                        {
                           logger.Level = newLevel;
                           logger.InfoFormat("Debug Message Level Changed: {0}", newLevel);
                        }
                     }
                  };
               }));

         // IDataRetriever - Transient
         container.Register(
            Component.For<IDataRetriever>()
               .ImplementedBy<LegacyDataRetriever>()
                  .LifeStyle.Transient);

         // INetworkOps - Transient
         container.Register(
            Component.For<INetworkOps>()
               .ImplementedBy<NetworkOps>()
                  .LifeStyle.Transient);

         // IUnitInfoDatabase - Singleton
         container.Register(
            Component.For<Data.SQLite.IUnitInfoDatabase>()
               .ImplementedBy<Data.SQLite.UnitInfoDatabase>());

         // IClientConfiguration - Singleton
         container.Register(
            Component.For<IClientConfiguration>()
               .ImplementedBy<ClientConfiguration>()
                  .OnCreate((kernel, instance) => instance.SubscribeToEvents(
                     kernel.Resolve<IProteinBenchmarkService>())));

         // IClientFactory - Singleton
         container.Register(
            Component.For<IClientFactory>()
               .ImplementedBy<ClientFactory>());

         // HFM.Core.FahClient - Transient
         container.Register(
            Component.For<FahClient>()
               .LifeStyle.Transient,
            Component.For<IFahClientFactory>()
               .AsFactory());

         // HFM.Client.TypedMessageConnection - Transient
         container.Register(
            Component.For<HFM.Client.IMessageConnection>()
               .ImplementedBy<HFM.Client.TypedMessageConnection>()
                  .LifeStyle.Transient);

         // LegacyClient - Transient
         container.Register(
            Component.For<LegacyClient>()
               .LifeStyle.Transient,
            Component.For<ILegacyClientFactory>()
               .AsFactory());

         // RetrievalModel - Singleton
         container.Register(
            Component.For<ScheduledTasks.RetrievalModel>());

         // IMarkupGenerator - Singleton
         container.Register(
            Component.For<ScheduledTasks.IMarkupGenerator>()
               .ImplementedBy<ScheduledTasks.MarkupGenerator>());

         // IWebsiteDeployer - Singleton
         container.Register(
            Component.For<ScheduledTasks.IWebsiteDeployer>()
               .ImplementedBy<ScheduledTasks.WebsiteDeployer>());

         // IQueryParametersContainer - Singleton
         container.Register(
            Component.For<Data.IQueryParametersContainer>()
               .ImplementedBy<Data.QueryParametersContainer>()
                  .OnCreate(instance => ((Data.QueryParametersContainer)instance).Read()));

         // IProteinBenchmarkCollection - Singleton
         container.Register(
            Component.For<IProteinBenchmarkService>()
               .ImplementedBy<ProteinBenchmarkService>()
                  .OnCreate(instance => ((ProteinBenchmarkService)instance).Read())
                  .OnDestroy(instance => ((ProteinBenchmarkService)instance).Write()));

         // IXmlStatsDataContainer - Singleton
         container.Register(
            Component.For<Data.IXmlStatsDataContainer>()
               .ImplementedBy<Data.XmlStatsDataContainer>()
                  .OnCreate(instance => ((Data.XmlStatsDataContainer)instance).Read()));

         // IProteinService - Singleton
         container.Register(
            Component.For<IProteinService>()
               .ImplementedBy<ProteinService>()
                  .OnCreate(instance => ((ProteinService)instance).Read()));

         // PluginLoader - Transient
         container.Register(
            Component.For<Plugins.PluginLoader>()
               .LifeStyle.Transient);

         // IProjectSummaryDownloader - Singleton
         container.Register(
            Component.For<IProjectSummaryDownloader>()
               .ImplementedBy<ProjectSummaryDownloader>());

         #region Plugins

         // IPluginManager<Plugins.IFileSerializer<T>> - Singleton
         container.Register(
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>(),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.HistoryEntry>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.HistoryEntry>>>());

         #endregion

         #endregion
      }
   }
}
