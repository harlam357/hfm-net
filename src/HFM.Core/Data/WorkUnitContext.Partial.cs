using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace HFM.Core.Data
{
    public partial class WorkUnitContext
    {
        private const string VersionStringDefault = "0.0.0";

        public async Task<string> GetDatabaseVersion()
        {
            if (!await Database.CanConnectAsync().ConfigureAwait(false))
            {
                return null;
            }

            var versionRow = await Versions.OrderByDescending(x => x.ID).FirstOrDefaultAsync().ConfigureAwait(false);
            return versionRow?.Version ?? VersionStringDefault;
        }
    }
}
