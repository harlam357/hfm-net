namespace HFM.Core.ScheduledTasks;

public enum ScheduledTaskChangedAction
{
    Started,
    Stopped,
    Running,
    Canceled,
    Faulted,
    Finished,
    AlreadyInProgress
}
