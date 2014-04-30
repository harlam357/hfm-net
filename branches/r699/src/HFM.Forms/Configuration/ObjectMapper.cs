/*
 * HFM.NET - Forms ObjectMapper Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using System.ComponentModel;

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
         Mapper.CreateMap<ClientSettings, LegacyClientSettingsModel>()
            .ForMember(dest => dest.CredentialsErrorMessage, opt => opt.Ignore());
         Mapper.CreateMap<LegacyClientSettingsModel, ClientSettings>()
            .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.Legacy));
         // fahclient settings model
         Mapper.CreateMap<ClientSettings, FahClientSettingsModel>()
            .ConstructUsing(dest => new FahClientSettingsModel(ServiceLocator.Resolve<ISynchronizeInvoke>()));
         Mapper.CreateMap<FahClientSettingsModel, ClientSettings>()
            .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.FahClient))
            .ForMember(dest => dest.LegacyClientSubType, opt => opt.UseValue(LegacyClientSubType.None))
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.FahLogFileName, opt => opt.Ignore())
            .ForMember(dest => dest.UnitInfoFileName, opt => opt.Ignore())
            .ForMember(dest => dest.QueueFileName, opt => opt.Ignore())
            .ForMember(dest => dest.Path, opt => opt.Ignore())
            .ForMember(dest => dest.FtpMode, opt => opt.Ignore())
            .ForMember(dest => dest.UtcOffsetIsZero, opt => opt.Ignore())
            .ForMember(dest => dest.ClientTimeOffset, opt => opt.Ignore());
         // user stats model
         Mapper.CreateMap<XmlStatsData, UserStatsDataModel>()
            .ForMember(dest => dest.Logger, opt => opt.Ignore())
            .ForMember(dest => dest.ControlsVisible, opt => opt.Ignore());
      }
   }
}
