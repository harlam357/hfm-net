
using harlam357.Windows.Forms;

namespace HFM.Forms
{
   public interface IProgressDialogViewFactory
   {
      IProgressDialogView GetProgressDialog();

      IProgressDialogView GetProjectDownloadDialog();

      void Release(IProgressDialogView view);
   }
}
