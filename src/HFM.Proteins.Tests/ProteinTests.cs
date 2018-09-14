/*
 * HFM.NET - Protein Class Tests
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

using NUnit.Framework;

namespace HFM.Proteins
{
   [TestFixture]
   public class ProteinTests
   {
      [Test]
      public void Protein_DefaultValues_Test()
      {
         var protein = new Protein();
         Assert.AreEqual(0, protein.ProjectNumber);
         Assert.AreEqual("0.0.0.0", protein.ServerIP);
         Assert.AreEqual("Unknown", protein.WorkUnitName);
         Assert.AreEqual(0, protein.NumberOfAtoms);
         Assert.AreEqual(0, protein.PreferredDays);
         Assert.AreEqual(0, protein.MaximumDays);
         Assert.AreEqual(0, protein.Credit);
         Assert.AreEqual(100, protein.Frames);
         Assert.AreEqual("Unknown", protein.Core);
         Assert.AreEqual("Unassigned Description", protein.Description);
         Assert.AreEqual("Unknown", protein.Contact);
         Assert.AreEqual(0, protein.KFactor);
      }

      [Test]
      public void Protein_IsUnknown_ReturnsFalseWhenProjectNumberIsNotZero_Test()
      {
         var protein = new Protein { ProjectNumber = 1 };
         Assert.IsFalse(protein.IsUnknown);
      }

      [Test]
      public void Protein_IsUnknown_ReturnsTrueWhenProjectNumberIsZero_Test()
      {
         var protein = new Protein();
         Assert.IsTrue(protein.IsUnknown);
      }

      [Test]
      public void Protein_IsValid_ReturnsTrueWhenAllRequiredPropertiesArePopulated_Test()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3, MaximumDays = 5, Credit = 500, Frames = 100, KFactor = 26.4 };
         Assert.IsTrue(protein.IsValid);
      }

      [Test]
      public void Protein_IsValid_ReturnsFalseWhenAllRequiredPropertiesAreNotPopulated_Test()
      {
         var protein = new Protein();
         Assert.IsFalse(protein.IsValid);
      }

      [Test]
      public void Protein_DeepClone_Test()
      {
         // Arrange
         var p = new Protein
         {
            ProjectNumber = 1234,
            ServerIP = "1.2.3.4",
            WorkUnitName = "Foo",
            NumberOfAtoms = 20,
            PreferredDays = 3.0,
            MaximumDays = 5.0,
            Credit = 357,
            Frames = 5000,
            Core = "A7",
            Description = "http://foo.bar/biz",
            Contact = "harlam357",
            KFactor = 0.75
         };
         // Act
         var clone = p.DeepClone();
         // Assert
         Assert.AreNotSame(p, clone);
         Assert.AreEqual(p.ProjectNumber, clone.ProjectNumber);
         Assert.AreEqual(p.ServerIP, clone.ServerIP);
         Assert.AreEqual(p.WorkUnitName, clone.WorkUnitName);
         Assert.AreEqual(p.NumberOfAtoms, clone.NumberOfAtoms);
         Assert.AreEqual(p.PreferredDays, clone.PreferredDays);
         Assert.AreEqual(p.MaximumDays, clone.MaximumDays);
         Assert.AreEqual(p.Credit, clone.Credit);
         Assert.AreEqual(p.Frames, clone.Frames);
         Assert.AreEqual(p.Core, clone.Core);
         Assert.AreEqual(p.Description, clone.Description);
         Assert.AreEqual(p.Contact, clone.Contact);
         Assert.AreEqual(p.KFactor, clone.KFactor);
      }
   }
}
