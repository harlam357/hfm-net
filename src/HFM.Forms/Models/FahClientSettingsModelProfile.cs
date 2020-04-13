
using AutoMapper;

using HFM.Core.Client;

namespace HFM.Forms.Models
{
    public class FahClientSettingsModelProfile : Profile
    {
        public FahClientSettingsModelProfile()
        {
            CreateMap<ClientSettings, FahClientSettingsModel>();
            CreateMap<FahClientSettingsModel, ClientSettings>()
                .ForMember(dest => dest.ClientType, opt => opt.UseValue(ClientType.FahClient));
        }
    }
}
