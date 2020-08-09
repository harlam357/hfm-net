
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    public interface IProteinService
    {
        /// <summary>
        /// Gets the protein with the given project ID.
        /// </summary>
        /// <param name="projectID">The project ID of the protein to return.</param>
        /// <returns>The protein object with the given project ID or null if the protein does not exist.</returns>
        Protein Get(int projectID);

        /// <summary>
        /// Gets the protein with the given project ID or refreshes the service data from the project summary.
        /// </summary>
        /// <param name="projectID">The project ID of the protein to return.</param>
        /// <returns>The protein object with the given project ID or null if the protein does not exist.</returns>
        Protein GetOrRefresh(int projectID);

        /// <summary>
        /// Gets a collection of all protein project ID numbers.
        /// </summary>
        /// <returns>A collection of all protein project ID numbers.</returns>
        IReadOnlyCollection<int> GetProjects();

        /// <summary>
        /// Refreshes the service data from the project summary and returns a collection of objects detailing how the service data was changed.
        /// </summary>
        /// <param name="progress">The object used to report refresh progress.</param>
        /// <returns>A collection of objects detailing how the service data was changed</returns>
        IReadOnlyCollection<ProteinDictionaryChange> Refresh(IProgress<ProgressInfo> progress);
    }

    public class ProteinService : IProteinService
    {
        private readonly ProteinDataContainer _dataContainer;
        private readonly IProjectSummaryService _projectSummaryService;
        private readonly ILogger _logger;
        private ProteinDictionary _dictionary;

        public ProteinService(ProteinDataContainer dataContainer, IProjectSummaryService projectSummaryService, ILogger logger)
        {
            _dataContainer = dataContainer;
            _projectSummaryService = projectSummaryService;
            _logger = logger ?? NullLogger.Instance;
            _dictionary = CreateProteinDictionary(_dataContainer.Data);

            LastProjectRefresh = new Dictionary<int, DateTime>();
        }

        private static ProteinDictionary CreateProteinDictionary(IEnumerable<Protein> proteins)
        {
            var dictionary = new ProteinDictionary();
            foreach (var p in proteins)
            {
                dictionary.Add(p.ProjectNumber, p);
            }
            return dictionary;
        }

        public Protein Get(int projectID)
        {
            return _dictionary.TryGetValue(projectID, out Protein p) ? p : null;
        }

        public IReadOnlyCollection<int> GetProjects()
        {
            return _dictionary.Keys;
        }

        public Protein GetOrRefresh(int projectID)
        {
            if (_dictionary.TryGetValue(projectID, out Protein p))
            {
                return p;
            }

            if (_projectSummaryService is null || ProjectIDIsNotValid(projectID) || AutoRefreshIsNotAvailable(projectID))
            {
                return null;
            }

            _logger.Info($"Project ID {projectID} triggering project data refresh.");
            try
            {
                Refresh(null);
                if (_dictionary.TryGetValue(projectID, out p))
                {
                    return p;
                }
                LastProjectRefresh[projectID] = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return null;
        }

        private static bool ProjectIDIsNotValid(int projectID)
        {
            return projectID <= 0;
        }

        public IReadOnlyCollection<ProteinDictionaryChange> Refresh(IProgress<ProgressInfo> progress)
        {
            using (var stream = new MemoryStream())
            {
                _logger.Info("Downloading new project data from Stanford...");
                _projectSummaryService.CopyToStream(stream, progress);
                stream.Position = 0;

                var serializer = new ProjectSummaryJsonDeserializer();
                _dictionary = ProteinDictionary.CreateFromExisting(_dictionary, serializer.Deserialize(stream));
            }

            foreach (var info in _dictionary.Changes.Where(info => info.Result != ProteinDictionaryChangeResult.NoChange))
            {
                _logger.Info(info.ToString());
            }

            var now = DateTime.UtcNow;
            foreach (var key in LastProjectRefresh.Keys.ToList())
            {
                if (_dictionary.ContainsKey(key))
                {
                    LastProjectRefresh.Remove(key);
                }
                else
                {
                    LastProjectRefresh[key] = now;
                }
            }
            LastRefresh = now;

            Write();
            return _dictionary.Changes;
        }

        private void Write()
        {
            _dataContainer.Data = _dictionary.Values.ToList();
            _dataContainer.Write();
        }

        /// <summary>
        /// Gets the dictionary that contains previously queried project ID numbers that were not found and the date and time of the query.
        /// </summary>
        internal IDictionary<int, DateTime> LastProjectRefresh { get; }

        internal DateTime? LastRefresh { get; set; }

        private bool AutoRefreshIsNotAvailable(int projectID)
        {
            var utcNow = DateTime.UtcNow;

            if (LastRefresh.HasValue)
            {
                const double canRefreshAfterHours = 1.0;
                TimeSpan lastRefreshDifference = utcNow.Subtract(LastRefresh.Value);
                if (lastRefreshDifference.TotalHours < canRefreshAfterHours)
                {
                    _logger.Debug($"Refresh executed {lastRefreshDifference.TotalMinutes:0} minutes ago.");
                    return true;
                }
            }

            if (LastProjectRefresh.ContainsKey(projectID))
            {
                const double canRefreshAfterHours = 24.0;
                TimeSpan lastRefreshDifference = utcNow.Subtract(LastProjectRefresh[projectID]);
                if (lastRefreshDifference.TotalHours < canRefreshAfterHours)
                {
                    _logger.Debug($"Project {projectID} refresh executed {lastRefreshDifference.TotalMinutes:0} minutes ago.");
                    return true;
                }
            }

            return false;
        }
    }

    public class NullProteinService : IProteinService
    {
        public static NullProteinService Instance { get; } = new NullProteinService();

        public Protein Get(int projectID)
        {
            return null;
        }

        public Protein GetOrRefresh(int projectID)
        {
            return null;
        }

        public IReadOnlyCollection<int> GetProjects()
        {
            return new List<int>(0);
        }

        public IReadOnlyCollection<ProteinDictionaryChange> Refresh(IProgress<ProgressInfo> progress)
        {
            return new List<ProteinDictionaryChange>(0);
        }
    }
}
