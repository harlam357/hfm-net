
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HFM.Core.ScheduledTasks
{
    public class AggregateScheduledTask
    {
        private readonly Dictionary<string, ScheduledTask> _tasks;

        public AggregateScheduledTask()
        {
            _tasks = new Dictionary<string, ScheduledTask>();
        }

        public event EventHandler<ScheduledTaskChangedEventArgs> Changed;

        private void OnTaskChanged(object s, ScheduledTaskChangedEventArgs e)
        {
            Changed?.Invoke(s, e);
        }

        public bool Enabled
        {
            get { return _tasks.Values.Any(x => x.Enabled); }
        }

        public bool InProgress
        {
            get { return _tasks.Values.Any(x => x.InProgress); }
        }

        public void Run()
        {
            Run(true);
        }

        public void Run(bool enabled)
        {
            foreach (var task in _tasks.Values)
            {
                task.Run(enabled);
            }
        }

        public void Run(string key)
        {
            Run(key, true);
        }

        public void Run(string key, bool enabled)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Run(enabled);
            }
        }

        public void Run(string key, double interval)
        {
            Run(key, interval, true);
        }

        public void Run(string key, double interval, bool enabled)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Interval = interval;
                _tasks[key].Run(enabled);
            }
        }

        public void Start()
        {
            foreach (var task in _tasks.Values)
            {
                task.Start();
            }
        }

        public void Start(string key)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Start();
            }
        }

        public void Start(string key, double interval)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Interval = interval;
                _tasks[key].Start();
            }
        }

        public void Restart()
        {
            foreach (var task in _tasks.Values)
            {
                task.Restart();
            }
        }

        public void Restart(string key)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Restart();
            }
        }

        public void Restart(string key, double interval)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Interval = interval;
                _tasks[key].Restart();
            }
        }

        public void Stop()
        {
            foreach (var task in _tasks.Values)
            {
                task.Stop();
            }
        }

        public void Stop(string key)
        {
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Stop();
            }
        }

        public void Cancel()
        {
            foreach (var task in _tasks.Values)
            {
                task.Cancel();
            }
        }

        public IEnumerable<ScheduledTask> InnerScheduledTasks => _tasks.Values;
        
        public void Add(ScheduledTask scheduledTask)
        {
            string key = scheduledTask.Key;
            if (_tasks.ContainsKey(key))
            {
                _tasks[key].Stop();
                _tasks[key].Changed -= OnTaskChanged;
            }
            scheduledTask.Changed += OnTaskChanged;
            _tasks[key] = scheduledTask;
        }
    }
}
