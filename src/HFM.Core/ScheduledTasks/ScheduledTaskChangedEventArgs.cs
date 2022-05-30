namespace HFM.Core.ScheduledTasks;

public class ScheduledTaskChangedEventArgs : EventArgs
{
    public ScheduledTask Source { get; }

    public ScheduledTaskChangedAction Action { get; }

    public double? Interval { get; }

    public ScheduledTaskChangedEventArgs(ScheduledTask source, ScheduledTaskChangedAction action, double? interval)
    {
        Source = source;
        Action = action;
        Interval = interval;
    }

    public override string ToString()
    {
        return ToString(i => $"{i:#,##0} ms");
    }

    public string ToString(Func<double?, string> formatInterval)
    {
        string key = Source.Name;
        switch (Action)
        {
            case ScheduledTaskChangedAction.Started:
                return $"{key} task scheduled: {formatInterval(Interval)}";
            case ScheduledTaskChangedAction.Stopped:
                return $"{key} task stopped";
            case ScheduledTaskChangedAction.Running:
                return $"{key} task running";
            case ScheduledTaskChangedAction.Canceled:
                return $"{key} task canceled";
            case ScheduledTaskChangedAction.Faulted:
                return $"{key} task faulted: {Interval:#,##0} ms {Source.Exception}";
            case ScheduledTaskChangedAction.Finished:
                return $"{key} task finished: {Interval:#,##0} ms";
            case ScheduledTaskChangedAction.AlreadyInProgress:
                return $"{key} task already in progress";
            default:
                return base.ToString();
        }
    }
}
