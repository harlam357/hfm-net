
using System.IO;

namespace HFM.Client.Tests
{
   internal static class TestData
   {
      internal static readonly string QueueInfo = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\units.txt");
      internal static readonly string Heartbeat = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\heartbeat.txt");
      internal static readonly string Info = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\info.txt");
      internal static readonly string Options = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\options.txt");
      internal static readonly string SimulationInfo = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\simulation-info.txt");
      internal static readonly string SlotInfo = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slots.txt");
      internal static readonly string SlotOptions = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\slot-options.txt");
      internal static readonly string LogRestart = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\log-restart.txt");
      internal static readonly string LogUpdate = File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_1\\log-update_1.txt");

   }
}
