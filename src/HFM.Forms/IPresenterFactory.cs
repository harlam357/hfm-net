
namespace HFM.Forms
{
    public interface IPresenterFactory
    {
        HistoryPresenter GetHistoryPresenter();

        void Release(object presenter);
    }
}
