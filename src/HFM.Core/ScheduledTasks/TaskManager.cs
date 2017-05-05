
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HFM.Core.ScheduledTasks
{
   public class TaskManager
   {
      private readonly Dictionary<string, ScheduledTask> _tasks;

      public TaskManager()
      {
         _tasks = new Dictionary<string, ScheduledTask>();
      }

      public event EventHandler<ScheduledTaskChangedEventArgs> Changed;

      private void OnTaskChanged(object s, ScheduledTaskChangedEventArgs e)
      {
         var handler = Changed;
         if (handler != null)
         {
            handler(s, e);
         }
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

      internal void Wait()
      {
         Task.WaitAll(_tasks.Values.Select(x => x.InnerTask).Where(x => x != null).ToArray());
      }

      public ScheduledTask Add(string key, Action<CancellationToken> action, double interval)
      {
         if (_tasks.ContainsKey(key))
         {
            _tasks[key].Stop();
            _tasks[key].Changed -= OnTaskChanged;
         }
         var task = new ScheduledTask(key, action, interval);
         task.Changed += OnTaskChanged;
         _tasks[key] = task;
         return task;
      }
   }

   public enum ScheduledTaskChangedAction
   {
      Started,
      Stopped,
      Running,
      Finished,
      AlreadyInProgress
   }

   public class ScheduledTask
   {
      private readonly string _key;
      private readonly Action<CancellationToken> _action;
      private readonly System.Timers.Timer _timer;

      public ScheduledTask(Action<CancellationToken> action, double interval)
         : this(null, action, interval)
      {

      }

      public ScheduledTask(string key, Action<CancellationToken> action, double interval)
      {
         _key = key;
         _action = action;
         _timer = new System.Timers.Timer();
         _timer.Interval = interval;
         _timer.Elapsed += (s, e) => Run();
      }

      public event EventHandler<ScheduledTaskChangedEventArgs> Changed;

      private void OnTaskChanged(ScheduledTaskChangedAction action, double? interval = null)
      {
         var handler = Changed;
         if (handler != null)
         {
            handler(this, new ScheduledTaskChangedEventArgs(_key, action, interval));
         }
      }

      public string Key
      {
         get { return _key; }
      }

      public double Interval
      {
         get { return _timer.Interval; }
         set { _timer.Interval = value; }
      }

      public bool Enabled { get; private set; }

      public bool InProgress
      {
         get { return _innerTask != null && _innerTask.Status < TaskStatus.RanToCompletion; }
      }

      private Task _innerTask;

      internal Task InnerTask
      {
         get { return _innerTask; }
      }

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
         if (lastCts != null)
         {
            lastCts.Dispose();
         }

         _innerTask = Task.Factory.StartNew(() => _action(_cts.Token), _cts.Token);
         _innerTask.ContinueWith(t =>
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
         if (_cts != null)
         {
            _cts.Cancel();
         }
         Stop();
      }

      internal void Wait()
      {
         if (_innerTask != null)
         {
            _innerTask.Wait();
         }
      }
   }

   public sealed class ScheduledTaskChangedEventArgs : EventArgs
   {
      private readonly string _key;
      private readonly ScheduledTaskChangedAction _action;
      private readonly double? _interval;

      public string Key
      {
         get { return _key; }
      }

      public ScheduledTaskChangedAction Action
      {
         get { return _action; }
      }

      public double? Interval
      {
         get { return _interval; }
      }

      public ScheduledTaskChangedEventArgs(string key, ScheduledTaskChangedAction action, double? interval)
      {
         _key = key;
         _action = action;
         _interval = interval;
      }
   }
}
