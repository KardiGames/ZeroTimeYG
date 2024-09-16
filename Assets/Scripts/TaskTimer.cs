using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskTimer : MonoBehaviour
{
    public event Action OnTaskOrTimerChanged;
	
    private int _simultaneouslyTasks = 1;
    private int _maximumTasks = 1;
    private List<TaskByTimer> _tasksList= new();


    public int SimultaniouslyTasks
    {
        get => _simultaneouslyTasks;
        private set
        {
            if (value > 0)
                _simultaneouslyTasks = value;
        }
    }

    public int MaximumTasks
    {
        get => _maximumTasks; private set
        {
            if (value > 0)
                _maximumTasks = value;
        }
    }

    public int StartedTasks
    {
        get
        {
            int started = 0;
            foreach (var task in _tasksList)
                if (task.IsStarted())
                    started++;
            return started;
        }
    }

    public void SetupTaskTimer(int maximumTasks = 1, int simultaneously = 1)
    {

        SimultaniouslyTasks = simultaneously;
        MaximumTasks = maximumTasks;
		
		CompletePastTasks();
    }

    public void CompletePastTasks()
    {
        List<TaskByTimer> completedTasks = new();
        int startedTasks = 0;
        DateTime finishedTaskTime;
		bool changesHappened = false;

        for (int i = 0; (startedTasks <= _simultaneouslyTasks && i < _tasksList.Count); i++)
        {
            if (_tasksList[i].IsStarted())
            {
                if (_tasksList[i].FinishTime < DateTime.Now)
                {
                    _tasksList[i].Source.ApplyActionByTimer(_tasksList[i]);
                    finishedTaskTime = _tasksList[i].FinishTime;
                    _tasksList.RemoveAt(i);
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
        if (checkForMaximum && StartedTasks >= _simultaneouslyTasks)
            return;
		
		if (startIndex<0)
			return;

        for (int i = startIndex; i < _tasksList.Count; i++)
            if (!_tasksList[i].IsStarted())
            {
                _tasksList[i].TryToStart(startTime);
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
		if (_tasksList.Count >= MaximumTasks)
        {
			print("Error. You were trying more tasks than Maximum for tasklist");
			return;
        }

		_tasksList.Add(newTask);
		
		if (startImmediately==true) 
		{
			if (newTask.TryToStart())
            {
				MoveUp(_tasksList.Count-1, true);
            }
		} 
		OnTaskOrTimerChanged?.Invoke();
	}

	public bool IsPossibleToAdd() => _maximumTasks > _tasksList.Count;

	public bool PossibleToStart(TaskByTimer task)
    {
		if (!_tasksList.Contains(task)
			|| StartedTasks >= _simultaneouslyTasks)
			return false;
		return true;
	}
	
	public void MoveUp (int indexOfTask, bool toClosestStarted=false, int distance=1) {
		if (distance<1)
			return;
		
		int newIndex = indexOfTask-distance;
		TaskByTimer task=_tasksList[indexOfTask];
		
		if (toClosestStarted) {
			for(int i=indexOfTask-1; i>=0; i--)
				if (_tasksList[i].IsStarted()) {
					newIndex=i+1;
					break;
				}
		}
		
		if (newIndex<0) 
			newIndex=0;

		if (newIndex == indexOfTask)
			return;

		_tasksList.RemoveAt(indexOfTask);
		if (!_tasksList.Contains(task) && newIndex<_tasksList.Count)
			_tasksList.Insert(newIndex, task);
		OnTaskOrTimerChanged?.Invoke();
	}
	
	public void MoveUp(TaskByTimer task) => MoveUp(_tasksList.IndexOf(task));
	
	public void MoveDown (int indexOfTask, bool toTheBottom=false, int distance=1) {
		if (distance<1 || indexOfTask<0 || indexOfTask>=_tasksList.Count)
			return;
		
		TaskByTimer task = _tasksList[indexOfTask];
		int newIndex = indexOfTask+distance-1;
		_tasksList.RemoveAt(indexOfTask);
		
		if (_tasksList.Contains(task))
			return;
		
		if (newIndex==_tasksList.Count || toTheBottom)
			_tasksList.Add(task);
		else if (newIndex<_tasksList.Count)
			_tasksList.Insert(newIndex, task);
		
		OnTaskOrTimerChanged?.Invoke();
	}

	public void MoveDown(TaskByTimer task) => MoveDown(_tasksList.IndexOf(task));
	
	public TaskByTimer[] GetAllItems () {
		return _tasksList.ToArray();	
	}

	public bool Contains(TaskByTimer task) => _tasksList.Contains(task);

	public void ClearTaskTimer()
    {
		_tasksList.Clear();
		OnTaskOrTimerChanged?.Invoke();
		OnTaskOrTimerChanged = null;
	}

	public string ToJson(ITimerable sourceToCompare)
	{
		TaskTimerJsonData jsonTimerData = new() { simultaneouslyTasks = _simultaneouslyTasks, maximumTasks = _maximumTasks};
		for (int i=0; i<_tasksList.Count; i++)
			if (_tasksList[i].Source==sourceToCompare)
            {
				TaskByTimerJsonData jsonTaskData = new(_tasksList[i]);
				jsonTimerData.taskByTimerJsonList.Add(JsonUtility.ToJson(jsonTaskData));
            }
		return JsonUtility.ToJson(jsonTimerData);
        
	}

	public void FromJson(string jsonString, ITimerable sourceToSet)
	{
		print(jsonString);
		_tasksList.Clear();
		TaskTimerJsonData jsonTimer = JsonUtility.FromJson<TaskTimerJsonData>(jsonString);
		if (jsonTimer == null)
        {
			print("Error! TaskTimer tryed to load, but in haven't happend ( Loaded empty basic");
			return;
        }
		_simultaneouslyTasks=jsonTimer.simultaneouslyTasks;
		_maximumTasks=jsonTimer.maximumTasks;
		
		TaskByTimerJsonData taskDataToAdd;
		for (int i=0; i<jsonTimer.taskByTimerJsonList.Count; i++)
        {
			taskDataToAdd = JsonUtility.FromJson<TaskByTimerJsonData>(jsonTimer.taskByTimerJsonList[i]);
			if (taskDataToAdd!=null) 
				_tasksList.Add(new TaskByTimer(
					sourceToSet,
					taskDataToAdd.secondsToFinish,
					taskDataToAdd.TaskTag,
					taskDataToAdd.TaskName,
					taskDataToAdd.Description,
					DateTime.ParseExact(taskDataToAdd.FinishTime, "ddMMyyyyHHmmss", null)
					)
				); //TODO Make test if deserialization error
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

		public TaskByTimerJsonData(TaskByTimer taskToConvert)
        {
			TaskName = taskToConvert.TaskName;
			Description = taskToConvert.Description;
			TaskTag = taskToConvert.TaskTag;
			FinishTime = taskToConvert.FinishTime.ToString("ddMMyyyyHHmmss");
			secondsToFinish = taskToConvert.SecondsToFinish;
		}
		public TaskByTimerJsonData() { }
	}
}
