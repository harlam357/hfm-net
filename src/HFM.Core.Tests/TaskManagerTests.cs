
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class TaskManagerTests
   {
      [Test]
      public void TaskManager_Test()
      {
         // create manager and add tasks
         var manager = new TaskManager();
         manager.Changed += (s, e) => ReportAction(e);
         manager.Add("task1", ct => Thread.Sleep(500), 100);
         manager.Add("task2", ct => Thread.Sleep(500), 50);
         // stagger start tasks
         manager.Start("task1");
         Thread.Sleep(2000);
         manager.Start("task2");
         Thread.Sleep(3000);
         // stop tasks
         manager.Stop();
         // wait for completion (only in unit tests)
         manager.Wait();
      }

      [Test]
      public void TaskManager_WithCancellation_Test()
      {
         // create manager and add tasks
         var manager = new TaskManager();
         manager.Changed += (s, e) => ReportAction(e);
         manager.Add("task1", ct => Thread.Sleep(500), 100);
         manager.Add("task2", ct => Thread.Sleep(500), 50);
         // stagger start tasks
         manager.Start("task1");
         // schedule a cancellation in 2.5 to 5 seconds
         var cancelTask = Task.Factory.StartNew(() =>
         {
            var random = new Random();
            Thread.Sleep(random.Next(2500, 5000));
            manager.Cancel();
         });
         Thread.Sleep(2000);
         manager.Start("task2");
         Thread.Sleep(3000);
         // stop tasks
         manager.Stop();
         // wait for completion (only in unit tests)
         manager.Wait();
         // wait for completion of cancellation task
         cancelTask.Wait();
      }

      [Test]
      public void ScheduledTask_Test()
      {
         // create and start the task
         var task = new ScheduledTask("Task", ct => Thread.Sleep(500), 100);
         task.Changed += (s, e) => ReportAction(e);
         task.Start();
         // allow the task time to be scheduled and run
         Thread.Sleep(5000);
         // stop the task
         task.Stop();
         // wait for completion (only in unit tests)
         task.Wait();
      }

      [Test]
      public void ScheduledTask_WithCancellation_Test()
      {
         // create and start the task
         var task = new ScheduledTask("Task", ct => Thread.Sleep(500), 100);
         task.Changed += (s, e) => ReportAction(e);
         task.Start();
         // schedule a cancellation in 1 to 5 seconds
         var cancelTask = Task.Factory.StartNew(() =>
         {
            var random = new Random();
            Thread.Sleep(random.Next(1000, 5000));
            task.Cancel();
         });
         // allow the task time to be scheduled and run
         Thread.Sleep(5000);
         // stop the task
         task.Stop();
         // wait for completion (only in unit tests)
         task.Wait();
         // wait for completion of cancellation task
         cancelTask.Wait();
      }

      private static void ReportAction(ScheduledTaskChangedEventArges e)
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
            case ScheduledTaskChangedAction.Finished:
               message = String.Format("{0} task finished: {1}ms", e.Key, e.Interval);
               break;
            case ScheduledTaskChangedAction.AlreadyInProgress:
               message = String.Format("{0} task already in progress", e.Key);
               break;
         }
         Assert.IsNotNull(message);
         Debug.WriteLine(message);
      }
   }
}
