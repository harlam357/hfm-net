﻿using AutoMapper;

using HFM.Core.Client;

namespace HFM.Core.Data;

public class WorkUnitRowProfile : Profile
{
    public WorkUnitRowProfile()
    {
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
}
