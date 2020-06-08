
namespace HFM.Forms
{
    public interface IViewFactory
    {
        IQueryView GetQueryDialog();

        IBenchmarksView GetBenchmarksForm();

        IPreferencesView GetPreferencesDialog();

        IProteinCalculatorView GetProteinCalculatorForm();

        void Release(object view);
    }
}
