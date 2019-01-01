
using System;
using System.Globalization;

namespace HFM.Log
{
   /// <summary>
   /// Represents Folding@Home project data gathered from a <see cref="LogLine"/> object.
   /// </summary>
   public class WorkUnitProjectData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="WorkUnitProjectData"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
      public WorkUnitProjectData(WorkUnitProjectData other)
      {
         if (other == null) return;

         ProjectID = other.ProjectID;
         ProjectRun = other.ProjectRun;
         ProjectClone = other.ProjectClone;
         ProjectGen = other.ProjectGen;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="WorkUnitProjectData"/> class.
      /// </summary>
      /// <param name="projectID">The project ID (Number).</param>
      /// <param name="run">The project ID (Run).</param>
      /// <param name="clone">The project ID (Clone).</param>
      /// <param name="gen">The project ID (Gen).</param>
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

      /// <summary>
      /// Returns a string that represents the current <see cref="WorkUnitProjectData"/> object.
      /// </summary>
      /// <returns>A string that represents the current <see cref="WorkUnitProjectData"/> object.</returns>
      public override string ToString()
      {
         return String.Format(CultureInfo.InvariantCulture, "Project: {0} (Run {1}, Clone {2}, Gen {3})",
            ProjectID, ProjectRun, ProjectClone, ProjectGen);
      }
   }
}
