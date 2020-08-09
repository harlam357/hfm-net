using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class MessagesModel : ViewModelBase
    {
        public IPreferenceSet Preferences { get; }
        public ILoggerEvents LoggerEvents { get; }

        public MessagesModel(IPreferenceSet preferences, ILoggerEvents loggerEvents)
        {
            Preferences = preferences ?? new InMemoryPreferenceSet();
            LoggerEvents = loggerEvents ?? NullLoggerEvents.Instance;
            LoggerEvents.Logged += (s, e) => AddMessage(e.Messages);
        }

        public override void Load()
        {
            FormLocation = Preferences.Get<Point>(Preference.MessagesFormLocation);
            FormSize = Preferences.Get<Size>(Preference.MessagesFormSize);
        }

        public override void Save()
        {
            Preferences.Set(Preference.MessagesFormLocation, FormLocation);
            Preferences.Set(Preference.MessagesFormSize, FormSize);
            Preferences.Save();
        }

        public Point FormLocation { get; set; }

        public Size FormSize { get; set; }

        public const int MaxMessageCapacity = 512;

        public BindingList<string> Messages { get; } = new BindingList<string>(new List<string>(MaxMessageCapacity));

        private void AddMessage(ICollection<string> messages)
        {
            if (messages.Count > MaxMessageCapacity)
            {
                messages = messages.Skip(messages.Count - MaxMessageCapacity).ToList();
            }

            int newLineCount = Messages.Count + messages.Count;
            int delta = newLineCount - MaxMessageCapacity;

            Messages.RaiseListChangedEvents = false;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                {
                    Messages.RemoveAt(0);
                }
            }

            foreach (var message in messages)
            {
                Messages.Add(message);
            }
            Messages.RaiseListChangedEvents = true;
            Messages.ResetBindings();
        }
    }
}
