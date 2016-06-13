
using System;
using System.IO;

using NUnit.Framework;

namespace HFM.Log
{
   [TestFixture]
   public class UnitInfoLogTests
   {
      // ReSharper disable InconsistentNaming

      [Test]
      public void UnitInfoLog_Read_GPU2_5_Test()
      {
         var data = UnitInfoLog.Read("..\\..\\..\\TestFiles\\GPU2_5\\unitinfo.txt");
         Assert.AreEqual("p4744_lam5w_300K", data.ProteinName);
         Assert.AreEqual("-", data.ProteinTag);
         Assert.AreEqual(0, data.ProjectID);
         Assert.AreEqual(0, data.ProjectRun);
         Assert.AreEqual(0, data.ProjectClone);
         Assert.AreEqual(0, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 2, 20, 35, 41), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 5, 20, 35, 41), data.DueTime);
         Assert.AreEqual(73, data.Progress);
      }

      [Test]
      public void UnitInfoLog_Read_SMP_10_Test()
      {
         var data = UnitInfoLog.Read("..\\..\\..\\TestFiles\\SMP_10\\unitinfo.txt");
         Assert.AreEqual("Gromacs", data.ProteinName);
         Assert.AreEqual("P2683R6C12G21", data.ProteinTag);
         Assert.AreEqual(2683, data.ProjectID);
         Assert.AreEqual(6, data.ProjectRun);
         Assert.AreEqual(12, data.ProjectClone);
         Assert.AreEqual(21, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 12, 0, 9, 22), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 18, 0, 9, 22), data.DueTime);
         Assert.AreEqual(1724900, data.Progress);
      }

      // ReSharper restore InconsistentNaming

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void UnitInfoLog_Read_ArgumentException_Test1()
      {
         UnitInfoLog.Read(null);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void UnitInfoLog_Read_ArgumentException_Test2()
      {
         UnitInfoLog.Read(String.Empty);
      }

      [Test]
      [ExpectedException(typeof(DirectoryNotFoundException))]
      public void UnitInfoLog_Read_DirectoryNotFoundException_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\DoesNotExist\\unitinfo.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void UnitInfoLog_Read_Malformed_1_UnitInfo1_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo1.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void UnitInfoLog_Read_Malformed_1_UnitInfo2_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo2.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void UnitInfoLog_Read_Malformed_1_UnitInfo3_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo3.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void UnitInfoLog_Read_Malformed_1_UnitInfo4_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo4.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void UnitInfoLog_Read_Malformed_1_UnitInfo5_Test()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo5.txt");
      }
   }
}
