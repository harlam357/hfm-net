
using System;
using System.Globalization;

namespace HFM.Log
{
   public class WorkUnitProjectData
   {
      public WorkUnitProjectData(WorkUnitProjectData other)
      {
         if (other == null) return;

         ProjectID = other.ProjectID;
         ProjectRun = other.ProjectRun;
         ProjectClone = other.ProjectClone;
         ProjectGen = other.ProjectGen;
      }

      public WorkUnitProjectData(int projectID, int run, int clone, int gen)
      {
         ProjectID = projectID;
         ProjectRun = run;
         ProjectClone = clone;
         ProjectGen = gen;
      }

      /// <summary>
      /// Gets or sets the project ID (Number).
      /// </summary>
      public int ProjectID { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Run).
      /// </summary>
      public int ProjectRun { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Clone).
      /// </summary>
      public int ProjectClone { get; set; }

      /// <summary>
      /// Gets or sets the project ID (Gen).
      /// </summary>
      public int ProjectGen { get; set; }

      public override string ToString()
      {
         return String.Format(CultureInfo.InvariantCulture, "Project: {0} (Run {1}, Clone {2}, Gen {3})",
            ProjectID, ProjectRun, ProjectClone, ProjectGen);
      }
   }
}
