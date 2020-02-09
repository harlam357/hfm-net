
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
         task.Changed += (s, e) => ReportAction(e);
         task.Add("task1", ct => Thread.Sleep(10), 100);
         task.Add("task2", ct => Thread.Sleep(10), 50);

         // stagger start tasks
         task.Start("task1");
         task.Start("task2");

         // allow the tasks some time to run
         Thread.Sleep(1000);

         // stop tasks
         task.Stop();

         // wait for completion
         Task.WaitAll(task.InnerScheduledTasks.Select(x => x.InnerTask).Where(x => x != null).ToArray());
      }

      [Test]
      public void AggregateScheduledTask_WithCancellation_Test()
      {
         // create and add tasks
         var task = new AggregateScheduledTask();
         task.Changed += (s, e) => ReportAction(e);
         task.Add("task1", ct =>
         {
             ct.ThrowIfCancellationRequested();
             Thread.Sleep(10);
         }, 100);
         task.Add("task2", ct =>
         {
             ct.ThrowIfCancellationRequested();
             Thread.Sleep(10);
         }, 50);

         // stagger start tasks
         task.Start("task1");
         task.Start("task2");

         // schedule a cancellation and wait
         Task.Run(() =>
         {
             Thread.Sleep(1000);
             task.Cancel();
         }).Wait();

         // wait for completion
         Task.WaitAll(task.InnerScheduledTasks.Select(x => x.InnerTask).Where(x => x != null).ToArray());
      }

      [Test]
      public void ScheduledTask_Test()
      {
         // create and start the task
         var task = new ScheduledTask("Task", ct => Thread.Sleep(10), 100);
         task.Changed += (s, e) => ReportAction(e);
         task.Start();

         // allow the task time to be scheduled and run
         Thread.Sleep(1000);

         // stop the task
         task.Stop();

         // wait for completion
         task.InnerTask?.Wait();
      }

      [Test]
      public void ScheduledTask_WithCancellation_Test()
      {
         // create and start the task
         var task = new ScheduledTask("Task", ct =>
         {
             ct.ThrowIfCancellationRequested();
             Thread.Sleep(10);
         }, 100);
         task.Changed += (s, e) => ReportAction(e);
         task.Start();

         // schedule a cancellation and wait
         Task.Run(() =>
         {
             Thread.Sleep(1000);
             task.Cancel();
         }).Wait();

         // wait for completion
         task.InnerTask?.Wait();
      }

      private static void ReportAction(ScheduledTaskChangedEventArgs e)
      {
         string message = null;
         switch (e.Action)
         {
            case ScheduledTaskChangedAction.Started:
               message = String.Format("{0} task scheduled: {1}ms", e.Key, e.Interval);
               break;
            case ScheduledTaskChangedAction.Stopped:
               message = String.Format("{0} task stopped", e.Key);
               break;
            case ScheduledTaskChangedAction.Running:
               message = String.Format("{0} task running", e.Key);
               break;
            case ScheduledTaskChangedAction.Canceled:
                message = String.Format("{0} task canceled", e.Key);
                break;
            case ScheduledTaskChangedAction.Finished:
               message = String.Format("{0} task finished: {1}ms", e.Key, e.Interval);
               break;
            case ScheduledTaskChangedAction.AlreadyInProgress:
               message = String.Format("{0} task already in progress", e.Key);
               break;
         }
         Assert.IsNotNull(message);
         Console.WriteLine(message);
      }
   }
}
