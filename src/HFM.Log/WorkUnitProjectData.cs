
using System;
using System.Globalization;

namespace HFM.Log
{
   public class WorkUnitProjectData
   {
      public WorkUnitProjectData(int projectID, int run, int clone, int gen)
      {
         ProjectID = projectID;
         ProjectRun = run;
         ProjectClone = clone;
         ProjectGen = gen;
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Project Run Number
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Project Clone Number
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Project Gen (Generation) Number
      /// </summary>
      public int ProjectGen { get; set; }

      public override string ToString()
      {
         return String.Format(CultureInfo.InvariantCulture, "Project: {0} (Run {1}, Clone {2}, Gen {3})",
            ProjectID, ProjectRun, ProjectClone, ProjectGen);
      }
   }
}
