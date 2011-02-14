/*
 * HFM.NET - XML Stats Data Container Interface
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IXmlStatsDataContainer
   {
      /// <summary>
      /// User Stats Data
      /// </summary>
      XmlStatsData Data { get; }

      /// <summary>
      /// Is it Time for a Stats Update?
      /// </summary>
      bool TimeForUpdate();

      /// <summary>
      /// Get Overall User Data from EOC XML
      /// </summary>
      /// <param name="forceRefresh">Force Refresh or allow to check for next update time</param>
      void GetEocXmlData(bool forceRefresh);

      /// <summary>
      /// Read Binary File
      /// </summary>
      void Read();

      /// <summary>
      /// Write Binary File
      /// </summary>
      void Write();
   }
}
