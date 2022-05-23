using Microsoft.EntityFrameworkCore;

// ReSharper disable SpecifyACultureInStringConversionExplicitly
// ReSharper disable StringCompareToIsCultureSpecific

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
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone == Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen == Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName == stringValue),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString == stringValue),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName == stringValue),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam == Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion == stringValue),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted == Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds == Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result == stringValue),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned == (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished == (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => Math.Abs(x.Protein.KFactor - Double.Parse(stringValue)) < 0.001),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core == stringValue),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames == Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms == Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType == stringValue),
            WorkUnitRowColumn.PPD => q.Where(x => Math.Abs(x.PPD - Double.Parse(stringValue)) < 0.001),
            WorkUnitRowColumn.Credit => q.Where(x => Math.Abs(x.Credit - Double.Parse(stringValue)) < 0.001),
            WorkUnitRowColumn.BaseCredit => q.Where(x => Math.Abs(x.Protein.Credit - Double.Parse(stringValue)) < 0.001),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion == stringValue),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem == stringValue),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation == stringValue),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor == stringValue),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads == Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion == stringValue),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion == stringValue),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion == stringValue),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereGreaterThan(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID > Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun > Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone > Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen > Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam > Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted > Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds > Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned > (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished > (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => x.Protein.KFactor > Double.Parse(stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames > Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms > Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.PPD => q.Where(x => x.PPD > Double.Parse(stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => x.Credit > Double.Parse(stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => x.Protein.Credit > Double.Parse(stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads > Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion.CompareTo(stringValue) > 0),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion.CompareTo(stringValue) > 0),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereGreaterThanOrEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned >= (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished >= (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => x.Protein.KFactor >= Double.Parse(stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.PPD => q.Where(x => x.PPD >= Double.Parse(stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => x.Credit >= Double.Parse(stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => x.Protein.Credit >= Double.Parse(stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads >= Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion.CompareTo(stringValue) >= 0),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion.CompareTo(stringValue) >= 0),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereLessThan(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID < Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun < Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone < Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen < Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam < Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted < Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds < Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned < (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished < (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => x.Protein.KFactor < Double.Parse(stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames < Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms < Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.PPD => q.Where(x => x.PPD < Double.Parse(stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => x.Credit < Double.Parse(stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => x.Protein.Credit < Double.Parse(stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads < Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion.CompareTo(stringValue) < 0),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion.CompareTo(stringValue) < 0),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereLessThanOrEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned <= (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished <= (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => x.Protein.KFactor <= Double.Parse(stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.PPD => q.Where(x => x.PPD <= Double.Parse(stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => x.Credit <= Double.Parse(stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => x.Protein.Credit <= Double.Parse(stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads <= Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion.CompareTo(stringValue) <= 0),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion.CompareTo(stringValue) <= 0),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereLike(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => EF.Functions.Like(x.Protein.ProjectID.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => EF.Functions.Like(x.ProjectRun.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => EF.Functions.Like(x.ProjectClone.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => EF.Functions.Like(x.ProjectGen.ToString(), stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => EF.Functions.Like(x.SlotName, stringValue)),
            WorkUnitRowColumn.ConnectionString => q.Where(x => EF.Functions.Like(x.Client.ConnectionString, stringValue)),
            WorkUnitRowColumn.DonorName => q.Where(x => EF.Functions.Like(x.DonorName, stringValue)),
            WorkUnitRowColumn.DonorTeam => q.Where(x => EF.Functions.Like(x.DonorTeam.ToString(), stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => EF.Functions.Like(x.CoreVersion, stringValue)),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => EF.Functions.Like(x.FramesCompleted.ToString(), stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => EF.Functions.Like(x.FrameTimeInSeconds.ToString(), stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => EF.Functions.Like(x.Result, stringValue)),
            WorkUnitRowColumn.Assigned => throw new InvalidOperationException("Like is not supported by date time columns"),
            WorkUnitRowColumn.Finished => throw new InvalidOperationException("Like is not supported by date time columns"),
            WorkUnitRowColumn.KFactor => q.Where(x => EF.Functions.Like(x.Protein.KFactor.ToString(), stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => EF.Functions.Like(x.Protein.Core, stringValue)),
            WorkUnitRowColumn.Frames => q.Where(x => EF.Functions.Like(x.Protein.Frames.ToString(), stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => EF.Functions.Like(x.Protein.Atoms.ToString(), stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => EF.Functions.Like(x.SlotType, stringValue)),
            WorkUnitRowColumn.PPD => q.Where(x => EF.Functions.Like(x.PPD.ToString(), stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => EF.Functions.Like(x.Credit.ToString(), stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => EF.Functions.Like(x.Protein.Credit.ToString(), stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => EF.Functions.Like(x.Platform.ClientVersion, stringValue)),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => EF.Functions.Like(x.Platform.ClientVersion, stringValue)),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => EF.Functions.Like(x.Platform.Implementation, stringValue)),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => EF.Functions.Like(x.Platform.Processor, stringValue)),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => EF.Functions.Like(x.Platform.Threads.ToString(), stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => EF.Functions.Like(x.Platform.DriverVersion, stringValue)),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => EF.Functions.Like(x.Platform.ComputeVersion, stringValue)),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => EF.Functions.Like(x.Platform.CUDAVersion, stringValue)),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereNotLike(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => !EF.Functions.Like(x.Protein.ProjectID.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => !EF.Functions.Like(x.ProjectRun.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => !EF.Functions.Like(x.ProjectClone.ToString(), stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => !EF.Functions.Like(x.ProjectGen.ToString(), stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => !EF.Functions.Like(x.SlotName, stringValue)),
            WorkUnitRowColumn.ConnectionString => q.Where(x => !EF.Functions.Like(x.Client.ConnectionString, stringValue)),
            WorkUnitRowColumn.DonorName => q.Where(x => !EF.Functions.Like(x.DonorName, stringValue)),
            WorkUnitRowColumn.DonorTeam => q.Where(x => !EF.Functions.Like(x.DonorTeam.ToString(), stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => !EF.Functions.Like(x.CoreVersion, stringValue)),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => !EF.Functions.Like(x.FramesCompleted.ToString(), stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => !EF.Functions.Like(x.FrameTimeInSeconds.ToString(), stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => !EF.Functions.Like(x.Result, stringValue)),
            WorkUnitRowColumn.Assigned => throw new InvalidOperationException("Not Like is not supported by date time columns"),
            WorkUnitRowColumn.Finished => throw new InvalidOperationException("Not Like is not supported by date time columns"),
            WorkUnitRowColumn.KFactor => q.Where(x => !EF.Functions.Like(x.Protein.KFactor.ToString(), stringValue)),
            WorkUnitRowColumn.Core => q.Where(x => !EF.Functions.Like(x.Protein.Core, stringValue)),
            WorkUnitRowColumn.Frames => q.Where(x => !EF.Functions.Like(x.Protein.Frames.ToString(), stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => !EF.Functions.Like(x.Protein.Atoms.ToString(), stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => !EF.Functions.Like(x.SlotType, stringValue)),
            WorkUnitRowColumn.PPD => q.Where(x => !EF.Functions.Like(x.PPD.ToString(), stringValue)),
            WorkUnitRowColumn.Credit => q.Where(x => !EF.Functions.Like(x.Credit.ToString(), stringValue)),
            WorkUnitRowColumn.BaseCredit => q.Where(x => !EF.Functions.Like(x.Protein.Credit.ToString(), stringValue)),
            WorkUnitRowColumn.ClientVersion => q.Where(x => !EF.Functions.Like(x.Platform.ClientVersion, stringValue)),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => !EF.Functions.Like(x.Platform.ClientVersion, stringValue)),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => !EF.Functions.Like(x.Platform.Implementation, stringValue)),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => !EF.Functions.Like(x.Platform.Processor, stringValue)),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => !EF.Functions.Like(x.Platform.Threads.ToString(), stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => !EF.Functions.Like(x.Platform.DriverVersion, stringValue)),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => !EF.Functions.Like(x.Platform.ComputeVersion, stringValue)),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => !EF.Functions.Like(x.Platform.CUDAVersion, stringValue)),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }

    private static IQueryable<WorkUnitEntity> WhereNotEqual(IQueryable<WorkUnitEntity> source, WorkUnitQueryParameter parameter)
    {
        var q = source;
        string stringValue = parameter.Value as string;
        return parameter.Column switch
        {
            WorkUnitRowColumn.ID => throw new NotImplementedException(),
            WorkUnitRowColumn.ProjectID => q.Where(x => x.Protein.ProjectID != Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectRun => q.Where(x => x.ProjectRun != Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectClone => q.Where(x => x.ProjectClone != Int32.Parse(stringValue)),
            WorkUnitRowColumn.ProjectGen => q.Where(x => x.ProjectGen != Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotName => q.Where(x => x.SlotName != stringValue),
            WorkUnitRowColumn.ConnectionString => q.Where(x => x.Client.ConnectionString != stringValue),
            WorkUnitRowColumn.DonorName => q.Where(x => x.DonorName != stringValue),
            WorkUnitRowColumn.DonorTeam => q.Where(x => x.DonorTeam != Int32.Parse(stringValue)),
            WorkUnitRowColumn.CoreVersion => q.Where(x => x.CoreVersion != stringValue),
            WorkUnitRowColumn.FramesCompleted => q.Where(x => x.FramesCompleted != Int32.Parse(stringValue)),
            WorkUnitRowColumn.FrameTime => q.Where(x => x.FrameTimeInSeconds != Int32.Parse(stringValue)),
            WorkUnitRowColumn.Result => q.Where(x => x.Result != stringValue),
            WorkUnitRowColumn.Assigned => q.Where(x => x.Assigned != (DateTime)parameter.Value),
            WorkUnitRowColumn.Finished => q.Where(x => x.Finished != (DateTime)parameter.Value),
            WorkUnitRowColumn.KFactor => q.Where(x => Math.Abs(x.Protein.KFactor - Double.Parse(stringValue)) > 0.001),
            WorkUnitRowColumn.Core => q.Where(x => x.Protein.Core != stringValue),
            WorkUnitRowColumn.Frames => q.Where(x => x.Protein.Frames != Int32.Parse(stringValue)),
            WorkUnitRowColumn.Atoms => q.Where(x => x.Protein.Atoms != Int32.Parse(stringValue)),
            WorkUnitRowColumn.SlotType => q.Where(x => x.SlotType != stringValue),
            WorkUnitRowColumn.PPD => q.Where(x => Math.Abs(x.PPD - Double.Parse(stringValue)) > 0.001),
            WorkUnitRowColumn.Credit => q.Where(x => Math.Abs(x.Credit - Double.Parse(stringValue)) > 0.001),
            WorkUnitRowColumn.BaseCredit => q.Where(x => Math.Abs(x.Protein.Credit - Double.Parse(stringValue)) > 0.001),
            WorkUnitRowColumn.ClientVersion => q.Where(x => x.Platform.ClientVersion != stringValue),
            WorkUnitRowColumn.OperatingSystem => q.Where(x => x.Platform.OperatingSystem != stringValue),
            WorkUnitRowColumn.PlatformImplementation => q.Where(x => x.Platform.Implementation != stringValue),
            WorkUnitRowColumn.PlatformProcessor => q.Where(x => x.Platform.Processor != stringValue),
            WorkUnitRowColumn.PlatformThreads => q.Where(x => x.Platform.Threads != Int32.Parse(stringValue)),
            WorkUnitRowColumn.DriverVersion => q.Where(x => x.Platform.DriverVersion != stringValue),
            WorkUnitRowColumn.ComputeVersion => q.Where(x => x.Platform.ComputeVersion != stringValue),
            WorkUnitRowColumn.CUDAVersion => q.Where(x => x.Platform.CUDAVersion != stringValue),
            _ => throw new InvalidOperationException($"Column {parameter.Column} is not supported."),
        };
    }
}
