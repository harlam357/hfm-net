
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Core.ScheduledTasks
{
    [TestFixture]
    public class ScheduledTaskTests
    {
        [Test]
        public void AggregateScheduledTask_Test()
        {
            // create and add tasks
            var task = new AggregateScheduledTask();
            task.Changed += TaskChanged;
            task.Add(new DelegateScheduledTask("task1", ct => Thread.Sleep(10), 100));
            task.Add(new DelegateScheduledTask("task2", ct => Thread.Sleep(10), 50));
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));

            // stagger start tasks
            task.Start("task1");
            task.Start("task2");
            Assert.IsTrue(task.InnerScheduledTasks.All(t => t.Enabled));

            // allow the tasks some time to run
            Thread.Sleep(1000);

            // stop tasks
            task.Stop();
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));

            // wait for completion
            Task.WaitAll(task.InnerScheduledTasks.Select(x => x.InnerTask).Where(x => x != null).ToArray());
        }

        [Test]
        public void AggregateScheduledTask_WithCancellation_Test()
        {
            // create and add tasks
            var task = new AggregateScheduledTask();
            task.Changed += TaskChanged;
            task.Add(new DelegateScheduledTask("task1", ct =>
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(10);
            }, 100));
            task.Add(new DelegateScheduledTask("task2", ct =>
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(10);
            }, 50));
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));

            // stagger start tasks
            task.Start("task1");
            task.Start("task2");
            Assert.IsTrue(task.InnerScheduledTasks.All(t => t.Enabled));

            // schedule a cancellation and wait
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                task.Cancel();
            }).GetAwaiter().GetResult();
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));
            
            // wait for completion
            Task.WaitAll(task.InnerScheduledTasks.Select(x => x.InnerTask).Where(x => x != null).ToArray());
        }

        [Test]
        public void AggregateScheduledTask_WithException_Test()
        {
            // create and add tasks
            var task = new AggregateScheduledTask();
            task.Changed += TaskChanged;
            task.Add(new DelegateScheduledTask("task1", ct =>
            {
                Thread.Sleep(10);
                throw new Exception("test exception");
            }, 100));
            task.Add(new DelegateScheduledTask("task2", ct =>
            {
                Thread.Sleep(10);
                throw new Exception("test exception");
            }, 50));
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));

            // stagger start tasks
            task.Start("task1");
            task.Start("task2");
            Assert.IsTrue(task.InnerScheduledTasks.All(t => t.Enabled));

            // allow the tasks some time to run
            Thread.Sleep(1000);

            // stop tasks
            task.Stop();
            Assert.IsTrue(task.InnerScheduledTasks.All(t => !t.Enabled));
            Assert.IsTrue(task.InnerScheduledTasks.All(t => t.Exception != null));

            try
            {
                // wait for completion
                Task.WaitAll(task.InnerScheduledTasks.Select(x => x.InnerTask).Where(x => x != null).ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Test]
        public void DelegateScheduledTask_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct => Thread.Sleep(10), 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            Thread.Sleep(1000);

            // stop the task
            task.Stop();
            Assert.IsFalse(task.Enabled);

            // wait for completion
            task.InnerTask?.GetAwaiter().GetResult();
        }

        [Test]
        public void DelegateScheduledTask_WithCancellation_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct =>
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(10);
            }, 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // schedule a cancellation and wait
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                task.Cancel();
            }).GetAwaiter().GetResult();
            Assert.IsFalse(task.Enabled);

            // wait for completion
            task.InnerTask?.GetAwaiter().GetResult();
        }

        [Test]
        public void DelegateScheduledTask_WithException_Test()
        {
            // create and start the task
            var task = new DelegateScheduledTask("Task", ct =>
            {
                Thread.Sleep(10);
                throw new Exception("test exception");
            }, 100);
            task.Changed += TaskChanged;
            Assert.IsFalse(task.Enabled);

            task.Start();
            Assert.IsTrue(task.Enabled);

            // allow the task time to be scheduled and run
            Thread.Sleep(1000);

            // stop the task
            task.Stop();
            Assert.IsFalse(task.Enabled);
            Assert.IsNotNull(task.Exception);

            try
            {
                // wait for completion
                task.InnerTask?.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void TaskChanged(object sender, ScheduledTaskChangedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
