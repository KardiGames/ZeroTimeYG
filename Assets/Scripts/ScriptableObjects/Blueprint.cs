using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Blueprint", menuName = "Items/Bluepring")]
public class Blueprint : ScriptableItem
{
    public new long Amount => 1;
    public new bool Stackable => false;
    
    private ScriptableItem _itemToCreate;
    private float _secondsToFinish; 
	
	private List<ScriptableItem> _listOfResourses;
	private List<long> _amountOfResourses;

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
			if (this._itemToCreate.IsTheSameItem(blueprintToCompare))
				return true;
		}			
		return false;
    }
}
