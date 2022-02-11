using System.Globalization;

using AutoMapper;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data;

public class WorkUnitRowProfile : Profile
{
    public WorkUnitRowProfile()
    {
        CreateMap<WorkUnitModel, PetaPocoWorkUnitRow>()
            .ForMember(dest => dest.ID, opt => opt.Ignore())
            .ForMember(dest => dest.ProjectID, opt => opt.MapFrom(src => src.WorkUnit.ProjectID))
            .ForMember(dest => dest.ProjectRun, opt => opt.MapFrom(src => src.WorkUnit.ProjectRun))
            .ForMember(dest => dest.ProjectClone, opt => opt.MapFrom(src => src.WorkUnit.ProjectClone))
            .ForMember(dest => dest.ProjectGen, opt => opt.MapFrom(src => src.WorkUnit.ProjectGen))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SlotModel.SlotIdentifier.Name))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.SlotModel.SlotIdentifier.ClientIdentifier.ToConnectionString()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.WorkUnit.FoldingID))
            .ForMember(dest => dest.Team, opt => opt.MapFrom(src => src.WorkUnit.Team))
            .ForMember(dest => dest.CoreVersion, opt => opt.MapFrom(src => ConvertVersionToFloat(src.WorkUnit.CoreVersion)))
            .ForMember(dest => dest.FramesCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.FrameTimeValue, opt => opt.Ignore())
            .ForMember(dest => dest.ResultValue, opt => opt.MapFrom(src => (int)src.WorkUnit.UnitResult))
            .ForMember(dest => dest.Assigned, opt => opt.MapFrom(src => src.WorkUnit.Assigned))
            .ForMember(dest => dest.Finished, opt => opt.MapFrom(src => src.WorkUnit.Finished))
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

        CreateMap<WorkUnitEntity, WorkUnitEntityRow>()
            .ForMember(x => x.Name, opt => opt.MapFrom<WorkUnitRowNameValueResolver>())
            .ForMember(x => x.Path, opt => opt.MapFrom<WorkUnitRowPathValueResolver>())
            .ForMember(x => x.Username, opt => opt.MapFrom(src => src.DonorName))
            .ForMember(x => x.Team, opt => opt.MapFrom(src => src.DonorTeam))
            .ForMember(x => x.WorkUnitName, opt => opt.MapFrom(src => ""))
            .ForMember(x => x.ProjectID, opt => opt.MapFrom(src => src.Protein.ProjectID))
            .ForMember(x => x.KFactor, opt => opt.MapFrom(src => src.Protein.KFactor))
            .ForMember(x => x.Core, opt => opt.MapFrom(src => src.Protein.Core))
            .ForMember(x => x.Frames, opt => opt.MapFrom(src => src.Protein.Frames))
            .ForMember(x => x.Atoms, opt => opt.MapFrom(src => src.Protein.Atoms))
            .ForMember(x => x.BaseCredit, opt => opt.MapFrom(src => src.Protein.Credit))
            .ForMember(x => x.PreferredDays, opt => opt.MapFrom(src => src.Protein.TimeoutDays))
            .ForMember(x => x.MaximumDays, opt => opt.MapFrom(src => src.Protein.ExpirationDays))
            .ForMember(x => x.SlotType, opt => opt.MapFrom(src => ConvertToSlotType.FromCoreName(src.Protein.Core)));
    }

    private class WorkUnitRowNameValueResolver : IValueResolver<WorkUnitEntity, WorkUnitRow, string>
    {
        public string Resolve(WorkUnitEntity source, WorkUnitRow destination, string destMember, ResolutionContext context)
        {
            var guid = source.Client.Guid is null ? Guid.Empty : Guid.Parse(source.Client.Guid);
            var identifier = new SlotIdentifier(
                ClientIdentifier.FromConnectionString(source.Client.Name, source.Client.ConnectionString, guid),
                source.ClientSlot ?? SlotIdentifier.NoSlotID);
            return identifier.Name;
        }
    }

    private class WorkUnitRowPathValueResolver : IValueResolver<WorkUnitEntity, WorkUnitRow, string>
    {
        public string Resolve(WorkUnitEntity source, WorkUnitRow destination, string destMember, ResolutionContext context)
        {
            var guid = source.Client.Guid is null ? Guid.Empty : Guid.Parse(source.Client.Guid);
            var identifier = ClientIdentifier.FromConnectionString(source.Client.Name, source.Client.ConnectionString, guid);
            return identifier.ToConnectionString();
        }
    }

    private static float ConvertVersionToFloat(Version version)
    {
        if (version is null) return 0.0f;

        IFormatProvider provider = CultureInfo.InvariantCulture;
        string s = version.Major == 0 && version.Build >= 0
            ? String.Format(provider, "{0}.{1}", version.Minor, version.Build)
            : String.Format(provider, "{0}.{1}", version.Major, version.Minor);
        return Single.Parse(s, provider);
    }
}
