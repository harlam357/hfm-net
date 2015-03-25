
using harlam357.Windows.Forms;

namespace HFM.Forms
{
   public interface IViewFactory
   {
      IOpenFileDialogView GetOpenFileDialogView();

      ISaveFileDialogView GetSaveFileDialogView();

      IFolderBrowserView GetFolderBrowserView();

      IProgressDialogView GetProgressDialog();

      IProgressDialogAsyncView GetProgressDialogAsync();

      IProgressDialogAsyncView GetProjectDownloadDialog();

      IQueryView GetQueryDialog();

      IBenchmarksView GetBenchmarksForm();

      void Release(object view);
   }
}
