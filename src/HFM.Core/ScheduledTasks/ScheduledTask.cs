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
        Faulted,
        Finished,
        AlreadyInProgress
    }

    public class ScheduledTaskChangedEventArgs : EventArgs
    {
        public ScheduledTask Source { get; }

        public ScheduledTaskChangedAction Action { get; }

        public double? Interval { get; }

        public ScheduledTaskChangedEventArgs(ScheduledTask source, ScheduledTaskChangedAction action, double? interval)
        {
            Source = source;
            Action = action;
            Interval = interval;
        }

        public override string ToString()
        {
            return ToString(i => $"{i:#,##0} ms");
        }

        public string ToString(Func<double?, string> formatInterval)
        {
            string key = Source.Name;
            switch (Action)
            {
                case ScheduledTaskChangedAction.Started:
                    return $"{key} task scheduled: {formatInterval(Interval)}";
                case ScheduledTaskChangedAction.Stopped:
                    return $"{key} task stopped";
                case ScheduledTaskChangedAction.Running:
                    return $"{key} task running";
                case ScheduledTaskChangedAction.Canceled:
                    return $"{key} task canceled";
                case ScheduledTaskChangedAction.Faulted:
                    return $"{key} task faulted: {Source.Exception}";
                case ScheduledTaskChangedAction.Finished:
                    return $"{key} task finished: {Interval:#,##0} ms";
                case ScheduledTaskChangedAction.AlreadyInProgress:
                    return $"{key} task already in progress";
                default:
                    return base.ToString();
            }
        }
    }

    public class DelegateScheduledTask : ScheduledTask
    {
        public Action<CancellationToken> Action { get; }

        public DelegateScheduledTask(string name, Action<CancellationToken> action, double interval) : base(name, interval)
        {
            Action = action;
        }

        protected override void OnRun(CancellationToken ct)
        {
            Action(ct);
        }
    }

    public abstract class ScheduledTask
    {
        private readonly System.Timers.Timer _timer;

        protected ScheduledTask(string name) : this(name, 100.0)
        {

        }

        protected ScheduledTask(string name, double interval)
        {
            Name = name;
            _timer = new System.Timers.Timer();
            _timer.Interval = interval;
            _timer.Elapsed += (s, e) => Run();
        }

        public event EventHandler<ScheduledTaskChangedEventArgs> Changed;

        protected virtual void OnTaskChanged(ScheduledTaskChangedAction action, double? interval = null)
        {
            Changed?.Invoke(this, new ScheduledTaskChangedEventArgs(this, action, interval));
        }

        public string Name { get; }

        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public bool Enabled { get; private set; }

        public bool InProgress => InnerTask != null && InnerTask.Status < TaskStatus.RanToCompletion;

        public Exception Exception { get; private set; }

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

            InnerTask = Task.Run(() => OnRun(_cts.Token), _cts.Token);
            InnerTask.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.Faulted:
                        Exception = t.Exception?.InnerException;
                        OnTaskChanged(ScheduledTaskChangedAction.Faulted);
                        break;
                    case TaskStatus.Canceled:
                        OnTaskChanged(ScheduledTaskChangedAction.Canceled);
                        break;
                    case TaskStatus.RanToCompletion:
                        OnTaskChanged(ScheduledTaskChangedAction.Finished, sw.ElapsedMilliseconds);
                        if (_cts.Token.IsCancellationRequested)
                        {
                            OnTaskChanged(ScheduledTaskChangedAction.Canceled);
                        }
                        else if (Enabled)
                        {
                            Start();
                        }
                        break;
                }
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
        }

        protected abstract void OnRun(CancellationToken ct);

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
            Stop();
        }
    }
}
