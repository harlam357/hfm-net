/*
 * HFM.NET
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

using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using HFM.Core.DataTypes;
using HFM.Core.Services;
using HFM.Forms.Models;

namespace HFM.Forms.Configuration
{
   [ExcludeFromCodeCoverage]
   public class AutoMapperProfile : Profile
   {
      public AutoMapperProfile()
      {
         // client settings model
         CreateMap<ClientSettings, FahClientSettingsModel>();
         CreateMap<FahClientSettingsModel, ClientSettings>()
             .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.FahClient));
         // user stats model
         CreateMap<EocStatsData, UserStatsDataModel>()
            .DisableCtorValidation()
            .ForMember(dest => dest.Logger, opt => opt.Ignore())
            .ForMember(dest => dest.ControlsVisible, opt => opt.Ignore());
      }
   }
}
