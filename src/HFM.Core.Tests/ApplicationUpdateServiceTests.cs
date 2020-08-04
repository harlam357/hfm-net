using System;
using System.IO;

using NUnit.Framework;

namespace HFM.Core
{
    [TestFixture]
    public class ApplicationUpdateServiceTests
    {
        [Test]
        public void ApplicationUpdateService_GetApplicationUpdate_ReturnsObject()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                var update = new ApplicationUpdate { Version = "1.2.3.4" };

                using (FileStream stream = File.OpenWrite(path))
                {
                    new ApplicationUpdateSerializer().Serialize(stream, update);
                }
                var uri = new Uri(path);

                var service = new ApplicationUpdateService(null);
                // Act
                var result = service.GetApplicationUpdate(uri.AbsoluteUri);
                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("1.2.3.4", update.Version);
            }
        }
    }
}
