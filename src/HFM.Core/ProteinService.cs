/*
 * HFM.NET - Protein Service
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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

using HFM.Core.DataTypes;
using HFM.Proteins;

namespace HFM.Core
{
   public interface IProteinService
   {
      Protein Get(int projectId);

      IEnumerable<int> GetProjects();

      /// <summary>
      /// Clear the projects not found cache.
      /// </summary>
      void ClearProjectsNotFoundCache();

      Protein Get(int projectId, bool allowRefresh);

      /// <summary>
      /// Loads data and returns an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing details on how the data was changed.
      /// </summary>
      /// <param name="path">The file containing data to load into the service.</param>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing details on how the data was changed.</returns>
      IEnumerable<ProteinLoadInfo> Load(string path);

      void Write();
   }

   [CoverageExclude]
   public sealed class ProteinService : DataContainer<List<Protein>>, IProteinService
   {
      /// <summary>
      /// List of Projects that were not found.
      /// </summary>
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

      internal IDictionary<int, DateTime> ProjectsNotFound
      {
         get { return _projectsNotFound; }
      }

      public override Plugins.IFileSerializer<List<Protein>> DefaultSerializer
      {
         get { return new TabSerializer(); }
      }

      #endregion

      public Protein Get(int projectId)
      {
         return Get(projectId, false);
      }

      public IEnumerable<int> GetProjects()
      {
         return _dictionary.Keys;
      }

      internal void Add(Protein protein)
      {
         _dictionary.Add(protein.ProjectNumber, protein);
      }

      /// <summary>
      /// Clear the projects not found cache.
      /// </summary>
      public void ClearProjectsNotFoundCache()
      {
         _projectsNotFound.Clear();
      }

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

         Logger.Info("Project ID '{0}' not found.", projectId);
         if (_downloader != null)
         {
            _downloader.Download();
            try
            {
               var loadInfo = Load(_downloader.DownloadFilePath);
               foreach (var info in loadInfo.Where(info => info.Result != ProteinLoadResult.NoChange))
               {
                  Logger.Info(info.ToString());
               }
               Write();
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
         }

         AddToProjectsNotFound(projectId);

         return null;
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

      private void AddToProjectsNotFound(int projectId)
      {
         // if already on the not found list
         if (_projectsNotFound.ContainsKey(projectId))
         {
            // update the last download attempt date
            _projectsNotFound[projectId] = DateTime.Now;
         }
         else
         {
            _projectsNotFound.Add(projectId, DateTime.Now);
         }
      }

      /// <summary>
      /// Loads data and returns an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing details on how the data was changed.
      /// </summary>
      /// <param name="path">The file containing data to load into the service.</param>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing details on how the data was changed.</returns>
      public IEnumerable<ProteinLoadInfo> Load(string path)
      {
         return _dictionary.Load(path);
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
