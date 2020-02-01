/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Cache;

using harlam357.Core;

using HFM.Core.Data;
using HFM.Core.Net;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core
{
   public interface IProteinService
   {
      /// <summary>
      /// Gets the protein with the given project id.
      /// </summary>
      /// <param name="projectId">The project id of the protein to return.</param>
      /// <returns>The protein object with the given project id or null if the protein does not exist.</returns>
      Protein Get(int projectId);

      /// <summary>
      /// Gets the protein with the given project id.
      /// </summary>
      /// <param name="projectId">The project id of the protein to return.</param>
      /// <param name="allowRefresh">true to allow this Get method to refresh the service.</param>
      /// <returns>The protein object with the given project id or null if the protein does not exist.</returns>
      Protein Get(int projectId, bool allowRefresh);

      /// <summary>
      /// Gets a collection of all protein project id numbers.
      /// </summary>
      /// <returns>A collection of all protein project id numbers.</returns>
      IEnumerable<int> GetProjects();

      /// <summary>
      /// Refreshes the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <param name="progress">The object used to report refresh progress.</param>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      IReadOnlyCollection<ProteinDictionaryChange> Refresh(IProgress<ProgressInfo> progress);
   }

   public sealed class ProteinService : DataContainer<List<Protein>>, IProteinService
   {
      private ProteinDictionary _dictionary;
      private readonly IProjectSummaryDownloader _downloader;

      private readonly Dictionary<int, DateTime> _projectsNotFound;
      private DateTime? _lastRefreshTime;

      internal ProteinService()
         : this(null, null)
      {
         
      }

      public ProteinService(IPreferenceSet prefs, IProjectSummaryDownloader downloader)
      {
         _dictionary = new ProteinDictionary();
         _downloader = downloader;

         _projectsNotFound = new Dictionary<int, DateTime>();

         var path = prefs != null ? prefs.Get<string>(Preference.ApplicationDataFolderPath) : null;
         if (!String.IsNullOrEmpty(path))
         {
            FileName = Path.Combine(path, Constants.ProjectInfoFileName);
         }
      }

      #region Properties

      /// <summary>
      /// Gets the dictionary that contains previously queried project id numbers that were not found and the date and time of the query.
      /// </summary>
      internal IDictionary<int, DateTime> ProjectsNotFound
      {
         get { return _projectsNotFound; }
      }

      internal DateTime? LastRefreshTime
      {
         get { return _lastRefreshTime; }
         set { _lastRefreshTime = value; }
      }

      public override Serializers.IFileSerializer<List<Protein>> DefaultSerializer
      {
         get { return new TabSerializer(); }
      }

      private sealed class TabSerializer : Serializers.IFileSerializer<List<Protein>>
      {
         private readonly Proteins.TabDelimitedTextSerializer _serializer = new Proteins.TabDelimitedTextSerializer();

         public string FileExtension
         {
            get { return "tab"; }
         }

         public string FileTypeFilter
         {
            get { return "Project Info Tab Delimited Files|*.tab"; }
         }

         public List<Protein> Deserialize(string fileName)
         {
            return _serializer.ReadFile(fileName).ToList();
         }

         public void Serialize(string fileName, List<Protein> value)
         {
            _serializer.WriteFile(fileName, value);
         }
      }

      #endregion

      /// <summary>
      /// Gets the protein with the given project id.
      /// </summary>
      /// <param name="projectId">The project id of the protein to return.</param>
      /// <returns>The protein object with the given project id or null if the protein does not exist.</returns>
      public Protein Get(int projectId)
      {
         return Get(projectId, false);
      }

      /// <summary>
      /// Gets a collection of all protein project id numbers.
      /// </summary>
      /// <returns>A collection of all protein project id numbers.</returns>
      public IEnumerable<int> GetProjects()
      {
         return _dictionary.Keys;
      }

      internal void Add(Protein protein)
      {
         _dictionary.Add(protein.ProjectNumber, protein);
      }

      /// <summary>
      /// Gets the protein with the given project id.
      /// </summary>
      /// <param name="projectId">The project id of the protein to return.</param>
      /// <param name="allowRefresh">true to allow this Get method to refresh the service.</param>
      /// <returns>The protein object with the given project id or null if the protein does not exist.</returns>
      public Protein Get(int projectId, bool allowRefresh)
      {
         if (_dictionary.ContainsKey(projectId))
         {
            return _dictionary[projectId];
         }

         if (projectId == 0 || !allowRefresh || !CanRefresh(projectId))
         {
            return null;
         }

         if (_downloader != null)
         {
            Logger.Info(String.Format("Project ID {0} triggering project data refresh.", projectId));

            try
            {
               Refresh(null);
            }
            catch (Exception ex)
            {
               Logger.Error(ex.Message, ex);
            }

            if (_dictionary.ContainsKey(projectId))
            {
               return _dictionary[projectId];
            }

            // update the last download attempt date
            _projectsNotFound[projectId] = DateTime.UtcNow;
         }

         return null;
      }

      /// <summary>
      /// Refreshes the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <param name="progress">The object used to report refresh progress.</param>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      public IReadOnlyCollection<ProteinDictionaryChange> Refresh(IProgress<ProgressInfo> progress)
      {
         IReadOnlyCollection<ProteinDictionaryChange> dictionaryChanges;
         using (var stream = new MemoryStream())
         {
            Logger.Info("Downloading new project data from Stanford...");
            _downloader.Download(stream, progress);
            stream.Position = 0;

            var serializer = new ProjectSummaryJsonDeserializer();
            var newDictionary = ProteinDictionary.CreateFromExisting(_dictionary, serializer.Deserialize(stream));
            dictionaryChanges = newDictionary.Changes;
            _dictionary = newDictionary;
         }

         foreach (var info in dictionaryChanges.Where(info => info.Result != ProteinDictionaryChangeResult.NoChange))
         {
            Logger.Info(info.ToString());
         }

         var now = DateTime.UtcNow;
         foreach (var key in _projectsNotFound.Keys.ToList())
         {
            if (_dictionary.ContainsKey(key))
            {
               _projectsNotFound.Remove(key);
            }
            else
            {
               _projectsNotFound[key] = now;
            }
         }
         _lastRefreshTime = now;

         Write();
         return dictionaryChanges;
      }

      private bool CanRefresh(int projectId)
      {
         if (_lastRefreshTime.HasValue)
         {
            // if a download was attempted in the last hour, don't execute again
            TimeSpan lastDownloadDifference = DateTime.UtcNow.Subtract(_lastRefreshTime.Value);
            if (lastDownloadDifference.TotalHours < 1)
            {
               Logger.Debug(String.Format("Download executed {0:0} minutes ago.", lastDownloadDifference.TotalMinutes));
               return false;
            }
         }

         // if this project has already been looked for previously
         if (_projectsNotFound.ContainsKey(projectId))
         {
            // if it has been less than one day since this project triggered an
            // automatic download attempt just return a blank protein.
            if (DateTime.UtcNow.Subtract(_projectsNotFound[projectId]).TotalDays < 1)
            {
               return false;
            }
         }

         return true;
      }

      #region DataContainer<T>

      public override void Read()
      {
         // read the List<Protein>
         base.Read();
         // add each protein to the Dictionary<int, Protein>
         foreach (var protein in Data)
         {
            Add(protein);
         }
      }

      public override void Write()
      {
         Data = _dictionary.Values.ToList();

         base.Write();
      }

      #endregion
   }

   public interface IProjectSummaryDownloader
   {
      /// <summary>
      /// Downloads the project information.
      /// </summary>
      void Download(Stream stream, IProgress<ProgressInfo> progress);
   }

   public sealed class ProjectSummaryDownloader : IProjectSummaryDownloader
   {
      private readonly IPreferenceSet _prefs;

      public ProjectSummaryDownloader(IPreferenceSet prefs)
      {
         _prefs = prefs ?? throw new ArgumentNullException(nameof(prefs));
      }

      /// <summary>
      /// Downloads the project information.
      /// </summary>
      /// <remarks>Access to the Download method is synchronized.</remarks>
      public void Download(Stream stream, IProgress<ProgressInfo> progress)
      {
         IWebOperation httpWebOperation = WebOperation.Create(_prefs.Get<string>(Preference.ProjectDownloadUrl));
         if (progress != null)
         {
            httpWebOperation.ProgressChanged += (sender, e) =>
            {
               int progressPercentage = Convert.ToInt32(e.Length / (double)e.TotalLength * 100);
               string message = String.Format(CultureInfo.CurrentCulture, "Downloading {0} of {1} bytes...", e.Length, e.TotalLength);
               progress.Report(new ProgressInfo(progressPercentage, message));
            };
         }
         httpWebOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
         httpWebOperation.WebRequest.Proxy = _prefs.GetWebProxy();
         httpWebOperation.Download(stream);
      }
   }
}
