
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class TabSerializerTests
   {
      [Test]
      public void DeserializeTest1()
      {
         var serializer = new TabSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\ProjectInfo.tab").ToList();
         Assert.AreEqual(1409, proteins.Count);
      }

      [Test]
      public void SerializeTest1()
      {
         var proteins = new List<Protein>();
         proteins.Add(new Protein
                      {
                         ProjectNumber = 6900,
                         ServerIP = "1.2.3.4",
                         WorkUnitName = "Name of Work Unit",
                         NumberOfAtoms = 10000,
                         PreferredDays = 3,
                         MaximumDays = 5,
                         Credit = 500,
                         Frames = 100,
                         Core = "GRO-A5",
                         Description = "http://something.com",
                         Contact = "me",
                         KFactor = 26.4
                      });
         proteins.Add(new Protein
         {
            ProjectNumber = 6901,
            ServerIP = "5.6.7.8",
            WorkUnitName = "Work Unit Name",
            NumberOfAtoms = 78910,
            PreferredDays = 4,
            MaximumDays = 5,
            Credit = 512,
            Frames = 100,
            Core = "GRO-A5",
            Description = "http://somethingelse.com",
            Contact = "you",
            KFactor = 2
         });

         const string fileName = "ProjectInfoTest.tab";

         var serializer = new TabSerializer();
         serializer.Serialize(fileName, proteins);
         Assert.IsTrue(File.Exists(fileName));

         proteins = serializer.Deserialize(fileName).ToList();
         Assert.AreEqual(2, proteins.Count);
      }
   }
}
