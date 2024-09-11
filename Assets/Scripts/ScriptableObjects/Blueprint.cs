using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Blueprint", menuName = "Items/Blueprint")]
public class Blueprint : Item
{
    [SerializeField] private Item _itemToCreate;
	[SerializeField] private float _secondsToFinish;

	[SerializeField] private List<Item> _listOfResourses;
	[SerializeField] private List<long> _amountOfResourses;

	public Item ItemToCreate => _itemToCreate;
    public float SecondsToFinish => _secondsToFinish;
	public List<Item> ListOfResourses => new List<Item>(_listOfResourses);
	public List<long> AmountsOfResourses => new List<long>(_amountOfResourses);


	public bool IsAnoughResourses (Inventory factoryStorage) {
		if (_listOfResourses.Count != _amountOfResourses.Count)
			return false;
		
		if (_listOfResourses.Count == 0) 
			return true;
		
		for (int i=0; i<_listOfResourses.Count; i++) {
			long amount = 0;
			foreach (Item item in factoryStorage.GetAllItems(_listOfResourses[i].ItemName)) 
				if (item.IsTheSameItem(_listOfResourses[i]))
					amount+=item.Amount;
				
			if (amount < _amountOfResourses[i])
				return false;
		}
		
		return true;
	}
	
	public override bool IsTheSameItem(Item itemToCompare) 
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

	/*public override string ToJson()
	{
		BlueprintJsonData jsonBlueprint = new();
		jsonBlueprint.itemToCreateJsonString = _itemToCreate.ToJson();
		jsonBlueprint.itemToCreateName=_itemToCreate.ItemName;
		jsonBlueprint.secondsToFinish = _secondsToFinish;

		for (int i = 0; i < _listOfResourses.Count; i++)
			jsonBlueprint.resourses.Add(_listOfResourses[i].ItemName);
		jsonBlueprint.amounts = _amountOfResourses;

		return JsonUtility.ToJson(jsonBlueprint);
	}

	public override void FromJson(string jsonString)
	{
		
		BlueprintJsonData jsonBlueprint = JsonUtility.FromJson<BlueprintJsonData>(jsonString);
		_secondsToFinish = jsonBlueprint.secondsToFinish;
		_amountOfResourses = jsonBlueprint.amounts;
		if (_listOfResourses==null)
			_listOfResourses=new(_amountOfResourses.Count);
		_listOfResourses.Clear();

		for (int i = 0; i < jsonBlueprint.resourses.Count; i++)
			_listOfResourses.Add(Item.GetItem(jsonBlueprint.resourses[i]));
		
		_itemToCreate=Item.GetItem(jsonBlueprint.itemToCreateName);
		_itemToCreate.FromJson(jsonBlueprint.itemToCreateJsonString);
				
		if (_itemToCreate==null || _listOfResourses.Count!=_amountOfResourses.Count)
        {
			Debug.Log ("Error! Mistake on FromJson() in Blueprint (item is destroying)");
			Destroy(this);
        }
	}

	[Serializable]
	private class BlueprintJsonData
	{
		public string itemToCreateJsonString;
		public string itemToCreateName;
		public float secondsToFinish;
		public List<string> resourses = new();
		public List<long> amounts;
	}*/
}
