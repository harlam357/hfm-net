
using System;
using System.Drawing;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.SlotXml
{
    public class XmlBuilderProfile : Profile
    {
        public XmlBuilderProfile()
        {
            CreateMap<SlotModel, SlotData>()
                .ForMember(dest => dest.StatusColor, opt => opt.MapFrom(src => ColorTranslator.ToHtml(src.Status.GetStatusColor())))
                .ForMember(dest => dest.StatusFontColor, opt => opt.MapFrom(src => ColorTranslator.ToHtml(HtmlBuilder.GetHtmlFontColor(src.Status))))
                .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ShowETADate ? src.ETADate.ToStringOrEmpty() : src.ETA.ToString()))
                .ForMember(dest => dest.Core, opt => opt.MapFrom(src => src.Core ?? String.Empty))
                .ForMember(dest => dest.CoreId, opt => opt.MapFrom(src => src.CoreID ?? String.Empty))
                .ForMember(dest => dest.DownloadTime, opt => opt.MapFrom(src => src.DownloadTime.ToStringOrEmpty()))
                .ForMember(dest => dest.PreferredDeadline, opt => opt.MapFrom(src => src.PreferredDeadline.ToStringOrEmpty()))
                .ForMember(dest => dest.Protein, opt => opt.MapFrom(src => src.WorkUnitModel.CurrentProtein));

            CreateMap<Log.LogLine, LogLine>();
            CreateMap<Proteins.Protein, Protein>()
                .ForMember(dest => dest.ServerIP, opt => opt.MapFrom(src => src.ServerIP ?? String.Empty))
                .ForMember(dest => dest.WorkUnitName, opt => opt.MapFrom(src => src.WorkUnitName ?? String.Empty))
                .ForMember(dest => dest.Core, opt => opt.MapFrom(src => src.Core ?? String.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? String.Empty))
                .ForMember(dest => dest.Contact, opt => opt.MapFrom(src => src.Contact ?? String.Empty));
        }
    }
}
