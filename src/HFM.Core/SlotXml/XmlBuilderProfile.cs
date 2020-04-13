
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
                .ForMember(dest => dest.ETA, opt => opt.MapFrom(src => src.ShowETADate ? src.ETADate.ToStringOrUnknown() : src.ETA.ToString()))
                .ForMember(dest => dest.DownloadTime, opt => opt.MapFrom(src => src.DownloadTime.ToStringOrUnknown()))
                .ForMember(dest => dest.PreferredDeadline, opt => opt.MapFrom(src => src.PreferredDeadline.ToStringOrUnknown()))
                .ForMember(dest => dest.Protein, opt => opt.MapFrom(src => src.WorkUnitModel.CurrentProtein));

            CreateMap<Log.LogLine, LogLine>();
            CreateMap<Proteins.Protein, Protein>();
        }
    }
}
