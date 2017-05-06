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
using System.Linq;
using System.Threading.Tasks;

using HFM.Core.Data;
using HFM.Core.DataTypes;
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
      /// Resets the data that halts redundant service refresh calls.
      /// </summary>
      void ResetRefreshParameters();

      /// <summary>
      /// Refreshs the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      IEnumerable<ProteinLoadInfo> Refresh();

      /// <summary>
      /// Refreshs the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <param name="progress">The object used to report refresh progress.</param>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      Task<IEnumerable<ProteinLoadInfo>> RefreshAsync(IProgress<harlam357.Core.ComponentModel.ProgressChangedEventArgs> progress);
   }

   public sealed class ProteinService : DataContainer<List<Protein>>, IProteinService
   {
      private readonly Dictionary<Int32, DateTime> _projectsNotFound;
      private readonly ProteinDictionary _dictionary;
      private readonly IProjectSummaryDownloader _downloader;

      internal ProteinService()
         : this(null, null)
      {
         
      }

      public ProteinService(IPreferenceSet prefs, IProjectSummaryDownloader downloader)
      {
         _projectsNotFound = new Dictionary<Int32, DateTime>(); 
         _dictionary = new ProteinDictionary();
         _downloader = downloader;

         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.ProjectInfoFileName);
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

      public override Plugins.IFileSerializer<List<Protein>> DefaultSerializer
      {
         get { return new TabSerializer(); }
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
      /// Resets the data that halts redundant service refresh calls.
      /// </summary>
      public void ResetRefreshParameters()
      {
         _projectsNotFound.Clear();
         if (_downloader != null)
         {
            _downloader.ResetDownloadParameters();
         }
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

         if (!allowRefresh || projectId == 0 || CheckProjectsNotFound(projectId))
         {
            return null;
         }

         if (_downloader != null)
         {
            Logger.InfoFormat("Project ID {0} triggering project data refresh.", projectId);

            try
            {
               Refresh();
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }

            if (_dictionary.ContainsKey(projectId))
            {
               // remove it from the not found list and return it
               _projectsNotFound.Remove(projectId);
               return _dictionary[projectId];
            }

            // update the last download attempt date
            _projectsNotFound[projectId] = DateTime.Now;
         }

         return null;
      }

      /// <summary>
      /// Refreshs the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      public IEnumerable<ProteinLoadInfo> Refresh()
      {
         _downloader.Download();
         return Load();
      }

      /// <summary>
      /// Refreshs the service data and returns a collection of objects detailing how the service data was changed.
      /// </summary>
      /// <param name="progress">The object used to report refresh progress.</param>
      /// <returns>A collection of objects detailing how the service data was changed</returns>
      public Task<IEnumerable<ProteinLoadInfo>> RefreshAsync(IProgress<harlam357.Core.ComponentModel.ProgressChangedEventArgs> progress)
      {
         return Task.Factory.StartNew(() =>
         {
            _downloader.DownloadAsync(progress).Wait();
            return Load();
         });
      }

      private IEnumerable<ProteinLoadInfo> Load()
      {
         IEnumerable<ProteinLoadInfo> loadInfo = _dictionary.Load(_downloader.FilePath).ToList();
         foreach (var info in loadInfo.Where(info => info.Result != ProteinLoadResult.NoChange))
         {
            Logger.Info(info.ToString());
         }
         Write();
         return loadInfo;
      }

      private bool CheckProjectsNotFound(int projectId)
      {
         // if this project has already been looked for previously
         if (_projectsNotFound.ContainsKey(projectId))
         {
            // if it has been less than one day since this project triggered an
            // automatic download attempt just return a blank protein.
            if (DateTime.Now.Subtract(_projectsNotFound[projectId]).TotalDays < 1)
            {
               return true;
            }
         }

         return false;
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
}
