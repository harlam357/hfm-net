using System.Globalization;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRowCsvFileSerializerTests
    {
        private List<WorkUnitRow> _value;
        private string _expected;

        [SetUp]
        public void BeforeEach()
        {
            // Arrange
            _value = new List<WorkUnitRow> { new() };
            _expected = "DatabaseID,ProjectID,ProjectRun,ProjectClone,ProjectGen,Name,Path,Username,Team,CoreVersion,FramesCompleted,FrameTime,Result,Assigned,Finished,WorkUnitName,KFactor,Core,Frames,Atoms,PreferredDays,MaximumDays,SlotType,PPD,Credit,BaseCredit";
            _expected += Environment.NewLine;
            _expected += "0,0,0,0,0,,,,0,,0,00:00:00,,01/01/0001 00:00:00,01/01/0001 00:00:00,,0,,0,0,0,0,,0,0,0";
            _expected += Environment.NewLine;
        }

        [Test]
        public void WorkUnitRowCsvFileSerializer_Serialize_UsingCurrentCulture_Test()
        {
            // Act
            string actual;
            using (var writer = new StringWriter())
            {
                WorkUnitRowCsvFileSerializer.Serialize(writer, _value);
                actual = writer.ToString();
            }
            Assert.AreEqual(_expected, actual);
        }

        [Test]
        public void WorkUnitRowCsvFileSerializer_Serialize_UsingCzechCulture_Test()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");
            // Act
            string actual;
            using (var writer = new StringWriter())
            {
                WorkUnitRowCsvFileSerializer.Serialize(writer, _value);
                actual = writer.ToString();
            }
            // Assert
            Assert.AreEqual(_expected, actual);
        }
    }
}
