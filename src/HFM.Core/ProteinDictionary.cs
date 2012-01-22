/*
 * HFM.NET - Core Protein Dictionary
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
   public interface IProteinDictionary : IDictionary<int, Protein>
   {
      /// <summary>
      /// Clear the Projects not found cache
      /// </summary>
      void ClearProjectsNotFoundCache();

      Protein GetProteinOrDownload(int projectId);

      /// <summary>
      /// Load element values into the ProteinDictionary and return an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.
      /// </summary>
      /// <param name="values">The <paramref name="values"/> to load into the ProteinDictionary. <paramref name="values"/> cannot be null.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="values"/> is null.</exception>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.</returns>
      IEnumerable<ProteinLoadInfo> Load(IEnumerable<Protein> values);

      #region DataContainer<T>

      void Read();

      List<Protein> Read(string filePath, Plugins.IFileSerializer<List<Protein>> serializer);

      void Write();

      void Write(string filePath, Plugins.IFileSerializer<List<Protein>> serializer);

      #endregion
   }

   [CoverageExclude]
   public sealed class ProteinDictionary : DataContainer<List<Protein>>, IProteinDictionary
   {
      /// <summary>
      /// List of Projects that were not found.
      /// </summary>
      private readonly Dictionary<Int32, DateTime> _projectsNotFound;
      private readonly Proteins.ProteinDictionary _dictionary;
      private readonly IProjectSummaryDownloader _downloader;

      public ProteinDictionary()
         : this(null, null)
      {
         
      }

      public ProteinDictionary(IPreferenceSet prefs, IProjectSummaryDownloader downloader)
      {
         _projectsNotFound = new Dictionary<Int32, DateTime>(); 
         _dictionary = new Proteins.ProteinDictionary();
         _downloader = downloader;

         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.ProjectInfoFileName);
         }
      }

      #region Properties

      public override Plugins.IFileSerializer<List<Protein>> DefaultSerializer
      {
         get { return new TabSerializer(); }
      }

      #endregion

      /// <summary>
      /// Clear the Projects not found cache
      /// </summary>
      public void ClearProjectsNotFoundCache()
      {
         _projectsNotFound.Clear();
      }

      public Protein GetProteinOrDownload(int projectId)
      {
         if (projectId == 0) return new Protein();
         if (ContainsKey(projectId)) return this[projectId];

         if (CheckProjectsNotFound(projectId))
         {
            return new Protein();
         }

         Logger.Info("Project ID '{0}' not found.", projectId);

         if (_downloader != null)
         {
            // Execute a Download (Stanford)
            _downloader.DownloadFromStanford();
            try
            {
               Load(Read(_downloader.DownloadFilePath, new HtmlSerializer()));
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
            }

            if (ContainsKey(projectId))
            {
               // remove it from the not found list and return it
               _projectsNotFound.Remove(projectId);
               return this[projectId];
            }

            /*** Disable HFM Web Download For Now ***/

            //// Execute a Download (HFM Web)
            //_downloader.DownloadFromHfmWeb();
            //try
            //{
            //   var proteinList = Read(_downloader.DownloadFilePath, new Serializers.XmlFileSerializer<List<Protein>>());
            //   var protein = proteinList.FirstOrDefault(x => x.ProjectNumber == projectId);
            //   if (protein != null)
            //   {
            //      Add(protein.ProjectNumber, protein);
            //   }
            //}
            //catch (Exception ex)
            //{
            //   Logger.ErrorFormat(ex, "{0}", ex.Message);
            //}

            //if (ContainsKey(projectId))
            //{
            //   // remove it from the not found list and return it
            //   _projectsNotFound.Remove(projectId);
            //   return this[projectId];
            //}
         }

         AddToProjectsNotFound(projectId);

         // return a blank protein
         return new Protein();
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

      public IEnumerable<ProteinLoadInfo> Load(IEnumerable<Protein> values)
      {
         return _dictionary.Load(values);
      }

      #region DataContainer<T>

      public override void Read()
      {
         // read the List<Protein>
         base.Read();
         // add each protein to the Dictionary<int, Protein>
         foreach (var protein in Data)
         {
            Add(protein.ProjectNumber, protein);
         }
      }

      public override void Write()
      {
         Data = _dictionary.Values.ToList();

         base.Write();
      }

      public override void Write(string filePath, Plugins.IFileSerializer<List<Protein>> serializer)
      {
         Data = _dictionary.Values.ToList();

         base.Write(filePath, serializer);
      }

      #endregion

      #region IDictionary<int,Protein> Members

      public void Add(int key, Protein value)
      {
         _dictionary.Add(key, value);
      }

      public bool ContainsKey(int key)
      {
         return _dictionary.ContainsKey(key);
      }

      public ICollection<int> Keys
      {
         get { return _dictionary.Keys; }
      }

      public bool Remove(int key)
      {
         return _dictionary.Remove(key);
      }

      public bool TryGetValue(int key, out Protein value)
      {
         return _dictionary.TryGetValue(key, out value);
      }

      public ICollection<Protein> Values
      {
         get { return _dictionary.Values; }
      }

      public Protein this[int key]
      {
         get { return _dictionary[key]; }
         set { _dictionary[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<int,Protein>> Members

      void ICollection<KeyValuePair<int, Protein>>.Add(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         _dictionary.Clear();
      }

      bool ICollection<KeyValuePair<int, Protein>>.Contains(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      public void CopyTo(KeyValuePair<int, Protein>[] array, int index)
      {
         _dictionary.CopyTo(array, index);
      }

      public int Count
      {
         get { return _dictionary.Count; }
      }

      bool ICollection<KeyValuePair<int, Protein>>.IsReadOnly
      {
         get { return false; }
      }

      bool ICollection<KeyValuePair<int, Protein>>.Remove(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<int,Protein>> Members

      public IEnumerator<KeyValuePair<int, Protein>> GetEnumerator()
      {
         return _dictionary.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
