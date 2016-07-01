/*
 * HFM.NET - Core Container Installer
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.IO;

using Castle.Core.Logging;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core.Configuration
{
   [CoverageExclude]
   public class ContainerInstaller : IWindsorInstaller
   {
      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         #region Service Interfaces

         // IAutoRun - Singleton
         container.Register(
            Component.For<IAutoRun>()
               .ImplementedBy<AutoRun>());

         // ILogger - Singleton
         container.Register(
            Component.For<ILogger>()
               .ImplementedBy<Logging.Logger>());

         // IDataRetriever - Transient
         container.Register(
            Component.For<IDataRetriever>()
               .ImplementedBy<LegacyDataRetriever>()
                  .LifeStyle.Transient);

         // IExternalProcessStarter - Singleton
         container.Register(
            Component.For<IExternalProcessStarter>()
               .ImplementedBy<ExternalProcessStarter>());

         // INetworkOps - Transient
         container.Register(
            Component.For<INetworkOps>()
               .ImplementedBy<NetworkOps>()
                  .LifeStyle.Transient);

         // IStatusLogic - Singleton
         container.Register(
            Component.For<IStatusLogic>()
               .ImplementedBy<StatusLogic>());

         // IUnitInfoDatabase - Singleton
         container.Register(
            Component.For<IUnitInfoDatabase>()
               .ImplementedBy<UnitInfoDatabase>());

         // IClientConfiguration - Singleton
         container.Register(
            Component.For<IClientConfiguration>()
               .ImplementedBy<ClientConfiguration>()
                  .OnCreate((kernel, instance) =>
                     instance.ClientEdited += (s, e) =>
                        UpdateBenchmarkData(kernel.Resolve<IProteinBenchmarkCollection>(), e)));

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

         // IClientSettingsManager - Singleton
         container.Register(
            Component.For<IClientSettingsManager>()
               .ImplementedBy<ClientSettingsManager>());

         // IMarkupGenerator - Singleton
         container.Register(
            Component.For<IMarkupGenerator>()
               .ImplementedBy<MarkupGenerator>());

         // IWebsiteDeployer - Singleton
         container.Register(
            Component.For<IWebsiteDeployer>()
               .ImplementedBy<WebsiteDeployer>());

         // IQueryParametersCollection - Singleton
         container.Register(
            Component.For<IQueryParametersCollection>()
               .ImplementedBy<QueryParametersCollection>()
                  .OnCreate((kernel, instance) => instance.Read()));

         // IProteinBenchmarkCollection - Singleton
         container.Register(
            Component.For<IProteinBenchmarkCollection>()
               .ImplementedBy<ProteinBenchmarkCollection>()
                  .OnCreate((kernel, instance) => instance.Read()));

         // IXmlStatsDataContainer - Singleton
         container.Register(
            Component.For<IXmlStatsDataContainer>()
               .ImplementedBy<XmlStatsDataContainer>()
                  .OnCreate((kernel, instance) => instance.Read()));

         // IProteinService - Singleton
         container.Register(
            Component.For<IProteinService>()
               .ImplementedBy<ProteinService>()
                  .OnCreate((kernel, instance) => ((ProteinService)instance).Read()));

         // PluginLoader - Transient
         container.Register(
            Component.For<Plugins.PluginLoader>()
               .LifeStyle.Transient);

         // IProjectSummaryDownloader - Singleton
         container.Register(
            Component.For<IProjectSummaryDownloader>()
               .ImplementedBy<ProjectSummaryDownloader>()
                  .OnCreate((kernel, instance) => ((ProjectSummaryDownloader)instance).FilePath = Path.GetTempFileName()));

         #region Plugins

         // IPluginManager<Plugins.IFileSerializer<T>> - Singleton
         container.Register(
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.Protein>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.Protein>>>(),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>(),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ClientSettings>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ClientSettings>>>(),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.HistoryEntry>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.HistoryEntry>>>());

         #endregion

         #endregion
      }

      private static void UpdateBenchmarkData(IProteinBenchmarkCollection benchmarkCollection, ClientEditedEventArgs e)
      {
         // the name changed
         if (e.PreviousName != e.NewName)
         {
            // update the Names in the benchmark collection
            benchmarkCollection.UpdateOwnerName(e.PreviousName, e.PreviousPath, e.NewName);
         }
         // the path changed
         if (!Paths.Equal(e.PreviousPath, e.NewPath))
         {
            // update the Paths in the benchmark collection
            benchmarkCollection.UpdateOwnerPath(e.NewName, e.PreviousPath, e.NewPath);
         }
      }
   }
}
