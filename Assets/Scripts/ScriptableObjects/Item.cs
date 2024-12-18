using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    private static List<Item> _allItems=new();
    public static Item[] AllItems => _allItems.ToArray(); //TODO detele it after. exists just for tests
	
	[SerializeField] protected string _itemName = "ErrorItem";
    [SerializeField] private bool _stackable=false;
    [SerializeField] private long _amount = 1;
    [SerializeField] private Sprite _icon;

    public string ItemName => _itemName;
    public virtual bool Stackable { get => _stackable; }
    public virtual long Amount
    {
        get => _amount;
        set //TODO m.b.delete setter
        {
            if (Stackable && value >= 0)
                _amount = value;
            else
                Debug.Log("Trying to set negative value (" + value + ") to Item " + ItemName+". Or change value of NotStackable item");
        }
    }
    public virtual int AC => 0;
    public virtual Sprite Icon => _icon;
    public virtual Color IconColor => Color.white;


    public static void LoadItems() {
		foreach (object objectToLoad in Resources.LoadAll("", Type.GetType("Item", true, false)))
			if (objectToLoad is Item itemToLoad)
				if (_allItems.Find(item => item.ItemName == itemToLoad.ItemName) == null)
					_allItems.Add(itemToLoad);
				else
					Debug.Log("Error! Have tryed to load items with same name "+ itemToLoad.ItemName);
    }
	
	public Item Clone()
    {
        return Instantiate(this);
    }

    private Item Clone (long newAmount)
    {
        long oldAmount = _amount;
        _amount = newAmount;
        Item copiedItem = Instantiate(this);
        _amount = oldAmount;
        return copiedItem;
    }
        
    public Item Split (long amountToGet)
    {
        if (_amount <= amountToGet)
            return null;

        //TODO mb. if _amount==amountToGet do Destroy(this) ?? Check logic

        _amount -= amountToGet;
        return Clone(amountToGet);
    }
	
    public bool TryToUnite (Item itemToAdd)
    {
        if (!IsTheSameItem(itemToAdd))
            return false;

        if (!Stackable)
            return false;

        _amount += itemToAdd.Amount;
        Destroy(itemToAdd);

        return (itemToAdd==null);
    }

    public virtual bool IsTheSameItem(Item itemToCompare)
    {
        return this._itemName == itemToCompare._itemName && this.GetType()==itemToCompare.GetType();
        //TODO make test if the same name but diff types
    }

    public virtual string ToJson ()
    {
        ItemJsonData itemJson = new ItemJsonData()
        {
            a = Amount,
        };
		return JsonUtility.ToJson(itemJson);
    }

    public virtual void FromJson (string jsonString)
    {
        ItemJsonData jsonItem = JsonUtility.FromJson<ItemJsonData>(jsonString);
        if (jsonItem == null){
			Debug.Log ("Error! Mistake on FromJson() in SrcItem (item is destroying)");
            Destroy(this);
		}
		_amount=jsonItem.a;
    }

    public static Item GetItem(string itemName)
    {
        return _allItems.Find (item => item.ItemName == itemName).Clone();
    }

    public static Item[] GetAllItems()
    {
        return _allItems.Select(item => item.Clone()).ToArray();
    }
	
	[Serializable]
	protected class ItemJsonData {
		public long a;
	}
}
