
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using NUnit.Framework;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class WorkUnitHistoryRowCsvFileSerializerTests
    {
        [Test]
        public void WorkUnitHistoryRowCsvFileSerializer_Serialize_UsingCurrentCulture_Test()
        {
            // Arrange
            var value = new List<WorkUnitHistoryRow>();
            value.Add(new WorkUnitHistoryRow());
            var serializer = new WorkUnitHistoryRowCsvFileSerializer();
            string expected = "DatabaseID,ProjectID,ProjectRun,ProjectClone,ProjectGen,Name,Path,Username,Team,CoreVersion,FramesCompleted,FrameTime,Result,DownloadDateTime,CompletionDateTime,WorkUnitName,KFactor,Core,Frames,Atoms,PreferredDays,MaximumDays,SlotType,PPD,Credit";
            expected += Environment.NewLine;
            expected += "0,0,0,0,0,,,,0,0,0,00:00:00,,01/01/0001 00:00:00,01/01/0001 00:00:00,,0,,0,0,0,0,,0,0";
            expected += Environment.NewLine;
            string actual;
            // Act
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                actual = writer.ToString();
            }
            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorkUnitHistoryRowCsvFileSerializer_Serialize_UsingCzechCulture_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            var value = new List<WorkUnitHistoryRow>();
            value.Add(new WorkUnitHistoryRow());
            var serializer = new WorkUnitHistoryRowCsvFileSerializer();
            string expected = "DatabaseID,ProjectID,ProjectRun,ProjectClone,ProjectGen,Name,Path,Username,Team,CoreVersion,FramesCompleted,FrameTime,Result,DownloadDateTime,CompletionDateTime,WorkUnitName,KFactor,Core,Frames,Atoms,PreferredDays,MaximumDays,SlotType,PPD,Credit";
            expected += Environment.NewLine;
            expected += "0,0,0,0,0,,,,0,0,0,00:00:00,,01/01/0001 00:00:00,01/01/0001 00:00:00,,0,,0,0,0,0,,0,0";
            expected += Environment.NewLine;
            string actual;
            // Act
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, value);
                actual = writer.ToString();
            }
            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
