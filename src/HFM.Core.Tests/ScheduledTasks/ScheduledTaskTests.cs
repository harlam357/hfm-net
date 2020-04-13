
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
