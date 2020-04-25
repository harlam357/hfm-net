
using AutoMapper;

using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    public class WorkUnitRowProfile : Profile
    {
        public WorkUnitRowProfile()
        {
            CreateMap<WorkUnitModel, WorkUnitRow>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.Data.ProjectID))
                .ForMember(dest => dest.ProjectRun, opt => opt.MapFrom(src => src.Data.ProjectRun))
                .ForMember(dest => dest.ProjectClone, opt => opt.MapFrom(src => src.Data.ProjectClone))
                .ForMember(dest => dest.ProjectGen, opt => opt.MapFrom(src => src.Data.ProjectGen))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SlotModel.SlotIdentifier.Name))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.SlotModel.SlotIdentifier.Client.ToPath()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Data.FoldingID))
                .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.Data.Team))
                .ForMember(dest => dest.CoreVersion, opt => opt.MapFrom(src => src.Data.CoreVersion))
                .ForMember(dest => dest.FramesCompleted, opt => opt.Ignore())
                .ForMember(dest => dest.FrameTimeValue, opt => opt.Ignore())
                .ForMember(dest => dest.ResultValue, opt => opt.MapFrom(src => (int)src.Data.UnitResult))
                .ForMember(dest => dest.DownloadDateTime, opt => opt.MapFrom(src => src.Data.DownloadTime))
                .ForMember(dest => dest.CompletionDateTime, opt => opt.MapFrom(src => src.Data.FinishedTime))
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
