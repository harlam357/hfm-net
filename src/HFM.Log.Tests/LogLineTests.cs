
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class LogLineTests
   {
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
         Assert.AreEqual(LogLineType.ClientArguments, logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LogLine_Create_VerifyPropertyValuesAfterCreate_Test()
      {
         // Arrange & Act
         var logLine = LogLine.Create("Foo", 1, LogLineType.ClientArguments, TimeSpan.FromMinutes(1), "Foo");
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LazyLogLine_VerifyPropertyValuesCanBeSet_Test()
      {
         // Arrange
         var logLine = LogLine.CreateLazy(null, 0, LogLineType.None, line => null, line => null);
         // Act
         logLine.Raw = "Foo";
         logLine.Index = 1;
         logLine.LineType = LogLineType.ClientArguments;
         logLine.TimeStamp = TimeSpan.FromMinutes(1);
         logLine.Data = "Foo";
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }

      [Test]
      public void LazyLogLine_Create_VerifyPropertyValuesAfterCreate_Test()
      {
         // Arrange & Act
         var logLine = LogLine.CreateLazy("Foo", 1, LogLineType.ClientArguments, line => TimeSpan.FromMinutes(1), line => "Foo");
         // Assert
         Assert.AreEqual("Foo", logLine.Raw);
         Assert.AreEqual(1, logLine.Index);
         Assert.AreEqual(LogLineType.ClientArguments, logLine.LineType);
         Assert.AreEqual(TimeSpan.FromMinutes(1), logLine.TimeStamp);
         Assert.AreEqual("Foo", logLine.Data);
      }
   }
}
