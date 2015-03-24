
using harlam357.Windows.Forms;

namespace HFM.Forms
{
   // TODO: Rename source file
   public interface IViewFactory
   {
      IProgressDialogView GetProgressDialog();

      IProgressDialogAsyncView GetProgressDialogAsync();

      IProgressDialogAsyncView GetProjectDownloadDialog();

      void Release(object view);
   }
}
