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
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
               Component.For<ILazyComponentLoader>()
                  .ImplementedBy<LazyOfTComponentLoader>());

            // ILogger - Singleton
            container.Register(
               Component.For<Logging.ILogger, Logging.ILoggerEvents>()
                  .ImplementedBy<Logging.Logger>()
                  .UsingFactoryMethod(() => new Logging.Logger(Application.DataFolderPath)));

            // IPreferenceSet - Singleton
            container.Register(
               Component.For<Preferences.IPreferenceSet>()
                  .ImplementedBy<Preferences.PreferenceSet>()
                  .UsingFactoryMethod(() => new Preferences.PreferenceSet(Application.Path, Application.DataFolderPath, Application.FullVersion)));

            // IWorkUnitRepository - Singleton
            container.Register(
               Component.For<Data.IWorkUnitRepository>()
                  .ImplementedBy<Data.WorkUnitRepository>());

            // ClientConfiguration - Singleton
            container.Register(
               Component.For<Client.ClientConfiguration>());

            // ClientFactory - Singleton
            container.Register(
               Component.For<Client.ClientFactory>());

            // HFM.Core.FahClient - Transient
            container.Register(
               Component.For<Client.FahClient>()
                  .LifeStyle.Transient,
               Component.For<Client.IFahClientFactory>()
                  .AsFactory());

            // HFM.Client.TypedMessageConnection - Transient
            container.Register(
               Component.For<HFM.Client.IMessageConnection>()
                  .ImplementedBy<HFM.Client.TypedMessageConnection>()
                     .LifeStyle.Transient);

            // WorkUnitQueryDataContainer - Singleton
            container.Register(
               Component.For<Data.WorkUnitQueryDataContainer>()
                     .OnCreate(instance => instance.Read()));

            // ProteinBenchmarkDataContainer - Singleton
            container.Register(
                Component.For<Data.ProteinBenchmarkDataContainer>()
                    .OnCreate(instance => instance.Read())
                    .OnDestroy(instance => instance.Write()));

            // IProteinBenchmarkService - Singleton
            container.Register(
               Component.For<WorkUnits.IProteinBenchmarkService>()
                  .ImplementedBy<WorkUnits.ProteinBenchmarkService>());

            // IEocStatsService - Singleton
            container.Register(
                Component.For<Services.IEocStatsService>()
                    .ImplementedBy<Services.EocStatsService>());

            // EocStatsDataContainer - Singleton
            container.Register(
               Component.For<Data.EocStatsDataContainer>()
                     .OnCreate(instance => instance.Read()));

            // EocStatsScheduledTask - Singleton
            container.Register(
                Component.For<ScheduledTasks.EocStatsScheduledTask>());

            // ProteinDataContainer
            container.Register(
                Component.For<Data.ProteinDataContainer>()
                    .OnCreate(instance => instance.Read()));

            // IProteinService - Singleton
            container.Register(
               Component.For<WorkUnits.IProteinService>()
                  .ImplementedBy<WorkUnits.ProteinService>());

            // IProjectSummaryService - Singleton
            container.Register(
               Component.For<Services.IProjectSummaryService>()
                  .ImplementedBy<Services.ProjectSummaryService>());
        }
    }
}
