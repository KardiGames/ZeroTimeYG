using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskByTimer
{

    public string TaskName { get; }
    public string Description { get; }
    public string TaskTag { get; }
    public ITimerable Source { get; }
    public DateTime FinishTime { get; private set; } = new();
    public float SecondsToFinish { get; private set; }


    public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name="Task Default Name", string description="")
    {
        Source = source;
        SecondsToFinish = secondsToFinish < 1 ? 1 : secondsToFinish;
        TaskName = name;
        Description = description;
        TaskTag = tag;
    }
	
	public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name, string description, DateTime finishTime)
    {
        Source = source;
        SecondsToFinish = secondsToFinish < -1.1f ? 1 : secondsToFinish;
        TaskName = name;
        Description = description;
        TaskTag = tag;
		FinishTime = finishTime;
    }

    public bool IsStarted() => SecondsToFinish < 0;

    public bool TryToStart () => TryToStart(DateTime.Now);

    public bool TryToStart(DateTime startTime)
    {
        if (startTime>DateTime.Now)
			return false;
        if (!Source.TaskTimer.PossibleToStart(this))
            return false;
		
		FinishTime = startTime.AddSeconds(SecondsToFinish);
        SecondsToFinish = -1;
        return true;
    }

    public void Pause ()
    {
        SecondsToFinish = (float)(FinishTime - DateTime.Now).TotalSeconds;
    }
}