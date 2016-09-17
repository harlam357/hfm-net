
using System;
using System.Globalization;

namespace HFM.Core.DataTypes
{
   public static partial class Extensions
   {
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
