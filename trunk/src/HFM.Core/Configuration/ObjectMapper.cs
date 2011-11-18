
using AutoMapper;

using HFM.Core.DataTypes;
using HFM.Queue;

namespace HFM.Core.Configuration
{
   public static class ObjectMapper
   {
      public static void CreateMaps()
      {
         Mapper.CreateMap<QueueData, ClientQueue>();
         Mapper.CreateMap<QueueEntry, ClientQueueEntry>();
      }
   }
}
