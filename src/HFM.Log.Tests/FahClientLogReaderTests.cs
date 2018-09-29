
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using HFM.Log.FahClient;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class FahClientLogReaderTests
   {
      [Test]
      public void FahClientLogReader_ReadLine_Test()
      {
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
         using (var reader = new FahClientLogReader(textReader))
         {
            LogLine logLine;
            while ((logLine = reader.ReadLine()) != null)
            {
               Assert.IsNotNull(logLine);
               Debug.WriteLine(logLine);
            }
         }
      }

      [Test]
      public async Task FahClientLogReader_ReadLineAsync_Test()
      {
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
         using (var reader = new FahClientLogReader(textReader))
         {
            LogLine logLine;
            while ((logLine = await reader.ReadLineAsync()) != null)
            {
               Assert.IsNotNull(logLine);
               Debug.WriteLine(logLine);
            }
         }
      }
   }
}
