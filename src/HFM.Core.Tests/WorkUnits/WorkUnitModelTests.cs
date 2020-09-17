using System;
using System.Collections.Generic;

using HFM.Core.Client;
using HFM.Log;
using HFM.Proteins;

using Moq;

using NUnit.Framework;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class WorkUnitModelTests
    {
        #region Assigned

        [Test]
        public void WorkUnitModel_AssignedTest1()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Assigned.ToLocalTime(), workUnitModel.Assigned);
        }

        [Test]
        public void WorkUnitModel_AssignedTest5()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.MinValue };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Assigned, workUnitModel.Assigned);
        }

        #endregion

        #region Timeout

        [Test]
        public void WorkUnitModel_TimeoutTest1()
        {
            var workUnit = new WorkUnit { Timeout = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Timeout.ToLocalTime(), workUnitModel.Timeout);
        }

        [Test]
        public void WorkUnitModel_TimeoutTest5()
        {
            var workUnit = new WorkUnit { Timeout = DateTime.MinValue };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Timeout, workUnitModel.Timeout);
        }

        #endregion

        #region Finished

        [Test]
        public void WorkUnitModel_FinishedTest1()
        {
            var workUnit = new WorkUnit { Finished = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Finished.ToLocalTime(), workUnitModel.Finished);
        }

        [Test]
        public void WorkUnitModel_FinishedTest5()
        {
            var workUnit = new WorkUnit { Finished = DateTime.MinValue };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Finished, workUnitModel.Finished);
        }

        #endregion

        #region PreferredDeadline

        [Test]
        public void WorkUnitModel_PreferredDeadlineTest1()
        {
            var protein = new Protein { ProjectNumber = 1, PreferredDays = 3 };
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(3).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void WorkUnitModel_PreferredDeadlineTest2()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow, Timeout = DateTime.UtcNow.Add(TimeSpan.FromDays(5)) };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);

            // PreferredDeadline comes from DueTime when ProteinIsUnknown
            Assert.AreEqual(workUnit.Assigned.AddDays(5).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void WorkUnitModel_PreferredDeadlineTest3()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.MinValue };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Assigned, workUnitModel.PreferredDeadline);
        }

        [Test]
        public void WorkUnitModel_PreferredDeadlineTest4()
        {
            // daylight savings time test (in DST => Standard Time)
            var protein = new Protein { ProjectNumber = 1, PreferredDays = 7 };
            var workUnit = new WorkUnit { Assigned = new DateTime(2011, 11, 1) };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(7).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void WorkUnitModel_PreferredDeadlineTest5()
        {
            // daylight savings time test (in Standard Time => DST)
            var protein = new Protein { ProjectNumber = 1, PreferredDays = 7 };
            var workUnit = new WorkUnit { Assigned = new DateTime(2011, 3, 9) };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(7).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        #endregion

        #region FinalDeadline

        [Test]
        public void WorkUnitModel_FinalDeadlineTest1()
        {
            var protein = new Protein { ProjectNumber = 1, MaximumDays = 6 };
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(6).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        [Test]
        public void WorkUnitModel_FinalDeadlineTest2()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(DateTime.MinValue, workUnitModel.FinalDeadline);
        }

        [Test]
        public void WorkUnitModel_FinalDeadlineTest3()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.MinValue };
            var workUnitModel = CreateWorkUnitModel(new Protein(), workUnit);
            Assert.AreEqual(workUnit.Assigned, workUnitModel.FinalDeadline);
        }

        [Test]
        public void WorkUnitModel_FinalDeadlineTest4()
        {
            // daylight savings time test (in DST => Standard Time)
            var protein = new Protein { ProjectNumber = 1, MaximumDays = 7 };
            var workUnit = new WorkUnit { Assigned = new DateTime(2011, 11, 1) };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(7).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        [Test]
        public void WorkUnitModel_FinalDeadlineTest5()
        {
            // daylight savings time test (in Standard Time => DST)
            var protein = new Protein { ProjectNumber = 1, MaximumDays = 7 };
            var workUnit = new WorkUnit { Assigned = new DateTime(2011, 3, 9) };
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);
            Assert.AreEqual(workUnit.Assigned.AddDays(7).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        #endregion

        #region CurrentProtein

        [Test]
        public void WorkUnitModel_CurrentProteinTest1()
        {
            var protein = new Protein { ProjectNumber = 2669 };
            var workUnitModel = CreateWorkUnitModel(protein, new WorkUnit());

            Assert.AreSame(protein, workUnitModel.CurrentProtein);
        }

        [Test]
        public void WorkUnitModel_CurrentProteinTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            Assert.IsTrue(WorkUnitModel.ProteinIsUnknown(workUnitModel.CurrentProtein));
        }

        #endregion

        #region Frame and Percent Complete

        [Test]
        public void WorkUnitModel_FramesCompleteTest1()
        {
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 1 });
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(1, workUnitModel.FramesComplete);
        }

        [Test]
        public void WorkUnitModel_FramesCompleteTest2()
        {
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = -1 });
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(0, workUnitModel.FramesComplete);
        }

        [Test]
        public void WorkUnitModel_FramesCompleteTest3()
        {
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 101 });
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(100, workUnitModel.FramesComplete);
        }

        [Test]
        public void WorkUnitModel_FramesCompleteTest4()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());

            Assert.AreEqual(0, workUnitModel.FramesComplete);
        }

        [Test]
        public void WorkUnitModel_PercentCompleteTest1()
        {
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 5 });
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(5, workUnitModel.PercentComplete);
        }

        [Test]
        public void WorkUnitModel_PercentCompleteTest2()
        {
            var protein = new Protein { Frames = 200 };
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { ID = 5 });
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(2, workUnitModel.PercentComplete);
        }

        #endregion

        #region PerSectionTests

        [Test]
        public void WorkUnitModel_PerUnitDownloadTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow, FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            workUnit.UnitRetrievalTime = workUnit.Assigned.ToLocalTime().Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(600, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(600), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(144, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerUnitDownloadTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerUnitDownloadTest3()
        {
            var workUnit = new WorkUnit { FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerUnitDownloadTest4()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow, FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, -1));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerAllSectionsTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(360, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(360), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
            Assert.AreEqual(240, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.AllFrames, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerAllSectionsTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.AllFrames, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerThreeSectionsTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(376, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(376), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(229.78723, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastThreeFrames, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerThreeSectionsTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastThreeFrames, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerLastSectionTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(380, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(380), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
            Assert.AreEqual(227.36842, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_PerLastSectionTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest1()
        {
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(380, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(380), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest2()
        {
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(376, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(376), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest3()
        {
            var workUnit = new WorkUnit { FramesObserved = 5 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(new TimeSpan(0, 5, 10), 1),
                     CreateFrame(new TimeSpan(0, 6, 20), 2),
                     CreateFrame(new TimeSpan(0, 6, 10), 3),
                     CreateFrame(new TimeSpan(0, 6, 20), 4));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(360, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(360), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest4()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow, FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            workUnit.UnitRetrievalTime = workUnit.Assigned.ToLocalTime().Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(600, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(600), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest5()
        {
            var mockBenchmarkService = new Mock<IProteinBenchmarkService>();
            var benchmark = new ProteinBenchmark { FrameTimes = { new ProteinBenchmarkFrameTime { Duration = TimeSpan.FromMinutes(10) } } };
            mockBenchmarkService.Setup(x => x.GetBenchmark(It.IsAny<SlotIdentifier>(), It.IsAny<ProteinBenchmarkIdentifier>())).Returns(benchmark);
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit(), mockBenchmarkService.Object);

            Assert.AreEqual(TimeSpan.FromMinutes(10), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        [Test]
        public void WorkUnitModel_TimePerSectionTest6()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            Assert.AreEqual(TimeSpan.Zero, workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        #endregion

        #region Credit, UPD, PPD

        [Test]
        public void WorkUnitModel_CreditUPDAndPPDTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6 };
            var utcNow = DateTime.UtcNow;
            var workUnit = new WorkUnit { Finished = utcNow, Assigned = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(848.528, workUnitModel.GetCredit(SlotStatus.RunningNoFrameTimes, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(2036.4672, workUnitModel.GetPPD(SlotStatus.RunningNoFrameTimes, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
        }

        [Test]
        public void WorkUnitModel_CreditUPDAndPPDTest2()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6 };
            var utcNow = DateTime.UtcNow;
            var workUnit = new WorkUnit { Finished = utcNow, Assigned = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(1897.367, workUnitModel.GetCredit(SlotStatus.Running, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(4553.6808, workUnitModel.GetPPD(SlotStatus.Running, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
        }

        [Test]
        public void WorkUnitModel_CreditUPDAndPPDTest3()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var workUnit = new WorkUnit { FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(protein, workUnit);

            Assert.AreEqual(100, workUnitModel.GetCredit(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(240, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void WorkUnitModel_CreditUPDAndPPDTest4()
        {
            var workUnit = new WorkUnit { FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(0, workUnitModel.GetCredit(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        #endregion

        #region ETA

        [Test]
        public void WorkUnitModel_EtaTest1()
        {
            var workUnit = new WorkUnit { FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(TimeSpan.FromMinutes(582), workUnitModel.GetEta(PPDCalculation.LastFrame));
        }

        [Test]
        public void WorkUnitModel_EtaTest2()
        {
            var workUnit = new WorkUnit { Assigned = DateTime.UtcNow, FramesObserved = 4 };
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(CreateFrame(TimeSpan.Zero, 0),
                     CreateFrame(TimeSpan.FromMinutes(4), 1),
                     CreateFrame(TimeSpan.FromMinutes(5), 2),
                     CreateFrame(TimeSpan.FromMinutes(6), 3));
            workUnit.Frames = frames;
            workUnit.UnitRetrievalTime = workUnit.Assigned.Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.AreEqual(workUnit.Assigned.Add(TimeSpan.FromMinutes(612)), workUnitModel.GetEtaDate(PPDCalculation.LastFrame));
        }

        #endregion

        #region AllFramesCompleted

        [Test]
        public void WorkUnitModel_AllFramesCompleted1()
        {
            var workUnit = new WorkUnit();
            var frames = new Dictionary<int, LogLineFrameData>().With(CreateFrame(TimeSpan.Zero, 100));
            workUnit.Frames = frames;
            var workUnitModel = CreateWorkUnitModel(null, workUnit);

            Assert.IsTrue(workUnitModel.AllFramesCompleted);
        }

        [Test]
        public void WorkUnitModel_AllFramesCompleted2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            Assert.IsFalse(workUnitModel.AllFramesCompleted);
        }

        #endregion

        #region Helpers

        private static WorkUnitModel CreateWorkUnitModel(Protein protein, WorkUnit workUnit, IProteinBenchmarkService benchmarkService = null)
        {
            var slotModel = new SlotModel(new NullClient(null, null, benchmarkService) { Settings = new ClientSettings() });
            return new WorkUnitModel(slotModel, workUnit)
            {
                CurrentProtein = protein ?? new Protein()
            };
        }

        private static LogLineFrameData CreateFrame(TimeSpan duration, int frameId)
        {
            return new LogLineFrameData { ID = frameId, Duration = duration };
        }

        #endregion
    }
}
