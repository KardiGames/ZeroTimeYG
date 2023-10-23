using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour, ITimerable
{
    private Dictionary<TaskByTimer, ScriptableItem> factoryLines = new();
    private TaskTimer _taskTimer;
	[SerializeField] private Inventory storage;
	
	public TaskTimer TaskTimer => _taskTimer;
	
	public void ApplyActionByTimer(TaskByTimer action)
    {
        if (action.Source != this) {
            print("INCORRECT ACTION");
			return;
		}
		
		bool check = storage.TryToAdd(this, factoryLines[action].Clone());
		if (check) 
			factoryLines.Remove(action);
    }

    // Start is called before the first frame update
    public void Start()
    {
        _taskTimer = GetComponent<TaskTimer>();

    }

    public void StartProduction(Blueprint blueprint)
    {
        //TODO check IsAnoughSkill
		
		if (!blueprint.IsAnoughResourses(storage))
			return;

		SpendResources(blueprint);

		TaskByTimer productionTask = new(this, blueprint.SecondsToFinish, "", blueprint.ItemToCreate.ItemName, "Produce "+blueprint.ItemToCreate.Amount+ " "+ blueprint.ItemToCreate.ItemName);
		if (productionTask==null)
			return;
        
		TaskTimer.AddTask(productionTask);
		factoryLines.Add(productionTask, blueprint.ItemToCreate);
		Destroy (blueprint);
    }

	private void SpendResources(Blueprint blueprint)
	{
		//TODO think. m.b. check IsAnoughResourses

		List<ScriptableItem> resoursesList = blueprint.ListOfResourses;
		List<long> resoursesAmounts = blueprint.AmountsOfResourses;
		for (int i = 0; i < resoursesList.Count; i++)
		{
			storage.Remove(this, resoursesList[i], resoursesAmounts[i]);
		}
	}
}
