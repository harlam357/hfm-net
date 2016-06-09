
using System.Collections.Generic;

namespace HFM.Log
{
   internal static class Extensions
   {
      internal static T PeekOrDefault<T>(this Stack<T> stack)
      {
         return stack.Count == 0 ? default(T) : stack.Peek();
      }
   }
}
