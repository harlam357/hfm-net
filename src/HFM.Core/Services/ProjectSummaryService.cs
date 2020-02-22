
using System;
using System.IO;
using System.Net.Cache;

using harlam357.Core;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Core.Services
{
    public interface IProjectSummaryService
    {
        /// <summary>
        /// Copies the project summary data to the given stream.
        /// </summary>
        void CopyToStream(Stream stream, IProgress<ProgressInfo> progress);
    }

    public class ProjectSummaryService : IProjectSummaryService
    {
        private readonly IPreferenceSet _prefs;

        public ProjectSummaryService(IPreferenceSet prefs)
        {
            _prefs = prefs ?? throw new ArgumentNullException(nameof(prefs));
        }
        
        public void CopyToStream(Stream stream, IProgress<ProgressInfo> progress)
        {
            var webOperation = WebOperation.Create(_prefs.Get<string>(Preference.ProjectDownloadUrl));
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
            webOperation.WebRequest.Proxy = _prefs.GetWebProxy();
            webOperation.Download(stream);
        }
    }
}