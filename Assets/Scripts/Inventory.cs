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
	
	public ScriptableItem[] GetAllItems () {
		return inventoryItems.ToArray();	
	}
	
	
	public ScriptableItem[] GetAllItems(string itemName) {
		return inventoryItems.FindAll(item => item.name == itemName).ToArray();
	}
	
	public long  GetItemAmount (string itemName) {
		long amount =0;
		foreach (ScriptableItem item in GetAllItems(itemName)) {
			amount+= item.Amount;
		}
		return amount;
	}

	public void TransferTo(object sender, Inventory toInventory, ScriptableItem item, long amount = 1)
    {
		if (amount < 1 || item.Amount<amount || !inventoryItems.Contains(item))
			return;
		
		if (!item.Stackable || item.Amount==amount)
        {
			bool check = toInventory.TryToAdd(sender, item);
			if (check)
				inventoryItems.Remove(item);
        } else
        {
			ScriptableItem partOfStack = Instantiate(item);
			partOfStack.Amount = amount;

			bool check = toInventory.TryToAdd(sender, partOfStack);
			if (check)
				item.Amount -= amount;
        }
		OnInventoryContentChanged?.Invoke();
	}
	
	public bool TryToAdd (object sender, ScriptableItem item) {
		if (!item.Stackable)
        {
			inventoryItems.Add(item); //TODO think about adding item.Clone()
			OnInventoryItemAddedEvent?.Invoke(sender, item, item.Amount);
			OnInventoryContentChanged?.Invoke();
			return true;
        }

		var itemInInventory = inventoryItems.Find(existingItem => existingItem.ItemName == item.ItemName);
		if (itemInInventory == null)
			inventoryItems.Add(item); //TODO think about adding item.Clone()
		else
			itemInInventory.Amount += item.Amount;

		OnInventoryItemAddedEvent?.Invoke(sender, item, item.Amount);
		OnInventoryContentChanged?.Invoke();
		return true;
	}
	
	public void Remove (object sender, string itemName, long amount=1) {
		var foundItems = GetAllItems(itemName);
		if (foundItems.Length == 0)
			return;

		if (!foundItems[0].Stackable)
		{
			inventoryItems.Remove(foundItems[0]);
			OnInventoryContentChanged?.Invoke();
			return;
		}
		else if (amount < 0)
			return;


		int count = foundItems.Length;
		for (int i=count-1; i>=0; i--)
        {
			if (foundItems[i].Amount>=amount)
            {
				foundItems[i].Amount -= amount;
				if (foundItems[i].Amount <= 0)
					inventoryItems.Remove(foundItems[i]);
				OnInventoryItemRemovedEvent?.Invoke(sender, foundItems[i], amount);
				break;
            }

			amount -= foundItems[i].Amount;
			inventoryItems.Remove(foundItems[i]);
			OnInventoryItemRemovedEvent?.Invoke(sender, foundItems[i], foundItems[i].Amount);
			OnInventoryContentChanged?.Invoke();
		}
	}
	
	public bool HasItem (string itemName, out ScriptableItem item) {
		item = GetItem(itemName);
		return item != null;
	}
	
}
