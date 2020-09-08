using System.Windows.Forms;
using HFM.Core.Net;
using HFM.Forms.Models;
using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Forms.Controls
{
    [TestFixture]
    public class RadioPanelTests
    {
        [Test]
        public void RadioPanel_SetBindingProperties()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            var panel = new TestRadioPanel();
            // Act
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Assert
            Assert.IsTrue(panel.HasDataSource);
            Assert.IsTrue(panel.HasDataSourcePropertyChangedEventInfo);
            Assert.IsTrue(panel.HasDataSourceValueMemberPropertyInfo);
        }

        [Test]
        public void RadioPanel_SetValueMemberNull()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            var panel = new TestRadioPanel();
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Act
            panel.ValueMember = null;
            // Assert
            Assert.IsTrue(panel.HasDataSource);
            Assert.IsTrue(panel.HasDataSourcePropertyChangedEventInfo);
            Assert.IsFalse(panel.HasDataSourceValueMemberPropertyInfo);
        }

        [Test]
        public void RadioPanel_SetDataSourceNull()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            var panel = new TestRadioPanel();
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Act
            panel.DataSource = null;
            // Assert
            Assert.IsFalse(panel.HasDataSource);
            Assert.IsFalse(panel.HasDataSourcePropertyChangedEventInfo);
            Assert.IsFalse(panel.HasDataSourceValueMemberPropertyInfo);
        }

        [Test]
        public void RadioPanel_InitializeRadioButtonCheckedFromInt32()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            model.FtpMode = FtpMode.Active;
            var panel = new TestRadioPanel();
            panel.Controls.Add(new RadioButton { Tag = (int)FtpMode.Passive });
            panel.Controls.Add(new RadioButton { Tag = (int)FtpMode.Active });
            // Act
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Assert
            Assert.IsFalse(((RadioButton)panel.Controls[0]).Checked);
            Assert.IsTrue(((RadioButton)panel.Controls[1]).Checked);
        }

        [Test]
        public void RadioPanel_SetsControlsTaggedWithInt32CheckedFromDataSource()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            model.FtpMode = FtpMode.Active;
            var panel = new TestRadioPanel();
            panel.Controls.Add(new RadioButton { Tag = (int)FtpMode.Passive });
            panel.Controls.Add(new RadioButton { Tag = (int)FtpMode.Active });
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Act
            model.FtpMode = FtpMode.Passive;
            // Assert
            Assert.IsTrue(((RadioButton)panel.Controls[0]).Checked);
            Assert.IsFalse(((RadioButton)panel.Controls[1]).Checked);
        }

        [Test]
        public void RadioPanel_InitializeRadioButtonCheckedFromString()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            model.FtpMode = FtpMode.Active;
            var panel = new TestRadioPanel();
            panel.Controls.Add(new RadioButton { Tag = ((int)FtpMode.Passive).ToString() });
            panel.Controls.Add(new RadioButton { Tag = ((int)FtpMode.Active).ToString() });
            // Act
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Assert
            Assert.IsFalse(((RadioButton)panel.Controls[0]).Checked);
            Assert.IsTrue(((RadioButton)panel.Controls[1]).Checked);
        }

        [Test]
        public void RadioPanel_SetsControlsTaggedWithStringCheckedFromDataSource()
        {
            // Arrange
            var model = new WebGenerationModel(new InMemoryPreferencesProvider());
            model.FtpMode = FtpMode.Active;
            var panel = new TestRadioPanel();
            panel.Controls.Add(new RadioButton { Tag = ((int)FtpMode.Passive).ToString() });
            panel.Controls.Add(new RadioButton { Tag = ((int)FtpMode.Active).ToString() });
            panel.DataSource = model;
            panel.ValueMember = nameof(WebGenerationModel.FtpMode);
            // Act
            model.FtpMode = FtpMode.Passive;
            // Assert
            Assert.IsTrue(((RadioButton)panel.Controls[0]).Checked);
            Assert.IsFalse(((RadioButton)panel.Controls[1]).Checked);
        }

        private class TestRadioPanel : RadioPanel
        {
            public bool HasDataSource => DataSource != null;
            public bool HasDataSourcePropertyChangedEventInfo => DataSourcePropertyChangedEventInfo != null;
            public bool HasDataSourceValueMemberPropertyInfo => DataSourceValueMemberPropertyInfo != null;
        }
    }
}
