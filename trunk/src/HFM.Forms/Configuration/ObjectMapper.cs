
using AutoMapper;

using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms.Configuration
{
   public static class ObjectMapper
   {
      public static void CreateMaps()
      {
         Mapper.CreateMap<ClientSettings, LegacyClientSettingsModel>();
      }
   }
}
