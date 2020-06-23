
namespace HFM.Forms
{
    public interface IViewFactory
    {
        IQueryView GetQueryDialog();

        IBenchmarksView GetBenchmarksForm();

        IProteinCalculatorView GetProteinCalculatorForm();

        void Release(object view);
    }
}
