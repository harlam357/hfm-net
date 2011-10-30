
using System.Collections.Generic;

namespace HFM.Proteins
{
   public enum ProteinLoadResult
   {
      Add,
      Change,
      NoChange,
   }

   public sealed class ProteinLoadInfo
   {
      private readonly int _projectNumber;
      /// <summary>
      /// Project Number
      /// </summary>
      public int ProjectNumber
      {
         get { return _projectNumber; }
      }

      private readonly ProteinLoadResult _result;

      public ProteinLoadResult Result
      {
         get { return _result; }
      }

      private readonly IEnumerable<ProteinPropertyChange> _changes;

      public IEnumerable<ProteinPropertyChange> Changes
      {
         get { return _changes; }
      }

      internal ProteinLoadInfo(int projectNumber, ProteinLoadResult result)
         : this(projectNumber, result, null)
      {

      }

      internal ProteinLoadInfo(int projectNumber, ProteinLoadResult result, IEnumerable<ProteinPropertyChange> changes)
      {
         _projectNumber = projectNumber;
         _result = result;
         _changes = changes;
      }
   }

   public sealed class ProteinPropertyChange
   {
      private readonly string _name;
      public string Name
      {
         get { return _name; }
      }

      private readonly string _oldValue;
      public string OldValue
      {
         get { return _oldValue; }
      }

      private readonly string _newValue;
      public string NewValue
      {
         get { return _newValue; }
      }

      internal ProteinPropertyChange(string name, string oldValue, string newValue)
      {
         _name = name;
         _oldValue = oldValue;
         _newValue = newValue;
      }
   }
}
