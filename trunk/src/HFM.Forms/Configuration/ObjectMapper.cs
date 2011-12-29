/*
 * HFM.NET - Forms ObjectMapper Class
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

using AutoMapper;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Configuration
{
   [CoverageExclude]
   public static class ObjectMapper
   {
      public static void CreateMaps()
      {
         // legacy settings model
         Mapper.CreateMap<ClientSettings, LegacyClientSettingsModel>();
         Mapper.CreateMap<LegacyClientSettingsModel, ClientSettings>()
            .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.Legacy));
         // fahclient settings model
         Mapper.CreateMap<ClientSettings, FahClientSettingsModel>();
         Mapper.CreateMap<FahClientSettingsModel, ClientSettings>();
         // user stats model
         Mapper.CreateMap<XmlStatsData, UserStatsDataModel>();
      }
   }
}
