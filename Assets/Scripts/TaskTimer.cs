using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTimer : MonoBehaviour
{
    public event Action OnTaskOrTimerChanged;
	
    private int simultaneouslyTasks = 1;
    private int maximumTasks = 1;
    private List<TaskByTimer> tasksList= new();


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

    public void SetupTaskTimer(int maximumTasks = 1, int simultaneously = 1)
    {

        this.simultaneouslyTasks = simultaneously;
        this.maximumTasks = maximumTasks;
		
		CompletePastTasks();
    }

    public void CompletePastTasks()
    {
        List<TaskByTimer> completedTasks = new();
        int startedTasks = 0;
        DateTime finishedTaskTime;
		bool changesHappened = false;

        for (int i = 0; (startedTasks <= simultaneouslyTasks && i < tasksList.Count); i++)
        {
            if (tasksList[i].IsStarted())
            {
                if (tasksList[i].FinishTime < DateTime.Now)
                {
                    tasksList[i].Source.ApplyActionByTimer(tasksList[i]);
                    finishedTaskTime = tasksList[i].FinishTime;
                    tasksList.Remove(tasksList[i]);
					changesHappened=true;
                    StartQueuedTask(finishedTaskTime, false);
					i--;
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
                tasksList[i].TryToStartTask(startTime);
				OnTaskOrTimerChanged?.Invoke();
                return;
            }
    }
	
	public void AddTask (ITimerable source, float secondsToFinish, string tag, bool startImmediately=false) {
		AddTask (new TaskByTimer (source, secondsToFinish, tag), startImmediately);
	}
	
	public void AddTask (TaskByTimer newTask, bool startImmediately=false) {
		if (newTask == null) 
			return;
		if (tasksList.Count >= MaximumTasks)
        {
			print("Error. You were trying more tasks than Maximum for tasklist");
			return;
        }

		tasksList.Add(newTask);
		
		if (startImmediately==true) 
		{
			if (newTask.TryToStartTask())
            {
				MoveUp(tasksList.Count-1, true);
            }
		} 
		OnTaskOrTimerChanged?.Invoke();
	}

	public bool PossibleToStart(TaskByTimer task)
    {
		if (!tasksList.Contains(task)
			|| StartedTasks >= simultaneouslyTasks)
			return false;
		return true;
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
	
	public void MoveDown (int indexOfTask, bool toTheBottom=false, int distance=1) {
		if (distance<1 || indexOfTask<0 || indexOfTask>=tasksList.Count)
			return;
		
		TaskByTimer task = tasksList[indexOfTask];
		int newIndex = indexOfTask+distance-1;
		tasksList.RemoveAt(indexOfTask);
		
		if (tasksList.Contains(task))
			return;
		
		if (newIndex==tasksList.Count || toTheBottom)
			tasksList.Add(task);
		else if (newIndex<tasksList.Count)
			tasksList.Insert(newIndex, task);
		
		OnTaskOrTimerChanged?.Invoke();
	}

	public void MoveDown(TaskByTimer task) => MoveDown(tasksList.IndexOf(task));
	
	public TaskByTimer[] GetAllItems () {
		return tasksList.ToArray();	
	}

	public bool Contains(TaskByTimer task) => tasksList.Contains(task);

	public void ClearTaskTimer()
    {
		tasksList.Clear();
		OnTaskOrTimerChanged?.Invoke();
		OnTaskOrTimerChanged = null;
	}

	public string ToJson(ITimerable sourceToCompare)
	{
		TaskTimerJsonData jsonTimerData = new() { simultaneouslyTasks = simultaneouslyTasks, maximumTasks = maximumTasks};
		for (int i=0; i<tasksList.Count; i++)
			if (tasksList[i].Source==sourceToCompare)
            {
				TaskByTimerJsonData jsonTaskData = new(tasksList[i]);
				jsonTimerData.taskByTimerJsonList.Add(JsonUtility.ToJson(jsonTaskData));
            }
		return JsonUtility.ToJson(jsonTimerData);
        
	}

	public void FromJson(string jsonString, ITimerable sourceToSet)
	{
		tasksList.Clear();
		TaskTimerJsonData jsonTimer = JsonUtility.FromJson<TaskTimerJsonData>(jsonString);
		if (jsonTimer == null)
			return;
		simultaneouslyTasks=jsonTimer.simultaneouslyTasks;
		maximumTasks=jsonTimer.maximumTasks;
		
		TaskByTimerJsonData taskDataToAdd=new();
		for (int i=0; i<jsonTimer.taskByTimerJsonList.Count; i++)
        {
			taskDataToAdd = JsonUtility.FromJson<TaskByTimerJsonData>(jsonTimer.taskByTimerJsonList[i]);
			if (taskDataToAdd!=null) 
				tasksList.Add(new TaskByTimer(
					sourceToSet, 
					taskDataToAdd.secondsToFinish, 
					taskDataToAdd.TaskTag, 
					taskDataToAdd.TaskName, 
					taskDataToAdd.Description, 
					taskDataToAdd.OnPause, 
					DateTime.ParseExact(taskDataToAdd.FinishTime,"ddMMyyyyHHmmss",null)
				)); //TODO Make test if deserialization error
        }
	}

	[Serializable]
	protected class TaskTimerJsonData
	{
		public int simultaneouslyTasks = 0;
		public int maximumTasks = 0;
		public List<string> taskByTimerJsonList=new();
	}

	[Serializable]
	protected class TaskByTimerJsonData
    {
		public string TaskName;
		public string Description;
		public string TaskTag;
		public string FinishTime;
		public float secondsToFinish;
		public bool OnPause;
		public TaskByTimerJsonData(TaskByTimer taskToConvert)
        {
			TaskName = taskToConvert.TaskName;
			Description = taskToConvert.Description;
			TaskTag = taskToConvert.TaskTag;
			FinishTime = taskToConvert.FinishTime.ToString("ddMMyyyyHHmmss");
			secondsToFinish = taskToConvert.secondsToFinish;
			OnPause = taskToConvert.OnPause;
		}
		public TaskByTimerJsonData() { }
	}
}
