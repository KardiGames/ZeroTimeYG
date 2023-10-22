using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTimer : MonoBehaviour
{
    public event Action OnTaskOrTimerChanged;
	
	[SerializeField] ScriptableItem testScriptItemToCreate; //DODO just 4 Test
	
	private ITimerable source;
    private int simultaneouslyTasks = 1;
    private int maximumTasks = 1;
    private List<TaskByTimer> tasksList { get; } = new();


    public int SimultaniouslyTasks
    {
        get => simultaneouslyTasks;
        set
        {
            if (value > 0)
                simultaneouslyTasks = value;
        }
    }

    public int MaximumTasks
    {
        get => maximumTasks; set
        {
            if (value > 0)
                maximumTasks = value;
        }
    }

    public int StartedTasks
    {
        get
        {
            int started = 0;
            foreach (var task in tasksList)
                if (task.IsStarted())
                    started++;
            return started;
        }
    }

    private void Start()
    {
		ITimerable factory = gameObject.GetComponent<Factory>();
		SetupTaskTimer(factory, 5,2);
        TaskByTimer task1 = new(factory, 3, "1", "New Task #1", "some description");
		AddTask(task1);
		TaskByTimer task2 = new(factory, 10, "2", "New Task #2", "some description");
		AddTask(task2);
		TaskByTimer task3 = new(factory, 5, "3", "New Task #3");
		AddTask(task3);
	
	}

    public void SetupTaskTimer(ITimerable source, int maximumTasks = 1, int simultaneously = 1)
    {
        if (source != null)
            return;
        this.source = source;
        this.simultaneouslyTasks = simultaneously;
        this.maximumTasks = maximumTasks;

    }

    public void CheckForCompletion()
    {
        List<TaskByTimer> completedTasks = new();
        int startedTasks = 0;
        DateTime finishedTaskTime;
		bool changesHappened = false;

        for (int i = 0; (startedTasks <= simultaneouslyTasks && i < tasksList.Count); i++)
        {
            if (tasksList[i].IsStarted())
            {
                if (tasksList[i].FinishTime >= DateTime.Now)
                {
                    tasksList[i].Source.ApplyActionByTimer(tasksList[i]);
                    finishedTaskTime = tasksList[i].FinishTime;
                    tasksList.Remove(tasksList[i]);
					changesHappened=true;
                    StartQueuedTask(finishedTaskTime, false, i--);
                }
                else
                    startedTasks++;
            }
        }
		
		if (changesHappened)
			OnTaskOrTimerChanged?.Invoke();

    }

    private void StartQueuedTask(DateTime startTime, bool checkForMaximum = true, int startIndex = 0)
    {
        if (checkForMaximum && StartedTasks >= simultaneouslyTasks)
            return;
		
		if (startIndex<0)
			return;

        for (int i = startIndex; i < tasksList.Count; i++)
            if (!tasksList[i].IsStarted() && !tasksList[i].OnPause)
            {
                tasksList[i].StartTask(startTime);
				OnTaskOrTimerChanged?.Invoke();
                return;
            }
    }
	
	private void AddTask (ITimerable source, float secondsToFinish, string tag, bool startImmediately=false) {
		AddTask (new TaskByTimer (source, secondsToFinish, tag), startImmediately);
	}
	
	public void AddTask (TaskByTimer newTask, bool startImmediately=false) {
		if (newTask == null) 
			return;
		tasksList.Add(newTask);
		
		if (startImmediately==true) 
		{
			if (TryToStartTask (newTask))
				MoveUp(tasksList.Count-1, true);
		} else
			OnTaskOrTimerChanged?.Invoke();
	}
	
	private bool TryToStartTask (TaskByTimer task) {
		if (!tasksList.Contains(task)
			|| StartedTasks >= simultaneouslyTasks)
			return false;
			
		task.StartTask();
		OnTaskOrTimerChanged?.Invoke();
		return task.IsStarted();
	}
	
	public void MoveUp (int indexOfTask, bool toClosestStarted=false, int distance=1) {
		if (distance<1)
			return;
		
		int newIndex = indexOfTask-distance;
		TaskByTimer task=tasksList[indexOfTask];
		
		if (toClosestStarted) {
			for(int i=indexOfTask-1; i>=0; i--)
				if (tasksList[i].IsStarted() || tasksList[i].OnPause) {
					newIndex=i+1;
					break;
				}
		}
		
		if (newIndex<0) 
			newIndex=0;
		
		tasksList.RemoveAt(indexOfTask);
		if (!tasksList.Contains(task) && newIndex<tasksList.Count)
			tasksList.Insert(newIndex, task);
		OnTaskOrTimerChanged?.Invoke();
	}
	
	public void MoveUp(TaskByTimer task) => MoveUp(tasksList.IndexOf(task));
	
	public void MoveDown (int indexOfTask, bool toClosestStarted=false, int distance=1) {
		if (distance<1 || indexOfTask<0 || indexOfTask>=tasksList.Count)
			return;
		
		TaskByTimer task = tasksList[indexOfTask];
		int newIndex = indexOfTask+distance;
		tasksList.RemoveAt(indexOfTask);
		
		if (tasksList.Contains(task))
			return;
		
		if (newIndex==tasksList.Count)
			tasksList.Add(task);
		else if (newIndex<tasksList.Count)
			tasksList.Insert(newIndex, task);
		
		OnTaskOrTimerChanged?.Invoke();
	}

	public void MoveDown(TaskByTimer task) => MoveDown(tasksList.IndexOf(task));
	
	public TaskByTimer[] GetAllItems () {
		return tasksList.ToArray();	
	}	
	
}
