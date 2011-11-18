/*
 * HFM.NET - HFM Container Installer
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

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Configuration
{
   public class ContainerInstaller : IWindsorInstaller
   {
      #region IWindsorInstaller Members

      public void Install(IWindsorContainer container, IConfigurationStore store)
      {
         // ArgumentProcessor - Transient
         container.Register(
            Component.For<ArgumentProcessor>()
               .LifeStyle.Transient);
      }

      #endregion
   }
}
