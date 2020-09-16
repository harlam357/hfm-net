using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using NUnit.Framework;

using HFM.Core;
using HFM.Forms.Mocks;
using HFM.Forms.Models;
using HFM.Forms.Presenters.Mocks;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    [TestFixture]
    public class ApplicationUpdatePresenterTests
    {
        [Test]
        public void ApplicationUpdatePresenter_CancelClicked_SetsDialogResultCancelAndCloses()
        {
            // Arrange
            using (var presenter = new MockDialogApplicationUpdatePresenter(new ApplicationUpdateModel(new ApplicationUpdate())))
            {
                presenter.ShowDialog(null);
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                presenter.CancelClick();
                // Assert
                Assert.IsFalse(presenter.MockDialog.Shown);
                Assert.AreEqual(DialogResult.Cancel, presenter.Dialog.DialogResult);
            }
        }

        [Test]
        public async Task ApplicationUpdatePresenter_DownloadClicked_DoesNothingWhenThereIsNoSelectedUpdateFile()
        {
            // Arrange
            var model = CreateUpdateModel("foo", ApplicationUpdateFileType.Executable);
            using (var presenter = new MockDialogApplicationUpdatePresenter(model))
            {
                presenter.ShowDialog(null);
                model.SelectedUpdateFile = null;
                Assert.IsTrue(presenter.MockDialog.Shown);
                // Act
                var saveFile = new MockFileDialogPresenterReturnsFileName(null, "bar");
                await presenter.DownloadClick(saveFile);
                // Assert
                Assert.IsTrue(presenter.MockDialog.Shown);
                Assert.AreEqual(0, saveFile.Invocations.Count);
            }
        }

        [Test]
        public async Task ApplicationUpdatePresenter_DownloadClicked_DownloadsSetsDialogResultOKAndCloses()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string sourceFile = artifacts.GetRandomFilePath();
                string targetFile = artifacts.GetRandomFilePath();

                var model = CreateUpdateModel(sourceFile, ApplicationUpdateFileType.Executable);
                using (var presenter = new MockDialogApplicationUpdatePresenter(model))
                {
                    presenter.ShowDialog(null);
                    Assert.IsTrue(presenter.MockDialog.Shown);
                    // Act
                    var saveFile = new MockFileDialogPresenterReturnsFileName(_ => DialogResult.OK, targetFile);
                    await presenter.DownloadClick(saveFile);
                    // Assert
                    Assert.IsFalse(presenter.MockDialog.Shown);
                    Assert.AreEqual(DialogResult.OK, presenter.Dialog.DialogResult);
                    Assert.AreEqual(1, saveFile.Invocations.Count);
                }
            }
        }

        [Test]
        public async Task ApplicationUpdatePresenter_DownloadClicked_WhenFileTypeIsExecutableSetsLocalFilePathAndReadyToBeExecuted()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string sourceFile = artifacts.GetRandomFilePath();
                string targetFile = artifacts.GetRandomFilePath();

                var model = CreateUpdateModel(sourceFile, ApplicationUpdateFileType.Executable);
                using (var presenter = new MockDialogApplicationUpdatePresenter(model))
                {
                    presenter.ShowDialog(null);
                    // Act
                    var saveFile = new MockFileDialogPresenterReturnsFileName(_ => DialogResult.OK, targetFile);
                    await presenter.DownloadClick(saveFile);
                    // Assert
                    Assert.AreEqual(targetFile, model.SelectedUpdateFileLocalFilePath);
                    Assert.IsTrue(model.SelectedUpdateFileIsReadyToBeExecuted);
                    Assert.AreEqual(1, saveFile.Invocations.Count);
                }
            }
        }

        [Test]
        public async Task ApplicationUpdatePresenter_DownloadClicked_WhenFileTypeIsDownloadOnlySetsLocalFilePathAndReadyToBeExecutedIsFalse()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                string sourceFile = artifacts.GetRandomFilePath();
                string targetFile = artifacts.GetRandomFilePath();

                var model = CreateUpdateModel(sourceFile, ApplicationUpdateFileType.DownloadOnly);
                using (var presenter = new MockDialogApplicationUpdatePresenter(model))
                {
                    presenter.ShowDialog(null);
                    // Act
                    var saveFile = new MockFileDialogPresenterReturnsFileName(_ => DialogResult.OK, targetFile);
                    await presenter.DownloadClick(saveFile);
                    // Assert
                    Assert.AreEqual(targetFile, model.SelectedUpdateFileLocalFilePath);
                    Assert.IsFalse(model.SelectedUpdateFileIsReadyToBeExecuted);
                    Assert.AreEqual(1, saveFile.Invocations.Count);
                }
            }
        }

        [Test]
        public async Task ApplicationUpdatePresenter_DownloadClicked_ShowsMessageBoxWhenDownloadFails()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var updateFile = new ApplicationUpdateFile { Description = "Foo", Name = "Foo.zip", HttpAddress = @"C:\DoesNotExist\Foo.zip" };
                var update = new ApplicationUpdate
                {
                    UpdateFiles = new List<ApplicationUpdateFile> { updateFile }
                };
                var model = new ApplicationUpdateModel(update);
                model.SelectedUpdateFile = update.UpdateFiles.First();

                var messageBox = new MockMessageBoxPresenter();
                using (var presenter = new MockDialogApplicationUpdatePresenter(model, messageBox))
                {
                    presenter.ShowDialog(null);
                    Assert.IsTrue(presenter.MockDialog.Shown);
                    // Act
                    var saveFile = new MockFileDialogPresenterReturnsFileName(_ => DialogResult.OK, artifacts.GetRandomFilePath());
                    await presenter.DownloadClick(saveFile);
                    // Assert
                    Assert.AreEqual(1, messageBox.Invocations.Count);
                    Assert.IsTrue(presenter.MockDialog.Shown);
                    Assert.AreEqual(1, saveFile.Invocations.Count);
                }
            }
        }

        private static ApplicationUpdateModel CreateUpdateModel(string sourceFile, ApplicationUpdateFileType updateType)
        {
            File.WriteAllText(sourceFile, "FoobarFizzbizz");

            string sourceFileName = Path.GetFileName(sourceFile);
            var updateFile = new ApplicationUpdateFile { Description = "Foo", Name = sourceFileName, HttpAddress = sourceFile, Size = (int)new FileInfo(sourceFile).Length, UpdateType = (int)updateType };
            var update = new ApplicationUpdate
            {
                UpdateFiles = new List<ApplicationUpdateFile> { updateFile }
            };
            return new ApplicationUpdateModel(update);
        }

        private class MockDialogApplicationUpdatePresenter : ApplicationUpdatePresenter
        {
            public MockDialogApplicationUpdatePresenter(ApplicationUpdateModel model) : base(model, null, null, null)
            {

            }

            public MockDialogApplicationUpdatePresenter(ApplicationUpdateModel model, MessageBoxPresenter messageBox) : base(model, null, null, messageBox)
            {

            }

            public MockWin32Dialog MockDialog => Dialog as MockWin32Dialog;

            protected override IWin32Dialog OnCreateDialog() => new MockWin32Dialog();
        }

        private class MockFileDialogPresenterReturnsFileName : MockFileDialogPresenter
        {
            private readonly string _fileName;

            public MockFileDialogPresenterReturnsFileName(Func<IWin32Window, DialogResult> dialogResultProvider, string fileName) : base(dialogResultProvider)
            {
                _fileName = fileName;
            }

            public override string FileName
            {
                get => _fileName;
                set { }
            }
        }
    }
}
