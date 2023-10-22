using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskByTimer
{
    public string TaskTag { get; }
    public string TaskName { get;}
    public string Description { get; }
    
    public ITimerable Source { get; }
    public DateTime FinishTime { get; private set; }
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
        OnPause = false;
    }

    public bool IsStarted() => secondsToFinish < 0;

    public void StartTask () => StartTask(DateTime.Now);

    public void StartTask(DateTime startTime)
    {
        if (startTime>DateTime.Now)
			return;
		
		FinishTime = startTime.AddSeconds(secondsToFinish);
        secondsToFinish = -1;
		OnPause=false;
    }
	
	public void SetPause() {
		if (IsStarted()) {
            secondsToFinish = (float)(FinishTime - DateTime.Now).TotalSeconds;
			OnPause=true;
		}
	}
}