using AutoMapper;

using HFM.Core.Client;

namespace HFM.Core.Data;

public class WorkUnitRowProfile : Profile
{
    public WorkUnitRowProfile()
    {
        CreateMap<WorkUnitEntity, WorkUnitRow>()
            .ForMember(x => x.ProjectID, opt => opt.MapFrom(src => src.Protein.ProjectID))
            .ForMember(x => x.SlotName, opt => opt.MapFrom<WorkUnitRowSlotNameValueResolver>())
            .ForMember(x => x.ConnectionString, opt => opt.MapFrom<WorkUnitRowConnectionStringValueResolver>())
            .ForMember(x => x.KFactor, opt => opt.MapFrom(src => src.Protein.KFactor))
            .ForMember(x => x.Core, opt => opt.MapFrom(src => src.Protein.Core))
            .ForMember(x => x.Frames, opt => opt.MapFrom(src => src.Protein.Frames))
            .ForMember(x => x.Atoms, opt => opt.MapFrom(src => src.Protein.Atoms))
            .ForMember(x => x.SlotType, opt => opt.MapFrom(src => ConvertToSlotType.FromCoreName(src.Protein.Core)))
            .ForMember(x => x.BaseCredit, opt => opt.MapFrom(src => src.Protein.Credit))
            .ForMember(x => x.ClientVersion, opt => opt.MapFrom(src => src.Platform.ClientVersion))
            .ForMember(x => x.OperatingSystem, opt => opt.MapFrom(src => src.Platform.OperatingSystem))
            .ForMember(x => x.DriverVersion, opt => opt.MapFrom(src => src.Platform.DriverVersion))
            .ForMember(x => x.ComputeVersion, opt => opt.MapFrom(src => src.Platform.ComputeVersion))
            .ForMember(x => x.CUDAVersion, opt => opt.MapFrom(src => src.Platform.CUDAVersion));
    }

    private class WorkUnitRowSlotNameValueResolver : IValueResolver<WorkUnitEntity, WorkUnitRow, string>
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

    private class WorkUnitRowConnectionStringValueResolver : IValueResolver<WorkUnitEntity, WorkUnitRow, string>
    {
        public string Resolve(WorkUnitEntity source, WorkUnitRow destination, string destMember, ResolutionContext context)
        {
            var guid = source.Client.Guid is null ? Guid.Empty : Guid.Parse(source.Client.Guid);
            var identifier = ClientIdentifier.FromConnectionString(source.Client.Name, source.Client.ConnectionString, guid);
            return identifier.ToConnectionString();
        }
    }
}
