
using System;
using System.Collections.Concurrent;
using System.Threading;

// http://stackoverflow.com/questions/7669666/what-is-the-correct-way-to-dispose-elements-held-inside-a-threadlocalidisposabl

namespace HFM.Core
{
   public class ThreadLocalDisposable<T> : IDisposable where T : IDisposable
   {
      private readonly ThreadLocal<T> _threadLocal;
      private readonly ConcurrentBag<T> _values = new ConcurrentBag<T>();

      public ThreadLocalDisposable(Func<T> valueFactory)
      {
         _threadLocal = new ThreadLocal<T>(() =>
         {
            var value = valueFactory();
            _values.Add(value);
            return value;
         });
      }

      public void Dispose()
      {
         _threadLocal.Dispose();
         Array.ForEach(_values.ToArray(), t => t.Dispose());
      }

      public override string ToString()
      {
         return _threadLocal.ToString();
      }

      public bool IsValueCreated
      {
         get { return _threadLocal.IsValueCreated; }
      }

      public T Value
      {
         get { return _threadLocal.Value; }
      }
   }
}
