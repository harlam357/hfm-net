
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Log.FahClient;

namespace HFM.Log
{
   // These tests are examples of how to extend and override the FahLog and FahLogReader implementations.

   [TestFixture]
   public class ExtendingFahLogTests
   {
      #region DefineCustomLogLineTypeAndParseData

      private static class CustomLogLineType
      {
         public const int ConnectingTo = 100;
      }

      private class CustomLogLineTypeResolver : FahClientLogLineTypeResolver
      {
         protected override LogLineType OnResolveLogLineType(string line)
         {
            var logLineType = base.OnResolveLogLineType(line);
            if (logLineType != LogLineType.None)
            {
               return logLineType;
            }

            if (line.Contains("Connecting to ")) return CustomLogLineType.ConnectingTo;
            return LogLineType.None;
         }
      }

      private class CustomLogLineParserDictionary : FahClientLogLineDataParserDictionary
      {
         public CustomLogLineParserDictionary()
         {
            Add(CustomLogLineType.ConnectingTo, logLine => logLine.Raw.Substring(33));
         }
      }

      private class CustomFahClientLogTextReader : FahClientLogTextReader
      {
         public CustomFahClientLogTextReader(TextReader textReader)
            : base(textReader, new CustomLogLineTypeResolver(), new CustomLogLineParserDictionary(), null)
         {

         }
      }

      // This test uses a custom FahClientLogTextReader to define a new LogLineType detection and LogLineType data parsing.
      [Test]
      public void FahClientLog_DefineCustomLogLineTypeAndParseData_Test()
      {
         // Arrange
         var log = new FahClientLog();
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
         using (var reader = new CustomFahClientLogTextReader(textReader))
         {
            // Act
            log.Read(reader);
         }
         var lines = LogLineEnumerable.Create(log).Where(x => x.LineType == CustomLogLineType.ConnectingTo).ToList();
         foreach (var logLine in lines)
         {
            System.Diagnostics.Debug.WriteLine(logLine.Data);
         }
         // Assert
         Assert.IsTrue(lines.Count > 0);
      }

      #endregion

      #region AddCustomDataToClientRunData

      private class CustomClientRunData : ClientRunData
      {
         public CustomClientRunData(ClientRunData clientRunData)
            : base(clientRunData)
         {
            
         }

         public List<int> UniqueProjectIDs { get; set; }
      }

      private class CustomRunDataAggregator : FahClientRunDataAggregator
      {
         protected override ClientRunData OnGetClientRunData(ClientRun clientRun)
         {
            var baseData = base.OnGetClientRunData(clientRun);
            var customData = new CustomClientRunData(baseData);
            customData.StartTime = baseData.StartTime;
            customData.UniqueProjectIDs =
               clientRun.SlotRuns
                  .Select(x => x.Value)
                  .SelectMany(x => x.UnitRuns)
                  .Select(x => x.Data.ProjectID)
                  .Distinct()
                  .ToList();
            return customData;
         }
      }

      private class CustomFahClientLog : FahClientLog
      {
         public CustomFahClientLog()
            : base(new CustomRunDataAggregator())
         {
            
         }
      }

      // This test uses a custom FahClientLog, defines a new ClientRunData property, and adds additional aggregator logic to support populating the property.
      // The property value is trivial and something that could easily be gleaned through a LINQ statement.  For demonstration purposes only.
      [Test]
      public void FahClientLog_AddCustomDataToClientRunData_Test()
      {
         // Arrange
         var log = new CustomFahClientLog();
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_14\\log.txt"))
         using (var reader = new FahClientLogTextReader(textReader))
         {
            // Act
            log.Read(reader);
         }
         var clientRun = log.ClientRuns.Last();

         var allProjectIDs = clientRun.SlotRuns
            .Select(x => x.Value)
            .SelectMany(x => x.UnitRuns)
            .Select(x => x.Data.ProjectID)
            .ToList();
         System.Diagnostics.Debug.WriteLine("Total Number of Project IDs: {0}", allProjectIDs.Count);
         var customData = (CustomClientRunData)clientRun.Data;
         System.Diagnostics.Debug.WriteLine("Unique Number of Project IDs: {0}", customData.UniqueProjectIDs.Count);
         foreach (var projectID in customData.UniqueProjectIDs)
         {
            System.Diagnostics.Debug.WriteLine(projectID);
         }
         // Assert
         Assert.AreNotEqual(allProjectIDs.Count, customData.UniqueProjectIDs.Count);
      }

      #endregion
   }
}
