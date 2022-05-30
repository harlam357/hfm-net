using HFM.Core.Logging;
using HFM.Core.Services;

namespace HFM.Forms.Services
{
    internal static class FahUserServiceExtensions
    {
        internal static async Task<FahUser> FindUserAndLogError(this FahUserService userService, string name, ILogger logger)
        {
            try
            {
                return await userService.FindUser(name).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return new FahUser();
        }
    }
}
