using System.Globalization;

using HFM.Core.Serializers;

namespace HFM.Core.Data
{
    public class WorkUnitRowCsvFileSerializer : IFileSerializer<List<WorkUnitRow>>
    {
        public string FileExtension => "csv";

        public string FileTypeFilter => "Comma Separated Value Files|*.csv";

        public List<WorkUnitRow> Deserialize(string path) =>
            throw new NotSupportedException("History entry csv deserialization is not supported.");

        public void Serialize(string path, List<WorkUnitRow> value)
        {
            using var writer = new StreamWriter(path, false);
            Serialize(writer, value);
        }

        internal static void Serialize(TextWriter writer, List<WorkUnitRow> value)
        {
            string line = String.Join(",", new[]
            {
                "DatabaseID",
                "ProjectID",
                "ProjectRun",
                "ProjectClone",
                "ProjectGen",
                "SlotName",
                "ConnectionString",
                "DonorName",
                "DonorTeam",
                "CoreVersion",
                "FramesCompleted",
                "FrameTime",
                "Result",
                "Assigned",
                "Finished",
                "KFactor",
                "Core",
                "Frames",
                "Atoms",
                "SlotType",
                "PPD",
                "Credit",
                "BaseCredit"
            });
            writer.WriteLine(line);

            var provider = CultureInfo.InvariantCulture;
            foreach (var h in value)
            {
                line = String.Join(",", new object[]
                {
                    h.ID,
                    h.ProjectID,
                    h.ProjectRun,
                    h.ProjectClone,
                    h.ProjectGen,
                    h.SlotName,
                    h.ConnectionString,
                    h.DonorName,
                    h.DonorTeam,
                    h.CoreVersion,
                    h.FramesCompleted,
                    h.FrameTime,
                    h.Result,
                    h.Assigned.ToString(provider),
                    h.Finished.ToString(provider),
                    h.KFactor.ToString(provider),
                    h.Core,
                    h.Frames,
                    h.Atoms,
                    h.SlotType,
                    h.PPD.ToString(provider),
                    h.Credit.ToString(provider),
                    h.BaseCredit.ToString(provider)
                });
                writer.WriteLine(line);
            }
        }
    }
}
