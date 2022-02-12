using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

using NUnit.Framework;

using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRepositoryTests
    {
        private const string TestDataFile = "TestFiles\\TestData.db3";
        private string _testDataFileCopy;

        private const string TestData2File = "TestFiles\\TestData2.db3";
        private string _testData2FileCopy;

        // this file is the same as TestDataFile but has already had UpgradeWuHistory1() run on it
        private const string TestDataFileUpgraded = "TestFiles\\TestData_1.db3";
        private string _testDataFileUpgradedCopy;

        private string _testScratchFile;

        private ArtifactFolder _artifacts;
        private WorkUnitRepository _repository;
        private readonly IProteinService _proteinService = CreateProteinService();

        #region Setup and TearDown

        [SetUp]
        public void Init()
        {
            SetupTestDataFileCopies();

            _repository = new WorkUnitRepository(null, _proteinService);
        }

        private void SetupTestDataFileCopies()
        {
            _artifacts = new ArtifactFolder();

            // sometimes the file is not finished
            // copying before we attempt to open
            // the copied file.  Halt the thread
            // for a bit to ensure the copy has
            // completed.

            _testDataFileCopy = _artifacts.GetRandomFilePath();
            File.Copy(TestDataFile, _testDataFileCopy, true);
            Thread.Sleep(100);

            _testData2FileCopy = _artifacts.GetRandomFilePath();
            File.Copy(TestData2File, _testData2FileCopy, true);
            Thread.Sleep(100);

            _testDataFileUpgradedCopy = _artifacts.GetRandomFilePath();
            File.Copy(TestDataFileUpgraded, _testDataFileUpgradedCopy, true);
            Thread.Sleep(100);

            _testScratchFile = _artifacts.GetRandomFilePath();
        }

        [TearDown]
        public void Destroy()
        {
            _artifacts?.Dispose();
            _repository?.Dispose();
        }

        #endregion

        #region Connected

        [Test]
        public void WorkUnitRepository_Connected_Test1()
        {
            _repository.Initialize(_testScratchFile);
            VerifyWuHistoryTableSchema(_testScratchFile);
            Assert.AreEqual(Application.Version, _repository.GetDatabaseVersion());
            Assert.AreEqual(true, _repository.Connected);
        }

        #endregion

        #region Upgrade

        [Test]
        public void WorkUnitRepository_Upgrade_v092_Test1()
        {
            // Assert (pre-condition)
            Assert.AreEqual(15, GetWuHistoryColumnCount(_testDataFileCopy));
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
            // Arrange
            _repository.Initialize(_testDataFileCopy);
            // Act
            if (_repository.RequiresUpgrade())
            {
                _repository.Upgrade();
            }
            // Assert
            VerifyWuHistoryTableSchema(_testDataFileCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileCopy));
            Assert.AreEqual(Version.Parse("0.9.2"), Version.Parse(_repository.GetDatabaseVersion()));
        }

        [Test]
        public void WorkUnitRepository_Upgrade_v092_AlreadyUpgraded_Test()
        {
            // Assert (pre-condition)
            VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
            // Arrange
            _repository.Initialize(_testDataFileUpgradedCopy);
            // Act
            var result = _repository.RequiresUpgrade();
            // Assert
            Assert.IsFalse(result);
            VerifyWuHistoryTableSchema(_testDataFileUpgradedCopy);
            Assert.AreEqual(44, GetWuHistoryRowCount(_testDataFileUpgradedCopy));
            Assert.IsTrue(Version.Parse("0.9.2") <= Version.Parse(_repository.GetDatabaseVersion()));
        }

        [Test]
        public void WorkUnitRepository_Upgrade_v092_Test2()
        {
            // Assert (pre-condition)
            Assert.AreEqual(15, GetWuHistoryColumnCount(_testData2FileCopy));
            Assert.AreEqual(285, GetWuHistoryRowCount(_testData2FileCopy));
            // Arrange
            _repository.Initialize(_testData2FileCopy);
            // Act
            if (_repository.RequiresUpgrade())
            {
                _repository.Upgrade();
            }
            // Assert
            VerifyWuHistoryTableSchema(_testData2FileCopy);
            // 32 duplicates deleted
            Assert.AreEqual(253, GetWuHistoryRowCount(_testData2FileCopy));
            Assert.AreEqual(Version.Parse("0.9.2"), Version.Parse(_repository.GetDatabaseVersion()));
        }

        #endregion

        #region Static Helpers

        private static int GetWuHistoryColumnCount(string dataSource)
        {
            using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
            {
                con.Open();
                using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", con))
                using (var table = new DataTable())
                {
                    adapter.Fill(table);
                    foreach (DataRow row in table.Rows)
                    {
                        Debug.WriteLine(row[1].ToString());
                    }
                    return table.Rows.Count;
                }
            }
        }

        private static void VerifyWuHistoryTableSchema(string dataSource)
        {
            using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
            {
                con.Open();
                using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", con))
                using (var table = new DataTable())
                {
                    adapter.Fill(table);
                    Assert.AreEqual(23, table.Rows.Count);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        var row = table.Rows[i];
                        // notnull check
                        Assert.AreEqual(1, row[3]);
                        // dflt_value check
                        if (i < 15)
                        {
                            Assert.IsTrue(row[4].Equals(DBNull.Value));
                        }
                        else
                        {
                            Assert.IsFalse(row[4].Equals(DBNull.Value));
                        }
                        // pk check
                        Assert.AreEqual(i == 0 ? 1 : 0, row[5]);
                    }

                }
            }
        }

        private static int GetWuHistoryRowCount(string dataSource)
        {
            using (var con = new SQLiteConnection(@"Data Source=" + dataSource))
            {
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM WuHistory";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static IProteinService CreateProteinService()
        {
            var collection = new List<Protein>();

            var protein = new Protein();
            protein.ProjectNumber = 6600;
            protein.WorkUnitName = "WorkUnitName";
            protein.Core = "GROGPU2";
            protein.Credit = 450;
            protein.KFactor = 0;
            protein.Frames = 100;
            protein.NumberOfAtoms = 5000;
            protein.PreferredDays = 2;
            protein.MaximumDays = 3;
            collection.Add(protein);

            protein = new Protein();
            protein.ProjectNumber = 5797;
            protein.WorkUnitName = "WorkUnitName2";
            protein.Core = "GROGPU2";
            protein.Credit = 675;
            protein.KFactor = 2.3;
            protein.Frames = 100;
            protein.NumberOfAtoms = 7000;
            protein.PreferredDays = 2;
            protein.MaximumDays = 3;
            collection.Add(protein);

            protein = new Protein();
            protein.ProjectNumber = 8011;
            protein.WorkUnitName = "WorkUnitName3";
            protein.Core = "GRO-A4";
            protein.Credit = 106.6;
            protein.KFactor = 0.75;
            protein.Frames = 100;
            protein.NumberOfAtoms = 9000;
            protein.PreferredDays = 2.13;
            protein.MaximumDays = 4.62;
            collection.Add(protein);

            protein = new Protein();
            protein.ProjectNumber = 6903;
            protein.WorkUnitName = "WorkUnitName4";
            protein.Core = "GRO-A5";
            protein.Credit = 22706;
            protein.KFactor = 38.05;
            protein.Frames = 100;
            protein.NumberOfAtoms = 11000;
            protein.PreferredDays = 5;
            protein.MaximumDays = 12;
            collection.Add(protein);

            var dataContainer = new ProteinDataContainer();
            dataContainer.Data = collection;
            return new ProteinService(dataContainer, null, null);
        }

        #endregion
    }
}
