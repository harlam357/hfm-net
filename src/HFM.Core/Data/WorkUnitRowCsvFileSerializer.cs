/*
 * HFM.NET - History Entry CSV Serializer
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.IO;

using HFM.Core.Serializers;

namespace HFM.Core.Data
{
    public class WorkUnitRowCsvFileSerializer : IFileSerializer<List<WorkUnitRow>>
    {
        public string FileExtension => "csv";

        public string FileTypeFilter => "Comma Separated Value Files|*.csv";

        public List<WorkUnitRow> Deserialize(string path)
        {
            throw new NotSupportedException("History entry csv deserialization is not supported.");
        }

        public void Serialize(string path, List<WorkUnitRow> value)
        {
            using (var writer = new StreamWriter(path, false))
            {
                Serialize(writer, value);
            }
        }

        internal void Serialize(TextWriter writer, List<WorkUnitRow> value)
        {
            string line = String.Join(",", new[]
            {
                "DatabaseID",
                "ProjectID",
                "ProjectRun",
                "ProjectClone",
                "ProjectGen",
                "Name",
                "Path",
                "Username",
                "Team",
                "CoreVersion",
                "FramesCompleted",
                "FrameTime",
                //"FrameTimeValue",
                "Result",
                //"ResultValue",
                "DownloadDateTime",
                "CompletionDateTime",
                "WorkUnitName",
                "KFactor",
                "Core",
                "Frames",
                "Atoms",
                //"BaseCredit",
                "PreferredDays",
                "MaximumDays",
                "SlotType",
                "PPD",
                "Credit"
            });
            writer.WriteLine(line);
            foreach (var h in value)
            {
                line = String.Join(",", new object[]
                {
                    h.ID,
                    h.ProjectID,
                    h.ProjectRun,
                    h.ProjectClone,
                    h.ProjectGen,
                    h.Name,
                    h.Path,
                    h.Username,
                    h.Team,
                    h.CoreVersion.ToString(CultureInfo.InvariantCulture),
                    h.FramesCompleted,
                    h.FrameTime,
                    //h.FrameTimeValue,
                    h.Result,
                    //h.ResultValue,
                    h.DownloadDateTime.ToString(CultureInfo.InvariantCulture),
                    h.CompletionDateTime.ToString(CultureInfo.InvariantCulture),
                    h.WorkUnitName,
                    h.KFactor.ToString(CultureInfo.InvariantCulture),
                    h.Core,
                    h.Frames,
                    h.Atoms,
                    //h.BaseCredit,
                    h.PreferredDays.ToString(CultureInfo.InvariantCulture),
                    h.MaximumDays.ToString(CultureInfo.InvariantCulture),
                    h.SlotType,
                    h.PPD.ToString(CultureInfo.InvariantCulture),
                    h.Credit.ToString(CultureInfo.InvariantCulture)
                });
                writer.WriteLine(line);
            }
        }
    }
}
