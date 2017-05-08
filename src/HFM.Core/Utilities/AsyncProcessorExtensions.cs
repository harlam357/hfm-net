
using System;
using System.Threading.Tasks;

using harlam357.Core.ComponentModel;

namespace HFM.Core
{
   public static class AsyncProcessorExtensions
   {
      public static Action<IAsyncProcessor, object> ExecuteAsyncWithProgressAction { get; set; }

      internal static async Task ExecuteAsyncWithProgress(this IAsyncProcessor processor, bool throwOnError)
      {
         await processor.ExecuteAsyncWithProgress(throwOnError, null).ConfigureAwait(false);
      }

      internal static async Task ExecuteAsyncWithProgress(this IAsyncProcessor processor, bool throwOnError, object state)
      {
         if (ExecuteAsyncWithProgressAction != null)
         {
            ExecuteAsyncWithProgressAction(processor, state);
         }
         else
         {
            await processor.ExecuteAsync(null).ConfigureAwait(false);
         }
         if (processor.Exception != null && throwOnError)
         {
            throw processor.Exception;
         }
      }
   }
}
