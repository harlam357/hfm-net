
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRowCsvFileSerializerTests
    {
        [Test]
        public void WorkUnitRowCsvFileSerializer_Serialize_UsingCurrentCulture_Test()
        {
            // Arrange
            var value = new List<WorkUnitRow>();
            value.Add(new WorkUnitRow());
            var serializer = new WorkUnitRowCsvFileSerializer();
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
        public void WorkUnitRowCsvFileSerializer_Serialize_UsingCzechCulture_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            var value = new List<WorkUnitRow>();
            value.Add(new WorkUnitRow());
            var serializer = new WorkUnitRowCsvFileSerializer();
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
