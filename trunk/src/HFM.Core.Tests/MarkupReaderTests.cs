/*
 * HFM.NET - Markup Reader Class Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using System.Linq;

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class MarkupReaderTests
   {
      private IPreferenceSet _prefs;

      [SetUp]
      public void Init()
      {
         _prefs = MockRepository.GenerateStub<IPreferenceSet>();
         _prefs.Stub(x => x.Get<PpdCalculationType>(Preference.PpdCalculation)).Return(PpdCalculationType.LastThreeFrames);
         _prefs.Stub(x => x.Get<BonusCalculationType>(Preference.CalculateBonus)).Return(BonusCalculationType.DownloadTime);
         _prefs.Stub(x => x.Get<bool>(Preference.ShowVersions)).Return(true);
         _prefs.Stub(x => x.Get<int>(Preference.DecimalPlaces)).Return(1);
         _prefs.Stub(x => x.Get<CompletedCountDisplayType>(Preference.CompletedCountDisplay)).Return(CompletedCountDisplayType.ClientRunTotal);
         _prefs.Stub(x => x.Get<bool>(Preference.EtaDate)).Return(false);
         _prefs.Stub(x => x.Get<string>(Preference.StanfordId)).Return("harlam357");
         _prefs.Stub(x => x.Get<int>(Preference.EtaDate)).Return(32);

         var container = new WindsorContainer();
         container.Register(
            Component.For<UnitInfoLogic>()
               .LifeStyle.Transient);
         ServiceLocator.SetContainer(container);
         container.Register(
            Component.For<IProteinBenchmarkCollection>()
               .ImplementedBy<ProteinBenchmarkCollection>());
         Core.Configuration.ObjectMapper.CreateMaps();
      }

      [Test]
      public void ReaderTest1()
      {
         var proteinDictionary = MockRepository.GenerateStub<IProteinDictionary>();
         proteinDictionary.Stub(x => x.GetProteinOrDownload(0)).IgnoreArguments().Return(new Protein());
         var markupReader = new MarkupReader(_prefs, proteinDictionary);
         var slots = markupReader.Read("..\\..\\TestFiles\\SlotSummary1.xml");
         Assert.AreEqual(9, slots.Count());
      }
   }
}
