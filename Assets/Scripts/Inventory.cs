using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public event Action<object, ScriptableItem, long> OnInventoryItemAddedEvent;
	public event Action<object, ScriptableItem, long> OnInventoryItemRemovedEvent;
	public event Action OnInventoryContentChanged;

	[SerializeField] private List<ScriptableItem> inventoryItems = new();

	public ScriptableItem GetItem(string itemName) {
		return inventoryItems.Find(item => item.ItemName == itemName);
	}

	public ScriptableItem[] GetAllItems() {
		return inventoryItems.ToArray();
	}

	public ScriptableItem[] GetAllItems(string itemName) {
		return inventoryItems.FindAll(item => item.ItemName == itemName).ToArray();
	}

	public ScriptableItem[] GetAllItems(ScriptableItem itemOfType)
	{
		return inventoryItems.FindAll(item => item.IsTheSameItem(itemOfType)).ToArray();
	}

	public long GetItemAmount(string itemName) {
		long amount = 0;
		foreach (ScriptableItem item in GetAllItems(itemName)) {
			amount += item.Amount;
		}
		return amount;
	}

	public void TransferTo(object sender, Inventory toInventory, ScriptableItem item, long amount = 1)
	{
		if (item == null || amount < 1 || item.Amount < amount || !inventoryItems.Contains(item))
			return;

		if (!item.Stackable || item.Amount == amount)
		{
			bool check = toInventory.TryToAdd(sender, item);
			if (check)
				inventoryItems.Remove(item);
		} else
		{
			ScriptableItem partOfStack = item.Split(amount);

			bool check = toInventory.TryToAdd(sender, partOfStack);
			if (!check)
				item.TryToUnite(partOfStack);
		}
		OnInventoryContentChanged?.Invoke();
	}

	public bool TryToAdd(object sender, ScriptableItem item) {
		if (!item.Stackable)
		{
			inventoryItems.Add(item);
		} else {
			var itemInInventory = inventoryItems.Find(existingItem => existingItem.IsTheSameItem(item));
			if (itemInInventory == null)
				inventoryItems.Add(item);
			else
				itemInInventory.TryToUnite(item);
		}
		OnInventoryItemAddedEvent?.Invoke(sender, item, item.Amount);
		OnInventoryContentChanged?.Invoke();
		return true;
	}

	public void Remove(object sender, string itemName, long amount = 1) => RemoveByArray(sender, GetAllItems(itemName), amount);
	public void Remove(object sender, ScriptableItem itemOfType, long amount = 1) => RemoveByArray(sender, GetAllItems(itemOfType), amount);
	public void RemoveThisItem(object sender, ScriptableItem item) => RemoveByArray(sender, new ScriptableItem[] { item }, item.Amount);

	private void RemoveByArray(object sender, ScriptableItem[] items, long amount) {
		if (items.Length == 0)
			return;

		if (!items[0].Stackable)
		{
			inventoryItems.Remove(items[0]);
			OnInventoryContentChanged?.Invoke();
			return;
		}
		else if (amount < 0)
			return;


		int count = items.Length;
		for (int i = count - 1; i >= 0; i--)
		{
			if (items[i].Amount >= amount)
			{
				items[i].Amount -= amount;
				if (items[i].Amount <= 0)
					inventoryItems.Remove(items[i]);
				OnInventoryItemRemovedEvent?.Invoke(sender, items[i], amount);
				break;
			}

			amount -= items[i].Amount;
			inventoryItems.Remove(items[i]);
			OnInventoryItemRemovedEvent?.Invoke(sender, items[i], items[i].Amount);
			OnInventoryContentChanged?.Invoke();
		}
	}

	public void ClearInventory(object sender) {
        if (OnInventoryItemRemovedEvent != null)
            foreach (var item in inventoryItems)
                OnInventoryItemRemovedEvent.Invoke(sender, item, item.Amount);
        inventoryItems.Clear();
        OnInventoryContentChanged?.Invoke();
    } 
	
	public bool HasItem (string itemName, out ScriptableItem item) {
		item = GetItem(itemName);
		return item != null;
	}
	public bool Contains(ScriptableItem item) => inventoryItems.Contains(item);

	public void FromJson(string jsonString)
	{
		inventoryItems.Clear();
		InventoryJsonData jsonInventory = JsonUtility.FromJson<InventoryJsonData>(jsonString);
		if (jsonInventory == null)
			return;
		ScriptableItem itemToAdd;
		for (int i=0; i<jsonInventory.inventory.Count; i++)
        {
			itemToAdd = (ScriptableItem)ScriptableObject.CreateInstance(Type.GetType(jsonInventory.inventory[i++]));
			if (itemToAdd == null)
				continue;
			itemToAdd.FromJson(jsonInventory.inventory[i]);
			if (itemToAdd!=null)
				inventoryItems.Add(itemToAdd); //TODO Make test if deserialization error
        }
	}

	public string ToJson()
	{
		InventoryJsonData jsonInventory = new();
		for (int i=0; i<inventoryItems.Count; i++)
        {
			jsonInventory.inventory.Add(inventoryItems[i].GetType().Name);
			jsonInventory.inventory.Add(inventoryItems[i].ToJson());
        }
		
		return JsonUtility.ToJson(jsonInventory);

	}

	[Serializable]
	protected class InventoryJsonData
	{
		public List<string> inventory=new();
	}
}
