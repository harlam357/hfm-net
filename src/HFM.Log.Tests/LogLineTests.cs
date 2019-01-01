
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class LogLineTests
   {
      [Test]
      public void LogLine_ConstructorWithParameters_Test()
      {
         // Arrange
         var logLine = new LogLine
         {
            Raw = "Foo",
            Index = 1,
            LineType = LogLineType.LogOpen,
            TimeStamp = TimeSpan.FromMinutes(1),
            Data = "Bar"
         };
         // Act
         var copy = new LogLine(logLine.Raw, logLine.Index, logLine.LineType, logLine.TimeStamp, logLine.Data);
         // Assert
         Assert.AreEqual(logLine.Raw, copy.Raw);
         Assert.AreEqual(logLine.Index, copy.Index);
         Assert.AreEqual(logLine.LineType, copy.LineType);
         Assert.AreEqual(logLine.TimeStamp, copy.TimeStamp);
         Assert.AreEqual(logLine.Data, copy.Data);
      }

      [Test]
      public void LogLine_VerifyPropertyValuesCanBeSet_Test()
      {
         // Arrange
         var logLine = new LogLine();
         // Act
         logLine.Raw = "Foo";
         logLine.Index = 1;
         logLine.LineType = LogLineType.ClientArguments;
         logLine.TimeStamp = TimeSpan.FromMinutes(1);
         logLine.Data = "Foo";
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, (int)logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LogLine_Create_VerifyPropertyValuesAfterCreate_Test()
      {
         // Arrange & Act
         var logLine = new LogLine("Foo", 1, LogLineType.ClientArguments, TimeSpan.FromMinutes(1), "Foo");
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, (int)logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LazyLogLine_VerifyPropertyValuesCanBeSet_Test()
      {
         // Arrange
         var logLine = new LazyLogLine(null, 0, LogLineType.None, line => null, line => null);
         // Act
         logLine.Raw = "Foo";
         logLine.Index = 1;
         logLine.LineType = LogLineType.ClientArguments;
         logLine.TimeStamp = TimeSpan.FromMinutes(1);
         logLine.Data = "Foo";
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, (int)logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LazyLogLine_Create_VerifyPropertyValuesAfterCreate_Test()
      {
         // Arrange & Act
         var logLine = new LazyLogLine("Foo", 1, LogLineType.ClientArguments, line => TimeSpan.FromMinutes(1), line => "Foo");
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, (int)logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }
   }
}
