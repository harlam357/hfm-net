
using System;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class WorkUnitFrameDataTests
   {
      [Test]
      public void WorkUnitFrameData_CopyConstructor_Test()
      {
         // Arrange
         var data = new WorkUnitFrameData
         {
            ID = 1,
            RawFramesComplete = 10000,
            RawFramesTotal = 1000000,
            TimeStamp = TimeSpan.FromMinutes(1),
            Duration = TimeSpan.FromMinutes(2)
         };
         // Act
         var copy = new WorkUnitFrameData(data);
         // Assert
         Assert.AreEqual(data.ID, copy.ID);
         Assert.AreEqual(data.RawFramesComplete, copy.RawFramesComplete);
         Assert.AreEqual(data.RawFramesTotal, copy.RawFramesTotal);
         Assert.AreEqual(data.TimeStamp, copy.TimeStamp);
         Assert.AreEqual(data.Duration, copy.Duration);
      }

      [Test]
      public void WorkUnitFrameData_CopyConstructor_OtherIsNull_Test()
      {
         // Act
         var copy = new WorkUnitFrameData(null);
         // Assert
         Assert.AreEqual(0, copy.ID);
         Assert.AreEqual(0, copy.RawFramesComplete);
         Assert.AreEqual(0, copy.RawFramesTotal);
         Assert.AreEqual(default(TimeSpan), copy.TimeStamp);
         Assert.AreEqual(default(TimeSpan), copy.Duration);
      }
   }
}
