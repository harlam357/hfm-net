
using System.Collections.Generic;

namespace HFM.Log.Internal
{
   internal static class CollectionExtensions
   {
      /// <summary>
      /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey,TValue}" /> if the key does not already exist.
      /// </summary>
      /// <param name="dictionary">The dictionary.</param>
      /// <param name="key">The object to use as the key of the element to add.</param>
      /// <param name="value">The object to use as the value of the element to add.</param>
      /// <returns>true if the key did not already exist and the provided key and value were added; otherwise, false.</returns>
      internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
      {
         if (dictionary.ContainsKey(key))
         {
            return false;
         }
         dictionary.Add(key, value);
         return true;
      }
   }
}
