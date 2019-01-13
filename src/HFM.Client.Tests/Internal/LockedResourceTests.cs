
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;
using Rhino.Mocks;

namespace HFM.Client.Internal
{
   [TestFixture]
   public class LockedResourceTests
   {
      public interface ITestResource
      {
         void Action();

         int Func();
      }

      public interface IDisposableTestResource : ITestResource, IDisposable
      {
         
      }

      [Test]
      public void LockedResource_DefaultValues_Test()
      {
         var locked = new LockedResource<ITestResource>();
         Assert.IsFalse(locked.IsAvailable);
      }

      [Test]
      public void LockedResource_Set_Resource_Test()
      {
         var locked = new LockedResource<ITestResource>();
         locked.Set(MockRepository.GenerateStub<ITestResource>());
         Assert.IsTrue(locked.IsAvailable);
      }

      [Test]
      public void LockedResource_Set_DisposableResource_Test()
      {
         var locked = new LockedResource<ITestResource>();
         locked.Set(MockRepository.GenerateStub<IDisposableTestResource>());
         Assert.IsTrue(locked.IsAvailable);
      }

      [Test]
      public void LockedResource_Set_Resource_Release_Resource_Test()
      {
         var locked = new LockedResource<ITestResource>();
         locked.Set(MockRepository.GenerateStub<ITestResource>());
         Assert.IsTrue(locked.IsAvailable);
         locked.Release();
         Assert.IsFalse(locked.IsAvailable);
      }

      [Test]
      public void LockedResource_Set_DisposableResource_Release_DisposableResource_Test()
      {
         var disposableResource = MockRepository.GenerateMock<IDisposableTestResource>();

         var locked = new LockedResource<ITestResource>();
         locked.Set(disposableResource);
         Assert.IsTrue(locked.IsAvailable);
         locked.Release();
         Assert.IsFalse(locked.IsAvailable);

         disposableResource.AssertWasCalled(x => x.Dispose());
      }

      [Test]
      public void LockedResource_Set_DisposableResource_Set_Resource_Test()
      {
         var disposableResource = MockRepository.GenerateMock<IDisposableTestResource>();

         var locked = new LockedResource<ITestResource>();
         locked.Set(disposableResource);
         Assert.IsTrue(locked.IsAvailable);
         locked.Set(MockRepository.GenerateStub<ITestResource>());
         Assert.IsTrue(locked.IsAvailable);
         
         disposableResource.AssertWasCalled(x => x.Dispose());
      }

      [Test]
      public void LockedResource_Set_Resource_Execute_Action_Release_Resource_Test()
      {
         var mre = new ManualResetEvent(false);
         var locked = new LockedResource<ITestResource>();
         var resource = MockRepository.GenerateMock<ITestResource>();

         resource.Expect(x => x.Action()).Do(new Action(() =>
         {
            locked.Release();
            mre.Set();
         })).Repeat.Once();

         locked.Set(resource);
         
         var tasks = new List<Task>();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Action())));
         mre.WaitOne();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => Assert.Fail())));
         
         Task.WaitAll(tasks.ToArray());
         resource.VerifyAllExpectations();
      }

      [Test]
      public void LockedResource_Set_Resource_Execute_Func_Release_Resource_Test()
      {
         var mre = new ManualResetEvent(false);
         var locked = new LockedResource<ITestResource>();
         var resource = MockRepository.GenerateMock<ITestResource>();

         resource.Expect(x => x.Func()).Do(new Func<int>(() =>
         {
            locked.Release();
            mre.Set();
            return 1;
         })).Repeat.Once();

         locked.Set(resource);

         var tasks = new List<Task>();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Func())));
         mre.WaitOne();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Func())));

         Task.WaitAll(tasks.ToArray());
         Assert.AreEqual(1, ((Task<int>)tasks[0]).Result);
         Assert.AreEqual(0, ((Task<int>)tasks[1]).Result);
         resource.VerifyAllExpectations();
      }

      [Test]
      public void LockedResource_Set_DisposableResource_Execute_Action_Release_Resource_Test()
      {
         var mre = new ManualResetEvent(false);
         var locked = new LockedResource<ITestResource>();
         var resource = MockRepository.GenerateMock<IDisposableTestResource>();

         resource.Expect(x => x.Action()).Do(new Action(() =>
         {
            locked.Release();
            mre.Set();
         })).Repeat.Once();

         locked.Set(resource);

         var tasks = new List<Task>();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Action())));
         mre.WaitOne();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => Assert.Fail())));

         Task.WaitAll(tasks.ToArray());
         resource.VerifyAllExpectations();
      }

      [Test]
      public void LockedResource_Set_DisposableResource_Execute_Func_Release_Resource_Test()
      {
         var mre = new ManualResetEvent(false);
         var locked = new LockedResource<ITestResource>();
         var resource = MockRepository.GenerateMock<IDisposableTestResource>();

         resource.Expect(x => x.Func()).Do(new Func<int>(() =>
         {
            locked.Release();
            mre.Set();
            return 1;
         })).Repeat.Once();

         locked.Set(resource);

         var tasks = new List<Task>();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Func())));
         mre.WaitOne();
         tasks.Add(Task.Factory.StartNew(() => locked.Execute(x => x.Func())));

         Task.WaitAll(tasks.ToArray());
         Assert.AreEqual(1, ((Task<int>)tasks[0]).Result);
         Assert.AreEqual(0, ((Task<int>)tasks[1]).Result);
         resource.VerifyAllExpectations();
      }
   }
}
