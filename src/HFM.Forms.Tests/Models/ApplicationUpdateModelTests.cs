using System.Collections.Generic;

using NUnit.Framework;

using HFM.Core;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class ApplicationUpdateModelTests
    {
        [Test]
        public void ApplicationUpdateModel_Load_UpdateFileList_IsEmptyWhenApplicationUpdateIsNull()
        {
            // Arrange
            var model = new ApplicationUpdateModel(null);
            // Act
            model.Load();
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(0, model.UpdateFilesList.Count);
        }

        [Test]
        public void ApplicationUpdateModel_Load_UpdateFileList_IsEmptyWhenUpdateFilesIsNull()
        {
            // Arrange
            var model = new ApplicationUpdateModel(new ApplicationUpdate());
            // Act
            model.Load();
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(0, model.UpdateFilesList.Count);
        }

        [Test]
        public void ApplicationUpdateModel_Load_UpdateFileList_IsCreatedFromUpdateFiles()
        {
            // Arrange
            var update = new ApplicationUpdate
            {
                UpdateFiles = new List<ApplicationUpdateFile>
                {
                    new ApplicationUpdateFile { Description = "Foo" },
                    new ApplicationUpdateFile { Description = "Bar" }
                }
            };
            var model = new ApplicationUpdateModel(update);
            // Act
            model.Load();
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(2, model.UpdateFilesList.Count);
        }
    }
}
