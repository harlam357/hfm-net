/*
 * HFM.NET - Service Locator Class
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

using System.Diagnostics.CodeAnalysis;

using Castle.Windsor;

namespace HFM.Core
{
   [CoverageExclude]
   public static class ServiceLocator
   {
      private static IWindsorContainer _container;

      public static void SetContainer(IWindsorContainer container)
      {
         _container = container;
      }

      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      public static T Resolve<T>()
      {
         return _container.Resolve<T>();
      }

      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      public static T Resolve<T>(string key)
      {
         return _container.Resolve<T>(key);
      }
   }
}
