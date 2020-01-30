
namespace HFM.Forms
{
   public interface IPresenterFactory
   {
      IFahClientSetupPresenter GetFahClientSetupPresenter();

      HistoryPresenter GetHistoryPresenter();

      void Release(object presenter);
   }
}
