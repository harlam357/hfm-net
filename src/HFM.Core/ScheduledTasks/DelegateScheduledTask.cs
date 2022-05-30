namespace HFM.Core.ScheduledTasks;

public class DelegateScheduledTask : ScheduledTask
{
    public Action<CancellationToken> Action { get; }

    public DelegateScheduledTask(string name, Action<CancellationToken> action, double interval) : base(name, interval)
    {
        Action = action;
    }

    protected override void OnRun(CancellationToken ct)
    {
        Action(ct);
    }
}
