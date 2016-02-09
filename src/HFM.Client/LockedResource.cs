
using System;
using System.Diagnostics;

namespace HFM.Client
{
   internal class LockedResource<T> where T : class
   {
      private enum ThreadSafetyMode
      {
         ExecutionAndPublication,
         Publication
      }

      private readonly object _lockObject = new object();

      private T _value;
      private ThreadSafetyMode _mode;

      public bool IsAvailable
      {
         get
         {
            if (_mode == ThreadSafetyMode.ExecutionAndPublication)
            {
               lock (_lockObject)
               {
                  return _value != null;
               }
            }
            Debug.Assert(_mode == ThreadSafetyMode.Publication);
            return _value != null;
         }
      }

      public void Set(T value)
      {
         lock (_lockObject)
         {
            ReleaseInternal();
            _value = value;
            _mode = value is IDisposable ? ThreadSafetyMode.ExecutionAndPublication : ThreadSafetyMode.Publication;
         }
      }

      public void Release()
      {
         lock (_lockObject)
         {
            ReleaseInternal();
            _value = null;
         }
      }

      private void ReleaseInternal()
      {
         var v = _value;
         if (v == null)
         {
            return;
         }
         if (_mode == ThreadSafetyMode.ExecutionAndPublication)
         {
            ((IDisposable)v).Dispose();
         }
      }

      public void Execute(Action<T> action)
      {
         if (_mode == ThreadSafetyMode.ExecutionAndPublication)
         {
            lock (_lockObject)
            {
               var v = _value;
               if (v == null)
               {
                  return;
               }
               action(v);
            }
         }
         else
         {
            Debug.Assert(_mode == ThreadSafetyMode.Publication);
            T v;
            lock (_lockObject)
            {
               v = _value;
            }
            if (v == null)
            {
               return;
            }
            action(v);
         }
      }

      public TResult Execute<TResult>(Func<T, TResult> func)
      {
         if (_mode == ThreadSafetyMode.ExecutionAndPublication)
         {
            lock (_lockObject)
            {
               var v = _value;
               if (v == null)
               {
                  return default(TResult);
               }
               return func(v);
            }
         }
         else
         {
            Debug.Assert(_mode == ThreadSafetyMode.Publication);
            T v;
            lock (_lockObject)
            {
               v = _value;
            }
            if (v == null)
            {
               return default(TResult);
            }
            return func(v);
         }
      }
   }
}
