/*
 * HFM.NET - Core ObjectMapper Class
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
         Mapper.CreateMap<QueueData, ClientQueue>()
            .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.Legacy));
         Mapper.CreateMap<QueueEntry, ClientQueueEntry>()
            .ForMember(dest => dest.WaitingOn, opt => opt.Ignore())
            .ForMember(dest => dest.Attempts, opt => opt.Ignore())
            .ForMember(dest => dest.NextAttempt, opt => opt.Ignore());
         Mapper.CreateMap<SlotModel, SlotData>()
            .ForMember(dest => dest.GridData, opt => opt.MapFrom(src => Mapper.Map<SlotModel, GridData>(src)));
         Mapper.CreateMap<SlotModel, GridData>()
            .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => src.Status.GetHtmlColor()))
            .ForMember(dest => dest.StatusFontColor, opt => opt.MapFrom(src => src.Status.GetHtmlFontColor()))
            .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ShowETADate ? src.ETADate.ToDateString() : src.ETA.ToString()))
            .ForMember(dest => dest.Failed, opt => opt.MapFrom(src => src.TotalRunFailedUnits))
            .ForMember(dest => dest.DownloadTime, opt => opt.MapFrom(src => src.DownloadTime.ToDateString()))
            .ForMember(dest => dest.PreferredDeadline, opt => opt.MapFrom(src => src.PreferredDeadline.ToDateString()));
         Mapper.CreateMap<SlotData, SlotModel>()
            .ForMember(dest => dest.Prefs, opt => opt.Ignore())
            .ForMember(dest => dest.UnitInfoLogic, opt => opt.Ignore())
            .ForMember(dest => dest.Settings, opt => opt.Ignore())
            .ForMember(dest => dest.SlotOptions, opt => opt.Ignore())
            .ForMember(dest => dest.UserIdIsDuplicate, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectIsDuplicate, opt => opt.Ignore())
            .ForMember(dest => dest.TimeOfLastUnitStart, opt => opt.Ignore())
            .ForMember(dest => dest.TimeOfLastFrameProgress, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(x => x.GridData.Status))
            .ForMember(dest => dest.Queue, opt => opt.Ignore()) // Ignore for now, may want to include later
            .ForMember(dest => dest.UnitLogLines, opt => opt.Ignore()); // Ignore for now, may want to include later
         Mapper.CreateMap<UnitInfo, HistoryEntry>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OwningSlotName))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.OwningClientPath))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.FoldingID))
            .ForMember(dest => dest.FramesCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.FrameTimeValue, opt => opt.Ignore())
            .ForMember(dest => dest.ResultValue, opt => opt.MapFrom(src => (int)src.UnitResult))
            .ForMember(dest => dest.DownloadDateTime, opt => opt.MapFrom(src => src.DownloadTime))
            .ForMember(dest => dest.CompletionDateTime, opt => opt.MapFrom(src => src.FinishedTime))
            .ForMember(dest => dest.WorkUnitName, opt => opt.Ignore())
            .ForMember(dest => dest.KFactor, opt => opt.Ignore())
            .ForMember(dest => dest.Core, opt => opt.Ignore())
            .ForMember(dest => dest.Frames, opt => opt.Ignore())
            .ForMember(dest => dest.Atoms, opt => opt.Ignore())
            .ForMember(dest => dest.BaseCredit, opt => opt.Ignore())
            .ForMember(dest => dest.PreferredDays, opt => opt.Ignore())
            .ForMember(dest => dest.MaximumDays, opt => opt.Ignore())
            .ForMember(dest => dest.SlotType, opt => opt.Ignore())
            .ForMember(dest => dest.ProductionView, opt => opt.Ignore());
      }
   }
}
