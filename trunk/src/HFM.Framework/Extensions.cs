/*
 * HFM.NET - Framework Extension Methods
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public static class Extensions
   {
      /// <summary>
      /// Equals Project (R/C/G) and Download Time?
      /// </summary>
      public static bool EqualsUnitInfoLogic(this IUnitInfoLogic source, IUnitInfoLogic unitInfoLogic)
      {
         // if the Projects are known
         if (source != null && source.UnitInfoData.ProjectIsUnknown() == false &&
             unitInfoLogic != null && unitInfoLogic.UnitInfoData.ProjectIsUnknown() == false)
         {
            // equals the Project and Download Time
            if (source.UnitInfoData.EqualsProject(unitInfoLogic.UnitInfoData) &&
                source.UnitInfoData.DownloadTime.Equals(unitInfoLogic.UnitInfoData.DownloadTime))
            {
               return true;
            }
         }

         return false;
      }
   }
}
