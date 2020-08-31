using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using HFM.Core.Logging;
using HFM.Preferences;

using NUnit.Framework;

namespace HFM.Forms.Models
{
    [TestFixture]
    public class MessagesModelTests
    {
        [Test]
        public void MessagesModel_ReceivesMessagesFromLoggerEvents()
        {
            // Arrange
            var loggerEvents = new MockLoggerEvents();
            var model = new MessagesModel(null, loggerEvents);
            // Act
            loggerEvents.RaiseLogged(new[] { "Foo", "Bar" });
            // Assert
            Assert.AreEqual(2, model.Messages.Count);
        }

        [Test]
        public void MessagesModel_LoggerEventsLoggedRaisesOneListChangedEvent()
        {
            // Arrange
            var loggerEvents = new MockLoggerEvents();
            var model = new MessagesModel(null, loggerEvents);
            int listChanged = 0;
            model.Messages.ListChanged += (s, e) => listChanged++;
            // Act
            loggerEvents.RaiseLogged(new[] { "Foo", "Bar" });
            // Assert
            Assert.AreEqual(1, listChanged);
        }

        [Test]
        public void MessagesModel_HoldsMaximumCapacityOfMessages()
        {
            // Arrange
            var loggerEvents = new MockLoggerEvents();
            var model = new MessagesModel(null, loggerEvents);
            int numberOfMessages = MessagesModel.MaxMessageCapacity + 1;
            var messages = Enumerable.Range(0, numberOfMessages)
                .Select(x => "Foo")
                .ToList();
            // Act
            loggerEvents.RaiseLogged(messages);
            // Assert
            Assert.AreNotEqual(numberOfMessages, model.Messages.Count);
            Assert.AreEqual(MessagesModel.MaxMessageCapacity, model.Messages.Count);
        }

        [Test]
        public void MessagesModel_ReplacesMessages()
        {
            // Arrange
            var loggerEvents = new MockLoggerEvents();
            var model = new MessagesModel(null, loggerEvents);
            int numberOfMessages = MessagesModel.MaxMessageCapacity / 2;
            for (int i = 0; i < 4; i++)
            {
                var messages = Enumerable.Range(0, numberOfMessages)
                    .Select(x => $"Foo{i}")
                    .ToList();
                // Act
                loggerEvents.RaiseLogged(messages);
            }
            // Assert
            IEnumerable<string> expected = Enumerable.Range(0, numberOfMessages)
                .Select(x => "Foo2")
                .Concat(Enumerable.Range(0, numberOfMessages)
                    .Select(x => "Foo3"));
            CollectionAssert.AreEqual(expected, model.Messages);
        }

        [Test]
        public void MessagesModel_Load_FormLocationAndSize()
        {
            // Arrange
            var preferences = new InMemoryPreferencesProvider();
            preferences.Set(Preference.MessagesFormLocation, new Point(10, 20));
            preferences.Set(Preference.MessagesFormSize, new Size(30, 40));
            var model = new MessagesModel(preferences, null);
            // Act
            model.Load();
            // Assert
            Assert.AreEqual(new Point(10, 20), model.FormLocation);
            Assert.AreEqual(new Size(30, 40), model.FormSize);
        }

        [Test]
        public void MessagesModel_Save_FormLocationAndSize()
        {
            // Arrange
            var preferences = new InMemoryPreferencesProvider();
            var model = new MessagesModel(preferences, null);
            model.FormLocation = new Point(50, 60);
            model.FormSize = new Size(70, 80);
            // Act
            model.Save();
            // Assert
            Assert.AreEqual(new Point(50, 60), preferences.Get<Point>(Preference.MessagesFormLocation));
            Assert.AreEqual(new Size(70, 80), preferences.Get<Size>(Preference.MessagesFormSize));
        }

        private class MockLoggerEvents : ILoggerEvents
        {
            public event EventHandler<LoggedEventArgs> Logged;

            public void RaiseLogged(ICollection<string> messages)
            {
                Logged?.Invoke(this, new LoggedEventArgs(messages));
            }
        }
    }
}
