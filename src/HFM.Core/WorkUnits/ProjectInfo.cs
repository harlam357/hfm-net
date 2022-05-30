namespace HFM.Core.WorkUnits;

public interface IProjectInfo
{
    int ProjectID { get; }
    int ProjectRun { get; }
    int ProjectClone { get; }
    int ProjectGen { get; }
}

public class ProjectInfo : IProjectInfo
{
    public int ProjectID { get; set; }
    public int ProjectRun { get; set; }
    public int ProjectClone { get; set; }
    public int ProjectGen { get; set; }

    public override string ToString() => this.ToProjectString();
}
