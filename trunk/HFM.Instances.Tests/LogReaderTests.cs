
using System.Collections.Generic;

using NUnit.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class LogReaderTests
   {
      private LogReader reader;
   
      [SetUp]
      public void Init()
      {
         reader = new LogReader();
      }
      
      [Test]
      public void SMPTestLog1()
      {
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "SMP Test FAHlog 1";

         reader.ReadLogText(Instance, "..\\..\\TestFiles\\SMP Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.LogPositions[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[0], 30);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[1], 150);
         Assert.AreEqual(reader.LogPositions[1].ClientStartPosition, 274);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[0], 302);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[1], 402);

         ICollection<string> logLines = reader.GetPreviousWorkUnitLog();
         Assert.IsNotNull(logLines);

         logLines = reader.GetCurrentWorkUnitLog();
         Assert.IsNotNull(logLines);
      }

      [Test]
      public void GPUTestLog1()
      {
         ClientInstance Instance = new ClientInstance(InstanceType.PathInstance);
         Instance.InstanceName = "GPU Test FAHlog 1";

         reader.ReadLogText(Instance, "..\\..\\TestFiles\\GPU Test FAHlog 1.txt");
         reader.ScanFAHLog(Instance);
         Assert.AreEqual(reader.LogPositions[0].ClientStartPosition, 2);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[0], 130);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[1], 326);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[2], 387);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[3], 449);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[4], 510);
         Assert.AreEqual(reader.LogPositions[0].UnitStartPositions[5], 571);

         Assert.AreEqual(reader.LogPositions[1].ClientStartPosition, 618);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[0], 663);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[1], 737);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[2], 935);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[3], 1132);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[4], 1329);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[5], 1526);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[6], 1724);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[7], 1926);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[8], 2123);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[9], 2321);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[10], 2518);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[11], 2715);
         Assert.AreEqual(reader.LogPositions[1].UnitStartPositions[12], 2917);

         ICollection<string> logLines = reader.GetPreviousWorkUnitLog();
         Assert.IsNotNull(logLines);

         logLines = reader.GetCurrentWorkUnitLog();
         Assert.IsNotNull(logLines);
      }
   }
}
