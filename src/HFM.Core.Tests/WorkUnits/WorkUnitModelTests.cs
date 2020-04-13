/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Client;
using HFM.Log;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class WorkUnitModelTests
    {
        private IProteinBenchmarkService _benchmarkService;

        [SetUp]
        public void Init()
        {
            _benchmarkService = MockRepository.GenerateStub<IProteinBenchmarkService>();
        }

        #region DownloadTime

        [Test]
        public void DownloadTimeTest1()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime(), workUnitModel.DownloadTime);
        }

        [Test]
        public void DownloadTimeTest2()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 60;
            Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(60)), workUnitModel.DownloadTime);
        }

        [Test]
        public void DownloadTimeTest3()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime, workUnitModel.DownloadTime);
        }

        [Test]
        public void DownloadTimeTest4()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = -60;
            Assert.AreEqual(unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(60)), workUnitModel.DownloadTime);
        }

        [Test]
        public void DownloadTimeTest5()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.MinValue};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime, workUnitModel.DownloadTime);
        }

        #endregion

        #region DueTime

        [Test]
        public void DueTimeTest1()
        {
            var unitInfo = new WorkUnit {DueTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DueTime.ToLocalTime(), workUnitModel.DueTime);
        }

        [Test]
        public void DueTimeTest2()
        {
            var unitInfo = new WorkUnit {DueTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 60;
            Assert.AreEqual(unitInfo.DueTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(60)), workUnitModel.DueTime);
        }

        [Test]
        public void DueTimeTest3()
        {
            var unitInfo = new WorkUnit {DueTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DueTime, workUnitModel.DueTime);
        }

        [Test]
        public void DueTimeTest4()
        {
            var unitInfo = new WorkUnit {DueTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = -60;
            Assert.AreEqual(unitInfo.DueTime.Add(TimeSpan.FromMinutes(60)), workUnitModel.DueTime);
        }

        [Test]
        public void DueTimeTest5()
        {
            var unitInfo = new WorkUnit {DueTime = DateTime.MinValue};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DueTime, workUnitModel.DueTime);
        }

        #endregion

        #region FinishedTime

        [Test]
        public void FinishedTimeTest1()
        {
            var unitInfo = new WorkUnit {FinishedTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime(), workUnitModel.FinishedTime);
        }

        [Test]
        public void FinishedTimeTest2()
        {
            var unitInfo = new WorkUnit {FinishedTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 60;
            Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(60)), workUnitModel.FinishedTime);
        }

        [Test]
        public void FinishedTimeTest3()
        {
            var unitInfo = new WorkUnit {FinishedTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.FinishedTime, workUnitModel.FinishedTime);
        }

        [Test]
        public void FinishedTimeTest4()
        {
            var unitInfo = new WorkUnit {FinishedTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = -60;
            Assert.AreEqual(unitInfo.FinishedTime.Add(TimeSpan.FromMinutes(60)), workUnitModel.FinishedTime);
        }

        [Test]
        public void FinishedTimeTest5()
        {
            var unitInfo = new WorkUnit {FinishedTime = DateTime.MinValue};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.FinishedTime, workUnitModel.FinishedTime);
        }

        #endregion

        #region PreferredDeadline

        [Test]
        public void PreferredDeadlineTest1()
        {
            var protein = new Protein {ProjectNumber = 1, PreferredDays = 3};
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(3).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void PreferredDeadlineTest2()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow, DueTime = DateTime.UtcNow.Add(TimeSpan.FromDays(5))};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            // PreferredDeadline comes from DueTime when Protein.IsUnknown
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(5).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void PreferredDeadlineTest3()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.MinValue};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime, workUnitModel.PreferredDeadline);
        }

        [Test]
        public void PreferredDeadlineTest4()
        {
            // daylight savings time test (in DST => Standard Time)
            var protein = new Protein {ProjectNumber = 1, PreferredDays = 7};
            var unitInfo = new WorkUnit {DownloadTime = new DateTime(2011, 11, 1)};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        [Test]
        public void PreferredDeadlineTest5()
        {
            // daylight savings time test (in Standard Time => DST)
            var protein = new Protein {ProjectNumber = 1, PreferredDays = 7};
            var unitInfo = new WorkUnit {DownloadTime = new DateTime(2011, 3, 9)};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(), workUnitModel.PreferredDeadline);
        }

        #endregion

        #region FinalDeadline

        [Test]
        public void FinalDeadlineTest1()
        {
            var protein = new Protein {ProjectNumber = 1, MaximumDays = 6};
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(6).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        [Test]
        public void FinalDeadlineTest2()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.UtcNow};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(DateTime.MinValue, workUnitModel.FinalDeadline);
        }

        [Test]
        public void FinalDeadlineTest3()
        {
            var unitInfo = new WorkUnit {DownloadTime = DateTime.MinValue};
            var workUnitModel = CreateWorkUnitModel(new Protein(), unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime, workUnitModel.FinalDeadline);
        }

        [Test]
        public void FinalDeadlineTest4()
        {
            // daylight savings time test (in DST => Standard Time)
            var protein = new Protein {ProjectNumber = 1, MaximumDays = 7};
            var unitInfo = new WorkUnit {DownloadTime = new DateTime(2011, 11, 1)};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        [Test]
        public void FinalDeadlineTest5()
        {
            // daylight savings time test (in Standard Time => DST)
            var protein = new Protein {ProjectNumber = 1, MaximumDays = 7};
            var unitInfo = new WorkUnit {DownloadTime = new DateTime(2011, 3, 9)};
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(), workUnitModel.FinalDeadline);
        }

        #endregion

        #region CurrentProtein

        [Test]
        public void CurrentProteinTest1()
        {
            var protein = new Protein { ProjectNumber = 2669 };
            var workUnitModel = CreateWorkUnitModel(protein, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreSame(protein, workUnitModel.CurrentProtein);
        }

        [Test]
        public void CurrentProteinTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            Assert.IsTrue(WorkUnitModel.ProteinIsUnknown(workUnitModel.CurrentProtein));
        }

        #endregion

        #region Frame and Percent Complete

        [Test]
        public void FramesCompleteTest1()
        {
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 1 });
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(1, workUnitModel.FramesComplete);
        }

        [Test]
        public void FramesCompleteTest2()
        {
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = -1 });
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.FramesComplete);
        }

        [Test]
        public void FramesCompleteTest3()
        {
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 101 });
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(100, workUnitModel.FramesComplete);
        }

        [Test]
        public void FramesCompleteTest4()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.FramesComplete);
        }

        [Test]
        public void PercentCompleteTest1()
        {
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 5 });
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(5, workUnitModel.PercentComplete);
        }

        [Test]
        public void PercentCompleteTest2()
        {
            var protein = new Protein { Frames = 200 };
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { ID = 5 });
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(2, workUnitModel.PercentComplete);
        }

        #endregion

        #region PerSectionTests

        [Test]
        public void PerUnitDownloadTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(600, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(600), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(144, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void PerUnitDownloadTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void PerUnitDownloadTest3()
        {
            var unitInfo = new WorkUnit { FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void PerUnitDownloadTest4()
        {
            var unitInfo = new WorkUnit { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, -1));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.EffectiveRate, BonusCalculation.None));
        }

        [Test]
        public void PerAllSectionsTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(360, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(360), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
            Assert.AreEqual(240, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.AllFrames, BonusCalculation.None));
        }

        [Test]
        public void PerAllSectionsTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.AllFrames, BonusCalculation.None));
        }

        [Test]
        public void PerThreeSectionsTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(376, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(376), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(229.78723, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastThreeFrames, BonusCalculation.None));
        }

        [Test]
        public void PerThreeSectionsTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastThreeFrames, BonusCalculation.None));
        }

        [Test]
        public void PerLastSectionTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(380, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(380), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
            Assert.AreEqual(227.36842, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void PerLastSectionTest2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(0), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void TimePerSectionTest1()
        {
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(380, workUnitModel.GetRawTime(PPDCalculation.LastFrame));
            Assert.AreEqual(TimeSpan.FromSeconds(380), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        [Test]
        public void TimePerSectionTest2()
        {
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(376, workUnitModel.GetRawTime(PPDCalculation.LastThreeFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(376), workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames));
        }

        [Test]
        public void TimePerSectionTest3()
        {
            var unitInfo = new WorkUnit { FramesObserved = 5 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(new TimeSpan(0, 5, 10), 1),
                     CreateFrameData(new TimeSpan(0, 6, 20), 2),
                     CreateFrameData(new TimeSpan(0, 6, 10), 3),
                     CreateFrameData(new TimeSpan(0, 6, 20), 4));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(360, workUnitModel.GetRawTime(PPDCalculation.AllFrames));
            Assert.AreEqual(TimeSpan.FromSeconds(360), workUnitModel.GetFrameTime(PPDCalculation.AllFrames));
        }

        [Test]
        public void TimePerSectionTest4()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = true;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(600, workUnitModel.GetRawTime(PPDCalculation.EffectiveRate));
            Assert.AreEqual(TimeSpan.FromSeconds(600), workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate));
        }

        [Test]
        public void TimePerSectionTest5()
        {
            var benchmark = new ProteinBenchmark { FrameTimes = { new ProteinBenchmarkFrameTime { Duration = TimeSpan.FromMinutes(10) } } };
            _benchmarkService.Stub(x => x.GetBenchmark(new SlotIdentifier(), 0)).IgnoreArguments().Return(benchmark);
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(TimeSpan.FromMinutes(10), workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        [Test]
        public void TimePerSectionTest6()
        {
            _benchmarkService = null;
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;
            Assert.AreEqual(TimeSpan.Zero, workUnitModel.GetFrameTime(PPDCalculation.LastFrame));
        }

        #endregion

        #region Credit, UPD, PPD

        [Test]
        public void CreditUPDAndPPDTest1()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6 };
            var utcNow = DateTime.UtcNow;
            var unitInfo = new WorkUnit { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(848.528, workUnitModel.GetCredit(SlotStatus.RunningNoFrameTimes, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(2036.4672, workUnitModel.GetPPD(SlotStatus.RunningNoFrameTimes, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
        }

        [Test]
        public void CreditUPDAndPPDTest2()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6 };
            var utcNow = DateTime.UtcNow;
            var unitInfo = new WorkUnit { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(1897.367, workUnitModel.GetCredit(SlotStatus.Running, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(4553.6808, workUnitModel.GetPPD(SlotStatus.Running, PPDCalculation.LastFrame, BonusCalculation.DownloadTime));
        }

        [Test]
        public void CreditUPDAndPPDTest3()
        {
            var protein = new Protein { ProjectNumber = 1, Credit = 100 };
            var unitInfo = new WorkUnit { FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(protein, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(100, workUnitModel.GetCredit(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(240, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        [Test]
        public void CreditUPDAndPPDTest4()
        {
            var unitInfo = new WorkUnit { FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(0, workUnitModel.GetCredit(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
            Assert.AreEqual(2.4, workUnitModel.GetUPD(PPDCalculation.LastFrame));
            Assert.AreEqual(0, workUnitModel.GetPPD(SlotStatus.Unknown, PPDCalculation.LastFrame, BonusCalculation.None));
        }

        #endregion

        #region ETA

        [Test]
        public void EtaTest1()
        {
            var unitInfo = new WorkUnit { FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(TimeSpan.FromMinutes(582), workUnitModel.GetEta(PPDCalculation.LastFrame));
        }

        [Test]
        public void EtaTest2()
        {
            var unitInfo = new WorkUnit { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(CreateFrameData(TimeSpan.Zero, 0),
                     CreateFrameData(TimeSpan.FromMinutes(4), 1),
                     CreateFrameData(TimeSpan.FromMinutes(5), 2),
                     CreateFrameData(TimeSpan.FromMinutes(6), 3));
            unitInfo.FrameData = frameDataDictionary;
            unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.AreEqual(unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(612)), workUnitModel.GetEtaDate(PPDCalculation.LastFrame));
        }

        #endregion

        #region AllFramesCompleted

        [Test]
        public void AllFramesCompleted1()
        {
            var unitInfo = new WorkUnit();
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(CreateFrameData(TimeSpan.Zero, 100));
            unitInfo.FrameData = frameDataDictionary;
            var workUnitModel = CreateWorkUnitModel(null, unitInfo);
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.IsTrue(workUnitModel.AllFramesCompleted);
        }

        [Test]
        public void AllFramesCompleted2()
        {
            var workUnitModel = CreateWorkUnitModel(null, new WorkUnit());
            workUnitModel.UtcOffsetIsZero = false;
            workUnitModel.ClientTimeOffset = 0;

            Assert.IsFalse(workUnitModel.AllFramesCompleted);
        }

        #endregion

        #region Helpers

        private WorkUnitModel CreateWorkUnitModel(Protein protein, WorkUnit workUnit)
        {
            return new WorkUnitModel(_benchmarkService)
            {
                CurrentProtein = protein,
                Data = workUnit
            };
        }

        private static WorkUnitFrameData CreateFrameData(TimeSpan duration, int frameId)
        {
            return new WorkUnitFrameData { ID = frameId, Duration = duration };
        }

        #endregion
    }
}
