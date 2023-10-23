using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Blueprint", menuName = "Items/Blueprint")]
public class Blueprint : ScriptableItem
{
    public new long Amount => 1;
    public new bool Stackable => false;
    
    [SerializeField] private ScriptableItem _itemToCreate;
	[SerializeField] private float _secondsToFinish;

	[SerializeField] private List<ScriptableItem> _listOfResourses;
	[SerializeField] private List<long> _amountOfResourses;

	public ScriptableItem ItemToCreate => _itemToCreate;
    public float SecondsToFinish => _secondsToFinish;
	public List<ScriptableItem> ListOfResourses => new List<ScriptableItem>(_listOfResourses);
	public List<long> AmountsOfResourses => new List<long>(_amountOfResourses);


	public bool IsAnoughResourses (Inventory factoryStorage) {
		if (_listOfResourses.Count != _amountOfResourses.Count)
			return false;
		
		if (_listOfResourses.Count == 0) 
			return true;
		
		for (int i=0; i<_listOfResourses.Count; i++) {
			long amount = 0;
			foreach (ScriptableItem item in factoryStorage.GetAllItems(_listOfResourses[i].ItemName)) 
				if (item.IsTheSameItem(_listOfResourses[i]))
					amount+=item.Amount;
				
			if (amount < _amountOfResourses[i])
				return false;
		}
		
		return true;
	}
	
	public override bool IsTheSameItem(ScriptableItem itemToCompare) 
    {
        if (!base.IsTheSameItem(itemToCompare))
			return false;
		
		if (itemToCompare is Blueprint blueprintToCompare) 
		{
			if (this._itemToCreate.IsTheSameItem(blueprintToCompare.ItemToCreate))
				return true;
		}			
		return false;
    }

	public override string ToJson()
	{
		BlueprintJsonData jsonBlueprint = new();
		jsonBlueprint.itemName = _itemName;
		jsonBlueprint.itemToCreateJsonString = _itemToCreate.ToJson();
		jsonBlueprint.secondsToFinish = _secondsToFinish;

		for (int i = 0; i < _listOfResourses.Count; i++)
			jsonBlueprint.resourses.Add(_listOfResourses[i].ItemName);
		jsonBlueprint.amounts = _amountOfResourses;

		return JsonUtility.ToJson(jsonBlueprint);
	}
	public override void FromJson(string jsonString)
	{
		
		BlueprintJsonData jsonBlueprint = JsonUtility.FromJson<BlueprintJsonData>(jsonString);
		_itemName = jsonBlueprint.itemName;
		_secondsToFinish = jsonBlueprint.secondsToFinish;
		_amountOfResourses = jsonBlueprint.amounts;
		_listOfResourses.Clear();

		for (int i = 0; i < jsonBlueprint.resourses.Count; i++)
			_listOfResourses.Add(ScriptableItem.GetItem(jsonBlueprint.resourses[i]));
				
		if (this._itemName == "ErrorItem" || _itemToCreate==null || _listOfResourses.Count!=_amountOfResourses.Count)
        {
			Destroy(this);

        }
	}

	[Serializable]
	private class BlueprintJsonData
	{
		public string itemName = "ErrorItem";
		public string itemToCreateJsonString;
		public float secondsToFinish;
		public List<string> resourses = new();
		public List<long> amounts;
	}
}
