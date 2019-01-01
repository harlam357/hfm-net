
using System;
using System.IO;

using NUnit.Framework;

namespace HFM.Log.Legacy
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
      public void UnitInfoLog_Read_ThrowsWhenPathIsNull_Test()
      {
         Assert.Throws<ArgumentNullException>(() => UnitInfoLog.Read(null));
      }

      [Test]
      public void UnitInfoLog_Read_ThrowsWhenPathIsEmptyString_Test()
      {
         Assert.Throws<ArgumentException>(() => UnitInfoLog.Read(String.Empty));
      }

      [Test]
      public void UnitInfoLog_Read_ThrowsWhenDirectoryNotFound_Test()
      {
         Assert.Throws<DirectoryNotFoundException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\DoesNotExist\\unitinfo.txt"));
      }

      [Test]
      public void UnitInfoLog_Read_LegacyUnitinfo_ThrowsOnTagFormat_Test()
      {
         Assert.Throws<FormatException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\LegacyUnitinfo\\unitinfo_TagFormat.txt"));
      }

      [Test]
      public void UnitInfoLog_Read_LegacyUnitinfo_ThrowsOnDownloadDateFormat_Test()
      {
         Assert.Throws<FormatException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\LegacyUnitinfo\\unitinfo_DownloadDateFormat.txt"));
      }

      [Test]
      public void UnitInfoLog_Read_LegacyUnitinfo_ThrowsOnDueDateFormat_Test()
      {
         Assert.Throws<FormatException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\LegacyUnitinfo\\unitinfo_DueDateFormat.txt"));
      }

      [Test]
      public void UnitInfoLog_Read_LegacyUnitinfo_ThrowsOnDueTimeFormat_Test()
      {
         Assert.Throws<FormatException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\LegacyUnitinfo\\unitinfo_DueTimeFormat.txt"));
      }

      [Test]
      public void UnitInfoLog_Read_LegacyUnitinfo_ThrowsOnProgressOverflow_Test()
      {
         Assert.Throws<FormatException>(() => UnitInfoLog.Read("..\\..\\..\\TestFiles\\LegacyUnitinfo\\unitinfo_ProgressOverflow.txt"));
      }
   }
}
