/*
 * HFM.NET - Core Container Installer
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core.Configuration
{
   [CoverageExclude]
   public class ContainerInstaller : IWindsorInstaller
   {
      #region IWindsorInstaller Members

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         #region Service Interfaces

         // ILogger - Singleton
         container.Register(
            Component.For<ILogger>()
               .ImplementedBy<Logging.Logger>());

         // IFahClientDataAggregator - Transient
         container.Register(
            Component.For<IFahClientDataAggregator>()
               .ImplementedBy<FahClientDataAggregator>()
               .LifeStyle.Transient);

         // ILegacyDataAggregator - Transient
         container.Register(
            Component.For<ILegacyDataAggregator>()
               .ImplementedBy<LegacyDataAggregator>()
               .LifeStyle.Transient);

         // IDataRetriever - Transient
         container.Register(
            Component.For<IDataRetriever>()
               .ImplementedBy<LegacyDataRetriever>()
               .LifeStyle.Transient);

         // UnitInfoLogic - Transient
         container.Register(
            Component.For<UnitInfoLogic>()
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

         // IFahClientInterface - Transient
         container.Register(
            Component.For<IFahClientInterface>()
               .ImplementedBy<FahClientInterfaceAdapter>()
               .LifeStyle.Transient);

         // IStatusLogic - Singleton
         container.Register(
            Component.For<IStatusLogic>()
               .ImplementedBy<StatusLogic>());

         // IUnitInfoDatabase - Singleton
         container.Register(
            Component.For<IUnitInfoDatabase>()
               .ImplementedBy<UnitInfoDatabase>());

         // ProteinDataUpdater - Transient
         container.Register(
            Component.For<ProteinDataUpdater>()
               .LifeStyle.Transient);

         // IClientDictionary - Singleton
         container.Register(
            Component.For<IClientDictionary>()
               .ImplementedBy<ClientDictionary>());

         // IClientFactory - Singleton
         container.Register(
            Component.For<IClientFactory>()
               .ImplementedBy<ClientFactory>());

         // FahClient - Transient
         container.Register(
            Component.For<FahClient>()
            .LifeStyle.Transient);

         // LegacyClient - Transient
         container.Register(
            Component.For<LegacyClient>()
            .LifeStyle.Transient);

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

         // IUnitInfoCollection - Singleton
         container.Register(
            Component.For<IUnitInfoCollection>()
               .ImplementedBy<UnitInfoCollection>()
               .OnCreate((kernel, instance) => instance.Read()));

         // IXmlStatsDataContainer - Singleton
         container.Register(
            Component.For<IXmlStatsDataContainer>()
               .ImplementedBy<XmlStatsDataContainer>()
               .OnCreate((kernel, instance) => instance.Read()));

         // IProteinDictionary - Singleton
         container.Register(
            Component.For<IProteinDictionary>()
               .ImplementedBy<ProteinDictionary>()
               .OnCreate((kernel, instance) => instance.Read()));

         // PluginLoader - Transient
         container.Register(
            Component.For<Plugins.PluginLoader>()
            .LifeStyle.Transient);

         // IProjectSummaryDownloader - Singleton
         container.Register(
            Component.For<IProjectSummaryDownloader>()
               .ImplementedBy<ProjectSummaryDownloader>()
               .OnCreate((kernel, instance) => instance.DownloadFilePath = Path.GetTempFileName()));

         #region Plugins

         // IPluginManager<Plugins.IFileSerializer<T>> - Singleton
         container.Register(
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.Protein>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.Protein>>>(),
               //.Named("PluginManager.ProteinFileSerializer"),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>(),
               //.Named("PluginManager.ProteinBenchmarkFileSerializer"));
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ClientSettings>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ClientSettings>>>());
               //.Named("PluginManager.ClientSettingsFileSerializer"));

         #endregion

         #endregion
      }

      #endregion
   }
}
