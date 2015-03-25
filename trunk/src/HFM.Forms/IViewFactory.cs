
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

      IPreferencesView GetPreferencesDialog();

      void Release(object view);
   }
}
