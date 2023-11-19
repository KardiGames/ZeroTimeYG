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
	public bool OnPause {get; private set; }

    public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name="Task Default Name", string desccription="")
    {
        Source = source;
        secondsToFinish = secondsToFinish < 1 ? 1 : secondsToFinish;
        this.secondsToFinish = secondsToFinish;
        this.TaskName = name;
        this.Description = desccription;
        this.TaskTag = tag;
        this.OnPause = false;
    }
	public TaskByTimer(ITimerable source, float secondsToFinish, string tag, string name, string desccription, bool onPause, DateTime finishTime)
    {
        Source = source;
        this.secondsToFinish = secondsToFinish < 1 ? 1 : secondsToFinish;
        this.secondsToFinish = secondsToFinish;
        this.TaskName = name;
        this.Description = desccription;
        this.TaskTag = tag;
        this.OnPause = onPause;
		this.FinishTime = finishTime;
    }

    public bool IsStarted() => secondsToFinish < 0;

    public bool TryToStartTask () => TryToStartTask(DateTime.Now);

    public bool TryToStartTask(DateTime startTime)
    {
        if (startTime>DateTime.Now)
			return false;
        if (!Source.TaskTimer.PossibleToStart(this))
            return false;
		
		FinishTime = startTime.AddSeconds(secondsToFinish);
        secondsToFinish = -1;
		OnPause=false;
        return true;
    }
	
	public void SetPause() {
		if (IsStarted()) {
            secondsToFinish = (float)(FinishTime - DateTime.Now).TotalSeconds;
			OnPause=true;
		}
	}
  
}