using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Presenters
{
    public class NullServiceScopeFactory : IServiceScopeFactory
    {
        public static NullServiceScopeFactory Instance { get; } = new NullServiceScopeFactory();

        public IServiceScope CreateScope() => null;
    }
}
