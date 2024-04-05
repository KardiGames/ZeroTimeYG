using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskByTimer
{

    public string TaskName { get;}
    public string Description { get; }
    public string TaskTag { get; }
    public ITimerable Source { get; }
    public DateTime FinishTime { get; private set; } = new();
    public float secondsToFinish { get; private set; }


    public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name="Task Default Name", string desccription="")
    {
        Source = source;
        this.secondsToFinish = secondsToFinish < 1 ? 1 : secondsToFinish;
        this.TaskName = name;
        this.Description = desccription;
        this.TaskTag = tag;
    }
	
	public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name, string desccription, DateTime finishTime)
    {
        Source = source;
        this.secondsToFinish = secondsToFinish < 1 ? 1 : secondsToFinish;
        this.TaskName = name;
        this.Description = desccription;
        this.TaskTag = tag;
		this.FinishTime = finishTime;
    }

    public bool IsStarted() => secondsToFinish < 0;

    public bool TryToStart () => TryToStart(DateTime.Now);

    public bool TryToStart(DateTime startTime)
    {
        if (startTime>DateTime.Now)
			return false;
        if (!Source.TaskTimer.PossibleToStart(this))
            return false;
		
		FinishTime = startTime.AddSeconds(secondsToFinish);
        secondsToFinish = -1;
        return true;
    }

    public void Pause ()
    {
        secondsToFinish = (float)(FinishTime - DateTime.Now).TotalSeconds;
    }
}