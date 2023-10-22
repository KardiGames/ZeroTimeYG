public interface ITimerable
{
    public TaskTimer TaskTimer { get; }
    public void ApplyActionByTimer(TaskByTimer action);
}
