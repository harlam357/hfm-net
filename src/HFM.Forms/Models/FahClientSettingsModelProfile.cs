
using AutoMapper;

using HFM.Core.Client;

namespace HFM.Forms.Models
{
    public class FahClientSettingsModelProfile : Profile
    {
        public FahClientSettingsModelProfile()
        {
            CreateMap<ClientSettings, FahClientSettingsModel>()
                .ForMember(dest => dest.ClientSettings, opt => opt.Ignore())
                .ForMember(dest => dest.ConnectEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.Slots, opt => opt.Ignore());
            CreateMap<FahClientSettingsModel, ClientSettings>()
                .ForMember(dest => dest.ClientType, opt => opt.MapFrom(src => ClientType.FahClient));
        }
    }
}
