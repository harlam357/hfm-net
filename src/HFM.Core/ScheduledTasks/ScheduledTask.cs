
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HFM.Core.ScheduledTasks
{
    public enum ScheduledTaskChangedAction
    {
        Started,
        Stopped,
        Running,
        Canceled,
        Finished,
        AlreadyInProgress
    }

    public sealed class ScheduledTaskChangedEventArgs : EventArgs
    {
        public string Key { get; }

        public ScheduledTaskChangedAction Action { get; }

        public double? Interval { get; }

        public ScheduledTaskChangedEventArgs(string key, ScheduledTaskChangedAction action, double? interval)
        {
            Key = key;
            Action = action;
            Interval = interval;
        }
    }

    public class ScheduledTask
    {
        private readonly System.Timers.Timer _timer;

        public ScheduledTask(Action<CancellationToken> action, double interval)
            : this(null, action, interval)
        {

        }

        public ScheduledTask(string key, Action<CancellationToken> action, double interval)
        {
            Key = key;
            Action = action;
            _timer = new System.Timers.Timer();
            _timer.Interval = interval;
            _timer.Elapsed += (s, e) => Run();
        }

        public event EventHandler<ScheduledTaskChangedEventArgs> Changed;

        private void OnTaskChanged(ScheduledTaskChangedAction action, double? interval = null)
        {
            Changed?.Invoke(this, new ScheduledTaskChangedEventArgs(Key, action, interval));
        }

        public string Key { get; }

        public Action<CancellationToken> Action { get; }

        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public bool Enabled { get; private set; }

        public bool InProgress => InnerTask != null && InnerTask.Status < TaskStatus.RanToCompletion;

        internal Task InnerTask { get; private set; }

        private CancellationTokenSource _cts;

        public void Run()
        {
            Run(true);
        }

        public void Run(bool enabled)
        {
            Enabled |= enabled;
            if (InProgress)
            {
                OnTaskChanged(ScheduledTaskChangedAction.AlreadyInProgress);
                return;
            }

            var sw = Stopwatch.StartNew();

            _timer.Stop();
            OnTaskChanged(ScheduledTaskChangedAction.Running);

            var lastCts = Interlocked.Exchange(ref _cts, new CancellationTokenSource());
            lastCts?.Dispose();

            InnerTask = Task.Run(() => Action(_cts.Token), _cts.Token);
            InnerTask.ContinueWith(t =>
            {
                _cts.Token.ThrowIfCancellationRequested();
                OnTaskChanged(ScheduledTaskChangedAction.Finished, sw.ElapsedMilliseconds);
                if (Enabled)
                {
                    _cts.Token.ThrowIfCancellationRequested();
                    Start();
                }
            }, _cts.Token);
        }

        public void Start()
        {
            Enabled = true;
            if (InProgress)
            {
                OnTaskChanged(ScheduledTaskChangedAction.AlreadyInProgress);
                return;
            }

            if (!_timer.Enabled)
            {
                _timer.Start();
                OnTaskChanged(ScheduledTaskChangedAction.Started, _timer.Interval);
            }
        }

        public void Restart()
        {
            Enabled = true;
            if (InProgress)
            {
                OnTaskChanged(ScheduledTaskChangedAction.AlreadyInProgress);
                return;
            }

            if (_timer.Enabled)
            {
                _timer.Stop();
            }
            _timer.Start();
            OnTaskChanged(ScheduledTaskChangedAction.Started, _timer.Interval);
        }

        public void Stop()
        {
            Enabled = false;
            if (_timer.Enabled)
            {
                _timer.Stop();
                OnTaskChanged(ScheduledTaskChangedAction.Stopped);
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
            OnTaskChanged(ScheduledTaskChangedAction.Canceled);
            Stop();
        }
    }
}