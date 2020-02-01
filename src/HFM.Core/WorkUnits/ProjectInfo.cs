/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Globalization;

namespace HFM.Core.WorkUnits
{
    public interface IProjectInfo
    {
        /// <summary>
        /// Project ID Number
        /// </summary>
        int ProjectID { get; }

        /// <summary>
        /// Project ID (Run)
        /// </summary>
        int ProjectRun { get; }

        /// <summary>
        /// Project ID (Clone)
        /// </summary>
        int ProjectClone { get; }

        /// <summary>
        /// Project ID (Gen)
        /// </summary>
        int ProjectGen { get; }
    }

    public class ProjectInfo : IProjectInfo
    {
        /// <summary>
        /// Project ID Number
        /// </summary>
        public int ProjectID { get; set; }

        /// <summary>
        /// Project ID (Run)
        /// </summary>
        public int ProjectRun { get; set; }

        /// <summary>
        /// Project ID (Clone)
        /// </summary>
        public int ProjectClone { get; set; }

        /// <summary>
        /// Project ID (Gen)
        /// </summary>
        public int ProjectGen { get; set; }

        public override string ToString()
        {
            return this.ToProjectString();
        }
    }

    public static class ProjectInfoExtensions
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
        internal static string ToProjectString(this IProjectInfo projectInfo)
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
