using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data;

public static class WorkUnitQueryable
{
    public static IQueryable<WorkUnitEntity> Where(this IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        q = parameter.Operator switch
        {
            WorkUnitQueryOperator.Equal => WhereEqual(q, parameter),
            WorkUnitQueryOperator.GreaterThan => WhereGreaterThan(q, parameter),
            WorkUnitQueryOperator.GreaterThanOrEqual => WhereGreaterThanOrEqual(q, parameter),
            WorkUnitQueryOperator.LessThan => WhereLessThan(q, parameter),
            WorkUnitQueryOperator.LessThanOrEqual => WhereLessThanOrEqual(q, parameter),
            WorkUnitQueryOperator.Like => WhereLike(q, parameter),
            WorkUnitQueryOperator.NotLike => WhereNotLike(q, parameter),
            WorkUnitQueryOperator.NotEqual => WhereNotEqual(q, parameter),
            _ => throw new InvalidOperationException($"Operator {parameter.Operator} is not supported.")
        };
        return q;
    }

    private static IQueryable<WorkUnitEntity> WhereEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = (string)parameter.Value;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen == Int32.Parse(stringValue)),
            WorkUnitRowColumn.Name => q.Where(x => x.Client.Name == stringValue),
            WorkUnitRowColumn.Path => q.Where(x => x.Client.ConnectionString == stringValue),
            WorkUnitRowColumn.Username => q.Where(x => x.DonorName == stringValue),
            WorkUnitRowColumn.Team => q.Where(x => x.DonorTeam == Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion == stringValue),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted == Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds == Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result == WorkUnitResultString.FromWorkUnitResult((WorkUnitResult)Int32.Parse(stringValue))),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned == (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished == (DateTime)parameter.Value),
            WorkUnitRowColumn.WorkUnitName => throw new NotImplementedException(),
            WorkUnitRowColumn.KFactor => q.Where(x => Math.Abs(x.Protein.KFactor - Double.Parse(stringValue)) < 0.001),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core == stringValue),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames == Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms == Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => ConvertToSlotType.FromCoreName(x.Protein.Core).ToString() == stringValue),
            WorkUnitRowColumn.PPD => throw new NotImplementedException(),
            WorkUnitRowColumn.Credit => throw new NotImplementedException(),
            WorkUnitRowColumn.BaseCredit => q.Where(x => Math.Abs(x.Protein.Credit - Double.Parse(stringValue)) < 0.001),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereGreaterThan(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereGreaterThanOrEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereLessThan(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereLessThanOrEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereLike(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereNotLike(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
    private static IQueryable<WorkUnitEntity> WhereNotEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter) => throw new NotImplementedException();
}
