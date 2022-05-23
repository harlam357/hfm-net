using System.Net.Cache;

using HFM.Core.Net;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.WorkUnits;

public interface IProjectSummaryService
{
    ICollection<Protein> GetProteins(IProgress<ProgressInfo> progress);
}

public class ProjectSummaryService : IProjectSummaryService
{
    private readonly IPreferences _preferences;

    public ProjectSummaryService(IPreferences preferences)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
    }

    public ICollection<Protein> GetProteins(IProgress<ProgressInfo> progress)
    {
        var requestUri = new Uri(_preferences.Get<string>(Preference.ProjectDownloadUrl));
        var webOperation = WebOperation.Create(requestUri);
        if (progress != null)
        {
            webOperation.ProgressChanged += (sender, e) =>
            {
                int progressPercentage = Convert.ToInt32(e.Length / (double)e.TotalLength * 100);
                string message = $"Downloading {e.Length} of {e.TotalLength} bytes...";
                progress.Report(new ProgressInfo(progressPercentage, message));
            };
        }
        webOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        webOperation.WebRequest.Proxy = WebProxyFactory.Create(_preferences);
        using (var stream = new MemoryStream())
        {
            webOperation.Download(stream);
            stream.Position = 0;

            var serializer = new ProjectSummaryJsonDeserializer();
            return serializer.Deserialize(stream);
        }
    }
}
