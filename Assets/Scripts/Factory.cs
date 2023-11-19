using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour, ITimerable, IWorldBuilding
{
	[SerializeField] SaveData _saveSystem;
	
	[SerializeField] private Inventory _storage;
	private TaskTimer _taskTimer;

	public int X { get; private set; } = 0;
	public int Y { get; private set; } = 0;
	[SerializeField] private string _name;
	

	private Dictionary<TaskByTimer, ScriptableItem> factoryLines = new();
		
	public TaskTimer TaskTimer => _taskTimer;
	public string Name => _name;
	
	public void ApplyActionByTimer(TaskByTimer action)
    {
        if (action.Source != this) {
            print("INCORRECT ACTION");
			return;
		}
		
		bool check = _storage.TryToAdd(this, factoryLines[action].Clone());
		if (check) 
			factoryLines.Remove(action);
    }

    // Start is called before the first frame update
    public void Start()
    {
        _taskTimer = GetComponent<TaskTimer>();
		_taskTimer.SetupTaskTimer(5, 2);
		for (int i=0; i<8;i++)
			_storage.TryToAdd(this, ScriptableItem.GetItem("Resource"));
		_storage.TryToAdd(this, ScriptableItem.GetItem("Pistol Blueprint"));
		GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, ScriptableItem.GetItem("Pistol Blueprint"));
		GameObject.Find("PlayerCharacter").GetComponent<Inventory>().TryToAdd(this, ScriptableItem.GetItem("Steel Armor"));
	}

    public void AddFactoryLine(Blueprint blueprint, bool startProductionImmediately = false)
    {
        //TODO check IsAnoughSkill
		
		if (!_storage.Contains(blueprint) || !blueprint.IsAnoughResourses(_storage))
			return;

		TaskByTimer productionTask = new(this, blueprint.SecondsToFinish, "", blueprint.ItemToCreate.ItemName, "Produce "+blueprint.ItemToCreate.Amount+ " "+ blueprint.ItemToCreate.ItemName);
		if (productionTask==null)
			return;
        
		TaskTimer.AddTask(productionTask);
		if (TaskTimer.Contains(productionTask))
        {
			SpendResources(blueprint);
			factoryLines.Add(productionTask, blueprint.ItemToCreate);
			_storage.RemoveThisItem(this, blueprint);
			Destroy(blueprint);
		} else
        {
			print("Error. Factory line is not created");
        }
		
    }

	private void SpendResources(Blueprint blueprint)
	{
		//TODO think. m.b. check IsAnoughResourses

		List<ScriptableItem> resoursesList = blueprint.ListOfResourses;
		List<long> resoursesAmounts = blueprint.AmountsOfResourses;
		for (int i = 0; i < resoursesList.Count; i++)
		{
			_storage.Remove(this, resoursesList[i], resoursesAmounts[i]);
		}
	}

	public void ExitBuilding() 
	{
		_saveSystem.SaveBuilding(this);
		_storage.ClearInventory(this);
		_taskTimer.ClearTaskTimer();
		factoryLines.Clear();
		_name="";
	}

    public string ToJson()
    {
		FactoryJsonData jsonFactory = new() { x=X, y=Y, _name=_name};
		jsonFactory.storageJsonString=_storage.ToJson();

		TaskByTimer[] taskTimerArray = _taskTimer.GetAllItems();
		if (taskTimerArray.Any(tt => tt.Source != this) || taskTimerArray.Length != factoryLines.Count)
			return JsonUtility.ToJson(new { _name = "Error Factory Data 1" });

		jsonFactory.taskTimerJsonString = _taskTimer.ToJson(this);

		for (int i = 0; i < taskTimerArray.Length; i++)
			if (factoryLines.ContainsKey(taskTimerArray[i]))
            {
				jsonFactory.factoryJsonLines.Add(factoryLines[taskTimerArray[i]].ToJson());
				jsonFactory.factoryLinesTypes.Add(factoryLines[taskTimerArray[i]].GetType().Name);
			}
				
			else
				return JsonUtility.ToJson(new { _name = "Error Factory Data 2" });
		return JsonUtility.ToJson(jsonFactory);
    }

    public void FromJson(string jsonString)
    {
		factoryLines.Clear();
		
		FactoryJsonData jsonFactory = JsonUtility.FromJson<FactoryJsonData>(jsonString);
		X = jsonFactory.x;
		Y = jsonFactory.y;
		_name = jsonFactory._name;
		_storage.FromJson(jsonFactory.storageJsonString);
		_taskTimer.FromJson(jsonFactory.taskTimerJsonString, this);

		TaskByTimer[] taskTimerArray = _taskTimer.GetAllItems();
		ScriptableItem itemToLine;
		for (int i=0; i<taskTimerArray.Length; i++)
        {
			itemToLine = (ScriptableItem)ScriptableObject.CreateInstance(Type.GetType(jsonFactory.factoryLinesTypes[i]));
			if (itemToLine == null)
				continue;
			itemToLine.FromJson(jsonFactory.factoryJsonLines[i]);
			if (itemToLine != null)
				factoryLines.Add(taskTimerArray[i], itemToLine); //TODO Make test if deserialization error
        }
	}
	
	[Serializable]
	protected class FactoryJsonData
	{
	
		public string _name;
		public int x;
		public int y;
		public string storageJsonString;
		public string taskTimerJsonString;

		public List<string> factoryLinesTypes=new();
		public List<string> factoryJsonLines=new();
	
	}
}
