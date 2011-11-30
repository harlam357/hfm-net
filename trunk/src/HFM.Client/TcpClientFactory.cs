
namespace HFM.Client
{
   public interface ITcpClientFactory
   {
      ITcpClient Create();
   }

   [CoverageExclude]
   public class TcpClientFactory : ITcpClientFactory
   {
      public ITcpClient Create()
      {
         return new TcpClientAdapter();
      }
   }
}
