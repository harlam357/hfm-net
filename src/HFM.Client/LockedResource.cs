
using System;

namespace HFM.Client
{
   internal class LockedResource<T> where T : class
   {
      private readonly object _lockObject = new object();

      private T _value;
      private bool _isAvailable;

      public bool IsAvailable
      {
         get { return _isAvailable; }
      }

      public void Set(T value)
      {
         lock (_lockObject)
         {
            _value = value;
            _isAvailable = value != null;
         }
      }

      //public void Release()
      //{
      //   Release(null);
      //}

      public void Release(Action<T> releaseAction)
      {
         lock (_lockObject)
         {
            if (_value == null)
            {
               return;
            }
            if (releaseAction != null)
            {
               releaseAction(_value);
            }
            _isAvailable = false;
            _value = null;
         }
      }

      public void Execute(Action<T> action)
      {
         lock (_lockObject)
         {
            if (_value == null)
            {
               return;
            }
            action(_value);
         }
      }

      public TResult Execute<TResult>(Func<T, TResult> func)
      {
         lock (_lockObject)
         {
            if (_value == null)
            {
               return default(TResult);
            }
            return func(_value);
         }
      }
   }
}
