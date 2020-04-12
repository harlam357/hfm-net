
using AutoMapper;

using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    public class WorkUnitRowProfile : Profile
    {
        public WorkUnitRowProfile()
        {
            CreateMap<WorkUnit, WorkUnitRow>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SlotIdentifier.Name))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.SlotIdentifier.Client.ToPath()))
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
                .ForMember(dest => dest.ProductionView, opt => opt.Ignore())
                .ForMember(dest => dest.PPD, opt => opt.Ignore())
                .ForMember(dest => dest.Credit, opt => opt.Ignore());
        }
    }
}
