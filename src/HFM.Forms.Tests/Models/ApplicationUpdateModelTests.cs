using System.Collections.Generic;

using NUnit.Framework;

using HFM.Core;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class ApplicationUpdateModelTests
    {
        [Test]
        public void ApplicationUpdateModel_UpdateFileList_IsEmptyWhenApplicationUpdateIsNull()
        {
            // Act
            var model = new ApplicationUpdateModel(null);
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(0, model.UpdateFilesList.Count);
        }

        [Test]
        public void ApplicationUpdateModel_UpdateFileList_IsEmptyWhenUpdateFilesIsNull()
        {
            // Act
            var model = new ApplicationUpdateModel(new ApplicationUpdate());
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(0, model.UpdateFilesList.Count);
        }

        [Test]
        public void ApplicationUpdateModel_UpdateFileList_IsCreatedFromUpdateFiles()
        {
            // Act
            var update = new ApplicationUpdate
            {
                UpdateFiles = new List<ApplicationUpdateFile>
                {
                    new ApplicationUpdateFile(),
                    new ApplicationUpdateFile()
                }
            };
            var model = new ApplicationUpdateModel(update);
            // Assert
            Assert.IsNotNull(model.UpdateFilesList);
            Assert.AreEqual(2, model.UpdateFilesList.Count);
        }
    }
}
