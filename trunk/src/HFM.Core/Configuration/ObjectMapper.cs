/*
 * HFM.NET - Core ObjectMapper Class
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

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Markup;
using HFM.Queue;

namespace HFM.Core.Configuration
{
   [CoverageExclude]
   public static class ObjectMapper
   {
      public static void CreateMaps()
      {
         Mapper.CreateMap<QueueData, ClientQueue>();
         Mapper.CreateMap<QueueEntry, ClientQueueEntry>();
         Mapper.CreateMap<SlotModel, SlotData>()
            .ForMember(dest => dest.GridData, opt => opt.MapFrom(Mapper.Map<SlotModel, GridData>));
         Mapper.CreateMap<SlotModel, GridData>()
            .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.Status.GetHtmlColor()))
            .ForMember(dest => dest.StatusFontColor, opt => opt.MapFrom(src => src.Status.GetHtmlFontColor()))
            .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ShowETADate ? src.ETADate.ToDateString() : src.ETA.ToString()))
            .ForMember(dest => dest.Failed, opt => opt.MapFrom(src => src.TotalRunFailedUnits))
            .ForMember(dest => dest.DownloadTime, opt => opt.MapFrom(src => src.DownloadTime.ToDateString()))
            .ForMember(dest => dest.PreferredDeadline, opt => opt.MapFrom(src => src.PreferredDeadline.ToDateString()));
      }
   }
}
