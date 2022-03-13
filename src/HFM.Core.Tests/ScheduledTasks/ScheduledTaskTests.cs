using HFM.Core.Logging;

using NUnit.Framework;

namespace HFM.Core.ScheduledTasks
{
    [TestFixture]
    public class ScheduledTaskTests
    {
        private static ILogger Logger { get; } = TestLogger.Instance;

        [Test]
        [Ignore("Hanging on CI build")]
        public async Task DelegateScheduledTask_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct => Thread.Sleep(10), 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            await Task.Delay(1000);

            // stop the task
            task.Stop();
            Assert.IsFalse(task.Enabled);

            // wait for completion
            await task.InnerTask;
        }

        [Test]
        public async Task DelegateScheduledTask_WithCancellation_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct =>
            {
                while (!ct.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    ct.ThrowIfCancellationRequested();
                }
            }, 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            await Task.Delay(1000);

            // cancel the task
            task.Cancel();
            Assert.IsFalse(task.Enabled);

            try
            {
                // wait for completion
                await task.InnerTask;
            }
            catch (OperationCanceledException ex)
            {
                Logger.Debug(ex.ToString());
            }
        }

        [Test]
        public async Task DelegateScheduledTask_CancelAfterCompletion_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct =>
            {
                Thread.Sleep(1000);
            }, 10);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            await Task.Delay(100);

            // cancel the task
            task.Cancel();
            Assert.IsFalse(task.Enabled);

            // wait for completion
            await task.InnerTask;
        }

        [Test]
        public async Task DelegateScheduledTask_WithException_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTaskWithRunCount("Task", ct =>
            {
                Thread.Sleep(10);
                throw new Exception("test exception");
            }, 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            await Task.Delay(1000);

            // stop the task
            task.Stop();
            Assert.IsFalse(task.Enabled);
            Assert.IsNotNull(task.Exception);

            try
            {
                // wait for completion
                await task.InnerTask;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString());
            }

            Assert.IsTrue(task.RunCount > 1, "RunCount should be greater than 1");
        }

        private static void TaskChanged(object sender, ScheduledTaskChangedEventArgs e)
        {
            Logger.Debug(e.ToString());
        }

        private class DelegateScheduledTaskWithRunCount : DelegateScheduledTask
        {
            public DelegateScheduledTaskWithRunCount(string name, Action<CancellationToken> action, double interval) : base(name, action, interval)
            {
            }

            public int RunCount { get; set; }

            protected override void OnRun(CancellationToken ct)
            {
                RunCount++;
                base.OnRun(ct);
            }
        }
    }
}
