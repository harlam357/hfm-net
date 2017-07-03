﻿/*
 * HFM.NET - Protein Dictionary Tests
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
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinDictionaryTests
   {
      [Test]
      public void ProteinDictionary_LoadEmptyDictionaryFromJsonFile_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // Act
         var results = dictionary.Load("..\\..\\TestFiles\\summary.json");
         // Assert
         Assert.AreEqual(624, results.Count);
         Assert.AreEqual(623, dictionary.Count);
      }

      // loads data from external resource
      [Ignore]
      [Test]
      public void ProteinDictionary_LoadEmptyDictionaryFromJsonUri_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // Act
         var results = dictionary.Load(new Uri(StanfordWebAddresses.JsonProjectSummary));
         // Assert
         Assert.AreNotEqual(0, results.Count);
         Assert.AreNotEqual(0, dictionary.Count);
      }

      [Test]
      public void ProteinDictionary_LoadEmptyDictionaryFromJsonStream_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // Act
         using (var stream = File.OpenRead("..\\..\\TestFiles\\summary.json"))
         {
            var results = dictionary.Load(stream);
            // Assert
            Assert.AreEqual(624, results.Count);
            Assert.AreEqual(623, dictionary.Count);
         }
      }

      [Test]
      public void ProteinDictionary_LoadEmptyDictionary_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // build the collection of proteins to load
         var values = new List<Protein>();
         values.Add(CreateValidProtein(1));
         values.Add(CreateValidProtein(2));
         values.Add(new Protein { ProjectNumber = 3 });
         // Act
         var results = dictionary.Load(values).ToList();
         // Assert
         Assert.AreEqual(2, results.Count);
         Assert.AreEqual(1, results[0].ProjectNumber);
         Assert.AreEqual(ProteinLoadResult.Added, results[0].Result);
         Assert.IsNull(results[0].Changes);
         Assert.AreEqual(2, results[1].ProjectNumber);
         Assert.AreEqual(ProteinLoadResult.Added, results[1].Result);
         Assert.IsNull(results[1].Changes);
      }

      [Test]
      public void ProteinDictionary_LoadPopulatedDictionaryAndVerifyLoadResults_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // add proteins so we have something that already exists
         dictionary.Add(1, CreateValidProtein(1));
         dictionary.Add(2, CreateValidProtein(2));
         dictionary.Add(3, CreateValidProtein(3));
         // build the collection of proteins to load
         var values = new List<Protein>();
         var protein = CreateValidProtein(1);
         protein.Credit = 100;
         values.Add(protein);
         protein = CreateValidProtein(2);
         protein.MaximumDays = 3;
         protein.KFactor = 26.4;
         values.Add(protein);
         values.Add(CreateValidProtein(3));
         // Act
         var results = dictionary.Load(values).ToList();
         // Assert
         Assert.AreEqual(3, results.Count);
         // check index 0
         Assert.AreEqual(1, results[0].ProjectNumber);
         Assert.AreEqual(ProteinLoadResult.Changed, results[0].Result);
         var changes = results[0].Changes.ToList();
         Assert.AreEqual(1, changes.Count);
         Assert.AreEqual("Credit", changes[0].Name);
         Assert.AreEqual("1", changes[0].OldValue);
         Assert.AreEqual("100", changes[0].NewValue);
         // check index 1
         Assert.AreEqual(2, results[1].ProjectNumber);
         Assert.AreEqual(ProteinLoadResult.Changed, results[1].Result);
         changes = results[1].Changes.ToList();
         Assert.AreEqual(2, changes.Count);
         Assert.AreEqual("MaximumDays", changes[0].Name);
         Assert.AreEqual("1", changes[0].OldValue);
         Assert.AreEqual("3", changes[0].NewValue);
         Assert.AreEqual("KFactor", changes[1].Name);
         Assert.AreEqual("0", changes[1].OldValue);
         Assert.AreEqual("26.4", changes[1].NewValue);
         // check index 2
         Assert.AreEqual(3, results[2].ProjectNumber);
         Assert.AreEqual(ProteinLoadResult.NoChange, results[2].Result);
         Assert.IsNull(results[2].Changes);
      }

      [Test]
      public void ProteinDictionary_LoadPopulatedDictionaryAndVerifyDictionaryContents_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // add proteins so we have something that already exists
         dictionary.Add(1, CreateValidProtein(1));
         dictionary.Add(2, CreateValidProtein(2));
         dictionary.Add(3, CreateValidProtein(3));
         // build the collection of proteins to load
         var values = new List<Protein>();
         var protein = CreateValidProtein(1);
         protein.Credit = 100;
         values.Add(protein);
         protein = CreateValidProtein(2);
         protein.MaximumDays = 3;
         protein.KFactor = 26.4;
         values.Add(protein);
         values.Add(CreateValidProtein(3));
         // Act
         dictionary.Load(values);
         // Assert
         Assert.AreEqual(3, dictionary.Count);
         // check project 1
         Assert.AreEqual(1, dictionary[1].ProjectNumber);
         Assert.AreEqual(100, dictionary[1].Credit);
         // check project 2
         Assert.AreEqual(2, dictionary[2].ProjectNumber);
         Assert.AreEqual(3, dictionary[2].MaximumDays);
         Assert.AreEqual(26.4, dictionary[2].KFactor);
         // check project 3
         Assert.AreEqual(3, dictionary[3].ProjectNumber);
      }

      [Test]
      public void ProteinDictionary_AddToEmptyDictionary_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // Act
         dictionary.Add(1, CreateValidProtein(1));
         // Assert
         Assert.AreEqual(1, dictionary.Count);
      }

      [Test]
      public void ProteinDictionary_AddThrowsArgumentExceptionWhenProteinAlreadyExists_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         dictionary.Add(1, CreateValidProtein(1));
         // Act & Assert
         Assert.Throws<ArgumentException>(() => dictionary.Add(1, CreateValidProtein(1)));
      }

      [Test]
      public void ProteinDictionary_AddThrowsArgumentNullExceptionWhenProteinIsNull_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentNullException>(() => dictionary.Add(1, null));
      }

      [Test]
      public void ProteinDictionary_AddThrowsArgumentExceptionWhenProteinIsNotValid_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentException>(() => dictionary.Add(1, new Protein()));
      }

      [Test]
      public void ProteinDictionary_AddThrowsArgumentExceptionWhenKeyAndProteinIdDoNotMatch_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentException>(() => dictionary.Add(1, CreateValidProtein(2)));
      }

      [Test]
      public void ProteinDictionary_IndexSetToEmptyDictionary_Test()
      {
         // Arrange
         var dictionary = new ProteinDictionary();
         // Act
         dictionary[1] = CreateValidProtein(1);
         // Assert
         Assert.AreEqual(1, dictionary.Count);
      }

      [Test]
      public void ProteinDictionary_IndexSetThrowsArgumentNullExceptionWhenProteinIsNull_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentNullException>(() => dictionary[1] = null);
      }

      [Test]
      public void ProteinDictionary_IndexSetThrowsArgumentExceptionWhenProteinIsNotValid_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentException>(() => dictionary[1] = new Protein());
      }

      [Test]
      public void ProteinDictionary_IndexSetTThrowsArgumentExceptionWhenKeyAndProteinIdDoNotMatch_Test()
      {
         var dictionary = new ProteinDictionary();
         Assert.Throws<ArgumentException>(() => dictionary[1] = CreateValidProtein(2));
      }

      private static Protein CreateValidProtein(int projectNumber)
      {
         return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
      }
   }
}
