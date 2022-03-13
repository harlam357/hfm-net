using HFM.Core.WorkUnits;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitRepositoryReadOnlyTests
    {
        private const string TestDataFile = "TestFiles\\TestData.db3";
        private string _testDataFileCopy;

        private const string TestData2File = "TestFiles\\TestData2.db3";
        private string _testData2FileCopy;

        private ArtifactFolder _artifacts;
        private WorkUnitRepository _repository;
        private readonly IProteinService _proteinService = WorkUnitRepositoryTests.CreateProteinService();

        [OneTimeSetUp]
        public void BeforeAll()
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
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            _artifacts?.Dispose();
            _repository?.Dispose();
        }

        [Test]
        public void WorkUnitRepository_Fetch_All_Test()
        {
            Initialize(_testDataFileCopy);
            var rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.DownloadTime);
            Assert.AreEqual(44, rows.Count);
        }

        [Test]
        public void WorkUnitRepository_Fetch_All_Test2()
        {
            Initialize(_testData2FileCopy);
            var rows = _repository.Fetch(WorkUnitQuery.SelectAll, BonusCalculation.FrameTime);
            Assert.AreEqual(253, rows.Count);
        }

        private void Initialize(string path)
        {
            _repository.Initialize(path);
            _repository.Upgrade();
        }
    }
}
