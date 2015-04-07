
namespace HFM.Forms
{
   public interface IPresenterFactory
   {
      IFahClientSetupPresenter GetFahClientSetupPresenter();

      ILegacyClientSetupPresenter GetLegacyClientSetupPresenter();

      HistoryPresenter GetHistoryPresenter();

      void Release(object presenter);
   }
}
