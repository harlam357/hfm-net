
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log.FahClient
{
    [TestFixture]
    public class FahClientLogTests
    {
        [Test]
        public async Task FahClientLog_ReadAsync_FromFahLogReader_Test()
        {
            // Arrange
            var log = new FahClientLog();
            using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                // Act
                await log.ReadAsync(reader);
            }
            // Assert
            Assert.IsTrue(log.ClientRuns.Count > 0);
        }

        [Test]
        public async Task FahClientLog_ReadAsync_FromPath_Test()
        {
            var log = await FahClientLog.ReadAsync("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt");
            Assert.IsTrue(log.ClientRuns.Count > 0);
        }

        [Test]
        public void FahClientLog_Clear_Test()
        {
            // Arrange
            var log = new FahClientLog();
            using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
            using (var reader = new FahClientLogTextReader(textReader))
            {
                log.Read(reader);
            }
            Assert.IsTrue(log.ClientRuns.Count > 0);
            // Act
            log.Clear();
            // Assert
            Assert.AreEqual(0, log.ClientRuns.Count);
        }

        // ReSharper disable InconsistentNaming

        [Test]
        public void FahClientLog_Read_Client_v7_10_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 0
            var expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 85, 402);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 25, 32);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 10;
            expectedUnitRunData.ProjectID = 7610;
            expectedUnitRunData.ProjectRun = 630;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 59;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
            var frameData = new WorkUnitFrameData();
            frameData.ID = 24;
            frameData.RawFramesComplete = 480000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(125930000000);
            frameData.Duration = TimeSpan.FromTicks(0);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 25;
            frameData.RawFramesComplete = 500000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(130980000000);
            frameData.Duration = TimeSpan.FromTicks(5050000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 26;
            frameData.RawFramesComplete = 520000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(135990000000);
            frameData.Duration = TimeSpan.FromTicks(5010000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 27;
            frameData.RawFramesComplete = 540000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(141010000000);
            frameData.Duration = TimeSpan.FromTicks(5020000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 28;
            frameData.RawFramesComplete = 560000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(146130000000);
            frameData.Duration = TimeSpan.FromTicks(5120000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 29;
            frameData.RawFramesComplete = 580000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(151190000000);
            frameData.Duration = TimeSpan.FromTicks(5060000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 30;
            frameData.RawFramesComplete = 600000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(156270000000);
            frameData.Duration = TimeSpan.FromTicks(5080000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 31;
            frameData.RawFramesComplete = 620000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(161400000000);
            frameData.Duration = TimeSpan.FromTicks(5130000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 32;
            frameData.RawFramesComplete = 640000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(166570000000);
            frameData.Duration = TimeSpan.FromTicks(5170000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 33;
            frameData.RawFramesComplete = 660000;
            frameData.RawFramesTotal = 2000000;
            frameData.TimeStamp = TimeSpan.FromTicks(171680000000);
            frameData.Duration = TimeSpan.FromTicks(5110000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 0;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup SlotRun 1
            expectedSlotRun = new SlotRun(expectedRun, 1);
            expectedRun.SlotRuns.Add(1, expectedSlotRun);

            // Setup SlotRun 1 - UnitRun 0
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 90, 349);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 25, 36);
            expectedUnitRunData.CoreVersion = "1.31";
            expectedUnitRunData.FramesObserved = 100;
            expectedUnitRunData.ProjectID = 5767;
            expectedUnitRunData.ProjectRun = 3;
            expectedUnitRunData.ProjectClone = 138;
            expectedUnitRunData.ProjectGen = 144;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
            frameData = new WorkUnitFrameData();
            frameData.ID = 1;
            frameData.RawFramesComplete = 1;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(123790000000);
            frameData.Duration = TimeSpan.FromTicks(0);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 2;
            frameData.RawFramesComplete = 2;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(124120000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 3;
            frameData.RawFramesComplete = 3;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(124460000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 4;
            frameData.RawFramesComplete = 4;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(124790000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 5;
            frameData.RawFramesComplete = 5;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(125130000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 6;
            frameData.RawFramesComplete = 6;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(125460000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 7;
            frameData.RawFramesComplete = 7;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(125800000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 8;
            frameData.RawFramesComplete = 8;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(126140000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 9;
            frameData.RawFramesComplete = 9;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(126470000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 10;
            frameData.RawFramesComplete = 10;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(126810000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 11;
            frameData.RawFramesComplete = 11;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(127140000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 12;
            frameData.RawFramesComplete = 12;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(127480000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 13;
            frameData.RawFramesComplete = 13;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(127810000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 14;
            frameData.RawFramesComplete = 14;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(128150000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 15;
            frameData.RawFramesComplete = 15;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(128480000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 16;
            frameData.RawFramesComplete = 16;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(128820000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 17;
            frameData.RawFramesComplete = 17;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(129150000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 18;
            frameData.RawFramesComplete = 18;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(129490000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 19;
            frameData.RawFramesComplete = 19;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(129820000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 20;
            frameData.RawFramesComplete = 20;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(130160000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 21;
            frameData.RawFramesComplete = 21;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(130490000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 22;
            frameData.RawFramesComplete = 22;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(130830000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 23;
            frameData.RawFramesComplete = 23;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(131170000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 24;
            frameData.RawFramesComplete = 24;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(131500000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 25;
            frameData.RawFramesComplete = 25;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(131840000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 26;
            frameData.RawFramesComplete = 26;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(132170000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 27;
            frameData.RawFramesComplete = 27;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(132510000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 28;
            frameData.RawFramesComplete = 28;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(132840000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 29;
            frameData.RawFramesComplete = 29;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(133180000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 30;
            frameData.RawFramesComplete = 30;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(133510000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 31;
            frameData.RawFramesComplete = 31;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(133850000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 32;
            frameData.RawFramesComplete = 32;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(134180000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 33;
            frameData.RawFramesComplete = 33;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(134520000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 34;
            frameData.RawFramesComplete = 34;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(134850000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 35;
            frameData.RawFramesComplete = 35;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(135190000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 36;
            frameData.RawFramesComplete = 36;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(135520000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 37;
            frameData.RawFramesComplete = 37;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(135860000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 38;
            frameData.RawFramesComplete = 38;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(136200000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 39;
            frameData.RawFramesComplete = 39;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(136530000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 40;
            frameData.RawFramesComplete = 40;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(136870000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 41;
            frameData.RawFramesComplete = 41;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(137200000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 42;
            frameData.RawFramesComplete = 42;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(137540000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 43;
            frameData.RawFramesComplete = 43;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(137870000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 44;
            frameData.RawFramesComplete = 44;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(138210000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 45;
            frameData.RawFramesComplete = 45;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(138540000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 46;
            frameData.RawFramesComplete = 46;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(138880000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 47;
            frameData.RawFramesComplete = 47;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(139210000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 48;
            frameData.RawFramesComplete = 48;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(139550000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 49;
            frameData.RawFramesComplete = 49;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(139880000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 50;
            frameData.RawFramesComplete = 50;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(140220000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 51;
            frameData.RawFramesComplete = 51;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(140550000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 52;
            frameData.RawFramesComplete = 52;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(140890000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 53;
            frameData.RawFramesComplete = 53;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(141230000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 54;
            frameData.RawFramesComplete = 54;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(141560000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 55;
            frameData.RawFramesComplete = 55;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(141900000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 56;
            frameData.RawFramesComplete = 56;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(142230000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 57;
            frameData.RawFramesComplete = 57;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(142570000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 58;
            frameData.RawFramesComplete = 58;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(142900000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 59;
            frameData.RawFramesComplete = 59;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(143240000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 60;
            frameData.RawFramesComplete = 60;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(143570000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 61;
            frameData.RawFramesComplete = 61;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(143910000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 62;
            frameData.RawFramesComplete = 62;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(144240000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 63;
            frameData.RawFramesComplete = 63;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(144580000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 64;
            frameData.RawFramesComplete = 64;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(144910000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 65;
            frameData.RawFramesComplete = 65;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(145250000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 66;
            frameData.RawFramesComplete = 66;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(145580000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 67;
            frameData.RawFramesComplete = 67;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(145920000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 68;
            frameData.RawFramesComplete = 68;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(146260000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 69;
            frameData.RawFramesComplete = 69;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(146590000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 70;
            frameData.RawFramesComplete = 70;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(146930000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 71;
            frameData.RawFramesComplete = 71;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(147260000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 72;
            frameData.RawFramesComplete = 72;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(147600000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 73;
            frameData.RawFramesComplete = 73;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(147930000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 74;
            frameData.RawFramesComplete = 74;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(148270000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 75;
            frameData.RawFramesComplete = 75;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(148600000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 76;
            frameData.RawFramesComplete = 76;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(148940000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 77;
            frameData.RawFramesComplete = 77;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(149270000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 78;
            frameData.RawFramesComplete = 78;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(149610000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 79;
            frameData.RawFramesComplete = 79;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(149940000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 80;
            frameData.RawFramesComplete = 80;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(150280000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 81;
            frameData.RawFramesComplete = 81;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(150610000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 82;
            frameData.RawFramesComplete = 82;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(150950000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 83;
            frameData.RawFramesComplete = 83;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(151280000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 84;
            frameData.RawFramesComplete = 84;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(151620000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 85;
            frameData.RawFramesComplete = 85;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(151960000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 86;
            frameData.RawFramesComplete = 86;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(152290000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 87;
            frameData.RawFramesComplete = 87;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(152630000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 88;
            frameData.RawFramesComplete = 88;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(152960000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 89;
            frameData.RawFramesComplete = 89;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(153300000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 90;
            frameData.RawFramesComplete = 90;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(153630000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 91;
            frameData.RawFramesComplete = 91;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(153970000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 92;
            frameData.RawFramesComplete = 92;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(154300000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 93;
            frameData.RawFramesComplete = 93;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(154640000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 94;
            frameData.RawFramesComplete = 94;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(154970000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 95;
            frameData.RawFramesComplete = 95;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(155310000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 96;
            frameData.RawFramesComplete = 96;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(155650000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 97;
            frameData.RawFramesComplete = 97;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(155990000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 98;
            frameData.RawFramesComplete = 98;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(156320000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 99;
            frameData.RawFramesComplete = 99;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(156660000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 100;
            frameData.RawFramesComplete = 100;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(156990000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 276, 413);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 21, 52);
            expectedUnitRunData.CoreVersion = "1.31";
            expectedUnitRunData.FramesObserved = 53;
            expectedUnitRunData.ProjectID = 5772;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 364;
            expectedUnitRunData.ProjectGen = 252;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
            frameData = new WorkUnitFrameData();
            frameData.ID = 1;
            frameData.RawFramesComplete = 1;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(157520000000);
            frameData.Duration = TimeSpan.FromTicks(0);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 2;
            frameData.RawFramesComplete = 2;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(157860000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 3;
            frameData.RawFramesComplete = 3;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(158190000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 4;
            frameData.RawFramesComplete = 4;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(158530000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 5;
            frameData.RawFramesComplete = 5;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(158870000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 6;
            frameData.RawFramesComplete = 6;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(159200000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 7;
            frameData.RawFramesComplete = 7;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(159540000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 8;
            frameData.RawFramesComplete = 8;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(159870000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 9;
            frameData.RawFramesComplete = 9;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(160210000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 10;
            frameData.RawFramesComplete = 10;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(160550000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 11;
            frameData.RawFramesComplete = 11;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(160880000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 12;
            frameData.RawFramesComplete = 12;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(161220000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 13;
            frameData.RawFramesComplete = 13;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(161550000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 14;
            frameData.RawFramesComplete = 14;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(161890000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 15;
            frameData.RawFramesComplete = 15;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(162230000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 16;
            frameData.RawFramesComplete = 16;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(162560000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 17;
            frameData.RawFramesComplete = 17;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(162900000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 18;
            frameData.RawFramesComplete = 18;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(163230000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 19;
            frameData.RawFramesComplete = 19;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(163570000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 20;
            frameData.RawFramesComplete = 20;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(163900000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 21;
            frameData.RawFramesComplete = 21;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(164240000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 22;
            frameData.RawFramesComplete = 22;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(164580000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 23;
            frameData.RawFramesComplete = 23;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(164910000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 24;
            frameData.RawFramesComplete = 24;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(165250000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 25;
            frameData.RawFramesComplete = 25;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(165580000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 26;
            frameData.RawFramesComplete = 26;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(165920000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 27;
            frameData.RawFramesComplete = 27;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(166260000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 28;
            frameData.RawFramesComplete = 28;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(166590000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 29;
            frameData.RawFramesComplete = 29;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(166930000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 30;
            frameData.RawFramesComplete = 30;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(167260000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 31;
            frameData.RawFramesComplete = 31;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(167600000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 32;
            frameData.RawFramesComplete = 32;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(167940000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 33;
            frameData.RawFramesComplete = 33;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(168270000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 34;
            frameData.RawFramesComplete = 34;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(168610000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 35;
            frameData.RawFramesComplete = 35;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(168940000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 36;
            frameData.RawFramesComplete = 36;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(169280000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 37;
            frameData.RawFramesComplete = 37;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(169620000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 38;
            frameData.RawFramesComplete = 38;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(169960000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 39;
            frameData.RawFramesComplete = 39;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(170290000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 40;
            frameData.RawFramesComplete = 40;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(170630000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 41;
            frameData.RawFramesComplete = 41;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(170960000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 42;
            frameData.RawFramesComplete = 42;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(171300000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 43;
            frameData.RawFramesComplete = 43;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(171640000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 44;
            frameData.RawFramesComplete = 44;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(171970000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 45;
            frameData.RawFramesComplete = 45;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(172310000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 46;
            frameData.RawFramesComplete = 46;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(172640000000);
            frameData.Duration = TimeSpan.FromTicks(330000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 47;
            frameData.RawFramesComplete = 47;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(172980000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 48;
            frameData.RawFramesComplete = 48;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(173320000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 49;
            frameData.RawFramesComplete = 49;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(173660000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 50;
            frameData.RawFramesComplete = 50;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(174020000000);
            frameData.Duration = TimeSpan.FromTicks(360000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 51;
            frameData.RawFramesComplete = 51;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(174360000000);
            frameData.Duration = TimeSpan.FromTicks(340000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 52;
            frameData.RawFramesComplete = 52;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(174710000000);
            frameData.Duration = TimeSpan.FromTicks(350000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 53;
            frameData.RawFramesComplete = 53;
            frameData.RawFramesTotal = 100;
            frameData.TimeStamp = TimeSpan.FromTicks(175130000000);
            frameData.Duration = TimeSpan.FromTicks(420000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 1
            expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 1;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            var expectedRunData = new FahClientClientRunData();
            expectedRunData.StartTime = new DateTime(2012, 1, 11, 3, 24, 22, DateTimeKind.Utc);
            expectedRun.Data = expectedRunData;

            var actualRun = fahLog.ClientRuns.ElementAt(0);
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_13_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_13\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 1
            var expectedSlotRun = new SlotRun(expectedRun, 1);
            expectedRun.SlotRuns.Add(1, expectedSlotRun);

            // Setup SlotRun 1 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 2, 74, 212);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 57, 36);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 28;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 416;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 32;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 161, 522);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 44, 55);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 274;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 54;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 471, 831);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 41, 51);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 681;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 51;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 780, 1141);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 38, 53);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 1573;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 38;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1090, 1451);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 36, 43);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 529;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 41;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1400, 1760);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 33, 23);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 715;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 49;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1709, 2070);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 30, 18);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 248;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 51;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2019, 2301);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 26, 26);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 88;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 1719;
            expectedUnitRunData.ProjectClone = 9;
            expectedUnitRunData.ProjectGen = 68;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 1
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 7;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup SlotRun 0
            expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 79, 271);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 57, 36);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 31;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 340;
            expectedUnitRunData.ProjectClone = 5;
            expectedUnitRunData.ProjectGen = 36;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 219, 581);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 59, 51);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 430;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 48;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 529, 890);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 57, 4);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 291;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 54;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 838, 1200);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 54, 4);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 1958;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 48;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1148, 1510);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 51, 12);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 509;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 33;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1458, 1819);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 48, 2);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 507;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 49;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1767, 2129);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 44, 44);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13001;
            expectedUnitRunData.ProjectRun = 228;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 62;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2078, 2302);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 41, 56);
            expectedUnitRunData.CoreVersion = "0.0.52";
            expectedUnitRunData.FramesObserved = 86;
            expectedUnitRunData.ProjectID = 13000;
            expectedUnitRunData.ProjectRun = 671;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 50;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 7;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            expectedRun.Data = new FahClientClientRunData();
            expectedRun.Data.StartTime = new DateTime(2014, 7, 25, 13, 57, 36, DateTimeKind.Utc);

            var actualRun = fahLog.ClientRuns.Last();
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_14_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_14\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 1
            var expectedSlotRun = new SlotRun(expectedRun, 1);
            expectedRun.SlotRuns.Add(1, expectedSlotRun);

            // Setup SlotRun 1 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 98, 643);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 47, 0);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 91;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 14;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 87;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 605, 987);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 20, 45);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10496;
            expectedUnitRunData.ProjectRun = 80;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 3;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 948, 1284);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 28, 59);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9704;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 171;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1244, 1550);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 55, 55);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9712;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 167;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1509, 1838);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 47, 52);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 88;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 170;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1801, 2094);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 48, 25);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9704;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 11;
            expectedUnitRunData.ProjectGen = 504;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2054, 2355);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 15, 47);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2316, 2436);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 37, 49);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 25;
            expectedUnitRunData.ProjectClone = 17;
            expectedUnitRunData.ProjectGen = 67;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 8
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2437, 2597);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 23, 26);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10493;
            expectedUnitRunData.ProjectRun = 3;
            expectedUnitRunData.ProjectClone = 24;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 9
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2598, 2860);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 23, 32);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 18;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 86;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 10
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2821, 3125);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 21, 22);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 5;
            expectedUnitRunData.ProjectGen = 18;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 11
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3085, 3329);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 20, 13);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 243;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 12
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3292, 3384);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 57, 19);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 38;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 88;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 13
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3385, 3752);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 57, 25);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 57;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 14
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3712, 4035);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 59, 8);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 15
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3997, 4117);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 22, 23);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11410;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 16
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4118, 4522);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 22, 29);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 17
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4482, 4910);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 24, 30);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 15;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 18
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4870, 5183);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 26, 12);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 9;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 19
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5143, 5455);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 24, 55);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 36;
            expectedUnitRunData.ProjectClone = 7;
            expectedUnitRunData.ProjectGen = 79;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 20
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5415, 5565);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 51, 20);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 8;
            expectedUnitRunData.ProjectClone = 10;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 21
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5562, 5771);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 3, 23);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 22
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 5772, 6113);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 3, 29);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 22;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 23
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6075, 6551);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 5, 11);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11410;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 25;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 24
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6511, 6937);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 1, 34);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11411;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 34;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 25
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6897, 6986);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 26, 40);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 22;
            expectedUnitRunData.ProjectClone = 11;
            expectedUnitRunData.ProjectGen = 65;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 26
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6987, 7417);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 26, 45);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 10;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 27
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7376, 7767);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 28, 31);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 45;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 153;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 28
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 7727, 8058);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 29, 45);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 256;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 29
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8021, 8468);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 31, 15);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11410;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 30
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8430, 8512);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 27, 24);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 12;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 13;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 24;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 31
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8506, 8827);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 50, 30);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 12;
            expectedUnitRunData.ProjectClone = 44;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 32
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8786, 9081);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 55, 33);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 31;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 33
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9041, 9365);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 7, 39);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 9;
            expectedUnitRunData.ProjectClone = 28;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 34
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9327, 9690);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 19, 56);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10494;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 7;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 35
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9650, 9928);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 18, 18);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 43;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 36
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9893, 10116);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 8, 13);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 3;
            expectedUnitRunData.ProjectClone = 40;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 37
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10079, 10384);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 57, 46);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 25;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 77;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 38
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10346, 10767);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 55, 31);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 12;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 75;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 39
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 10727, 10947);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 41, 55);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9152;
            expectedUnitRunData.ProjectRun = 15;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 40
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10909, 11465);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 8, 10);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9212;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 50;
            expectedUnitRunData.ProjectGen = 38;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 41
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11424, 11639);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 14, 3);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 119;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 114;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 42
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11603, 11712);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 9, 36);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 79;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 43
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11705, 12288);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 9, 51);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10494;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 31;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 44
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12245, 12511);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 8, 21);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 11;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 45
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12473, 12894);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 28, 57);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 44;
            expectedUnitRunData.ProjectGen = 6;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 46
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12895, 13205);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 18, 36);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 33;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 47
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13173, 13488);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 18, 45);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 29;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 48
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 13450, 13747);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 53, 8);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 68;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 49
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13688, 14009);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 28, 21);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 30;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 50
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13970, 14320);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 20, 43);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 10;
            expectedUnitRunData.ProjectClone = 33;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 51
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 14279, 14810);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 16, 32);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 74;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 52
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14772, 14951);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 8, 36);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 22;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 53
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 14913, 15092);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 9, 45);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 24;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 54
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15053, 15248);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 10, 50);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 55
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15193, 15370);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 11, 54);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 24;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 56
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15332, 15525);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 12, 47);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 22;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 57
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15472, 15650);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 13, 55);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 24;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 58
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15612, 15789);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 14, 45);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 59
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15761, 15927);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 15, 58);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 60
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15901, 16067);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 17, 14);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 61
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16041, 16209);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 18, 33);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 62
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16171, 16349);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 19, 28);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 22;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 63
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16310, 16502);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 20, 35);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 64
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16449, 16643);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 21, 42);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 65
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16589, 16764);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 22, 34);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 66
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16738, 16906);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 23, 31);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 13;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 67
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16868, 17044);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 24, 39);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 68
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17018, 17185);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 25, 55);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 69
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17158, 17326);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 27, 7);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 70
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17298, 17465);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 28, 25);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 71
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17439, 17625);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 29, 33);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 72
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17569, 17764);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 30, 30);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 73
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17737, 17923);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 31, 45);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 74
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17867, 18043);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 32, 58);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 14;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 75
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18017, 18186);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 33, 55);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 76
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18147, 18323);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 34, 47);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 77
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18296, 18465);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 35, 59);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 78
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18426, 18605);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 37, 13);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 79
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18567, 18762);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 38, 23);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 80
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18707, 18885);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 39, 31);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 15;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 81
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18846, 18995);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 40, 25);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 82
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18996, 19178);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 41, 38);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 15;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 83
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19123, 19318);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 42, 47);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 13;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 84
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19262, 19440);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 43, 38);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 85
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19402, 19581);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 44, 32);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 86
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19542, 19721);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 45, 45);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 87
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19681, 19857);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 46, 47);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 5;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 88
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19831, 20000);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 48, 0);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 10;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 89
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19961, 20140);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 49, 5);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 16;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 90
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20101, 20279);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 50, 5);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 91
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20240, 20419);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 51, 10);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 16;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 92
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20380, 20574);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 52, 16);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 6;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 93
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20520, 20714);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 53, 23);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 94
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20659, 20837);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 54, 15);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 13;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 95
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20799, 20977);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 55, 9);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 96
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20939, 21133);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 56, 21);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 16;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 97
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21078, 21257);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 57, 32);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 98
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21218, 21396);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 58, 24);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 5;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 99
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21358, 21566);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 59, 26);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 100
            expectedUnitRun = new UnitRun(expectedSlotRun, 4, 21512, 21723);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 0, 40);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 10;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 101
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21696, 22021);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 7, 1);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 9;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 102
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21963, 22162);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 8, 6);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 16;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 103
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22139, 22429);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 14, 28);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 104
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 22417, 22715);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 20, 54);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 12;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 105
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22676, 22847);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 22, 0);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11501;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 10;
            expectedUnitRunData.ProjectGen = 3;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 1 - UnitRun 106
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22888, 22954);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 0;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 1
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 95;
            expectedSlotRunData.FailedUnits = 11;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup SlotRun 2
            expectedSlotRun = new SlotRun(expectedRun, 2);
            expectedRun.SlotRuns.Add(2, expectedSlotRun);

            // Setup SlotRun 2 - UnitRun 0
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 113, 296);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 50, 34);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10496;
            expectedUnitRunData.ProjectRun = 83;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 7;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 293, 599);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 50, 39);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9704;
            expectedUnitRunData.ProjectRun = 15;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 125;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 558, 827);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 15, 50);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9151;
            expectedUnitRunData.ProjectRun = 10;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 791, 1097);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 47, 24);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 350;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1060, 1405);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 47, 26);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9704;
            expectedUnitRunData.ProjectRun = 8;
            expectedUnitRunData.ProjectClone = 7;
            expectedUnitRunData.ProjectGen = 247;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1367, 1690);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 12, 22);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9704;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 12;
            expectedUnitRunData.ProjectGen = 205;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1650, 1976);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 37, 54);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 215;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 3;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1939, 2294);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 37, 41);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 8
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2256, 2744);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 36, 45);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10493;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 24;
            expectedUnitRunData.ProjectGen = 18;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 9
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2703, 3057);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 33, 58);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 61;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 10
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3017, 3478);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 36, 7);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 27;
            expectedUnitRunData.ProjectGen = 15;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 11
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3438, 3700);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 45, 57);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 11;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 12
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3662, 3983);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 42, 51);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 26;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 13
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3945, 4281);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 44, 39);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 28;
            expectedUnitRunData.ProjectClone = 18;
            expectedUnitRunData.ProjectGen = 61;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 14
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4241, 4392);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 54, 52);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 11;
            expectedUnitRunData.ProjectClone = 5;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 15
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4393, 4700);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 54, 56);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 14;
            expectedUnitRunData.ProjectGen = 80;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 16
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4626, 4671);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 51, 53);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 25;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 68;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 17
            expectedUnitRun = new UnitRun(expectedSlotRun, 3, 4672, 4769);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 12, 29);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10493;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 18
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4770, 5094);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 12, 35);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 5;
            expectedUnitRunData.ProjectClone = 38;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 19
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5053, 5370);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 21, 56);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 158;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 26;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 20
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 5333, 5720);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 21, 54);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 47;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 21
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5680, 5987);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 31, 23);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 16;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 67;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 22
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5947, 6252);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 27, 56);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11402;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 45;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 23
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6212, 6434);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 29, 44);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 256;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 9;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 24
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6397, 6659);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 29, 19);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 17;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 18;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 25
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6621, 6736);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 4, 32);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 16;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 25;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 26
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6737, 7077);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 4, 40);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 350;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 17;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 27
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7040, 7266);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 4, 15);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 49;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 28
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7226, 7522);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 50, 30);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 92;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 125;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 29
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 7486, 7673);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 50, 15);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 81;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 18;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 30
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7674, 7949);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 2, 44);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 9;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 60;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 31
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7909, 8223);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 48, 48);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 43;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 32
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8183, 8409);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 58, 24);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10495;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 80;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 33
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8369, 8700);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 54, 32);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 177;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 17;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 34
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8663, 9029);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 53, 52);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10493;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 15;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 35
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8989, 9309);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 48, 23);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 30;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 36
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9269, 9571);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 47, 22);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 10;
            expectedUnitRunData.ProjectClone = 34;
            expectedUnitRunData.ProjectGen = 6;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 37
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9531, 9880);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 57, 14);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11400;
            expectedUnitRunData.ProjectRun = 7;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 13;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 38
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9835, 10279);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 54, 1);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 29;
            expectedUnitRunData.ProjectClone = 8;
            expectedUnitRunData.ProjectGen = 4;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 39
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 10241, 10570);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 52, 57);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 3;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 40
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10530, 11051);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 38, 34);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 13;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 41
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11015, 11215);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 37, 27);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 77;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 42
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11178, 11798);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 26, 51);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9212;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 33;
            expectedUnitRunData.ProjectGen = 77;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 43
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11760, 11973);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 32, 4);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 12;
            expectedUnitRunData.ProjectClone = 46;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 44
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11935, 12164);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 22, 8);
            expectedUnitRunData.CoreVersion = "0.0.4";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10490;
            expectedUnitRunData.ProjectRun = 182;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 14;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 45
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12127, 12221);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 21, 46);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 26;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 30;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 46
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12215, 12584);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 7, 3);
            expectedUnitRunData.CoreVersion = "0.0.14";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 31;
            expectedUnitRunData.ProjectClone = 19;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 47
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12546, 12986);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 2, 35);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 36;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 3;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 11;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 48
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12946, 13288);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 56, 30);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11401;
            expectedUnitRunData.ProjectRun = 26;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 49
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13251, 13580);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 42, 59);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 16;
            expectedUnitRunData.ProjectClone = 46;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 50
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13540, 13935);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 33, 19);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11403;
            expectedUnitRunData.ProjectRun = 15;
            expectedUnitRunData.ProjectClone = 21;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 51
            expectedUnitRun = new UnitRun(expectedSlotRun, 2, 13895, 14169);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 37, 12);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 10494;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 38;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.BAD_WORK_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 52
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14170, 14422);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 37, 23);
            expectedUnitRunData.CoreVersion = null;
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9201;
            expectedUnitRunData.ProjectRun = 19;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 0;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 53
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14404, 21577);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 26, 58);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 23;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 2 - UnitRun 54
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21504, 22974);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 27, 17);
            expectedUnitRunData.CoreVersion = "0.0.16";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 9852;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 27;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.INTERRUPTED;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 2
            expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 46;
            expectedSlotRunData.FailedUnits = 8;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            var expectedRunData = new FahClientClientRunData();
            expectedRunData.StartTime = new DateTime(2015, 12, 8, 12, 44, 41, DateTimeKind.Utc);
            expectedRun.Data = expectedRunData;

            var actualRun = fahLog.ClientRuns.ElementAt(0);
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_15_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_15\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 0
            var expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 72, 312);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 17, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 69;
            expectedUnitRunData.ProjectID = 11633;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 33;
            expectedUnitRunData.ProjectGen = 25;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 232, 467);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 35, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 867;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 113;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 411, 647);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 34, 47);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11636;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 69;
            expectedUnitRunData.ProjectGen = 31;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 568, 807);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 18, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10197;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 16;
            expectedUnitRunData.ProjectGen = 39;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 746, 971);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 54, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 779;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 981;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 908, 1150);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 13, 20);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11626;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 168;
            expectedUnitRunData.ProjectGen = 20;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1072, 1330);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 26, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11636;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 92;
            expectedUnitRunData.ProjectGen = 15;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1251, 1493);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 54, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9762;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 25;
            expectedUnitRunData.ProjectGen = 166;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 8
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1430, 1650);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 48, 23);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 23;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 73;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 9
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1594, 1803);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 38, 21);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6396;
            expectedUnitRunData.ProjectRun = 18;
            expectedUnitRunData.ProjectClone = 23;
            expectedUnitRunData.ProjectGen = 248;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 10
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1750, 1985);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 4, 20);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11626;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 42;
            expectedUnitRunData.ProjectGen = 25;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 11
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1906, 2137);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 18, 39);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6392;
            expectedUnitRunData.ProjectRun = 30;
            expectedUnitRunData.ProjectClone = 31;
            expectedUnitRunData.ProjectGen = 288;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 12
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2084, 2319);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 8, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11634;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 204;
            expectedUnitRunData.ProjectGen = 24;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 13
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2240, 2472);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 21, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6393;
            expectedUnitRunData.ProjectRun = 18;
            expectedUnitRunData.ProjectClone = 38;
            expectedUnitRunData.ProjectGen = 194;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 14
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2419, 2630);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 17, 23);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 78;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 96;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 15
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2574, 2787);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 3, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 592;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 125;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 16
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2731, 2968);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 46, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11641;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 317;
            expectedUnitRunData.ProjectGen = 8;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 17
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2888, 3120);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 13, 7);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6398;
            expectedUnitRunData.ProjectRun = 65;
            expectedUnitRunData.ProjectClone = 27;
            expectedUnitRunData.ProjectGen = 285;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 18
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3067, 3283);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 39, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 10197;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 32;
            expectedUnitRunData.ProjectGen = 30;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 19
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3223, 3439);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 16, 40);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 60;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 34;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 20
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3383, 3592);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 5, 18);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6393;
            expectedUnitRunData.ProjectRun = 25;
            expectedUnitRunData.ProjectClone = 28;
            expectedUnitRunData.ProjectGen = 150;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 21
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3539, 3751);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 32, 2);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 26;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 98;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 22
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3695, 3907);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 17, 10);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 79;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 55;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 23
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3851, 4070);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 36, 51);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9761;
            expectedUnitRunData.ProjectRun = 86;
            expectedUnitRunData.ProjectClone = 26;
            expectedUnitRunData.ProjectGen = 88;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 24
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4007, 4224);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 6, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6391;
            expectedUnitRunData.ProjectRun = 111;
            expectedUnitRunData.ProjectClone = 34;
            expectedUnitRunData.ProjectGen = 82;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 25
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4171, 4383);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 49, 57);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 205;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 79;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 26
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4326, 4538);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 36, 5);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6395;
            expectedUnitRunData.ProjectRun = 28;
            expectedUnitRunData.ProjectClone = 10;
            expectedUnitRunData.ProjectGen = 135;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 27
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4484, 4703);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 1, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 414;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 1025;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 28
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4640, 4860);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 18, 59);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 102;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 29
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4803, 5024);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 3, 51);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 514;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 998;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 30
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4961, 5180);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 20, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 683;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 69;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 31
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5124, 5336);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 7, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 538;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 71;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 32
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5280, 5490);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 53, 50);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 8600;
            expectedUnitRunData.ProjectRun = 121;
            expectedUnitRunData.ProjectClone = 13;
            expectedUnitRunData.ProjectGen = 312;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 33
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5437, 5648);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 19, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 805;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 91;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 34
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5592, 5829);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 30, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11635;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 664;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 35
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5749, 5993);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 23, 22);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 2534;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 701;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 36
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5929, 6156);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 40, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 1080;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 934;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 37
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6093, 6312);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 57, 31);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 246;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 38
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6256, 6493);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 40, 34);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11641;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 588;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 39
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6413, 6648);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 6, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 384;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 1;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 40
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6592, 6827);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 54, 44);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11625;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 896;
            expectedUnitRunData.ProjectGen = 2;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 41
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6749, 7008);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 40, 34);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11641;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 608;
            expectedUnitRunData.ProjectGen = 5;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 42
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6928, 7171);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 54, 52);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9752;
            expectedUnitRunData.ProjectRun = 424;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 975;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 43
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7107, 7324);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 10, 54);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6393;
            expectedUnitRunData.ProjectRun = 124;
            expectedUnitRunData.ProjectClone = 20;
            expectedUnitRunData.ProjectGen = 115;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 44
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7271, 7483);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 36, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 184;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 131;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 45
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7427, 7636);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 35, 3);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6392;
            expectedUnitRunData.ProjectRun = 20;
            expectedUnitRunData.ProjectClone = 26;
            expectedUnitRunData.ProjectGen = 172;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 46
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7583, 7795);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 2, 22);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 241;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 80;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 47
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7739, 7976);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 49, 25);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11636;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 89;
            expectedUnitRunData.ProjectGen = 27;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 48
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7896, 8128);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 30, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6394;
            expectedUnitRunData.ProjectRun = 58;
            expectedUnitRunData.ProjectClone = 26;
            expectedUnitRunData.ProjectGen = 222;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 49
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8075, 8283);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 56, 54);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 6398;
            expectedUnitRunData.ProjectRun = 2;
            expectedUnitRunData.ProjectClone = 20;
            expectedUnitRunData.ProjectGen = 319;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 50
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8230, 8444);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 22, 57);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11643;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 9;
            expectedUnitRunData.ProjectGen = 21;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 51
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8386, 8601);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 12, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 112;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 80;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 52
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8545, 8816);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 25, 11);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 40;
            expectedUnitRunData.ProjectID = 11626;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 886;
            expectedUnitRunData.ProjectGen = 6;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 53
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8736, 8997);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 57, 0);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 11642;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 95;
            expectedUnitRunData.ProjectGen = 26;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 54
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8917, 9016);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 30, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 21;
            expectedUnitRunData.ProjectID = 10197;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 11;
            expectedUnitRunData.ProjectGen = 32;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 54;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            var expectedRunData = new FahClientClientRunData();
            expectedRunData.StartTime = new DateTime(2016, 3, 5, 5, 0, 47, DateTimeKind.Utc);
            expectedRun.Data = expectedRunData;

            var actualRun = fahLog.ClientRuns.ElementAt(0);
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(2, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));

        }

        [Test]
        public void FahClientLog_Read_Client_v7_16_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 0
            var expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 73, 192);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 51, 3);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 39;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 587;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 266;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 142, 347);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 35, 32);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 17;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 245;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 294, 502);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 14, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 233;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 160;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 3
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 449, 660);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 52, 41);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 61;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 258;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 4
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 604, 814);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 35, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 144;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 226;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 5
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 761, 969);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 11, 44);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 723;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 240;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 6
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 916, 1121);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 55, 6);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 693;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 232;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 7
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1071, 1277);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 34, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 875;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 209;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 8
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1224, 1432);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 13, 4);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 479;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 233;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 9
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1379, 1584);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 54, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 802;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 155;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 10
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1534, 1739);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 33, 13);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 346;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 78;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 11
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1686, 1895);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 12, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 424;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 150;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 12
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1842, 2052);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 49, 25);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 172;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 205;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 13
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1997, 2205);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 28, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 59;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 130;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 14
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2152, 2360);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 8, 17);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 60;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 220;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 15
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2307, 2516);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 50, 44);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 606;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 253;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 16
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2463, 2671);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 31, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 559;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 225;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 17
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2618, 2826);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 13, 7);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 633;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 184;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 18
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2773, 2982);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 51, 23);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 159;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 360;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 19
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2929, 3137);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 34, 5);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 796;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 89;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 20
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3084, 3292);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 16, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 519;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 254;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 21
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3239, 3447);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 59, 13);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 615;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 227;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 22
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3394, 3603);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 39, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 457;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 305;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 23
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3550, 3758);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 18, 26);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 229;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 222;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 24
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3705, 3913);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 58, 2);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 476;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 124;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 25
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3860, 4069);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 38, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 671;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 226;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 26
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4016, 4224);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 19, 20);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 166;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 109;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 27
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4171, 4379);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 59, 52);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 716;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 64;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 28
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4326, 4534);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 45, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 800;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 160;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 29
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4481, 4690);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 24, 41);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 720;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 281;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 30
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4637, 4849);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 6, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 844;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 239;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 31
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4792, 5004);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 47, 55);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 401;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 135;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 32
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4949, 5158);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 31, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 250;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 65;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 33
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5105, 5313);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 12, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 904;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 71;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 34
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5260, 5466);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 54, 5);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 900;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 226;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 35
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5415, 5623);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 37, 31);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 204;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 74;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 36
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5568, 5777);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 20, 8);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 219;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 58;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 37
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5724, 5929);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 4, 21);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 445;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 252;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 38
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5879, 6084);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 48, 12);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 701;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 120;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 39
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6031, 6238);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 31, 15);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 608;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 88;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 40
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6187, 6392);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 10, 51);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 286;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 159;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 41
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6339, 6547);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 53, 57);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 710;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 243;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 42
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6494, 6699);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 35, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 533;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 218;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 43
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6649, 6855);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 15, 41);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 264;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 58;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 44
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6802, 7010);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 58, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 339;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 87;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 45
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6957, 7167);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 38, 25);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 259;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 89;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 46
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7112, 7321);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 22, 12);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 786;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 138;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 47
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7268, 7476);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 0, 59);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 835;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 183;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 48
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7423, 7631);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 43, 11);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 516;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 231;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 49
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7578, 7787);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 24, 53);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 802;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 93;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 50
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7733, 7941);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 7, 0);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 758;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 285;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 51
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7889, 8094);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 48, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 664;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 181;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 52
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8041, 8249);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 31, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 69;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 98;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 53
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8196, 8405);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 13, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 54;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 157;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 54
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8351, 8560);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 56, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 629;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 60;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 55
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8507, 8716);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 35, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 808;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 75;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 56
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8662, 8870);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 15, 53);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 28;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 42;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 57
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8817, 9026);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 58, 5);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 875;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 63;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 58
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8973, 9178);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 40, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 558;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 56;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 59
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9128, 9333);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 23, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 74;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 145;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 60
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9280, 9488);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 6, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 603;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 55;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 61
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9435, 9644);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 48, 59);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 742;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 147;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 62
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9591, 9801);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 28, 4);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 96;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 267;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 63
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9746, 9956);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 9, 27);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 376;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 175;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 64
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9901, 10110);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 48, 44);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 747;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 246;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 65
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10057, 10267);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 29, 15);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 613;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 187;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 66
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10212, 10417);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 12, 27);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 641;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 155;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 67
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10367, 10572);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 56, 55);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 862;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 295;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 68
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10519, 10728);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 39, 22);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 447;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 136;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 69
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10675, 10883);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 18, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 129;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 284;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 70
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10830, 11039);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 58, 50);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 673;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 178;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 71
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10985, 11194);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 37, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 178;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 194;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 72
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11141, 11346);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 19, 39);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 340;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 184;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 73
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11296, 11501);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 1, 20);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 774;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 266;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 74
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11448, 11656);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 43, 8);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 663;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 296;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 75
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11603, 11814);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 23, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 108;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 176;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 76
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11759, 11969);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 7, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 678;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 204;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 77
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11914, 12119);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 48, 52);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 552;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 216;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 78
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12069, 12275);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 30, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 710;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 275;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 79
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12222, 12430);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 9, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 350;
            expectedUnitRunData.ProjectClone = 6;
            expectedUnitRunData.ProjectGen = 110;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 80
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12377, 12585);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 52, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 287;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 200;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 81
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12532, 12742);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 32, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 95;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 239;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 82
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12687, 12896);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 18, 51);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 895;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 148;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 83
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12843, 13051);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 1, 3);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 556;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 196;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 84
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12998, 13206);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 42, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 881;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 171;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 85
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13153, 13362);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 23, 42);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 851;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 119;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 86
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13309, 13516);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 5, 44);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 324;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 173;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 87
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13464, 13671);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 44, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 70;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 75;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 88
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13616, 13825);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 22, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 687;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 88;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 89
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13772, 13981);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 6, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 395;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 227;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 90
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13928, 14134);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 47, 27);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 316;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 141;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 91
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14083, 14286);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 28, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 70;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 178;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 92
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14236, 14441);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 7, 50);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 582;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 275;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 93
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14388, 14597);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 52, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 909;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 113;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 94
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14544, 14749);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 31, 20);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 303;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 91;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 95
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14699, 14904);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 12, 12);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 921;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 274;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 96
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14851, 15060);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 54, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 863;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 317;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 97
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15007, 15212);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 35, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 150;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 238;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 98
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15162, 15368);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 20, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 82;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 256;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 99
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15314, 15519);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 59, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 590;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 204;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 100
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15469, 15675);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 44, 26);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 820;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 251;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 101
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15622, 15832);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 28, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 194;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 306;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 102
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15777, 15987);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 12, 51);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 112;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 199;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 103
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15932, 16138);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 51, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 1;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 211;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 104
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16088, 16290);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 30, 59);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 792;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 149;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 105
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16240, 16445);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 12, 25);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 79;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 295;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 106
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16392, 16600);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 56, 52);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 169;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 172;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 107
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16547, 16756);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 40, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 551;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 244;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 108
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16703, 16913);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 23, 6);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 804;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 327;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 109
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16858, 17066);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 3, 12);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 111;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 178;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 110
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17013, 17222);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 46, 23);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 406;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 221;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 111
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17169, 17374);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 27, 15);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 453;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 207;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 112
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17324, 17529);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 10, 7);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 533;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 208;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 113
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17476, 17686);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 56, 15);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 784;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 279;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 114
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17631, 17842);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 36, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 193;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 273;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 115
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17787, 17995);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 17, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 637;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 296;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 116
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17942, 18145);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 2, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 185;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 312;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 117
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18097, 18305);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 47, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 877;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 262;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 118
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18248, 18463);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 27, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 761;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 173;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 119
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18407, 18623);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 9, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 144;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 239;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 120
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18563, 18780);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 48, 3);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 529;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 167;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 121
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18723, 18939);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 30, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 84;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 331;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 122
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18883, 19097);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 9, 42);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 494;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 188;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 123
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19041, 19258);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 51, 8);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 199;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 364;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 124
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19199, 19416);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 31, 30);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 256;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 292;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 125
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19359, 19575);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 13, 27);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 150;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 279;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 126
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19518, 19736);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 55, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 479;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 291;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 127
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19677, 19895);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 40, 32);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 56;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 306;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 128
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19836, 20056);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 20, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 189;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 233;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 129
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19996, 20215);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 1, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 161;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 130
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20156, 20374);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 42, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 869;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 312;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 131
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20315, 20536);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 26, 55);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 154;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 293;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 132
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20474, 20690);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 6, 56);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 231;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 280;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 133
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20637, 20825);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 48, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 413;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 274;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 134
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20792, 21006);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 29, 40);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 181;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 310;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 135
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20953, 21162);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 15, 13);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 528;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 274;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 136
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21109, 21317);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 54, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 595;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 262;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 137
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21264, 21472);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 34, 40);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 86;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 246;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 138
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21419, 21627);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 17, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 81;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 217;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 139
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21574, 21780);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 57, 29);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 430;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 263;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 140
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21730, 21935);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 36, 11);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 869;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 319;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 141
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21882, 22090);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 17, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 159;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 237;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 142
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22037, 22246);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 59, 30);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 883;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 176;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 143
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22193, 22401);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 43, 23);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 481;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 216;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 144
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22348, 22557);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 25, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 31;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 307;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 145
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22503, 22712);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 7, 56);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 158;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 266;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 146
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22659, 22870);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 48, 2);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 563;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 314;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 147
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22815, 23022);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 29, 14);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 333;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 299;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 148
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22970, 23177);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 9, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 189;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 242;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 149
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23122, 23328);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 50, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 86;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 274;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 150
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23278, 23483);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 34, 10);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 49;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 298;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 151
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23430, 23638);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 17, 22);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 38;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 313;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 152
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23585, 23793);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 57, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 733;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 151;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 153
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23740, 23949);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 42, 6);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 179;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 197;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 154
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23896, 24104);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 25, 33);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 66;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 328;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 155
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24051, 24259);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 9, 26);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 45;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 262;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 156
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24206, 24415);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 51, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 793;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 298;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 157
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24362, 24572);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 33, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 72;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 267;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 158
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24517, 24725);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 11, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 497;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 274;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 159
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24672, 24882);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 54, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 234;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 218;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 160
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24827, 25033);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 37, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 512;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 249;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 161
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24983, 25185);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 22, 17);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 865;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 338;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 162
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25135, 25340);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 6, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 321;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 241;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 163
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25287, 25496);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 54, 38);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 738;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 281;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 164
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25443, 25651);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 40, 36);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 803;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 188;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 165
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25598, 25803);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 22, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 553;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 300;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 166
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25753, 25958);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 3, 15);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 135;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 306;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 167
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25905, 26111);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 48, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 149;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 322;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 168
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26061, 26268);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 30, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 605;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 238;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 169
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26213, 26421);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 15, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 38;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 178;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 170
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26368, 26577);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 55, 27);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 521;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 285;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 171
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26524, 26732);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 36, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 86;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 154;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 172
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26679, 26884);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 18, 55);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 274;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 292;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 173
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26834, 27039);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 1, 57);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 36;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 248;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 174
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26986, 27199);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 44, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 867;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 250;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 175
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27142, 27349);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 29, 6);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 698;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 322;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 176
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27299, 27506);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 8, 22);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 665;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 278;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 177
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27451, 27660);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 48, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 875;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 176;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 178
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27607, 27812);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 30, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 514;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 250;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 179
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27762, 27964);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(6, 14, 32);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 329;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 331;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 180
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27914, 28122);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 54, 39);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 382;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 312;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 181
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28066, 28276);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 35, 41);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 4;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 328;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 182
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28223, 28431);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 16, 7);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 321;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 192;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 183
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28378, 28588);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 57, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 601;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 283;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 184
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28533, 28741);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 35, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 19;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 245;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 185
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28688, 28897);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 15, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 790;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 283;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 186
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28844, 29052);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 54, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 296;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 250;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 187
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28999, 29209);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 33, 8);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 342;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 289;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 188
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29154, 29363);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 13, 14);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 65;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 242;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 189
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29310, 29517);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 54, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 910;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 255;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 190
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29465, 29670);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 36, 18);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 514;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 256;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 191
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29617, 29823);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(2, 16, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 105;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 325;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 192
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29772, 29979);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 58, 35);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 109;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 281;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 193
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29926, 30134);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 38, 12);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 115;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 135;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 194
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30081, 30286);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 20, 4);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 86;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 273;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 195
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30236, 30442);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(9, 2, 6);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 296;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 343;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 196
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30389, 30597);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 40, 28);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 632;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 208;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 197
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30544, 30752);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 19, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 759;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 272;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 198
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30699, 30907);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 58, 16);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 79;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 232;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 199
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30854, 31065);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 38, 52);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 599;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 305;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 200
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31010, 31222);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 19, 58);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 189;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 273;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 201
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31167, 31372);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 3, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 116;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 301;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 202
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31322, 31528);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 41, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 656;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 342;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 203
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31475, 31680);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 24, 55);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 381;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 156;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 204
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31630, 31838);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 7, 1);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9036;
            expectedUnitRunData.ProjectRun = 228;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 275;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 205
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31782, 31991);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 52, 14);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9033;
            expectedUnitRunData.ProjectRun = 118;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 310;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 206
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31938, 32147);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(3, 37, 26);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 129;
            expectedUnitRunData.ProjectClone = 3;
            expectedUnitRunData.ProjectGen = 292;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 207
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32094, 32302);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 18, 42);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 783;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 297;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 208
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32249, 32457);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(7, 2, 9);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 771;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 279;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 209
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32404, 32613);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 41, 46);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 767;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 363;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 210
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32560, 32768);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(10, 24, 57);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9032;
            expectedUnitRunData.ProjectRun = 555;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 235;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 211
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32715, 32920);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(12, 3, 43);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9040;
            expectedUnitRunData.ProjectRun = 13;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 240;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 212
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32870, 33076);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 44, 45);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 508;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 314;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 213
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33022, 33233);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 27, 37);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 564;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 305;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 214
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33178, 33386);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(17, 8, 19);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 68;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 273;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 215
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33333, 33541);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(18, 49, 21);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9031;
            expectedUnitRunData.ProjectRun = 459;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 250;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 216
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33488, 33696);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(20, 29, 48);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9039;
            expectedUnitRunData.ProjectRun = 864;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 289;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 217
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33643, 33852);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(22, 8, 49);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9037;
            expectedUnitRunData.ProjectRun = 28;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 277;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 218
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33799, 34007);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(23, 49, 26);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9035;
            expectedUnitRunData.ProjectRun = 6;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 238;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 219
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33954, 34159);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 31, 3);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 9034;
            expectedUnitRunData.ProjectRun = 739;
            expectedUnitRunData.ProjectClone = 0;
            expectedUnitRunData.ProjectGen = 272;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 220
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 34109, 34345);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(4, 41, 24);
            expectedUnitRunData.CoreVersion = "2.27";
            expectedUnitRunData.FramesObserved = 5;
            expectedUnitRunData.ProjectID = 9038;
            expectedUnitRunData.ProjectRun = 353;
            expectedUnitRunData.ProjectClone = 1;
            expectedUnitRunData.ProjectGen = 238;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 220;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            var expectedRunData = new FahClientClientRunData();
            expectedRunData.StartTime = new DateTime(2016, 6, 18, 15, 50, 21, DateTimeKind.Utc);
            expectedRun.Data = expectedRunData;

            var actualRun = fahLog.ClientRuns.ElementAt(0);
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(2, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_17_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_17\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 0
            var expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 0, 89, 366);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(5, 47, 36);
            expectedUnitRunData.CoreVersion = "0.0.11";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13127;
            expectedUnitRunData.ProjectRun = 53;
            expectedUnitRunData.ProjectClone = 4;
            expectedUnitRunData.ProjectGen = 59;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 292, 540);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(8, 45, 15);
            expectedUnitRunData.CoreVersion = "0.0.11";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 8673;
            expectedUnitRunData.ProjectRun = 21;
            expectedUnitRunData.ProjectClone = 2;
            expectedUnitRunData.ProjectGen = 61;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 2
            expectedUnitRun = new UnitRun(expectedSlotRun, 0, 472, 662);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 26, 21);
            expectedUnitRunData.CoreVersion = "0.0.11";
            expectedUnitRunData.FramesObserved = 101;
            expectedUnitRunData.ProjectID = 13801;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 2696;
            expectedUnitRunData.ProjectGen = 19;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 3;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            expectedRun.Data = new FahClientClientRunData();
            expectedRun.Data.StartTime = new DateTime(2017, 3, 13, 5, 47, 4, DateTimeKind.Utc);

            var actualRun = fahLog.ClientRuns.Last();
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_18_Test()
        {
            // Scan
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_18\\log.txt");

            // Setup ClientRun 0
            var expectedRun = new ClientRun(null, 0);

            // Setup SlotRun 0
            var expectedSlotRun = new SlotRun(expectedRun, 0);
            expectedRun.SlotRuns.Add(0, expectedSlotRun);

            // Setup SlotRun 0 - UnitRun 0
            var expectedUnitRun = new UnitRun(expectedSlotRun, 0, 73, 276);
            var expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 50, 5);
            expectedUnitRunData.CoreVersion = "0.0.18";
            expectedUnitRunData.FramesObserved = 0;
            expectedUnitRunData.ProjectID = 14402;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 4867;
            expectedUnitRunData.ProjectGen = 36;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.UNKNOWN_ENUM;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
            expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRun 0 - UnitRun 1
            expectedUnitRun = new UnitRun(expectedSlotRun, 1, 206, 287);
            expectedUnitRunData = new FahClientUnitRunData();
            expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 50, 12);
            expectedUnitRunData.CoreVersion = "0.0.18";
            expectedUnitRunData.FramesObserved = 11;
            expectedUnitRunData.ProjectID = 14401;
            expectedUnitRunData.ProjectRun = 0;
            expectedUnitRunData.ProjectClone = 1699;
            expectedUnitRunData.ProjectGen = 47;
            expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
            frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
            var frameData = new WorkUnitFrameData();
            frameData.ID = 0;
            frameData.RawFramesComplete = 1;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(570160000000);
            frameData.Duration = TimeSpan.FromTicks(0);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 1;
            frameData.RawFramesComplete = 1250;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(571430000000);
            frameData.Duration = TimeSpan.FromTicks(1270000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 2;
            frameData.RawFramesComplete = 2500;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(572690000000);
            frameData.Duration = TimeSpan.FromTicks(1260000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 3;
            frameData.RawFramesComplete = 3750;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(573930000000);
            frameData.Duration = TimeSpan.FromTicks(1240000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 4;
            frameData.RawFramesComplete = 5000;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(575180000000);
            frameData.Duration = TimeSpan.FromTicks(1250000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 5;
            frameData.RawFramesComplete = 6250;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(576440000000);
            frameData.Duration = TimeSpan.FromTicks(1260000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 6;
            frameData.RawFramesComplete = 7500;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(577680000000);
            frameData.Duration = TimeSpan.FromTicks(1240000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 7;
            frameData.RawFramesComplete = 8750;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(578910000000);
            frameData.Duration = TimeSpan.FromTicks(1230000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 8;
            frameData.RawFramesComplete = 10000;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(580160000000);
            frameData.Duration = TimeSpan.FromTicks(1250000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 9;
            frameData.RawFramesComplete = 11250;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(581400000000);
            frameData.Duration = TimeSpan.FromTicks(1240000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            frameData = new WorkUnitFrameData();
            frameData.ID = 10;
            frameData.RawFramesComplete = 12500;
            frameData.RawFramesTotal = 125000;
            frameData.TimeStamp = TimeSpan.FromTicks(582650000000);
            frameData.Duration = TimeSpan.FromTicks(1250000000);
            frameDataDictionary.Add(frameData.ID, frameData);
            expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
            expectedUnitRun.Data = expectedUnitRunData;
            expectedSlotRun.UnitRuns.Add(expectedUnitRun);

            // Setup SlotRunData 0
            var expectedSlotRunData = new FahClientSlotRunData();
            expectedSlotRunData.CompletedUnits = 0;
            expectedSlotRunData.FailedUnits = 0;
            expectedSlotRun.Data = expectedSlotRunData;

            // Setup ClientRunData 0
            var expectedRunData = new FahClientClientRunData();
            expectedRunData.StartTime = new DateTime(2020, 2, 13, 1, 48, 13, DateTimeKind.Utc);
            expectedRun.Data = expectedRunData;

            var actualRun = fahLog.ClientRuns.ElementAt(0);
            AssertClientRun.AreEqual(expectedRun, actualRun, true);

            Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
            Assert.IsTrue(LogLineEnumerable.Create(actualRun.SlotRuns[0].UnitRuns[0]).Any(x => x.LineType == LogLineType.WorkUnitTooManyErrors));
        }

        [Test]
        public void FahClientLog_Read_Client_v7_fr_FR_Test()
        {
            // Act
            var fahLog = FahClientLog.Read("..\\..\\..\\TestFiles\\Client_v7_fr-FR\\log.txt");
            // Assert
            Assert.AreEqual(1, fahLog.ClientRuns.Count);
            var clientRun = fahLog.ClientRuns.First();
            Assert.AreEqual(2, clientRun.SlotRuns.Count);
            var slotRun0 = clientRun.SlotRuns[0];
            Assert.AreEqual(1, slotRun0.UnitRuns.Count);
            var slotRun1 = clientRun.SlotRuns[1];
            Assert.AreEqual(1, slotRun1.UnitRuns.Count);
        }
    }

    // ReSharper restore InconsistentNaming
}
