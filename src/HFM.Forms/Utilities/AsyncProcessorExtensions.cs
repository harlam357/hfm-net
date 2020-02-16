
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using harlam357.Core.ComponentModel;
using harlam357.Windows.Forms;

using HFM.Core.Data;

namespace HFM.Forms
{
   internal static class AsyncProcessorExtensions
   {
      internal static Task ExecuteAsyncWithProgress(this IAsyncProcessor processor, bool throwOnError)
      {
         return processor.ExecuteAsyncWithProgress(throwOnError, null);
      }

      internal static Task ExecuteAsyncWithProgress(this IAsyncProcessor processor, bool throwOnError, object state)
      {
         using (IProgressDialogAsyncView dialog = new ProgressDialogAsync())
         {
            dialog.AsyncProcessor = processor;
            dialog.Icon = Properties.Resources.hfm_48_48;
            dialog.Text = Core.Application.NameAndVersion;
            var owner = GetOwnerFromState(state);
            if (owner == null)
            {
               dialog.StartPosition = FormStartPosition.CenterScreen;
            }
            dialog.ShowDialog(owner);
         }
         if (processor.Exception != null && throwOnError)
         {
            throw processor.Exception;
         }
         return Task.FromResult<object>(null);
      }

      private static IWin32Window GetOwnerFromState(object state)
      {
         var openForms = Application.OpenForms.OfType<Form>().ToList();

         if (state is WorkUnitRepository)
         {
            var historyView = openForms.Find(x => x is IHistoryView);
            if (historyView != null)
            {
               return historyView;
            }
         }

         var win32Window = state as IWin32Window;
         if (win32Window != null)
         {
            return win32Window;
         }

         return openForms.Find(x => x is IMainView);
      }
   }
}
