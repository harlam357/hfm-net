
using System;

namespace HFM.Framework
{
   public interface IProteinCollection
   {
      /// <summary>
      /// ProjectInfo.tab File Location
      /// </summary>
      string ProjectInfoLocation { get; }

      /// <summary>
      /// Project Summary HTML File Location
      /// </summary>
      Uri ProjectSummaryLocation { get; set; }

      /// <summary>
      /// Project (Protein) Data has been Updated
      /// </summary>
      event EventHandler ProjectInfoUpdated;

      /// <summary>
      /// Execute Primary Collection Load Sequence
      /// </summary>
      void Load();

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      bool LoadFromTabDelimitedFile();

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      /// <param name="ProjectInfoFilePath">Path to File</param>
      bool LoadFromTabDelimitedFile(string ProjectInfoFilePath);

      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      IAsyncResult BeginDownloadFromStanford();

      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      void DownloadFromStanford();

      /// <summary>
      /// Read Project Information from HTML (psummary.html)
      /// </summary>
      void ReadFromProjectSummaryHtml(Uri location);

      /// <summary>
      /// Get Protein (should be called from worker thread)
      /// </summary>
      /// <param name="ProjectID">Project ID</param>
      IProtein GetProtein(int ProjectID);

      /// <summary>
      /// Get a New Protein from the Collection
      /// </summary>
      IProtein GetNewProtein();

      void Add(int key, IProtein value);
      bool Remove(int key);
      void Clear();
      
      bool ContainsKey(int key);
      bool ContainsValue(IProtein value);
      
      bool TryGetValue(int key, out IProtein value);
      
      IProtein this[int key] { get; set; }
      int Count { get; }
   }
}