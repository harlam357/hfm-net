
using System;
using System.Globalization;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public static partial class Extensions
   {
      /// <summary>
      /// Is the project information unknown?
      /// </summary>
      /// <returns>true if Project (R/C/G) has not been identified; otherwise, false.</returns>
      internal static bool ProjectIsUnknown(this IProjectInfo projectInfo)
      {
         if (projectInfo == null)
         {
            return true;
         }
         return projectInfo.ProjectID == 0 &&
                projectInfo.ProjectRun == 0 &&
                projectInfo.ProjectClone == 0 &&
                projectInfo.ProjectGen == 0;
      }

      /// <summary>
      /// Determines whether the specified project information is equal to this project information.
      /// </summary>
      /// <returns>true if the specified Project (R/C/G) is equal to the this Project (R/C/G); otherwise, false.</returns>
      internal static bool EqualsProject(this IProjectInfo projectInfo1, IProjectInfo projectInfo2)
      {
         if (projectInfo1 == null || projectInfo2 == null)
         {
            return false;
         }
         return projectInfo1.ProjectID == projectInfo2.ProjectID &&
                projectInfo1.ProjectRun == projectInfo2.ProjectRun &&
                projectInfo1.ProjectClone == projectInfo2.ProjectClone &&
                projectInfo1.ProjectGen == projectInfo2.ProjectGen;
      }

      /// <summary>
      /// Returns a string that represents the Project (R/C/G) information.
      /// </summary>
      public static string ToProjectString(this IProjectInfo projectInfo)
      {
         if (projectInfo == null)
         {
            return String.Empty;
         }
         return String.Format(CultureInfo.InvariantCulture, "Project: {0} (Run {1}, Clone {2}, Gen {3})", 
            projectInfo.ProjectID, projectInfo.ProjectRun, projectInfo.ProjectClone, projectInfo.ProjectGen);
      }

      /// <summary>
      /// Returns a short string that represents the Project (R/C/G) information.
      /// </summary>
      public static string ToShortProjectString(this IProjectInfo projectInfo)
      {
         if (projectInfo == null)
         {
            return String.Empty;
         }
         return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})",
            projectInfo.ProjectID, projectInfo.ProjectRun, projectInfo.ProjectClone, projectInfo.ProjectGen);
      }
   }
}
