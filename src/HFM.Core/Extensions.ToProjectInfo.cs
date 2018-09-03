
using HFM.Core.DataTypes;

namespace HFM.Core
{
   public static partial class Extensions
   {
      internal static ProjectInfo ToProjectInfo(this HFM.Client.DataTypes.Unit value)
      {
         return new ProjectInfo
         {
            ProjectID = value.Project,
            ProjectRun = value.Run,
            ProjectClone = value.Clone,
            ProjectGen = value.Gen
         };
      }

      internal static ProjectInfo ToProjectInfo(this Log.UnitRunData value)
      {
         return new ProjectInfo
         {
            ProjectID = value.ProjectID,
            ProjectRun = value.ProjectRun,
            ProjectClone = value.ProjectClone,
            ProjectGen = value.ProjectGen
         };
      }

      internal static ProjectInfo ToProjectInfo(this Log.UnitInfoLogData value)
      {
         if (value == null) return null;
         return new ProjectInfo
         {
            ProjectID = value.ProjectID,
            ProjectRun = value.ProjectRun,
            ProjectClone = value.ProjectClone,
            ProjectGen = value.ProjectGen
         };
      }

      internal static ProjectInfo ToProjectInfo(this Queue.QueueEntry value)
      {
         return new ProjectInfo
         {
            ProjectID = value.ProjectID,
            ProjectRun = value.ProjectRun,
            ProjectClone = value.ProjectClone,
            ProjectGen = value.ProjectGen
         };
      }
   }
}