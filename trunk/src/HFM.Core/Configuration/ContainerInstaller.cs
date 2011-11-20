/*
 * HFM.NET - Core Container Installer
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core.Configuration
{
   public class ContainerInstaller : IWindsorInstaller
   {
      #region IWindsorInstaller Members

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         #region Service Interfaces

         // IDataAggregator - Transient
         container.Register(
            Component.For<IDataAggregator>()
               .ImplementedBy<DataAggregator>()
               .LifeStyle.Transient);

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

         // IProteinBenchmarkCollection - Singleton
         container.Register(
            Component.For<IProteinBenchmarkCollection>()
               .ImplementedBy<ProteinBenchmarkCollection>()
               .OnCreate((kernel, instance) => instance.Read()));

         // IQueryParametersCollection - Singleton
         container.Register(
            Component.For<IQueryParametersCollection>()
               .ImplementedBy<QueryParametersCollection>());

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

         #region Plugins

         // IPluginManager<Plugins.IFileSerializer<T>> - Singleton
         container.Register(
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.Protein>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.Protein>>>(),
               //.Named("PluginManager.ProteinFileSerializer"),
            Component.For<Plugins.IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>()
               .ImplementedBy<Plugins.FileSerializerPluginManager<List<DataTypes.ProteinBenchmark>>>());
               //.Named("PluginManager.ProteinBenchmarkFileSerializer"));

         #endregion

         #endregion
      }

      #endregion
   }
}
